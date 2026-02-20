using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointmentByCustomer;

public class CancelAppointmentByCustomerCommandRequest : IRequest<ApiResponse<CancelAppointmentByCustomerCommandResponse>>
{
    public int AppointmentId { get; set; }
}
