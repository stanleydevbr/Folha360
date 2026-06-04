namespace Folha360.Application.DTOs;

public sealed record LoginResponse(string Token, DateTime ExpiresAt, string Perfil, string Nome);
