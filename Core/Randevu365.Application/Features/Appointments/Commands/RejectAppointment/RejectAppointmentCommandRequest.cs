using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.RejectAppointment;

public class RejectAppointmentCommandRequest : IRequest<ApiResponse<RejectAppointmentCommandResponse>>
{
    public int AppointmentId { get; set; }
    public string? BusinessNotes { get; set; }
}
