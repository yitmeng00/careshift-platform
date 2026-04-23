using ClinicalScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalScheduler.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).HasMaxLength(200).IsRequired();
        builder.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Detail).HasMaxLength(500);
        builder.Property(a => a.Icon).HasMaxLength(10);
        builder.Property(a => a.PerformedBy).HasMaxLength(100).IsRequired();

        builder.HasOne(a => a.Staff)
            .WithMany(s => s.AuditLogs)
            .HasForeignKey(a => a.StaffId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
