using Folha360.Eventos.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Eventos.Infrastructure.Data;

/// <summary>
/// DbContext de runtime do módulo de Eventos Trabalhistas (F03).
/// Herda de Folha360DbContext (modelo canônico) e expõe DbSets tipados para queries.
/// NÃO contém configurações de modelo — o modelo canônico está em Folha360DbContext.
/// NÃO contém migrations — as migrations são gerenciadas pelo Folha360.Infrastructure.
/// </summary>
public class EventosDbContext : Folha360DbContext
{
    public EventosDbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }

    public new DbSet<Admissao> Admissoes => Set<Admissao>();
    public new DbSet<Ferias> Ferias => Set<Ferias>();
    public new DbSet<Afastamento> Afastamentos => Set<Afastamento>();
    public new DbSet<Desligamento> Desligamentos => Set<Desligamento>();
    public new DbSet<AlteracaoContratual> AlteracoesContratuais => Set<AlteracaoContratual>();
}
