using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandRequest : IRequest<ApiResponse<CompleteAppointmentCommandResponse>>
{
    public int AppointmentId { get; set; }
}
