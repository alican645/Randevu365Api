using BCrypt;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Register;

public abstract class RegisterHandlerBase<TRequest> : IRequestHandler<TRequest, ApiResponse<RegisterResponse>>
    where TRequest : RegisterRequest
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    protected abstract string Role { get; }

    protected RegisterHandlerBase(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ApiResponse<RegisterResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>().GetAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return ApiResponse<RegisterResponse>.FailResult("Bu email adresi zaten kayıtlı.");
        }

        var cacheKey = $"email_verify:{request.Email}";
        if (!_cache.TryGetValue(cacheKey, out string? cachedCode) || cachedCode != request.VerificationCode)
        {
            return ApiResponse<RegisterResponse>.FailResult("Geçersiz veya süresi dolmuş doğrulama kodu.");
        }

        _cache.Remove(cacheKey);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var userInformation = new AppUserInformation
            {
                Name = request.Name,
                Surname = request.Surname,
                PhoneNumber = request.PhoneNumber,
            };

            await _unitOfWork.GetWriteRepository<AppUserInformation>().AddAsync(userInformation);
            await _unitOfWork.SaveAsync();

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var appUser = new Domain.Entities.AppUser(request.Email, hashedPassword, Role)
            {
                AppUserInformationId = userInformation.Id,
                AppUserInformation = userInformation
            };

            await _unitOfWork.GetWriteRepository<Domain.Entities.AppUser>().AddAsync(appUser);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var registerResponse = new RegisterResponse
            {
                UserId = appUser.Id
            };

            return ApiResponse<RegisterResponse>.CreatedResult(registerResponse, "Kullanıcı başarıyla oluşturuldu.");
        }
        catch(Exception ex)
        {
            
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResponse<RegisterResponse>.FailResult(ex.Message);
        }
    }
}

public class RegisterCustomerHandler : RegisterHandlerBase<RegisterCustomerRequest>
{
    protected override string Role => Roles.Customer;

    public RegisterCustomerHandler(IUnitOfWork unitOfWork, IMemoryCache cache) : base(unitOfWork, cache) { }
}

public class RegisterBusinessOwnerHandler : RegisterHandlerBase<RegisterBusinessOwnerRequest>
{
    protected override string Role => Roles.BusinessOwner;

    public RegisterBusinessOwnerHandler(IUnitOfWork unitOfWork, IMemoryCache cache) : base(unitOfWork, cache) { }
}
