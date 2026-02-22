using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.RequestSlot;

public class RequestSlotCommandRequest : IRequest<ApiResponse<RequestSlotCommandResponse>>
{
    public SlotPackageType PackageType { get; set; } = SlotPackageType.Single;
    public SlotPaymentMethod PaymentMethod { get; set; }
    public string? ExternalTransactionId { get; set; }
}
