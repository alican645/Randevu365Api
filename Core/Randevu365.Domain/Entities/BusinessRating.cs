using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class BusinessRating : BaseEntity
{
    public int BusinessId { get; set; }
    public int AppUserId { get; set; }
    public int Rating { get; set; }
    public virtual Business? Business { get; set; }
    public virtual AppUser? AppUser { get; set; }
}