using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetBusinessAppointments;

public class GetBusinessAppointmentsQueryRequest : IRequest<ApiResponse<IList<GetBusinessAppointmentsQueryResponse>>>
{
    public DateOnly? Date { get; set; }
    public AppointmentStatus? Status { get; set; }
}
