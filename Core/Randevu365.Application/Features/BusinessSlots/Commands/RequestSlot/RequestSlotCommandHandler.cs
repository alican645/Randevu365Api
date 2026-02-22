using MediatR;
using Microsoft.Extensions.Configuration;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.RequestSlot;

public class RequestSlotCommandHandler : IRequestHandler<RequestSlotCommandRequest, ApiResponse<RequestSlotCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;

    public RequestSlotCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _configuration = configuration;
    }

    public async Task<ApiResponse<RequestSlotCommandResponse>> Handle(RequestSlotCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<RequestSlotCommandResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");

        var packageKey = request.PackageType.ToString();
        var priceStr = _configuration[$"BusinessSlot:Packages:{packageKey}"];
        var totalPrice = decimal.TryParse(priceStr, out var parsed) ? parsed : 0m;

        var quantity = (int)request.PackageType;
        var pricePerSlot = quantity > 0 ? totalPrice / quantity : totalPrice;
        var packageId = Guid.NewGuid();

        var slots = new List<BusinessSlot>();
        for (int i = 0; i < quantity; i++)
        {
            slots.Add(new BusinessSlot
            {
                AppUserId = _currentUserService.UserId.Value,
                PurchasePrice = pricePerSlot,
                PaymentStatus = SlotPaymentStatus.Completed,
                PaidAt = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                ExternalTransactionId = request.ExternalTransactionId,
                PackageId = packageId,
                PackageType = request.PackageType
            });
        }

        await _unitOfWork.GetWriteRepository<BusinessSlot>().AddRangeAsync(slots);
        await _unitOfWork.SaveAsync();

        return ApiResponse<RequestSlotCommandResponse>.CreatedResult(
            new RequestSlotCommandResponse
            {
                PackageId = packageId,
                PackageType = request.PackageType,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PricePerSlot = pricePerSlot,
                PaymentStatus = SlotPaymentStatus.Completed
            },
            "Slot paketi başarıyla satın alındı ve aktif edildi.");
    }
}
