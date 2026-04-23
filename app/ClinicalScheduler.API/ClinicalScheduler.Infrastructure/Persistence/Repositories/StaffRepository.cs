using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Infrastructure.Persistence.Repositories;

public class StaffRepository(AppDbContext context) : IStaffRepository
{
    public async Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Staff
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Staff
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Staff
            .Include(s => s.Department)
            .OrderBy(s => s.FullName)
            .ToListAsync(cancellationToken);

    public async Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        context.Staff.Add(staff);
        await context.SaveChangesAsync(cancellationToken);
        return staff;
    }

    public async Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        context.Staff.Update(staff);
        await context.SaveChangesAsync(cancellationToken);
    }
}
