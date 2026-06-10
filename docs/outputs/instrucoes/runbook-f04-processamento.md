# Runbook F04 — Processamento da Folha

## Visão Geral

Este runbook documenta os procedimentos operacionais para o módulo de Processamento da Folha (F04) do sistema Folha360.

## Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/folha/processar` | Iniciar processamento |
| GET | `/api/folha/processamento/{id}` | Status do processamento |
| POST | `/api/folha/processamento/{id}/cancelar` | Cancelar processamento |
| POST | `/api/folha/reprocessar` | Reprocessar período |
| GET | `/api/folha/processamento/{id}/itens` | Itens da folha |
| POST | `/api/folha/{id}/reabrir` | Reabrir folha fechada |
| GET | `/api/folha/{id}/reabertura/status` | Status da reabertura |
| GET | `/api/folha/{empresaId}/{periodo}/historico` | Histórico de versões |
| GET | `/api/folha/fechamento/{empresaId}/{periodo}` | Status da cadeia de fechamento |
| GET | `/api/folha/holerites/{processamentoId}` | Listar holerites |
| GET | `/api/folha/holerites/{processamentoId}/{funcionarioId}` | Download holerite PDF |

## Cenários Operacionais

### 1. Como Processar uma Folha Mensal

```bash
curl -X POST http://localhost:8080/api/folha/processar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "empresaId": "uuid-da-empresa",
    "periodo": "2026-06",
    "tipoCalculo": "Mensal"
  }'
```

Resposta: `202 Accepted` com `{ "processamentoId": "uuid" }`

Acompanhar via SignalR: conectar ao hub `/hubs/processamento?empresaId={uuid}`

### 2. Como Reprocessar uma Folha

```bash
curl -X POST http://localhost:8080/api/folha/reprocessar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{ "processamentoId": "uuid-do-processamento-original" }'
```

O sistema fará soft delete do processamento original e criará uma nova versão.

### 3. Como Reabrir uma Folha Fechada

```bash
curl -X POST http://localhost:8080/api/folha/{processamentoId}/reabrir \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "motivo": "Rubrica de horas extras com fórmula incorreta — necessário corrigir base de cálculo",
    "autor": "uuid-do-operador"
  }'
```

**Regras**:
- Motivo deve ter no mínimo 20 caracteres
- Apenas perfis Operador e Admin podem reabrir
- Prazo: até 60 dias após fechamento para Operador; após, apenas Admin com justificativa de 50+ caracteres
- Folha já reaberta não pode ser reaberta novamente (409 Conflict)

### 4. Como Diagnosticar Falhas de Cálculo

1. Verificar status: `GET /api/folha/processamento/{id}`
2. Se status = `Falho`, verificar campo `erro`
3. Consultar itens com erro: `GET /api/folha/processamento/{id}/itens`
4. Verificar logs no Seq: filtrar por `ProcessamentoId`
5. Causas comuns:
   - Rubrica sem base de cálculo configurada
   - Fórmula com erro de sintaxe
   - Tabela progressiva não encontrada para o período
   - Ciclo detectado em composição de rubricas

### 5. Como Recuperar de Falha na Saga de Reabertura

Se a Saga de Reabertura entrar em timeout:

1. Verificar status: `GET /api/folha/{id}/reabertura/status`
2. Estados possíveis:
   - `AguardandoReversaoFiscais`: F05 não respondeu em 5 min
   - `AguardandoRetificacaoESocial`: F07 não respondeu em 30 min
   - `FalhaReversao`: timeout atingido
3. Ação manual:
   - Verificar se F05/F07 estão operacionais
   - Se necessário, reenviar comando de reversão manualmente
   - Reprocessar a folha após correção

### 6. Como Escalar (Aumentar Performance)

- Aumentar batches: ajustar `SemaphoreSlim` de 1.000 para 2.000
- Aumentar workers MassTransit: configurar `ConcurrentConsumerLimit`
- Escalar horizontalmente: adicionar mais instâncias do worker
- Verificar métricas Prometheus:
  - `folha360_processamento_duracao_segundos`
  - `folha360_funcionarios_processados_total`
  - `folha360_cache_hit_ratio`

## Monitoramento

### Métricas Prometheus
- `folha360_processamento_duracao_segundos` (histograma por tipo_calculo)
- `folha360_funcionarios_processados_total` (counter)
- `folha360_erros_calculo_total` (counter por fase)
- `folha360_cache_hit_ratio` (gauge)

### Alertas Recomendados
- Tempo de processamento > 2h para 100K funcionários
- Taxa de erro > 5% dos funcionários
- Cache hit ratio < 80%
- Filas RabbitMQ com > 1000 mensagens pendentes

## Tabelas do Banco

| Tabela | Descrição |
|--------|-----------|
| `processamento_folha` | Cabeçalho do processamento |
| `item_folha` | Itens calculados por funcionário/rubrica |
| `holerite` | Metadados dos holerites gerados |
| `cadeia_fechamento` | Estado da cadeia de fechamento cross-módulo |

## Cache Redis

| Padrão de Chave | TTL |
|-----------------|-----|
| `cache:rubricas:{empresa_id}` | 1 hora |
| `cache:rubrica:{empresa_id}:{rubrica_id}` | 1 hora |
| `cache:tabela:irrf:{ano}` | 24 horas |
| `cache:tabela:inss:{ano}` | 24 horas |

Invalidação via pub/sub no canal `invalidate:rubricas`.
