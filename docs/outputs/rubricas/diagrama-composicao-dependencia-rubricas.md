# Diagrama de Composição e Dependência — Rubricas

## Summary
Este artefato apresenta os diagramas de composição e dependência do subsistema de rubricas do Folha360. Inclui: (1) o diagrama de dependência entre módulos no contexto das rubricas, (2) o diagrama de composição hierárquica das rubricas para cada tipo de cálculo (mensal, férias, 13º, rescisão), e (3) o fluxo de dependência entre rubricas para o cálculo da folha.

---

## Diagrama de Dependência entre Módulos (Contexto Rubricas)

```mermaid
graph TB
    subgraph "Módulo Cadastros"
        RUB["Rubricas<br/>CRUD + Fórmulas + Composição"]
        FUNC["Funcionários<br/>Dados Contratuais"]
        EMP["Empresas<br/>Configurações"]
    end

    subgraph "Módulo Eventos Trabalhistas"
        EVT["Eventos<br/>Férias, Afastamentos,<br/>Rescisão"]
    end

    subgraph "Módulo Cálculo da Folha"
        ENGINE["Motor de Cálculo<br/>Avaliador de Rubricas"]
        CACHE_RUB["Cache de Rubricas<br/>Redis"]
        FOLHA["FolhaMensal<br/>Resultados"]
    end

    subgraph "Módulo Obrigações Fiscais"
        FIS["Apuração Fiscal<br/>INSS, IRRF, FGTS"]
    end

    subgraph "Módulo e-Social"
        ES["Integração e-Social<br/>S-1200, S-1210, S-1010"]
    end

    RUB -->|"1. Rubricas vigentes (REST API)"| ENGINE
    RUB -->|"2. Evento RubricaAlterada"| CACHE_RUB
    FUNC -->|"3. Dados contratuais (REST API)"| ENGINE
    EVT -->|"4. Eventos do período (REST API)"| ENGINE
    ENGINE -->|"5. FolhaMensal + FolhaRubrica"| FOLHA
    ENGINE -->|"6. Evento FolhaFechada"| FIS
    ENGINE -->|"7. Evento EventoRemuneracaoGerado"| ES
    FIS -->|"8. Evento EventoFiscalGerado"| ES
    RUB -->|"9. Tabela 03 (S-1010)"| ES

    style RUB fill:#4CAF50,color:#fff
    style ENGINE fill:#2196F3,color:#fff
    style CACHE_RUB fill:#FF9800,color:#fff
    style ES fill:#9C27B0,color:#fff
```

---

## Diagrama de Composição: Cálculo Mensal

