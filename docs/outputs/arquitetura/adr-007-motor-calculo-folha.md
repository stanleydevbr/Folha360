# ADR-007: Motor de Cálculo da Folha de Pagamento

## Title
Arquitetura do Motor de Cálculo com 4 Fases e Componentes Especializados

## Status
Accepted (Junho 2026)

## Context
O processamento da folha de pagamento é o core do sistema Folha360. É necessário um motor de cálculo que suporte 11 tipos de cálculo (mensal, férias, 13º, rescisão, dissídio, complementar, auxílio-doença, salário-maternidade, acordo, estágio, RPA), processe 100.000 funcionários em menos de 2 horas, e garanta rastreabilidade completa de cada valor calculado até sua rubrica de origem.

## Decision drivers

1. **Performance**: Processar 100K funcionários em < 2 horas
2. **Rastreabilidade**: Cada valor deve ser rastreável até sua rubrica e fórmula
3. **Testabilidade**: Componentes independentes e testáveis isoladamente
4. **Conformidade fiscal**: Cálculos de INSS, IRRF e FGTS seguindo legislação vigente
5. **Extensibilidade**: Suporte a 11 tipos de cálculo com regras específicas
6. **Resiliência**: Graceful degradation quando Redis indisponível

## Options considered

### Opção 1: Motor monolítico com switch-case por tipo de rubrica
- **Prós**: Simples de implementar
- **Contras**: Difícil de testar, baixa extensibilidade, acoplamento alto

### Opção 2: Motor com 4 fases sequenciais + componentes especializados (ESCOLHIDA)
- **Prós**: Alta testabilidade, componentes independentes, rastreabilidade por fase, extensível
- **Contras**: Maior complexidade inicial

### Opção 3: Micro-serviços de cálculo por tipo
- **Prós**: Escalabilidade independente
- **Contras**: Latência de rede, complexidade operacional, overengineering para MVP

## Decision

**Motor de Cálculo baseado em 4 fases sequenciais com componentes especializados**, implementado como Domain Services no módulo `Folha360.Processamento.Domain`.

### Arquitetura

```
MotorCalculo (orquestrador)
├── Fase 1: Vencimentos (ordem 1-99)
├── Fase 2: Bases (ordem 100-199)
├── Fase 3: Descontos (ordem 200-299)
└── Fase 4: Totais (ordem 300-399)

Componentes especializados:
├── AvaliadorExpressao: Fórmulas NCalc com sandbox
├── ResolvedorComposicao: Composições hierárquicas (DFS com detecção de ciclos)
├── AplicadorTabelaProgressiva: Tabelas IRRF/INSS versionadas
├── CalculadorMedia: Médias móveis (3, 6, 12 meses)
└── AvaliadorCondicional: Condições booleanas para rubricas condicionais
```

### Tecnologias
- **NCalc** (CoreCLR-NCalc 2.2.92): Avaliação de expressões matemáticas
- **Redis**: Cache de rubricas, tabelas progressivas e composições
- **MassTransit/RabbitMQ**: Processamento assíncrono
- **Task.WhenAll + SemaphoreSlim**: Paralelismo em batches de 1.000

## Consequences

### Positivas
- Componentes testáveis isoladamente (cobertura > 80%)
- Rastreabilidade completa: cada item_folha registra rubrica, fase, fórmula e base de cálculo
- Extensível: novos tipos de cálculo adicionam regras sem alterar o motor
- Cache Redis reduz latência de consulta a rubricas e tabelas

### Negativas
- Maior complexidade inicial (6 interfaces + implementações)
- Dependência do Redis para performance (com graceful degradation)
- Necessidade de sincronização entre cache e banco (pub/sub)

## Follow-up actions
- [x] Implementar 4 fases de processamento
- [x] Implementar 5 componentes especializados
- [x] Migrar MotorCalculo do Cadastros.Domain para Processamento.Domain
- [x] Configurar cache Redis com invalidação pub/sub
- [x] Teste de carga com amostra de 100 requisições
- [x] Relatório de performance documentado ([ver relatório](../features/relatorio-performance-f04.md))
- [ ] Teste de carga com 100K funcionários (requer seed data em massa)
