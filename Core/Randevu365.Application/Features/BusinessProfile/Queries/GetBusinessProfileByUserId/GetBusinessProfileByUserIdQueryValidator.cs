using FluentValidation;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetBusinessProfileByUserId;

public class GetBusinessProfileByUserIdQueryValidator : AbstractValidator<GetBusinessProfileByUserIdQueryRequest>
{
    public GetBusinessProfileByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID'si giriniz.");
    }
}
