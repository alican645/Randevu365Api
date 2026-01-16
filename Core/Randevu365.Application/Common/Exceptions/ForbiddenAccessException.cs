namespace Randevu365.Application.Common.Exceptions;

/// <summary>
/// Yetkisiz erişim hatası (403 Forbidden)
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Bu işlemi gerçekleştirmek için yetkiniz yok.")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}
