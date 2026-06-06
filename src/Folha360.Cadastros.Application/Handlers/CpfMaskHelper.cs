namespace Folha360.Cadastros.Application.Handlers;

internal static class CpfMaskHelper
{
    public static string Mask(string cpf)
        => cpf.Length == 11 ? $"***.{cpf[3..6]}.{cpf[6..9]}-**" : "***";
}
