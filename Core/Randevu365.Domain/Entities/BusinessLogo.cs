using Randevu365.Domain.Base;

namespace Randevu365.Domain.Entities;

public class BusinessLogo : BaseEntity
{
    public required string LogoUrl { get; set; }
    public int BusinessId { get; set; }
    public virtual Business Business { get; set; } = null!;
}