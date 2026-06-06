# Issue: Ações Recomendadas — Code Review F03 (Não Bloqueantes)

**Origem**: Code Review do módulo F03 — Gestão de Eventos Trabalhistas  
**Data**: 06/06/2026  
**Severidade**: Baixa/Média (não bloqueante para merge)  
**Status**: Backlog

---

## Ações Recomendadas

### 1. Adicionar log de warning nos consumers de XML quando entidade não encontrada

**Severidade**: 🟢 Baixa  
**Arquivos**: `src/Folha360.Eventos.Application/Consumers/GerarXmlFeriasConsumer.cs`, `GerarXmlAfastamentoConsumer.cs`, `GerarXmlDesligamentoConsumer.cs`, `GerarXmlAlteracaoContratualConsumer.cs`

**Problema**: Apenas o `GerarXmlAdmissaoConsumer` loga um warning quando a entidade não é encontrada. Os outros 4 consumers retornam silenciosamente (`return;`), dificultando o diagnóstico em produção.

**Ação**: Padronizar o log de warning em todos os consumers de XML, seguindo o padrão do `GerarXmlAdmissaoConsumer`:

```csharp
if (ferias is null)
{
    _logger.LogWarning("Ferias {FeriasId} não encontrada para geração de XML", msg.FeriasId);
    return;
}
```

---

### 2. Adicionar validação de unicidade de admissão por funcionário

**Severidade**: 🟡 Média  
**Arquivos**: `src/Folha360.Eventos.Application/Consumers/FuncionarioCadastradoConsumer.cs`, `src/Folha360.Eventos.Application/Handlers/AdmissaoHandlers.cs`, `src/Folha360.Infrastructure/Data/Configurations/Eventos/AdmissaoConfiguration.cs`

**Problema**: Não há validação para evitar duplicidade de admissão para o mesmo funcionário. O `FuncionarioCadastradoConsumer` pode criar múltiplas admissões se o evento for reprocessado (ex.: retry após falha).

**Ação**:
- Adicionar verificação de existência de admissão ativa antes de criar uma nova no `FuncionarioCadastradoConsumer`
- OU adicionar índice único em `FuncionarioId` com filtro `deleted_at IS NULL` na `AdmissaoConfiguration`

---

### 3. Mover `Result<T>` para projeto compartilhado

**Severidade**: 🟢 Baixa  
**Arquivos**: `src/Folha360.Cadastros.Application/Result.cs`, `src/Folha360.Eventos.Application/Result.cs`, `src/Folha360.Eventos.Application/GlobalUsings.cs`

**Problema**: O `Result<T>` e `PaginatedResult<T>` estão definidos em `Folha360.Cadastros.Application`, forçando o módulo F03 a depender do F02. O arquivo `Result.cs` no F03 é um placeholder vazio que apenas reexporta via global usings.

**Ação**: Mover `Result<T>`, `Error` e `PaginatedResult<T>` para `Folha360.Application` (projeto já compartilhado entre módulos), eliminando a dependência F03 → F02 Application.

---

### 4. `FuncionarioCadastradoConsumer` deve buscar dados complementares do Funcionario

**Severidade**: 🟡 Média  
**Arquivo**: `src/Folha360.Eventos.Application/Consumers/FuncionarioCadastradoConsumer.cs`

**Problema**: A Tech Spec menciona: *"Busca dados complementares do funcionário (cargo, salário) via IFuncionarioRepository da F02"*. O consumer atual cria a `Admissao` diretamente com os dados do evento, sem buscar o `Funcionario` completo.

**Ação**: Adicionar injeção de `IFuncionarioRepository` e buscar o funcionário completo antes de criar a admissão, ou atualizar a Tech Spec para refletir a implementação atual.

---

### 5. Reduzir duplicação de código nos Handlers

**Severidade**: 🟢 Baixa  
**Arquivos**: `src/Folha360.Eventos.Application/Handlers/*.cs`

**Problema**: Os 5 handlers de criação seguem exatamente o mesmo padrão (criar entidade → salvar → publicar eventos → publicar comando XML → retornar DTO). O mesmo para Update, Delete, Get e List.

**Ação**: Considerar um handler base genérico ou um pipeline behavior do MediatR para reduzir duplicação. Exemplo:

```csharp
public abstract class CriarEventoHandler<TCommand, TEntity, TDto> 
    : IRequestHandler<TCommand, Result<TDto>>
    where TCommand : IRequest<Result<TDto>>
{
    // Lógica comum de criação
}
```

---

## Checklist

- [ ] 1. Padronizar logs de warning nos consumers de XML
- [ ] 2. Adicionar validação de unicidade de admissão
- [ ] 3. Mover Result<T> para Folha360.Application
- [ ] 4. Buscar dados complementares no FuncionarioCadastradoConsumer
- [ ] 5. Avaliar handler base genérico para reduzir duplicação
