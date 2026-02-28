using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Queries.GetPendingAppointmentsByBusiness;

public class GetPendingAppointmentsByBusinessQueryRequest : IRequest<ApiResponse<GetPendingAppointmentsByBusinessQueryResponse>>
{
    public int BusinessId { get; set; }
}
