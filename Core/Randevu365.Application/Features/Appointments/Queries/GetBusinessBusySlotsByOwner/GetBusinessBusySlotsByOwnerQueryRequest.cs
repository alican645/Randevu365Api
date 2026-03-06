using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Queries.GetBusinessBusySlotsByOwner;

public class GetBusinessBusySlotsByOwnerQueryRequest : IRequest<ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>>
{
    public int BusinessId { get; set; }
}
