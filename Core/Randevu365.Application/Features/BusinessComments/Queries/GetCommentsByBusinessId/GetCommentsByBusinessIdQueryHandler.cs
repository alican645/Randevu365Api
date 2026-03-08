using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Queries.GetCommentsByBusinessId;

public class GetCommentsByBusinessIdQueryHandler : IRequestHandler<GetCommentsByBusinessIdQueryRequest, ApiResponse<GetCommentsByBusinessIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommentsByBusinessIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetCommentsByBusinessIdQueryResponse>> Handle(GetCommentsByBusinessIdQueryRequest request, CancellationToken cancellationToken)
    {
       
        var validator = new GetCommentsByBusinessIdQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<GetCommentsByBusinessIdQueryResponse>.FailResult(errors);
        }

        var comments = await _unitOfWork.GetReadRepository<BusinessComment>().GetAllAsync(
            predicate: x => x.BusinessId == request.BusinessId,
            include: q => q.Include(c => c.AppUser).ThenInclude(u => u!.AppUserInformation)
        );

        var responseItems = comments.Select(c => new GetCommentsByBusinessIdQueryResponseItem
        {
            Id = c.Id,
            BusinessId = c.BusinessId,
            AppUserId = c.AppUserId,
            Comment = c.Comment,
            UserName = c.AppUser?.AppUserInformation?.Name,
            CreatedAt = c.CreatedAt
        }).ToList();

        var response = new GetCommentsByBusinessIdQueryResponse
        {
            Items = responseItems
        };

        return ApiResponse<GetCommentsByBusinessIdQueryResponse>.SuccessResult(response);
    }
}
