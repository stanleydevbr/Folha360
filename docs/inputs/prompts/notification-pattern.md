---
tags: [refatoracao, validacao, notification-pattern, backend]
status: rascunho
---

# Refatoração: Notification Pattern + Validação por Pipeline

## Task

Implementar o **Notification Pattern** no backend Folha360, substituindo validações baseadas em `ArgumentException` por um modelo declarativo de acumulação de erros, e automatizar a validação dos commands via **MediatR Pipeline Behavior** com FluentValidation.

---

## Goals

Eliminar `try/catch (ArgumentException)` de todos os handlers e centralizar validação de commands em um pipeline do MediatR, devolvendo ao cliente uma lista estruturada de erros de validação que o usuário possa corrigir e reenviar.

---

## Role

Você é um engenheiro de software sênior especializado em Clean Architecture + DDD + CQRS com .NET. Você conhece profundamente o projeto Folha360 — um monólito modular .NET 10 que segue os princípios de Domain-Driven Design, com módulos independentes (Cadastros, Eventos, Processamento, Fiscais, Relatorios, Esocial), cada um com suas camadas Domain, Application, Infrastructure e Presentation.

Você entende que:

- O projeto **já usa FluentValidation v12.1.1** com validators por módulo (ex: `CriarEmpresaCommandValidator`)
- O projeto **já tem** `Result<T>` e `PaginatedResult<T>` como tipos de retorno padronizados em `Folha360.Application`
- O projeto **já registra** MediatR v12.5.0 com Commands/Queries → Handlers
- **Não existe** nenhum `IPipelineBehavior` de validação atualmente — a validação FluentValidation é manual em alguns controllers e ignorada na maioria
- Os **Value Objects do domínio** (Cpf, Cnpj, Cbo, Endereco, etc.) lançam `ArgumentException` no construtor
- **Handlers capturam** `ArgumentException` e convertem em `Result<T>.Failure("VALIDACAO", ex.Message)`

---

## Workflow

Implemente esta refatoração em **6 etapas sequenciais**:

### Etapa 1: Notification Pattern — Domain Layer

Criar na camada de domínio compartilhado (`Folha360.Domain`) um mecanismo de notificação que **não dependa de exceções** para reportar violações de invariante.

1. Criar `Folha360.Domain/Abstractions/Notification.cs`:
   - Classe `Notification` com lista de `NotificationError`
   - Propriedade `bool HasErrors` (true se `Errors.Count > 0`)
   - Método `void AddError(string code, string message)`
   - Método `void AddError(NotificationError error)`
   - Método `void AddErrors(IEnumerable<NotificationError> errors)`
   - Método `void ThrowIfHasErrors()` — opcional, para compatibilidade com código legado

2. Criar `NotificationError` como `record` em `Folha360.Domain/Abstractions/NotificationError.cs`:
   ```csharp
   public sealed record NotificationError(string Code, string Message);
   ```

### Etapa 2: Notification Pattern — Value Objects

Modificar **todos os Value Objects** dos módulos para aceitar um `Notification` opcional em vez de lançar `ArgumentException`.

**Estratégia**: Adicionar um construtor/ factory method alternativo que recebe `Notification? notifications`, acumula erros em vez de lançar exceção, e retorna bool indicando sucesso. Manter o construtor original (com `throw`) para não quebrar código existente durante a transição.

Exemplo de padrão para Cpf, Cnpj, Cbo, Endereco, ProtocoloNumero, ReciboNumero:

```csharp
public static bool TryCreate(string numero, [NotNullWhen(false)] out Cpf? result, [NotNullWhen(true)] out NotificationError? error)
{
    // validação sem exceção
}
```
**OU** (caso prefira):

```csharp
public Cpf(string numero, Notification? notifications)
{
    // acumula erros em notifications, não lança exceção
    // se notifications for null, usa throw (compatibilidade)
}
```

**Value Objects a modificar** (por módulo):
- `Folha360.Cadastros.Domain/ValueObjects/`: `Cpf.cs`, `Cnpj.cs`, `Cbo.cs`, `Endereco.cs`
- `Folha360.Esocial.Domain/ValueObjects/`: `ProtocoloNumero.cs`, `ReciboNumero.cs`
- Outros Value Objects em módulos que tenham validação com `ArgumentException`

