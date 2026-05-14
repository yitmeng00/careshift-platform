using ClinicalScheduler.Application.Dashboard.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetPendingLeaves;

public record GetPendingLeavesQuery(string Role, int DepartmentId) : IRequest<List<PendingLeaveDto>>;
