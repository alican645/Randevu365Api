using FluentValidation;

namespace Randevu365.Application.Features.BusinessLocations.Commands.UpdateBusinessLocation;

public class UpdateBusinessLocationCommandValidator : AbstractValidator<UpdateBusinessLocationCommandRequest>
{
    public UpdateBusinessLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir konum seçiniz.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Enlem -90 ile 90 arasında olmalıdır.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Boylam -180 ile 180 arasında olmalıdır.");
    }
}
