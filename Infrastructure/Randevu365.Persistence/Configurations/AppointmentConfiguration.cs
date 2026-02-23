using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.CustomerNotes).HasMaxLength(500);
        builder.Property(x => x.BusinessNotes).HasMaxLength(500);

        builder.HasOne(x => x.AppUser)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BusinessService)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.BusinessServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BusinessId, x.AppointmentDate, x.RequestedStartTime });
    }
}
