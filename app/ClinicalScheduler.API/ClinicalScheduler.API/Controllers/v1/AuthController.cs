using Asp.Versioning;
using ClinicalScheduler.Application.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalScheduler.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}