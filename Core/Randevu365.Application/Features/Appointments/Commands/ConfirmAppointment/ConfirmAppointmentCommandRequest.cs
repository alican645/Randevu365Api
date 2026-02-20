using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.ConfirmAppointment;

public class ConfirmAppointmentCommandRequest : IRequest<ApiResponse<ConfirmAppointmentCommandResponse>>
{
    public int AppointmentId { get; set; }
}
