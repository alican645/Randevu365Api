using FluentValidation;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Businesses.Queries.GetBusinessesByBusinessCategory;

public class GetBusinessesByBusinessCategoryValidator : AbstractValidator<GetBusinessesByBusinessCategoryRequest>
{
    public GetBusinessesByBusinessCategoryValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .Must(v => BusinessCategoryExtensions.TryFromJson(v, out _))
            .WithMessage($"Geçersiz kategori. Geçerli değerler: {string.Join(", ", BusinessCategoryExtensions.ValidValues)}");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.");
    }
}
