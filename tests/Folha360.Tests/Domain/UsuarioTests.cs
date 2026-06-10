using Folha360.Domain;
using Folha360.Domain.Entities;

namespace Folha360.Tests.Domain;

[Trait("Category", "Unit")]
public class UsuarioTests
{
    [Fact]
    public void Usuario_ShouldCreateWithValidProperties()
    {
        var usuario = new Usuario(
            "admin@folha360.com.br",
            "hash123",
            "Administrador",
            PerfilAcesso.Admin,
            UsuarioStatus.Ativo);

        Assert.Equal("admin@folha360.com.br", usuario.Email);
        Assert.Equal("hash123", usuario.SenhaHash);
        Assert.Equal("Administrador", usuario.Nome);
        Assert.Equal(PerfilAcesso.Admin, usuario.Perfil);
        Assert.Equal(UsuarioStatus.Ativo, usuario.Status);
        Assert.Null(usuario.UltimoLogin);
    }

    [Fact]
    public void Usuario_RegistrarLogin_ShouldUpdateUltimoLogin()
    {
        var usuario = new Usuario(
            "test@test.com",
            "hash",
            "Test",
            PerfilAcesso.Consulta,
            UsuarioStatus.Ativo);

        Assert.Null(usuario.UltimoLogin);

        usuario.RegistrarLogin();

        Assert.NotNull(usuario.UltimoLogin);
    }
}
