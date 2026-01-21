namespace Randevu365.Application.Common.Exceptions;


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
