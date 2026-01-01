using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Commands.CreateBusiness;

public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommandRequest, ApiResponse<CreateBusinessCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CreateBusinessCommandResponse>> Handle(CreateBusinessCommandRequest request, CancellationToken cancellationToken)
    {
        var business = new Business
        {
            BusinessName = request.BusinessName,
            BusinessAddress = request.BusinessAddress,
            BusinessCity = request.BusinessCity,
            BusinessPhone = request.BusinessPhone,
            BusinessEmail = request.BusinessEmail,
            BusinessCountry = request.BusinessCountry,
            AppUserId = request.AppUserId
        };

        await _unitOfWork.GetWriteRepository<Business>().AddAsync(business);
        await _unitOfWork.SaveAsync();

        return ApiResponse<CreateBusinessCommandResponse>.SuccessResult(new CreateBusinessCommandResponse { Id = business.Id }, "Business added successfully.");
    }
}
