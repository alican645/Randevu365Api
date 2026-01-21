using FluentValidation;

namespace Randevu365.Application.Features.BusinessLogo.Commands.CreateBusinessLogo;

public class CreateBusinessLogoCommandValidator : AbstractValidator<CreateBusinessLogoCommandRequest>
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; 

    public CreateBusinessLogoCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.Logo)
            .NotNull().WithMessage("Logo dosyası gereklidir.")
            .Must(file => file == null || file.Length <= MaxFileSizeInBytes)
                .WithMessage("Logo dosyası en fazla 5MB olabilir.")
            .Must(file => file == null || _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                .WithMessage("Sadece JPG, JPEG, PNG, GIF ve WEBP formatları desteklenir.");
    }
}
