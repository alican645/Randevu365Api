using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Commands.DeleteBusiness;

public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommandRequest, ApiResponse<DeleteBusinessCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBusinessCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<DeleteBusinessCommandResponse>> Handle(DeleteBusinessCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new DeleteBusinessCommandValidator();
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<DeleteBusinessCommandResponse>.FailResult(errors);
        }

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
        {
            return ApiResponse<DeleteBusinessCommandResponse>.UnauthorizedResult("Kullanıcı kimliği bulunamadı.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(x => x.Id == request.Id);

        if (business == null)
        {
            return ApiResponse<DeleteBusinessCommandResponse>.NotFoundResult("İşyeri bulunamadı.");
        }

        if (business.AppUserId != currentUserId)
        {
            return ApiResponse<DeleteBusinessCommandResponse>.ForbiddenResult("Bu işyerini silme yetkiniz yok.");
        }

        await _unitOfWork.GetWriteRepository<Business>().HardDeleteAsync(business);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessCommandResponse>.SuccessResult(
            new DeleteBusinessCommandResponse { Id = request.Id },
            "İşyeri ve ilişkili tüm veriler başarıyla silindi.");
    }
}
