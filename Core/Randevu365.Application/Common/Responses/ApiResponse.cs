using System.Text.Json.Serialization;

namespace Randevu365.Application.Common.Responses;

/// <summary>
/// Tüm API endpoint'leri için standart response wrapper sınıfı.
/// Generic versiyon data dönen endpoint'ler için kullanılır.
/// </summary>
/// <typeparam name="T">Response data tipi</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// İşlemin başarılı olup olmadığını belirtir
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// HTTP Status Code
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Kullanıcıya gösterilecek mesaj
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    /// <summary>
    /// Response data objesi
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    /// <summary>
    /// Hata detayları listesi (validation hataları vb.)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Errors { get; init; }

    /// <summary>
    /// Başarılı response oluşturur
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, string? message = null, int statusCode = 200)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };

    /// <summary>
    /// Başarılı response oluşturur (201 Created)
    /// </summary>
    public static ApiResponse<T> CreatedResult(T data, string? message = null)
        => new()
        {
            Success = true,
            StatusCode = 201,
            Message = message,
            Data = data
        };

    /// <summary>
    /// Hatalı response oluşturur
    /// </summary>
    public static ApiResponse<T> FailResult(string message, int statusCode = 400)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };

    /// <summary>
    /// Çoklu hata içeren response oluşturur
    /// </summary>
    public static ApiResponse<T> FailResult(IEnumerable<string> errors, int statusCode = 400)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Errors = errors.ToList().AsReadOnly()
        };

    /// <summary>
    /// Unauthorized (401) response oluşturur
    /// </summary>
    public static ApiResponse<T> UnauthorizedResult(string message = "Yetkisiz erişim")
        => new()
        {
            Success = false,
            StatusCode = 401,
            Message = message
        };

    /// <summary>
    /// Forbidden (403) response oluşturur
    /// </summary>
    public static ApiResponse<T> ForbiddenResult(string message = "Erişim reddedildi")
        => new()
        {
            Success = false,
            StatusCode = 403,
            Message = message
        };

    /// <summary>
    /// NotFound (404) response oluşturur
    /// </summary>
    public static ApiResponse<T> NotFoundResult(string message = "Kayıt bulunamadı")
        => new()
        {
            Success = false,
            StatusCode = 404,
            Message = message
        };

    /// <summary>
    /// Conflict (409) response oluşturur
    /// </summary>
    public static ApiResponse<T> ConflictResult(string message = "Çakışma oluştu")
        => new()
        {
            Success = false,
            StatusCode = 409,
            Message = message
        };
}

/// <summary>
/// Data içermeyen response'lar için kullanılır.
/// Örn: Delete, Update işlemleri
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Başarılı response oluşturur (data olmadan)
    /// </summary>
    public static ApiResponse SuccessResult(string? message = null, int statusCode = 200)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        };

    /// <summary>
    /// NoContent (204) response oluşturur
    /// </summary>
    public static ApiResponse NoContentResult()
        => new()
        {
            Success = true,
            StatusCode = 204
        };
}
