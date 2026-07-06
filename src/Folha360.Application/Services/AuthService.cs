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
    private readonly ITenantRepository _tenantRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ITenantRepository tenantRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _tenantRepository = tenantRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(command.Email, ct);

        if (usuario == null || !PasswordHelper.VerifyPassword(command.Password, usuario.SenhaHash))
        {
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        if (usuario.Status != UsuarioStatus.Ativo)
        {
            throw new UnauthorizedAccessException("Usuário inativo ou bloqueado");
        }

        var accessToken = GenerateJwtToken(usuario);
        var refreshToken = GenerateJwtToken(usuario); // Simplificado: usa JWT como refresh token por enquanto
        var expiresAt = DateTime.UtcNow.AddHours(8);
        var roles = new[] { usuario.Perfil.ToString() };

        var user = new UserDto(
            Id: usuario.Id.ToString(),
            Nome: usuario.Nome,
            Email: usuario.Email,
            Roles: roles);

        var tenants = await _tenantRepository.GetAllActiveAsync(ct);
        var tenantDtos = tenants
            .Select(t => new TenantDto(
                Id: t.TenantId,
                Nome: t.Nome,
                Slug: t.TenantId))
            .ToList();

        return new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: expiresAt,
            User: user,
            Tenants: tenantDtos);
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

    public async Task<SessionResponse> RefreshSessionAsync(Guid userId, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(userId, ct);

        if (usuario == null || usuario.Status != UsuarioStatus.Ativo)
        {
            throw new UnauthorizedAccessException("Usuário não encontrado ou inativo");
        }

        var roles = new[] { usuario.Perfil.ToString() };
        var user = new UserDto(
            Id: usuario.Id.ToString(),
            Nome: usuario.Nome,
            Email: usuario.Email,
            Roles: roles);

        var tenants = await _tenantRepository.GetAllActiveAsync(ct);
        var tenantDtos = tenants
            .Select(t => new TenantDto(
                Id: t.TenantId,
                Nome: t.Nome,
                Slug: t.TenantId))
            .ToList();

        return new SessionResponse(User: user, Tenants: tenantDtos);
    }
}
