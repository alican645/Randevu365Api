using FluentValidation;

namespace Randevu365.Application.Features.BusinessProfile.Queries.GetNearbyBusinesses;

public class GetNearbyBusinessesQueryValidator : AbstractValidator<GetNearbyBusinessesQueryRequest>
{
    public GetNearbyBusinessesQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Enlem -90 ile 90 arasında olmalıdır.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Boylam -180 ile 180 arasında olmalıdır.");

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Yarıçap 0 ile 100 km arasında olmalıdır.");
    }
}
