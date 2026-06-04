# 🚀 Boas Práticas — DevOps e FinOps

> **Escopo**: CI/CD, containers, Kubernetes, otimização de custos  
> **Referências**: [Visão de Deployment](./deployment-view), [ADR-001](./adr-001-monolito-modular), [ADR-005](./adr-005-redis-cache-tabelas)  
> **Fonte completa**: [`docs/instrucoes/05-devops-finops.md`](../instrucoes/05-devops-finops.md)

---

## Containerização

- Dockerfile **multi-stage** (.NET 10 SDK → `aspnet:10.0-chiseled`)
- **Non-root**: `USER $APP_UID`
- `.dockerignore`: `bin/`, `obj/`, `.git/`, `tests/`, `docs/`

## Docker Compose (Dev)

- PostgreSQL 16, Redis 7, RabbitMQ 3.13, Seq
- Health checks em todos os serviços
- Secrets via variáveis de ambiente (`.env`)

## Kubernetes (Prod)

| Recurso | Config | Justificativa |
|---|---|---|
| api-folha | 3-10 réplicas, HPA | Pico no fechamento mensal |
| api-esocial | 2-5 réplicas, HPA | Pico no envio de lotes |
| api-relatorios | 2 réplicas | Lê réplica PostgreSQL |

## CI/CD (GitHub Actions)

```
Build → Lint → Test (80%+) → Security Scan → Deploy (auto dev, manual prod)
```

## Migrations

- Script itera sobre todos os schemas ativos
- Cada migração deve ser **reversível** (`Up()` e `Down()`)
- Testar com N tenants antes de prod

## FinOps

| Recurso | Otimização |
|---|---|
| Redis | TTL: 24h (tabelas), 1h (rubricas). Alerta se hit rate < 90% |
| PostgreSQL | PgBouncer pooling, read replica para relatórios, partitioning por ano |
| RabbitMQ | Mensagens persistentes só para e-Social, TTL em filas não-críticas |
| MinIO | Lifecycle: PDFs holerite → Glacier após 2 anos |

---

**Fonte completa com exemplos**: [`docs/instrucoes/05-devops-finops.md`](../instrucoes/05-devops-finops.md)
