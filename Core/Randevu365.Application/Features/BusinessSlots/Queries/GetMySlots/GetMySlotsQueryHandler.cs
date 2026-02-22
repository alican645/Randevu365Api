using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessSlots.Queries.GetMySlots;

public class GetMySlotsQueryHandler : IRequestHandler<GetMySlotsQueryRequest, ApiResponse<GetMySlotsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMySlotsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetMySlotsQueryResponse>> Handle(GetMySlotsQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<GetMySlotsQueryResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");

        var slots = await _unitOfWork.GetReadRepository<BusinessSlot>()
            .GetAllAsync(predicate: x => x.AppUserId == _currentUserService.UserId.Value && !x.IsDeleted);

        var result = new GetMySlotsQueryResponse
        {
            Slots = slots.Select(s => new SlotItemDto
            {
                Id = s.Id,
                PurchasePrice = s.PurchasePrice,
                PaymentStatus = s.PaymentStatus,
                IsUsed = s.IsUsed,
                PaidAt = s.PaidAt,
                CreatedAt = s.CreatedAt,
                PackageId = s.PackageId,
                PackageType = s.PackageType
            }).ToList()
        };

        return ApiResponse<GetMySlotsQueryResponse>.SuccessResult(result);
    }
}
