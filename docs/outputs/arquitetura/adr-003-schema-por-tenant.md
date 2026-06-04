# ADR-003: Schema por Tenant para Multi-Tenancy

## Status
**Accepted** (Junho 2026)

## Context
O sistema Folha360 precisa suportar múltiplas empresas (matriz e filiais) com isolamento total de dados. Cada empresa tem seus próprios funcionários, rubricas, eventos e-Social e histórico de pagamentos. A LGPD exige que dados de uma empresa possam ser excluídos sem afetar outras. Precisamos decidir a estratégia de isolamento no PostgreSQL.

## Decision Drivers
1. **Isolamento de dados**: Dados de empresas diferentes não podem se misturar
2. **LGPD**: Exclusão de dados por solicitação do titular (art. 18) deve ser simples e segura
3. **Backup/Restore**: Capacidade de restaurar dados de uma única empresa
4. **Performance**: Índices e consultas não devem degradar com crescimento de tenants
5. **Número de tenants**: Estimado em < 50 empresas

## Options Considered

| Opção | Prós | Contras |
|---|---|---|
| **Schema por Tenant (escolhido)** | Isolamento forte; backup individual; índices menores; exclusão trivial (drop schema) | Migrations em N schemas; mais conexões; limite prático ~100 tenants |
| **Discriminator Column (tenant_id)** | Simples; migrations únicas; suporta milhares de tenants | Isolamento fraco; toda query precisa WHERE; índices grandes; exclusão complexa |
| **Database por Tenant** | Isolamento máximo; escala independente; backup trivial | Conexões N×; custo alto; migrations N×; complexidade operacional |
| **Row-Level Security (RLS)** | Isolamento a nível de banco; transparente para aplicação | PostgreSQL only; performance overhead; debugging difícil |

## Decision
**Schema por Tenant**: Cada empresa terá seu próprio schema no PostgreSQL (`tenant_001`, `tenant_002`, etc.). Um schema `public` conterá tabelas compartilhadas (usuários do sistema, configurações globais). A aplicação resolve o schema com base no contexto da requisição (JWT claim `tenant_id` ou subdomínio).

## Consequences

### Positive
- Isolamento forte: um bug em query não expõe dados de outra empresa
- Exclusão LGPD: `DROP SCHEMA tenant_XXX CASCADE` resolve a exclusão de todos os dados
- Backup/Restore individual por empresa
- Índices menores e mais eficientes (cada schema tem seus próprios índices)
- Facilita mover empresa pesada para servidor separado no futuro

### Negative
- Migrations precisam ser executadas em N schemas
- Pool de conexões maior (cada schema pode ter conexões dedicadas)
- Limite prático de ~100 tenants (não é problema para o cenário atual)
- Complexidade para queries cross-tenant (relatórios consolidados — raro)

### Mitigações
- Script de migração automatizado que itera sobre todos os schemas
- Connection pool dimensionado para N schemas × 10 conexões
- Template schema para criação rápida de novos tenants
- Monitoramento de número de schemas e tempo de migração

## Follow-up Actions
- [ ] Criar template schema com todas as tabelas base
- [ ] Automatizar criação de schema no onboarding de nova empresa
- [ ] Script de migração que itera sobre schemas
- [ ] Teste de performance com 50 schemas simultâneos
- [ ] Revisar esta decisão se número de empresas ultrapassar 80
