using ClinicalScheduler.Domain.Enums;

namespace ClinicalScheduler.Application.Shifts;

internal static class ShiftTimeHelper
{
    internal static (DateTime StartTime, DateTime EndTime) Compute(DateOnly date, ShiftType shiftType)
        => shiftType switch
        {
            ShiftType.Morning   => (date.ToDateTime(new TimeOnly(7,  0), DateTimeKind.Utc),
                                    date.ToDateTime(new TimeOnly(15, 0), DateTimeKind.Utc)),
            ShiftType.Afternoon => (date.ToDateTime(new TimeOnly(15, 0), DateTimeKind.Utc),
                                    date.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc)),
            ShiftType.Night     => (date.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc),
                                    date.AddDays(1).ToDateTime(new TimeOnly(7, 0), DateTimeKind.Utc)),
            _ => throw new ArgumentOutOfRangeException(nameof(shiftType)),
        };
}
