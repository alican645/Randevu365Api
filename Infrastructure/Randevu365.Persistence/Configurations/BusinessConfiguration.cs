using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    private static BusinessCategory? ParseCategory(string? v) =>
        BusinessCategoryExtensions.TryFromJson(v, out var cat) ? cat : (BusinessCategory?)null;

    public void Configure(EntityTypeBuilder<Business> builder)
    {

        #region Temel property tanımlamaları 
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BusinessName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BusinessAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.BusinessCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BusinessPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.BusinessEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.BusinessCountry)
            .IsRequired()
            .HasMaxLength(100);

        var categoryConverter = new ValueConverter<BusinessCategory?, string?>(
            v => v.HasValue ? v.Value.ToJson() : null,
            v => ParseCategory(v));

        builder.Property(x => x.BusinessCategory)
            .HasConversion(categoryConverter)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        #endregion

        #region Cascade Silme İlişkileri
        builder.HasMany(x => x.BusinessLocations)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BusinessPhotos)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BusinessHours)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BusinessLogo)
            .WithOne(x => x.Business)
            .HasForeignKey<BusinessLogo>(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BusinessComments)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BusinessRatings)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BusinessServices)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Appointments)
            .WithOne(x => x.Business)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}