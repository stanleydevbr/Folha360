# Tech Spec G01 — Módulo Empresa

## Resumo Executivo

O módulo Empresa implementa o padrão **CQRS com MediatR** sobre uma arquitetura de **Domain-Driven Design (DDD)** com entidades ricas, value objects imutáveis e repositórios abstraídos. A Empresa é a raiz de agregação central — seus 9 subgrupos (Identificação, Lotação, Endereço, Contato, Configuração Fiscal, e-Social, Bancária, Geral e Processos) são implementados como entidades satélites com repositórios dedicados. Controllers magros delegam para handlers MediatR, com FluentValidation na camada de aplicação. Dados sensíveis (CPF de contatos, chave PIX) usam criptografia AES-256-GCM. A auditoria é garantida via soft delete com interceptors no DbContext.

**Mudanças arquiteturais identificadas**: (1) Empresa deve migrar do schema `public` para o schema do tenant; (2) endereço flat inline deve ser unificado na entidade `EnderecoEmpresa`.

## Arquitetura do Sistema

### Visão Geral dos Componentes

| Componente | Camada | Responsabilidade | Estado |
|-----------|--------|------------------|--------|
| `Empresa` | Domain | Raiz de agregação com dados cadastrais e fiscais | Existente — requer migração de schema |
| `Lotacao` | Domain | Estabelecimentos da empresa (matriz/filial/obra) | Existente |
| `EnderecoEmpresa` | Domain | Endereços múltiplos por tipo — absorverá endereço flat | **Modificado** — unificar endereço inline |
| `ContatoEmpresa` | Domain | Contatos com vigência e CPF criptografado | Existente |
| `ConfiguracaoBancariaEmpresa` | Domain | Contas bancárias por finalidade | Existente |
| `ConfiguracaoGeral` | Domain | Parâmetros chave-valor (hard delete) | Existente |
| `ConfiguracaoESocial` | Domain | Configuração 1:1 com Empresa | Existente |
| `ProcessoAdministrativo` | Domain | Processos judiciais/administrativos com rubricas N:N | Existente |
| `Cnpj` | Domain (VO) | Value object imutável com validação de dígitos | Existente |
| `IEmpresaRepository` | Domain | Contrato do repositório | Existente |
| `CadastrosDbContext` | Infrastructure | DbContext com interceptor de soft delete | Existente |
| `EmpresaConfiguration` | Infrastructure | Mapeamento EF Core (schema, índices, FKs) | **Modificado** — alterar schema |
| `CriarEmpresaHandler` | Application | Handler MediatR para criação com evento de domínio | Existente |
| `CriarEmpresaCommandValidator` | Application | FluentValidation (CNPJ 14 dígitos, Razão Social) | Existente |
| `EmpresasController` | Presentation | Endpoints REST com autorização por policy | Existente |
| `EmpresasEnderecosController` | Presentation | CRUD de endereços (injeção direta de repositório) | Existente |
| `EmpresasContatosController` | Presentation | CRUD de contatos (injeção direta de repositório) | Existente |
| `EmpresasConfiguracoesBancariasController` | Presentation | CRUD de contas bancárias | Existente |
| `EmpresasConfiguracoesGeraisController` | Presentation | GET/PUT de dicionário chave-valor | Existente |
| `EmpresasConfigESocialController` | Presentation | GET/PUT de configuração e-Social | Existente |
| `ProcessosAdministrativosController` | Presentation | CRUD de processos + sub-recursos de rubricas | Existente |

**Fluxo de dados principal**: `HTTP Request → Controller → MediatR Command/Query → Handler → Domain Entity/Repository → DbContext → PostgreSQL`

## Design de Implementação

### Interfaces Principais

