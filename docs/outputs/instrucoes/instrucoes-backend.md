# 📋 Boas Práticas — Backend .NET 10

> **Escopo**: ASP.NET Core, EF Core, MediatR, C#  
> **Referências**: [ADR-001](./adr-001-monolito-modular), [Visão em Camadas](./layered-architecture), [Fronteiras de Componentes](./component-boundaries)  
> **Fonte completa**: [`docs/instrucoes/01-backend-dotnet.md`](../instrucoes/01-backend-dotnet.md)

---

## Estrutura da Solução

```
Folha360/
├── src/
│   ├── Cadastros/
│   │   ├── Cadastros.Domain/          # Entidades, interfaces, value objects
│   │   ├── Cadastros.Application/     # Services, DTOs, Commands, Queries
│   │   ├── Cadastros.Infrastructure/  # Repositories, EF Core, Cache
│   │   └── Cadastros.Presentation/    # Controllers, Middleware
│   ├── Eventos/  Folha/  Fiscais/  Relatorios/  IntegracaoESocial/
├── tests/
└── shared/Folha360.Shared/
```

- Cada módulo = 4 projetos (Domain, Application, Infrastructure, Presentation)
- **Domain NÃO referencia** pacotes externos (zero dependências)

## Controller Magro

```csharp
[HttpPost("processar")]
public async Task<IActionResult> ProcessarFolha(
    [FromBody] ProcessarFolhaCommand command, CancellationToken ct)
{
    var result = await _mediator.Send(command, ct);
    return result.IsSuccess 
        ? AcceptedAtAction(nameof(StatusProcessamento), new { id = result.Value }, result.Value)
        : BadRequest(result.ToProblemDetails());
}
```

## Repository Pattern

- Interfaces no **Domain**, implementação no **Infrastructure**
- **NUNCA** retornar `IQueryable` (leaky abstraction)
- Eager loading com `.Include()`, nunca lazy loading

## MediatR / CQRS

- **Commands** (escrita) passam por Domain com validação
- **Queries** (leitura) podem usar Dapper para performance
- Pipeline Behaviors: Validation → Logging → Caching

## Tratamento de Erros

- `Result<T>` para erros de negócio (não exceptions)
- Problem Details RFC 7807 para API responses
- Middleware global para exceptions não tratadas

## Anti-Patterns

| ❌ Não Fazer | Consequência |
|---|---|
| Lógica de negócio no Controller | Regra não testável, duplicação |
| Repository leaky (retorna IQueryable) | Domain acoplado ao ORM |
| Exception para fluxo de negócio | Stack trace poluído |
| `.Result` ou `.Wait()` em async | Deadlock, thread pool starvation |
| Log de dados sensíveis (CPF, salário) | Violação LGPD |

---

**Fonte completa com exemplos de código**: [`docs/instrucoes/01-backend-dotnet.md`](../instrucoes/01-backend-dotnet.md)
