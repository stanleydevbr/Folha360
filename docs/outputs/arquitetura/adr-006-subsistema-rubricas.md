# ADR-006: Subsistema de Rubricas com Composição Hierárquica e Fórmulas

## Status
**Accepted** (Junho 2026)

## Context
O sistema Folha360 inicialmente modelava rubricas como uma entidade simples com código, descrição, natureza (vencimento/desconto) e flags de incidência (INSS, IRRF, FGTS). A [pesquisa de mercado](../rubricas/pesquisa-mercado-rubricas.md) revelou que sistemas concorrentes (Senior, ADP, SAP) oferecem recursos avançados como composição hierárquica de rubricas, fórmulas de cálculo parametrizáveis, tabelas progressivas e simuladores — funcionalidades que são diferenciais competitivos e necessárias para atender a convenções coletivas complexas e regras específicas de diferentes setores (construção civil, rural, saúde).

Além disso, a análise dos 11 tipos de cálculo de um Departamento Pessoal completo (mensal, férias, 13º, rescisão, dissídio, complementar, auxílio-doença, salário-maternidade, acordo, estágio, RPA) mostrou que o modelo simples original não suportaria a complexidade necessária.

## Decision Drivers
1. **Flexibilidade**: Empresas precisam customizar rubricas para convenções coletivas sem quebrar a compatibilidade com e-Social
2. **Completude**: Suportar TODOS os 11 tipos de cálculo de um DP completo
3. **Conformidade**: Aderência à Tabela 03 do e-Social (S-1010) e processos administrativos (S-1070)
4. **Diferencial competitivo**: Editor visual drag-and-drop, simulador, fórmulas — funcionalidades que sistemas como Totvs RM não oferecem
5. **Performance**: Cálculo de 100K funcionários com todas as rubricas em < 2 horas
6. **Segurança**: Fórmulas executadas em sandbox para evitar código malicioso

## Options Considered

| Opção | Prós | Contras |
|---|---|---|
| **Modelo simples (original)** | Simples de implementar; baixa complexidade | Não suporta composição hierárquica; sem fórmulas; não atende 11 tipos de cálculo; perde competitividade |
| **Subsistema expandido (escolhido)** | Flexibilidade total; composição hierárquica; fórmulas NCalc; tabelas progressivas; 11 tipos de cálculo; diferencial competitivo | Maior complexidade de implementação (13 sprints); curva de aprendizado; risco de performance |
| **Microserviço dedicado de rubricas** | Isolamento total; escala independente | Complexidade operacional; latência de rede; inconsistente com ADR-001 (monólito modular) |

## Decision
**Subsistema de rubricas expandido dentro do módulo Cadastros**, com as seguintes características:

### Modelo de Dados (7 tabelas)
1. **`rubrica`** (expandida): 30+ colunas incluindo 9 naturezas, 12 flags de incidência, 11 tipos de cálculo, teto/piso, vigência
2. **`grupo_rubrica`**: Agrupamento categórico (ex.: "Vencimentos Fixos", "Descontos Legais")
3. **`rubrica_composicao`**: Composição hierárquica entre rubricas com operadores (+, -, *, /) e percentuais
4. **`rubrica_formula`**: Fórmulas NCalc parametrizáveis com JSONB para parâmetros
5. **`rubrica_incidencia`**: Definição explícita de sobre quais bases/tributos cada rubrica incide
6. **`rubrica_tabela_progressiva`**: Faixas de alíquota e dedução para IRRF/INSS, versionável por ano
7. **`rubrica_historico`**: Trilha de auditoria imutável de todas as alterações

### Motor de Cálculo (4 fases)
- **Fase 1 — Vencimentos**: VALOR_FIXO, UNIDADE, HORA, DIA, PERCENTUAL, MEDIA, FORMULA, COMPOSICAO
- **Fase 2 — Bases**: BASE-INSS, BASE-FGTS, BASE-IRRF, BASE-DISSIDIO, BASE-MATERNIDADE
- **Fase 3 — Descontos**: TABELA_PROGRESSIVA (IRRF/INSS), PERCENTUAL, CONDICIONAL
- **Fase 4 — Totais**: TOTAL-VENCIMENTOS, TOTAL-DESCONTOS, LÍQUIDO

### Componentes do Motor
- `MotorCalculo`: Orquestrador das 4 fases
- `AvaliadorExpressao`: NCalc com sandbox (timeout 100ms, memory limit)
- `ResolvedorComposicao`: DFS com detecção de ciclos
- `AplicadorTabelaProgressiva`: Tabelas versionáveis por ano
- `CalculadorMedia`: Média móvel 12 meses
- `AvaliadorCondicional`: Condições parametrizáveis

### Cache
- Redis para rubricas, composições, fórmulas e tabelas progressivas
- Invalidação via pub/sub nos eventos `RubricaAlterada`, `RubricaCriada`, `TabelaProgressivaAtualizada`

## Consequences

### Positive
- Flexibilidade total para qualquer convenção coletiva ou regra específica de setor
- Suporte completo aos 11 tipos de cálculo de DP
- Diferencial competitivo significativo (editor visual, simulador, fórmulas)
- Conformidade garantida com Tabela 03 do e-Social via validação contínua
- Auditoria completa de alterações (LGPD)
- Cache Redis com invalidação inteligente mantém performance

### Negative
- Complexidade de implementação aumentou de ~2 sprints para 13 sprints
- Curva de aprendizado para usuários (fórmulas, composição)
- Risco de performance com muitas rubricas compostas e fórmulas complexas
- Maior superfície de testes (combinações de composição, fórmulas, incidências)

### Mitigações
- Faseamento: Fase 1 (CRUD) → Fase 2 (Composição) → Fase 3 (Motor) → Fase 4 (e-Social) → Fase 5 (Frontend) → Fase 6 (Performance)
- Sandbox NCalc com timeout 100ms e whitelist de funções
- Testes de carga com 100K funcionários e 50 rubricas cada (Fase 6)
- Editor visual drag-and-drop reduz barreira de entrada para composição
- Simulador de cálculo para validação prévia
- Documentação extensiva e treinamento (Fase 7)

## Follow-up Actions
- [ ] Implementar Fase 1: CRUD expandido + seed data (Sprint 1-2)
- [ ] Implementar Fase 2: Composição hierárquica + fórmulas NCalc (Sprint 3-4)
- [ ] Implementar Fase 3: Motor de cálculo com 11 tipos (Sprint 5-7)
- [ ] Implementar Fase 4: Integração e-Social S-1010/S-1070 (Sprint 8-9)
- [ ] Implementar Fase 5: Frontend com editor visual e simulador (Sprint 10-11)
- [ ] Implementar Fase 6: Testes de carga e otimização (Sprint 12)
- [ ] Implementar Fase 7: Documentação e treinamento (Sprint 13)
- [ ] Criar tarefa `refactor-f02-rubricas-expandido` para expandir F02
- [ ] Criar tarefa `refactor-f04-motor-calculo-expandido` para expandir F04
- [ ] Revisar esta decisão após conclusão da Fase 6 (testes de carga)
