using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryResponse
{
    public int Id { get; set; }
    public string? BusinessName { get; set; }
    public string? ServiceTitle { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? CustomerNotes { get; set; }
    public string? BusinessNotes { get; set; }
    public int AppUserId { get; set; }
    public int BusinessId { get; set; }
}
