using Folha360.Application.Commands;
using Folha360.Application.DTOs;

namespace Folha360.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken ct = default);
}
