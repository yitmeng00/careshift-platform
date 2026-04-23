using ClinicalScheduler.Application.Dashboard.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetRecentActivity;

public record GetRecentActivityQuery(int Count = 8) : IRequest<IReadOnlyList<ActivityEntryDto>>;
