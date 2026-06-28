# PRD G06 — Módulo Eventos Trabalhistas

## Visão Geral

O módulo Eventos Trabalhistas gerencia todos os eventos da vida funcional do trabalhador: 
admissão, desligamento, férias, afastamentos e alterações contratuais. Cada evento gera 
um registro auditável e, quando aplicável, o XML correspondente para envio ao e-Social.

**Problema resolvido**: O DP precisa de um registro cronológico e rastreável de todos os 
eventos da vida funcional, com dados estruturados para geração automática de eventos e-Social 
(S-2200, S-2206, S-2230, S-2299, S-2300, S-2306, S-2399).

**Público-alvo**: Operador DP (registro de eventos), Contador (validação e-Social).

## Objetivos

- **Rastreabilidade completa**: Timeline de eventos por funcionário
- **e-Social ready**: Dados estruturados para geração de XML de eventos
- **Consistência**: Validações de negócio (ex: não permitir admissão sem cargo; não permitir 
  férias sem período aquisitivo)
- **Tempo de registro**: Menos de 5 minutos por evento

## Histórias de Usuário

1. **Como Operador DP**, quero registrar uma admissão com data, cargo e salário inicial para 
   formalizar o vínculo e gerar o evento S-2200.
2. **Como Operador DP**, quero programar férias com período aquisitivo, data de início e dias 
   de gozo para gerar o evento S-2230.
3. **Como Operador DP**, quero registrar afastamentos (doença, maternidade, acidente) com CID 
   para gerar o evento S-2230/S-2231.
4. **Como Operador DP**, quero registrar alterações contratuais (promoção, mudança de salário) 
   para gerar o evento S-2206/S-2306.
5. **Como Operador DP**, quero registrar um desligamento com motivo e verbas rescisórias para 
   gerar o evento S-2299/S-2399.
6. **Como Operador DP**, quero visualizar a timeline completa de eventos de um funcionário 
   para auditoria e acompanhamento.

## Funcionalidades Principais

### F1. Admissão
**Requisitos funcionais**:
- RF01: CRUD de admissões com funcionário, empresa, data, cargo, salário inicial e tipo de contrato
- RF02: O sistema DEVE suportar período de experiência em meses
- RF03: O sistema DEVE armazenar XML do evento e-Social gerado

### F2. Desligamento
**Requisitos funcionais**:
- RF04: CRUD de desligamentos com data, motivo (7 tipos) e verbas rescisórias
- RF05: O sistema DEVE suportar motivos: Sem Justa Causa, Com Justa Causa, Pedido Demissão, 
  Término Contrato, Acordo Mútuo, Aposentadoria, Morte

### F3. Férias
**Requisitos funcionais**:
- RF06: CRUD de férias com data início, dias de gozo e período aquisitivo (início/fim)
- RF07: O sistema DEVE suportar tipos: Normais, Coletivas, Antecipadas, Dobro

### F4. Afastamentos (Eventos)
**Requisitos funcionais**:
- RF08: CRUD de afastamentos com data início, data fim prevista/efetiva e tipo
- RF09: O sistema DEVE suportar tipos: Doença, Acidente Trabalho, Maternidade, Paternidade, 
  Serviço Militar, Suspensão
- RF10: O sistema DEVE armazenar CID criptografado

### F5. Alterações Contratuais
**Requisitos funcionais**:
- RF11: CRUD de alterações com data, campos alterados (JSON), valor anterior e valor novo

### F6. Timeline de Eventos
**Requisitos funcionais**:
- RF12: O sistema DEVE expor endpoint agregado com todos os eventos de um funcionário 
  em ordem cronológica

## Experiência do Usuário

**Fluxo principal**: Acessar ficha do funcionário → Aba "Eventos" → Timeline cronológica → 
Botão "Novo Evento" → Selecionar tipo → Preencher formulário específico → Salvar.

**Acessibilidade**: Timeline com navegação por teclado, formulários com validação inline.

## Restrições Técnicas de Alto Nível

- **Segurança**: CID criptografado em repouso
- **e-Social**: Cada evento armazena XMLContent para transmissão
- **Validação**: Regras de negócio como "não permitir férias sem período aquisitivo" e 
  "não permitir desligamento sem data"
- **Auditoria**: Soft delete + `audit_log`

## Fora de Escopo

- Cálculo de férias (módulo Férias — G08)
- Cálculo de rescisão (módulo Rescisão — G10)
- Envio dos eventos ao e-Social (módulo e-Social — G12)

---

## Anexo A — Referência de Endpoints

| Subgrupo | Controller | Base Path |
|----------|-----------|-----------|
| 6.1 Admissão | `AdmissoesController` | `/api/admissoes` |
| 6.2 Desligamento | `DesligamentosController` | `/api/desligamentos` |
| 6.3 Férias | `FeriasController` | `/api/ferias` |
| 6.4 Afastamento | `AfastamentosController` | `/api/afastamentos` |
| 6.5 Alt. Contratual | `AlteracoesContratuaisController` | `/api/alteracoes-contratuais` |
| 6.6 Timeline | `EventosTrabalhistasController` | `/api/eventos-trabalhistas/funcionario/{id}` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 6.  
Jornada: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 6.
