using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Commands.DeleteBusiness;

public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommandRequest, ApiResponse<DeleteBusinessCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        var business = await _unitOfWork.GetReadRepository<Business>().GetAsync(x => x.Id == request.Id);

        if (business == null)
        {
            return ApiResponse<DeleteBusinessCommandResponse>.NotFoundResult("İşyeri bulunamadı.");
        }

        await _unitOfWork.GetWriteRepository<Business>().HardDeleteAsync(business);
        await _unitOfWork.SaveAsync();

        return ApiResponse<DeleteBusinessCommandResponse>.SuccessResult(new DeleteBusinessCommandResponse { Id = request.Id }, "İşyeri başarıyla silindi.");
    }
}