```csharp
// Domain/Abstractions/IEmpresaRepository.cs
public interface IEmpresaRepository
{
    Task<Empresa?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken ct);
    Task<IEnumerable<Empresa>> GetAllAsync(CancellationToken ct);
    Task<(IEnumerable<Empresa> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy,
        string? cnpj, string? razaoSocial, string? regimeTributario, CancellationToken ct);
    Task AddAsync(Empresa empresa, CancellationToken ct);
    Task UpdateAsync(Empresa empresa, CancellationToken ct);
    Task SoftDeleteAsync(Guid id, CancellationToken ct);
}

// Domain/Abstractions/IExpandedRepositories.cs
public interface IEnderecoEmpresaRepository
{
    Task<IEnumerable<EnderecoEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct);
    Task<EnderecoEmpresa?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(EnderecoEmpresa endereco, CancellationToken ct);
    Task UpdateAsync(EnderecoEmpresa endereco, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
// Padrão similar para IContatoEmpresaRepository, IConfiguracaoBancariaEmpresaRepository,
// IConfiguracaoGeralRepository, IConfiguracaoESocialRepository
```

### Modelos de Dados

#### Empresa (schema: tenant — **migração necessária**)

| Coluna | Tipo | Constraints |
|--------|------|-------------|
| `id` | `uuid` | PK, default `gen_random_uuid()` |
| `tenant_id` | `uuid` | NOT NULL, indexado |
| `cnpj` | `varchar(14)` | NOT NULL, unique index com filter `deleted_at IS NULL` |
| `razao_social` | `varchar(200)` | NOT NULL, indexado |
| `nome_fantasia` | `varchar(200)` | NULL |
| `cnae` | `varchar(7)` | NULL |
| `regime_tributario` | `varchar(30)` | NOT NULL |
| `fpas` | `varchar(10)` | NULL |
| `codigo_terceiros` | `varchar(50)` | NULL |
| `classificacao_tributaria` | `varchar(30)` | NULL |
| `matriz_filial` | `varchar(10)` | NULL |
| `cnpj_matriz` | `varchar(14)` | NULL |
| `telefone` | `varchar(20)` | NULL |
| `email` | `varchar(255)` | NULL |
| `created_at` | `timestamptz` | NOT NULL |
| `updated_at` | `timestamptz` | NOT NULL |
| `deleted_at` | `timestamptz` | NULL (soft delete) |

> **Mudança**: Campos de endereço flat (`endereco_logradouro`..`endereco_uf`) serão **removidos** e migrados para `EnderecoEmpresa` com `Tipo = "Principal"`.

#### EnderecoEmpresa (schema: tenant)

| Coluna | Tipo | Constraints |
|--------|------|-------------|
| `id` | `uuid` | PK |
| `empresa_id` | `uuid` | FK → empresa, NOT NULL |
| `tipo` | `varchar(20)` | NOT NULL |
| `logradouro` | `varchar(200)` | NOT NULL |
| `numero` | `varchar(20)` | NULL |
| `complemento` | `varchar(100)` | NULL |
| `bairro` | `varchar(100)` | NULL |
| `cep` | `varchar(8)` | NULL |
| `municipio_id` | `uuid` | FK → municipios_ibge (public) |
| `uf` | `varchar(2)` | NULL |
| `estrangeiro` | `boolean` | NOT NULL, default false |
| `pais` | `varchar(50)` | NULL |

Unique index: `(empresa_id, tipo)` com filter `deleted_at IS NULL`.

#### ContatoEmpresa (schema: tenant)

| Coluna | Tipo | Constraints |
|--------|------|-------------|
| `cpf` | `varchar(14)` | [SensitiveData] — AES-256-GCM |
| `contato_principal` | `boolean` | NOT NULL, default false |
| `ativo` | `boolean` | NOT NULL, default true |
| `data_inicio_vigencia` | `date` | NULL |
| `data_fim_vigencia` | `date` | NULL |

#### ConfiguracaoBancariaEmpresa (schema: tenant)

| Coluna | Tipo | Constraints |
|--------|------|-------------|
| `banco_id` | `uuid` | FK → bancos_febraban (public) |
| `agencia` | `varchar(10)` | NOT NULL |
| `conta` | `varchar(20)` | NOT NULL |
| `chave_pix` | `varchar(100)` | [SensitiveData] — AES-256-GCM |
| `finalidade` | `varchar(20)` | NOT NULL |
| `ativa` | `boolean` | NOT NULL, default true |

