# ADR-002: RabbitMQ como Message Broker

## Status
**Accepted** (Junho 2026)

## Context
O sistema Folha360 depende de comunicação assíncrona para: (a) eventos de domínio entre módulos (ex.: `FolhaFechada` → Obrigações Fiscais), (b) fila de eventos a enviar ao e-Social, (c) retry e dead-letter para falhas de envio. Precisamos escolher um message broker.

## Decision Drivers
1. **Simplicidade operacional**: Time pequeno; evitar complexidade desnecessária
2. **Padrões de mensageria**: Necessidade de filas de trabalho, retry com backoff, dead-letter queue, roteamento flexível
3. **Volume de mensagens**: Estimado em ~200K eventos/mês (~0.08 msg/s médio; pico de ~50 msg/s no fechamento)
4. **Persistência**: Mensagens não podem ser perdidas (eventos de domínio e envios ao e-Social)
5. **Custo**: Orçamento inicial limitado

## Options Considered

| Opção | Prós | Contras |
|---|---|---|
| **RabbitMQ (escolhido)** | AMQP padrão; roteamento flexível; dead-letter nativo; simples de operar; boa documentação | Throughput limitado (~50K msg/s); mensagens removidas após consumo |
| **Apache Kafka** | Altíssimo throughput; log imutável; replay de eventos; retenção longa | Complexidade operacional alta; ZooKeeper/KRaft; roteamento limitado; curva de aprendizado |
| **Azure Service Bus / AWS SQS** | Gerenciado; sem operação; alta disponibilidade | Vendor lock-in; custo variável; exige cloud |
| **Sem mensageria (chamadas HTTP síncronas)** | Simples; sem infra extra | Sem resiliência; acoplamento forte; sem retry; inviável para e-Social |

## Decision
**RabbitMQ**: Implantado como cluster de 3 nós no Kubernetes, com mensagens persistentes, publisher confirm, e dead-letter queues configuradas. Filas e exchanges são provisionadas via código (TopologyBuilder no início da aplicação).

## Consequences

### Positive
- Roteamento flexível com exchanges (direct, topic, fanout) adequado para eventos de domínio
- Dead-letter queue nativa para tratamento de falhas (essencial para retry de envio e-Social)
- Cluster 3 nós oferece alta disponibilidade
- Curva de aprendizado baixa (AMQP é padrão conhecido)
- Operação mais simples que Kafka

### Negative
- Mensagens são removidas após consumo (sem replay nativo)
- Throughput limitado comparado ao Kafka (não é problema para o volume atual)
- Precisa de +1 infra para gerenciar (cluster RabbitMQ)

### Mitigações
- Mensagens importantes são persistidas e possuem publisher confirm
- Reconciliação batch diária como safety net para eventos perdidos
- Monitoramento de filas (tamanho, dead-letter) com Prometheus + Grafana
- Se volume crescer 10x, migrar para Kafka (a abstração de message bus facilita)

## Follow-up Actions
- [ ] Provisionar cluster RabbitMQ 3 nós no Kubernetes
- [ ] Implementar TopologyBuilder para criar filas/exchanges via código
- [ ] Configurar dead-letter queues para cada tipo de evento
- [ ] Implementar reconciliação batch diária
- [ ] Configurar alertas para filas com mensagens acumuladas > 1000
