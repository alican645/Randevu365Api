using FluentValidation;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.DeleteBusinessPhoto;

public class DeleteBusinessPhotoCommandValidator : AbstractValidator<DeleteBusinessPhotoCommandRequest>
{
    public DeleteBusinessPhotoCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.BusinessPhotoId)
            .GreaterThan(0).WithMessage("Geçerli bir fotoğraf seçiniz.");
    }
}
