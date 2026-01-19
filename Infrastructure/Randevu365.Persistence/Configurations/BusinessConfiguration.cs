using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;


namespace Randevu365.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
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
        #endregion
    }
}