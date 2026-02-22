using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessSlots.Commands.ApproveSlot;

public class ApproveSlotCommandRequest : IRequest<ApiResponse<ApproveSlotCommandResponse>>
{
    public int SlotId { get; set; }
    public string? Notes { get; set; }
}
