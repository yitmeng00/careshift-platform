using ClinicalScheduler.Application.Shifts.Commands.CreateShift;
using ClinicalScheduler.Application.Shifts.Commands.DeleteShift;
using ClinicalScheduler.Application.Shifts.Commands.UpdateShift;
using ClinicalScheduler.Application.Shifts.Queries.GetShiftsByWeek;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalScheduler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShiftsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByWeek(
        [FromQuery] DateOnly? weekStart,
        [FromQuery] DateOnly? monthStart,
        [FromQuery] int? departmentId,
        CancellationToken ct)
    {
        var effectiveWeekStart = weekStart ?? DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok(await mediator.Send(new GetShiftsByWeekQuery(effectiveWeekStart, departmentId, monthStart), ct));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,DepartmentLead")]
    public async Task<IActionResult> Create([FromBody] CreateShiftCommand command, CancellationToken ct)
    {
        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetByWeek), new { weekStart = DateOnly.FromDateTime(dto.StartTime) }, dto);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,DepartmentLead")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShiftRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(new UpdateShiftCommand(id, request.NewDate), ct);
        return Ok(dto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,DepartmentLead")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var info = await mediator.Send(new DeleteShiftCommand(id), ct);
        return NoContent();
    }
}

public record UpdateShiftRequest(DateOnly NewDate);
