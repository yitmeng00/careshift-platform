using ClinicalScheduler.Application.Shifts.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Shifts.Queries.GetUpcomingShifts;

public record GetUpcomingShiftsQuery(int StaffId) : IRequest<List<ShiftDto>>;