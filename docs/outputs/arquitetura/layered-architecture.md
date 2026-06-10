# Visão de Arquitetura em Camadas — Folha360

## Summary
Proposta de arquitetura em 4 camadas lógicas (Presentation, Application, Domain, Infrastructure) com dependência unidirecional em direção ao Domain, adaptada para o domínio de folha de pagamento compatível com e-Social v. S-1.3. A separação em camadas permite isolar regras de negócio fiscal/trabalhista das preocupações de infraestrutura (mensageria e-Social, persistência) e da interface com o usuário.

> **Atualização (Junho 2026)**: O subsistema de rubricas foi expandido com base na [pesquisa de mercado](../rubricas/pesquisa-mercado-rubricas.md) e [plano de ação](../rubricas/plano-acao-rubricas.md), introduzindo motor de cálculo com 4 fases, composição hierárquica de rubricas, editor visual drag-and-drop, e suporte a 11 tipos de cálculo de DP. Ver [ADR-006](./adr-006-subsistema-rubricas.md) para detalhes.

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
| `RubricasService` | CRUD de rubricas, grupos, composições, fórmulas, incidências, tabelas progressivas; validação de unicidade `(empresa_id, codigo)`; validação de `tipo_esocial` contra catálogo Tabela 03; versionamento e histórico de alterações |
| `FolhaService` | Iniciar cálculo da folha, orquestrar `MotorCalculo`, aplicar rubricas em 4 fases (vencimentos, bases, descontos, totais), gerar holerites, fechar período |
| `EventosTrabalhistasService` | Registrar admissão (S-2200), férias (S-2230), afastamentos (S-2230), desligamentos (S-2299) |
| `ObrigacoesFiscaisService` | Calcular IRRF, INSS, FGTS; gerar eventos S-5001/S-5002; apurar GPS |
| `RelatoriosService` | Gerar relatórios mensais, anuais, DIRF, RAIS; exportação CSV/PDF |
| `IntegracaoESocialService` | Enfileirar eventos, consultar recibos, reprocessar falhas, validar schemas XSD; gerar XML S-1010 (Tabela de Rubricas) e S-1070 (Processos Administrativos) |

**Regra**: Orquestra chamadas ao Domain. Não contém regras de negócio, apenas coordenação.

### 3. Domain (Domínio / Negócio)
**Responsabilidade**: Regras de negócio, entidades, value objects, agregados, eventos de domínio.
**Tecnologia**: .NET Domain Model (POCOs, agregados DDD)

| Agregado | Entidades | Regras |
|---|---|---|
| **Funcionario** | Dados pessoais, endereço, dependentes, documentos | Validação CPF, CTPS, PIS/PASEP; elegibilidade para eventos e-Social |
| **Empresa** | CNPJ, razão social, CNAE, regime tributário, FPAS | Classificação tributária; matriz/filiais |
| **Rubrica** | Código, natureza (Vencimento/Desconto/Beneficio/Informativo/Provisao/Base/Complemento/Reembolso/Estagio), incidências (INSS, IRRF, FGTS, contribuição sindical, 13º, férias, aviso prévio, rescisão, dissídio, salário-maternidade, auxílio-doença, adiantamento), tipo de cálculo (VALOR_FIXO/PERCENTUAL/HORA/FORMULA/COMPOSICAO/TABELA_PROGRESSIVA/UNIDADE/DIA/MEDIA/TETO/CONDICIONAL), teto/piso, vigência | Compatível com Tabela 03 do e-Social (S-1010); composição hierárquica via `rubrica_composicao`; fórmulas parametrizáveis via `rubrica_formula` com NCalc + sandbox; versionamento via `rubrica_historico` |
| **GrupoRubrica** | Código, descrição, natureza predominante | Agrupamento categórico para organização e relatórios (ex.: "Vencimentos Fixos", "Descontos Legais") |
| **RubricaComposicao** | Rubrica principal, rubrica componente, operador (+, -, *, /), percentual, obrigatoriedade | Define dependência hierárquica entre rubricas; detecção de ciclos via DFS |
| **RubricaFormula** | Expressão NCalc, parâmetros JSONB, versão | Fórmulas parametrizáveis com sandbox (timeout 100ms); whitelist de funções |
| **RubricaIncidencia** | Rubrica, tipo de incidência | Define sobre quais bases/tributos a rubrica incide |
| **RubricaTabelaProgressiva** | Faixas (de, até), alíquota, dedução | Tabelas progressivas de IRRF e INSS; versionável por ano |
| **RubricaHistorico** | Rubrica, dados anteriores (JSONB), motivo, usuário | Trilha de auditoria imutável para alterações em rubricas |
| **FolhaMensal** | Período, funcionário, rubricas aplicadas, total vencimentos, total descontos, líquido | Cálculo progressivo de IRRF; teto INSS; base FGTS; 4 fases do motor de cálculo |
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
| **CacheService** | Cache Redis para rubricas, tabelas progressivas (IRRF), resultados de cálculo; invalidação via pub/sub no evento `RubricaAlterada` |
| **MotorCalculo** | Motor de cálculo da folha com 4 fases (vencimentos, bases, descontos, totais); suporta 11 tipos de cálculo (mensal, férias, 13º, rescisão, dissídio, complementar, auxílio-doença, salário-maternidade, acordo, estágio, RPA) |
| **AvaliadorExpressao** | Avaliador de fórmulas NCalc com sandbox (timeout 100ms, memory limit); whitelist de funções permitidas |
| **ResolvedorComposicao** | Resolvedor de composição hierárquica de rubricas com detecção de ciclos (DFS) |
| **AplicadorTabelaProgressiva** | Aplicador de tabelas progressivas (IRRF, INSS) com suporte a múltiplas versões anuais |
| **CalculadorMedia** | Calculador de rubricas baseadas em média móvel (12 meses) para férias, 13º, rescisão |
| **AvaliadorCondicional** | Avaliador de rubricas condicionais (ex.: "aplicar se salário > X") |
| **CryptoService** | Criptografia em repouso (AES-256) para dados sensíveis (documentos, valores) — LGPD |
| **AuditLog** | Trilha de auditoria para todas as operações de escrita (LGPD) |
| **ValidadorXSD** | Validador de schemas XSD para eventos e-Social (S-1010, S-1070, S-1200, S-1210, S-2200, S-2230, S-2299, S-5001, S-5002) |

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
| **Validação** | Application (input) + Domain (negócio) | FluentValidation + Domain guards; validação de sintaxe de fórmulas; validação de conformidade e-Social (Tabela 03) |
| **Cache** | Infrastructure | Redis com invalidação pub/sub; TTL configurável por tipo de dado |
| **Simulação** | Application | Motor de cálculo em modo sandbox para simulação de rubricas em funcionário teste |
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
