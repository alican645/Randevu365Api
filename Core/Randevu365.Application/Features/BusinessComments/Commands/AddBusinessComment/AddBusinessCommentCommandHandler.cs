using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Commands.AddBusinessComment;

public class AddBusinessCommentCommandHandler : IRequestHandler<AddBusinessCommentCommandRequest, ApiResponse<AddBusinessCommentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddBusinessCommentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<AddBusinessCommentCommandResponse>> Handle(AddBusinessCommentCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<AddBusinessCommentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        if (string.IsNullOrWhiteSpace(request.Comment))
        {
            return ApiResponse<AddBusinessCommentCommandResponse>.FailResult("Yorum boş olamaz.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(x => x.Id == request.BusinessId);
        if (business == null)
        {
            return ApiResponse<AddBusinessCommentCommandResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        var comment = new BusinessComment
        {
            BusinessId = request.BusinessId,
            AppUserId = userId.Value,
            Comment = request.Comment
        };

        await _unitOfWork.GetWriteRepository<BusinessComment>().AddAsync(comment);
        await _unitOfWork.SaveAsync();

        var response = new AddBusinessCommentCommandResponse
        {
            Id = comment.Id,
            BusinessId = comment.BusinessId,
            AppUserId = comment.AppUserId,
            Comment = comment.Comment,
            CreatedAt = comment.CreatedAt
        };

        return ApiResponse<AddBusinessCommentCommandResponse>.CreatedResult(response, "Yorum başarıyla eklendi.");
    }
}
