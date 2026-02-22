using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Commands.ApproveSlot;

public class ApproveSlotCommandHandler : IRequestHandler<ApproveSlotCommandRequest, ApiResponse<ApproveSlotCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveSlotCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ApproveSlotCommandResponse>> Handle(ApproveSlotCommandRequest request, CancellationToken cancellationToken)
    {
        var slot = await _unitOfWork.GetReadRepository<BusinessSlot>()
            .GetAsync(predicate: x => x.Id == request.SlotId && !x.IsDeleted);

        if (slot == null)
            return ApiResponse<ApproveSlotCommandResponse>.NotFoundResult("Slot bulunamadı.");

        if (slot.PaymentStatus == SlotPaymentStatus.Completed)
            return ApiResponse<ApproveSlotCommandResponse>.ConflictResult("Bu slot zaten onaylanmış.");

        slot.PaymentStatus = SlotPaymentStatus.Completed;
        slot.PaidAt = DateTime.UtcNow;

        if (request.Notes is not null)
            slot.Notes = request.Notes;

        await _unitOfWork.GetWriteRepository<BusinessSlot>().UpdateAsync(slot);
        await _unitOfWork.SaveAsync();

        return ApiResponse<ApproveSlotCommandResponse>.SuccessResult(
            new ApproveSlotCommandResponse
            {
                SlotId = slot.Id,
                PaymentStatus = slot.PaymentStatus,
                PaidAt = slot.PaidAt.Value
            },
            "Slot başarıyla onaylandı.");
    }
}