```mermaid
graph TD
    subgraph "Vencimentos"
        SAL["SAL-BASE<br/>Salário Base Mensal<br/>VALOR_FIXO"]
        HE50["HORA-EXTRA-50<br/>Horas Extras 50%<br/>HORA"]
        HE100["HORA-EXTRA-100<br/>Horas Extras 100%<br/>HORA"]
        AN["ADIC-NOTURNO<br/>20% sobre Salário Base<br/>PERCENTUAL"]
        AP["ADIC-PERICULOSIDADE<br/>30% sobre Salário Base<br/>PERCENTUAL"]
        AI["ADIC-INSALUBRIDADE<br/>10/20/40% sobre Salário Mínimo<br/>PERCENTUAL"]
        COM["COMISSAO<br/>% sobre vendas<br/>PERCENTUAL"]
        DSR["DSR<br/>Média horas extras / dias úteis<br/>FORMULA"]
    end

    subgraph "Bases de Cálculo"
        B_INSS["BASE-INSS<br/>SAL+HE50+HE100+AN+AP+AI+COM+DSR<br/>COMPOSICAO"]
        B_FGTS["BASE-FGTS<br/>SAL+HE50+HE100+AN+AP+AI+COM+DSR<br/>COMPOSICAO"]
        B_IRRF["BASE-IRRF<br/>BASE-INSS - INSS - PENSAO - DED_DEP<br/>FORMULA"]
    end

    subgraph "Descontos"
        INSS["INSS<br/>Tabela Progressiva sobre BASE-INSS<br/>TABELA_PROGRESSIVA"]
        IRRF["IRRF<br/>Tabela Progressiva sobre BASE-IRRF<br/>TABELA_PROGRESSIVA"]
        PENSAO["PENSAO-ALIMENTICIA<br/>% sobre BASE-INSS<br/>PERCENTUAL"]
        FALTA["FALTA<br/>Horas não trabalhadas × valor_hora<br/>HORA"]
        ATRASO["ATRASO<br/>Horas atraso × valor_hora<br/>HORA"]
        VT["VALE-TRANSPORTE<br/>6% sobre Salário Base<br/>PERCENTUAL"]
        VR["VALE-REFEICAO<br/>20% coparticipação<br/>PERCENTUAL"]
    end

    subgraph "Totais"
        TOT_V["TOTAL-VENCIMENTOS<br/>Soma Fase 1<br/>COMPOSICAO"]
        TOT_D["TOTAL-DESCONTOS<br/>Soma Fase 3<br/>COMPOSICAO"]
        LIQ["LIQUIDO<br/>TOTAL-VENCIMENTOS - TOTAL-DESCONTOS<br/>FORMULA"]
    end

    SAL --> B_INSS
    HE50 --> B_INSS
    HE100 --> B_INSS
    AN --> B_INSS
    AP --> B_INSS
    AI --> B_INSS
    COM --> B_INSS
    DSR --> B_INSS

    SAL --> B_FGTS
    HE50 --> B_FGTS
    HE100 --> B_FGTS
    AN --> B_FGTS
    AP --> B_FGTS
    AI --> B_FGTS
    COM --> B_FGTS
    DSR --> B_FGTS

    B_INSS --> INSS
    B_INSS --> B_IRRF
    INSS -->|"deduz"| B_IRRF
    PENSAO -->|"deduz"| B_IRRF

    SAL --> TOT_V
    HE50 --> TOT_V
    HE100 --> TOT_V
    AN --> TOT_V
    AP --> TOT_V
    AI --> TOT_V
    COM --> TOT_V
    DSR --> TOT_V

    INSS --> TOT_D
    IRRF --> TOT_D
    PENSAO --> TOT_D
    FALTA --> TOT_D
    ATRASO --> TOT_D
    VT --> TOT_D
    VR --> TOT_D

    TOT_V --> LIQ
    TOT_D -->|"deduz"| LIQ
```

---

## Diagrama de Composição: 13º Salário

```mermaid
graph TD
    subgraph "Base 13º Salário"
        B13["BASE-13<br/>Média dos vencimentos com incide_13=true<br/>dos últimos 12 meses<br/>COMPOSICAO"]
    end

    subgraph "Vencimentos 13º"
        V13_1["13-SALARIO<br/>BASE-13 / 12 × meses_trabalhados<br/>FORMULA"]
        V13_2["13-ADIANTAMENTO<br/>50% do 13-SALARIO<br/>PERCENTUAL"]
    end

    subgraph "Descontos 13º"
        INSS13["INSS-13<br/>Tabela Progressiva sobre 13-SALARIO<br/>TABELA_PROGRESSIVA"]
        IRRF13["IRRF-13<br/>Tabela Progressiva sobre 13-SALARIO<br/>TABELA_PROGRESSIVA"]
        PENS13["PENSAO-13<br/>% sobre 13-SALARIO<br/>PERCENTUAL"]
    end

    subgraph "Totais 13º"
        TOT13_V["TOTAL-13-VENCIMENTOS<br/>COMPOSICAO"]
        TOT13_D["TOTAL-13-DESCONTOS<br/>COMPOSICAO"]
        LIQ13["LIQUIDO-13<br/>TOTAL-13-VENCIMENTOS - TOTAL-13-DESCONTOS<br/>FORMULA"]
        LIQ13_2["13-PARCELA-FINAL<br/>LIQUIDO-13 - 13-ADIANTAMENTO<br/>FORMULA"]
    end

    B13 --> V13_1
    V13_1 --> V13_2
    V13_1 --> INSS13
    V13_1 --> IRRF13
    V13_1 --> PENS13

    V13_1 --> TOT13_V
    INSS13 --> TOT13_D
    IRRF13 --> TOT13_D
    PENS13 --> TOT13_D

    TOT13_V --> LIQ13
    TOT13_D -->|"deduz"| LIQ13
    LIQ13 --> LIQ13_2
    V13_2 -->|"deduz"| LIQ13_2
```

