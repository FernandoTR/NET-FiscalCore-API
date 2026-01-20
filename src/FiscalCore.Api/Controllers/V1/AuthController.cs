using Asp.Versioning;
using FiscalCore.Application.DTOs.Auth;
using FiscalCore.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FiscalCore.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    /// <summary>
    /// Autenticación de usuario (login)
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
            return Unauthorized("Credenciales inválidas");

        return Ok(result);
    }

    /// <summary>
    /// Renovación de access token mediante refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (result is null)
            return Unauthorized("Refresh token inválido o expirado");

        return Ok(result);
    }


}
