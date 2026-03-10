using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ConversationId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.IsClosed)
            .HasDefaultValue(false);

        builder.HasOne(c => c.Appointment)
            .WithOne(a => a.Conversation)
            .HasForeignKey<Conversation>(c => c.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.AppointmentId)
            .IsUnique()
            .HasFilter("\"AppointmentId\" IS NOT NULL");

        builder.HasIndex(c => c.ConversationId);
    }
}
