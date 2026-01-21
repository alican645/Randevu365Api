using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.AppUser.Queries.GetAllUser;

public class GetAllAppUserQueryHandler : IRequestHandler<GetAllAppUserQueryRequest, ApiResponse<IList<GetAllAppUserQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public GetAllAppUserQueryHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }
    public async Task<ApiResponse<IList<GetAllAppUserQueryResponse>>> Handle(GetAllAppUserQueryRequest request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
             .GetAllAsync(
                 predicate: x => !x.IsDeleted,
                 include: q => q.Include(x => x.AppUserInformation));

        var response = users.Select(u => new GetAllAppUserQueryResponse
        {
            Id = u.Id,
            Name = u.AppUserInformation?.Name ?? string.Empty,
            Surname = u.AppUserInformation?.Surname ?? string.Empty,
            Email = u.Email,
            Role = u.Role
        }).ToList();
        return ApiResponse<IList<GetAllAppUserQueryResponse>>.SuccessResult(response);
    }
}