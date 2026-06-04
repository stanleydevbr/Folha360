# Visão de Arquitetura em Camadas — Folha360

## Summary
Proposta de arquitetura em 4 camadas lógicas (Presentation, Application, Domain, Infrastructure) com dependência unidirecional em direção ao Domain, adaptada para o domínio de folha de pagamento compatível com e-Social v. S-1.3. A separação em camadas permite isolar regras de negócio fiscal/trabalhista das preocupações de infraestrutura (mensageria e-Social, persistência) e da interface com o usuário.

## Camadas

### 1. Presentation (Apresentação)
**Responsabilidade**: Interface com o usuário e APIs externas.
**Tecnologia**: React (SPA) + .NET API Controllers (REST)

| Elemento | Descrição |
|---|---|
| **Folha360 Web** | SPA React com dashboard admin, gestão de funcionários, visualização de holerites, relatórios |
| **API Controllers** | Endpoints REST em .NET: `CadastrosController`, `FolhaController`, `RelatoriosController`, `EventosController`, `FiscaisController`, `eSocialController` |
| **Auth Gateway** | Autenticação JWT + autorização por perfil (admin, operador, consulta) |

**Regra**: NUNCA acessa banco diretamente. Apenas chama serviços da camada Application.

### 2. Application (Aplicação / Casos de Uso)
**Responsabilidade**: Orquestração de casos de uso, coordenação de transações distribuídas, DTOs, validação de entrada.
**Tecnologia**: .NET Application Services, MediatR (CQRS leve)

| Serviço | Casos de Uso |
|---|---|
| `CadastrosService` | CRUD funcionários, cargos, rubricas, empresas; validação de dados obrigatórios e-Social |
| `FolhaService` | Iniciar cálculo da folha, aplicar rubricas, gerar holerites, fechar período |
| `EventosTrabalhistasService` | Registrar admissão (S-2200), férias (S-2230), afastamentos (S-2230), desligamentos (S-2299) |
| `ObrigacoesFiscaisService` | Calcular IRRF, INSS, FGTS; gerar eventos S-5001/S-5002; apurar GPS |
| `RelatoriosService` | Gerar relatórios mensais, anuais, DIRF, RAIS; exportação CSV/PDF |
| `IntegracaoESocialService` | Enfileirar eventos, consultar recibos, reprocessar falhas, validar schemas XSD |

**Regra**: Orquestra chamadas ao Domain. Não contém regras de negócio, apenas coordenação.

### 3. Domain (Domínio / Negócio)
**Responsabilidade**: Regras de negócio, entidades, value objects, agregados, eventos de domínio.
**Tecnologia**: .NET Domain Model (POCOs, agregados DDD)

| Agregado | Entidades | Regras |
|---|---|---|
| **Funcionario** | Dados pessoais, endereço, dependentes, documentos | Validação CPF, CTPS, PIS/PASEP; elegibilidade para eventos e-Social |
| **Empresa** | CNPJ, razão social, CNAE, regime tributário, FPAS | Classificação tributária; matriz/filiais |
| **Rubrica** | Código, natureza (vencimento/desconto), incidências (INSS, IRRF, FGTS) | Compatível com tabela 03 do e-Social |
| **FolhaMensal** | Período, funcionário, rubricas aplicadas, total vencimentos, total descontos, líquido | Cálculo progressivo de IRRF; teto INSS; base FGTS |
| **EventoTrabalhista** | Tipo (S-2200/2230/2299), data, funcionário, dados específicos | Validação de prazos legais; sequenciamento de eventos |
| **LoteESocial** | Eventos agrupados, protocolo, status (pendente/enviado/processado/erro) | Controle de sequência; retificação; exclusão |
| **ProcessamentoFolha** | Período, empresa, status, logs de execução | Idempotência; reprocessamento parcial |

**Regra**: NÃO depende de frameworks externos. É o coração do sistema. As regras fiscais e trabalhistas vivem aqui.

### 4. Infrastructure (Infraestrutura)
**Responsabilidade**: Persistência, mensageria, integração externa, cache, logging.
**Tecnologia**: Entity Framework Core, PostgreSQL, RabbitMQ/Kafka, Redis, Serilog

| Componente | Descrição |
|---|---|
| **Folha360DbContext** | EF Core DbContext com mapeamento para PostgreSQL |
| **Repositories** | Implementações de `IFuncionarioRepository`, `IFolhaRepository`, etc. |
| **eSocialMessageBus** | Publicação/consumo de eventos e-Social via RabbitMQ |
| **eSocialHttpClient** | Chamadas HTTP ao gateway do e-Social (envio de lotes, consulta de recibos) |
| **CacheService** | Cache Redis para rubricas, tabelas progressivas (IRRF), resultados de cálculo |
| **CryptoService** | Criptografia em repouso (AES-256) para dados sensíveis (documentos, valores) — LGPD |
| **AuditLog** | Trilha de auditoria para todas as operações de escrita (LGPD) |

## Regras de Dependência

```
Presentation ──► Application ──► Domain ◄── Infrastructure
                                      ▲
                                      │
                              (via interfaces/DIP)
```

- **Allowed**: Presentation → Application → Domain; Infrastructure → Domain (implementando interfaces)
- **Forbidden**: Domain → Infrastructure; Domain → Application; Application → Presentation
- **DIP (Inversion)**: Application define interfaces (`IRepository`, `IMessageBus`); Infrastructure implementa

## Cross-Cutting Concerns

| Concern | Onde Reside | Mecanismo |
|---|---|---|
| **Autenticação/Autorização** | Presentation (middleware) | JWT + políticas ASP.NET |
| **Logging** | Infrastructure | Serilog → Elasticsearch / Seq |
| **Validação** | Application (input) + Domain (negócio) | FluentValidation + Domain guards |
| **Resiliência** | Infrastructure | Polly (retry, circuit breaker) para chamadas e-Social |
| **Criptografia** | Infrastructure | AES-256 + Hash SHA-256 para senhas |
| **Auditoria** | Infrastructure | Interceptor EF Core + AuditLog table |
| **Health Checks** | Presentation + Infrastructure | ASP.NET Health Checks + probes |

## Evidence vs Assumptions

**Evidências (baseadas no contexto)**:
- Stack definida: .NET + PostgreSQL + React
- Necessidade de compatibilidade com layouts e-Social v. S-1.3
- Sistema distribuído e modular

**Assumptions**:
- Times de desenvolvimento separados por módulo (~3-5 devs por módulo)
- Implantação on-premise ou cloud privada (dados sensíveis de RH)
- Volume de 100K funcionários com pico de processamento no fechamento mensal

## Riscos

| Risco | Severidade | Mitigação |
|---|---|---|
| Acoplamento entre módulos de domínio | Média | Agregados bem definidos; eventos de domínio para comunicação entre módulos |
| Complexidade de sincronização com e-Social | Alta | Camada de infraestrutura dedicada; filas de retry; idempotência |
| Performance do cálculo para 100K+ funcionários | Alta | Processamento paralelo; cache de tabelas; batch processing |

## Recommended Next Skill
`component-boundary-reviewer` — para definir as fronteiras explícitas entre os 6 módulos de domínio.
