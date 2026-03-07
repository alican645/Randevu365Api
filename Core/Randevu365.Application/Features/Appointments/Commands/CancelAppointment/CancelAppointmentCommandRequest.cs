using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandRequest : IRequest<ApiResponse<CancelAppointmentCommandResponse>>
{
    public int AppointmentId { get; set; }
    public string? CancellationReason { get; set; }
}
