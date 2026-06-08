<task>
    Definir cenários de atributos de qualidade (Quality Attribute Scenarios) para o módulo de Cálculo da Folha do Folha360, cobrindo os domínios específicos de vínculos sindicais, planos de saúde, bases de cálculo governamentais e demais integrações que impactam a precisão e eficiência do processamento da folha de pagamento.
</task>

<goals>
    Gerar um documento estruturado de cenários de qualidade mensuráveis, no formato ISO 25010 / SEI Quality Attribute Scenario, que complemente e refine o artefato existente `docs/outputs/arquitetura/quality-attribute-scenarios.md` com foco nos domínios de cálculo fiscal e social.
</goals>

<role>
    Você é um arquiteto de software especialista em sistemas de folha de pagamento brasileira, com profundo conhecimento em: e-Social (leiautes S-1.3), legislação trabalhista (CLT), legislação previdenciária (INSS, IRRF, FGTS), convenções coletivas de trabalho, e atributos de qualidade conforme ISO 25010. Você trabalha no projeto Folha360 — um monólito modular .NET 10 + PostgreSQL + React com arquitetura DDD e multi-tenancy schema-per-tenant.
</role>

<requirements>

### Business

- O sistema processa folha de pagamento para 100.000+ funcionários em ambientes multi-empresa (schema-per-tenant)
- Cálculos envolvem: vencimentos, descontos legais (INSS, IRRF, FGTS), benefícios (VT, VR, plano de saúde), contribuição sindical, convênios e pensões
- O sistema deve obedecer a 11 tipos de cálculo: mensal, férias, 13º, rescisão, dissídio, complementar, auxílio-doença, salário-maternidade, acordo, estágio, RPA
- Rubricas são parametrizáveis com fórmulas (NCalc), composição hierárquica e tabelas progressivas versionadas por ano
- Eventos trabalhistas (S-2200, S-2230, S-2299) disparam automaticamente impactos na folha
- Conformidade com LGPD (art. 18), legislação fiscal (RFB) e normas do e-Social (S-1200, S-1210, S-5001, S-5002)
- Os cálculos devem considerar bases de cálculo atualizadas definidas pelo governo, como por exemplo: IRRF, Salário-Familia

### Technical

- **Stack**: .NET 10 (backend), PostgreSQL (banco), React (frontend), RabbitMQ (mensageria), Redis (cache)
- **Arquitetura**: Monólito modular DDD com 6 módulos: Cadastros, Eventos Trabalhistas, Cálculo da Folha, Obrigações Fiscais, Relatórios, Integração e-Social
- **Motor de cálculo**: 4 fases (vencimentos → bases → descontos → totais), processamento em lotes de 1000 funcionários
- **Integrações externas**: e-Social gov.br (envio de eventos XML), possíveis integrações com sindicatos (contribuição), operadoras de plano de saúde (descontos)
- **Padrão de comunicação interna**: Choreography (eventos RMQ) entre Eventos Trabalhistas → Cálculo; Orquestrador de fechamento (saga) entre Cálculo → Obrigações Fiscais
- **Artefato de referência**: `docs/outputs/arquitetura/quality-attribute-scenarios.md` (já contém cenários P1-P3, A1-A3, S1-S3, SC1-SC2, M1-M3, U1)

### UI/UX

- Operadores de RH precisam de feedback visual durante o processamento (progresso, ETA)
- Configuração de rubricas e fórmulas deve ser acessível a usuários não-técnicos com validação em sandbox
- Simulação de impacto de novas rubricas disponível antes do fechamento da folha

</requirements>

<workflow>
    1. **Analisar o documento existente**: Leia `docs/outputs/arquitetura/quality-attribute-scenarios.md` para entender os cenários já cobertos e identificar lacunas nos domínios de vínculos sindicais, planos de saúde e bases de cálculo governamentais.
    2. **Mapear domínios de cálculo não cobertos**: Identifique cenários específicos que impactam a precisão do cálculo e ainda não foram modelados — ex.: alteração de alíquota sindical por convenção coletiva, reajuste de plano de saúde, mudança na tabela de INSS, inclusão de nova rubrica legal obrigatória.
    3. **Gerar cenários por atributo de qualidade**: Para cada lacuna identificada, crie cenários no formato padronizado (Attribute | Source/Stimulus | Environment | Artifact | Response | Response Measure | Priority), cobrindo pelo menos: Performance, Availability, Security, Maintainability, Scalability.
    4. **Priorizar com matriz de criticidade**: Classifique cada cenário como Crítico, Alto ou Médio, considerando impacto legal/fiscal, frequência de ocorrência e esforço de mitigação.
    5. **Consolidar no artefato final**: Gere o documento complementar com sumário executivo, cenários detalhados, matriz de prioridades e recomendações de próximos passos (ex.: `architecture-risk-assessor`).
</workflow>

<output>
    Documento Markdown seguindo a mesma estrutura de `docs/outputs/arquitetura/quality-attribute-scenarios.md`:

    ```
    # Quality Attribute Scenarios — Folha360: Domínios de Cálculo Fiscal e Social

    ## Summary
    [Resumo executivo com escopo e relação com o artefato principal]

    ## Performance
    ### Cenário P4: [Título]
    - **Attribute**: ...
    - **Source/stimulus**: ...
    - **Environment**: ...
    - **Artifact**: ...
    - **Response**: ...
    - **Response measure**: ...
    - **Priority**: ...

    ## [Demais atributos: Availability, Security, Maintainability, Scalability, Usability]

    ## Matriz de Prioridades
    | ID | Cenário | Atributo | Prioridade |

    ## Evidence vs Assumptions
    ## Recommended Next Skill
    ```

    Cada cenário DEVE ter medida quantitativa (tempo, percentual, taxa). NÃO use medidas vagas como "rápido" ou "adequado".
</output>

<critical>

### Skills obrigatórias

- `quality-attribute-scenario-writer` — para estruturar os cenários no formato SEI/ISO 25010
- `architecture-risk-assessor` — recomendado como próximo passo após a definição dos cenários

### Fora do Escopo

- *NÃO* gere cenários já cobertos pelo artefato principal (`P1-P3, A1-A3, S1-S3, SC1-SC2, M1-M3, U1`). Apenas referencie-os quando necessário.
- *NÃO* proponha alterações na stack tecnológica ou na arquitetura do projeto.
- *NÃO* inclua cenários de implementação (tasks, código, endpoints) — o foco é exclusivamente em atributos de qualidade.
- *NÃO* use medidas subjetivas ou qualitativas como "bom", "rápido", "robusto". Toda medida deve ser quantitativa e verificável.
- *NUNCA* ignore a legislação brasileira (CLT, INSS, IRRF, FGTS, LGPD, e-Social) ao definir os estímulos e medidas dos cenários.

</critical>
