using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.RevertConfirmedAppointment;

public class RevertConfirmedAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
