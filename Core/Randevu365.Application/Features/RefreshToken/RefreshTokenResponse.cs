namespace Randevu365.Application.Features.RefreshToken;

public class RefreshTokenResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshTokenExpiry { get; set; }
}
