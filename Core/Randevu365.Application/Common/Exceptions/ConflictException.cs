namespace Randevu365.Application.Common.Exceptions;

/// <summary>
/// Çakışma hatası (409 Conflict)
/// Örn: Aynı email ile kayıtlı kullanıcı var
/// </summary>
public class ConflictException : Exception
{
    public ConflictException() : base("Kaynak zaten mevcut.")
    {
    }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string name, object key)
        : base($"'{name}' entity with identifier '{key}' already exists.")
    {
    }
}
