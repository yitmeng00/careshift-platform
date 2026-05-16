using Asp.Versioning;
using ClinicalScheduler.Application.Auth.Commands.Login;
using ClinicalScheduler.Application.Auth.Commands.Logout;
using ClinicalScheduler.Application.Auth.Commands.Refresh;
using ClinicalScheduler.Application.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalScheduler.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";

    private void SetRefreshTokenCookie(string token, int expiryDays)
    {
        Response.Cookies.Append(RefreshTokenCookie, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(expiryDays),
        });
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        SetRefreshTokenCookie(result.RefreshToken, 7);
        return Ok(new LoginResponseDto(result.AccessToken, result.ExpiresIn, result.Staff));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var token = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Refresh token missing.");

        var result = await mediator.Send(new RefreshCommand(token), cancellationToken);
        SetRefreshTokenCookie(result.RefreshToken, 7);
        return Ok(new LoginResponseDto(result.AccessToken, result.ExpiresIn, result.Staff));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var token = Request.Cookies[RefreshTokenCookie];
        if (!string.IsNullOrEmpty(token))
            await mediator.Send(new LogoutCommand(token), cancellationToken);

        ClearRefreshTokenCookie();
        return NoContent();
    }
}