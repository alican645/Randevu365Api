using System.Security.Cryptography;
using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ApiResponse> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(u => u.Email == request.Email && !u.IsDeleted, enableTracking: true);

        // Always return success to prevent email enumeration
        if (user == null)
            return new ApiResponse { Success = true, StatusCode = 200, Message = "Eger bu email adresi kayitliysa, sifre sifirlama linki gonderildi." };

        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        user.RefreshToken = resetToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(15);

        await _unitOfWork.GetWriteRepository<Domain.Entities.AppUser>().UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return new ApiResponse { Success = true, StatusCode = 200, Message = "Eger bu email adresi kayitliysa, sifre sifirlama linki gonderildi." };
    }
}
