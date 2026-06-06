---
description: "Use when: modifying Dockerfile, docker-compose, adding new .NET projects/modules, configuring health checks, setting up MassTransit/RabbitMQ, Redis, MinIO, or any infrastructure service environment variables. Covers Docker build optimization, health check tools, and env var auditing for new modules."
applyTo: ["Dockerfile", "docker-compose*.yml", "docker-compose*.yaml", "docker/**"]
---

# Docker & Infra Guidelines — Folha360

Regras derivadas das lições aprendidas no deploy do módulo F02 (Gestão de Cadastros).
Aplicam-se a qualquer novo módulo (F03, F04, etc.) ou alteração em infraestrutura.

---

## 1. Dockerfile: Copiar .csproj de TODOS os projetos

**Sempre que um novo projeto .NET for adicionado ao monólito modular**, adicionar uma linha `COPY` no Dockerfile para o `.csproj`, ANTES do `dotnet restore`.

Formato:
```dockerfile
COPY src/<Projeto>/*.csproj src/<Projeto>/
```

Exemplo para F02:
```dockerfile
COPY src/Folha360.Cadastros.Domain/*.csproj src/Folha360.Cadastros.Domain/
COPY src/Folha360.Cadastros.Application/*.csproj src/Folha360.Cadastros.Application/
COPY src/Folha360.Cadastros.Infrastructure/*.csproj src/Folha360.Cadastros.Infrastructure/
COPY src/Folha360.Cadastros.Presentation/*.csproj src/Folha360.Cadastros.Presentation/
```

## 2. Dockerfile: Health Check precisa de `curl`

A imagem `mcr.microsoft.com/dotnet/aspnet:10.0` é minimalista e **não inclui `curl` nem `wget`**.

O Dockerfile DEVE instalar `curl` no estágio de runtime:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
```

## 3. docker-compose.app.yml: Variáveis de ambiente para TODOS os serviços

Para cada novo serviço de infraestrutura usado por um módulo, adicionar as variáveis de ambiente correspondentes no serviço `folha360-webapi`.

**Template de variáveis por serviço:**

| Serviço | Variáveis |
|---|---|
| PostgreSQL | `ConnectionStrings__Postgres` |
| Redis | `ConnectionStrings__Redis` |
| RabbitMQ/MassTransit | `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password` + `ConnectionStrings__RabbitMQ` |
| MinIO | `MinIO__Endpoint`, `MinIO__AccessKey`, `MinIO__SecretKey`, `MinIO__BucketName` |
| Seq | `Seq__Url` |
| JWT | `Jwt__Secret` |

**IMPORTANTE:** O MassTransit lê configurações separadas (`RabbitMQ:Host`, `RabbitMQ:Username`, `RabbitMQ:Password`), NÃO apenas a connection string. Ambas as formas devem estar presentes.

## 4. Health Check: Usar `curl`

O health check no docker-compose deve usar `curl` (que foi instalado no Dockerfile):

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:8080/health/live"]
  interval: 10s
  timeout: 5s
  retries: 3
  start_period: 15s
```

**NUNCA usar `wget` ou `/dev/tcp`** — não funcionam na imagem `aspnet:10.0`.

## 5. PostgreSQL: Extensões no `init.sh`

**Sempre que um novo módulo usar extensões PostgreSQL**, adicionar ao `docker/postgres/init.sh`:

```sql
CREATE EXTENSION IF NOT EXISTS vector;       -- pgvector (F01)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";  -- UUID generation (F01)
CREATE EXTENSION IF NOT EXISTS pg_trgm;      -- Trigram indexes for text search (F02)
-- Adicionar novas extensões aqui
```

## 6. Snake Case Naming Convention

O projeto usa `EFCore.NamingConventions` com `UseSnakeCaseNamingConvention()`.
Isso garante que todas as colunas no banco usem `snake_case` (ex: `deleted_at`, `razao_social`),
evitando inconsistências entre PascalCase do C# e snake_case do PostgreSQL.

**NUNCA** remover essa configuração dos DbContexts no DI.

## 7. Checklist de Deploy para Novo Módulo

- [ ] Adicionar `COPY` dos novos `.csproj` no `Dockerfile`
- [ ] Adicionar variáveis de ambiente no `docker-compose.app.yml` para TODOS os serviços de infra usados
- [ ] Health check usa `curl` (já instalado no Dockerfile)
- [ ] RabbitMQ: verificar `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password`
- [ ] Redis: verificar `ConnectionStrings__Redis`
- [ ] MinIO: verificar `MinIO__*` (se aplicável)
- [ ] PostgreSQL: verificar extensões no `docker/postgres/init.sh`
- [ ] Rodar `dotnet build` completo antes do deploy
- [ ] Rodar `docker compose -f docker-compose.yml -f docker-compose.app.yml up -d --build`
- [ ] Verificar logs com `docker logs folha360-webapi`
- [ ] Se o banco já existir com schema antigo, dropar o volume: `docker volume rm folha360_postgres-data`
