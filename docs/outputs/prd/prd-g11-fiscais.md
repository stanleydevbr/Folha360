# PRD G11 — Módulo Obrigações Fiscais

## Visão Geral

O módulo Obrigações Fiscais realiza a apuração de tributos (IRRF, INSS, FGTS, Contribuição 
Sindical, PIS, COFINS, CSLL, ISS) a partir da folha processada, gera guias de recolhimento 
(GPS, DARF, GRF) e produz lançamentos contábeis exportáveis (CSV, SPED).

**Problema resolvido**: O contador precisa apurar tributos com precisão, gerar guias nos 
formatos oficiais e manter rastreabilidade contábil de cada lançamento, reduzindo risco de 
multas e autuações fiscais.

**Público-alvo**: Contador.

## Objetivos

- **Automação**: Apuração automática a partir da folha processada
- **Conformidade**: Guias nos padrões oficiais (GPS, DARF, GRF)
- **Rastreabilidade**: Lançamentos contábeis com débito/crédito e exportação SPED
- **Versionamento**: Regras fiscais versionadas por vigência

## Histórias de Usuário

1. **Como Contador**, quero visualizar o resumo da apuração fiscal do período para conferir 
   os valores antes de gerar as guias.
2. **Como Contador**, quero gerar guias de recolhimento (GPS para INSS, DARF para IRRF, 
   GRF para FGTS) para pagamento.
3. **Como Contador**, quero baixar o PDF das guias para envio ao financeiro.
4. **Como Contador**, quero registrar o pagamento das guias com data e valor para controle.
5. **Como Contador**, quero consultar o calendário de obrigações do ano para não perder prazos.
6. **Como Contador**, quero gerenciar regras fiscais (alíquotas, códigos de receita) 
   versionadas por vigência.
7. **Como Contador**, quero exportar lançamentos contábeis em CSV e SPED para integração 
   com sistemas contábeis.

## Funcionalidades Principais

### F1. Apuração Fiscal
**Requisitos funcionais**:
- RF01: O sistema DEVE apurar tributos automaticamente a partir da folha processada
- RF02: O sistema DEVE suportar tributos: IRRF, INSS, FGTS, Contribuição Sindical, PIS, 
  COFINS, CSLL, ISS
- RF03: O sistema DEVE exibir status: Pendente, Em Processamento, Concluído, Falho, Revertido
- RF04: O sistema DEVE expor resumo com totais por tributo e guias pendentes

### F2. Guias de Recolhimento
**Requisitos funcionais**:
- RF05: O sistema DEVE gerar guias nos formatos GPS (INSS), DARF (IRRF) e GRF (FGTS)
- RF06: O sistema DEVE armazenar PDFs das guias no MinIO
- RF07: O sistema DEVE permitir download da guia em PDF
- RF08: O sistema DEVE permitir registrar pagamento (valor pago, data)
- RF09: O sistema DEVE rastrear status: Pendente, Gerada, Paga, Vencida, Cancelada

### F3. Regras Fiscais
**Requisitos funcionais**:
- RF10: CRUD de regras fiscais com tributo, versão, vigência e código da receita
- RF11: O sistema DEVE versionar regras (não sobrescrever)

### F4. Lançamentos Contábeis
**Requisitos funcionais**:
- RF12: O sistema DEVE gerar lançamentos contábeis (débito/crédito) por tributo
- RF13: O sistema DEVE exportar em CSV, SPED ECD e SPED ECF

## Experiência do Usuário

**Fluxo principal**: Dashboard fiscal → Resumo da apuração → Verificar guias → Baixar PDFs → 
Registrar pagamentos → Exportar lançamentos contábeis.

**Acessibilidade**: Tabelas de guias com indicadores visuais de status (pendente/vencida/paga) 
e contraste adequado. Calendário de obrigações navegável por teclado.

## Restrições Técnicas de Alto Nível

- **Integração**: Apuração depende do processamento da folha (G07)
- **Regras**: Versionamento de regras fiscais por vigência (não sobrescrever)
- **Exportação**: Formatos CSV, SPED ECD e SPED ECF
- **Armazenamento**: PDFs de guias no MinIO

## Fora de Escopo

- Processamento da folha (módulo Folha — G07)
- Envio das guias para bancos (feature futura — integração com gateways de pagamento)
- Apuração de PIS/COFINS/CSLL/ISS sobre faturamento (fora do escopo da folha)

---

## Anexo A — Referência de Endpoints

| Funcionalidade | Controller | Base Path |
|---------------|-----------|-----------|
| Regras Fiscais | `RegraFiscalController` | `/api/fiscais/regras` |
| Guias | `GuiaController` | `/api/fiscais/guias` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 11.  
Jornada: `docs/outputs/agrupamento/workflow-jornada.md` — Jornadas 5 e 8.
