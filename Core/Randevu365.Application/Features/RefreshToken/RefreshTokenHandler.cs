using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, ApiResponse<RefreshTokenResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<RefreshTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var readRepository = _unitOfWork.GetReadRepository<Domain.Entities.AppUser>();
        var user = await readRepository.GetAsync(
            u => u.RefreshToken == request.RefreshToken && !u.IsDeleted,
            enableTracking: true);

        if (user == null)
        {
            return ApiResponse<RefreshTokenResponse>.UnauthorizedResult("Geçersiz refresh token.");
        }

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return ApiResponse<RefreshTokenResponse>.UnauthorizedResult("Refresh token süresi dolmuş.");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = newRefreshTokenExpiry;
        await _unitOfWork.GetWriteRepository<Domain.Entities.AppUser>().UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        return ApiResponse<RefreshTokenResponse>.SuccessResult(new RefreshTokenResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiry = newRefreshTokenExpiry
        });
    }
}
