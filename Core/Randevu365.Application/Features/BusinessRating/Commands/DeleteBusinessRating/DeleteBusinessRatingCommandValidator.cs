using FluentValidation;

namespace Randevu365.Application.Features.BusinessRating.Commands.DeleteBusinessRating;

public class DeleteBusinessRatingCommandValidator : AbstractValidator<DeleteBusinessRatingCommandRequest>
{
    public DeleteBusinessRatingCommandValidator()
    {
        RuleFor(x => x.RatingId)
            .GreaterThan(0).WithMessage("Geçerli bir değerlendirme seçiniz.");
    }
}
