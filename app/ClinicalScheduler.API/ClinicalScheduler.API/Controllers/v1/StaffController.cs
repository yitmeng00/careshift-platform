using Asp.Versioning;
using ClinicalScheduler.Application.Personnel.Queries.GetStaffList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalScheduler.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class StaffController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken ct)
        => Ok(await mediator.Send(new GetStaffListQuery(), ct));
}