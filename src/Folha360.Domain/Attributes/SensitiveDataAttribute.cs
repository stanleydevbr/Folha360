namespace Folha360.Domain.Attributes;

/// <summary>
/// Marca uma propriedade como dado sensível.
/// Propriedades com este atributo:
/// - São criptografadas em repouso (AES-256-GCM)
/// - NUNCA aparecem em logs (substituídas por [REDACTED])
/// - São mascaradas para perfis de consulta (***.xxx.xxx-**)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SensitiveDataAttribute : Attribute
{
}
