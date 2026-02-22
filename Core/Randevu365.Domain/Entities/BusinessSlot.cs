using Randevu365.Domain.Base;
using Randevu365.Domain.Enum;

namespace Randevu365.Domain.Entities;

public class BusinessSlot : BaseEntity
{
    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }

    public decimal PurchasePrice { get; set; }
    public SlotPaymentStatus PaymentStatus { get; set; } = SlotPaymentStatus.Pending;
    public SlotPaymentMethod PaymentMethod { get; set; }
    public string? ExternalTransactionId { get; set; }
    public DateTime? PaidAt { get; set; }

    public bool IsUsed { get; set; } = false;
    public int? UsedForBusinessId { get; set; }
    public virtual Business? UsedForBusiness { get; set; }
    public DateTime? UsedAt { get; set; }

    public string? Notes { get; set; }

    public Guid? PackageId { get; set; }
    public SlotPackageType? PackageType { get; set; }
}
