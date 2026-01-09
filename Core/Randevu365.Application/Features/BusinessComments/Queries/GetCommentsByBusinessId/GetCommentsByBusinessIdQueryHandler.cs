using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByBusinessId;

public class GetCommentsByBusinessIdQueryHandler : IRequestHandler<GetCommentsByBusinessIdQueryRequest, ApiResponse<IList<GetCommentsByBusinessIdQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommentsByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IList<GetCommentsByBusinessIdQueryResponse>>> Handle(GetCommentsByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
        var comments = await _unitOfWork.GetReadRepository<BusinessComment>().GetAllAsync(
            predicate: x => x.BusinessId == request.BusinessId,
            include: q => q.Include(c => c.AppUser).ThenInclude(u => u!.AppUserInformation)
        );

        var response = comments.Select(c => new GetCommentsByBusinessIdQueryResponse
        {
            Id = c.Id,
            BusinessId = c.BusinessId,
            AppUserId = c.AppUserId,
            Comment = c.Comment,
            UserName = c.AppUser?.AppUserInformation?.Name,
            CreatedAt = c.CreatedAt
        }).ToList();

        return ApiResponse<IList<GetCommentsByBusinessIdQueryResponse>>.SuccessResult(response);
    }
}
