namespace Folha360.Domain;

public enum PerfilAcesso
{
    Admin = 0,
    Operador = 1,
    Contador = 2,
    Consulta = 3,
}

public enum TenantStatus
{
    Ativo = 0,
    Inativo = 1,
    Excluido = 2,
}

public enum UsuarioStatus
{
    Ativo = 0,
    Inativo = 1,
    Bloqueado = 2,
}
