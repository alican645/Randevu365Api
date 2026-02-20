using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetMyAppointments;

public class GetMyAppointmentsQueryResponse
{
    public int Id { get; set; }
    public string? BusinessName { get; set; }
    public string? ServiceTitle { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
}
