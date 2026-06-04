# Runtime View — Folha360

## Summary
Descrição dos principais fluxos de execução do Folha360, incluindo interações síncronas e assíncronas, componentes envolvidos em tempo de execução, pontos de falha e estratégias de recuperação. Foco nos dois fluxos mais críticos: **Processamento da Folha Mensal** e **Envio de Eventos ao e-Social**.

---

## Fluxo 1: Processamento da Folha Mensal

### Sequência Principal

```mermaid
sequenceDiagram
    actor Admin as Admin RH
    participant API as api-folha
    participant Cache as Redis
    participant CAD as api-cadastros
    participant EVT as api-eventos
    participant DB as PostgreSQL
    participant RMQ as RabbitMQ
    participant FIS as api-fiscais

    Admin->>API: POST /api/folha/processar {periodo, empresaId}
    API->>DB: Verifica se período já foi processado (idempotência)
    alt Período já processado
        API-->>Admin: 409 Conflict (já processado)
    else Novo processamento
        API->>DB: Cria ProcessamentoFolha (status=INICIADO)
        API-->>Admin: 202 Accepted {processamentoId}

        Note over API: Processamento em background (Task Parallel)

        par Para cada funcionário (em lotes de 1000)
            API->>Cache: Busca tabelas progressivas (IRRF, INSS)
            API->>CAD: GET /api/funcionarios/{id}/dados-contratuais
            API->>EVT: GET /api/eventos?funcionarioId={id}&periodo={mes}
            API->>Cache: Busca rubricas vigentes
            API->>API: Calcula vencimentos, descontos, líquido
            API->>DB: Insere FolhaMensal (batch de 1000)
        end

        API->>DB: Atualiza ProcessamentoFolha (status=CONCLUIDO)
        API->>RMQ: Publica FolhaFechada {periodo, empresaId, ...}
        RMQ->>FIS: Consome FolhaFechada (dispara apuração fiscal)
    end
```

### Elementos em Tempo de Execução

| Elemento | Papel no Fluxo | Escala |
|---|---|---|
| **api-folha** | Orquestrador do cálculo; processamento paralelo | 3+ réplicas; cada uma processa N lotes |
| **Redis** | Cache de tabelas progressivas (IRRF, INSS) e rubricas | Hit rate esperado > 95% |
| **api-cadastros** | Fornece dados contratuais dos funcionários | Consultas em cache local |
| **api-eventos** | Fornece eventos trabalhistas do período | Consultas em cache local |
| **PostgreSQL** | Persiste folhas calculadas e status do processamento | Batch insert otimizado |
| **RabbitMQ** | Notifica conclusão para módulo fiscal | 1 mensagem por empresa/período |

### Interações Síncronas
- Admin → api-folha: `POST /api/folha/processar` (resposta imediata com 202)
- api-folha → api-cadastros: consulta de dados contratuais (HTTP)
- api-folha → api-eventos: consulta de eventos do período (HTTP)
- api-folha → Redis: leitura de tabelas/rubricas

### Interações Assíncronas
- api-folha → RabbitMQ → api-fiscais: evento `FolhaFechada`

### Pontos de Falha

| Ponto | Falha | Impacto | Recuperação |
|---|---|---|---|
| **api-cadastros indisponível** | Não consegue ler dados de funcionários | Cálculo não inicia para funcionários órfãos | Retry 3x com backoff; marca como `PENDENTE_CADASTRO`; reprocessa após recuperação |
| **Redis indisponível** | Cache miss → consulta PostgreSQL | Degradação de performance (3-5x mais lento) | Fallback para PostgreSQL; recalcula tabelas em memória |
| **Timeout no batch insert** | Lote de 1000 registros falha | Perda parcial do processamento | Rollback do lote; retry com lote menor (500); marca lote como `ERRO` |
| **Estouro de memória** | 100K funcionários carregados simultaneamente | OOM kill no container | Streaming/batch processing; limite de 1000 funcionários por vez |
| **Processamento excede 2h** | SLA não atendido | Atraso no fechamento e envio e-Social | Alerta em 1h30; escala horizontal (HPA); particionar por empresa |

---

## Fluxo 2: Envio de Eventos ao e-Social

### Sequência Principal

```mermaid
sequenceDiagram
    participant FOL as api-folha
    participant FIS as api-fiscais
    participant RMQ as RabbitMQ
    participant ES as api-esocial
    participant DB as PostgreSQL
    participant GOV as e-Social gov.br

    FOL->>RMQ: EventoRemuneracaoGerado (S-1200, XML)
    FIS->>RMQ: EventoFiscalGerado (S-5001, XML)

    RMQ->>ES: Consome eventos

    loop Para cada evento recebido
        ES->>ES: Valida XML contra XSD (schema S-1.3)
        alt XML invalido
            ES->>DB: Marca evento como ERRO_VALIDACAO
            ES->>RMQ: Publica EventoComErro
        else XML valido
            ES->>DB: Insere na tabela LoteESocial
        end
    end

    Note over ES: Agrupamento a cada 5 min ou 100 eventos

    ES->>DB: Busca eventos PENDENTES para o lote
    ES->>ES: Assina XML do lote com certificado A1
    ES->>GOV: POST /ws/enviarLote
    alt Sucesso
        GOV-->>ES: 200 OK protocolo
        ES->>DB: Atualiza LoteESocial status ENVIADO
        ES->>ES: Agenda consulta de recibo
    else Erro gov.br
        GOV-->>ES: 500 / timeout
        ES->>DB: Atualiza LoteESocial status ERRO_ENVIO
        ES->>ES: Agenda retry com backoff
    end

    Note over ES: Consulta de recibo

    ES->>GOV: GET /ws/consultarRecibo
    alt Processado com sucesso
        GOV-->>ES: Recibo
        ES->>DB: Atualiza eventos como PROCESSADO
        ES->>RMQ: Publica LoteProcessado
    else Processado com erro
        GOV-->>ES: Recibo com erros
        ES->>DB: Atualiza eventos como PROCESSADO_COM_ERRO
        ES->>RMQ: Publica EventoComErro
    else Ainda processando
        GOV-->>ES: 202 em processamento
        ES->>ES: Reagenda consulta em 5 min
    end
```

