using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Queries.GetMySlots;

public class GetMySlotsQueryResponse
{
    public List<SlotItemDto> Slots { get; set; } = new();
}

public class SlotItemDto
{
    public int Id { get; set; }
    public decimal PurchasePrice { get; set; }
    public SlotPaymentStatus PaymentStatus { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? PackageId { get; set; }
    public SlotPackageType? PackageType { get; set; }
}
