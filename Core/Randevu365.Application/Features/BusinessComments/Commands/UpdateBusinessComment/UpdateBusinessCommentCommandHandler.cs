using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Commands.UpdateBusinessComment;

public class UpdateBusinessCommentCommandHandler : IRequestHandler<UpdateBusinessCommentCommandRequest, ApiResponse<UpdateBusinessCommentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBusinessCommentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<UpdateBusinessCommentCommandResponse>> Handle(UpdateBusinessCommentCommandRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<UpdateBusinessCommentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        if (string.IsNullOrWhiteSpace(request.Comment))
        {
            return ApiResponse<UpdateBusinessCommentCommandResponse>.FailResult("Yorum boş olamaz.");
        }

        var comment = await _unitOfWork.GetReadRepository<BusinessComment>().GetAsync(x => x.Id == request.CommentId);

        if (comment == null)
        {
            return ApiResponse<UpdateBusinessCommentCommandResponse>.NotFoundResult("Yorum bulunamadı.");
        }

        // Sadece kendi yorumunu güncelleyebilir
        if (comment.AppUserId != userId.Value)
        {
            return ApiResponse<UpdateBusinessCommentCommandResponse>.ForbiddenResult("Bu yorumu güncelleme yetkiniz yok.");
        }

        comment.Comment = request.Comment;

        await _unitOfWork.GetWriteRepository<BusinessComment>().UpdateAsync(comment);
        await _unitOfWork.SaveAsync();

        var response = new UpdateBusinessCommentCommandResponse
        {
            Id = comment.Id,
            BusinessId = comment.BusinessId,
            AppUserId = comment.AppUserId,
            Comment = comment.Comment
        };

        return ApiResponse<UpdateBusinessCommentCommandResponse>.SuccessResult(response, "Yorum başarıyla güncellendi.");
    }
}
