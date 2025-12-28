using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class Business : BaseEntity
{
    public required string BusinessName { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessCity { get; set; }
    public required string BusinessPhone { get; set; }
    public required string BusinessEmail { get; set; }
    public required string BusinessCountry { get; set; }

    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }
    public virtual ICollection<BusinessLocation> BusinessLocations { get; set; } = new List<BusinessLocation>();
    public virtual ICollection<BusinessPhoto> BusinessPhotos { get; set; } = new List<BusinessPhoto>();
}