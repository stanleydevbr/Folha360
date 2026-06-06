# Lições Aprendidas — Deploy Docker F02 (Gestão de Cadastros)

**Data:** 2026-06-05
**Contexto:** Atualização do Dockerfile e docker-compose.app.yml para o módulo F02 e troubleshooting de migrations EF Core + health checks + MassTransit/RabbitMQ.

---

## 1. Dockerfile: Projetos precisam ser copiados ANTES do restore

### Problema
O `Dockerfile` original só copiava os 6 projetos base da F01. Os 4 novos projetos do módulo F02 (`Folha360.Cadastros.*`) não eram copiados, fazendo o `dotnet restore` falhar ao não encontrar as referências.

### Solução
Adicionar uma linha `COPY` para cada `.csproj` novo no estágio de build, ANTES do `dotnet restore`:

```dockerfile
COPY src/Folha360.Cadastros.Domain/*.csproj src/Folha360.Cadastros.Domain/
COPY src/Folha360.Cadastros.Application/*.csproj src/Folha360.Cadastros.Application/
COPY src/Folha360.Cadastros.Infrastructure/*.csproj src/Folha360.Cadastros.Infrastructure/
COPY src/Folha360.Cadastros.Presentation/*.csproj src/Folha360.Cadastros.Presentation/
```

### Lição
> **Sempre que um novo projeto for adicionado ao monólito modular, atualizar o Dockerfile com a cópia do `.csproj` correspondente.** Isso faz parte do checklist de setup de qualquer novo módulo (F03, F04, etc.).

---

## 2. EF Core: Dois DbContexts no mesmo banco = Conflito de Migrations

### Problema
A arquitetura do monólito modular usa dois DbContexts que apontam para o mesmo banco:
- `Folha360DbContext` (tabelas base: `tenant`, `usuario`, `audit_log`)
- `CadastrosDbContext` (herda de `Folha360DbContext` + 10 entidades F02)

Quando `MigrateAsync()` era chamado no `Folha360DbContext`, o EF Core detectava que o modelo tinha mudanças pendentes (as entidades do `CadastrosDbContext`) e lançava `PendingModelChangesWarning`, crashando a aplicação.

### Solução Inicial (Paliativa — 2026-06-05)
1. Suprimir `PendingModelChangesWarning` no `Folha360DbContext.OnConfiguring()`
2. Usar `Folha360DbContext` para migrations (tabelas base) + `EnsureCreatedAsync` no `CadastrosDbContext` (tabelas F02)

### Solução Definitiva (Refatoração — 2026-06-06)
Após análise de alternativas profissionais de mercado, o projeto foi refatorado para a arquitetura de **Migrations Assembly Centralizado**:

- `Folha360DbContext` → modelo CANÔNICO com TODAS as entidades de TODOS os módulos
- `CadastrosDbContext` → runtime-only, herda do canônico, sem configurações próprias
- `Folha360.Infrastructure` → ÚNICO assembly com migrations
- `Program.cs` → ÚNICA chamada `MigrateAsync()` no `Folha360DbContext`
- `IEntityTypeConfiguration<T>` → configurações extraídas para arquivos separados por módulo
- `UseSnakeCaseNamingConvention()` → padronização de nomenclatura PostgreSQL
- `EncryptionConverter` → movido para `Folha360.Infrastructure` (compartilhado)

### Lição
> **Para monólitos modulares com múltiplos módulos, usar Migrations Assembly Centralizado com um DbContext canônico.** Isso elimina conflitos de migrations, respeita bounded contexts (via DbContexts runtime-only por módulo), e escala para N módulos. A abordagem com `EnsureCreated` + supressão de warnings NÃO é profissional e não escala.

---

## 3. MassTransit/RabbitMQ: Configuração de conexão vs Connection String

### Problema
O `docker-compose.app.yml` definia `ConnectionStrings__RabbitMQ` (formato de connection string), mas o código no `ServiceCollectionExtensions` lia configurações separadas:
```csharp
cfg.Host(configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
{
    h.Username(configuration["RabbitMQ:Username"] ?? "folha360");
    h.Password(configuration["RabbitMQ:Password"] ?? "folha360");
});
```

Como as variáveis `RabbitMQ:Host`, `RabbitMQ:Username` e `RabbitMQ:Password` não existiam, o MassTransit usava os defaults (`folha360`/`folha360`), enquanto a senha real era `Folha360@Dev`. Resultado: `ACCESS_REFUSED`.

### Solução
Adicionar as variáveis de ambiente individuais no `docker-compose.app.yml`:
```yaml
RabbitMQ__Host: rabbitmq
RabbitMQ__Username: ${RABBITMQ_USER:-folha360}
RabbitMQ__Password: ${RABBITMQ_PASSWORD:-Folha360@Dev}
```

