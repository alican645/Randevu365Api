using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.BusinessLocations.Commands.CreateBusinessLocation;

public class CreateBusinessLocationCommandHandler : IRequestHandler<CreateBusinessLocationCommandRequest, ApiResponse<CreateBusinessLocationCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessLocationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CreateBusinessLocationCommandResponse>> Handle(CreateBusinessLocationCommandRequest request, CancellationToken cancellationToken)
    {
        var location = new BusinessLocation(request.BusinessId, request.Latitude, request.Longitude);

        await _unitOfWork.GetWriteRepository<BusinessLocation>().AddAsync(location);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessLocationCommandResponse>.SuccessResult(new CreateBusinessLocationCommandResponse { Id = location.Id }, "Business location added successfully.");
    }
}
