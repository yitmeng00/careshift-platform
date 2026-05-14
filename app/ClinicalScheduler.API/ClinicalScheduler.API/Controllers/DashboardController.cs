using ClinicalScheduler.Application.Dashboard.Queries.GetDashboardStats;
using ClinicalScheduler.Application.Dashboard.Queries.GetPendingLeaves;
using ClinicalScheduler.Application.Dashboard.Queries.GetTodayShifts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicalScheduler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetDashboardStatsQuery(), cancellationToken));

    [HttpGet("today-shifts")]
    public async Task<IActionResult> GetTodayShifts(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetTodayShiftsQuery(), cancellationToken));

    [HttpGet("pending-leaves")]
    [Authorize(Roles = "Admin,DepartmentLead")]
    public async Task<IActionResult> GetPendingLeaves(CancellationToken cancellationToken)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var deptId = int.Parse(User.FindFirstValue("departmentId") ?? "0");
        return Ok(await mediator.Send(new GetPendingLeavesQuery(role, deptId), cancellationToken));
    }
}
