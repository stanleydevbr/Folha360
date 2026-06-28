# PRD G12 — Módulo e-Social

## Visão Geral

O módulo e-Social gerencia a transmissão de eventos ao Sistema de Escrituração Digital das 
Obrigações Fiscais, Previdenciárias e Trabalhistas (e-Social). Organiza eventos em lotes, 
realiza assinatura digital via certificado A1/A3, envia ao governo e gerencia falhas com 
reprocessamento.

**Problema resolvido**: O contador precisa enviar eventos trabalhistas ao governo dentro dos 
prazos legais, com certificado digital válido, rastreando o status de cada evento e tratando 
falhas de forma ágil para evitar multas.

**Público-alvo**: Contador (envio e acompanhamento), Admin (certificados).

## Objetivos

- **Cobertura**: Suportar todos os eventos do e-Social relevantes (S-1000 a S-5002)
- **Confiabilidade**: Tratamento de falhas com reprocessamento e tentativas
- **Segurança**: Assinatura digital com certificado A1 (arquivo) e A3 (token)
- **Rastreabilidade**: Status detalhado de cada evento e lote

## Histórias de Usuário

1. **Como Admin**, quero fazer upload do certificado digital A1 (arquivo PFX) para assinar 
   eventos do e-Social.
2. **Como Admin**, quero testar conexão com certificado A3 (token) para validar antes do envio.
3. **Como Contador**, quero enviar um lote de eventos ao e-Social para transmitir as 
   obrigações do período.
4. **Como Contador**, quero acompanhar o status do lote (assinando, enviado, processado) 
   para saber quando o governo processou.
5. **Como Contador**, quero visualizar a lista de eventos enviados com status individual 
   (pendente, validado, assinado, enviado, processado, erro).
6. **Como Contador**, quero tratar falhas de envio (validação, assinatura, processamento) 
   e reprocessar eventos com erro.

## Funcionalidades Principais

### F1. Certificados Digitais
**Requisitos funcionais**:
- RF01: O sistema DEVE permitir upload de certificado A1 (arquivo PFX + senha)
- RF02: O sistema DEVE permitir testar certificado A3 (token + PIN)
- RF03: O sistema DEVE exibir status do certificado: emitente, CNPJ, data expiração, 
  dias restantes, ativo/expirado

### F2. Lotes de Eventos
**Requisitos funcionais**:
- RF04: O sistema DEVE permitir envio de lote com empresa e tipo de ambiente (Produção/Homologação)
- RF05: O sistema DEVE rastrear status do lote: Pendente, Assinando, Assinado, Enviado, 
  Processado, Erro, Parcialmente Processado
- RF06: O sistema DEVE armazenar protocolo de envio e recibo do governo

### F3. Eventos
**Requisitos funcionais**:
- RF07: O sistema DEVE suportar todos os eventos do e-Social (S-1000 a S-5002)
- RF08: O sistema DEVE rastrear status: Pendente, Validado, Assinado, Enviado, Processado, 
  Erro, Retificado
- RF09: O sistema DEVE armazenar XML de cada evento e hash de assinatura

### F4. Falhas e Reprocessamento
**Requisitos funcionais**:
- RF10: O sistema DEVE classificar erros: Validação, Assinatura, Envio, Processamento, Governo
- RF11: O sistema DEVE registrar código de erro, mensagem e XML original
- RF12: O sistema DEVE permitir reprocessamento de eventos com falha
- RF13: O sistema DEVE rastrear número de tentativas e data da última tentativa

## Experiência do Usuário

**Fluxo principal**: Verificar certificado (válido?) → Selecionar período → "Enviar Lote" → 
Acompanhar status → Verificar eventos → Tratar falhas → Reprocessar.

**Acessibilidade**: Status de eventos com indicadores visuais e textuais. Alertas de 
certificado próximo do vencimento.

## Restrições Técnicas de Alto Nível

- **Certificados**: Suporte a A1 (PKCS#12) e A3 (token USB/smartcard via PKCS#11)
- **Assinatura**: XML assinado conforme Manual de Orientação do e-Social (MOS)
- **Ambiente**: Produção e Produção Restrita (homologação)
- **Integração**: Eventos gerados pelos módulos G06 (Eventos), G07 (Folha), G11 (Fiscais)

## Fora de Escopo

- Geração do XML dos eventos (cada módulo de origem gera seu XML)
- Envio em tempo real (apenas lotes)
- Integração com DCTFWeb e EFD-Reinf (feature futura)

---

## Anexo A — Referência de Endpoints

| Funcionalidade | Controller | Base Path |
|---------------|-----------|-----------|
| Lotes | `LoteController` | `/api/esocial/lotes` |
| Falhas | `FalhaController` | `/api/esocial/falhas` |
| Certificados | `CertificadoController` | `/api/esocial/certificados` |

Ver detalhes em: `docs/outputs/agrupamento/agrupamento-cadastros-processos.md` — Seção 12.  
Jornada: `docs/outputs/agrupamento/workflow-jornada.md` — Jornada 5.
