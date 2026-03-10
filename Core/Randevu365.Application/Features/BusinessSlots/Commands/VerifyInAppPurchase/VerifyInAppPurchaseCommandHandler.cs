using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.VerifyInAppPurchase;

public class VerifyInAppPurchaseCommandHandler : IRequestHandler<VerifyInAppPurchaseCommandRequest, ApiResponse<VerifyInAppPurchaseCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIapVerificationService _iapVerificationService;
    private readonly IConfiguration _configuration;
    private readonly IAppDbContext _context;

    public VerifyInAppPurchaseCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IIapVerificationService iapVerificationService,
        IConfiguration configuration,
        IAppDbContext context)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _iapVerificationService = iapVerificationService;
        _configuration = configuration;
        _context = context;
    }

    public async Task<ApiResponse<VerifyInAppPurchaseCommandResponse>> Handle(
        VerifyInAppPurchaseCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<VerifyInAppPurchaseCommandResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");

        // Platform'a göre doğrulama
        IapVerificationResult result;
        if (request.Platform.Equals("apple", StringComparison.OrdinalIgnoreCase))
        {
            result = await _iapVerificationService.VerifyAppleReceiptAsync(request.ReceiptData);
        }
        else
        {
            result = await _iapVerificationService.VerifyGooglePurchaseAsync(
                request.PackageName!, request.ProductId!, request.ReceiptData);
        }

        if (!result.IsValid)
            return ApiResponse<VerifyInAppPurchaseCommandResponse>.FailResult(
                result.ErrorMessage ?? "Satın alma doğrulaması başarısız.");

        // İdempotency kontrolü
        var existingSlot = await _context.BusinessSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ExternalTransactionId == result.TransactionId, cancellationToken);

        if (existingSlot is not null)
            return ApiResponse<VerifyInAppPurchaseCommandResponse>.ConflictResult(
                "Bu satın alma işlemi zaten işlenmiş.");

        // Fiyat hesaplama
        var packageKey = request.PackageType.ToString();
        var priceStr = _configuration[$"BusinessSlot:Packages:{packageKey}"];
        var totalPrice = decimal.TryParse(priceStr, out var parsed) ? parsed : 0m;

        var quantity = (int)request.PackageType;
        var pricePerSlot = quantity > 0 ? totalPrice / quantity : totalPrice;
        var packageId = Guid.NewGuid();

        // Slot oluşturma
        var slots = new List<BusinessSlot>();
        for (int i = 0; i < quantity; i++)
        {
            slots.Add(new BusinessSlot
            {
                AppUserId = _currentUserService.UserId.Value,
                PurchasePrice = pricePerSlot,
                PaymentStatus = SlotPaymentStatus.Completed,
                PaidAt = DateTime.UtcNow,
                PaymentMethod = SlotPaymentMethod.InAppPurchase,
                ExternalTransactionId = result.TransactionId,
                PackageId = packageId,
                PackageType = request.PackageType
            });
        }

        await _unitOfWork.GetWriteRepository<BusinessSlot>().AddRangeAsync(slots);
        await _unitOfWork.SaveAsync();

        return ApiResponse<VerifyInAppPurchaseCommandResponse>.CreatedResult(
            new VerifyInAppPurchaseCommandResponse
            {
                PackageId = packageId,
                PackageType = request.PackageType,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PricePerSlot = pricePerSlot,
                PaymentStatus = SlotPaymentStatus.Completed,
                TransactionId = result.TransactionId
            },
            "Satın alma doğrulandı ve slotlar başarıyla oluşturuldu.");
    }
}
