using FluentValidation;

namespace Randevu365.Application.Features.BusinessPhotos.Queries.GetBusinessPhotosByBusinessId;

public class GetBusinessPhotosByBusinessIdQueryValidator : AbstractValidator<GetBusinessPhotosByBusinessIdQueryRequest>
{
    public GetBusinessPhotosByBusinessIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme ID'si giriniz.");
    }
}
