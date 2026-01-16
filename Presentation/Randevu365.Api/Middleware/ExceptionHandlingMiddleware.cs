using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Randevu365.Application.Common.Exceptions;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => HandleValidationException(validationEx),
            NotFoundException notFoundEx => HandleNotFoundException(notFoundEx),
            ForbiddenAccessException forbiddenEx => HandleForbiddenException(forbiddenEx),
            ConflictException conflictEx => HandleConflictException(conflictEx),
            BadRequestException badRequestEx => HandleBadRequestException(badRequestEx),
            UnauthorizedAccessException unauthorizedEx => HandleUnauthorizedException(unauthorizedEx),
            _ => HandleUnknownException(exception)
        };

        // Loglama
        LogException(exception, statusCode);

        // Response yazma
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }

    private (int StatusCode, object Response) HandleValidationException(ValidationException ex)
    {
        var response = new
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = ex.Message,
            Errors = ex.Errors
        };
        return ((int)HttpStatusCode.BadRequest, response);
    }

    private (int StatusCode, object Response) HandleNotFoundException(NotFoundException ex)
    {
        var response = ApiResponse<object>.NotFoundResult(ex.Message);
        return ((int)HttpStatusCode.NotFound, response);
    }

    private (int StatusCode, object Response) HandleForbiddenException(ForbiddenAccessException ex)
    {
        var response = ApiResponse<object>.ForbiddenResult(ex.Message);
        return ((int)HttpStatusCode.Forbidden, response);
    }

    private (int StatusCode, object Response) HandleConflictException(ConflictException ex)
    {
        var response = ApiResponse<object>.ConflictResult(ex.Message);
        return ((int)HttpStatusCode.Conflict, response);
    }

    private (int StatusCode, object Response) HandleBadRequestException(BadRequestException ex)
    {
        var response = ApiResponse<object>.FailResult(ex.Message, (int)HttpStatusCode.BadRequest);
        return ((int)HttpStatusCode.BadRequest, response);
    }

    private (int StatusCode, object Response) HandleUnauthorizedException(UnauthorizedAccessException ex)
    {
        var response = ApiResponse<object>.UnauthorizedResult(ex.Message);
        return ((int)HttpStatusCode.Unauthorized, response);
    }

    private (int StatusCode, object Response) HandleUnknownException(Exception ex)
    {
        // Production'da hata detayını gizle
        var message = _environment.IsDevelopment()
            ? ex.Message
            : "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.";

        var response = new
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = message,
            // Development'ta stack trace göster
            Detail = _environment.IsDevelopment() ? ex.StackTrace : null
        };

        return ((int)HttpStatusCode.InternalServerError, response);
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= 500)
        {
            _logger.LogError(exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                Activity.Current?.Id ?? "N/A");
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning(
                "Client error occurred: {ExceptionType} - {Message}",
                exception.GetType().Name,
                exception.Message);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
