using Folha360.Domain;
using Folha360.Domain.Attributes;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade ContatoEmpresa — contatos da empresa (Diretor, Gerente, Contador, etc.).
/// Schema: tenant.
/// </summary>
public class ContatoEmpresa : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Tipo { get; private set; } = null!;
    public string Nome { get; private set; } = null!;

    [SensitiveData]
    public string? Cpf { get; private set; }

    public string? Cargo { get; private set; }
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public string? Celular { get; private set; }
    public bool ContatoPrincipal { get; private set; }
    public bool Ativo { get; private set; } = true;
    public DateOnly? DataInicioVigencia { get; private set; }
    public DateOnly? DataFimVigencia { get; private set; }

    private ContatoEmpresa()
        : base()
    {
    }

    public ContatoEmpresa(
        Guid empresaId,
        string tipo,
        string nome,
        string? cpf = null,
        string? cargo = null,
        string? email = null,
        string? telefone = null,
        string? celular = null,
        bool contatoPrincipal = false,
        DateOnly? dataInicioVigencia = null,
        DateOnly? dataFimVigencia = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Tipo = tipo;
        Nome = nome;
        Cpf = cpf;
        Cargo = cargo;
        Email = email;
        Telefone = telefone;
        Celular = celular;
        ContatoPrincipal = contatoPrincipal;
        DataInicioVigencia = dataInicioVigencia;
        DataFimVigencia = dataFimVigencia;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string nome,
        string? cpf = null,
        string? cargo = null,
        string? email = null,
        string? telefone = null,
        string? celular = null,
        bool? contatoPrincipal = null,
        bool? ativo = null,
        DateOnly? dataInicioVigencia = null,
        DateOnly? dataFimVigencia = null)
    {
        Nome = nome;
        Cpf = cpf;
        Cargo = cargo;
        Email = email;
        Telefone = telefone;
        Celular = celular;

        if (contatoPrincipal.HasValue)
        {
            ContatoPrincipal = contatoPrincipal.Value;
        }

        if (ativo.HasValue)
        {
            Ativo = ativo.Value;
        }

        DataInicioVigencia = dataInicioVigencia;
        DataFimVigencia = dataFimVigencia;
        UpdatedAt = DateTime.UtcNow;
    }
}
