using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(f => f.Id);
        builder.HasOne(f => f.AppUser).WithMany(u => u.Favorites).HasForeignKey(f => f.AppUserId);
        builder.HasOne(f => f.Business).WithMany(b => b.Favorites).HasForeignKey(f => f.BusinessId);
        builder.HasIndex(f => new { f.AppUserId, f.BusinessId }).IsUnique();
    }
}
