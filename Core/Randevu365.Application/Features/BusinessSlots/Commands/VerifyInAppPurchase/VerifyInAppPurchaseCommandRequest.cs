using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.VerifyInAppPurchase;

public class VerifyInAppPurchaseCommandRequest : IRequest<ApiResponse<VerifyInAppPurchaseCommandResponse>>
{
    public string Platform { get; set; } = string.Empty;
    public string ReceiptData { get; set; } = string.Empty;
    public string? PackageName { get; set; }
    public string? ProductId { get; set; }
    public SlotPackageType PackageType { get; set; }
}
