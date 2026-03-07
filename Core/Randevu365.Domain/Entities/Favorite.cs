using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class Favorite : BaseEntity
{
    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }

    public int BusinessId { get; set; }
    public virtual Business? Business { get; set; }
}
