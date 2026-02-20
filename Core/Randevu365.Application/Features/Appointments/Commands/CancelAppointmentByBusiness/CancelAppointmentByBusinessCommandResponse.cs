using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointmentByBusiness;

public class CancelAppointmentByBusinessCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
