using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
