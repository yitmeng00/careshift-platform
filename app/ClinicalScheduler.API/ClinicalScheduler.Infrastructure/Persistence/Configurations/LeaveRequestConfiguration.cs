using ClinicalScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalScheduler.Infrastructure.Persistence.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.LeaveType).HasConversion<string>().IsRequired();
        builder.Property(r => r.Status).HasConversion<string>().IsRequired();
        builder.Property(r => r.Reason).HasMaxLength(500).IsRequired();
        builder.Property(r => r.ReviewNote).HasMaxLength(500);

        builder.HasOne(r => r.Staff)
            .WithMany(s => s.LeaveRequests)
            .HasForeignKey(r => r.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ReviewedBy)
            .WithMany()
            .HasForeignKey(r => r.ReviewedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(r => r.AuditEntries)
            .WithOne(e => e.LeaveRequest)
            .HasForeignKey(e => e.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
