using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessSlotConfiguration : IEntityTypeConfiguration<BusinessSlot>
{
    public void Configure(EntityTypeBuilder<BusinessSlot> builder)
    {
        builder.Property(x => x.PurchasePrice).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(x => x.PaymentStatus).HasConversion<int>().IsRequired();
        builder.Property(x => x.PaymentMethod).HasConversion<int>();
        builder.Property(x => x.ExternalTransactionId).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasOne(x => x.AppUser)
            .WithMany(x => x.BusinessSlots)
            .HasForeignKey(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UsedForBusiness)
            .WithOne()
            .HasForeignKey<BusinessSlot>(x => x.UsedForBusinessId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.PackageType).HasConversion<int?>();
        builder.HasIndex(x => new { x.AppUserId, x.IsUsed, x.PaymentStatus });
        builder.HasIndex(x => x.PackageId);
    }
}
