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

        // Business Hours — mevcut saatleri sil, yenileri ekle
        if (request.BusinessHours != null)
        {
            if (business.BusinessHours.Count > 0)
            {
                await _unitOfWork.GetWriteRepository<BusinessHour>().HardDeleteRangeAsync(business.BusinessHours.ToList());
            }

            if (request.BusinessHours.Count > 0)
            {
                var hours = request.BusinessHours.Select(h => new BusinessHour
                {
                    Day = h.Day!,
                    OpenTime = h.OpenTime!,
                    CloseTime = h.CloseTime!,
                    BusinessId = business.Id
                }).ToList();

                await _unitOfWork.GetWriteRepository<BusinessHour>().AddRangeAsync(hours);
            }
        }

        // Business Services — delta güncelleme
        if (request.BusinessServices != null)
        {
            var requestServiceIds = request.BusinessServices
                .Where(s => s.Id.HasValue)
                .Select(s => s.Id!.Value)
                .ToHashSet();

            // DB'de var ama request'te olmayan servisler
            var servicesToRemove = business.BusinessServices
                .Where(s => !requestServiceIds.Contains(s.Id))
                .ToList();

            foreach (var service in servicesToRemove)
            {
                if (service.Appointments.Count > 0)
                {
                    // Randevusu olan servisler soft-delete yapılır
                    service.IsDeleted = true;
                    await _unitOfWork.GetWriteRepository<BusinessService>().UpdateAsync(service);
                }
                else
                {
                    await _unitOfWork.GetWriteRepository<BusinessService>().HardDeleteAsync(service);
                }
            }

            // Request'te Id'si olan mevcut servisler → güncelle
            foreach (var serviceDto in request.BusinessServices.Where(s => s.Id.HasValue))
            {
                var existing = business.BusinessServices.FirstOrDefault(s => s.Id == serviceDto.Id);
                if (existing == null) continue;
                existing.ServiceTitle = serviceDto.ServiceTitle!;
                existing.ServiceContent = serviceDto.ServiceContent!;
                existing.MaxConcurrentCustomers = serviceDto.MaxConcurrentCustomers;
                existing.ServicePrice = serviceDto.ServicePrice;
                await _unitOfWork.GetWriteRepository<BusinessService>().UpdateAsync(existing);
            }

            // Request'te Id'si olmayan servisler → yeni ekle
            var newServices = request.BusinessServices
                .Where(s => !s.Id.HasValue)
                .Select(s => new BusinessService
                {
                    ServiceTitle = s.ServiceTitle!,
                    ServiceContent = s.ServiceContent!,
                    MaxConcurrentCustomers = s.MaxConcurrentCustomers,
                    ServicePrice = s.ServicePrice,
                    BusinessId = business.Id
                }).ToList();

            if (newServices.Count > 0)
                await _unitOfWork.GetWriteRepository<BusinessService>().AddRangeAsync(newServices);
        }

        // Business Photos — delta yaklaşımı: belirtilenler silinir, yeniler eklenir
        if (request.PhotoIdsToDelete != null && request.PhotoIdsToDelete.Count > 0)
        {
            var photosToDelete = business.BusinessPhotos
                .Where(p => request.PhotoIdsToDelete.Contains(p.Id))
                .ToList();

            foreach (var photo in photosToDelete)
            {
                if (!string.IsNullOrEmpty(photo.PhotoPath))
                    await _fileService.DeleteFileAsync(photo.PhotoPath);
            }

            if (photosToDelete.Count > 0)
                await _unitOfWork.GetWriteRepository<BusinessPhoto>().HardDeleteRangeAsync(photosToDelete);
        }

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
