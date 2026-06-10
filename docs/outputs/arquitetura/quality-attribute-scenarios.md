# Quality Attribute Scenarios — Folha360

## Summary
Cenários concretos e mensuráveis de atributos de qualidade para guiar decisões arquiteturais e validar a implementação. Cada cenário segue o formato: **Atributo | Fonte/Estímulo | Ambiente | Artefato | Resposta | Medida | Prioridade**.

---

## Performance

### Cenário P1: Processamento da Folha em Larga Escala
- **Attribute**: Performance (throughput)
- **Source/stimulus**: Admin RH inicia processamento da folha mensal para 100.000 funcionários
- **Environment**: Sistema em operação normal, ambiente de produção
- **Artifact**: Módulo de Cálculo da Folha (`api-folha`)
- **Response**: Sistema processa todos os funcionários e gera holerites
- **Response measure**: Tempo total < 2 horas; throughput > 50 funcionários/segundo
- **Priority**: Crítica

### Cenário P2: Consulta de Holerite Individual
- **Attribute**: Performance (latency)
- **Source/stimulus**: Funcionário ou admin consulta holerite de um mês específico
- **Environment**: Sistema em operação normal, horário comercial
- **Artifact**: API de Relatórios (`api-relatorios`) → PostgreSQL read replica
- **Response**: Sistema retorna o holerite com todos os detalhes de rubricas
- **Response measure**: Tempo de resposta < 2 segundos (p95)
- **Priority**: Alta

### Cenário P3: Geração de Relatório Anual (DIRF)
- **Attribute**: Performance (throughput)
- **Source/stimulus**: Contador solicita DIRF anual para 100.000 funcionários
- **Environment**: Final de ano fiscal, pico de uso
- **Artifact**: Módulo de Relatórios
- **Response**: Sistema gera arquivo CSV/PDF completo
- **Response measure**: Tempo total < 10 minutos; sem impactar sistema transacional
- **Priority**: Média

---

## Disponibilidade (Availability)

### Cenário A1: Indisponibilidade do Banco Primário
- **Attribute**: Availability
- **Source/stimulus**: PostgreSQL Primary sofre falha crítica (hardware/software)
- **Environment**: Produção, horário de pico (fechamento da folha)
- **Artifact**: PostgreSQL (Primary + Replica + Patroni)
- **Response**: Failover automático para réplica; sistema retoma operação
- **Response measure**: RTO < 5 minutos; RPO < 1 hora (último backup)
- **Priority**: Crítica

### Cenário A2: Indisponibilidade do e-Social gov.br
- **Attribute**: Availability (graceful degradation)
- **Source/stimulus**: Portal e-Social fica indisponível por 4 horas
- **Environment**: Durante janela de envio de eventos
- **Artifact**: Módulo de Integração e-Social (`api-esocial`)
- **Response**: Sistema continua operando; eventos acumulam na fila RMQ; envio retoma automaticamente quando portal volta
- **Response measure**: Nenhum evento perdido; envio concluído em até 2h após restabelecimento
- **Priority**: Alta

### Cenário A3: Pico de Acesso no Fechamento
- **Attribute**: Availability (elasticity)
- **Source/stimulus**: 50 usuários simultâneos iniciam operações de fechamento no mesmo dia
- **Environment**: Dia 25-30 do mês (fechamento)
- **Artifact**: APIs de Cálculo e Relatórios
- **Response**: Sistema escala automaticamente (HPA) para atender a demanda
- **Response measure**: Nenhuma requisição com erro 5xx; latência p95 < 5s
- **Priority**: Alta

---

## Segurança (Security)

### Cenário S1: Acesso Não Autorizado a Dados Sensíveis
- **Attribute**: Security (confidentiality)
- **Source/stimulus**: Atacante interno tenta acessar salários de outros funcionários via API
- **Environment**: Produção
- **Artifact**: Auth Gateway (JWT + políticas de autorização)
- **Response**: Sistema rejeita requisição com 403 Forbidden; registra tentativa em audit log
- **Response measure**: 100% das requisições não autorizadas são bloqueadas; tentativas alertam em < 1 min
- **Priority**: Crítica

### Cenário S2: Criptografia de Dados em Repouso
- **Attribute**: Security (data-at-rest)
- **Source/stimulus**: Disco do servidor de banco de dados é roubado ou acessado fisicamente
- **Environment**: Produção
- **Artifact**: PostgreSQL (criptografia AES-256) + CryptoService
- **Response**: Dados sensíveis (CPF, CTPS, salários) estão ilegíveis sem chave
- **Response measure**: Nenhum dado sensível legível; chave de criptografia armazenada em HSM/KMS separado
- **Priority**: Crítica

### Cenário S3: Conformidade LGPD — Exclusão de Dados
- **Attribute**: Security (compliance)
- **Source/stimulus**: Ex-funcionário solicita exclusão de seus dados pessoais (art. 18 LGPD)
- **Environment**: Produção
- **Artifact**: Módulo de Cadastros + Audit Log
- **Response**: Sistema anonimiza ou exclui dados pessoais; mantém apenas dados obrigatórios por lei (ex.: retenção fiscal por 5 anos)
- **Response measure**: Exclusão concluída em < 7 dias; registro de exclusão em audit log imutável
- **Priority**: Alta

---

## Escalabilidade (Scalability)

