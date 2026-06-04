# 🗄️ Boas Práticas — Banco de Dados

> **Escopo**: PostgreSQL, migrations, multi-tenant, performance  
> **Referências**: [Modelo de Banco](./database-model), [ADR-003](./adr-003-schema-por-tenant)  
> **Fonte completa**: [`docs/instrucoes/06-database.md`](../instrucoes/06-database.md)

---

## Multi-Tenant (Schema por Tenant)

```
PostgreSQL
├── public              ← Shared (usuarios, empresas, audit_log)
├── tenant_001          ← Empresa 1 (funcionario, folha_mensal, ...)
├── tenant_002          ← Empresa 2 (mesmas tabelas)
└── ...
```

- Resolução via JWT claim `tenant_id` → `search_path` dinâmico
- Query filter global para soft delete (`deleted_at IS NULL`)

## Índices Prioritários (P1)

| Tabela | Índice | Tipo | Motivo |
|---|---|---|---|
| `funcionario` | `(empresa_id, status)` | B-tree | Listar ativos por empresa |
| `folha_mensal` | `(funcionario_id, periodo)` | B-tree | Consultar holerite |
| `processamento_folha` | `(empresa_id, periodo)` | UNIQUE | Idempotência |
| `funcionario` | `(cpf)` | Hash | Busca exata criptografada |

## Migrations

- **Sem dados** em migrations (usar seeders)
- Cada migração deve ser **reversível**
- Script itera sobre N schemas (tenants)

## Dados Sensíveis (LGPD)

| Coluna | Criptografia | Busca |
|---|---|---|
| `cpf` (funcionario, dependente) | AES-256 | Hash index |
| `pis_pasep`, `ctps_numero` | AES-256 | — |
| `liquido` (folha_mensal) | AES-256 | — |

## Auditoria

- Interceptor EF Core grava `audit_log` automaticamente (quem, quando, o quê)
- **Append-only**: nunca atualiza ou deleta
- NÃO logar colunas criptografadas

## Backup

- `pg_dump` por schema (restore individual por empresa)
- WAL archiving para RPO < 5 min

## Anti-Patterns

| ❌ Não Fazer | Consequência |
|---|---|
| Query sem índice em WHERE | Full table scan |
| N+1 queries (lazy loading em loop) | Milhares de queries, timeout |
| Migration com dados hardcoded | Falha em outros ambientes |
| `SELECT *` | Dados desnecessários, dados sensíveis expostos |

---

**Fonte completa com exemplos de código**: [`docs/instrucoes/06-database.md`](../instrucoes/06-database.md)
