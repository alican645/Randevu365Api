using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Commands.ApproveAppointment;

public class ApproveAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
    public TimeOnly? ApproveStartTime { get; set; }
    public TimeOnly? ApproveEndTime { get; set; }
}
