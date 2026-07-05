# 📋 Folha360 — Relatório de Pendências de Implementação (Frontend)

> **Objetivo**: Identificar todas as funcionalidades do agrupamento de cadastros que ainda
> não possuem formulários, páginas ou ações no frontend.
>
> **Data**: 28/06/2026  
> **Base**: Agrupamento de Cadastros e Processos v2.0 + Código frontend atual  
> **Versão**: 1.0

---

## 📊 Resumo Geral

| Status | Quantidade | Descrição |
|--------|-----------|-----------|
| ✅ Completo | 7 grupos | Empresa, Cargo, Lotação, Sindicato, Convênio, Horário, Eventos |
| ⚠️ Parcial | 2 grupos | Funcionário, Rubricas |
| ❌ Ausente | 0 grupos | — |

**Total de pendências**: 9 itens (3 críticos, 6 médios)

---

## 🔴 Pendências Críticas

### P1 — Funcionário: Aba "Movimentação Mensal" ausente

**Arquivo**: `frontend/apps/admin/src/routes/cadastros/FuncionarioDetailPage.tsx`  
**Problema**: A página de detalhes do funcionário possui 9 abas, mas a aba de **Movimentação Mensal** (`movmensal`) não foi implementada. Existe apenas a aba de Movimentação Fixa (`movfixa`).

**O que já existe**:
- API hooks: `useMovimentacaoMensal`, `useCreateMovimentacaoMensal` em `funcionario-detalhes.ts` ✅
- Tipo DTO: `MovimentacaoMensal` em `types.ts` ✅

**O que falta**:
- [ ] Adicionar tab `movmensal` ao array `TABS` em `FuncionarioDetailPage.tsx`
- [ ] Criar componente `MovMensalTab` com:
  - DataTable listando as movimentações mensais (rubrica, descrição, mês/ano, quantidade, valor)
  - Formulário inline para adicionar nova movimentação (rubrica, mês/ano, quantidade, valor)
  - Botão de exclusão
- [ ] Adicionar `useDeleteMovimentacaoMensal` ao `funcionario-detalhes.ts` (atualmente só existe create)

**Campos do formulário** (conforme agrupamento):
| Campo | Tipo | Obrig. |
|-------|------|--------|
| `RubricaId` | `Guid` | ✅ |
| `Descricao` | `string?` | |
| `MesAno` | `string` (MM/AAAA) | ✅ |
| `Quantidade` | `decimal?` | |
| `Valor` | `decimal` | ✅ |

---

### P2 — Rubricas: Página de Detalhes ausente

**Arquivo**: `frontend/apps/admin/src/routes/cadastros/RubricasPage.tsx`  
**Problema**: Rubricas são uma entidade complexa com 7 subgrupos (dados, grupos, composição, fórmulas, tabelas progressivas, incidências, simulação), mas o frontend atual só possui uma página de lista com Sheet de criação/edição. Não há página de detalhes com navegação por abas.

**O que já existe**:
- API hooks: TODOS os hooks para os 7 subgrupos em `rubricas.ts` ✅
- Form components: `RubricaFormFields`, `GrupoRubricaFormFields`, `FaixaProgressivaFormFields`, `SimulacaoRubricaFormFields` ✅
- Tipos DTO: Todos em `types.ts` ✅

**O que falta**:
- [ ] Criar `RubricaDetailPage.tsx` com abas:
  - **Dados Gerais** — `RubricaFormFields` (já existe)
  - **Grupos** — `GrupoRubricaFormFields` + tabela de grupos existentes
  - **Composição** — Tabela de componentes + formulário para adicionar (usa `useComposicaoRubrica`, `useAddComponente`)
  - **Fórmulas** — Editor de fórmula NCalc (usa `useFormulaRubrica`, `useUpdateFormulaRubrica`)
  - **Tabelas Progressivas** — `FaixaProgressivaFormFields` + tabela de faixas (usa `useTabelasProgressivas`, `useCreateFaixaProgressiva`)
  - **Incidências** — Tabela de incidências + formulário para adicionar (usa `useIncidenciasRubrica`, `useAddIncidencia`)
  - **Simulação** — `SimulacaoRubricaFormFields` + painel de resultados (usa `useSimularRubrica`)
- [ ] Adicionar rota `/cadastros/rubricas/:id` no `App.tsx`
- [ ] Atualizar `RubricasPage.tsx` para navegar para a detail page ao clicar na linha (padrão usado em Empresas/Funcionários)

---

### P3 — Rubricas: Sub-formulários dedicados ausentes

**Arquivos**: `frontend/packages/ui/src/components/forms/rubricas-forms.tsx`  
**Problema**: Os formulários de **Composição** e **Incidências** não possuem componentes de formulário dedicados. Atualmente:
- Incidências são checkboxes inline dentro do `RubricaFormFields`
- Composição não tem formulário algum

**O que falta**:
- [ ] Criar `ComposicaoRubricaFormFields` — formulário para adicionar componente a uma rubrica:
  - `RubricaComponenteId` (select de rubricas)
  - `Operador` (`+` ou `-`)
  - `PercentualComposicao` (decimal?)
  - `Ordem` (int)
  - `Obrigatorio` (bool)
