# PRD G03 — Módulo Cargo

## Visão Geral

O módulo Cargo gerencia a estrutura de cargos e funções da empresa, vinculando cada posição 
à Classificação Brasileira de Ocupações (CBO). É pré-requisito para o cadastro de funcionários 
e para o envio correto de eventos ao e-Social.

**Problema resolvido**: O DP precisa de uma tabela de cargos padronizada com CBOs oficiais, 
evitando códigos inválidos que causam rejeição no e-Social e inconsistências na RAIS.

**Público-alvo**: Operador DP.

## Objetivos

- **Padronizar cargos** com CBOs oficiais (Tabela MTb)
- **Facilitar o cadastro** de funcionários com busca rápida de cargos
- **Conformidade e-Social**: 100% de cargos com CBO válido
- **Tempo de cadastro**: Menos de 2 minutos por cargo

## Histórias de Usuário

1. **Como Operador DP**, quero cadastrar um cargo com nome e CBO para vinculá-lo a funcionários.
2. **Como Operador DP**, quero buscar cargos por nome ou código CBO para localizar rapidamente.
3. **Como Operador DP**, quero definir faixa salarial (mínimo/máximo) para orientar contratações.

## Funcionalidades Principais

### F1. CRUD de Cargos
**Requisitos funcionais**:
- RF01: O sistema DEVE permitir cadastrar cargo com nome, CBO (6 dígitos) e descrição da função
- RF02: O sistema DEVE validar CBO contra tabela oficial (`cbo_ocupacao`)
- RF03: O sistema DEVE permitir definir salário base mínimo e máximo
- RF04: O sistema DEVE permitir filtrar cargos por empresa, nome e CBO

## Experiência do Usuário

Formulário simples com busca de CBO (autocomplete/search select na tabela de CBOs oficiais). 
Lista com paginação, busca por nome e código.

## Restrições Técnicas de Alto Nível

- **Integração**: CBO referencia tabela `cbo_ocupacao` (schema público, ~2.500 registros)
- **Multi-tenancy**: Cargos isolados por tenant/empresa
- **Auditoria**: Soft delete + `audit_log`

## Fora de Escopo

- Gestão da tabela CBO (seed data carregado na fundação — F01)
- Progressão de carreira e plano de cargos e salários (feature futura)

---

## Anexo A — Referência de Endpoints

| Controller | Base Path | Endpoints |
|-----------|-----------|-----------|
| `CargosController` | `/api/cargos` | GET list, GET by id, POST, PUT, DELETE |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 3.
