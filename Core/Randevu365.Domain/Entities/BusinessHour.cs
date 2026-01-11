
using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class BusinessHour : BaseEntity
{
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
    public int BusinessId { get; set; }
    public virtual Business Business { get; set; } = null!;
}