namespace Randevu365.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQueryResponse
{
    public int TotalUsers { get; set; }
    public int TotalBusinesses { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int TotalComments { get; set; }
}
