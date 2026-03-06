using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.RevertConfirmedAppointment;

public class RevertConfirmedAppointmentCommandRequest : IRequest<ApiResponse<RevertConfirmedAppointmentCommandResponse>>
{
    public int AppointmentId { get; set; }
}
