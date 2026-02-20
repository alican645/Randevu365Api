using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.ConfirmAppointment;

public class ConfirmAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
