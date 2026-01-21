namespace Randevu365.Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base("Geçersiz istek.")
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
