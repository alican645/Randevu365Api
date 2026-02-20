using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class BusinessService : BaseEntity
{
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; } = 1;

    public int BusinessId { get; set; }
    public virtual Business Business { get; set; } = null!;
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
