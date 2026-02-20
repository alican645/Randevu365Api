using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
