namespace Randevu365.Application.DTOs;



public class BusinessServiceDetailDto
{
    public int? Id { get; set; }
    public required decimal ServicePrice { get; set; }
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; }
    public bool IsDeleted { get; set; }
}