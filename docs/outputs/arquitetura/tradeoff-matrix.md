# Tradeoff Matrix — Folha360

## Summary
Análise de tradeoffs das principais decisões arquiteturais do Folha360. Cada tradeoff avalia opções concorrentes contra critérios relevantes (performance, complexidade, custo, manutenibilidade, risco), mostrando o que se ganha e o que se perde em cada caminho.

---

## Tradeoff 1: Monólito Modular vs Microservices

### Contexto
O sistema tem 6 módulos de domínio (Cadastros, Eventos, Folha, Fiscais, Relatórios, e-Social). Precisamos decidir se implantamos como serviços independentes (microservices) ou como um monólito modular (deploy único, módulos isolados em código).

| Critério | Monólito Modular (escolhido) | Microservices |
|---|---|---|
| **Performance** | ✅ Comunicação em processo (baixa latência) | ❌ Latência de rede entre serviços; serialização |
| **Complexidade** | ✅ Deploy único; debugging mais simples | ❌ Orquestração complexa; distributed tracing obrigatório |
| **Custo infra** | ✅ Menos containers; menos rede; menos RAM | ❌ Cada serviço exige seu próprio container + reservas |
| **Escala independente** | ❌ Escala tudo junto (mais recursos) | ✅ Escala apenas o módulo sob carga (ex.: api-folha no fechamento) |
| **Isolamento de falha** | ❌ Crash derruba todos os módulos | ✅ Falha em um serviço não afeta os outros |
| **Manutenibilidade** | ✅ Código em um repositório; refatoração mais fácil | ❌ Contratos entre serviços precisam ser versionados |
| **Deploy independente** | ❌ Deploy é tudo ou nada | ✅ Cada serviço pode ter seu próprio ciclo de deploy |
| **Adequação ao time** | ✅ Time pequeno/médio (5-15 devs) | ❌ Exige times autônomos por serviço (3-5 devs cada) |

### Decisão: **Monólito Modular**
**Racional**: Para o estágio inicial do projeto, com time estimado de 5-15 desenvolvedores, o monólito modular oferece menor complexidade operacional com isolamento lógico entre módulos. A migração para microservices pode ser feita gradualmente (strangler fig pattern) quando o sistema crescer.

### O que observar (riscos):
- Se o módulo de Cálculo da Folha crescer muito, pode exigir escala independente → extrair primeiro
- Manter disciplina de não criar acoplamento entre módulos (respeitar bounded contexts)
- Preparar contratos como se fossem remotos (facilita extração futura)

---

## Tradeoff 2: RabbitMQ vs Kafka para Mensageria

### Contexto
O sistema depende de comunicação assíncrona entre módulos (eventos de domínio) e envio de eventos ao e-Social. Precisamos escolher o message broker.

| Critério | RabbitMQ (escolhido) | Kafka |
|---|---|---|
| **Curva de aprendizado** | ✅ Mais simples; AMQP padrão; boa doc | ❌ Conceitos complexos (partitions, consumer groups, offsets) |
| **Roteamento** | ✅ Exchanges, bindings, routing keys flexíveis | ❌ Roteamento limitado (tópicos); precisa de camada extra |
| **Garantia de entrega** | ✅ ACKs, publisher confirm, dead-letter | ✅ Log distribuído; replay de eventos |
| **Throughput** | ❌ ~50K msg/s por nó | ✅ ~1M+ msg/s |
| **Persistência de longo prazo** | ❌ Mensagens são removidas após consumo | ✅ Log imutável; retenção configurável (dias/semanas) |
| **Operação** | ✅ Mais simples de operar; cluster 3 nós | ❌ Exige ZooKeeper/KRaft; tuning complexo |
| **Caso de uso principal** | ✅ Filas de tarefas, RPC, eventos de domínio | ✅ Streaming, event sourcing, analytics |
| **Recursos** | ✅ Menos RAM/disco | ❌ Mais RAM (page cache); disco para logs |

### Decisão: **RabbitMQ**
**Racional**: O volume estimado de eventos do Folha360 (~200K eventos/mês) não justifica a complexidade do Kafka. RabbitMQ atende bem os padrões de fila de trabalho, retry com backoff, e dead-letter queue necessários para a integração com e-Social.

### O que observar (riscos):
- Se surgir necessidade de event sourcing ou analytics em tempo real → migrar para Kafka
- Configurar persistência de mensagens e cluster para alta disponibilidade
- Monitorar tamanho das filas (evitar overflow)

---

## Tradeoff 3: Redis vs In-Memory Cache para Tabelas Progressivas

### Contexto
O cálculo da folha consulta intensivamente tabelas progressivas (IRRF, INSS) e rubricas. Precisamos decidir onde armazenar esse cache.

| Critério | Redis (escolhido) | In-Memory (ConcurrentDictionary) |
|---|---|---|
| **Performance** | ✅ Sub-milissegundo (rede local) | ✅ Microssegundo (acesso direto) |
| **Compartilhamento** | ✅ Todas as réplicas compartilham cache | ❌ Cada réplica tem sua própria cópia (cold start) |
| **Invalidação** | ✅ Invalidação centralizada (pub/sub) | ❌ Invalidação por réplica (complexa) |
| **Persistência** | ✅ Sobrevive a restart do container | ❌ Perdido no restart (reconstrução) |
| **Operação** | ❌ +1 infra para gerenciar | ✅ Sem infra extra |
| **Memória** | ✅ Memória dedicada (não compete com app) | ❌ Compete com memória da aplicação (.NET GC) |

