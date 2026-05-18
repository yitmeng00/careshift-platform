using ClinicalScheduler.Application.Leaves.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Leaves.Commands.ReviewLeave;

public record ReviewLeaveCommand(
    int LeaveRequestId,
    int ReviewerId,
    string ReviewerName,
    string Action,  // "Approve" | "Reject"
    string? Note) : IRequest<LeaveRequestDto>;