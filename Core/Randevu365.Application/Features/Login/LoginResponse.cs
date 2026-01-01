namespace Randevu365.Application.Features.Login;

public record LoginResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshTokenExpiry { get; set; }
    public required string Role { get; set; }
}