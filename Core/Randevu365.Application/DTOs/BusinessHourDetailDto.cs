namespace Randevu365.Application.DTOs;


public class BusinessHourDetailDto
{
    public int Id { get; set; }
    public required string Day { get; set; }
    public required string OpenTime { get; set; }
    public required string CloseTime { get; set; }
    public bool IsDeleted { get; set; }
}