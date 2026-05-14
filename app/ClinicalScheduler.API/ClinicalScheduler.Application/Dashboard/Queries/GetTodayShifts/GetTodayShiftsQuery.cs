using ClinicalScheduler.Application.Dashboard.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetTodayShifts;

public record GetTodayShiftsQuery : IRequest<List<TodayShiftDto>>;
