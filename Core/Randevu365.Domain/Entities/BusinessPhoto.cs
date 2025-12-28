using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class BusinessPhoto : BaseEntity
{
    public int BusinessId { get; set; }
    public virtual Business? Business { get; set; }

    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; }
}