# Pesquisa de Mercado — Cadastro de Rubricas em Sistemas de Folha

## Summary
Pesquisa comparativa sobre como os principais fornecedores de soluções de folha de pagamento no Brasil implementam o cadastro de rubricas. O objetivo é identificar melhores práticas, pontos fortes e fracos de cada abordagem, e sugerir uma solução otimizada para o projeto Folha360.

---

## Metodologia

- **Fontes**: Documentação oficial, manuais de usuário, vídeos tutoriais, fóruns especializados
- **Fornecedores analisados**: 6 (Totvs RM, Senior, ADP, SAP, e-Social Simples, Pontomais)
- **Critérios**: Flexibilidade, conformidade e-Social, usabilidade, performance, auditoria
- **Data**: Junho 2026

---

## Matriz Comparativa

| Critério | Totvs RM | Senior | ADP | SAP HCM | e-Social Simples | Pontomais | **Folha360 (proposto)** |
|---|---|---|---|---|---|---|---|
| **Rubricas pré-definidas** | ✅ 200+ | ✅ 150+ | ✅ 300+ | ✅ 250+ | ✅ 50+ | ✅ 80+ | ✅ 50+ (seed Tabela 03) |
| **Customização de rubricas** | ⚠️ Limitada | ✅ Flexível | ⚠️ Limitada | ✅ Flexível | ❌ Não permite | ⚠️ Básica | ✅ Total (fórmulas, composição) |
| **Fórmulas de cálculo** | ❌ Não | ✅ Sim (script) | ✅ Sim (expressões) | ✅ Sim (PCR) | ❌ Não | ❌ Não | ✅ Sim (NCalc + sandbox) |
| **Composição hierárquica** | ❌ Não | ⚠️ Parcial | ❌ Não | ⚠️ Parcial | ❌ Não | ❌ Não | ✅ Sim (drag-and-drop) |
| **Conformidade e-Social automática** | ✅ Sim | ✅ Sim | ✅ Sim | ✅ Sim | ✅ Sim | ⚠️ Manual | ✅ Sim (validação contínua) |
| **Versionamento de rubricas** | ❌ Não | ⚠️ Parcial | ✅ Sim | ✅ Sim | ❌ Não | ❌ Não | ✅ Sim (histórico completo) |
| **Simulador de cálculo** | ⚠️ Básico | ✅ Sim | ✅ Sim | ✅ Sim | ❌ Não | ❌ Não | ✅ Sim (funcionário teste) |
| **Editor visual** | ❌ Não | ✅ Sim | ❌ Não | ❌ Não | ❌ Não | ❌ Não | ✅ Sim (drag-and-drop) |
| **Cache para performance** | ❌ Não | ❌ Não | ✅ Sim | ✅ Sim | ❌ Não | ❌ Não | ✅ Sim (Redis) |
| **Multi-tenant** | ✅ Sim | ✅ Sim | ✅ Sim | ✅ Sim | ❌ Não | ❌ Não | ✅ Sim (schema por tenant) |
| **Auditoria (LGPD)** | ⚠️ Parcial | ⚠️ Parcial | ✅ Sim | ✅ Sim | ❌ Não | ❌ Não | ✅ Sim (imutável) |
| **Preço (estimado)** | $$$$ | $$$ | $$$$$ | $$$$$ | $ | $$ | $$$ (alvo) |

---

## Análise Detalhada por Fornecedor

### 1. Totvs RM (Mercado: Grandes Empresas)

**Abordagem**: Rubricas pré-definidas com código e-Social; customização limitada a parâmetros básicos.

**Fluxo de cadastro**:
1. Seleciona rubrica da lista pré-definida (200+ opções)
2. Configura parâmetros: valor, percentual, incidências
3. Rubrica automaticamente vinculada ao código e-Social

**Pontos fortes**:
- Conformidade e-Social garantida (rubricas pré-validadas)
- Simples de usar (poucas opções = menos erros)
- Boa performance (sem fórmulas complexas)

**Pontos fracos**:
- Pouca flexibilidade para rubricas específicas de convenções coletivas
- Sem suporte a fórmulas personalizadas
- Difícil adaptar para empresas com regras muito específicas (ex.: construção civil, rural)

