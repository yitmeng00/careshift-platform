using ClinicalScheduler.Application.Shifts.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Shifts.Queries.GetShiftsByWeek;

public record GetShiftsByWeekQuery(
    DateOnly WeekStart,
    int? DepartmentId = null,
    DateOnly? MonthStart = null)
    : IRequest<List<ShiftDto>>;
