namespace Folha360.Application.DTOs;

public sealed record UserDto(
    string Id,
    string Nome,
    string Email,
    string[] Roles);
