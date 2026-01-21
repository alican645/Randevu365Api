using BCrypt;
using MediatR;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Register;

public class RegisterHandler : IRequestHandler<RegisterRequest, ApiResponse<RegisterResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.GetReadRepository<Domain.Entities.AppUser>().GetAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return ApiResponse<RegisterResponse>.FailResult("Bu email adresi zaten kayıtlı.");
        }

        var userInformation = new AppUserInformation
        {
            Name = request.Name,
            Surname = request.Surname,
            Age = request.Age,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            Height = request.Height,
            Weight = request.Weight
        };

        await _unitOfWork.GetWriteRepository<AppUserInformation>().AddAsync(userInformation);
        await _unitOfWork.SaveAsync();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var appUser = new Domain.Entities.AppUser(request.Email, hashedPassword, request.Role)
        {
            AppUserInformationId = userInformation.Id,
            AppUserInformation = userInformation
        };

        await _unitOfWork.GetWriteRepository<Domain.Entities.AppUser>().AddAsync(appUser);
        await _unitOfWork.SaveAsync();

        var registerResponse = new RegisterResponse
        {
            UserId = appUser.Id
        };

        return ApiResponse<RegisterResponse>.CreatedResult(registerResponse, "Kullanıcı başarıyla oluşturuldu.");
    }
}
