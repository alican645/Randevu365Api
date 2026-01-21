using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessComments.Commands.DeleteBusinessComment;

public class DeleteBusinessCommentCommandHandler : IRequestHandler<DeleteBusinessCommentCommandRequest, ApiResponse<DeleteBusinessCommentCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBusinessCommentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<DeleteBusinessCommentCommandResponse>> Handle(DeleteBusinessCommentCommandRequest request, CancellationToken cancellationToken)
    {
        
        var validator = new DeleteBusinessCommentCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<DeleteBusinessCommentCommandResponse>.FailResult(errors);
        }

        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return ApiResponse<DeleteBusinessCommentCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var comment = await _unitOfWork.GetReadRepository<BusinessComment>().GetAsync(x => x.Id == request.CommentId);

        if (comment == null)
        {
            return ApiResponse<DeleteBusinessCommentCommandResponse>.NotFoundResult("Yorum bulunamadı.");
        }

        // Sadece kendi yorumunu silebilir
        if (comment.AppUserId != userId.Value)
        {
            return ApiResponse<DeleteBusinessCommentCommandResponse>.ForbiddenResult("Bu yorumu silme yetkiniz yok.");
        }

        await _unitOfWork.GetWriteRepository<BusinessComment>().HardDeleteAsync(comment);
        await _unitOfWork.SaveAsync();

        var response = new DeleteBusinessCommentCommandResponse
        {
            Id = request.CommentId
        };

        return ApiResponse<DeleteBusinessCommentCommandResponse>.SuccessResult(response, "Yorum başarıyla silindi.");
    }
}