**Inspiração para Folha360**: Seed data de rubricas pré-validadas como ponto de partida.

---

### 2. Senior (Mercado: Médias e Grandes Empresas)

**Abordagem**: Rubricas parametrizáveis com fórmulas em linguagem de script proprietária.

**Fluxo de cadastro**:
1. Cria nova rubrica ou copia de existente
2. Define código, descrição, natureza (vencimento/desconto)
3. Configura incidências (INSS, IRRF, FGTS, etc.)
4. Opcional: escreve fórmula em linguagem de script
5. Testa rubrica em "folha de simulação"

**Pontos fortes**:
- Muito flexível (fórmulas permitem qualquer regra)
- Editor de fórmulas com autocomplete
- Simulador integrado para teste

**Pontos fracos**:
- Curva de aprendizado alta para fórmulas
- Risco de erro em fórmulas mal escritas
- Linguagem de script proprietária (vendor lock-in)
- Performance pode degradar com muitas fórmulas complexas

**Inspiração para Folha360**: Simulador integrado + editor de fórmulas com syntax highlighting.

---

### 3. ADP (Mercado: Multinacionais)

**Abordagem**: Rubricas pré-definidas por país com regras fiscais automáticas. Foco em conformidade global.

**Fluxo de cadastro**:
1. Seleciona país (Brasil)
2. Rubricas brasileiras já vêm pré-configuradas com regras fiscais
3. Customização limitada a parâmetros de valor
4. Atualização automática de tabelas progressivas (IRRF, INSS)

**Pontos fortes**:
- Conformidade fiscal garantida e atualizada automaticamente
- Suporte a múltiplos países (bom para multinacionais)
- Versionamento de alterações fiscais

**Pontos fracos**:
- Pouca customização para regras locais específicas
- Custo muito elevado
- Dependente de atualizações do fornecedor para novas regras

**Inspiração para Folha360**: Atualização automática de tabelas progressivas + versionamento.

---

### 4. SAP HCM (Mercado: Grandes Corporações)

**Abordagem**: Sistema de regras de cálculo (PCR - Personnel Calculation Rules) extremamente flexível e complexo.

**Fluxo de cadastro**:
1. Define "wage types" (rubricas) no customizing
2. Configura "schemas" de cálculo (sequência de processamento)
3. Escreve regras PCR para condições especiais
4. Testa em ambiente de simulação

**Pontos fortes**:
- Extremamente flexível (qualquer regra é possível)
- Motor de cálculo robusto e testado globalmente
- Versionamento completo de configurações

**Pontos fracos**:
- Complexidade extrema (requer consultor especializado)
- Custo altíssimo (licenciamento + consultoria)
- Tempo de implantação longo (6-12 meses)
- Overkill para empresas médias brasileiras

**Inspiração para Folha360**: Conceito de "schema de cálculo" (ordem de processamento das rubricas).

---

### 5. e-Social Simples (Mercado: Micro e Pequenas Empresas)

**Abordagem**: Rubricas fixas e limitadas ao mínimo exigido pelo e-Social. Sem customização.

**Fluxo de cadastro**:
1. Rubricas já vêm definidas (50+)
2. Usuário apenas informa valores
3. Sem possibilidade de criar novas rubricas

**Pontos fortes**:
- Extremamente simples
- Barato
- Conformidade garantida (poucas rubricas = menos risco)

**Pontos fracos**:
- Não atende empresas com necessidades específicas
- Sem suporte a convenções coletivas complexas
- Não escala para médias/grandes empresas

**Inspiração para Folha360**: Simplicidade como modo "básico" para pequenas empresas.

---

### 6. Pontomais (Mercado: Pequenas e Médias Empresas)

**Abordagem**: Foco em controle de ponto + folha simplificada. Rubricas básicas com alguma customização.

**Fluxo de cadastro**:
1. Rubricas pré-definidas (80+)
2. Permite criar novas rubricas com parâmetros básicos
3. Sem suporte a fórmulas complexas
4. Integração com controle de ponto (horas extras automáticas)

**Pontos fortes**:
- Integração nativa com controle de ponto
- Interface moderna e amigável
- Preço acessível

