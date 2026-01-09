using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessRatingConfiguration : IEntityTypeConfiguration<BusinessRating>
{
    public void Configure(EntityTypeBuilder<BusinessRating> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Rating).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired().HasDefaultValue(DateTime.Now);
        builder.HasOne(b => b.Business).WithMany(b => b.BusinessRatings).HasForeignKey(b => b.BusinessId);
        builder.HasOne(b => b.AppUser).WithMany(b => b.BusinessRatings).HasForeignKey(b => b.AppUserId);
    }
}
