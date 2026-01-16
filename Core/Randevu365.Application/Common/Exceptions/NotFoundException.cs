namespace Randevu365.Application.Common.Exceptions;

/// <summary>
/// Kayıt bulunamadı hatası (404 Not Found)
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException() : base("Kayıt bulunamadı.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key) 
        : base($"'{name}' entity with id '{key}' was not found.")
    {
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
