using FluentValidation;

namespace Randevu365.Application.Features.BusinessRating.Commands.UpdateBusinessRating;

public class UpdateBusinessRatingCommandValidator : AbstractValidator<UpdateBusinessRatingCommandRequest>
{
    public UpdateBusinessRatingCommandValidator()
    {
        RuleFor(x => x.RatingId)
            .GreaterThan(0).WithMessage("Geçerli bir değerlendirme seçiniz.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");
    }
}
