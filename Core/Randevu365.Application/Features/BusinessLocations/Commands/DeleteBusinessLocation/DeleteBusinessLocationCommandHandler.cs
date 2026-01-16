using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLocations.Commands.DeleteBusinessLocation;

public class DeleteBusinessLocationCommandHandler : IRequestHandler<DeleteBusinessLocationCommandRequest, ApiResponse<DeleteBusinessLocationCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessLocationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DeleteBusinessLocationCommandResponse>> Handle(DeleteBusinessLocationCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new DeleteBusinessLocationCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<DeleteBusinessLocationCommandResponse>.FailResult(errors);
        }

        var location = await _unitOfWork.GetReadRepository<BusinessLocation>().GetAsync(x => x.Id == request.Id);

        if (location == null)
        {
            return ApiResponse<DeleteBusinessLocationCommandResponse>.NotFoundResult("İşyeri konumu bulunamadı.");
        }

        await _unitOfWork.GetWriteRepository<BusinessLocation>().HardDeleteAsync(location);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessLocationCommandResponse>.SuccessResult(new DeleteBusinessLocationCommandResponse { Id = request.Id }, "İşyeri konumu başarıyla silindi.");
    }
}
