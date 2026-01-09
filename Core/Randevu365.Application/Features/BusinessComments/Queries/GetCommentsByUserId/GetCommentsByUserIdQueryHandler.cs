using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByUserId;

public class GetCommentsByUserIdQueryHandler : IRequestHandler<GetCommentsByUserIdQueryRequest, ApiResponse<IList<GetCommentsByUserIdQueryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetCommentsByUserIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<IList<GetCommentsByUserIdQueryResponse>>> Handle(GetCommentsByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<IList<GetCommentsByUserIdQueryResponse>>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var comments = await _unitOfWork.GetReadRepository<BusinessComment>().GetAllAsync(
            predicate: x => x.AppUserId == userId,
            include: q => q.Include(c => c.Business)
        );

        var response = comments.Select(c => new GetCommentsByUserIdQueryResponse
        {
            Id = c.Id,
            BusinessId = c.BusinessId,
            BusinessName = c.Business?.BusinessName,
            Comment = c.Comment,
            CreatedAt = c.CreatedAt
        }).ToList();

        return ApiResponse<IList<GetCommentsByUserIdQueryResponse>>.SuccessResult(response);
    }
}
