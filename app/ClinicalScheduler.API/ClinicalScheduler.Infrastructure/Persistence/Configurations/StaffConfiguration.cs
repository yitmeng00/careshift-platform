using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalScheduler.Infrastructure.Persistence.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.FullName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(200).IsRequired();
        builder.Property(s => s.PasswordHash).IsRequired();
        builder.Property(s => s.Role).HasConversion<string>().IsRequired();
        builder.Property(s => s.EmploymentType).HasConversion<string>();
        builder.Property(s => s.Initials).HasMaxLength(5);
        builder.Property(s => s.Phone).HasMaxLength(30);

        builder.HasIndex(s => s.Email).IsUnique();

        builder.HasOne(s => s.Department)
            .WithMany(d => d.Staff)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
