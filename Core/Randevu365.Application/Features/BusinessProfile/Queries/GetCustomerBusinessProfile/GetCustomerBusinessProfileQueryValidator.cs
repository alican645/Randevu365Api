using FluentValidation;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetCustomerBusinessProfile;

public class GetCustomerBusinessProfileQueryValidator : AbstractValidator<GetCustomerBusinessProfileQueryRequest>
{
    public GetCustomerBusinessProfileQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0)
            .WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
