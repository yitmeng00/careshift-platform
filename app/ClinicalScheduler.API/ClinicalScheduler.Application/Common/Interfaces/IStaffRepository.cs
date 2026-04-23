using ClinicalScheduler.Domain.Entities;

namespace ClinicalScheduler.Application.Common.Interfaces;

public interface IStaffRepository
{
    Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Staff> AddAsync(Staff staff, CancellationToken cancellationToken = default);
    Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default);
}
