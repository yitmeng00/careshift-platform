using ClinicalScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalScheduler.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ShiftType).HasConversion<string>().IsRequired();
        builder.Property(s => s.Notes).HasMaxLength(500);

        builder.HasOne(s => s.Staff)
            .WithMany(st => st.Shifts)
            .HasForeignKey(s => s.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Department)
            .WithMany(d => d.Shifts)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
