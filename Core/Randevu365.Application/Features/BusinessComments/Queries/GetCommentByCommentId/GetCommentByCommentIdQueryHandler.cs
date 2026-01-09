using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentByCommentId;

public class GetCommentByCommentIdQueryHandler : IRequestHandler<GetCommentByCommentIdQueryRequest, ApiResponse<GetCommentByCommentIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommentByCommentIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetCommentByCommentIdQueryResponse>> Handle(GetCommentByCommentIdQueryRequest request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.GetReadRepository<BusinessComment>().GetAsync(
            predicate: x => x.Id == request.CommentId,
            include: q => q.Include(c => c.AppUser).ThenInclude(u => u!.AppUserInformation)
        );

        if (comment == null)
        {
            return ApiResponse<GetCommentByCommentIdQueryResponse>.NotFoundResult("Yorum bulunamadı.");
        }

        var response = new GetCommentByCommentIdQueryResponse
        {
            Id = comment.Id,
            BusinessId = comment.BusinessId,
            AppUserId = comment.AppUserId,
            Comment = comment.Comment,
            UserName = comment.AppUser?.AppUserInformation?.Name,
            CreatedAt = comment.CreatedAt
        };

        return ApiResponse<GetCommentByCommentIdQueryResponse>.SuccessResult(response);
    }
}
