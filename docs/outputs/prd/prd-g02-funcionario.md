# PRD G02 — Módulo Funcionário

## Visão Geral

O módulo Funcionário é o coração do cadastro de pessoas no Folha360. Gerencia todos os dados 
do trabalhador: identificação pessoal, documentos, contrato de trabalho, dependentes, 
remuneração, benefícios, dados bancários, movimentações, afastamentos e informações e-Social.

**Problema resolvido**: O DP precisa de um cadastro completo e unificado do funcionário, 
eliminando planilhas paralelas e garantindo que todos os dados necessários para o cálculo da 
folha, obrigações fiscais e envio ao e-Social estejam em um único local.

**Público-alvo**: Operador DP (cadastro e manutenção), Consulta (visualização de dados próprios).

## Objetivos

- **Cadastro completo** em 15-20 minutos por funcionário (todas as 10 seções)
- **Reduzir erros de folha** garantindo que dados de contrato, dependentes e remuneração 
  estejam consistentes antes do processamento
- **Conformidade LGPD**: CPF, salário e PIX criptografados em repouso (AES-256-GCM)
- **Rastreabilidade**: Soft delete + auditoria em todas as operações
- **e-Social ready**: Dados estruturados para geração automática de eventos S-2200, S-2206, S-2300

## Histórias de Usuário

1. **Como Operador DP**, quero cadastrar um funcionário com nome, CPF, data de admissão, 
   cargo e lotação para iniciar o vínculo empregatício no sistema.
2. **Como Operador DP**, quero adicionar documentos (CTPS, RG, PIS/PASEP) com datas de 
   emissão e validade para manter o dossiê digital completo.
3. **Como Operador DP**, quero configurar o contrato de trabalho com tipo, salário, horário 
   e sindicato para que a folha seja calculada corretamente.
4. **Como Operador DP**, quero cadastrar dependentes para IRRF e salário-família para que 
   os descontos e benefícios sejam aplicados corretamente.
5. **Como Operador DP**, quero configurar remuneração e benefícios (VT, VR, plano de saúde) 
   para que os valores sejam calculados na folha.
6. **Como Operador DP**, quero cadastrar dados bancários (conta principal e PIX) para que o 
   pagamento seja realizado corretamente.
7. **Como Operador DP**, quero lançar movimentações fixas e mensais (horas extras, comissões) 
   para alimentar o cálculo da folha.
8. **Como Operador DP**, quero registrar afastamentos (doença, maternidade, acidente) com CID 
   para que a folha reflita corretamente o período.
9. **Como Operador DP**, quero preencher informações e-Social (PCD, reservista, primeiro 
   emprego) para conformidade na transmissão.
10. **Como Funcionário (Consulta)**, quero visualizar meus dados cadastrais, holerites e 
    eventos para acompanhar minha vida funcional.

## Funcionalidades Principais

### F1. Dados Pessoais
**O que faz**: CRUD dos dados de identificação do funcionário.

**Requisitos funcionais**:
- RF01: O sistema DEVE permitir cadastrar funcionário com nome, CPF (11 dígitos), data de 
  admissão, cargo, lotação e salário base
- RF02: O sistema DEVE validar CPF (dígitos verificadores) antes de persistir
- RF03: O sistema DEVE criptografar CPF em repouso (AES-256-GCM)
- RF04: O sistema DEVE retornar CPF mascarado (`***.456.789-**`) nas consultas
- RF05: O sistema DEVE permitir filtrar por empresa, status, cargo, lotação e nome
- RF06: O sistema DEVE armazenar dados de endereço residencial e contatos

### F2. Documentos
**O que faz**: CRUD de documentos do funcionário com upload de anexos.

**Requisitos funcionais**:
- RF07: O sistema DEVE permitir múltiplos documentos por funcionário
- RF08: O sistema DEVE classificar por tipo (CPF/RG/CNH/CTPS/PIS-PASEP/NIS-NIT/Título/Certidão/RNE-CIE)
- RF09: O sistema DEVE armazenar número do documento criptografado
- RF10: O sistema DEVE permitir anexar arquivo comprobatório (via MinIO)

### F3. Contrato de Trabalho
**O que faz**: Gerenciar o vínculo empregatício com todos os parâmetros contratuais.

**Requisitos funcionais**:
- RF11: O sistema DEVE suportar tipos de contrato: CLT Indeterminado, Determinado, Experiência, 
  Aprendiz, Estágio, Intermitente, Temporário, PJ, Cooperado, Autônomo
- RF12: O sistema DEVE suportar tipos de salário: Mensalista, Horista, Diarista, Semanalista, Tarefa
- RF13: O sistema DEVE vincular contrato a horário de trabalho, sindicato e categoria do trabalhador
- RF14: O sistema DEVE armazenar data de término para contratos determinados

### F4. Dependentes
**O que faz**: CRUD de dependentes para IRRF, salário-família e pensão alimentícia.

**Requisitos funcionais**:
- RF15: O sistema DEVE classificar dependente por tipo (Filho/Enteado/Cônjuge/Pais/Irmão/Curatela/Pensão)
- RF16: O sistema DEVE permitir marcar dependente para IRRF e/ou salário-família
- RF17: O sistema DEVE suportar pensão alimentícia com valor fixo ou percentual

### F5. Remuneração e Benefícios
**O que faz**: Configurar remuneração base e benefícios do funcionário.

