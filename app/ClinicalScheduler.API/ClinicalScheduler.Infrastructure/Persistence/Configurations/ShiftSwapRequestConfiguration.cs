using ClinicalScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalScheduler.Infrastructure.Persistence.Configurations;

public class ShiftSwapRequestConfiguration : IEntityTypeConfiguration<ShiftSwapRequest>
{
    public void Configure(EntityTypeBuilder<ShiftSwapRequest> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Status).HasConversion<string>().IsRequired();
        builder.Property(r => r.Reason).HasMaxLength(500).IsRequired();

        builder.HasOne(r => r.Requester)
            .WithMany()
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Requestee)
            .WithMany()
            .HasForeignKey(r => r.RequesteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RequesterShift)
            .WithMany()
            .HasForeignKey(r => r.RequesterShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RequesteeShift)
            .WithMany()
            .HasForeignKey(r => r.RequesteeShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.AuditEntries)
            .WithOne(e => e.ShiftSwapRequest)
            .HasForeignKey(e => e.ShiftSwapRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
