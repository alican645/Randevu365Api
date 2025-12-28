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
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
