using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
