# PRD G04 — Módulo Estrutura de Apoio

## Visão Geral

O módulo Estrutura de Apoio consolida as tabelas de referência e cadastros auxiliares usados 
por todos os outros módulos do Folha360: sindicatos, convênios, horários de trabalho e tabelas 
de domínio compartilhado (CBO, Natureza Jurídica, Municípios IBGE, Bancos Febraban).

**Problema resolvido**: O DP precisa de tabelas auxiliares consistentes para evitar erros em 
cascata — um sindicato mal cadastrado gera contribuição incorreta na folha; um município 
inválido gera rejeição no e-Social.

**Público-alvo**: Admin (setup inicial), Operador DP (manutenção).

## Objetivos

- **Centralizar tabelas de apoio** usadas por Empresa, Funcionário e Folha
- **Garantir integridade referencial** entre todos os módulos
- **Seed data oficial**: CBOs MTb, Naturezas Jurídicas (Tabela 21 e-Social), Municípios IBGE, 
  Bancos Febraban carregados na fundação

## Histórias de Usuário

1. **Como Admin**, quero cadastrar sindicatos (patronais e laborais) com percentuais de 
   contribuição para que a folha calcule os descontos corretamente.
2. **Como Admin**, quero cadastrar convênios (plano de saúde, VR, VT) com percentuais de 
   custeio para vinculá-los aos funcionários.
3. **Como Admin**, quero cadastrar horários de trabalho com jornada, intervalos e tolerância 
   para padronizar os contratos.
4. **Como Operador DP**, quero consultar CBOs, naturezas jurídicas, municípios e bancos para 
   preencher cadastros sem digitar códigos manualmente.

## Funcionalidades Principais

### F1. Sindicatos
**Requisitos funcionais**:
- RF01: CRUD de sindicatos com código, nome, CNPJ e tipo (Patronal/Laboral)
- RF02: O sistema DEVE armazenar % de contribuição sindical e assistencial
- RF03: O sistema DEVE filtrar por empresa

### F2. Convênios
**Requisitos funcionais**:
- RF04: CRUD de convênios com nome, tipo (Saúde/Odontológico/VR/VA/VT/Seguro/Previdência/Outros)
- RF05: O sistema DEVE armazenar valor mensal, % empresa e % funcionário

### F3. Horários de Trabalho
**Requisitos funcionais**:
- RF06: CRUD de horários com código, descrição e tipo (Fixo/Flexível/Turno/Escala)
- RF07: O sistema DEVE armazenar carga horária diária e semanal, horários de início/fim e intervalo
- RF08: O sistema DEVE armazenar tolerância de atraso em minutos

### F4. Tabelas de Referência (Lookups)
**Requisitos funcionais**:
- RF09: O sistema DEVE expor endpoints somente leitura para CBO, Natureza Jurídica, 
  Municípios IBGE e Bancos Febraban
- RF10: O sistema DEVE suportar filtro por UF e nome em municípios (5.570 registros)
- RF11: O sistema DEVE suportar busca textual em CBOs (~2.500 registros) e bancos (~150 registros)

## Experiência do Usuário

Formulários simples para sindicatos, convênios e horários. Tabelas de referência são somente 
leitura, consumidas via autocomplete/search select nos formulários de Empresa e Funcionário.

## Restrições Técnicas de Alto Nível

- **Seed data**: Tabelas de referência carregadas via bulk insert (PostgreSQL COPY) na fundação
- **Cache**: Tabelas de lookup podem ser cacheadas em Redis (baixa volatilidade)
- **Multi-tenancy**: Sindicatos, convênios e horários são isolados por tenant; tabelas de 
  referência são compartilhadas (schema público)

## Fora de Escopo

- Atualização automática de tabelas oficiais (CBO, municípios) — processo manual/seed
- Gestão de acordos coletivos de sindicatos (feature futura)

---

## Anexo A — Referência de Endpoints

| Subgrupo | Controller | Base Path |
|----------|-----------|-----------|
| 4.1 Sindicato | `SindicatosController` | `/api/sindicatos` |
| 4.2 Convênio | `ConveniosController` | `/api/convenios` |
| 4.3 Horário | `HorariosTrabalhoController` | `/api/horarios-trabalho` |
| 4.4 Lookups | `CbosController` | `GET /api/cbos` |
| 4.4 Lookups | `NaturezasJuridicasController` | `GET /api/naturezas-juridicas` |
| 4.4 Lookups | `MunicipiosController` | `GET /api/municipios` |
| 4.4 Lookups | `BancosController` | `GET /api/bancos` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 4.