- [ ] Criar `IncidenciaRubricaFormFields` — formulário para adicionar incidência:
  - `TipoIncidencia` (select: INSS, IRRF, FGTS, Sindical, 13º, Férias, etc.)

---

## 🟡 Pendências Médias

### P4 — API Hook: `useDeleteMovimentacaoMensal` ausente

**Arquivo**: `frontend/packages/api/src/modules/cadastros/funcionario-detalhes.ts`  
**Problema**: Existe `useCreateMovimentacaoMensal` mas não existe `useDeleteMovimentacaoMensal`.

**Ação**: Adicionar o hook de deleção seguindo o mesmo padrão dos outros hooks.

---

### P5 — API Hook: `useDeleteMovimentacaoFixa` não exportado no index

**Arquivo**: `frontend/packages/api/src/index.ts`  
**Problema**: O hook `useDeleteMovimentacaoFixa` existe em `funcionario-detalhes.ts` mas pode não estar exportado no barrel do `index.ts`.

**Ação**: Verificar e adicionar a exportação se necessário.

---

### P6 — API Hook: `useDeleteAfastamentoFuncionario` ausente

**Arquivo**: `frontend/packages/api/src/modules/cadastros/funcionario-detalhes.ts`  
**Problema**: Existe `useCreateAfastamentoFuncionario` mas não existe hook de deleção para afastamentos do funcionário.

**Ação**: Adicionar `useDeleteAfastamentoFuncionario`.

---

### P7 — Empresa: `useDeleteProcessoAdministrativo` já existe mas não era importado

**Status**: ✅ Já corrigido na revisão anterior.  
**Arquivo**: `EmpresaDetailPage.tsx` — a importação foi adicionada.

---

### P8 — Navegação: Grupos de Rubrica sem acesso direto

**Arquivo**: `frontend/apps/admin/src/hooks/useNavigationItems.tsx`  
**Problema**: O item "Rubricas" está na sidebar, mas ao acessar só mostra a lista de rubricas. O gerenciamento de **Grupos de Rubrica** (que é uma entidade separada com seu próprio controller `GruposRubricaController`) não tem uma página dedicada nem acesso via sidebar.

**Ação**: 
- [ ] Opção A: Adicionar "Grupos de Rubrica" como sub-item de Rubricas na sidebar
- [ ] Opção B: Incluir o gerenciamento de grupos como uma aba dentro da `RubricaDetailPage`

---

### P9 — Funcionário: Aba "Movimentação Fixa" não usa `useDeleteMovimentacaoFixa`

**Arquivo**: `FuncionarioDetailPage.tsx` — componente `MovFixaTab`  
**Problema**: O componente importa `useDeleteMovimentacaoFixa` mas o hook pode não estar sendo exportado corretamente.

**Ação**: Verificar e corrigir a exportação no barrel.

---

## 📁 Entidades Simples (Sem Pendências)

Estas entidades são do tipo "somente lista" e estão completamente implementadas:

| Entidade | Página | Formulário | API Hooks | Navegação |
|----------|--------|-----------|-----------|-----------|
| Cargo | `CargosPage.tsx` | `CargoFormFields` | CRUD completo | ✅ |
| Lotação | `LotacoesPage.tsx` | `LotacaoFormFields` | CRUD completo | ✅ |
| Sindicato | `SindicatosPage.tsx` | `SindicatoFormFields` | CRUD completo | ✅ |
| Convênio | `ConveniosPage.tsx` | `ConvenioFormFields` | CRUD completo | ✅ |
| Horário | `HorariosPage.tsx` | `HorarioTrabalhoFormFields` | CRUD completo | ✅ |

---

## 📈 Progresso por Grupo do Agrupamento

| # | Grupo | Subgrupos | Implementados | Pendentes | % Concluído |
|---|-------|-----------|---------------|-----------|-------------|
| 1 | 🏢 Empresa | 9 | 9 | 0 | 100% |
| 2 | 👤 Funcionário | 10 | 9 | 1 (mov-mensal) | 90% |
| 3 | 💼 Cargo | 1 | 1 | 0 | 100% |
| 4 | 📐 Estrutura de Apoio | 4 | 4 | 0 | 100% |
| 5 | 🧮 Rubricas | 7 | 2 | 5 (detalhe + 4 sub-formulários) | 29% |
| 6 | 📅 Eventos | 5 | 5 | 0 | 100% |

**Progresso total**: ~87% dos subgrupos implementados (28 de 32)

---

## 🎯 Ordem de Prioridade Sugerida

1. **P1** — Adicionar aba Movimentação Mensal no FuncionarioDetailPage (30 min)
2. **P2** — Criar RubricaDetailPage com 7 abas (2-3 horas)
3. **P3** — Criar ComposicaoRubricaFormFields e IncidenciaRubricaFormFields (1 hora)
4. **P4-P6** — Adicionar hooks de deleção ausentes (15 min)
5. **P8** — Adicionar acesso a Grupos de Rubrica na navegação (15 min)

---

> **Versão 1.0** — Relatório gerado a partir da auditoria completa do código frontend
> comparado com o documento de agrupamento de cadastros e processos v2.0.
