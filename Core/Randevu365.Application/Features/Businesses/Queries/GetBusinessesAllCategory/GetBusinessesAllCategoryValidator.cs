using FluentValidation;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesAllCategory;

public class GetBusinessesAllCategoryValidator : AbstractValidator<GetBusinessesAllCategoryRequest>
{
    public GetBusinessesAllCategoryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.");
    }
}
