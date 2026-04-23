using ClinicalScheduler.Application.Dashboard.Queries.GetDashboardStats;
using ClinicalScheduler.Application.Dashboard.Queries.GetRecentActivity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalScheduler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetDashboardStatsQuery(), cancellationToken));

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetRecentActivityQuery(), cancellationToken));
}
