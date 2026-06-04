# ADR-004: Processamento Assíncrono da Folha de Pagamento

## Status
**Accepted** (Junho 2026)

## Context
O cálculo da folha de pagamento para 100.000 funcionários pode levar até 2 horas. A API não pode manter uma conexão HTTP aberta por todo esse período. Precisamos decidir entre processamento síncrono (API espera o resultado) ou assíncrono (API retorna imediatamente e processa em background).

## Decision Drivers
1. **Experiência do usuário**: Operador de RH não pode esperar 2h com a tela carregando
2. **Resiliência**: Se o processamento falhar, deve ser possível retomar sem perder o trabalho
3. **Escalabilidade**: Múltiplos processamentos podem ocorrer em paralelo (empresas diferentes)
4. **Observabilidade**: Operador precisa acompanhar o progresso do processamento
5. **Timeouts**: HTTP timeouts típicos (30-120s) são incompatíveis com 2h de processamento

## Options Considered

| Opção | Prós | Contras |
|---|---|---|
| **Assíncrono com polling/SignalR (escolhido)** | Resposta imediata (202); progresso em tempo real; retry automático; escala independente | Complexidade adicional; precisa de notificação de conclusão |
| **Síncrono com timeout longo** | Implementação simples; resultado imediato | Timeout inevitável; UX péssima; sem retry; conexão ocupada |
| **Batch job agendado (cron)** | Desacoplado; horário previsível | Sem feedback em tempo real; difícil de reprocessar sob demanda |

## Decision
**Processamento Assíncrono**: A API `POST /api/folha/processar` retorna imediatamente `202 Accepted` com um `processamentoId`. O processamento é executado em background (background job via Hangfire ou Channel<T>). O frontend consulta o progresso via `GET /api/folha/processamento/{id}/status` (polling a cada 5s) ou recebe atualizações em tempo real via SignalR. Ao concluir, um evento `FolhaFechada` é publicado no RabbitMQ.

## Consequences

### Positive
- API responde imediatamente (boa UX)
- Processamento em background permite paralelismo (PLINQ/Task Parallel)
- Retry automático em caso de falha (Hangfire retry policy)
- Progresso visível em tempo real (SignalR)
- Múltiplas empresas podem processar simultaneamente
- Timeout não é problema

### Negative
- Complexidade adicional: fila de jobs, notificação, idempotência
- Usuário não recebe resultado imediato (precisa aguardar notificação)
- Se SignalR falhar, usuário depende de polling
- Precisa garantir idempotência (não processar mesma folha duas vezes)

### Mitigações
- Idempotência: verificar se período já foi processado antes de iniciar
- Polling como fallback se SignalR indisponível
- Timeout máximo do job configurado para 3h (alerta se exceder)
- Log detalhado de cada etapa para debugging

## Follow-up Actions
- [ ] Implementar endpoint `POST /api/folha/processar` retornando 202
- [ ] Implementar endpoint `GET /api/folha/processamento/{id}/status`
- [ ] Configurar SignalR Hub para progresso em tempo real
- [ ] Implementar idempotência (verificação de período já processado)
- [ ] Configurar alerta se processamento exceder 2h