### Etapa 3: Validation Pipeline Behavior (MediatR)

Criar um `IPipelineBehavior<TRequest, TResponse>` que executa automaticamente o FluentValidation **antes** do handler ser invocado.

1. Criar `Folha360.Application/Pipeline/ValidationBehaviour.cs`:

```csharp
using FluentValidation;
using MediatR;

namespace Folha360.Application.Pipeline;

public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
            return await next(cancellationToken);

        // Mapeia ValidationFailure para Error (Formatação)
        var errors = failures
            .Select(f => new Error(
                f.PropertyName.ToSnakeCase().ToUpper(),
                f.ErrorMessage))
            .ToList();

        // Cria TResponse como Failure — precisa de reflexão ou factory
        return CreateFailureResponse(errors);
    }

    private static TResponse CreateFailureResponse(List<Error> errors)
    {
        // Usa ativação via reflexão para criar Result<T>.Failure(errors)
        var responseType = typeof(TResponse);

        // Se for Result<T>
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", new[] { typeof(List<Error>) });

            return (TResponse)failureMethod!.Invoke(null, new object[] { errors })!;
        }

        // Se for PaginatedResult<T>
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(PaginatedResult<>))
        {
            var resultType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(PaginatedResult<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", new[] { typeof(string), typeof(string) });

            // PaginatedResult só tem Failure(string code, string message)
            // Pega o primeiro erro apenas
            var first = errors.First();
            return (TResponse)failureMethod!.Invoke(null, new object[] { first.Code, first.Message })!;
        }

        throw new InvalidOperationException($"Tipo de resposta não suportado: {responseType.Name}");
    }
}
```

