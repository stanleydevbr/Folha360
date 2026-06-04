# 📋 Índice de Boas Práticas de Desenvolvimento

> Guia de referência rápida para o time de desenvolvimento do Folha360.  
> Todas as instruções são baseadas nos [ADRs](./adr-001-monolito-modular) e artefatos de arquitetura aceitos.

---

## Stack Resumida

| Camada | Tecnologia |
|--------|-----------|
| Backend | .NET 10, ASP.NET Core, EF Core, MediatR |
| Frontend | React 18, TypeScript, Vite, Tailwind CSS, TanStack Query |
| Banco | PostgreSQL 16 (schema-per-tenant), Redis 7 |
| Mensageria | RabbitMQ 3.13 |
| Container | Docker, Kubernetes (prod), Docker Compose (dev) |
| CI/CD | GitHub Actions |

---

## Instruções por Escopo

### [🔧 Backend .NET 10](./instrucoes-backend)
- Estrutura da solução (4 projetos por módulo)
- Controller magro + MediatR/CQRS
- Repository pattern com EF Core
- Result pattern (não exceptions para negócio)
- Multi-tenant via search_path dinâmico

### [⚛️ Frontend React](./instrucoes-frontend)
- Feature-first folder structure
- TanStack Query para server state
- React Hook Form + Zod
- WCAG 2.1 AA acessibilidade
- SignalR + polling para processamento assíncrono

### [🧩 Design Patterns](./instrucoes-design-patterns)
- Repository, Strategy, Factory, Observer, Saga
- CQRS (MediatR Commands + Dapper Queries)
- Circuit Breaker (Polly)
- Idempotência em processamento

### [📐 Princípios SOLID](./instrucoes-solid)
- **S**: FolhaService ≠ ObrigacoesFiscaisService
- **O**: Strategy para IRRF anual (nova classe, sem if/else)
- **L**: Repository PostgreSQL = InMemory (testes)
- **I**: IFuncionarioReadOnlyRepository ≠ IFuncionarioWriteRepository
- **D**: Domain define interfaces, Infrastructure implementa

### [🚀 DevOps e FinOps](./instrucoes-devops-finops)
- Dockerfile multi-stage (chiseled, non-root)
- Kubernetes: HPA, health checks, network policies
- CI/CD: build → lint → test → security → deploy
- Migrations em N schemas (tenants)
- FinOps: Redis TTL, PgBouncer, right-sizing

### [🗄️ Banco de Dados](./instrucoes-database)
- Schema por tenant (ADR-003)
- Índices prioritários (P1/P2)
- Criptografia AES-256 (LGPD)
- Audit log automático (interceptor EF Core)
- Backup por schema (restore individual)

### [🛡️ Segurança](./instrucoes-seguranca)
- JWT com claims de tenant + role
- Refresh token rotation + blacklist Redis
- Rate limiting (5 tentativas login/15min)
- AES-256 dados repouso, TLS 1.3 trânsito
- LGPD: direito ao esquecimento, consentimento
- Headers: HSTS, CSP, X-Frame-Options

---

## ADRs que embasam as instruções

| ADR | Decisão | Impacto nas Instruções |
|-----|---------|----------------------|
| [ADR-001](./adr-001-monolito-modular) | Monólito Modular | Projetos .NET por módulo, interfaces entre bounded contexts |
| [ADR-002](./adr-002-rabbitmq-message-broker) | RabbitMQ | Observer pattern, eventos de domínio via message bus |
| [ADR-003](./adr-003-schema-por-tenant) | Schema por Tenant | Migrations em N schemas, query filters, tenant middleware |
| [ADR-004](./adr-004-processamento-assincrono-folha) | Processamento Assíncrono | 202 Accepted, SignalR, idempotência |
| [ADR-005](./adr-005-redis-cache-tabelas) | Redis Cache | TTL (24h tabelas, 1h rubricas), fallback PostgreSQL |

---

## Regras Globais

1. **NUNCA contradizer ADRs** — Se um ADR diz "schema por tenant", as instruções DEVEM seguir isso
2. **Contextualizar sempre** — Exemplos usam domínio de folha de pagamento (Funcionário, Rubrica, Holerite)
3. **Priorizar por risco** — Dados sensíveis (LGPD) e envio ao e-Social têm prioridade máxima
4. **Incluir "NÃO FAZER"** — Anti-patterns são tão importantes quanto boas práticas
