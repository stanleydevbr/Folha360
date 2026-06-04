# 🧩 Boas Práticas — Design Patterns

> **Escopo**: Padrões de projeto aplicados ao domínio Folha360  
> **Referências**: [Fronteiras de Componentes](./component-boundaries), [ADR-002](./adr-002-rabbitmq-message-broker)  
> **Fonte completa**: [`docs/instrucoes/03-design-patterns.md`](../instrucoes/03-design-patterns.md)

---

## Padrões por Contexto

| Padrão | Quando Usar | Contexto Folha360 |
|---|---|---|
| **Repository** | Abstrair acesso a dados | Interfaces no Domain, EF Core no Infrastructure |
| **Strategy** | Algoritmos intercambiáveis | Versionamento IRRF/INSS (muda anualmente) |
| **Factory** | Criação de objetos complexos | Eventos e-Social (S-1200, S-2200, etc.) |
| **Observer** | Desacoplamento de eventos | `FolhaFechada` → Fiscais → e-Social via RabbitMQ |
| **Saga** | Transações distribuídas | Fluxo Folha → Fiscais → e-Social com compensação |
| **Decorator** | Compor comportamentos | MediatR Pipeline Behaviors (logging, validation) |
| **Adapter** | Integrar sistemas externos | `EsocialGovAdapter` para chamadas ao gov.br |
| **CQRS** | Separar leitura e escrita | Commands (processar folha) vs Queries (holerite) |
| **Circuit Breaker** | Resiliência | Polly para chamadas e-Social gov.br |
| **Idempotency** | Operações repetidas | UNIQUE (empresa_id, periodo) em processamento_folha |

## Fluxo Saga: Folha → Fiscais → e-Social

```
Folha Fecha ──► FolhaFechada ──► Fiscais Apura
                                        │
                               ObrigacoesApuradas
                                        │
                                        ▼
                                  e-Social Envia Lote
                                   │            │
                            Processado        Com Erro
                                │                │
                          Publica Sucesso   Retry + backoff
```

---

**Fonte completa com exemplos de código**: [`docs/instrucoes/03-design-patterns.md`](../instrucoes/03-design-patterns.md)
