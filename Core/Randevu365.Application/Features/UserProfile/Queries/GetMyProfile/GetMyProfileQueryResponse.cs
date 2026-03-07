namespace Randevu365.Application.Features.UserProfile.Queries.GetMyProfile;

public class GetMyProfileQueryResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public int? Height { get; set; }
    public int? Weight { get; set; }
    public DateTime CreatedAt { get; set; }
}
