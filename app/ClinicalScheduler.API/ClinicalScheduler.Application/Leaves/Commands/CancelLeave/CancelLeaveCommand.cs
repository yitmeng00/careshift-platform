using MediatR;

namespace ClinicalScheduler.Application.Leaves.Commands.CancelLeave;

public record CancelLeaveCommand(int LeaveRequestId, int CallerStaffId) : IRequest;