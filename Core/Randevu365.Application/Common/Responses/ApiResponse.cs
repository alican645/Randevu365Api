using System.Text.Json.Serialization;

namespace Randevu365.Application.Common.Responses;

/// Tüm API endpoint'leri için standart response wrapper sınıfı.
/// Generic versiyon data dönen endpoint'ler için kullanılır.
public class ApiResponse<T>
{

    public bool Success { get; init; }
    
    public int StatusCode { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Errors { get; init; }
    
    public static ApiResponse<T> SuccessResult(T data, string? message = null, int statusCode = 200)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    
    public static ApiResponse<T> CreatedResult(T data, string? message = null)
        => new()
        {
            Success = true,
            StatusCode = 201,
            Message = message,
            Data = data
        };
    
    public static ApiResponse<T> FailResult(string message, int statusCode = 400)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };
    
    public static ApiResponse<T> FailResult(IEnumerable<string> errors, int statusCode = 400)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Errors = errors.ToList().AsReadOnly()
        };
    
    public static ApiResponse<T> UnauthorizedResult(string message = "Yetkisiz erişim")
        => new()
        {
            Success = false,
            StatusCode = 401,
            Message = message
        };
    
    public static ApiResponse<T> ForbiddenResult(string message = "Erişim reddedildi")
        => new()
        {
            Success = false,
            StatusCode = 403,
            Message = message
        };
    
    public static ApiResponse<T> NotFoundResult(string message = "Kayıt bulunamadı")
        => new()
        {
            Success = false,
            StatusCode = 404,
            Message = message
        };
    
    public static ApiResponse<T> ConflictResult(string message = "Çakışma oluştu")
        => new()
        {
            Success = false,
            StatusCode = 409,
            Message = message
        };

    public static ApiResponse<T> PaymentRequiredResult(string message = "Ödeme gerekli.")
        => new()
        {
            Success = false,
            StatusCode = 402,
            Message = message
        };
}

/// Data içermeyen response'lar için kullanılır.
/// Örn: Delete, Update işlemleri
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResult(string? message = null, int statusCode = 200)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        };
    
    public static ApiResponse NoContentResult()
        => new()
        {
            Success = true,
            StatusCode = 204
        };
}
