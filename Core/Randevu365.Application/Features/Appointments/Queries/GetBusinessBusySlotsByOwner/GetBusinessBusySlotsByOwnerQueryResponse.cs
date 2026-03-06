namespace Randevu365.Application.Features.Appointments.Queries.GetBusinessBusySlotsByOwner;

public class GetBusinessBusySlotsByOwnerQueryResponseItem
{
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly? ApproveStartTime { get; set; }
    public TimeOnly? ApproveEndTime { get; set; }
}

public class GetBusinessBusySlotsByOwnerQueryResponse
{
    public List<GetBusinessBusySlotsByOwnerQueryResponseItem> Items { get; set; } = new();
}