### Decisão: **Redis**
**Racional**: Embora in-memory seja mais rápido, Redis oferece compartilhamento entre réplicas (importante no fechamento com HPA), invalidação centralizada e persistência. O custo operacional de +1 container Redis é baixo.

### O que observar (riscos):
- Latência de rede: manter Redis no mesmo namespace K8s
- Configurar TTL adequado (tabelas mudam anualmente; rubricas mudam raramente)
- Fallback para PostgreSQL se Redis indisponível

---

## Tradeoff 4: Schema por Tenant vs Discriminator Column (Multi-Tenant)

### Contexto
O sistema precisa suportar múltiplas empresas (matriz/filiais). Precisamos decidir como isolar dados no PostgreSQL.

| Critério | Schema por Tenant (escolhido) | Discriminator Column (tenant_id) |
|---|---|---|
| **Isolamento** | ✅ Isolamento forte (schemas separados) | ❌ Isolamento fraco (depende de WHERE em toda query) |
| **Backup/Restore** | ✅ Backup por tenant; restore individual | ❌ Backup único; restore de 1 tenant é complexo |
| **Performance** | ✅ Índices menores (por tenant) | ❌ Índices grandes (todos tenants); requer tenant_id em todos |
| **Complexidade** | ❌ Migrations em N schemas; mais conexões | ✅ Migrations simples; conexão única |
| **Escala** | ✅ Mover tenant pesado para servidor separado | ❌ Difícil separar tenants |
| **LGPD** | ✅ Exclusão de dados por tenant é trivial (drop schema) | ❌ Exclusão requer DELETE com WHERE (mais lento) |
| **Número de tenants** | ❌ Viável até ~100 tenants | ✅ Suporta milhares de tenants |

### Decisão: **Schema por Tenant**
**Racional**: O número esperado de empresas é baixo (< 50), o que torna schema por tenant viável. O isolamento forte é importante para LGPD (exclusão de dados) e para backup/restore individual. A complexidade de migrations é mitigada com tooling (scripts de migração por schema).

### O que observar (riscos):
- Se número de empresas crescer para 100+, reavaliar
- Automatizar criação de schema para novas empresas
- Pool de conexões precisa ser dimensionado (N schemas × conexões)

---

## Tradeoff 5: Processamento Síncrono vs Assíncrono do Cálculo da Folha

### Contexto
O cálculo da folha para 100K funcionários pode levar até 2 horas. Precisamos decidir se a API espera o resultado (síncrono) ou retorna imediatamente e processa em background (assíncrono).

| Critério | Assíncrono (escolhido) | Síncrono |
|---|---|---|
| **Experiência do usuário** | ✅ Resposta imediata (202); progresso via polling/SignalR | ❌ Request bloqueado por até 2h; timeout provável |
| **Resiliência** | ✅ Retry automático em caso de falha | ❌ Falha = perder tudo; usuário precisa reenviar |
| **Complexidade** | ❌ Precisa de fila de jobs; notificação de conclusão | ✅ Implementação mais simples |
| **Observabilidade** | ✅ Progresso rastreável; métricas por etapa | ❌ Difícil saber o que aconteceu se der timeout |
| **Escala** | ✅ Múltiplos workers processam em paralelo | ❌ Uma requisição = um thread bloqueado |

### Decisão: **Assíncrono (202 Accepted + polling/SignalR)**
**Racional**: Para um processo que pode levar até 2 horas, a abordagem síncrona é inviável (timeouts HTTP, UX ruim). A abordagem assíncrona permite processamento em background, paralelismo, retry e feedback de progresso.

### O que observar (riscos):
- Implementar notificação de conclusão (SignalR ou webhook)
- Garantir idempotência (não processar mesma folha duas vezes)
- Timeout máximo do job (configurar para 3h; alertar se exceder)

---

## Matriz Consolidada de Decisões

| # | Decisão | Opção A | Opção B | Escolha | Principal Motivo |
|---|---|---|---|---|---|
| T1 | Arquitetura de deploy | Monólito Modular | Microservices | **Monólito Modular** | Complexidade adequada ao time; extração gradual possível |
| T2 | Message Broker | RabbitMQ | Kafka | **RabbitMQ** | Volume não justifica Kafka; melhor roteamento e simplicidade |
| T3 | Cache de tabelas | Redis | In-Memory | **Redis** | Compartilhamento entre réplicas; invalidação centralizada |
| T4 | Multi-Tenant | Schema por Tenant | Discriminator Column | **Schema por Tenant** | Isolamento forte; LGPD; backup individual |
| T5 | Processamento da Folha | Assíncrono | Síncrono | **Assíncrono** | Processo de 2h inviável síncrono; melhor UX e resiliência |

## Evidence vs Assumptions

**Evidências**:
- e-Social exige envio de eventos em lote (já é assíncrono por natureza)
- LGPD exige exclusão de dados por titular (schema por tenant facilita)
- Volume de ~200K eventos/mês é baixo para Kafka

**Assumptions**:
- Time de desenvolvimento 5-15 pessoas (justifica monólito modular)
- Número de empresas < 50 (viabiliza schema por tenant)
- Redis e RabbitMQ disponíveis na infraestrutura alvo

## Recommended Next Skill
`architecture-option-generator` — para documentar opções arquiteturais que ainda estão em aberto ou que podem ser revisadas no futuro.