### Cenário SC1: Crescimento de Funcionários
- **Attribute**: Scalability
- **Source/stimulus**: Empresa cresce de 10.000 para 100.000 funcionários em 2 anos
- **Environment**: Produção, sem redesign arquitetural
- **Artifact**: Todos os módulos
- **Response**: Sistema mantém SLAs de performance e disponibilidade
- **Response measure**: Tempo de processamento da folha cresce linearmente (não exponencial); escala horizontal com + réplicas
- **Priority**: Alta

### Cenário SC2: Múltiplas Empresas (Multi-Tenant)
- **Attribute**: Scalability (multi-tenancy)
- **Source/stimulus**: Adição de 10 novas empresas (matriz + filiais) ao sistema
- **Environment**: Produção
- **Artifact**: PostgreSQL (schema por empresa ou tenant_id)
- **Response**: Sistema isola dados de cada empresa; fechamento pode ser paralelizado por empresa
- **Response measure**: Tempo de fechamento de 10 empresas < 4 horas (paralelo); isolamento total de dados entre tenants
- **Priority**: Média

---

## Manutenibilidade (Maintainability)

### Cenário M1: Alteração de Regra Fiscal (Nova Tabela IRRF)
- **Attribute**: Maintainability (modifiability)
- **Source/stimulus**: Receita Federal publica nova tabela progressiva de IRRF
- **Environment**: Desenvolvimento → Homologação → Produção
- **Artifact**: Módulo de Cálculo da Folha (estratégia de cálculo) + Tabela `rubrica_tabela_progressiva`
- **Response**: Equipe atualiza tabela e implanta em produção
- **Response measure**: Tempo para atualizar e implantar < 4 horas; sem alteração de código (tabela parametrizada e versionada por ano); nova versão da tabela coexiste com anterior até virada do ano
- **Priority**: Alta

### Cenário M2: Novo Layout e-Social (Nova Nota Técnica)
- **Attribute**: Maintainability (evolvability)
- **Source/stimulus**: Governo publica nova versão de layout e-Social (ex.: S-1.4)
- **Environment**: Desenvolvimento
- **Artifact**: Módulo de Integração e-Social (schemas XSD + validação)
- **Response**: Equipe atualiza schemas XSD e regras de validação
- **Response measure**: Tempo para compatibilidade < 2 semanas; schemas versionados; testes de regressão automatizados
- **Priority**: Alta

### Cenário M3: Customização de Rubrica por Convenção Coletiva
- **Attribute**: Maintainability (flexibility)
- **Source/stimulus**: Empresa precisa criar rubrica específica de convenção coletiva (ex.: "Adicional de Quebra de Caixa" com fórmula `{SALARIO_BASE} * 0.20` limitado a teto de R$ 500)
- **Environment**: Produção, interface de cadastro de rubricas
- **Artifact**: Módulo de Cadastros (subsistema de rubricas) + Editor de fórmulas
- **Response**: Usuário cria rubrica com fórmula, teto e composição sem necessidade de deploy ou alteração de código
- **Response measure**: Tempo para criar e testar rubrica < 15 minutos; fórmula validada em sandbox; simulação disponível imediatamente
- **Priority**: Média

---

## Usabilidade (Usability)

### Cenário U1: Operador Processa Folha com Feedback
- **Attribute**: Usability
- **Source/stimulus**: Operador de RH inicia processamento da folha e quer acompanhar progresso
- **Environment**: Produção, interface web React
- **Artifact**: Dashboard de processamento (React + SignalR/polling)
- **Response**: Interface mostra barra de progresso, funcionários processados, tempo estimado
- **Response measure**: Progresso visível em tempo real (< 5s de atraso na atualização); ETA com precisão de ±15%
- **Priority**: Média

---

## Matriz de Prioridades

| ID | Cenário | Atributo | Prioridade |
|---|---|---|---|
| P1 | Processamento 100K func. < 2h | Performance | **Crítica** |
| A1 | Failover PostgreSQL < 5min | Availability | **Crítica** |
| S1 | Bloqueio acesso não autorizado | Security | **Crítica** |
| S2 | Criptografia dados em repouso | Security | **Crítica** |
| P2 | Consulta holerite < 2s | Performance | Alta |
| A2 | Resiliência e-Social indisponível | Availability | Alta |
| A3 | Escala no fechamento | Availability | Alta |
| S3 | Exclusão LGPD < 7 dias | Security | Alta |
| SC1 | Crescimento 10K → 100K func. | Scalability | Alta |
| M1 | Alteração tabela IRRF < 4h | Maintainability | Alta |
| M2 | Novo layout e-Social < 2 sem | Maintainability | Alta |
| M3 | Customização rubrica < 15min | Maintainability | Média |
| P3 | DIRF anual < 10 min | Performance | Média |
| SC2 | Multi-tenant 10 empresas | Scalability | Média |
| U1 | Dashboard de progresso | Usability | Média |

## Evidence vs Assumptions

**Evidências**:
- LGPD exige exclusão de dados pessoais (art. 18)
- e-Social publica Notas Técnicas periódicas com novos layouts
- Receita Federal publica tabelas de IRRF anualmente

**Assumptions**:
- 100.000 funcionários é o teto esperado nos próximos 5 anos
- 2 horas é o SLA aceitável para processamento da folha
- Equipe de 3-5 desenvolvedores por módulo

## Recommended Next Skill
`architecture-risk-assessor` — para identificar e classificar riscos arquiteturais baseados nestes cenários.
