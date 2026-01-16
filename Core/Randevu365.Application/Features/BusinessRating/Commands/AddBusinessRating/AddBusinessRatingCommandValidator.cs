using FluentValidation;

namespace Randevu365.Application.Features.BusinessRating.Commands.AddBusinessRating;

public class AddBusinessRatingCommandValidator : AbstractValidator<AddBusinessRatingCommandRequest>
{
    public AddBusinessRatingCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");
    }
}
