using ClinicalScheduler.Application.Leaves.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Leaves.Queries.GetApprovedLeaves;

public record GetApprovedLeavesQuery(DateOnly From, DateOnly To) : IRequest<List<ApprovedLeaveStaffDto>>;