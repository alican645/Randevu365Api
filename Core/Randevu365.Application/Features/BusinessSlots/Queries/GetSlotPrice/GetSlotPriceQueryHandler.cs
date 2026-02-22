using MediatR;
using Microsoft.Extensions.Configuration;
using Randevu365.Application.Common.Responses;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.BusinessSlots.Queries.GetSlotPrice;

public class GetSlotPriceQueryHandler : IRequestHandler<GetSlotPriceQueryRequest, ApiResponse<GetSlotPriceQueryResponse>>
{
    private readonly IConfiguration _configuration;

    public GetSlotPriceQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ApiResponse<GetSlotPriceQueryResponse>> Handle(GetSlotPriceQueryRequest request, CancellationToken cancellationToken)
    {
        var packageTypes = new[] { SlotPackageType.Single, SlotPackageType.Triple, SlotPackageType.Bundle5 };

        var packages = packageTypes.Select(pt =>
        {
            var priceStr = _configuration[$"BusinessSlot:Packages:{pt}"];
            var totalPrice = decimal.TryParse(priceStr, out var parsed) ? parsed : 0m;
            var quantity = (int)pt;
            return new SlotPackageDto
            {
                PackageType = pt,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PricePerSlot = quantity > 0 ? totalPrice / quantity : totalPrice
            };
        }).ToList();

        return Task.FromResult(ApiResponse<GetSlotPriceQueryResponse>.SuccessResult(
            new GetSlotPriceQueryResponse { Packages = packages }));
    }
}
