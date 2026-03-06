using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CreateAppointmentByOwner;

public class CreateAppointmentByOwnerCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
