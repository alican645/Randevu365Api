using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessLocationConfiguration : IEntityTypeConfiguration<BusinessLocation>
{
    public void Configure(EntityTypeBuilder<BusinessLocation> builder)
    {
        builder.ToTable("BusinessLocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Latitude)
            .IsRequired()
            .HasPrecision(18, 8);

        builder.Property(x => x.Longitude)
            .IsRequired()
            .HasPrecision(18, 8);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Business ile ilişki (parent tarafından tanımlanıyor, burada sadece FK index)
        builder.HasIndex(x => x.BusinessId);
    }
}