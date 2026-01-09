using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessPhotoConfiguration : IEntityTypeConfiguration<BusinessPhoto>
{
    public void Configure(EntityTypeBuilder<BusinessPhoto> builder)
    {
        builder.ToTable("BusinessPhotos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PhotoPath)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.Now);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Business ile ilişki (parent tarafından tanımlanıyor, burada sadece FK index)
        builder.HasIndex(x => x.BusinessId);
    }
}