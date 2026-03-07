using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(u => u.Email == request.Email && !u.IsDeleted, enableTracking: true);

        if (user == null)
            return new ApiResponse { Success = false, StatusCode = 400, Message = "Gecersiz sifre sifirlama istegi." };

        if (user.RefreshToken != request.ResetToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            return new ApiResponse { Success = false, StatusCode = 400, Message = "Gecersiz veya suresi dolmus sifirlama kodu." };

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        await _unitOfWork.GetWriteRepository<Domain.Entities.AppUser>().UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        return new ApiResponse { Success = true, StatusCode = 200, Message = "Sifreniz basariyla guncellendi." };
    }
}
