using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryRequest : IRequest<ApiResponse<GetAppointmentByIdQueryResponse>>
{
    public int AppointmentId { get; set; }
}
