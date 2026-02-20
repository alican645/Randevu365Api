using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessServiceConfiguration : IEntityTypeConfiguration<BusinessService>
{
    public void Configure(EntityTypeBuilder<BusinessService> builder)
    {
        builder.Property(x => x.ServiceTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ServiceContent)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.MaxConcurrentCustomers)
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(x => x.Business)
            .WithMany(x => x.BusinessServices)
            .HasForeignKey(x => x.BusinessId);
    }
}
