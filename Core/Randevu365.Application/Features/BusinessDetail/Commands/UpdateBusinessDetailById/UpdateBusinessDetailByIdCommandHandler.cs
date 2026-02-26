using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;
using BusinessLogoEntity = Randevu365.Domain.Entities.BusinessLogo;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetailById;

public class UpdateBusinessDetailByIdCommandHandler : IRequestHandler<UpdateBusinessDetailByIdCommandRequest, ApiResponse<UpdateBusinessDetailByIdCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;

    public UpdateBusinessDetailByIdCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileService = fileService;
    }

    public async Task<ApiResponse<UpdateBusinessDetailByIdCommandResponse>> Handle(UpdateBusinessDetailByIdCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateBusinessDetailByIdCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<UpdateBusinessDetailByIdCommandResponse>.FailResult(errors);
        }

        if (_currentUserService.UserId is null)
        {
            return ApiResponse<UpdateBusinessDetailByIdCommandResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(
                predicate: x => x.Id == request.BusinessId && x.AppUserId == _currentUserService.UserId.Value && !x.IsDeleted,
                include: i => i
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessHours)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.BusinessServices)
                        .ThenInclude(s => s.Appointments)
                    .Include(b => b.BusinessLocations),
                enableTracking: true
            );

        if (business == null)
        {
            return ApiResponse<UpdateBusinessDetailByIdCommandResponse>.NotFoundResult("İşletme bulunamadı.");
        }

        business.BusinessName = request.BusinessName!;
        business.BusinessAddress = request.BusinessAddress!;
        business.BusinessCity = request.BusinessCity!;
        business.BusinessPhone = request.BusinessPhone!;
        business.BusinessEmail = request.BusinessEmail!;
        business.BusinessCountry = request.BusinessCountry!;

        if (request.BusinessCategory != null && BusinessCategoryExtensions.TryFromJson(request.BusinessCategory, out var category))
        {
            business.BusinessCategory = category;
        }

        await _unitOfWork.GetWriteRepository<Business>().UpdateAsync(business);

        // Business Logo
        if (request.BusinessLogo != null)
        {
            var logoUrl = await _fileService.UploadFileAsync(request.BusinessLogo, $"business/{business.Id}/logo");
            if (business.BusinessLogo != null)
            {
                business.BusinessLogo.LogoUrl = logoUrl;
                await _unitOfWork.GetWriteRepository<BusinessLogoEntity>().UpdateAsync(business.BusinessLogo);
            }
            else
            {
                var logo = new BusinessLogoEntity
                {
                    LogoUrl = logoUrl,
                    BusinessId = business.Id
                };
                await _unitOfWork.GetWriteRepository<BusinessLogoEntity>().AddAsync(logo);
            }
        }

        // Business Hours — Id=0 → ekle, IsDeleted=true → sil, IsDeleted=false → güncelle
        if (request.BusinessHours != null)
        {
            foreach (var hourDto in request.BusinessHours)
            {
                if (hourDto.Id == 0)
                {
                    await _unitOfWork.GetWriteRepository<BusinessHour>().AddAsync(new BusinessHour
                    {
                        Day = hourDto.Day,
                        OpenTime = hourDto.OpenTime,
                        CloseTime = hourDto.CloseTime,
                        BusinessId = business.Id
                    });
                }
                else
                {
                    var existing = business.BusinessHours.FirstOrDefault(h => h.Id == hourDto.Id);
                    if (existing == null) continue;

                    if (hourDto.IsDeleted)
                    {
                        await _unitOfWork.GetWriteRepository<BusinessHour>().HardDeleteAsync(existing);
                    }
                    else
                    {
                        existing.Day = hourDto.Day;
                        existing.OpenTime = hourDto.OpenTime;
                        existing.CloseTime = hourDto.CloseTime;
                        await _unitOfWork.GetWriteRepository<BusinessHour>().UpdateAsync(existing);
                    }
                }
            }
        }

        // Business Services — Id=null → ekle, IsDeleted=true → sil (randevusu varsa SoftDelete), IsDeleted=false → güncelle
        if (request.BusinessServices != null)
        {
            foreach (var serviceDto in request.BusinessServices)
            {
                if (!serviceDto.Id.HasValue)
                {
                    await _unitOfWork.GetWriteRepository<BusinessService>().AddAsync(new BusinessService
                    {
                        ServiceTitle = serviceDto.ServiceTitle,
                        ServiceContent = serviceDto.ServiceContent,
                        MaxConcurrentCustomers = serviceDto.MaxConcurrentCustomers,
                        ServicePrice = serviceDto.ServicePrice,
                        BusinessId = business.Id
                    });
                }
                else
                {
                    var existing = business.BusinessServices.FirstOrDefault(s => s.Id == serviceDto.Id);
                    if (existing == null) continue;

                    if (serviceDto.IsDeleted)
                    {
                        if (existing.Appointments.Count > 0)
                        {
                            existing.IsDeleted = true;
                            await _unitOfWork.GetWriteRepository<BusinessService>().UpdateAsync(existing);
                        }
                        else
                        {
                            await _unitOfWork.GetWriteRepository<BusinessService>().HardDeleteAsync(existing);
                        }
                    }
                    else
                    {
                        existing.ServiceTitle = serviceDto.ServiceTitle;
                        existing.ServiceContent = serviceDto.ServiceContent;
                        existing.MaxConcurrentCustomers = serviceDto.MaxConcurrentCustomers;
                        existing.ServicePrice = serviceDto.ServicePrice;
                        await _unitOfWork.GetWriteRepository<BusinessService>().UpdateAsync(existing);
                    }
                }
            }
        }

        // Business Photos — ExistingPhotos üzerinden IsDeleted=true olanları sil
        if (request.ExistingPhotos != null)
        {
            foreach (var photoDto in request.ExistingPhotos)
            {
                if (photoDto.Id == 0) continue;

                var existing = business.BusinessPhotos.FirstOrDefault(p => p.Id == photoDto.Id);
                if (existing == null) continue;

                if (photoDto.IsDeleted)
                {
                    if (!string.IsNullOrEmpty(existing.PhotoPath))
                        await _fileService.DeleteFileAsync(existing.PhotoPath);
                    await _unitOfWork.GetWriteRepository<BusinessPhoto>().HardDeleteAsync(existing);
                }
            }
        }

        // Yeni fotoğraflar — IFormFile olarak gelir
        if (request.BusinessPhotos != null && request.BusinessPhotos.Count > 0)
        {
            var newPhotos = new List<BusinessPhoto>();
            foreach (var file in request.BusinessPhotos)
            {
                var photoPath = await _fileService.UploadFileAsync(file, $"business/{business.Id}/photos");
                newPhotos.Add(new BusinessPhoto
                {
                    PhotoPath = photoPath,
                    IsActive = true,
                    BusinessId = business.Id
                });
            }
            await _unitOfWork.GetWriteRepository<BusinessPhoto>().AddRangeAsync(newPhotos);
        }

        // Business Location — upsert
        if (request.Location != null)
        {
            var existingLocation = business.BusinessLocations.FirstOrDefault();
            if (existingLocation != null)
            {
                existingLocation.Latitude = request.Location.Latitude;
                existingLocation.Longitude = request.Location.Longitude;
                await _unitOfWork.GetWriteRepository<BusinessLocation>().UpdateAsync(existingLocation);
            }
            else
            {
                var location = new BusinessLocation(business.Id, request.Location.Latitude, request.Location.Longitude);
                await _unitOfWork.GetWriteRepository<BusinessLocation>().AddAsync(location);
            }
        }

        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateBusinessDetailByIdCommandResponse>.SuccessResult(
            new UpdateBusinessDetailByIdCommandResponse { Id = business.Id },
            "İşletme detayları başarıyla güncellendi.");
    }
}
