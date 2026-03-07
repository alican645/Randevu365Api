using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.UserProfile.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommandRequest, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return new ApiResponse { Success = false, StatusCode = 401, Message = "Kullanici kimliği bulunamadi." };

        var user = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(u => u.Id == _currentUserService.UserId.Value && !u.IsDeleted, enableTracking: true);

        if (user == null)
            return new ApiResponse { Success = false, StatusCode = 404, Message = "Kullanici bulunamadi." };

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            return new ApiResponse { Success = false, StatusCode = 400, Message = "Mevcut sifre yanlis." };

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _unitOfWork.GetWriteRepository<Domain.Entities.AppUser>().UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        return new ApiResponse { Success = true, StatusCode = 200, Message = "Sifre basariyla degistirildi." };
    }
}
