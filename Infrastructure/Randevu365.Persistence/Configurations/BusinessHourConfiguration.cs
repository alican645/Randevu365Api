using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessHourConfiguration : IEntityTypeConfiguration<BusinessHour>
{
    public void Configure(EntityTypeBuilder<BusinessHour> builder)
    {
        builder.HasOne(x => x.Business)
            .WithMany(x => x.BusinessHours)
            .HasForeignKey(x => x.BusinessId);
    }
}
