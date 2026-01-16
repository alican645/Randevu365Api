using FluentValidation;

namespace Randevu365.Application.Features.BusinessHours.Queries.GetBusinessHoursByBusinessId;

public class GetBusinessHoursByBusinessIdQueryValidator : AbstractValidator<GetBusinessHoursByBusinessIdQueryRequest>
{
    public GetBusinessHoursByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
