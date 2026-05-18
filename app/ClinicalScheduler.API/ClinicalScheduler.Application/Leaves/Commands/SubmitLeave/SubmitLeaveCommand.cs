using ClinicalScheduler.Application.Leaves.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Leaves.Commands.SubmitLeave;

public record SubmitLeaveCommand(
    int StaffId,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason) : IRequest<LeaveRequestDto>;