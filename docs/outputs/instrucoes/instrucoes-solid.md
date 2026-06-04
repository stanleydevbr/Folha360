# 📐 Boas Práticas — Princípios SOLID

> **Escopo**: SOLID aplicado ao domínio Folha360  
> **Referências**: [Visão em Camadas](./layered-architecture), [Fronteiras de Componentes](./component-boundaries)  
> **Fonte completa**: [`docs/instrucoes/04-solid.md`](../instrucoes/04-solid.md)

---

## S — Single Responsibility

> Uma classe deve ter **um, e apenas um, motivo para mudar**.

- `FolhaService` calcula folha — **nunca** apura impostos ou envia e-Social
- `ObrigacoesFiscaisService` apura impostos — **nunca** calcula folha
- Cada handler MediatR = um caso de uso

## O — Open/Closed

> Aberto para extensão, fechado para modificação.

- **Strategy pattern** para cálculos fiscais: `TabelaIrrf2026`, `TabelaIrrf2027` — nova classe, sem modificar existente
- **Factory pattern** para eventos e-Social: novos tipos = novo factory method

## L — Liskov Substitution

> Subtipos devem ser substituíveis por seus tipos base.

- `IFuncionarioRepository` → `FuncionarioRepository` (PostgreSQL) ou `InMemoryFuncionarioRepository` (testes)
- Ambos funcionam igualmente — nenhum lança `NotSupportedException`

## I — Interface Segregation

> Clientes não devem depender de interfaces que não usam.

- `IFuncionarioReadOnlyRepository` (leitura) separado de `IFuncionarioWriteRepository` (escrita)
- Relatórios recebe apenas interfaces de leitura/query

## D — Dependency Inversion

> Módulos de alto nível não devem depender de módulos de baixo nível.

```
Presentation ──► Application ──► Domain ◄── Infrastructure
                                      ▲
                              (via interfaces/DIP)
```

- Domain define `IMessageBus`, `ICacheService`, `ICryptoService`
- Infrastructure implementa com RabbitMQ, Redis, AES-256
- **Domain tem ZERO referências a pacotes NuGet externos**

---

**Fonte completa com exemplos de código**: [`docs/instrucoes/04-solid.md`](../instrucoes/04-solid.md)
