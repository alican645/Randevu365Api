using FluentValidation;

namespace Randevu365.Application.Features.BusinessSlots.Commands.VerifyInAppPurchase;

public class VerifyInAppPurchaseCommandValidator : AbstractValidator<VerifyInAppPurchaseCommandRequest>
{
    public VerifyInAppPurchaseCommandValidator()
    {
        RuleFor(x => x.Platform)
            .NotEmpty().WithMessage("Platform bilgisi gereklidir.")
            .Must(p => p.ToLowerInvariant() is "apple" or "google")
            .WithMessage("Platform 'apple' veya 'google' olmalıdır.");

        RuleFor(x => x.ReceiptData)
            .NotEmpty().WithMessage("Receipt verisi gereklidir.");

        RuleFor(x => x.PackageType)
            .IsInEnum().WithMessage("Geçerli bir paket türü seçiniz.");

        RuleFor(x => x.PackageName)
            .NotEmpty().WithMessage("Google satın alımları için PackageName gereklidir.")
            .When(x => x.Platform.Equals("google", StringComparison.OrdinalIgnoreCase));

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Google satın alımları için ProductId gereklidir.")
            .When(x => x.Platform.Equals("google", StringComparison.OrdinalIgnoreCase));
    }
}
