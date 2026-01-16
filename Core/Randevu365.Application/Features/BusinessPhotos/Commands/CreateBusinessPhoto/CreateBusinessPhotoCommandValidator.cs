using FluentValidation;

namespace Randevu365.Application.Features.BusinessPhotos.Commands.CreateBusinessPhoto;

public class CreateBusinessPhotoCommandValidator : AbstractValidator<CreateBusinessPhotoCommandRequest>
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MB

    public CreateBusinessPhotoCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .GreaterThan(0).WithMessage("Geçerli bir işletme seçiniz.");

        RuleFor(x => x.Photo)
            .NotNull().WithMessage("Fotoğraf dosyası gereklidir.")
            .Must(file => file == null || file.Length <= MaxFileSizeInBytes)
                .WithMessage("Fotoğraf dosyası en fazla 10MB olabilir.")
            .Must(file => file == null || _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                .WithMessage("Sadece JPG, JPEG, PNG, GIF ve WEBP formatları desteklenir.");
    }
}
