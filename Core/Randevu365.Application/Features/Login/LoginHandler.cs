using BCrypt.Net;
using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Login;

public class LoginHandler : IRequestHandler<LoginRequest, ApiResponse<LoginResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public LoginHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        // Get user by email
        var readRepository = _unitOfWork.GetReadRepository<Domain.Entities.AppUser>();
        var user = await readRepository.GetAsync(u => u.Email == request.Email && !u.IsDeleted);

        // Validate user exists
        if (user == null)
        {
            return ApiResponse<LoginResponse>.UnauthorizedResult("Geçersiz email veya şifre.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return ApiResponse<LoginResponse>.UnauthorizedResult("Geçersiz email veya şifre.");
        }

        var role = user.Role.ToString();
        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Set refresh token expiry (7 days)
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // TODO: Store refresh token in database for validation

        var loginResponse = new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry,
            Role = role

        };

        return ApiResponse<LoginResponse>.SuccessResult(loginResponse, "Giriş başarılı.");
    }
}