---

## Diagrama de Composição: Férias

```mermaid
graph TD
    subgraph "Base Férias"
        BFER["BASE-FERIAS<br/>Média 12 meses de vencimentos<br/>com incide_ferias=true<br/>COMPOSICAO"]
    end

    subgraph "Vencimentos Férias"
        FER["FERIAS<br/>(BASE-FERIAS / 30) × dias_gozo<br/>FORMULA"]
        TERCO["1-3-FERIAS<br/>FERIAS / 3<br/>FORMULA"]
        ABONO["ABONO-PECUNIARIO<br/>(BASE-FERIAS / 30) × dias_abono<br/>FORMULA"]
        TERCO_ABONO["1-3-ABONO<br/>ABONO-PECUNIARIO / 3<br/>FORMULA"]
    end

    subgraph "Descontos Férias"
        INSS_FER["INSS-FERIAS<br/>Tabela Progressiva<br/>TABELA_PROGRESSIVA"]
        IRRF_FER["IRRF-FERIAS<br/>Tabela Progressiva<br/>TABELA_PROGRESSIVA"]
    end

    subgraph "Totais Férias"
        TOT_FER_V["TOTAL-FERIAS-VENCIMENTOS<br/>COMPOSICAO"]
        TOT_FER_D["TOTAL-FERIAS-DESCONTOS<br/>COMPOSICAO"]
        LIQ_FER["LIQUIDO-FERIAS<br/>FORMULA"]
    end

    BFER --> FER
    BFER --> ABONO
    FER --> TERCO
    ABONO --> TERCO_ABONO

    FER --> INSS_FER
    TERCO --> INSS_FER
    ABONO --> INSS_FER
    TERCO_ABONO --> INSS_FER

    FER --> IRRF_FER
    TERCO --> IRRF_FER
    ABONO --> IRRF_FER
    TERCO_ABONO --> IRRF_FER

    FER --> TOT_FER_V
    TERCO --> TOT_FER_V
    ABONO --> TOT_FER_V
    TERCO_ABONO --> TOT_FER_V

    INSS_FER --> TOT_FER_D
    IRRF_FER --> TOT_FER_D

    TOT_FER_V --> LIQ_FER
    TOT_FER_D -->|"deduz"| LIQ_FER
```

---

## Diagrama de Composição: Rescisão

