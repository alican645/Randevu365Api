using FluentValidation;

namespace Randevu365.Application.Features.Businesses.Queries.GetTopRatedBusinesses;

public class GetTopRatedBusinessesQueryValidator : AbstractValidator<GetTopRatedBusinessesQueryRequest>
{
    public GetTopRatedBusinessesQueryValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0).WithMessage("Count 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Count en fazla 100 olabilir.");
    }
}
