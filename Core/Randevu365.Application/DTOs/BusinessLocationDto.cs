namespace Randevu365.Application.DTOs;


public class BusinessLocationDto
{
    public int Id { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDeleted { get; set; }
}