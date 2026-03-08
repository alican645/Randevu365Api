using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
namespace Randevu365.Application.Features.UserProfile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQueryRequest, ApiResponse<GetMyProfileQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<GetMyProfileQueryResponse>> Handle(GetMyProfileQueryRequest request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return ApiResponse<GetMyProfileQueryResponse>.UnauthorizedResult("Kullanici kimliği bulunamadi.");

        var user = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(u => u.Id == _currentUserService.UserId.Value && !u.IsDeleted,
                include: q => q.Include(u => u.AppUserInformation!));

        if (user == null)
            return ApiResponse<GetMyProfileQueryResponse>.NotFoundResult("Kullanici bulunamadi.");

        return ApiResponse<GetMyProfileQueryResponse>.SuccessResult(new GetMyProfileQueryResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role,
            Name = user.AppUserInformation?.Name,
            Surname = user.AppUserInformation?.Surname,
            PhoneNumber = user.AppUserInformation?.PhoneNumber,
            CreatedAt = user.CreatedAt
        });
    }
}
