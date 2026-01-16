using FluentValidation;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessById;

public class GetBusinessByIdQueryValidator : AbstractValidator<GetBusinessByIdQueryRequest>
{
    public GetBusinessByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
