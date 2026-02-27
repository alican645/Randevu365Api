using FluentValidation;

namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinessesPaginated;

public class GetAllBusinessesPaginatedValidator : AbstractValidator<GetAllBusinessesPaginatedRequest>
{
    public GetAllBusinessesPaginatedValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.");
    }
}