Unique index: `(empresa_id, finalidade)`.

#### ConfiguracaoESocial (schema: tenant)

Relação **1:1** com Empresa (unique index em `empresa_id`). Sem soft delete.

### Endpoints de API

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| `GET` | `/api/empresas` | Consulta | Listar com paginação e filtros |
| `GET` | `/api/empresas/{id}` | Consulta | Obter por ID |
| `POST` | `/api/empresas` | Operador | Criar empresa (dispara `EmpresaCadastradaEvent`) |
| `PUT` | `/api/empresas/{id}` | Operador | Atualizar (merge do ID da rota no command) |
| `DELETE` | `/api/empresas/{id}` | Operador | Soft delete |
| `GET` | `/api/lotacoes` | Consulta | Listar lotações com paginação |
| `GET` | `/api/lotacoes/{id}` | Consulta | Obter lotação |
| `POST` | `/api/lotacoes` | Operador | Criar lotação |
| `PUT` | `/api/lotacoes/{id}` | Operador | Atualizar lotação |
| `DELETE` | `/api/lotacoes/{id}` | Operador | Soft delete (bloqueado se houver funcionários vinculados) |
| `GET` | `/api/empresas/{empresaId}/enderecos` | Consulta | Listar endereços |
| `POST` | `/api/empresas/{empresaId}/enderecos` | Operador | Adicionar endereço |
| `DELETE` | `/api/empresas/{empresaId}/enderecos/{id}` | Operador | Remover endereço |
| `GET` | `/api/empresas/{empresaId}/contatos` | Consulta | Listar contatos |
| `POST` | `/api/empresas/{empresaId}/contatos` | Operador | Adicionar contato |
| `DELETE` | `/api/empresas/{empresaId}/contatos/{id}` | Operador | Remover contato |
| `GET` | `/api/empresas/{empresaId}/configuracoes-bancarias` | Consulta | Listar contas bancárias |
| `POST` | `/api/empresas/{empresaId}/configuracoes-bancarias` | Operador | Adicionar conta |
| `DELETE` | `/api/empresas/{empresaId}/configuracoes-bancarias/{id}` | Operador | Remover conta |
| `GET` | `/api/empresas/{empresaId}/configuracoes-gerais` | Consulta | Listar parâmetros |
| `PUT` | `/api/empresas/{empresaId}/configuracoes-gerais` | Operador | Atualizar em lote (Dictionary) |
| `GET` | `/api/empresas/{empresaId}/config-esocial` | Contador | Obter config e-Social |
| `PUT` | `/api/empresas/{empresaId}/config-esocial` | Admin | Atualizar config e-Social |
| `GET` | `/api/processos-administrativos` | Consulta | Listar processos |
| `POST` | `/api/processos-administrativos` | Operador | Criar processo |
| `PUT` | `/api/processos-administrativos/{id}` | Operador | Atualizar processo |
| `DELETE` | `/api/processos-administrativos/{id}` | Operador | Soft delete |
| `GET` | `/api/processos-administrativos/{id}/rubricas` | Consulta | Listar rubricas vinculadas |
| `POST` | `/api/processos-administrativos/{id}/rubricas` | Operador | Vincular rubrica |
| `DELETE` | `/api/processos-administrativos/{id}/rubricas/{rpId}` | Operador | Desvincular rubrica |

## Pontos de Integração

### Integrações Internas

| Origem | Destino | Mecanismo | Propósito |
|--------|---------|-----------|-----------|
| `CriarEmpresaHandler` | RabbitMQ | `EmpresaCadastradaEvent` via MassTransit | Notificar outros módulos sobre nova empresa |
| `EnderecoEmpresa` | `MunicipioIBGE` (public) | FK `municipio_id` | Referência geográfica normalizada |
| `ConfiguracaoBancariaEmpresa` | `BancoFebraban` (public) | FK `banco_id` | Referência bancária oficial |
| `ContatoEmpresa` | `[SensitiveData]` interceptor | AES-256-GCM | Criptografia de CPF em repouso |
| `ConfiguracaoBancariaEmpresa` | `[SensitiveData]` interceptor | AES-256-GCM | Criptografia de chave PIX |

