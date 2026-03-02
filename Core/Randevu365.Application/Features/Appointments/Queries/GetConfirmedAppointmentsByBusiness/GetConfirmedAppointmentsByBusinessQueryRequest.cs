using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Queries.GetConfirmedAppointmentsByBusiness;

public class GetConfirmedAppointmentsByBusinessQueryRequest : IRequest<ApiResponse<GetConfirmedAppointmentsByBusinessQueryResponse>>
{
    public int BusinessId { get; set; }
}
