using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne(x => x.AppUserInformation)
            .WithOne()
            .HasForeignKey<AppUser>(x => x.AppUserInformationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Businesses)
            .WithOne(x => x.AppUser)
            .HasForeignKey(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Email için unique index
        builder.HasIndex(x => x.Email).IsUnique();
    }
}