using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.ApproveSlot;

public class ApproveSlotCommandResponse
{
    public int SlotId { get; set; }
    public SlotPaymentStatus PaymentStatus { get; set; }
    public DateTime PaidAt { get; set; }
}
