using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class AppUserInformationConfiguration : IEntityTypeConfiguration<AppUserInformation>
{
    public void Configure(EntityTypeBuilder<AppUserInformation> builder)
    {

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Surname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Age)
            .IsRequired();

        builder.Property(x => x.Gender)
            .IsRequired()
            .HasMaxLength(1);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Height)
            .IsRequired();

        builder.Property(x => x.Weight)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}