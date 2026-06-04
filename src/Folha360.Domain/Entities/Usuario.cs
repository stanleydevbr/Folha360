namespace Folha360.Domain.Entities;

public sealed class Usuario : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public PerfilAcesso Perfil { get; private set; }
    public UsuarioStatus Status { get; private set; }
    public DateTime? UltimoLogin { get; private set; }

    private Usuario()
    {
    }

    public Usuario(string email, string senhaHash, string nome, PerfilAcesso perfil, UsuarioStatus status)
    {
        Email = email;
        SenhaHash = senhaHash;
        Nome = nome;
        Perfil = perfil;
        Status = status;
    }

    public void RegistrarLogin()
    {
        UltimoLogin = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
