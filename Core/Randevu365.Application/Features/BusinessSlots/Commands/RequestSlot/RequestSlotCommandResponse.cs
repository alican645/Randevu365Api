using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.RequestSlot;

public class RequestSlotCommandResponse
{
    public Guid PackageId { get; set; }
    public SlotPackageType PackageType { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal PricePerSlot { get; set; }
    public SlotPaymentStatus PaymentStatus { get; set; }
}
