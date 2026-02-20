using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetBusinessAppointments;

public class GetBusinessAppointmentsQueryResponse
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public string? ServiceTitle { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? CustomerNotes { get; set; }
}
