using FluentValidation;

namespace Randevu365.Application.Features.BusinessLocations.Queries.GetBusinessLocationByBusinessId;

public class GetBusinessLocationByBusinessIdQueryValidator : AbstractValidator<GetBusinessLocationByBusinessIdQueryRequest>
{
    public GetBusinessLocationByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
