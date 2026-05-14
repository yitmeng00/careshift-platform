using ClinicalScheduler.Domain.Enums;
using FluentValidation;

namespace ClinicalScheduler.Application.Shifts.Commands.CreateShift;

public class CreateShiftCommandValidator : AbstractValidator<CreateShiftCommand>
{
    public CreateShiftCommandValidator()
    {
        RuleFor(x => x.StaffId).GreaterThan(0).WithMessage("Staff is required.");
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("Department is required.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
        RuleFor(x => x.ShiftType)
            .Must(st => Enum.TryParse<ShiftType>(st, out _))
            .WithMessage("Shift type must be Morning, Afternoon, or Night.");
    }
}
