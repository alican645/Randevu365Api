namespace Randevu365.Application.DTOs;


public class BusinessPhotoDto 
{
    public int Id { get; set; }
    public string PhotoPath { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}