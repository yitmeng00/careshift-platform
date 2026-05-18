using ClinicalScheduler.Application.Leaves.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Leaves.Queries.GetLeaveRequests;

public record GetLeaveRequestsQuery(
    int CallerStaffId,
    int CallerDepartmentId,
    string CallerRole) : IRequest<List<LeaveRequestDto>>;