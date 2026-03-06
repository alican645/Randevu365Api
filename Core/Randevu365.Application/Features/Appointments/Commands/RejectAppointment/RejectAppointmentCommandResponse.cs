using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.RejectAppointment;

public class RejectAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
