# PRD G01 — Módulo Empresa

## Visão Geral

O módulo Empresa é a raiz do sistema Folha360. Gerencia todos os dados cadastrais, fiscais e 
configurações do empregador — a entidade central a partir da qual todos os demais cadastros 
(funcionários, rubricas, lotações) e processos (folha, tributos, e-Social) são organizados.

**Problema resolvido**: Departamentos Pessoais e escritórios contábeis precisam de uma fonte 
única da verdade para os dados da empresa, com todas as configurações fiscais, tributárias e 
de e-Social centralizadas, eliminando retrabalho e inconsistências entre sistemas.

**Público-alvo**: Admin (setup inicial), Operador DP (manutenção), Contador (configurações fiscais).

## Objetivos

- **Centralizar dados da empresa** em um único local com rastreabilidade completa (auditoria)
- **Reduzir erros de configuração fiscal** que geram multas e retrabalhos no e-Social
- **Suportar multi-estabelecimentos** (matriz, filiais, obras) com lotações independentes
- **Tempo de cadastro**: Setup inicial completo em menos de 2 horas
- **Conformidade**: 100% de aderência aos campos obrigatórios do e-Social (S-1000, S-1005)

## Histórias de Usuário

1. **Como Admin**, quero cadastrar uma nova empresa com CNPJ, razão social e regime tributário 
   para que o sistema possa operar para este empregador.
2. **Como Admin**, quero configurar o ambiente e-Social e fazer upload do certificado digital 
   para que os eventos possam ser transmitidos ao governo.
3. **Como Operador DP**, quero cadastrar múltiplas lotações (matriz, filiais, obras) para 
   classificar corretamente os funcionários no e-Social.
4. **Como Operador DP**, quero gerenciar endereços e contatos da empresa por tipo 
   (fiscal, cobrança, entrega) para atender diferentes finalidades.
5. **Como Contador**, quero configurar parâmetros gerais (dia de pagamento, % adiantamento) 
   para que a folha seja processada com as regras corretas.
6. **Como Operador DP**, quero registrar processos administrativos e judiciais vinculando 
   rubricas afetadas para que o cálculo da folha reflita decisões judiciais.

## Funcionalidades Principais

### F1. Identificação da Empresa
**O que faz**: CRUD completo dos dados cadastrais da empresa (CNPJ, razão social, nome fantasia, 
CNAE, natureza jurídica, porte, matriz/filial).

**Requisitos funcionais**:
- RF01: O sistema DEVE permitir cadastrar empresa com CNPJ (14 dígitos), razão social e regime tributário
- RF02: O sistema DEVE validar CNPJ (dígitos verificadores) antes de persistir
- RF03: O sistema DEVE impedir duplicidade de CNPJ no mesmo tenant
- RF04: O sistema DEVE permitir filtrar empresas por CNPJ, razão social e regime tributário
- RF05: O sistema DEVE registrar data de criação e atualização em todas as operações

### F2. Lotação
**O que faz**: Gerenciar estabelecimentos da empresa (matriz, filiais, obras) com código, 
descrição e tipo e-Social.

**Requisitos funcionais**:
- RF06: O sistema DEVE permitir múltiplas lotações por empresa
- RF07: O sistema DEVE classificar lotação por tipo e-Social (Matriz/Filial/Obra/Estabelecimento/Unidade/Gerencial)
- RF08: O sistema DEVE permitir filtrar lotações por empresa, código e descrição

### F3. Endereços
**O que faz**: Gerenciar múltiplos endereços da empresa por tipo (principal, fiscal, cobrança, 
entrega, obra) com referência ao município IBGE.

**Requisitos funcionais**:
- RF09: O sistema DEVE permitir múltiplos endereços por empresa
- RF10: O sistema DEVE classificar endereço por tipo (Principal/Fiscal/Cobrança/Entrega/Obra)
- RF11: O sistema DEVE vincular endereço ao município IBGE via `MunicipioId`

### F4. Contatos
**O que faz**: Gerenciar contatos da empresa por tipo (sócio, contador, RH, preposto) com 
vigência e dados de comunicação.

**Requisitos funcionais**:
- RF12: O sistema DEVE permitir múltiplos contatos por empresa
- RF13: O sistema DEVE classificar contato por tipo (Diretor/Gerente/Sócio/Procurador/Contador/RH/TI/Preposto)
- RF14: O sistema DEVE permitir definir vigência (data início/fim) para cada contato
- RF15: O sistema DEVE permitir marcar um contato como principal

### F5. Configurações Fiscais
**O que faz**: Parâmetros tributários integrados à entidade Empresa (regime, FPAS, código 
terceiros, classificação tributária).

**Requisitos funcionais**:
- RF16: O sistema DEVE armazenar regime tributário (Simples Nacional/Lucro Presumido/Lucro Real)
- RF17: O sistema DEVE armazenar FPAS, código de terceiros e classificação tributária

### F6. Configuração e-Social
**O que faz**: Parâmetros específicos para transmissão de eventos ao e-Social.