### Tratamento de Erros

| Cenário | Código HTTP | Error Code |
|---------|-------------|------------|
| CNPJ duplicado | 422 | `CNPJ_DUPLICADO` |
| CNPJ inválido (dígitos) | 422 | `VALIDACAO` |
| Empresa não encontrada | 404 | `NAO_ENCONTRADO` |
| Lotação com funcionários vinculados | 422 | `VINCULO_ATIVO` |
| Código de lotação duplicado | 422 | `CODIGO_DUPLICADO` |
| Erro de validação FluentValidation | 422 | `VALIDACAO` |

## Abordagem de Testes

### Testes de Unidade

**Componentes a testar**:
- `Cnpj` value object — validar algoritmo de dígitos verificadores, rejeitar sequências inválidas
- `CriarEmpresaCommandValidator` — validar CNPJ (14 dígitos), RazaoSocial obrigatório, Email formato
- `CriarLotacaoCommandValidator` — validar Codigo e Descricao obrigatórios
- `CriarEmpresaHandler` (com mocks) — verificar fluxo: validação → verificação duplicidade → criação → evento
- `ExcluirLotacaoHandler` — verificar bloqueio quando há funcionários vinculados

**Padrão de mock**: Moq + xUnit. Mockar `IEmpresaRepository`, `ITenantContext`, `IMessageBus`. NÃO mockar value objects ou entidades de domínio.

### Testes de Integração

- **EmpresaRepository**: Testar `GetByCnpjAsync` com `IgnoreQueryFilters`, `GetPagedAsync` com filtros combinados
- **Soft delete**: Verificar que `Remove` → `Modified` + `DeletedAt` populado
- **Tenant isolation**: Verificar que queries não vazam dados entre schemas
- **Evento de domínio**: Verificar que `EmpresaCadastradaEvent` é publicado na criação

### Testes de E2E

- **Playwright**: Fluxo completo de criação de empresa → cadastro de endereços → contatos → configuração bancária → configuração e-Social
- Verificar validação inline no frontend (CNPJ inválido, campos obrigatórios)
- Verificar máscara de CPF em contatos e chave PIX em contas bancárias

## Sequenciamento de Desenvolvimento

### Ordem de Construção

1. **Migração de schema** — Mover `Empresa` de `public` para tenant schema; remover campos de endereço flat; criar migration EF Core
2. **Unificação de endereço** — Migrar dados de endereço flat para `EnderecoEmpresa` com `Tipo = "Principal"`; remover propriedades inline do `Empresa`
3. **Atualizar DTOs e Commands** — Remover campos de endereço do `CriarEmpresaCommand`/`EmpresaDto`; ajustar mapeamentos nos handlers
4. **Atualizar validators** — Ajustar `CriarEmpresaCommandValidator` removendo regras de endereço
5. **Testes de regressão** — Validar que todos os testes existentes passam após as mudanças
6. **Endpoint de endereço principal** — Garantir que `POST /api/empresas/{id}/enderecos` com `Tipo = "Principal"` funcione como substituto

### Dependências Técnicas

- **Infraestrutura**: PostgreSQL com suporte a schema-per-tenant; RabbitMQ para eventos de domínio
- **Migração**: Script de migração de dados do endereço flat para `EnderecoEmpresa` (executado uma única vez)
- **Seed data**: Tabelas `municipios_ibge` e `bancos_febraban` populadas no schema `public`

## Monitoramento e Observabilidade

### Métricas (Prometheus)

| Métrica | Tipo | Descrição |
|---------|------|-----------|
| `folha360_empresa_created_total` | Counter | Total de empresas criadas |
| `folha360_empresa_operation_duration_seconds` | Histogram | Duração de operações CRUD |
| `folha360_empresa_soft_deleted_total` | Counter | Total de soft deletes |

