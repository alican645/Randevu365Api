using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.ApprovePackage;

public class ApprovePackageCommandHandler : IRequestHandler<ApprovePackageCommandRequest, ApiResponse<ApprovePackageCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApprovePackageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ApprovePackageCommandResponse>> Handle(ApprovePackageCommandRequest request, CancellationToken cancellationToken)
    {
        var slots = await _unitOfWork.GetReadRepository<BusinessSlot>()
            .GetAllAsync(predicate: x => x.PackageId == request.PackageId && !x.IsDeleted, enableTracking: true);

        if (slots == null || slots.Count == 0)
            return ApiResponse<ApprovePackageCommandResponse>.NotFoundResult("Paket bulunamadı.");

        if (slots.All(x => x.PaymentStatus == SlotPaymentStatus.Completed))
            return ApiResponse<ApprovePackageCommandResponse>.ConflictResult("Bu paket zaten onaylanmış.");

        var paidAt = DateTime.UtcNow;

        foreach (var slot in slots)
        {
            slot.PaymentStatus = SlotPaymentStatus.Completed;
            slot.PaidAt = paidAt;
            if (request.Notes is not null)
                slot.Notes = request.Notes;

            await _unitOfWork.GetWriteRepository<BusinessSlot>().UpdateAsync(slot);
        }

        await _unitOfWork.SaveAsync();

        return ApiResponse<ApprovePackageCommandResponse>.SuccessResult(
            new ApprovePackageCommandResponse
            {
                PackageId = request.PackageId,
                ApprovedSlotCount = slots.Count,
                PaidAt = paidAt
            },
            "Paket başarıyla onaylandı.");
    }
}
