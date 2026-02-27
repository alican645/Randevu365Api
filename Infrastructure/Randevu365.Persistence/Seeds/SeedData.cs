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

    // Name pools
    private static readonly string[] MaleNames =
    {
        "Ahmet", "Mehmet", "Ali", "Hasan", "Hüseyin", "İbrahim", "Mustafa", "Osman", "Yusuf", "Murat",
        "Emre", "Burak", "Cem", "Deniz", "Eren", "Fatih", "Gökhan", "Halil", "İsmail", "Kadir",
        "Levent", "Mert", "Nihat", "Onur", "Ömer", "Recep", "Serkan", "Tahir", "Uğur", "Volkan"
    };

    private static readonly string[] FemaleNames =
    {
        "Elif", "Ayşe", "Fatma", "Zeynep", "Emine", "Hatice", "Merve", "Mine", "Derya", "Sibel",
        "Gül", "Hülya", "İrem", "Jale", "Kezban", "Lale", "Melis", "Nur", "Özlem", "Pınar",
        "Rabia", "Seda", "Tuğba", "Ülkü", "Vildan", "Yasemin", "Zara", "Aslı", "Burcu", "Canan"
    };

    private static readonly string[] Surnames =
    {
        "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Yıldırım", "Öztürk", "Aydın", "Özdemir",
        "Arslan", "Doğan", "Kılıç", "Aslan", "Çetin", "Kara", "Koç", "Kurt", "Özkan", "Şimşek",
        "Polat", "Korkmaz", "Erdoğan", "Güneş", "Acar"
    };

    private static readonly string[] Cities = { "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya" };

    // Category config: (category, nameTemplates, services)
    private static readonly (BusinessCategory Category, string[] NameTemplates, (string Title, string Content, decimal Price)[] Services)[] CategoryConfigs =
    {
        (BusinessCategory.Kuafor,
            new[] { "{name} Kuaför Salonu", "Stil {surname} Berber", "{name}'in Kuaförü" },
            new[] { ("Saç Kesimi", "Profesyonel saç kesimi hizmeti.", 200m), ("Saç Boyama", "Saç boyama ve bakım hizmeti.", 350m) }),

        (BusinessCategory.Guzellik,
            new[] { "{name} Güzellik Merkezi", "Beauty {surname}", "Güzel {name} Salonu" },
            new[] { ("Cilt Bakımı", "Profesyonel cilt bakımı ve temizleme.", 450m), ("Manikür Pedikür", "El ve ayak bakımı hizmeti.", 300m) }),

        (BusinessCategory.Saglik,
            new[] { "{surname} Sağlık Kliniği", "Dr. {name} Tıp Merkezi", "{name} Sağlık" },
            new[] { ("Genel Muayene", "Detaylı fiziksel muayene.", 700m), ("Check-up", "Kapsamlı sağlık taraması.", 1200m) }),

        (BusinessCategory.Fitness,
            new[] { "{name} Fitness Center", "Power {surname} Gym", "{name} Spor Salonu" },
            new[] { ("Aylık Üyelik", "Bir aylık tam erişim üyeliği.", 800m), ("Personal Training", "Birebir antrenör eşliğinde egzersiz.", 500m) }),

        (BusinessCategory.Dis,
            new[] { "Dr. {name} Diş Kliniği", "{surname} Dental", "{name} Ağız ve Diş Sağlığı" },
            new[] { ("Diş Temizliği", "Profesyonel diş temizleme işlemi.", 400m), ("Dolgu", "Diş dolgu uygulaması.", 600m) }),

        (BusinessCategory.Masaj,
            new[] { "{name} Masaj Salonu", "Relax {surname} Spa", "{name} Terapi Merkezi" },
            new[] { ("Klasik Masaj", "Geleneksel masaj uygulaması.", 350m), ("Aromaterapi", "Aromaterapi masaj seansı.", 500m) }),

        (BusinessCategory.Veteriner,
            new[] { "{name} Veteriner Kliniği", "{surname} Pet Kliniği", "Dr. {name} Veteriner" },
            new[] { ("Genel Muayene", "Evcil hayvan sağlık muayenesi.", 300m), ("Aşılama", "Evcil hayvan aşılama hizmeti.", 200m) }),
    };

    private static (string Name, string Surname, string Gender) GetOwnerIdentity(int index)
    {
        // Alternate male/female
        if (index % 2 == 0)
        {
            var name = MaleNames[index % MaleNames.Length];
            var surname = Surnames[index % Surnames.Length];
            return (name, surname, "E");
        }
        else
        {
            var name = FemaleNames[index % FemaleNames.Length];
            var surname = Surnames[index % Surnames.Length];
            return (name, surname, "K");
        }
    }

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedAppUserInformations(modelBuilder);
        SeedAppUsers(modelBuilder);
        SeedBusinesses(modelBuilder);
        SeedBusinessLocations(modelBuilder);
        SeedBusinessHours(modelBuilder);
        SeedBusinessServices(modelBuilder);
        SeedBusinessLogos(modelBuilder);
        SeedBusinessPhotos(modelBuilder);
        SeedAppointments(modelBuilder);
        SeedBusinessSlots(modelBuilder);
        SeedBusinessComments(modelBuilder);
        SeedBusinessRatings(modelBuilder);
        SeedConversations(modelBuilder);
        SeedMessages(modelBuilder);
    }

    private static void SeedAppUserInformations(ModelBuilder modelBuilder)
    {
        var list = new List<AppUserInformation>
        {
            new() { Id = 1, Name = "Admin", Surname = "Randevu365", Age = 30, Gender = "E", PhoneNumber = "05001234567", Height = 175, Weight = 75, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 2, Name = "Ahmet", Surname = "Yılmaz", Age = 35, Gender = "E", PhoneNumber = "05321234567", Height = 180, Weight = 80, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 3, Name = "Elif", Surname = "Kaya", Age = 28, Gender = "K", PhoneNumber = "05331234567", Height = 165, Weight = 55, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 4, Name = "Mehmet", Surname = "Demir", Age = 42, Gender = "E", PhoneNumber = "05341234567", Height = 178, Weight = 85, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 5, Name = "Ayşe", Surname = "Çelik", Age = 25, Gender = "K", PhoneNumber = "05351234567", Height = 160, Weight = 52, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 6, Name = "Ali", Surname = "Öztürk", Age = 30, Gender = "E", PhoneNumber = "05361234567", Height = 182, Weight = 78, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 7, Name = "Zeynep", Surname = "Arslan", Age = 22, Gender = "K", PhoneNumber = "05371234567", Height = 168, Weight = 58, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
        };

        // New BusinessOwners: ID 8-357
        for (int i = 0; i < 350; i++)
        {
            int id = 8 + i;
            var (name, surname, gender) = GetOwnerIdentity(i);
            int age = 25 + (i % 20); // 25-44 arası
            int height = gender == "E" ? 170 + (i % 15) : 155 + (i % 15);
            int weight = gender == "E" ? 70 + (i % 20) : 50 + (i % 20);

            list.Add(new AppUserInformation
            {
                Id = id,
                Name = name,
                Surname = surname,
                Age = age,
                Gender = gender,
                PhoneNumber = $"05{(300 + i):D3}{(1000 + i):D4}",
                Height = height,
                Weight = weight,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<AppUserInformation>().HasData(list);
    }

    private static void SeedAppUsers(ModelBuilder modelBuilder)
    {
        var list = new List<AppUser>
        {
            new("admin@randevu365.com", AdminHash, "Administrator") { Id = 1, AppUserInformationId = 1, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new("ahmet@kuafor.com", BusinessHash, "BusinessOwner") { Id = 2, AppUserInformationId = 2, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new("elif@guzellik.com", BusinessHash, "BusinessOwner") { Id = 3, AppUserInformationId = 3, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new("mehmet@saglik.com", BusinessHash, "BusinessOwner") { Id = 4, AppUserInformationId = 4, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new("ayse@customer.com", CustomerHash, "Customer") { Id = 5, AppUserInformationId = 5, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new("ali@customer.com", CustomerHash, "Customer") { Id = 6, AppUserInformationId = 6, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new("zeynep@customer.com", CustomerHash, "Customer") { Id = 7, AppUserInformationId = 7, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
        };

        // New BusinessOwners: ID 8-357
        for (int i = 0; i < 350; i++)
        {
            int id = 8 + i;
            list.Add(new AppUser($"owner{id}@randevu365.com", BusinessHash, "BusinessOwner")
            {
                Id = id,
                AppUserInformationId = id,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<AppUser>().HasData(list);
    }

    private static void SeedBusinesses(ModelBuilder modelBuilder)
    {
        var list = new List<Business>
        {
            new()
            {
                Id = 1, BusinessName = "Ahmet'in Kuaförü", BusinessAddress = "Bağdat Caddesi No:42", BusinessCity = "İstanbul",
                BusinessPhone = "02161234567", BusinessEmail = "ahmet@kuafor.com", BusinessCountry = "Türkiye",
                BusinessCategory = BusinessCategory.Kuafor, AppUserId = 2,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new()
            {
                Id = 2, BusinessName = "Elif Güzellik Salonu", BusinessAddress = "İstiklal Caddesi No:78", BusinessCity = "İstanbul",
                BusinessPhone = "02121234567", BusinessEmail = "elif@guzellik.com", BusinessCountry = "Türkiye",
                BusinessCategory = BusinessCategory.Guzellik, AppUserId = 3,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new()
            {
                Id = 3, BusinessName = "Mehmet Sağlık Kliniği", BusinessAddress = "Nişantaşı Abdi İpekçi Cad. No:15", BusinessCity = "İstanbul",
                BusinessPhone = "02127654321", BusinessEmail = "mehmet@saglik.com", BusinessCountry = "Türkiye",
                BusinessCategory = BusinessCategory.Saglik, AppUserId = 4,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
        };

        // New businesses: ID 4-353 (350 businesses, 50 per category)
        int businessId = 4;
        int appUserId = 8;

        for (int catIdx = 0; catIdx < CategoryConfigs.Length; catIdx++)
        {
            var config = CategoryConfigs[catIdx];
            for (int j = 0; j < 50; j++)
            {
                var (name, surname, _) = GetOwnerIdentity(appUserId - 8);
                var template = config.NameTemplates[j % config.NameTemplates.Length];
                var businessName = template.Replace("{name}", name).Replace("{surname}", surname);
                var city = Cities[(catIdx * 50 + j) % Cities.Length];

                list.Add(new Business
                {
                    Id = businessId,
                    BusinessName = businessName,
                    BusinessAddress = $"{city} Merkez Mah. No:{10 + j}",
                    BusinessCity = city,
                    BusinessPhone = $"0{200 + catIdx}{1000 + j:D4}",
                    BusinessEmail = $"owner{appUserId}@randevu365.com",
                    BusinessCountry = "Türkiye",
                    BusinessCategory = config.Category,
                    AppUserId = appUserId,
                    CreatedAt = SeedDate,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                });

                businessId++;
                appUserId++;
            }
        }

        modelBuilder.Entity<Business>().HasData(list);
    }

    private static void SeedBusinessLocations(ModelBuilder modelBuilder)
    {
        // Existing 3 (anonymous type required due to constructor)
        var existing = new object[]
        {
            new { Id = 1, BusinessId = 1, Latitude = 40.9632m, Longitude = 29.0642m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new { Id = 2, BusinessId = 2, Latitude = 41.0340m, Longitude = 28.9770m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new { Id = 3, BusinessId = 3, Latitude = 41.0482m, Longitude = 28.9948m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
        };

        // New locations: ID 4-353
        var all = new List<object>(existing);
        for (int i = 0; i < 350; i++)
        {
            int id = 4 + i;
            int businessId = 4 + i;
            // Istanbul-centered coordinates with deterministic spread
            decimal lat = 41.01m + (decimal)(((i * 7) % 100 - 50) * 0.001);
            decimal lng = 28.97m + (decimal)(((i * 13) % 100 - 50) * 0.001);

            all.Add(new { Id = id, BusinessId = businessId, Latitude = lat, Longitude = lng, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false });
        }

        modelBuilder.Entity<BusinessLocation>().HasData(all.ToArray());
    }

    private static void SeedBusinessHours(ModelBuilder modelBuilder)
    {
        var hours = new List<BusinessHour>();
        var days = new[] { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };
        int id = 1;

        // All businesses: 1-3 (existing) + 4-353 (new)
        int totalBusinesses = 353;
        for (int businessId = 1; businessId <= totalBusinesses; businessId++)
        {
            foreach (var day in days)
            {
                string open, close;
                if (day == "Pazar")
                {
                    open = "Kapalı";
                    close = "Kapalı";
                }
                else if (day == "Cumartesi")
                {
                    open = "10:00";
                    close = "16:00";
                }
                else
                {
                    open = "09:00";
                    close = "18:00";
                }

                hours.Add(new BusinessHour
                {
                    Id = id++, BusinessId = businessId, Day = day, OpenTime = open, CloseTime = close,
                    CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
                });
            }
        }

        modelBuilder.Entity<BusinessHour>().HasData(hours);
    }

    private static void SeedBusinessServices(ModelBuilder modelBuilder)
    {
        var list = new List<BusinessService>
        {
            // Existing: Ahmet'in Kuaförü
            new() { Id = 1, BusinessId = 1, ServiceTitle = "Erkek Saç Kesimi", ServiceContent = "Profesyonel erkek saç kesimi, yıkama ve şekillendirme dahil.", MaxConcurrentCustomers = 1, ServicePrice = 250m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 2, BusinessId = 1, ServiceTitle = "Sakal Tıraşı", ServiceContent = "Geleneksel ustura ile sakal tıraşı ve bakım.", MaxConcurrentCustomers = 1, ServicePrice = 150m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 3, BusinessId = 1, ServiceTitle = "Saç Boyama", ServiceContent = "Erkek saç boyama hizmeti, renk danışmanlığı dahil.", MaxConcurrentCustomers = 1, ServicePrice = 400m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            // Existing: Elif Güzellik Salonu
            new() { Id = 4, BusinessId = 2, ServiceTitle = "Cilt Bakımı", ServiceContent = "Profesyonel cilt bakımı, temizleme ve nemlendirme işlemi.", MaxConcurrentCustomers = 2, ServicePrice = 500m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 5, BusinessId = 2, ServiceTitle = "Manikür & Pedikür", ServiceContent = "El ve ayak bakımı, oje uygulaması dahil.", MaxConcurrentCustomers = 3, ServicePrice = 350m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 6, BusinessId = 2, ServiceTitle = "Kaş Tasarımı", ServiceContent = "Profesyonel kaş şekillendirme ve tasarım.", MaxConcurrentCustomers = 1, ServicePrice = 200m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            // Existing: Mehmet Sağlık Kliniği
            new() { Id = 7, BusinessId = 3, ServiceTitle = "Genel Muayene", ServiceContent = "Detaylı fiziksel muayene ve sağlık değerlendirmesi.", MaxConcurrentCustomers = 1, ServicePrice = 800m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 8, BusinessId = 3, ServiceTitle = "Kan Tahlili", ServiceContent = "Kapsamlı kan tahlili ve sonuç değerlendirmesi.", MaxConcurrentCustomers = 5, ServicePrice = 600m, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
        };

        // New services: 2 per new business (ID 4-353)
        int serviceId = 9;
        int businessId = 4;

        for (int catIdx = 0; catIdx < CategoryConfigs.Length; catIdx++)
        {
            var services = CategoryConfigs[catIdx].Services;
            for (int j = 0; j < 50; j++)
            {
                for (int s = 0; s < services.Length; s++)
                {
                    list.Add(new BusinessService
                    {
                        Id = serviceId++,
                        BusinessId = businessId,
                        ServiceTitle = services[s].Title,
                        ServiceContent = services[s].Content,
                        MaxConcurrentCustomers = 1,
                        ServicePrice = services[s].Price,
                        CreatedAt = SeedDate,
                        UpdatedAt = SeedDate,
                        IsDeleted = false
                    });
                }
                businessId++;
            }
        }

        modelBuilder.Entity<BusinessService>().HasData(list);
    }

    private static void SeedBusinessLogos(ModelBuilder modelBuilder)
    {
        var list = new List<BusinessLogo>
        {
            new() { Id = 1, BusinessId = 1, LogoUrl = "/uploads/business/1/logo.png", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 2, BusinessId = 2, LogoUrl = "/uploads/business/2/logo.png", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 3, BusinessId = 3, LogoUrl = "/uploads/business/3/logo.png", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
        };

        for (int i = 0; i < 350; i++)
        {
            int id = 4 + i;
            int businessId = 4 + i;
            list.Add(new BusinessLogo
            {
                Id = id,
                BusinessId = businessId,
                LogoUrl = $"/uploads/business/{businessId}/logo.png",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<BusinessLogo>().HasData(list);
    }

    private static void SeedBusinessPhotos(ModelBuilder modelBuilder)
    {
        var list = new List<BusinessPhoto>
        {
            new() { Id = 1, BusinessId = 1, PhotoPath = "/uploads/business/1/photo1.jpg", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 2, BusinessId = 1, PhotoPath = "/uploads/business/1/photo2.jpg", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 3, BusinessId = 2, PhotoPath = "/uploads/business/2/photo1.jpg", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 4, BusinessId = 2, PhotoPath = "/uploads/business/2/photo2.jpg", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new() { Id = 5, BusinessId = 3, PhotoPath = "/uploads/business/3/photo1.jpg", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
        };

        for (int i = 0; i < 350; i++)
        {
            int id = 6 + i;
            int businessId = 4 + i;
            list.Add(new BusinessPhoto
            {
                Id = id,
                BusinessId = businessId,
                PhotoPath = $"/uploads/business/{businessId}/photo1.jpg",
                IsActive = true,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<BusinessPhoto>().HasData(list);
    }

    private static void SeedAppointments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().HasData(
            new Appointment
            {
                Id = 1, AppUserId = 5, BusinessId = 1, BusinessServiceId = 1,
                AppointmentDate = new DateOnly(2026, 2, 10), RequestedStartTime = new TimeOnly(10, 0), RequestedEndTime = new TimeOnly(10, 30),
                ApproveStartTime = new TimeOnly(10, 0), ApproveEndTime = new TimeOnly(10, 30),
                Status = AppointmentStatus.Completed, CustomerNotes = "Kısa kesim istiyorum.", BusinessNotes = "Tamamlandı.",
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new Appointment
            {
                Id = 2, AppUserId = 6, BusinessId = 1, BusinessServiceId = 2,
                AppointmentDate = new DateOnly(2026, 2, 12), RequestedStartTime = new TimeOnly(14, 0), RequestedEndTime = new TimeOnly(14, 30),
                ApproveStartTime = new TimeOnly(14, 0), ApproveEndTime = new TimeOnly(14, 30),
                Status = AppointmentStatus.Confirmed,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new Appointment
            {
                Id = 3, AppUserId = 7, BusinessId = 2, BusinessServiceId = 4,
                AppointmentDate = new DateOnly(2026, 2, 15), RequestedStartTime = new TimeOnly(11, 0), RequestedEndTime = new TimeOnly(12, 0),
                Status = AppointmentStatus.Pending, CustomerNotes = "Hassas cildim var.",
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new Appointment
            {
                Id = 4, AppUserId = 5, BusinessId = 2, BusinessServiceId = 5,
                AppointmentDate = new DateOnly(2026, 2, 8), RequestedStartTime = new TimeOnly(15, 0), RequestedEndTime = new TimeOnly(16, 0),
                ApproveStartTime = new TimeOnly(15, 0), ApproveEndTime = new TimeOnly(16, 0),
                Status = AppointmentStatus.Completed,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new Appointment
            {
                Id = 5, AppUserId = 6, BusinessId = 3, BusinessServiceId = 7,
                AppointmentDate = new DateOnly(2026, 2, 20), RequestedStartTime = new TimeOnly(9, 0), RequestedEndTime = new TimeOnly(9, 30),
                Status = AppointmentStatus.Cancelled, CustomerNotes = "Maalesef gelemeyeceğim.", BusinessNotes = "İptal edildi.",
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new Appointment
            {
                Id = 6, AppUserId = 7, BusinessId = 3, BusinessServiceId = 8,
                AppointmentDate = new DateOnly(2026, 3, 1), RequestedStartTime = new TimeOnly(10, 0), RequestedEndTime = new TimeOnly(10, 30),
                Status = AppointmentStatus.Pending,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            }
        );
    }

    private static void SeedBusinessSlots(ModelBuilder modelBuilder)
    {
        var packageId1 = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        modelBuilder.Entity<BusinessSlot>().HasData(
            new BusinessSlot
            {
                Id = 1, AppUserId = 2, PurchasePrice = 99.99m, PaymentStatus = SlotPaymentStatus.Completed,
                PaymentMethod = SlotPaymentMethod.CreditCard, PaidAt = SeedDate, IsUsed = true,
                UsedForBusinessId = 1, UsedAt = SeedDate, PackageId = packageId1, PackageType = SlotPackageType.Single,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new BusinessSlot
            {
                Id = 2, AppUserId = 3, PurchasePrice = 249.99m, PaymentStatus = SlotPaymentStatus.Completed,
                PaymentMethod = SlotPaymentMethod.BankTransfer, PaidAt = SeedDate, IsUsed = true,
                UsedForBusinessId = 2, UsedAt = SeedDate, PackageId = packageId1, PackageType = SlotPackageType.Triple,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new BusinessSlot
            {
                Id = 3, AppUserId = 4, PurchasePrice = 99.99m, PaymentStatus = SlotPaymentStatus.Pending,
                PaymentMethod = SlotPaymentMethod.Online, IsUsed = false,
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            },
            new BusinessSlot
            {
                Id = 4, AppUserId = 2, PurchasePrice = 99.99m, PaymentStatus = SlotPaymentStatus.Failed,
                PaymentMethod = SlotPaymentMethod.CreditCard, IsUsed = false, Notes = "Kart limiti yetersiz.",
                CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false
            }
        );
    }

    private static void SeedBusinessComments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessComment>().HasData(
            new BusinessComment { Id = 1, BusinessId = 1, AppUserId = 5, Comment = "Çok memnun kaldım, saç kesimi harika oldu!", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessComment { Id = 2, BusinessId = 1, AppUserId = 6, Comment = "Hızlı ve kaliteli hizmet. Teşekkürler.", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessComment { Id = 3, BusinessId = 2, AppUserId = 7, Comment = "Cilt bakımı çok profesyoneldi, kesinlikle tavsiye ederim.", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessComment { Id = 4, BusinessId = 2, AppUserId = 5, Comment = "Manikür pedikür hizmeti güzeldi.", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessComment { Id = 5, BusinessId = 3, AppUserId = 6, Comment = "Doktor bey çok ilgiliydi, detaylı muayene yaptı.", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessComment { Id = 6, BusinessId = 3, AppUserId = 7, Comment = "Klinik çok temiz ve düzenli.", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false }
        );
    }

    private static void SeedBusinessRatings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessRating>().HasData(
            new BusinessRating { Id = 1, BusinessId = 1, AppUserId = 5, Rating = 5, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessRating { Id = 2, BusinessId = 1, AppUserId = 6, Rating = 4, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessRating { Id = 3, BusinessId = 2, AppUserId = 7, Rating = 5, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessRating { Id = 4, BusinessId = 2, AppUserId = 5, Rating = 4, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessRating { Id = 5, BusinessId = 3, AppUserId = 6, Rating = 5, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new BusinessRating { Id = 6, BusinessId = 3, AppUserId = 7, Rating = 4, CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false }
        );
    }

    private static void SeedConversations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>().HasData(
            new Conversation { Id = 1, UserId = 5, OtherUserId = 2, ConversationId = "conv_5_2", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new Conversation { Id = 2, UserId = 6, OtherUserId = 3, ConversationId = "conv_6_3", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false }
        );
    }

    private static void SeedMessages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>().HasData(
            new Message { Id = 1, ConversationId = "conv_5_2", SenderId = 5, ReceiverId = 2, Content = "Merhaba, yarın için randevu alabilir miyim?", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new Message { Id = 2, ConversationId = "conv_5_2", SenderId = 2, ReceiverId = 5, Content = "Tabii, saat 14:00 uygun olur mu?", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new Message { Id = 3, ConversationId = "conv_6_3", SenderId = 6, ReceiverId = 3, Content = "Cilt bakımı hakkında bilgi alabilir miyim?", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false },
            new Message { Id = 4, ConversationId = "conv_6_3", SenderId = 3, ReceiverId = 6, Content = "Elbette! Cilt tipinize göre özel bakım paketlerimiz mevcut.", CreatedAt = SeedDate, UpdatedAt = SeedDate, IsDeleted = false }
        );
    }
}
