using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.CancelAppointmentByCustomer;

public class CancelAppointmentByCustomerCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
