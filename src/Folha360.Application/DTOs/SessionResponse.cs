namespace Folha360.Application.DTOs;

public sealed record SessionResponse(
    UserDto User,
    List<TenantDto> Tenants);
