using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.VerifyInAppPurchase;

public class VerifyInAppPurchaseCommandResponse
{
    public Guid PackageId { get; set; }
    public SlotPackageType PackageType { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal PricePerSlot { get; set; }
    public SlotPaymentStatus PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
}
