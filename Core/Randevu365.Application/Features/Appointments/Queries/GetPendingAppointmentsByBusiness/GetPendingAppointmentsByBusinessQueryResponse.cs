namespace Randevu365.Application.Features.Appointments.Queries.GetPendingAppointmentsByBusiness;

public class GetPendingAppointmentsByBusinessQueryResponseItem
{
    public int AppointmentId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly? RequestedStartTime { get; set; }
    public TimeOnly? RequestedEndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CustomerNotes { get; set; }

    // Customer bilgileri
    public string? CustomerName { get; set; }
    public string? CustomerSurname { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    // Service bilgileri
    public string? ServiceTitle { get; set; }
    public string? ServiceContent { get; set; }
    public decimal ServicePrice { get; set; }

    // Business bilgileri
    public int BusinessId { get; set; }
    public string? BusinessName { get; set; }

    public DateTime CreatedAt { get; set; }
}


public class GetPendingAppointmentsByBusinessQueryResponse
{
    public IList<GetPendingAppointmentsByBusinessQueryResponseItem> Items { get; set; } = new List<GetPendingAppointmentsByBusinessQueryResponseItem>();
}