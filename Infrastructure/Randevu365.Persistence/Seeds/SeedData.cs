using Microsoft.EntityFrameworkCore;
using Randevu365.Domain.Entities;
using Randevu365.Domain.Enum;

namespace Randevu365.Persistence.Seeds;

public static class SeedData
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Pre-computed BCrypt hashes
    private const string AdminHash = "$2a$11$iTswnw/5VyzC43Wy13w9vOTPQ.rwl6BZjcx3pH5XUdsGhzEwmVPNS";
    private const string BusinessHash = "$2a$11$Rqi6dTA8Q42o/eCnzg9prOPAPj2tRB3SX5O5HneHtE2w85nv4VKP.";
    private const string CustomerHash = "$2a$11$IGW3Vm5qpmfE4a.Yf2qUXuoeZL72Yl7SSljF1zpU00THU4a8h97kO";

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedAppUserInformations(modelBuilder);
        SeedAppUsers(modelBuilder);
        SeedBusinesses(modelBuilder);
        SeedBusinessComments(modelBuilder);
    }

    private static void SeedAppUserInformations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUserInformation>().HasData(
            // Admin
            new AppUserInformation { Id = 1, Name = "Admin", Surname = "Randevu365", Age = 30, Gender = "E", PhoneNumber = "05001234567", Height = 175, Weight = 75, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            // Business Owners (ID 2-6)
            new AppUserInformation { Id = 2, Name = "Ahmet", Surname = "Yılmaz", Age = 35, Gender = "E", PhoneNumber = "05321234567", Height = 180, Weight = 80, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 3, Name = "Elif", Surname = "Kaya", Age = 28, Gender = "K", PhoneNumber = "05331234567", Height = 165, Weight = 55, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 4, Name = "Mehmet", Surname = "Demir", Age = 42, Gender = "E", PhoneNumber = "05341234567", Height = 178, Weight = 85, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 5, Name = "Ayşe", Surname = "Çelik", Age = 25, Gender = "K", PhoneNumber = "05351234567", Height = 160, Weight = 52, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 6, Name = "Ali", Surname = "Öztürk", Age = 30, Gender = "E", PhoneNumber = "05361234567", Height = 182, Weight = 78, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            // Customers (ID 7-11)
            new AppUserInformation { Id = 7, Name = "Zeynep", Surname = "Arslan", Age = 22, Gender = "K", PhoneNumber = "05371234567", Height = 168, Weight = 58, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 8, Name = "Burak", Surname = "Doğan", Age = 27, Gender = "E", PhoneNumber = "05381234567", Height = 176, Weight = 74, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 9, Name = "Seda", Surname = "Kılıç", Age = 24, Gender = "K", PhoneNumber = "05391234567", Height = 163, Weight = 56, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 10, Name = "Emre", Surname = "Aslan", Age = 31, Gender = "E", PhoneNumber = "05401234567", Height = 179, Weight = 77, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUserInformation { Id = 11, Name = "Merve", Surname = "Çetin", Age = 26, Gender = "K", PhoneNumber = "05411234567", Height = 166, Weight = 54, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false }
        );
    }

    private static void SeedAppUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>().HasData(
            // Admin
            new AppUser("admin@randevu365.com", AdminHash, "Administrator") { Id = 1, AppUserInformationId = 1, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            // Business Owners (ID 2-6)
            new AppUser("owner1@randevu365.com", BusinessHash, "BusinessOwner") { Id = 2, AppUserInformationId = 2, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("owner2@randevu365.com", BusinessHash, "BusinessOwner") { Id = 3, AppUserInformationId = 3, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("owner3@randevu365.com", BusinessHash, "BusinessOwner") { Id = 4, AppUserInformationId = 4, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("owner4@randevu365.com", BusinessHash, "BusinessOwner") { Id = 5, AppUserInformationId = 5, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("owner5@randevu365.com", BusinessHash, "BusinessOwner") { Id = 6, AppUserInformationId = 6, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            // Customers (ID 7-11)
            new AppUser("customer1@randevu365.com", CustomerHash, "Customer") { Id = 7, AppUserInformationId = 7, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("customer2@randevu365.com", CustomerHash, "Customer") { Id = 8, AppUserInformationId = 8, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("customer3@randevu365.com", CustomerHash, "Customer") { Id = 9, AppUserInformationId = 9, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("customer4@randevu365.com", CustomerHash, "Customer") { Id = 10, AppUserInformationId = 10, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new AppUser("customer5@randevu365.com", CustomerHash, "Customer") { Id = 11, AppUserInformationId = 11, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false }
        );
    }

    private static void SeedBusinesses(ModelBuilder modelBuilder)
    {
        var categories = new[] { BusinessCategory.Kuafor, BusinessCategory.Guzellik, BusinessCategory.Saglik, BusinessCategory.Fitness, BusinessCategory.Dis };

        var businesses = new List<Business>();
        int businessId = 1;

        for (int ownerIndex = 0; ownerIndex < 5; ownerIndex++)
        {
            int appUserId = ownerIndex + 2; // userId 2-6
            var category = categories[ownerIndex];

            for (int j = 1; j <= 5; j++)
            {
                businesses.Add(new Business
                {
                    Id = businessId,
                    BusinessName = $"Business Owner {ownerIndex + 1} - İşletme {j}",
                    BusinessAddress = $"Dummy Adres No:{businessId}",
                    BusinessCity = "İstanbul",
                    BusinessPhone = $"0212{businessId:D7}",
                    BusinessEmail = $"owner{ownerIndex + 1}@randevu365.com",
                    BusinessCountry = "Türkiye",
                    BusinessCategory = category,
                    AppUserId = appUserId,
                    CreatedAt = SeedDate,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                });
                businessId++;
            }
        }

        modelBuilder.Entity<Business>().HasData(businesses);
    }

    private static void SeedBusinessComments(ModelBuilder modelBuilder)
    {
        var comments = new List<BusinessComment>();
        int commentId = 1;
        int[] customerIds = { 7, 8, 9, 10, 11 };

        string[] commentTexts =
        {
            "Çok memnun kaldım, teşekkürler!",
            "Hizmet kalitesi güzeldi.",
            "Kesinlikle tavsiye ederim.",
            "Gayet profesyonel bir işletme.",
            "Tekrar geleceğim, memnun kaldım."
        };

        for (int businessId = 1; businessId <= 25; businessId++)
        {
            // Her business'e 1-2 yorum
            int customerIndex = (businessId - 1) % 5;
            comments.Add(new BusinessComment
            {
                Id = commentId++,
                BusinessId = businessId,
                AppUserId = customerIds[customerIndex],
                Comment = commentTexts[customerIndex],
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate,
                IsDeleted = false
            });

            // Çift ID'li business'lere ikinci yorum ekle
            if (businessId % 2 == 0)
            {
                int secondCustomerIndex = (customerIndex + 1) % 5;
                comments.Add(new BusinessComment
                {
                    Id = commentId++,
                    BusinessId = businessId,
                    AppUserId = customerIds[secondCustomerIndex],
                    Comment = commentTexts[secondCustomerIndex],
                    CreatedAt = SeedDate,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                });
            }
        }

        modelBuilder.Entity<BusinessComment>().HasData(comments);
    }
}
