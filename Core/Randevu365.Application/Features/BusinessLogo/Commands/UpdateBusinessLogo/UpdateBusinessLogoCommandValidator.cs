using FluentValidation;

namespace Randevu365.Application.Features.BusinessLogo.Commands.UpdateBusinessLogo;

public class UpdateBusinessLogoCommandValidator : AbstractValidator<UpdateBusinessLogoCommandRequest>
{
    public UpdateBusinessLogoCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.LogoUrl)
            .NotEmpty().WithMessage("Logo URL boş olamaz.")
            .Must(BeValidUrl).WithMessage("Geçerli bir URL giriniz.");
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
