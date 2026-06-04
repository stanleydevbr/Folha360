# ADR-005: Redis como Cache de Tabelas Progressivas e Rubricas

## Status
**Accepted** (Junho 2026)

## Context
O cálculo da folha de pagamento consulta intensivamente tabelas progressivas (IRRF, INSS) e rubricas (vencimentos, descontos) para cada um dos 100.000 funcionários. Sem cache, isso geraria milhões de consultas ao PostgreSQL, comprometendo a performance e o SLA de 2 horas. Precisamos decidir a estratégia de cache.

## Decision Drivers
1. **Performance**: Cálculo da folha precisa processar > 50 funcionários/segundo
2. **Compartilhamento**: Múltiplas réplicas da `api-folha` (HPA) precisam compartilhar cache
3. **Invalidação**: Tabelas mudam anualmente (IRRF); rubricas mudam raramente; cache precisa ser invalidado centralizadamente
4. **Persistência**: Cache não pode ser perdido em restart de container (cold start seria lento)
5. **Custo operacional**: Infra adicional deve ser justificada

## Options Considered

| Opção | Prós | Contras |
|---|---|---|
| **Redis (escolhido)** | Compartilhado entre réplicas; invalidação centralizada (pub/sub); persistente; maduro | +1 infra; latência de rede (sub-ms em mesmo namespace) |
| **In-Memory (ConcurrentDictionary)** | Mais rápido (microssegundo); sem infra extra | Não compartilhado; cold start; invalidação complexa; compete com GC |
| **Materialized View (PostgreSQL)** | Sem infra extra; consistente; SQL padrão | Lento comparado a cache em memória; ainda sobrecarrega DB |
| **Sem cache (consultas diretas)** | Simples; sempre consistente | Inviável: milhões de consultas; SLA de 2h impossível |

## Decision
**Redis**: Tabelas progressivas (IRRF, INSS) e rubricas vigentes são carregadas no Redis no startup e invalidades via pub/sub quando alteradas. Cada réplica da `api-folha` consulta Redis antes de calcular cada funcionário. TTL configurado para 24h (tabelas) e 1h (rubricas), com fallback para PostgreSQL em caso de cache miss.

## Consequences

### Positive
- Cache compartilhado: todas as réplicas se beneficiam (importante no HPA)
- Invalidação centralizada: alterar rubrica → publicar `invalidate:rubricas` → todas réplicas atualizam
- Persistente: Redis RDB/AOF sobrevive a restart
- Sub-ms latency no mesmo namespace Kubernetes
- Reduz consultas ao PostgreSQL em > 95%

### Negative
- +1 container/infra para gerenciar (Redis)
- Latência de rede (embora mínima no mesmo namespace)
- Cache invalidation é um problema clássico (mas resolvido com pub/sub)
- Se Redis cair, performance degrada (fallback PostgreSQL)

### Mitigações
- Fallback automático para PostgreSQL se Redis indisponível (graceful degradation)
- TTL agressivo para evitar stale data
- Monitoramento de hit rate (alerta se < 90%)
- Redis Sentinel ou Cluster para alta disponibilidade

## Follow-up Actions
- [ ] Provisionar Redis no Kubernetes
- [ ] Implementar CacheService com fallback para PostgreSQL
- [ ] Configurar pub/sub para invalidação de cache
- [ ] Definir TTLs: tabelas progressivas (24h), rubricas (1h)
- [ ] Monitorar hit rate e latência
