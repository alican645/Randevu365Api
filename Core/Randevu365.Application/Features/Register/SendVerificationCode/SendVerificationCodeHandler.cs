using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;

namespace Randevu365.Application.Features.Register.SendVerificationCode;

public class SendVerificationCodeHandler : IRequestHandler<SendVerificationCodeRequest, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    public SendVerificationCodeHandler(IUnitOfWork unitOfWork, IEmailService emailService, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<ApiResponse> Handle(SendVerificationCodeRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>()
            .GetAsync(x => x.Email == request.Email);

        if (existingUser != null)
            return new ApiResponse { Success = false, StatusCode = 400, Message = "Bu email adresi zaten kayıtlı." };

        var code = Random.Shared.Next(100000, 999999).ToString();
        var cacheKey = $"email_verify:{request.Email}";

        _cache.Set(cacheKey, code, TimeSpan.FromMinutes(5));

        await _emailService.SendVerificationEmailAsync(request.Email, code);

        return ApiResponse.SuccessResult("Doğrulama kodu gönderildi.");
    }
}