**Requisitos funcionais**:
- RF18: O sistema DEVE armazenar adicionais: insalubridade, periculosidade, noturno, transferência
- RF19: O sistema DEVE gerenciar benefícios: VT, VR/VA, plano saúde, odontológico, seguro vida, 
  previdência privada (cada um com flag booleano e valor)

### F6. Dados Bancários
**O que faz**: Gerenciar contas bancárias do funcionário para pagamento.

**Requisitos funcionais**:
- RF20: O sistema DEVE permitir múltiplas contas bancárias por funcionário
- RF21: O sistema DEVE permitir marcar uma conta como principal
- RF22: O sistema DEVE criptografar chave PIX

### F7. Movimentação Fixa e Mensal
**O que faz**: Lançar rubricas recorrentes e específicas do mês.

**Requisitos funcionais**:
- RF23: O sistema DEVE permitir lançar rubricas fixas mensais (valor e quantidade)
- RF24: O sistema DEVE permitir lançar rubricas específicas por mês/ano (MM/AAAA)

### F8. Afastamentos
**O que faz**: Registrar afastamentos temporários do funcionário.

**Requisitos funcionais**:
- RF25: O sistema DEVE suportar tipos: Doença, Acidente Trabalho, Maternidade, Paternidade, 
  Serviço Militar, Mandato Sindical, Suspensão, Férias, Licença, Outros
- RF26: O sistema DEVE armazenar data início, data fim prevista e data fim efetiva
- RF27: O sistema DEVE permitir informar número do atestado/CID

### F9. Informações e-Social
**O que faz**: Dados complementares exigidos pelo e-Social.

**Requisitos funcionais**:
- RF28: O sistema DEVE armazenar indicador de deficiência (PCD), tipo e data do laudo
- RF29: O sistema DEVE armazenar flags: reservista, primeiro emprego, trabalhador aposentado
- RF30: O sistema DEVE armazenar registro profissional quando aplicável

## Experiência do Usuário

**Persona primária**: Operador DP — realiza cadastro completo em fluxo sequencial. Precisa de 
formulário dividido em abas/seções: Dados Pessoais → Documentos → Contrato → Dependentes → 
Remuneração → Bancários → Movimentações → Afastamentos → e-Social.

**Persona secundária**: Funcionário (Consulta) — visualiza apenas seus próprios dados. Precisa 
de interface limpa e simplificada, sem controles de edição.

**Acessibilidade**: Labels visíveis em todos os campos, validação inline, navegação por teclado, 
suporte a leitores de tela (aria-labels em campos com dados sensíveis mascarados).

## Restrições Técnicas de Alto Nível

- **Segurança**: CPF, salário base e chave PIX criptografados com AES-256-GCM
- **Multi-tenancy**: Dados isolados por schema; cada funcionário pertence a uma empresa
- **Auditoria**: Soft delete + `audit_log` em todas as tabelas
- **Integração**: Cargo referencia `CboOcupacao`; lotação referencia `Lotacao`; município 
  referencia `MunicipioIBGE`; banco referencia `BancoFebraban`
- **Eventos de domínio**: `FuncionarioCadastrado` publicado via RabbitMQ
- **Validação**: FluentValidation em todos os commands; CPF validado por algoritmo

## Fora de Escopo

- Cadastro de empresas (módulo Empresa — G01)
- Cadastro de cargos (módulo Cargo — G03)
- Configuração de rubricas (módulo Rubricas — G05)
- Processamento da folha (módulo Folha — G07)
- Portal do funcionário com autoatendimento (feature futura)

---

## Anexo A — Referência de Endpoints

Ver documento completo: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 2.

| Subgrupo | Controller | Base Path | Endpoints |
|----------|-----------|-----------|-----------|
| 2.1 Dados Pessoais | `FuncionariosController` | `/api/funcionarios` | GET list, GET by id, POST, PUT, DELETE |
| 2.2 Documentos | `DocumentosController` | `/api/documentos` | GET list, GET by id, POST, PUT, DELETE |
| 2.3 Contrato | `FuncionariosContratoController` | `/api/funcionarios/{id}/contrato` | GET, POST, PUT |
| 2.4 Dependentes | `DependentesController` | `/api/dependentes` | GET list, GET by id, POST, PUT, DELETE |
| 2.5 Remuneração | `FuncionariosRemuneracaoController` | `/api/funcionarios/{id}/remuneracao` | GET, PUT |
| 2.6 Bancários | `FuncionariosDadosBancariosController` | `/api/funcionarios/{id}/dados-bancarios` | GET list, GET by id, POST, DELETE |
| 2.7 Mov. Fixa | `FuncionariosMovimentacaoFixaController` | `/api/funcionarios/{id}/movimentacao-fixa` | GET list, GET by id, POST, DELETE |
| 2.8 Mov. Mensal | `FuncionariosMovimentacaoMensalController` | `/api/funcionarios/{id}/movimentacao-mensal` | GET list, POST, DELETE |
| 2.9 Afastamentos | `FuncionariosAfastamentosController` | `/api/funcionarios/{id}/afastamentos` | GET list, GET by id, POST, DELETE |
| 2.10 Info e-Social | `FuncionariosInfoESocialController` | `/api/funcionarios/{id}/info-esocial` | GET, PUT |

## Anexo B — Jornada de Referência

Ver documento completo: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 2 (Operador DP — Cadastro de Funcionário).
