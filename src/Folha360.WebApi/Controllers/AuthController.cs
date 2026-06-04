using Folha360.Application.Commands;
using Folha360.Application.DTOs;
using Folha360.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginCommand> _loginValidator;

    public AuthController(IAuthService authService, IValidator<LoginCommand> loginValidator)
    {
        _authService = authService;
        _loginValidator = loginValidator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var validationResult = await _loginValidator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return UnprocessableEntity(new
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Validation Error",
                Status = 422,
                Detail = "One or more validation errors occurred.",
                Instance = Request.Path,
                Errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            });
        }

        try
        {
            var response = await _authService.LoginAsync(command, ct);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = 401,
                Detail = "Credenciais inválidas",
                Instance = Request.Path
            });
        }
    }

    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Refresh()
    {
        // TODO: Implementar refresh token na F10 (segurança)
        return Ok(new { message = "Refresh token endpoint - not implemented yet" });
    }
}
