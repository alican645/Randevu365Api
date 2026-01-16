using FluentValidation;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileById;

public class GetBusinessProfileByIdQueryValidator : AbstractValidator<GetBusinessProfileByIdQueryRequest>
{
    public GetBusinessProfileByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir profil ID'si giriniz.");
    }
}
