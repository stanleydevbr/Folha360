# PRD G05 — Módulo Rubricas

## Visão Geral

O módulo Rubricas é o cérebro do cálculo da folha de pagamento. Gerencia todo o plano de 
rubricas (vencimentos, descontos, informativas), suas incidências, fórmulas de cálculo, 
composições hierárquicas, tabelas progressivas e versionamento.

**Problema resolvido**: O DP precisa de um motor de rubricas flexível que suporte qualquer 
regra de cálculo (valor fixo, percentual, fórmula, tabela progressiva, composição), com 
validação de conformidade contra a Tabela 03 do e-Social e simulação antes da produção.

**Público-alvo**: Operador DP (configuração), Contador (validação e conformidade).

## Objetivos

- **Flexibilidade**: Suportar 9 naturezas e 11 tipos de cálculo de rubricas
- **Conformidade e-Social**: 100% das rubricas validadas contra Tabela 03
- **Rastreabilidade**: Histórico completo de alterações com versionamento
- **Segurança**: Fórmulas em sandbox (NCalc com timeout 100ms)
- **Performance**: Cache Redis com invalidação pub/sub para evitar recálculo

## Histórias de Usuário

1. **Como Operador DP**, quero criar grupos de rubricas (vencimentos, descontos, informativas) 
   para organizar o plano de contas da folha.
2. **Como Operador DP**, quero cadastrar rubricas com código, descrição, natureza e tipo de 
   cálculo para compor a folha de pagamento.
3. **Como Operador DP**, quero configurar incidências (INSS, IRRF, FGTS, 13º, Férias) em cada 
   rubrica para que as bases de cálculo sejam apuradas corretamente.
4. **Como Operador DP**, quero criar rubricas com fórmulas (NCalc) para cálculos complexos 
   como médias e proporcionalidades.
5. **Como Operador DP**, quero configurar composição hierárquica de rubricas (ex: salário base 
   + adicionais = remuneração total) com detecção de ciclos.
6. **Como Contador**, quero criar e versionar tabelas progressivas (IRRF, INSS) por ano de 
   vigência para manter a folha atualizada com a legislação.
7. **Como Contador**, quero verificar a conformidade das rubricas contra a Tabela 03 do 
   e-Social para evitar rejeições no envio.
8. **Como Contador**, quero simular o cálculo de rubricas com um salário base para validar 
   o plano antes de processar a folha real.

## Funcionalidades Principais

### F1. Cadastro de Rubricas
**Requisitos funcionais**:
- RF01: CRUD de rubricas com código, descrição, natureza (9 tipos) e tipo cálculo (11 tipos)
- RF02: O sistema DEVE suportar 12 flags de incidência independentes (INSS, IRRF, FGTS, 
  Sindical, 13º, Férias, Aviso Prévio, Rescisão, Dissídio, Sal. Maternidade, Aux. Doença, Adiantamento)
- RF03: O sistema DEVE permitir ordenação de cálculo e exibição independentes
- RF04: O sistema DEVE suportar teto máximo, piso mínimo e prioridade de desconto
- RF05: O sistema DEVE permitir vigência (data início/fim) por rubrica

### F2. Grupos de Rubricas
**Requisitos funcionais**:
- RF06: CRUD de grupos com código, descrição, natureza e ordem de exibição
- RF07: O sistema DEVE permitir agrupar rubricas para organização hierárquica

### F3. Composição de Rubricas
**Requisitos funcionais**:
- RF08: O sistema DEVE permitir compor uma rubrica a partir de outras (operador + ou -)
- RF09: O sistema DEVE detectar e impedir ciclos na composição (DFS)
- RF10: O sistema DEVE permitir definir percentual de composição e obrigatoriedade

### F4. Fórmulas
**Requisitos funcionais**:
- RF11: O sistema DEVE suportar expressões NCalc com timeout de 100ms (sandbox)
- RF12: O sistema DEVE versionar fórmulas automaticamente a cada alteração

### F5. Tabelas Progressivas
**Requisitos funcionais**:
- RF13: CRUD de faixas progressivas vinculadas a uma rubrica e ano de vigência
- RF14: O sistema DEVE suportar faixas com valor de, valor até, alíquota e dedução

### F6. Incidências
**Requisitos funcionais**:
- RF15: O sistema DEVE permitir adicionar/remover incidências individualmente
- RF16: O sistema DEVE suportar 12 tipos de incidência

### F7. Histórico de Alterações
**Requisitos funcionais**:
- RF17: O sistema DEVE registrar todas as alterações com dados anteriores, novos, 
  motivo e usuário responsável
- RF18: O sistema DEVE permitir consulta paginada do histórico por rubrica

### F8. Conformidade e Simulação
**Requisitos funcionais**:
- RF19: O sistema DEVE validar rubricas contra Tabela 03 do e-Social e reportar problemas
- RF20: O sistema DEVE simular cálculo com salário base, tipo de contrato, horas e dias, 
  retornando valores por rubrica, totais, bases e erros

## Experiência do Usuário

**Fluxo principal**: Criar Grupos → Criar Rubricas → Configurar Incidências → Criar Fórmulas 
(se aplicável) → Configurar Composição (se aplicável) → Criar Tabelas Progressivas → 
Verificar Conformidade → Simular Cálculo.

**Acessibilidade**: Formulários com muitas opções (12 flags de incidência) devem usar 
agrupamento visual claro. Tabelas progressivas usam grid editável inline.

## Restrições Técnicas de Alto Nível

- **Segurança**: Fórmulas executadas em sandbox NCalc com timeout 100ms
- **Cache**: Redis com invalidação pub/sub para plano de rubricas
- **Eventos**: `RubricaCriada`, `RubricaAlterada`, `TabelaProgressivaAtualizada` via RabbitMQ
- **Conformidade**: Mapeamento completo Tabela 03 e-Social
- **Auditoria**: Histórico imutável de alterações (`rubrica_historico`)

## Fora de Escopo

- Cálculo da folha em si (módulo Folha — G07)
- Apuração de tributos (módulo Fiscais — G11)
- Importação de planos de rubricas de outros sistemas (feature futura)

---

## Anexo A — Referência de Endpoints

| Subgrupo | Controller/Sub-rota | Base Path |
|----------|---------------------|-----------|
| 5.1 Cadastro | `RubricasController` | `/api/rubricas` |
| 5.2 Grupos | `GruposRubricaController` | `/api/grupos-rubrica` |
| 5.3 Composição | Sub-rota | `/api/rubricas/{id}/composicao` |
| 5.4 Fórmulas | Sub-rota | `/api/rubricas/{id}/formula` |
| 5.5 Tabelas Progressivas | `TabelasProgressivasController` | `/api/tabelas-progressivas` |
| 5.6 Incidências | Sub-rota | `/api/rubricas/{id}/incidencias` |
| 5.7 Histórico | Sub-rota | `/api/rubricas/{id}/historico` |
| 5.8 Conformidade/Simulação | `RubricasController` | `/api/rubricas/conformidade`, `/api/rubricas/simular` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 5.  
Jornada: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 3.
