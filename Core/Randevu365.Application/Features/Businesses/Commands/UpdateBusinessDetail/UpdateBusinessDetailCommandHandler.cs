using MediatR;
using Microsoft.EntityFrameworkCore;
using Randevu365.Application.Common.Responses;
using Randevu365.Application.Interfaces;
using Randevu365.Domain.Entities;
using BusinessLogoEntity = Randevu365.Domain.Entities.BusinessLogo;

namespace Randevu365.Application.Features.Businesses.Commands.UpdateBusinessDetail;

public class UpdateBusinessDetailCommandHandler : IRequestHandler<UpdateBusinessDetailCommandRequest, ApiResponse<UpdateBusinessDetailCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;

    public UpdateBusinessDetailCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileService = fileService;
    }

    public async Task<ApiResponse<UpdateBusinessDetailCommandResponse>> Handle(UpdateBusinessDetailCommandRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateBusinessDetailCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<UpdateBusinessDetailCommandResponse>.FailResult(errors);
        }

        if (_currentUserService.UserId is null)
        {
            return ApiResponse<UpdateBusinessDetailCommandResponse>.UnauthorizedResult("Kullanıcı oturumu bulunamadı.");
        }

        var business = await _unitOfWork.GetReadRepository<Business>()
            .GetAsync(
                predicate: x => x.AppUserId == _currentUserService.UserId.Value,
                include: i => i
                    .Include(b => b.BusinessLogo)
                    .Include(b => b.BusinessHours)
                    .Include(b => b.BusinessPhotos)
                    .Include(b => b.BusinessServices),
                enableTracking: true
            );

        if (business == null)
        {
            return ApiResponse<UpdateBusinessDetailCommandResponse>.NotFoundResult("Kullanıcıya ait işletme bulunamadı.");
        }

        business.BusinessName = request.BusinessName!;
        business.BusinessAddress = request.BusinessAddress!;
        business.BusinessCity = request.BusinessCity!;
        business.BusinessPhone = request.BusinessPhone!;
        business.BusinessEmail = request.BusinessEmail!;
        business.BusinessCountry = request.BusinessCountry!;

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

        // Business Services — mevcut hizmetleri sil, yenileri ekle
        if (request.BusinessServices != null)
        {
            if (business.BusinessServices.Count > 0)
            {
                await _unitOfWork.GetWriteRepository<BusinessService>().HardDeleteRangeAsync(business.BusinessServices.ToList());
            }

            if (request.BusinessServices.Count > 0)
            {
                var services = request.BusinessServices.Select(s => new BusinessService
                {
                    ServiceTitle = s.ServiceTitle!,
                    ServiceContent = s.ServiceContent!,
                    MaxConcurrentCustomers = s.MaxConcurrentCustomers,
                    BusinessId = business.Id
                }).ToList();

                await _unitOfWork.GetWriteRepository<BusinessService>().AddRangeAsync(services);
            }
        }

        // Business Photos — delta yaklaşımı: belirtilenler silinir, yeniler eklenir

        // Silme: ownership kontrolü business.BusinessPhotos üzerinde yapılır (güvenli)
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
                await _unitOfWork.GetWriteRepository<BusinessPhoto>()
                    .HardDeleteRangeAsync(photosToDelete);
        }

        // Ekleme: sadece yeni gönderilen dosyalar yüklenir
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

        await _unitOfWork.SaveAsync();

        return ApiResponse<UpdateBusinessDetailCommandResponse>.SuccessResult(
            new UpdateBusinessDetailCommandResponse { Id = business.Id },
            "İşletme detayları başarıyla güncellendi.");
    }
}
