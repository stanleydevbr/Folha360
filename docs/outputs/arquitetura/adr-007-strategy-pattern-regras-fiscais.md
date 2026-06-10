# ADR-007: Strategy Pattern para Regras Fiscais Versionadas

## Status
Aceito (2026-06-09)

## Context
O módulo F05 (Obrigações Fiscais) precisa calcular 8 tributos diferentes (IRRF, INSS, FGTS, PIS, COFINS, CSLL, Contribuição Sindical, ISS), cada um com regras de cálculo específicas que mudam anualmente conforme a legislação brasileira. As alíquotas, faixas, tetos e deduções variam por ano-calendário, e o sistema deve garantir que cada período use a regra vigente na data, sem afetar períodos já processados.

Além disso, diferentes regimes tributários da empresa (Simples Nacional, Lucro Presumido, Lucro Real) afetam as alíquotas aplicáveis para PIS, COFINS e CSLL. Para ISS, a alíquota varia por município.

## Decision drivers
- Regras fiscais mudam anualmente (ex.: tabela IRRF 2026 vs 2027)
- Cada tributo tem lógica de cálculo distinta
- Regime tributário da empresa afeta alíquotas
- Períodos já processados não podem ser afetados por novas regras
- Facilidade para cadastrar novas versões de regras sem deploy
- Testabilidade: cada regra deve ser testável isoladamente

## Options considered

### Opção 1: Switch/Case com if-else por tributo
- **Prós**: Simples de implementar
- **Contras**: Violação do Open/Closed Principle; código monolítico difícil de manter; adicionar novo tributo requer alterar o método central

### Opção 2: Chain of Responsibility
- **Prós**: Desacoplamento entre handlers
- **Contras**: Complexidade desnecessária para 8 tributos; ordem de execução implícita

### Opção 3: Strategy Pattern com Factory (ESCOLHIDO)
- **Prós**: Cada tributo encapsulado em sua própria classe; Open/Closed Principle respeitado; testes isolados por tributo; factory resolve a implementação correta por `Tributo` enum
- **Contras**: Mais arquivos (8 serviços + 1 factory); boilerplate de DI

## Decision
Implementar **Strategy Pattern** com 8 implementações de `IRegraFiscalService`, uma para cada tributo. A factory `IRegraFiscalFactory` resolve a implementação correta via dicionário `Tributo → IRegraFiscalService`.

Os parâmetros de cada regra (alíquotas, faixas, tetos) são armazenados como JSONB na tabela `regra_fiscal`, versionados por ano-calendário. A aplicação consulta a regra vigente por `(tributo, data)` e passa os parâmetros para o serviço correspondente.

### Estrutura
```
IRegraFiscalService (interface)
  └─ Calcular(ApuracaoContext, RegraFiscalParametros) → ApuracaoResult

Implementações:
  ├─ IrpfRegraFiscalService
  ├─ InssRegraFiscalService
  ├─ FgtsRegraFiscalService
  ├─ PisRegraFiscalService
  ├─ CofinsRegraFiscalService
  ├─ CsllRegraFiscalService
  ├─ SindicalRegraFiscalService
  └─ IssRegraFiscalService

IRegraFiscalFactory
  └─ Resolver(Tributo) → IRegraFiscalService
```

### Versionamento
- Tabela `regra_fiscal` com `vigencia_inicio` e `vigencia_fim`
- Query: `WHERE tributo = @tributo AND vigencia_inicio <= @data AND (vigencia_fim IS NULL OR vigencia_fim >= @data)`
- Nova versão não afeta períodos já processados

## Consequences
- **Positivas**: Cada tributo é testável isoladamente (33 testes unitários, 0 falhas); adicionar novo tributo requer apenas nova classe + registro na factory; parâmetros versionados permitem atualização sem deploy
- **Negativas**: 8 classes de serviço + 1 factory; overhead de DI para registrar cada implementação
- **Riscos mitigados**: Regras fiscais incorretas (testes unitários com valores conhecidos); mudanças legislativas (versionamento anual)

## Follow-up actions
- [x] Implementar 8 serviços de regra fiscal
- [x] Implementar factory
- [x] Criar testes unitários para cada serviço
- [ ] Criar seed data com regras fiscais 2026
- [ ] Criar endpoint para cadastro de novas versões (RF33)
- [ ] Monitorar Diário Oficial para atualizações anuais
