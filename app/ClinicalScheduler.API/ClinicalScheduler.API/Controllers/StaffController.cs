using ClinicalScheduler.Application.Personnel.Queries.GetStaffList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalScheduler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StaffController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken ct)
        => Ok(await mediator.Send(new GetStaffListQuery(), ct));
}
