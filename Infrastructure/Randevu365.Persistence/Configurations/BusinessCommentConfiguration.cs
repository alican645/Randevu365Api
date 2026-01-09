
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class BusinessCommentConfiguration : IEntityTypeConfiguration<BusinessComment>
{
    public void Configure(EntityTypeBuilder<BusinessComment> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Comment).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired().HasDefaultValue(DateTime.Now);
        builder.HasOne(b => b.Business).WithMany(b => b.BusinessComments).HasForeignKey(b => b.BusinessId);
        builder.HasOne(b => b.AppUser).WithMany(b => b.BusinessComments).HasForeignKey(b => b.AppUserId);
    }
}