using FluentValidation;

namespace Randevu365.Application.Features.BusinessLogo.Queries.GetBusinessLogoByBusinessId;

public class GetBusinessLogoByBusinessIdQueryValidator : AbstractValidator<GetBusinessLogoByBusinessIdQueryRequest>
{
    public GetBusinessLogoByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
