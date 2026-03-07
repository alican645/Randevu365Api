using Microsoft.EntityFrameworkCore;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<AppUser> AppUsers { get; set; }
    DbSet<Business> Businesses { get; set; }
    DbSet<AppUserInformation> AppUserInformations { get; set; }
    DbSet<BusinessLocation> BusinessLocations { get; set; }
    DbSet<BusinessPhoto> BusinessPhotos { get; set; }
    DbSet<Appointment> Appointments { get; set; }
    DbSet<BusinessService> BusinessServices { get; set; }
    DbSet<BusinessSlot> BusinessSlots { get; set; }
    DbSet<BusinessHour> BusinessHours { get; set; }
    DbSet<Conversation> Conversations { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<AuditLog> AuditLogs { get; set; }
    DbSet<BusinessComment> BusinessComments { get; set; }
    DbSet<BusinessRating> BusinessRatings { get; set; }
    DbSet<BusinessLogo> BusinessLogos { get; set; }
    DbSet<Favorite> Favorites { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
