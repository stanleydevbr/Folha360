# PRD G10 — Módulo Rescisão

## Visão Geral

O módulo Rescisão processa o cálculo de verbas rescisórias utilizando o motor de cálculo da 
folha (G07) com `TipoCalculo = Rescisao`. Calcula saldo de salário, aviso prévio (indenizado 
ou trabalhado), 13º proporcional, férias vencidas e proporcionais (com 1/3), e multa de 40% 
do FGTS.

**Problema resolvido**: O DP precisa calcular a rescisão completa com todas as verbas legais, 
gerar o TRCT (Termo de Rescisão do Contrato de Trabalho) e as guias rescisórias (FGTS, GRRF).

**Público-alvo**: Operador DP.

## Objetivos

- **Completude**: Calcular todas as verbas rescisórias conforme CLT
- **Precisão**: Distinguir corretamente entre aviso prévio indenizado e trabalhado
- **Documentação**: Gerar TRCT em PDF e guias rescisórias
- **Prazo**: Cálculo concluído em até 30 minutos após o registro do desligamento

## Histórias de Usuário

1. **Como Operador DP**, quero processar a folha de rescisão para calcular saldo de salário, 
   aviso prévio, 13º proporcional, férias vencidas/proporcionais e multa FGTS.
2. **Como Operador DP**, quero gerar o TRCT (holerite rescisório) em PDF para homologação.
3. **Como Operador DP**, quero gerar as guias rescisórias (FGTS, GRRF) para saque pelo funcionário.

## Funcionalidades Principais

### F1. Cálculo Rescisório
**Requisitos funcionais**:
- RF01: O sistema DEVE processar rescisão via `POST /api/folha/processar` com TipoCalculo = Rescisao
- RF02: O sistema DEVE calcular saldo de salário (dias trabalhados no mês)
- RF03: O sistema DEVE calcular aviso prévio indenizado (30 dias + 3 dias por ano trabalhado)
- RF04: O sistema DEVE calcular 13º proporcional (1/12 por mês trabalhado ≥ 15 dias)
- RF05: O sistema DEVE calcular férias vencidas (simples e em dobro) + 1/3
- RF06: O sistema DEVE calcular férias proporcionais (1/12 por mês) + 1/3
- RF07: O sistema DEVE calcular multa de 40% do FGTS (50% em acordo mútuo)

### F2. Documentos Rescisórios
**Requisitos funcionais**:
- RF08: O sistema DEVE gerar TRCT em PDF (holerite rescisório)
- RF09: O sistema DEVE gerar guia GRRF (FGTS rescisório)

## Experiência do Usuário

Fluxo integrado: Evento de desligamento (G06) → Processar folha com TipoCalculo = Rescisao → 
Verificar itens (saldo, aviso, 13º, férias, multa FGTS) → Gerar TRCT → Gerar guias.

## Restrições Técnicas de Alto Nível

- **Motor**: Reutiliza `MotorCalculo` do módulo G07
- **Regras**: CLT art. 477 (prazo pagamento), art. 478-480 (aviso prévio e multa FGTS), 
  art. 146-148 (férias proporcionais), Lei 12.506/2011 (aviso prévio proporcional)

## Fora de Escopo

- Registro do evento de desligamento (módulo Eventos — G06)
- Homologação digital da rescisão (feature futura)

---

## Anexo A — Referência de Endpoints

Compartilha endpoints do módulo Folha (G07): `POST /api/folha/processar` (TipoCalculo=Rescisao).

Ver detalhes em: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 9.