```mermaid
graph TD
    subgraph "Verbas Rescisórias"
        SALDO["SALDO-SALARIO<br/>Salário Base / 30 × dias<br/>FORMULA"]
        AVISO["AVISO-PREVIO<br/>Salário Base + médias<br/>FORMULA"]
        DEC13["13-PROPORCIONAL<br/>Salário Base / 12 × meses<br/>FORMULA"]
        FER_PROP["FERIAS-PROPORCIONAIS<br/>Salário Base / 12 × meses × 1.3333<br/>FORMULA"]
        FER_VENC["FERIAS-VENCIDAS<br/>Salário Base × 1.3333<br/>FORMULA"]
        MULTA_FGTS["MULTA-FGTS<br/>40% sobre saldo FGTS<br/>PERCENTUAL"]
    end

    subgraph "Descontos Rescisão"
        INSS_RES["INSS-RESCISAO<br/>Tabela Progressiva<br/>TABELA_PROGRESSIVA"]
        IRRF_RES["IRRF-RESCISAO<br/>Tabela Progressiva<br/>TABELA_PROGRESSIVA"]
        PENS_RES["PENSAO-RESCISAO<br/>% sobre verbas<br/>PERCENTUAL"]
    end

    subgraph "Totais Rescisão"
        TOT_RES_V["TOTAL-RESCISAO-VENCIMENTOS<br/>COMPOSICAO"]
        TOT_RES_D["TOTAL-RESCISAO-DESCONTOS<br/>COMPOSICAO"]
        LIQ_RES["LIQUIDO-RESCISAO<br/>FORMULA"]
    end

    SALDO --> TOT_RES_V
    AVISO --> TOT_RES_V
    DEC13 --> TOT_RES_V
    FER_PROP --> TOT_RES_V
    FER_VENC --> TOT_RES_V
    MULTA_FGTS --> TOT_RES_V

    SALDO --> INSS_RES
    AVISO --> INSS_RES
    DEC13 --> INSS_RES

    SALDO --> IRRF_RES
    AVISO --> IRRF_RES
    DEC13 --> IRRF_RES

    SALDO --> PENS_RES
    AVISO --> PENS_RES

    INSS_RES --> TOT_RES_D
    IRRF_RES --> TOT_RES_D
    PENS_RES --> TOT_RES_D

    TOT_RES_V --> LIQ_RES
    TOT_RES_D -->|"deduz"| LIQ_RES
```

---

## Diagrama de Dependência: Incidências

```mermaid
graph LR
    subgraph "Rubricas de Vencimento"
        V1["Salário Base"]
        V2["Horas Extras"]
        V3["Adicional Noturno"]
        V4["Adic. Periculosidade"]
        V5["Comissões"]
        V6["DSR"]
    end

    subgraph "Incidências"
        I1["INSS<br/>incide_inss=true"]
        I2["IRRF<br/>incide_irrf=true"]
        I3["FGTS<br/>incide_fgts=true"]
        I4["13º Salário<br/>incide_decimo_terceiro=true"]
        I5["Férias<br/>incide_ferias=true"]
        I6["Rescisão<br/>incide_rescisao=true"]
    end

    V1 --> I1
    V1 --> I2
    V1 --> I3
    V1 --> I4
    V1 --> I5
    V1 --> I6

    V2 --> I1
    V2 --> I2
    V2 --> I3
    V2 --> I4
    V2 --> I5
    V2 --> I6

    V3 --> I1
    V3 --> I2
    V3 --> I3
    V3 --> I4
    V3 --> I5
    V3 --> I6

    V4 --> I1
    V4 --> I2
    V4 --> I3
    V4 --> I4
    V4 --> I5
    V4 --> I6

    V5 --> I1
    V5 --> I2
    V5 --> I3
    V5 --> I4
    V5 --> I5

    V6 --> I1
    V6 --> I2
    V6 --> I3
    V6 --> I4
    V6 --> I5
    V6 --> I6
```

---

## Diagrama de Composição: Dissídio Coletivo

