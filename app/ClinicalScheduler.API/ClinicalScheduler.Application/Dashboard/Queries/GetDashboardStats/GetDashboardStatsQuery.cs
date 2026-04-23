using ClinicalScheduler.Application.Dashboard.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;
