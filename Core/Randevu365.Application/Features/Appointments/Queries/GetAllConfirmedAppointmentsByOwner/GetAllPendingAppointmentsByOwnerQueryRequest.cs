using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Queries.GetAllPendingAppointmentsByOwner;

public class GetAllConfirmedAppointmentsByOwnerQueryRequest : IRequest<ApiResponse<GetAllConfirmedAppointmentsByOwnerQueryResponse>>
{
    public bool OnlyConfirmed { get; set; }
}