```mermaid
graph TD
    subgraph "Rubricas Afetadas pelo Dissídio"
        SAL_D["SAL-BASE<br/>Reajustado em 5%"]
        HE50_D["HORA-EXTRA-50<br/>Reajustado (base nova)"]
        AN_D["ADIC-NOTURNO<br/>Reajustado (base nova)"]
        AP_D["ADIC-PERICULOSIDADE<br/>Reajustado (base nova)"]
    end

    subgraph "Diferenças Retroativas"
        DIF_SAL["DIF-DISSIDIO-SALARIO<br/>(novo - antigo) × meses_retroativos<br/>FORMULA"]
        DIF_HE["DIF-DISSIDIO-HE<br/>(novo - antigo) × meses_retroativos<br/>FORMULA"]
        DIF_AN["DIF-DISSIDIO-AN<br/>(novo - antigo) × meses_retroativos<br/>FORMULA"]
    end

    subgraph "Encargos sobre Diferenças"
        INSS_DIF["INSS-DISSIDIO<br/>TABELA_PROGRESSIVA"]
        IRRF_DIF["IRRF-DISSIDIO<br/>TABELA_PROGRESSIVA"]
        FGTS_DIF["FGTS-DISSIDIO<br/>PERCENTUAL"]
    end

    subgraph "Totais Dissídio"
        TOT_DIS_V["TOTAL-DISSIDIO-VENCIMENTOS<br/>COMPOSICAO"]
        TOT_DIS_D["TOTAL-DISSIDIO-DESCONTOS<br/>COMPOSICAO"]
        LIQ_DIS["LIQUIDO-DISSIDIO<br/>FORMULA"]
    end

    SAL_D --> DIF_SAL
    HE50_D --> DIF_HE
    AN_D --> DIF_AN

    DIF_SAL --> TOT_DIS_V
    DIF_HE --> TOT_DIS_V
    DIF_AN --> TOT_DIS_V

    DIF_SAL --> INSS_DIF
    DIF_HE --> INSS_DIF
    DIF_AN --> INSS_DIF

    DIF_SAL --> IRRF_DIF
    DIF_HE --> IRRF_DIF
    DIF_AN --> IRRF_DIF

    DIF_SAL --> FGTS_DIF
    DIF_HE --> FGTS_DIF
    DIF_AN --> FGTS_DIF

    INSS_DIF --> TOT_DIS_D
    IRRF_DIF --> TOT_DIS_D
    FGTS_DIF --> TOT_DIS_D

    TOT_DIS_V --> LIQ_DIS
    TOT_DIS_D -->|"deduz"| LIQ_DIS
```

---

## Diagrama de Composição: Folha Complementar

```mermaid
graph TD
    subgraph "Folha Original (Período Fechado)"
        ORIG_V["TOTAL-VENCIMENTOS-ORIG<br/>Valor original"]
        ORIG_D["TOTAL-DESCONTOS-ORIG<br/>Valor original"]
        ORIG_L["LIQUIDO-ORIG<br/>Valor original"]
    end

    subgraph "Ajustes (Rubricas de Diferença)"
        DIF_SALARIAL["DIF-SALARIAL<br/>Correção de valor<br/>VALOR_FIXO"]
        DIF_RUB_INCLUSA["Rubrica Inclusa<br/>Rubrica esquecida<br/>VALOR_FIXO"]
        DIF_RUB_EXCLUSA["Rubrica Excluída<br/>Rubrica indevida (negativo)<br/>VALOR_FIXO"]
    end

    subgraph "Nova Folha (Recalculada)"
        NOVO_V["TOTAL-VENCIMENTOS-NOVO<br/>ORIG + DIF_SALARIAL + INCLUSA - EXCLUSA<br/>COMPOSICAO"]
        NOVO_D["TOTAL-DESCONTOS-NOVO<br/>Recalculado sobre nova base<br/>COMPOSICAO"]
        NOVO_L["LIQUIDO-NOVO<br/>FORMULA"]
    end

    subgraph "Folha Complementar (Diferença)"
        COMPL_V["COMPL-VENCIMENTOS<br/>NOVO_V - ORIG_V<br/>FORMULA"]
        COMPL_D["COMPL-DESCONTOS<br/>NOVO_D - ORIG_D<br/>FORMULA"]
        COMPL_L["COMPL-LIQUIDO<br/>COMPL_V - COMPL_D<br/>FORMULA"]
    end

    ORIG_V --> NOVO_V
    DIF_SALARIAL --> NOVO_V
    DIF_RUB_INCLUSA --> NOVO_V
    DIF_RUB_EXCLUSA -->|"deduz"| NOVO_V

    NOVO_V --> NOVO_D
    NOVO_V --> NOVO_L
    NOVO_D -->|"deduz"| NOVO_L

    NOVO_V --> COMPL_V
    ORIG_V -->|"subtrai"| COMPL_V
    NOVO_D --> COMPL_D
    ORIG_D -->|"subtrai"| COMPL_D

    COMPL_V --> COMPL_L
    COMPL_D -->|"deduz"| COMPL_L
```

