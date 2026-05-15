using ClinicalScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Staff> Staff { get; }
    DbSet<Shift> Shifts { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<Department> Departments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
