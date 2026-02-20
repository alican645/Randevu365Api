using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointmentByBusiness;

public class CancelAppointmentByBusinessCommandRequest : IRequest<ApiResponse<CancelAppointmentByBusinessCommandResponse>>
{
    public int AppointmentId { get; set; }
    public string? BusinessNotes { get; set; }
}
