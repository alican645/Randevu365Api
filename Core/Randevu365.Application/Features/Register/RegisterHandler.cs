using MediatR;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.Register;

public class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RegisterHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {

        var existingUser = await _unitOfWork.GetReadRepository<AppUser>().GetAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = "User already exists"
            };
        }
        var appUser = new AppUser(request.Email, request.Password, (Roles)request.Role);

        await _unitOfWork.GetWriteRepository<AppUser>().AddAsync(appUser);
        await _unitOfWork.SaveAsync();

        return new RegisterResponse
        {
            Success = true,
            Message = "User registered successfully"
        };
    }
}
