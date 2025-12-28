using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Context;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<AppUserInformation> AppUserInformations { get; set; }
    public DbSet<BusinessLocation> BusinessLocations { get; set; }
    public DbSet<BusinessPhoto> BusinessPhotos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