**Requisitos funcionais**:
- RF18: O sistema DEVE permitir configurar ambiente (Produção/Produção Restrita)
- RF19: O sistema DEVE armazenar tipo de certificado digital (A1/A3) e vencimento
- RF20: O sistema DEVE armazenar versão do layout, código transmissor e grupo e-Social (1-4)

### F7. Configurações Bancárias
**O que faz**: Gerenciar contas bancárias da empresa por finalidade (folha, tributos, fornecedor).

**Requisitos funcionais**:
- RF21: O sistema DEVE permitir múltiplas contas bancárias por empresa
- RF22: O sistema DEVE classificar conta por finalidade (Folha/Tributos/Fornecedor/Geral)
- RF23: O sistema DEVE criptografar chave PIX (AES-256-GCM)

### F8. Configurações Gerais
**O que faz**: Parâmetros chave-valor para regras de negócio (dia pagamento, % adiantamento).

**Requisitos funcionais**:
- RF24: O sistema DEVE permitir configuração por chave-valor (Dictionary)
- RF25: O sistema DEVE suportar chaves: DiaPagamento, PercentualAdiantamento, PercentualVT, 
  AbonoPecuniario, ToleranciaPonto

### F9. Processos Administrativos/Judiciais
**O que faz**: Registrar processos que impactam o cálculo da folha, vinculando rubricas afetadas.

**Requisitos funcionais**:
- RF26: O sistema DEVE permitir cadastrar processos com número, tipo e órgão
- RF27: O sistema DEVE permitir vincular múltiplas rubricas a um processo (N:N)
- RF28: O sistema DEVE filtrar processos por empresa e tipo (Administrativo/Judicial)

## Experiência do Usuário

**Persona primária**: Admin — realiza setup inicial único. Precisa de wizard/step-by-step 
para não esquecer configurações obrigatórias (ex: certificado digital antes de enviar e-Social).

**Persona secundária**: Operador DP — faz manutenção contínua. Precisa de acesso rápido 
aos sub-recursos (endereços, contatos, bancários) a partir da tela da empresa.

**Fluxo principal**: Login → Selecionar empresa → Abas: Identificação / Lotação / Endereços / 
Contatos / Fiscal / e-Social / Bancário / Geral / Processos.

**Acessibilidade**: Todos os formulários devem ter labels visíveis, validação inline e 
navegação por teclado. Contraste mínimo 4.5:1.

## Restrições Técnicas de Alto Nível

- **Multi-tenancy**: Dados isolados por schema (schema-per-tenant)
- **Segurança**: CNPJ armazenado em texto plano (identificador público); CPF de contatos 
  criptografado (AES-256-GCM); chave PIX criptografada
- **Auditoria**: Todas as operações de escrita registradas em `audit_log`
- **Integração**: Municípios referenciam tabela IBGE (`municipio_ibge`); bancos referenciam 
  tabela Febraban (`banco_febraban`)
- **Validação**: FluentValidation em todos os commands

## Fora de Escopo

- Cadastro de funcionários (módulo Funcionário — G02)
- Cadastro de cargos (módulo Cargo — G03)
- Envio de eventos e-Social (módulo e-Social — G12)
- Processamento da folha (módulo Folha — G07)
- Relatórios e exportações (módulo Relatórios — G13)

---

## Anexo A — Referência de Endpoints

Ver documento completo: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 1.

| Subgrupo | Controller | Base Path | Endpoints |
|----------|-----------|-----------|-----------|
| 1.1 Identificação | `EmpresasController` | `/api/empresas` | GET list, GET by id, POST, PUT, DELETE |
| 1.2 Lotação | `LotacoesController` | `/api/lotacoes` | GET list, GET by id, POST, PUT, DELETE |
| 1.3 Endereço | `EmpresasEnderecosController` | `/api/empresas/{id}/enderecos` | GET list, GET by id, POST, DELETE |
| 1.4 Contato | `EmpresasContatosController` | `/api/empresas/{id}/contatos` | GET list, GET by id, POST, DELETE |
| 1.5 Fiscal | (integrado à Empresa) | `PUT /api/empresas/{id}` | Via AtualizarEmpresaCommand |
| 1.6 e-Social | `EmpresasConfigESocialController` | `/api/empresas/{id}/config-esocial` | GET, PUT |
| 1.7 Bancário | `EmpresasConfiguracoesBancariasController` | `/api/empresas/{id}/configuracoes-bancarias` | GET list, GET by id, POST, DELETE |
| 1.8 Geral | `EmpresasConfiguracoesGeraisController` | `/api/empresas/{id}/configuracoes-gerais` | GET, PUT (Dictionary) |
| 1.9 Processos | `ProcessosAdministrativosController` | `/api/processos-administrativos` | GET list, GET by id, POST, PUT, DELETE + rubricas |

## Anexo B — Jornada de Referência

Ver documento completo: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 1 (Admin — Setup Inicial).
