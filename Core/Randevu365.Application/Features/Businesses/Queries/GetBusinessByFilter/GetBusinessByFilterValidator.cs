using FluentValidation;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessByFilter;

public class GetBusinessByFilterValidator : AbstractValidator<GetBusinessByFilterRequest>
{
    public GetBusinessByFilterValidator()
    {
        RuleFor(x => x.Category)
            .Must(v => BusinessCategoryExtensions.TryFromJson(v!, out _))
            .When(x => !string.IsNullOrEmpty(x.Category))
            .WithMessage($"Geçersiz kategori. Geçerli değerler: {string.Join(", ", BusinessCategoryExtensions.ValidValues)}");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.");
    }
}
