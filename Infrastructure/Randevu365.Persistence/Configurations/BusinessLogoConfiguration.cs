using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessLogoConfiguration : IEntityTypeConfiguration<BusinessLogo>
{
    public void Configure(EntityTypeBuilder<BusinessLogo> builder)
    {
        builder.HasOne(x => x.Business)
            .WithOne(x => x.BusinessLogo)
            .HasForeignKey<BusinessLogo>(x => x.BusinessId);
    }
}