### Elementos em Tempo de Execução

| Elemento | Papel no Fluxo | Escala |
|---|---|---|
| **api-esocial** | Validação, assinatura, envio, consulta de recibos | 2+ réplicas; fila consumida com concurrency |
| **RabbitMQ** | Fila de eventos a enviar; fila de retry; dead-letter | Cluster 2+ nós; mensagens persistentes |
| **PostgreSQL** | Armazena lotes, status, recibos | Primary para escrita |
| **e-Social gov.br** | Endpoint externo do governo | Fora do nosso controle |

### Interações Síncronas
- api-esocial → e-Social gov.br: HTTPS POST (envio) e GET (consulta)

### Interações Assíncronas
- api-folha → RabbitMQ → api-esocial: eventos de remuneração
- api-fiscais → RabbitMQ → api-esocial: eventos fiscais
- api-esocial → RabbitMQ → api-folha/api-fiscais: status de processamento

### Pontos de Falha

| Ponto | Falha | Impacto | Recuperação |
|---|---|---|---|
| **e-Social indisponível** | Portal gov.br fora do ar | Eventos acumulam na fila; risco de atraso legal | Retry com backoff exponencial (até 24h); dead-letter queue; alerta operacional |
| **Certificado expirado** | TLS handshake falha | Nenhum evento é enviado | Alerta 30 dias antes; procedimento de renovação documentado |
| **Schema XSD rejeitado** | Layout desatualizado | Lote inteiro rejeitado | Validação pré-envio; CI/CD monitora portal e-Social; rollback de schema |
| **Perda de mensagens RMQ** | RabbitMQ crash sem persistência | Eventos perdidos | Mensagens persistentes; publisher confirm; reconciliação batch diária |
| **Lote rejeitado por erro em 1 evento** | Evento inválido no lote | Lote inteiro rejeitado | Isolar evento com erro; reenviar lote sem ele; corrigir e reenviar evento isolado |

---

## Fluxo 3: Admissão de Funcionário (Cadeia Completa)

```mermaid
sequenceDiagram
    actor Admin as Admin RH
    participant CAD as api-cadastros
    participant EVT as api-eventos
    participant RMQ as RabbitMQ
    participant ES as api-esocial
    participant GOV as e-Social gov.br

    Admin->>CAD: POST /api/funcionarios (dados completos)
    CAD->>CAD: Valida CPF, CTPS, PIS
    CAD->>CAD: Criptografa documentos sensíveis
    CAD->>CAD: Persiste funcionário
    CAD->>RMQ: FuncionarioCadastrado {id, empresaId, dataAdmissao}
    CAD-->>Admin: 201 Created {funcionarioId}

    RMQ->>EVT: Consome FuncionarioCadastrado
    EVT->>EVT: Cria EventoTrabalhista (tipo=ADMISSAO)
    EVT->>EVT: Gera XML S-2200
    EVT->>RMQ: EventoTrabalhistaGerado {tipo: S-2200, xml, funcionarioId}

    RMQ->>ES: Consome EventoTrabalhistaGerado
    ES->>ES: Valida XML S-2200 contra XSD
    ES->>GOV: Envia evento S-2200
    GOV-->>ES: Recibo
    ES->>RMQ: LoteProcessado {protocolo, status=OK}
```

---

## Failure Points Consolidados

| # | Failure Point | Fluxo | Severidade | Recovery Strategy |
|---|---|---|---|---|
| FP1 | api-cadastros indisponível | Folha, Admissão | Alta | Circuit breaker; cache local; retry |
| FP2 | e-Social gov.br indisponível | Envio e-Social | Crítica | Retry 24h; dead-letter; alerta |
| FP3 | Certificado A1 expirado | Envio e-Social | Crítica | Monitor proativo; renovação |
| FP4 | Timeout cálculo > 2h | Folha | Média | HPA; particionamento; alerta |
| FP5 | Inconsistência de dados entre módulos | Todos | Média | Reconciliação batch; eventos de domínio |
| FP6 | RabbitMQ crash | Todos (assíncrono) | Alta | Cluster; persistent messages; replay |

## Evidence vs Assumptions

**Evidências**:
- e-Social define fluxos S-2200 (admissão), S-1200 (remuneração), S-5001 (fiscais)
- Processamento de folha é batch por natureza (mensal)

**Assumptions**:
- Volume de 100K funcionários processados em lotes de 1000 é viável
- RabbitMQ suporta o throughput de eventos (estimado: ~200K eventos/mês)
- e-Social responde em < 30s por lote

## Recommended Next Skill
`quality-attribute-scenario-writer` — para definir cenários concretos de qualidade (performance, disponibilidade, segurança).
