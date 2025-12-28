namespace Randevu365.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(int userId, string email, string role);
    string GenerateRefreshToken();
    int? ValidateRefreshToken(string refreshToken);
}
