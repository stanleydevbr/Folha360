# PRD G09 — Módulo Décimo Terceiro Salário

## Visão Geral

O módulo Décimo Terceiro processa o cálculo do 13º salário utilizando o motor de cálculo da 
folha (G07) com `TipoCalculo = DecimoTerceiro`. Processa a 1ª parcela (adiantamento, sem 
descontos) e a 2ª parcela (complemento, com INSS e IRRF).

**Problema resolvido**: O DP precisa calcular o 13º em duas parcelas distintas, com médias 
de verbas variáveis (horas extras, comissões) e aplicar descontos apenas na 2ª parcela, 
conforme legislação (Lei 4.090/62 e Lei 4.749/65).

**Público-alvo**: Operador DP.

## Objetivos

- **Conformidade legal**: 1ª parcela sem descontos, 2ª parcela com INSS/IRRF
- **Precisão**: Cálculo de médias de verbas variáveis (horas extras, comissões)
- **Prazo**: 1ª parcela até 30/novembro, 2ª parcela até 20/dezembro

## Histórias de Usuário

1. **Como Operador DP**, quero processar a 1ª parcela do 13º (adiantamento de 50%) para 
   pagamento até 30 de novembro.
2. **Como Operador DP**, quero processar a 2ª parcela do 13º com descontos de INSS e IRRF 
   para pagamento até 20 de dezembro.
3. **Como Operador DP**, quero calcular médias de horas extras e comissões para compor a 
   base do 13º corretamente.

## Funcionalidades Principais

### F1. Primeira Parcela
**Requisitos funcionais**:
- RF01: O sistema DEVE processar 1ª parcela via `POST /api/folha/processar` com TipoCalculo = DecimoTerceiro
- RF02: O sistema DEVE calcular 50% do salário base (ou proporcional aos meses trabalhados)
- RF03: O sistema DEVE não aplicar descontos na 1ª parcela

### F2. Segunda Parcela
**Requisitos funcionais**:
- RF04: O sistema DEVE processar 2ª parcela como complemento (total devido - 1ª parcela)
- RF05: O sistema DEVE aplicar descontos de INSS e IRRF sobre o valor total do 13º
- RF06: O sistema DEVE calcular médias de verbas variáveis (horas extras, comissões) do ano

### F3. Holerites do 13º
**Requisitos funcionais**:
- RF07: O sistema DEVE gerar holerites distintos para 1ª e 2ª parcelas

## Experiência do Usuário

Fluxo similar à folha mensal, com seleção de TipoCalculo = DecimoTerceiro. O sistema deve 
diferenciar claramente 1ª e 2ª parcelas na interface.

## Restrições Técnicas de Alto Nível

- **Motor**: Reutiliza `MotorCalculo` e `CalculadorMedia` do módulo G07
- **Regras**: Lei 4.090/62 (13º salário), Lei 4.749/65 (pagamento em parcelas), 
  INSS/IRRF exclusivos na 2ª parcela

## Fora de Escopo

- 13º proporcional na rescisão (módulo Rescisão — G10)

---

## Anexo A — Referência de Endpoints

Compartilha endpoints do módulo Folha (G07): `POST /api/folha/processar` (TipoCalculo=DecimoTerceiro).

Ver detalhes em: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 10.
