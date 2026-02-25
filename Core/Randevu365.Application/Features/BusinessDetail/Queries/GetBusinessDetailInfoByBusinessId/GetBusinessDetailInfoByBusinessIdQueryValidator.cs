using FluentValidation;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessDetailInfoByBusinessId;

public class GetBusinessDetailInfoByBusinessIdQueryValidator : AbstractValidator<GetBusinessDetailInfoByBusinessIdQueryRequest>
{
    public GetBusinessDetailInfoByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId).GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