> **IMPORTANTE**: A criação da resposta de falha por reflexão é um ponto crítico. Se a implementação por reflexão ficar muito complexa, considere uma das alternativas:
> - Usar uma interface `IValidationResult` implementada por `Result<T>` e `PaginatedResult<T>`
> - Usar uma base comum `ResultBase` com método `static abstract` (C# 11 static virtual members — disponível no .NET 10)

2. Registrar no `Folha360.IoC/ServiceCollectionExtensions.cs`:

```csharp
// Dentro de AddFolha360Infrastructure:
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
```

### Etapa 4: Remover try/catch ArgumentException dos Handlers

Em **todos os handlers** que capturam `ArgumentException`, remover o bloco `try/catch` e substituir por validação explícita no handler ou confiar no pipeline behavior.

**Handlers afetados (mapeados atualmente)**:
- `Folha360.Cadastros.Application/Handlers/EmpresaHandlers.cs` — `CriarEmpresaHandler`
- `Folha360.Cadastros.Application/Handlers/FuncionarioHandlers.cs` — `CriarFuncionarioHandler`
- `Folha360.Cadastros.Application/Handlers/CargoHandlers.cs` — 2 ocorrências

**O que fazer em cada handler**:
1. Remover `try { ... } catch (ArgumentException ex) { return Result<T>.Failure("VALIDACAO", ex.Message); }`
2. Se o handler usa Value Objects que ainda podem lançar `ArgumentException`, substituir pelo padrão `TryCreate`:
   ```csharp
   // ANTES:
   var cnpj = new Cnpj(cmd.Cnpj);

   // DEPOIS:
   if (!Cnpj.TryCreate(cmd.Cnpj, out var cnpj, out var error))
       return Result<EmpresaDto>.Failure(error.Code, error.Message);
   ```

3. Verificar se o command já tem validação FluentValidation. Se não tiver, **CRIAR** o validator (ou pelo menos garantir que a validação de formato/campos obrigatórios está coberta).

### Etapa 5: Remover validações duplicadas dos Handlers

Alguns handlers têm validações inline manuais (ex: `if (cmd.EmpresaId == Guid.Empty) return Result<...>.Failure(...)`) que **já estão** ou **deveriam estar** no FluentValidation.

1. Mover validações inline para os respectivos validators
2. Remover as validações inline dos handlers
3. Exceção: validações que dependem de consulta ao banco (ex: "já existe empresa com este CNPJ") — essas **permanecem** no handler, pois o pipeline behavior não acessa o banco

### Etapa 6: Verificar Controllers

Os controllers que atualmente chamam `_validator.ValidateAsync()` manualmente (ex: `AuthController`) devem ser revisados:

1. Remover chamadas manuais de `ValidateAsync` dos controllers
2. Remover injeção de validators nos controllers
3. A validação agora é automática via pipeline behavior
4. O controller só precisa verificar `result.IsSuccess` e retornar a resposta adequada

---

## Output

O resultado final deve ser:

1. **Código**:
   - Notification + NotificationError em `Folha360.Domain/Abstractions/`
   - ValidationBehaviour em `Folha360.Application/Pipeline/`
   - Value Objects modificados com `TryCreate` em cada módulo Domain
   - Handlers sem `try/catch ArgumentException`
   - Validators revisados/completados em cada módulo Application
   - DI registrado no `ServiceCollectionExtensions.cs`
   - Controllers simplificados (sem validação manual)

2. **Migrations**: Nenhuma — esta é uma refatoração puramente de código, sem alteração de schema.

3. **Build + Testes**: Ao final de cada etapa, executar `dotnet build` para garantir que não há erros de compilação.

---

## Endpoints

Endpoint de exemplo que será afetado:

```
POST /api/empresas
Body: { "cnpj": "invalid", "razaoSocial": "" }

Resposta ANTES:
422 UnprocessableEntity
{ "errors": [{ "code": "VALIDACAO", "message": "CNPJ deve ter 14 dígitos." }] }
(apenas 1 erro por vez — o primeiro ArgumentException)

Resposta DEPOIS:
422 UnprocessableEntity
{
  "errors": [
    { "code": "CNPJ", "message": "CNPJ deve ter 14 dígitos." },
    { "code": "RAZAO_SOCIAL", "message": "Razão Social é obrigatória." }
  ]
}
(múltiplos erros acumulados — usuário corrige tudo de uma vez)
```

---

## Tests

Validar cenários em **cada módulo**:

1. **Command com dados inválidos** → `IsSuccess == false` com lista de erros
2. **Command com dados válidos** → `IsSuccess == true`, processamento normal
3. **Value Object inválido** → erro acumulado no Notification, sem exceção
4. **Múltiplos erros simultâneos** → todos listados, não apenas o primeiro
5. **Handlers sem try/catch** → ArgumentException não é mais capturado (deve propagar como 500 se ocorrer)

---

## Critical

### Skills Obrigatórias
- **dev-best-practices** — seguir as boas práticas do projeto (nullable, TreatWarningsAsErrors, StyleCop)
- **efcore-migrations** — NÃO gerar migrations (refatoração code-only)
- **docker-infra** — NÃO alterar Dockerfile, docker-compose ou infraestrutura

### Regras do Projeto (NÃO VIOLAR)
1. **NUNCA** remover ou modificar construtores originais de Value Objects — additivade apenas (novos métodos `TryCreate` ou overloads com `Notification?`)
2. **NUNCA** alterar a assinatura de `Result<T>.Success` / `Result<T>.Failure`
3. **NUNCA** gerar migrations ou alterar schema do banco
4. **NUNCA** remover `UseSnakeCaseNamingConvention()` dos DbContexts
5. **NUNCA** remover `TreatWarningsAsErrors` ou `Nullable` do `Directory.Build.props`
6. **NUNCA** tocar nos projetos `Folha360.Infrastructure` ou `Folha360.WebApi` a não ser para registrar DI de novos serviços

### Sequência de Execução
1. Notification pattern → 2. Value Objects → 3. Pipeline Behavior → 4. Handlers → 5. Controllers
Cada etapa DEVE compilar antes de passar para a próxima. Executar `dotnet build` após cada etapa.

### Fora do Escopo
- ❌ **NÃO** alterar a camada de Infrastructure (DbContext, repositórios, multi-tenancy)
- ❌ **NÃO** alterar a camada de Presentation (controllers) exceto para remover validação manual
- ❌ **NÃO** adicionar novos pacotes NuGet
- ❌ **NÃO** gerar migrations
- ❌ **NÃO** alterar docker-compose, Dockerfile ou scripts de infraestrutura
- ❌ **NÃO** refatorar domain entities — apenas Value Objects