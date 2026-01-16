using FluentValidation;

namespace Randevu365.Application.Features.BusinessLocations.Commands.DeleteBusinessLocation;

public class DeleteBusinessLocationCommandValidator : AbstractValidator<DeleteBusinessLocationCommandRequest>
{
    public DeleteBusinessLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir konum seçiniz.");
    }
}
