using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Appointments.Queries.GetMyAppointments;

public class GetMyAppointmentsQueryResponseItem
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string ServiceTitle { get; set; } = string.Empty;
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly? RequestedStartTime { get; set; }
    public TimeOnly? RequestedEndTime { get; set; }
    public TimeOnly? ApproveStartTime { get; set; }
    public TimeOnly? ApproveEndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? CustomerNotes { get; set; }
    public string? BusinessNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetMyAppointmentsQueryResponse
{
    public List<GetMyAppointmentsQueryResponseItem> Items { get; set; }
}