### Lição
> **Garantir que TODAS as variáveis de ambiente que o código lê estejam definidas no docker-compose**, não apenas a connection string. Fazer uma auditoria de `IConfiguration` keys usadas em cada módulo e documentá-las.

---

## 4. Docker Health Check: `curl` não está disponível no ASP.NET 10

### Problema
O health check usava `curl`:
```yaml
test: ["CMD", "curl", "-f", "http://localhost:8080/health/live"]
```
Mas a imagem `mcr.microsoft.com/dotnet/aspnet:10.0` (baseada em Ubuntu Noble) **não inclui `curl`**. O health check falhava silenciosamente, marcando o container como `unhealthy`.

### Tentativas que NÃO funcionaram
- `wget` — também não disponível
- `/dev/tcp` — não suportado pelo shell `dash` (padrão no Ubuntu)

### Solução
Adicionar `curl` ao Dockerfile no estágio de runtime:
```dockerfile
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
```

### Lição
> **A imagem `aspnet:10.0` é minimalista.** Sempre verificar se ferramentas como `curl`, `wget`, `ping` estão disponíveis antes de usá-las em health checks. Alternativas: usar `dotnet` CLI para health check, ou health check via porta TCP com `/bin/sh -c "echo > /dev/tcp/localhost/8080"` (requer bash, não dash).

---

## 5. MinIO: Storage de documentos precisa de variáveis de ambiente

### Problema
O módulo F02 inclui gestão de documentos com storage no MinIO, mas as variáveis de ambiente não estavam configuradas no `docker-compose.app.yml`.

### Solução
Adicionar as variáveis:
```yaml
MinIO__Endpoint: minio:9000
MinIO__AccessKey: ${MINIO_ROOT_USER:-minioadmin}
MinIO__SecretKey: ${MINIO_ROOT_PASSWORD:-minioadmin}
MinIO__BucketName: folha360-documentos
```

### Lição
> **Para cada novo serviço de infraestrutura adicionado a um módulo, verificar se as variáveis de ambiente correspondentes estão no docker-compose.** Manter uma matriz de variáveis por módulo na documentação.

---

## 6. PostgreSQL: Extensões precisam estar no `init.sh`

### Problema
A migration criava índices GIN com `gin_trgm_ops`, mas a extensão `pg_trgm` não estava instalada. Erro: `classe de operadores "gin_trgm_ops" não existe`.

### Solução
Adicionar a extensão ao `docker/postgres/init.sh`:
```sql
CREATE EXTENSION IF NOT EXISTS pg_trgm;
```

### Lição
> **Sempre que uma migration usar extensões PostgreSQL (pg_trgm, postgis, etc.), adicionar o `CREATE EXTENSION` ao `init.sh`.** O container Postgres é recriado com volume limpo em ambientes de desenvolvimento, e a extensão precisa ser recriada.

---

## 7. Snake Case Naming Convention: Essencial para PostgreSQL + EF Core

### Problema
O query filter de soft delete usava `nameof(ISoftDeletable.DeletedAt)` que gerava a coluna `DeletedAt`, mas o Npgsql por padrão convertia para `deleted_at` (snake_case). A migration criava `DeletedAt` (PascalCase) e o runtime buscava `deleted_at` (snake_case). Resultado: `coluna "deleted_at" não existe`.

### Solução
Adotar `UseSnakeCaseNamingConvention()` do pacote `EFCore.NamingConventions` em TODOS os DbContexts:
```csharp
optionsBuilder.UseSnakeCaseNamingConvention().UseNpgsql(connectionString);
```
Isso garante que tanto as migrations quanto o runtime usem `snake_case` consistentemente.

### Lição
> **Sempre usar `UseSnakeCaseNamingConvention()` ao trabalhar com PostgreSQL + EF Core.** Isso evita inconsistências entre o naming do C# (PascalCase) e a convenção do PostgreSQL (snake_case). Configurar no DI (ServiceCollectionExtensions) E na factory de design-time (DbContextFactory).

---

## 8. Refatoração Final: Migrations Assembly Centralizado

### Decisão Arquitetural
Após análise das 3 opções profissionais de mercado (DbContext Único, Migrations Assembly Centralizado, SQL Scripts), foi escolhida a **Opção 2 — Migrations Assembly Centralizado** por ser a que melhor escala para múltiplos módulos, respeita bounded contexts DDD, e permite trabalho paralelo de times.

### Estrutura Final
```
Folha360.Infrastructure/              ← DONO das migrations
├── Data/
│   ├── Configurations/
│   │   ├── Base/                     ← 3 configurações
│   │   └── Cadastros/                ← 10 configurações
│   ├── Folha360DbContext.cs          ← Modelo CANÔNICO
│   ├── EncryptionConverter.cs       ← Compartilhado
│   └── Migrations/                   ← ÚNICO local

Folha360.Cadastros.Infrastructure/
├── Data/
│   └── CadastrosDbContext.cs         ← Runtime-only
```