---

## Diagrama de Composição: Salário-Maternidade

```mermaid
graph TD
    subgraph "Base de Cálculo"
        SAL_MAT["SALARIO-BASE<br/>Salário mensal<br/>VALOR_FIXO"]
        TETO_INSS_M["TETO-INSS<br/>Valor teto INSS<br/>TETO"]
    end

    subgraph "Benefício INSS"
        BEN_INSS["SAL-MATERNIDADE-INSS<br/>MIN(SALARIO_BASE, TETO_INSS)<br/>CONDICIONAL"]
    end

    subgraph "Complemento Patronal"
        COMPL_MAT["COMPL-SAL-MATERNIDADE<br/>SALARIO_BASE - BEN_INSS<br/>CONDICIONAL"]
    end

    subgraph "Descontos sobre Complemento"
        INSS_MAT["INSS-MATERNIDADE<br/>TABELA_PROGRESSIVA"]
        IRRF_MAT["IRRF-MATERNIDADE<br/>TABELA_PROGRESSIVA"]
    end

    subgraph "Totais"
        TOT_MAT_V["TOTAL-MATERNIDADE<br/>COMPOSICAO"]
        TOT_MAT_D["TOTAL-MATERNIDADE-DESC<br/>COMPOSICAO"]
        LIQ_MAT["LIQUIDO-MATERNIDADE<br/>FORMULA"]
    end

    SAL_MAT --> BEN_INSS
    TETO_INSS_M --> BEN_INSS
    SAL_MAT --> COMPL_MAT
    BEN_INSS -->|"subtrai"| COMPL_MAT

    COMPL_MAT --> INSS_MAT
    COMPL_MAT --> IRRF_MAT

    BEN_INSS --> TOT_MAT_V
    COMPL_MAT --> TOT_MAT_V

    INSS_MAT --> TOT_MAT_D
    IRRF_MAT --> TOT_MAT_D

    TOT_MAT_V --> LIQ_MAT
    TOT_MAT_D -->|"deduz"| LIQ_MAT
```

---

## Diagrama de Composição: Acordo Trabalhista

```mermaid
graph TD
    subgraph "Verbas do Acordo"
        SALDO_A["SALDO-SALARIO<br/>Integral<br/>FORMULA"]
        AVISO_A["AVISO-PREVIO-ACORDO<br/>50% do valor<br/>FORMULA"]
        DEC13_A["13-PROPORCIONAL<br/>Integral<br/>FORMULA"]
        FER_PROP_A["FERIAS-PROPORCIONAIS<br/>Integral + 1/3<br/>FORMULA"]
        FER_VENC_A["FERIAS-VENCIDAS<br/>Integral + 1/3<br/>FORMULA"]
    end

    subgraph "Multa FGTS Acordo"
        MULTA_FGTS_A["MULTA-FGTS-ACORDO<br/>20% sobre saldo FGTS<br/>PERCENTUAL"]
    end

    subgraph "Descontos"
        INSS_A["INSS-ACORDO<br/>TABELA_PROGRESSIVA"]
        IRRF_A["IRRF-ACORDO<br/>TABELA_PROGRESSIVA"]
    end

    subgraph "Totais"
        TOT_A_V["TOTAL-ACORDO-VENCIMENTOS<br/>COMPOSICAO"]
        TOT_A_D["TOTAL-ACORDO-DESCONTOS<br/>COMPOSICAO"]
        LIQ_A["LIQUIDO-ACORDO<br/>FORMULA"]
    end

    SALDO_A --> TOT_A_V
    AVISO_A --> TOT_A_V
    DEC13_A --> TOT_A_V
    FER_PROP_A --> TOT_A_V
    FER_VENC_A --> TOT_A_V
    MULTA_FGTS_A --> TOT_A_V

    SALDO_A --> INSS_A
    AVISO_A --> INSS_A
    DEC13_A --> INSS_A

    SALDO_A --> IRRF_A
    AVISO_A --> IRRF_A
    DEC13_A --> IRRF_A

    INSS_A --> TOT_A_D
    IRRF_A --> TOT_A_D

    TOT_A_V --> LIQ_A
    TOT_A_D -->|"deduz"| LIQ_A
```

