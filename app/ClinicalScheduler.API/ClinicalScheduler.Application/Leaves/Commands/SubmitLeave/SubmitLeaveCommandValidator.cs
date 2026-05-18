using ClinicalScheduler.Domain.Enums;
using FluentValidation;

namespace ClinicalScheduler.Application.Leaves.Commands.SubmitLeave;

public class SubmitLeaveCommandValidator : AbstractValidator<SubmitLeaveCommand>
{
    public SubmitLeaveCommandValidator()
    {
        RuleFor(x => x.LeaveType)
            .Must(v => Enum.TryParse<LeaveType>(v, out _))
            .WithMessage("Invalid leave type.");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Start date must be today or in the future.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}