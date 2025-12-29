namespace Randevu365.Application.Features.Register;

/// <summary>
/// Register işlemi sonucunda dönen data
/// </summary>
public record RegisterResponse
{
    /// <summary>
    /// Oluşturulan kullanıcının ID'si
    /// </summary>
    public int UserId { get; init; }
}