---

## Diagrama de Dependência: Incidências (Completo)

```mermaid
graph LR
    subgraph "Rubricas de Vencimento"
        V1["Salário Base"]
        V2["Horas Extras"]
        V3["Adicional Noturno"]
        V4["Adic. Periculosidade"]
        V5["Comissões"]
        V6["DSR"]
    end

    subgraph "Incidências"
        I1["INSS"]
        I2["IRRF"]
        I3["FGTS"]
        I4["13º Salário"]
        I5["Férias"]
        I6["Rescisão"]
        I7["Dissídio<br/>incide_dissidio"]
        I8["Salário-Maternidade<br/>incide_salario_maternidade"]
        I9["Auxílio-Doença<br/>incide_auxilio_doenca"]
        I10["Adiantamento<br/>incide_adiantamento"]
    end

    V1 --> I1; V1 --> I2; V1 --> I3; V1 --> I4; V1 --> I5; V1 --> I6; V1 --> I7; V1 --> I8; V1 --> I9; V1 --> I10
    V2 --> I1; V2 --> I2; V2 --> I3; V2 --> I4; V2 --> I5; V2 --> I6; V2 --> I7
    V3 --> I1; V3 --> I2; V3 --> I3; V3 --> I4; V3 --> I5; V3 --> I6; V3 --> I7
    V4 --> I1; V4 --> I2; V4 --> I3; V4 --> I4; V4 --> I5; V4 --> I6; V4 --> I7
    V5 --> I1; V5 --> I2; V5 --> I3; V5 --> I4; V5 --> I5
    V6 --> I1; V6 --> I2; V6 --> I3; V6 --> I4; V6 --> I5; V6 --> I6; V6 --> I7
```

---

## Diagrama de Fluxo: Cadastro e Atualização de Rubrica

```mermaid
sequenceDiagram
    actor Admin as Admin RH
    participant CAD as api-cadastros
    participant DB as PostgreSQL
    participant Cache as Redis
    participant RMQ as RabbitMQ
    participant FOL as api-folha
    participant ES as api-esocial

    Admin->>CAD: POST /api/rubricas {codigo, descricao, natureza, ...}
    CAD->>CAD: Valida dados (FluentValidation)
    CAD->>CAD: Valida tipo_esocial contra Tabela 03
    CAD->>CAD: Valida fórmula (sintaxe + segurança)
    CAD->>CAD: Valida composição (sem ciclos)
    CAD->>DB: INSERT INTO rubrica
    CAD->>DB: INSERT INTO rubrica_historico (criação)
    CAD->>DB: INSERT INTO rubrica_formula (se aplicável)
    CAD->>DB: INSERT INTO rubrica_composicao (se aplicável)
    CAD->>Cache: PUBLISH invalidate:rubricas
    CAD->>RMQ: Publica RubricaAlterada {rubricaId, empresaId, acao: 'CRIACAO'}
    CAD-->>Admin: 201 Created {rubricaId}

    Cache-->>FOL: Notifica invalidação de cache
    FOL->>Cache: Invalida cache local de rubricas

    RMQ-->>ES: Consome RubricaAlterada
    ES->>ES: Valida compatibilidade com Tabela 03
```

---

## Análise de Impacto: Alteração de Rubrica

