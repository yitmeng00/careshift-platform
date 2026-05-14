using ClinicalScheduler.Application.Personnel.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Personnel.Queries.GetStaffList;

public record GetStaffListQuery : IRequest<List<StaffSummaryDto>>;
