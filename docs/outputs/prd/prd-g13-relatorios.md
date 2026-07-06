# PRD G13 — Módulo Relatórios e Exportações

## Visão Geral

O módulo Relatórios gera todos os relatórios legais e gerenciais do Folha360: folha analítica 
e sintética, holerites em lote, resumos mensais e anuais, DIRF, RAIS, e permite agendamento 
de relatórios recorrentes com envio por e-mail.

**Problema resolvido**: O DP e o contador precisam gerar relatórios legais obrigatórios 
(DIRF, RAIS) e gerenciais (folha analítica, resumos) em formatos diversos (PDF, CSV, XML, 
JSON), com possibilidade de agendamento e distribuição automática.

**Público-alvo**: Operador DP (holerites, folha), Contador (DIRF, RAIS, resumos), 
Admin (materialized views).

## Objetivos

- **Cobertura legal**: DIRF e RAIS 100% compatíveis com programas oficiais
- **Flexibilidade**: Múltiplos formatos de exportação (PDF, CSV, XML, JSON)
- **Automação**: Agendamento de relatórios recorrentes com envio por e-mail
- **Performance**: Materialized views para consultas pesadas (DIRF, RAIS)

## Histórias de Usuário

1. **Como Operador DP**, quero gerar a folha analítica (detalhada por funcionário) para 
   conferência antes do fechamento.
2. **Como Operador DP**, quero gerar a folha sintética (totais por rubrica e departamento) 
   para visão consolidada.
3. **Como Operador DP**, quero gerar holerites em lote (PDF) para todos os funcionários 
   de uma vez.
4. **Como Contador**, quero gerar o resumo mensal com totais e variação percentual para 
   apresentar à diretoria.
5. **Como Contador**, quero gerar o resumo anual com totais mês a mês e médias.
6. **Como Contador**, quero gerar a DIRF anual para entrega à Receita Federal.
7. **Como Contador**, quero gerar a RAIS anual para entrega ao Ministério do Trabalho.
8. **Como Contador**, quero agendar relatórios recorrentes (ex: resumo mensal todo dia 5) 
   com envio automático por e-mail.
9. **Como Admin**, quero atualizar as materialized views para garantir dados frescos nos 
   relatórios.

## Funcionalidades Principais

### F1. Folha Analítica e Sintética
**Requisitos funcionais**:
- RF01: O sistema DEVE gerar folha analítica com itens por funcionário (vencimentos, descontos, líquido)
- RF02: O sistema DEVE gerar folha sintética com totais por rubrica e departamento
- RF03: O sistema DEVE permitir filtro por departamento e tipo de cálculo
- RF04: O sistema DEVE suportar exportação em JSON, CSV, XML e PDF

### F2. Resumos
**Requisitos funcionais**:
- RF05: O sistema DEVE gerar resumo mensal com totais (funcionários, vencimentos, descontos, 
  líquido, IRRF, INSS, FGTS) e variação vs. mês anterior
- RF06: O sistema DEVE gerar resumo anual com totais mês a mês e médias

### F3. Relatórios Legais
**Requisitos funcionais**:
- RF07: O sistema DEVE gerar DIRF com rendimentos tributáveis, isentos, IRRF retido, 
  13º e férias por funcionário
- RF08: O sistema DEVE gerar RAIS com vínculos, remunerações mensais (12 colunas), 
  13º e desligamentos

### F4. Holerites em Lote
**Requisitos funcionais**:
- RF09: O sistema DEVE gerar holerites em lote assíncrono com barra de progresso
- RF10: O sistema DEVE permitir selecionar todos os funcionários ou lista específica

### F5. Agendamentos
**Requisitos funcionais**:
- RF11: O sistema DEVE permitir agendar relatórios com recorrência (cron expression)
- RF12: O sistema DEVE permitir configurar destinatários (lista de e-mails)
- RF13: O sistema DEVE manter histórico de execuções com status, link do arquivo e log de erros

### F6. Materialized Views
**Requisitos funcionais**:
- RF14: O sistema DEVE permitir refresh manual das views materializadas
- RF15: O sistema DEVE usar views materializadas para DIRF, RAIS e resumos (performance)

## Experiência do Usuário

**Fluxo principal**: Selecionar tipo de relatório → Escolher empresa/período → Configurar 
filtros → Escolher formato → Gerar/Download → (Opcional) Agendar recorrência.

**Acessibilidade**: Tabelas de relatórios com navegação por teclado, cabeçalhos semânticos, 
texto alternativo para gráficos.

## Restrições Técnicas de Alto Nível

- **Performance**: Materialized views PostgreSQL para DIRF, RAIS e resumos
- **Geração assíncrona**: Holerites em lote via fila (RabbitMQ)
- **Armazenamento**: Arquivos gerados no MinIO com links temporários
- **Formatos**: PDF, CSV, XML, JSON
- **Email**: Integração com serviço de e-mail para distribuição

## Fora de Escopo

- Criação de relatórios customizados pelo usuário (feature futura)
- Dashboards interativos com gráficos (feature futura — F08 Portal)
- Exportação direta para sistemas contábeis (integração via API)

---

## Anexo A — Referência de Endpoints

| Funcionalidade | Controller | Base Path |
|---------------|-----------|-----------|
| Folha Analítica/Sintética | `FolhaController` | `/api/relatorios` |
| Resumos | `ResumoController` | `/api/relatorios` |
| Relatórios Legais | `RelatoriosLegaisController` | `/api/relatorios` |
| Holerites (Relatórios) | `HoleritesController` | `/api/relatorios/holerites` |
| Agendamentos | `AgendamentosController` | `/api/relatorios/agendamentos` |
| Email | `EmailController` | `/api/relatorios` |
| Materialized Views | `MaterializedViewsController` | `/api/relatorios/materialized-views` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 13.  
Jornada: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 8.
