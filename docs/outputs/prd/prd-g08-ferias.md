# PRD G08 — Módulo Férias (Cálculo)

## Visão Geral

O módulo Férias processa o cálculo específico de férias utilizando o motor de cálculo da 
folha (G07) com `TipoCalculo = Ferias`. Calcula 1/3 constitucional, abono pecuniário, 
adiantamento de 13º e descontos de INSS/IRRF sobre férias.

**Problema resolvido**: O DP precisa calcular férias com todas as verbas legais (CF/88, CLT), 
gerar recibo de férias e integrar com o evento e-Social S-2230.

**Público-alvo**: Operador DP.

## Objetivos

- **Precisão**: Cálculo correto de 1/3 constitucional, abono pecuniário e descontos
- **Integração**: Vinculado ao evento de férias (G06) e ao motor de cálculo (G07)
- **Documentação**: Recibo de férias em PDF

## Histórias de Usuário

1. **Como Operador DP**, quero processar férias de um funcionário para calcular o valor a 
   receber no período de gozo.
2. **Como Operador DP**, quero calcular abono pecuniário (venda de 1/3 das férias) quando 
   solicitado pelo funcionário.
3. **Como Operador DP**, quero gerar o recibo de férias em PDF para documentação e entrega.

## Funcionalidades Principais

### F1. Cálculo de Férias
**Requisitos funcionais**:
- RF01: O sistema DEVE processar férias via `POST /api/folha/processar` com TipoCalculo = Ferias
- RF02: O sistema DEVE calcular 1/3 constitucional automaticamente
- RF03: O sistema DEVE suportar abono pecuniário (conversão de 1/3 em dinheiro)
- RF04: O sistema DEVE calcular adiantamento de 13º quando solicitado
- RF05: O sistema DEVE aplicar descontos de INSS e IRRF sobre férias

### F2. Recibo de Férias
**Requisitos funcionais**:
- RF06: O sistema DEVE gerar holerite específico de férias em PDF
- RF07: O sistema DEVE exibir período aquisitivo no recibo

## Experiência do Usuário

Fluxo integrado: Evento de férias (G06) → Processar folha com TipoCalculo = Ferias → 
Verificar itens → Gerar recibo.

## Restrições Técnicas de Alto Nível

- **Motor**: Reutiliza `MotorCalculo` do módulo G07
- **Regras**: 1/3 constitucional (CF/88 art. 7º, XVII); abono pecuniário (CLT art. 143); 
  desconto INSS/IRRF sobre férias (legislação vigente)

## Fora de Escopo

- Registro do evento de férias (módulo Eventos — G06)
- Cálculo de férias coletivas com rateio por setor (feature futura)

---

## Anexo A — Referência de Endpoints

Compartilha endpoints do módulo Folha (G07): `POST /api/folha/processar` (TipoCalculo=Ferias), 
`GET /api/folha/processamento/{id}/itens`, `GET /api/folha/holerites/{id}/{funcionarioId}`.

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 7.