### Arquivos Criados (13)
- `TenantConfiguration.cs`, `UsuarioConfiguration.cs`, `AuditLogConfiguration.cs`
- `EmpresaConfiguration.cs` ... `HorarioTrabalhoConfiguration.cs` (10 arquivos)

### Arquivos Modificados (8)
- `Folha360DbContext.cs` — modelo canônico
- `CadastrosDbContext.cs` — runtime-only
- `Folha360.Infrastructure.csproj` — +ref `Cadastros.Domain`, +`EFCore.NamingConventions`
- `Folha360DbContextFactory.cs` — +`UseSnakeCaseNamingConvention`
- `ServiceCollectionExtensions.cs` — +snake_case nos 2 DbContexts
- `Program.cs` — única chamada `MigrateAsync`
- `docker/postgres/init.sh` — +`pg_trgm`
- `Dockerfile` — +`curl`

### Arquivos Removidos (3)
- `EncryptionConverter.cs` (movido para `Infrastructure`)
- `CadastrosDbContextFactory.cs` (não mais necessário)
- `Migrations/` do `Cadastros.Infrastructure`

---

## 9. Checklist para Novos Módulos (F03, F04, etc.) — ATUALIZADO

- [ ] Adicionar `COPY` dos novos `.csproj` no `Dockerfile`
- [ ] Adicionar variáveis de ambiente no `docker-compose.app.yml` para TODOS os serviços de infra
- [ ] Criar `IEntityTypeConfiguration<T>` para cada entidade em `Folha360.Infrastructure/Data/Configurations/[Modulo]/`
- [ ] Adicionar `DbSet<T>` + `ApplyConfiguration` no `Folha360DbContext`
- [ ] Criar `[Modulo]DbContext` runtime-only herdando de `Folha360DbContext`
- [ ] Registrar `[Modulo]DbContext` no DI com `UseSnakeCaseNamingConvention()`
- [ ] Adicionar extensões PostgreSQL ao `docker/postgres/init.sh` (se necessário)
- [ ] Gerar migration: `dotnet ef migrations add Add[Modulo]Entities --project src/Folha360.Infrastructure`
- [ ] `dotnet build` completo passa
- [ ] `docker compose up -d --build` e verificar logs
- [ ] Se banco existir com schema antigo: `docker volume rm folha360_postgres-data`

---

## 10. Arquivos Modificados (Final)

| Arquivo | Alteração |
|---|---|
| `Dockerfile` | +4 COPY projetos F02, +instalação de `curl` |
| `docker-compose.app.yml` | +MinIO env vars, +RabbitMQ env vars individuais |
| `docker/postgres/init.sh` | +`CREATE EXTENSION pg_trgm` |
| `src/Folha360.WebApi/Program.cs` | Única chamada `MigrateAsync()` no `Folha360DbContext` |
| `src/Folha360.Infrastructure/Data/Folha360DbContext.cs` | Modelo canônico com TODAS as entidades + soft delete filter |
| `src/Folha360.Infrastructure/Data/EncryptionConverter.cs` | **Movido** do `Cadastros.Infrastructure` |
| `src/Folha360.Infrastructure/Data/Folha360DbContextFactory.cs` | +`UseSnakeCaseNamingConvention` |
| `src/Folha360.Infrastructure/Folha360.Infrastructure.csproj` | +ref `Cadastros.Domain`, +`EFCore.NamingConventions` |
| `src/Folha360.Infrastructure/Data/Configurations/Base/*.cs` | **3 novos** — Tenant, Usuario, AuditLog |
| `src/Folha360.Infrastructure/Data/Configurations/Cadastros/*.cs` | **10 novos** — Empresa ... HorarioTrabalho |
| `src/Folha360.Cadastros.Infrastructure/Data/CadastrosDbContext.cs` | Runtime-only, delega ao canônico |
| `src/Folha360.Cadastros.Infrastructure/Data/EncryptionConverter.cs` | **Removido** (movido) |
| `src/Folha360.Cadastros.Infrastructure/Data/CadastrosDbContextFactory.cs` | **Removido** (não mais necessário) |
| `src/Folha360.IoC/ServiceCollectionExtensions.cs` | +`UseSnakeCaseNamingConvention` nos 2 DbContexts |
| `.github/instructions/efcore-migrations.instructions.md` | **Atualizado** — arquitetura final |
| `.github/instructions/docker-infra.instructions.md` | **Atualizado** — +extensões PostgreSQL, +snake_case |