**Pontos fracos**:
- Customização limitada
- Sem suporte a composição de rubricas
- Conformidade e-Social manual (usuário precisa mapear)

**Inspiração para Folha360**: Interface moderna + integração com controle de ponto.

---

## Síntese: O que o Mercado Ensina

### Padrões que Funcionam

1. **Seed data de rubricas padrão**: Todos os fornecedores partem de uma base pré-definida. Isso reduz erros e acelera a implantação.
2. **Validação de conformidade**: Os melhores fornecedores validam automaticamente a compatibilidade com e-Social.
3. **Simulador de cálculo**: Essencial para testar rubricas antes de aplicar em produção.
4. **Versionamento**: ADP e SAP versionam alterações; isso é crítico para auditoria fiscal.
5. **Cache de tabelas**: ADP e SAP usam cache agressivo para performance.

### Oportunidades de Diferenciação

1. **Composição hierárquica visual**: NENHUM concorrente oferece composição drag-and-drop de rubricas. Este é o principal diferencial do Folha360.
2. **Fórmulas com sintaxe aberta**: Usar NCalc (C#) em vez de linguagem proprietária reduz vendor lock-in.
3. **Modo básico + avançado**: Pequenas empresas usam seed data; grandes usam customização total.
4. **Conformidade contínua**: Validação automatizada contra Tabela 03 com alertas proativos.
5. **Sandbox de fórmulas**: Timeout e limites de recursos para evitar fórmulas maliciosas.

---

## Recomendação para o Folha360

Com base na pesquisa, o Folha360 deve adotar uma abordagem **híbrida**:

### Para Pequenas Empresas (Modo Básico)
- Usar apenas as rubricas do seed data (Tabela 03)
- Interface simplificada: selecionar rubrica → informar valor
- Conformidade e-Social automática

### Para Médias e Grandes Empresas (Modo Avançado)
- Customização total: criar, editar, compor rubricas
- Editor de fórmulas com syntax highlighting
- Composição hierárquica visual (drag-and-drop)
- Simulador de cálculo integrado
- Versionamento e auditoria completos

### Funcionalidades-Chave (Priorizadas)

| # | Funcionalidade | Prioridade | Diferencial? |
|---|---|---|---|
| 1 | Seed data Tabela 03 (50+ rubricas) | P0 - Essencial | Não |
| 2 | CRUD de rubricas com validações | P0 - Essencial | Não |
| 3 | Composição hierárquica visual | P0 - Essencial | **Sim** ⭐ |
| 4 | Fórmulas de cálculo (NCalc) | P1 - Importante | Parcial |
| 5 | Simulador de cálculo | P1 - Importante | Parcial |
| 6 | Validação contínua e-Social | P1 - Importante | **Sim** ⭐ |
| 7 | Cache Redis para rubricas | P1 - Importante | Parcial |
| 8 | Versionamento e auditoria | P1 - Importante | Parcial |
| 9 | Editor de fórmulas com autocomplete | P2 - Desejável | Não |
| 10 | Modo básico/avançado | P2 - Desejável | **Sim** ⭐ |

---

## Conclusão

O mercado de sistemas de folha de pagamento no Brasil oferece desde soluções extremamente simples (e-Social Simples) até plataformas complexas e caras (SAP, ADP). O Folha360 se posiciona no **meio-termo**: oferecer a flexibilidade das grandes plataformas (fórmulas, composição) com a usabilidade e preço acessível das soluções mais simples.

O grande diferencial competitivo será a **composição hierárquica visual de rubricas** — nenhum concorrente oferece isso atualmente. Combinado com validação contínua de conformidade e-Social e um motor de cálculo de alta performance (100K funcionários em < 2h), o Folha360 tem potencial para capturar o mercado de médias empresas que hoje sofrem com a rigidez do Totvs ou com a complexidade do SAP.

---

## Referências

- [Database Model — Rubricas](./database-model-rubricas.md)
- [Plano de Ação — Rubricas](./plano-acao-rubricas.md)
- [Diagrama de Composição e Dependência](./diagrama-composicao-dependencia-rubricas.md)
- [Runtime View — Cálculo com Rubricas](./runtime-view-calculo-rubricas.md)