### Logs

- **Information**: Criação de empresa (`Empresa {Id} criada com CNPJ {Cnpj}`)
- **Warning**: Tentativa de CNPJ duplicado, tentativa de excluir lotação com vínculos
- **Error**: Falha ao publicar `EmpresaCadastradaEvent` no RabbitMQ

### Health Check

- `GET /health/ready` — verifica conectividade com PostgreSQL (schema tenant)

## Considerações Técnicas

### Decisões Principais

| Decisão | Justificativa | Alternativa rejeitada |
|---------|---------------|----------------------|
| Empresa no schema tenant | Isolamento correto de dados multi-tenant; alinhamento com demais entidades | Schema público permitiria compartilhamento mas viola isolamento |
| Endereço unificado no `EnderecoEmpresa` | Elimina duplicação; endereço principal é apenas um tipo como os demais | Manter campos inline simplifica queries mas duplica modelo |
| Expanded controllers com injeção direta | Simplicidade para CRUD de sub-recursos; evita overhead de MediatR para operações triviais | Migrar para MediatR traria consistência mas adiciona complexidade desnecessária |
| `ConfiguracaoGeral` como key-value | Flexibilidade máxima para parâmetros sem alteração de schema | Tabela normalizada seria mais type-safe mas menos flexível |
| `ConfiguracaoESocial` sem soft delete | Relação 1:1 — não faz sentido "deletar" configuração; upsert é mais natural | Soft delete permitiria restore mas adiciona complexidade sem benefício |

### Riscos Conhecidos

| Risco | Impacto | Mitigação |
|-------|---------|-----------|
| Migração de schema (public → tenant) | ALTO — pode quebrar queries existentes | Executar em ambiente de homologação primeiro; script de rollback |
| Migração de endereço flat | MÉDIO — perda de dados se mal executada | Backup antes da migração; validação de integridade pós-migração |
| TenantId como string → Guid via MD5 | BAIXO — colisão de hash | MD5 de string curta (slug) tem probabilidade desprezível de colisão |

### Conformidade com Skills do Projeto

| Skill | Aplicabilidade | Status |
|-------|---------------|--------|
| `dev-best-practices` | Padrões DDD, CQRS, SOLID — seguir | ✅ Conforme |
| `efcore-migrations` | Migração de schema e dados — snake_case, schema-per-tenant | ✅ Conforme |
| `docker-infra` | Health checks, PostgreSQL, RabbitMQ | ✅ Conforme |

### Arquivos Relevantes

| Arquivo | Papel |
|---------|-------|
| `src/Folha360.Cadastros.Domain/Entities/Empresa.cs` | Entidade raiz — requer remoção de endereço flat |
| `src/Folha360.Cadastros.Domain/Entities/EnderecoEmpresa.cs` | Absorverá endereço principal |
| `src/Folha360.Infrastructure/Data/Configurations/Cadastros/EmpresaConfiguration.cs` | Alterar schema de `public` para tenant |
| `src/Folha360.Cadastros.Application/Commands/CadastrosCommands.cs` | Remover campos de endereço do `CriarEmpresaCommand` |
| `src/Folha360.Cadastros.Application/DTOs/CadastrosDtos.cs` | Remover campos de endereço do `EmpresaDto` |
| `src/Folha360.Cadastros.Application/Handlers/EmpresaHandlers.cs` | Ajustar mapeamento e remover setters de endereço |
| `src/Folha360.Cadastros.Application/Validators/CadastrosValidators.cs` | Ajustar `CriarEmpresaCommandValidator` |
| `src/Folha360.Cadastros.Presentation/Controllers/CoreControllers.cs` | `EmpresasController` — sem alterações |
| `src/Folha360.Cadastros.Presentation/Controllers/EmpresaExpandedControllers.cs` | Controllers expandidos — sem alterações |
| `src/Folha360.IoC/ServiceCollectionExtensions.cs` | Registro de DI — sem alterações |
