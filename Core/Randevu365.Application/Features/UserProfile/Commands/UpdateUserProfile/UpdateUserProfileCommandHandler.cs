using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
namespace Randevu365.Application.Features.UserProfile.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommandRequest, ApiResponse<UpdateUserProfileCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<UpdateUserProfileCommandResponse>> Handle(UpdateUserProfileCommandRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<UpdateUserProfileCommandResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var user = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(u => u.Id == _currentUserService.UserId.Value && !u.IsDeleted,
                include: q => q.Include(u => u.AppUserInformation!),
                enableTracking: true);

        if (user == null)
            return ApiResponse<UpdateUserProfileCommandResponse>.NotFoundResult("Kullanici bulunamadi.");

        if (user.AppUserInformation == null)
            return ApiResponse<UpdateUserProfileCommandResponse>.NotFoundResult("Kullanici bilgileri bulunamadi.");

        if (request.Name != null) user.AppUserInformation.Name = request.Name;
        if (request.Surname != null) user.AppUserInformation.Surname = request.Surname;
        if (request.PhoneNumber != null) user.AppUserInformation.PhoneNumber = request.PhoneNumber;

        await _unitOfWork.GetWriteRepository<Domain.Entities.AppUserInformation>().UpdateAsync(user.AppUserInformation);
        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateUserProfileCommandResponse>.SuccessResult(
            new UpdateUserProfileCommandResponse { UserId = user.Id },
            "Profil basariyla guncellendi.");
    }
}
