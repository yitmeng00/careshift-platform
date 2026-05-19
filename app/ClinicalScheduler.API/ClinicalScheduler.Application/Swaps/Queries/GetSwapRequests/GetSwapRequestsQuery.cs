using ClinicalScheduler.Application.Swaps.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Swaps.Queries.GetSwapRequests;

public record GetSwapRequestsQuery(
    int CallerStaffId,
    int CallerDepartmentId,
    string CallerRole)
    : IRequest<List<SwapRequestDto>>;