# PRD G07 — Módulo Folha de Pagamento

## Visão Geral

O módulo Folha de Pagamento é o núcleo operacional do Folha360. Executa o processamento 
completo da folha: aplica rubricas, calcula vencimentos e descontos, gera holerites e 
gerencia a cadeia de fechamento com suporte a reabertura versionada.

**Problema resolvido**: O DP precisa processar a folha de centenas de funcionários em minutos, 
com rastreabilidade completa de cada item calculado (fórmula aplicada, base de cálculo), 
suporte a reprocessamento e auditoria de versões.

**Público-alvo**: Operador DP (processamento), Contador (fechamento).

## Objetivos

- **Performance**: Processar 1.000 funcionários em menos de 5 minutos
- **Confiabilidade**: 100% de rastreabilidade dos itens calculados (fórmula, base, valor)
- **Auditoria**: Versionamento de processamentos com suporte a reabertura
- **Flexibilidade**: Suportar 11 tipos de cálculo (Mensal, Férias, 13º, Rescisão, Dissídio, 
  Complementar, Auxílio Doença, Salário Maternidade, Acordo, Estágio, RPA)

## Histórias de Usuário

1. **Como Operador DP**, quero iniciar o processamento da folha para uma empresa e período 
   para calcular vencimentos e descontos de todos os funcionários.
2. **Como Operador DP**, quero acompanhar o progresso do processamento (funcionários 
   processados, com erro) para agir rapidamente em caso de falhas.
3. **Como Operador DP**, quero visualizar os itens calculados de cada funcionário (rubrica, 
   base de cálculo, valor, fórmula aplicada) para auditar o resultado.
4. **Como Operador DP**, quero reprocessar a folha após corrigir inconsistências para 
   obter um resultado correto.
5. **Como Operador DP**, quero reabrir uma folha já fechada com justificativa e registro 
   de autor para correções pós-fechamento.
6. **Como Operador DP**, quero consultar o histórico de processamentos de um período para 
   rastrear versões e reaberturas.
7. **Como Operador DP**, quero gerar holerites em lote (PDF) para distribuir aos funcionários.
8. **Como Contador**, quero verificar a cadeia de fechamento (folha processada → obrigações 
   apuradas → eventos e-Social enviados → fechamento concluído).

## Funcionalidades Principais

### F1. Processamento da Folha
**Requisitos funcionais**:
- RF01: O sistema DEVE iniciar processamento assíncrono (202 Accepted) com empresa, período e tipo de cálculo
- RF02: O sistema DEVE suportar 11 tipos de cálculo
- RF03: O sistema DEVE retornar status: Pendente, Em Processamento, Concluído, Falho, Cancelado, Reaberta
- RF04: O sistema DEVE contabilizar total de funcionários, processados, com erro e totais financeiros
- RF05: O sistema DEVE expor endpoint de cancelamento de processamento em andamento

### F2. Itens da Folha
**Requisitos funcionais**:
- RF06: O sistema DEVE registrar cada item calculado com: rubrica, fase (Vencimentos/Bases/Descontos/Totais), 
  base de cálculo, valor, fórmula aplicada e ordem
- RF07: O sistema DEVE permitir filtrar itens por funcionário

### F3. Reprocessamento e Reabertura
**Requisitos funcionais**:
- RF08: O sistema DEVE permitir reprocessar uma folha a partir de um processamento existente
- RF09: O sistema DEVE permitir reabrir folha fechada com motivo e autor registrados
- RF10: O sistema DEVE versionar automaticamente a cada reabertura

### F4. Holerites
**Requisitos funcionais**:
- RF11: O sistema DEVE gerar holerites em PDF armazenados no MinIO
- RF12: O sistema DEVE expor endpoint de download do PDF por funcionário

### F5. Cadeia de Fechamento
**Requisitos funcionais**:
- RF13: O sistema DEVE gerenciar etapas de fechamento: Folha Processada, Obrigações Apuradas, 
  Eventos e-Social Enviados, Fechamento Concluído
- RF14: O sistema DEVE versionar a cadeia de fechamento com histórico

## Experiência do Usuário

**Fluxo principal**: Dashboard da folha → Selecionar empresa/período → "Processar Folha" → 
Acompanhar progresso (barra de progresso) → Revisar itens → Reprocessar se necessário → 
Aprovar fechamento → Gerar holerites em lote.

**Acessibilidade**: Status de processamento deve ser anunciado via aria-live para leitores 
de tela. Tabelas de itens calculados com navegação por teclado.

## Restrições Técnicas de Alto Nível

- **Processamento assíncrono**: Filas RabbitMQ para não bloquear o usuário
- **Motor de cálculo**: `MotorCalculo` com resolvedor de composição, avaliador de expressão 
  (NCalc), aplicador de tabela progressiva e calculador de média
- **Armazenamento**: Holerites em PDF no MinIO
- **Versionamento**: Cada reabertura gera nova versão do processamento

## Fora de Escopo

- Configuração de rubricas (módulo Rubricas — G05)
- Apuração de tributos (módulo Fiscais — G11)
- Envio ao e-Social (módulo e-Social — G12)
- Relatórios gerenciais (módulo Relatórios — G13)

---

## Anexo A — Referência de Endpoints

| Funcionalidade | Controller | Base Path |
|---------------|-----------|-----------|
| Processamento | `ProcessamentoController` | `/api/folha` |
| Holerites | `HoleriteController` | `/api/folha/holerites` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 7.  
Jornada: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 4.