| Alteração | Impacto | Módulos Afetados | Mitigação |
|---|---|---|---|
| Alterar `incide_inss` | Muda base de cálculo INSS para todos os funcionários | Cálculo Folha, Obrigações Fiscais | Versionar rubrica; aplicar a partir do próximo período |
| Alterar `incide_dissidio` | Afeta base de cálculo de dissídio | Cálculo Folha | Validar contra convenção coletiva; publicar `RubricaAlterada` |
| Alterar `incide_salario_maternidade` | Altera complemento de salário-maternidade | Cálculo Folha | Revisar impacto em licenças ativas |
| Alterar `formula_calculo` | Muda valor calculado para funcionários | Cálculo Folha | Invalidar cache; reprocessar período atual se necessário |
| Alterar `tipo_esocial` | Muda classificação no e-Social | Integração e-Social | Validar contra Tabela 03; publicar `RubricaAlterada` |
| Desativar rubrica (`ativo=false`) | Remove rubrica dos cálculos futuros | Cálculo Folha | Soft disable; manter em períodos já fechados |
| Alterar `rubrica_base_id` | Muda referência para cálculo percentual | Cálculo Folha | Validar existência da nova base; invalidar cache |
| Alterar composição | Muda valor de rubricas compostas | Cálculo Folha | Recalcular totais; invalidar cache |
| Alterar `tipo_calculo` | Muda completamente a lógica da rubrica | Cálculo Folha, e-Social | Versionar como nova rubrica; manter histórico da anterior |
| Criar dissídio | Reajusta múltiplas rubricas com retroativo | Cálculo Folha, Obrigações Fiscais | Gerar folha complementar; recalcular impostos retroativos |

---

## Tipos de Cálculo do Motor (Catálogo Completo)

| Tipo | Descrição | Exemplo de Uso | Parâmetros |
|---|---|---|---|
| `VALOR_FIXO` | Valor monetário fixo | Salário Base, Gratificação, Auxílio Creche | `valor_fixo` |
| `UNIDADE` | Valor unitário × quantidade | Vale Transporte (R$ 5,00 × 44), Vale Refeição | `valor_fixo`, variável `QUANTIDADE` |
| `HORA` | Quantidade de horas × valor hora × percentual | Horas Extras 50%, Horas Extras 100% | `percentual`, variável `QUANTIDADE_HORAS` |
| `DIA` | Quantidade de dias × valor dia | Saldo de Salário, Recesso Estágio, Licença | variável `QUANTIDADE_DIAS` |
| `PERCENTUAL` | Percentual sobre rubrica base | Adicional Noturno, Periculosidade, Comissão | `percentual`, `rubrica_base_id` |
| `MEDIA` | Média de rubricas nos últimos N meses | Média HE p/ Férias, Média Comissões p/ 13º | `rubrica_media` (qtd_meses, tipo, rubricas_origem) |
| `FORMULA` | Expressão matemática parametrizável | DSR, Provisões, Diferenças | `rubrica_formula` (expressao, parametros) |
| `COMPOSICAO` | Soma/subtração de rubricas componentes | Total Vencimentos, Base INSS, Base IRRF | `rubrica_composicao` (componentes, operadores) |
| `TABELA_PROGRESSIVA` | Alíquota progressiva por faixa | INSS, IRRF, Salário Família | `rubrica_tabela_progressiva` (faixas, aliquotas, deduções) |
| `TETO` | Aplica valor máximo (teto legal) | Teto INSS (R$ 7.786,02), Teto Salário Família | `teto_maximo`, `rubrica_base_id` |
| `CONDICIONAL` | Se condição X então Y senão Z | INSS com teto, IRRF com dedução dependente, FGTS rescisão | `rubrica_condicional` (condicao, valor_se_verdadeiro, valor_se_falso) |

---

## Referências Cruzadas

- [Database Model — Rubricas](./database-model-rubricas.md)
- [Runtime View — Cálculo com Rubricas](./runtime-view-calculo-rubricas.md)
- [Plano de Ação — Rubricas](./plano-acao-rubricas.md)
- [PRD-F02 — Gestão de Cadastros](../../tasks/prd-f02-gestao-cadastros/prd.md)
- [Component Boundaries](../arquitetura/component-boundaries.md)
