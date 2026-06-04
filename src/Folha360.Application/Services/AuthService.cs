using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Folha360.Application.Commands;
using Folha360.Application.DTOs;
using Folha360.Domain;
using Folha360.Domain.Abstractions;
using Folha360.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Folha360.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(command.Email, ct);

        if (usuario == null || !VerifyPassword(command.Password, usuario.SenhaHash))
        {
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        if (usuario.Status != UsuarioStatus.Ativo)
        {
            throw new UnauthorizedAccessException("Usuário inativo ou bloqueado");
        }

        var token = GenerateJwtToken(usuario);
        var perfilName = usuario.Perfil.ToString();

        return new LoginResponse(token, DateTime.UtcNow.AddHours(8), perfilName, usuario.Nome);
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var secret = _configuration["Jwt:Secret"] ?? "Folha360@DevSecretKey@2026!TempKey";
        var issuer = _configuration["Jwt:Issuer"] ?? "Folha360";
        var audience = _configuration["Jwt:Audience"] ?? "Folha360.Api";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
            new Claim("perfil", usuario.Perfil.ToString()),
            new Claim("nome", usuario.Nome),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHash = Convert.ToBase64String(hashBytes);
        return computedHash == hash;
    }
}
