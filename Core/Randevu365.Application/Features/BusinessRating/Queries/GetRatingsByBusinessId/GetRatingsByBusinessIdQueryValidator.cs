using FluentValidation;

namespace Randevu365.Application.Features.BusinessRating.Queries.GetRatingsByBusinessId;

public class GetRatingsByBusinessIdQueryValidator : AbstractValidator<GetRatingsByBusinessIdQueryRequest>
{
    public GetRatingsByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
