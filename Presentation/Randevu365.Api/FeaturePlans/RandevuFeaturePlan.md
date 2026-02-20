# Plan: Randevu (Appointment) Feature

## Bağlam ve Problem

Sistemde işletme hizmetleri (`BusinessService`) ve çalışma saatleri (`BusinessHour`) zaten tanımlı;
`MaxConcurrentCustomers` alanıyla bir hizmetin aynı anda kaç müşteriye verilebileceği de tutulmakta.
Ancak müşterilerin bu hizmetlere randevu oluşturmasını, işletme sahibinin bu randevuları
yönetmesini sağlayacak bir `Appointment` entity'si henüz yok.

Bu plan, sisteme:
- Müşteri tarafı: randevu oluşturma, iptal etme, kendi randevularını listeleme
- İşletme sahibi tarafı: randevuyu onaylama, tamamlama, iptal etme, işletmenin randevularını listeleme
- Kapasite doğrulaması: aynı slot'ta MaxConcurrentCustomers aşılırsa randevu reddedilir

özelliklerini ekler.

---

## Tasarım Kararları

### 1. Zaman Modeli: DateOnly + TimeOnly vs DateTime

| Yaklaşım | Avantaj | Dezavantaj |
|---|---|---|
| `DateOnly + TimeOnly StartTime + EndTime` | Tarih/saat ayrımı net, ileride timezone desteği kolay | İki alan |
| `DateTime AppointmentDateTime` | Tek alan | Bitiş saati implicit, kapasite hesabı zorlaşır |

**Karar: `DateOnly AppointmentDate + TimeOnly StartTime + TimeOnly EndTime`**
Bitiş saatini açık tutmak, aynı slota çakışan randevuları sorgulamayı ve MaxConcurrentCustomers
kontrolünü basitleştirir.

### 2. Status Yönetimi: Enum vs String

**Karar: `AppointmentStatus` enum (int olarak DB'de saklanır)**
- Pending (0) → Beklemede
- Confirmed (1) → Onaylandı
- Cancelled (2) → İptal Edildi
- Completed (3) → Tamamlandı

### 3. İptal Yetkisi

- Müşteri yalnızca kendi randevusunu iptal edebilir (sadece Pending veya Confirmed durumdayken)
- İşletme sahibi tüm durumları yönetebilir

---

## Değiştirilecek / Eklenecek Dosyalar (Uygulama Sırası)

---

### 1. Domain — Yeni Enum Ekle

**Dosya:** `Core/Randevu365.Domain/Enum/AppointmentStatus.cs`

```csharp
namespace Randevu365.Domain.Enum;

public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}
```

---

### 2. Domain — Yeni Entity Ekle

**Dosya:** `Core/Randevu365.Domain/Entities/Appointment.cs`

```csharp
using Randevu365.Domain.Base;
using Randevu365.Domain.Enum;

namespace Randevu365.Domain.Entities;

public class Appointment : BaseEntity
{
    // Müşteri (randevuyu alan kişi)
    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }

    // İşletme
    public int BusinessId { get; set; }
    public virtual Business? Business { get; set; }

    // Hangi hizmet için randevu alındığı
    public int BusinessServiceId { get; set; }
    public virtual BusinessService? BusinessService { get; set; }

    // Tarih ve saat dilimi
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    // Durum
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    // Notlar (opsiyonel)
    public string? CustomerNotes { get; set; }
    public string? BusinessNotes { get; set; }
}
```

---

### 3. Domain — Navigation Property Eklemeleri

#### 3a. AppUser

**Dosya:** `Core/Randevu365.Domain/Entities/AppUser.cs`

```csharp
public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
```

#### 3b. Business

**Dosya:** `Core/Randevu365.Domain/Entities/Business.cs`

```csharp
public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
```

#### 3c. BusinessService

**Dosya:** `Core/Randevu365.Domain/Entities/BusinessService.cs`

```csharp
public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
```

---

### 4. Persistence — EF Konfigürasyonu Ekle

**Dosya:** `Infrastructure/Randevu365.Persistence/Configurations/AppointmentConfiguration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Randevu365.Domain.Entities;

namespace Randevu365.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CustomerNotes)
            .HasMaxLength(500);

        builder.Property(x => x.BusinessNotes)
            .HasMaxLength(500);

        // Müşteri silinirse randevular korunur (Restrict)
        builder.HasOne(x => x.AppUser)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // İşletme silinirse randevular da silinir (Cascade)
        builder.HasOne(x => x.Business)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Hizmet silinirse randevular korunur (Restrict)
        builder.HasOne(x => x.BusinessService)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.BusinessServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tarih + saat sorgularını hızlandırmak için index
        builder.HasIndex(x => new { x.BusinessId, x.AppointmentDate, x.StartTime });
    }
}
```

---

### 5. Persistence — DbContext Güncelle

**Dosya:** `Infrastructure/Randevu365.Persistence/Context/AppDbContext.cs`

```csharp
public DbSet<Appointment> Appointments { get; set; }
```

---

### 6. Persistence — BusinessConfiguration Güncelle

**Dosya:** `Infrastructure/Randevu365.Persistence/Configurations/BusinessConfiguration.cs`

`Appointments` cascade delete ilişkisini ekle:

```csharp
builder.HasMany(x => x.Appointments)
    .WithOne(x => x.Business)
    .HasForeignKey(x => x.BusinessId)
    .OnDelete(DeleteBehavior.Cascade);
```

---

### 7. Persistence — Migration Oluştur

```bash
dotnet ef migrations add AddAppointmentTable \
  --project Infrastructure/Randevu365.Persistence \
  --startup-project Presentation/Randevu365.Api
```

---

### 8. Application — Customer: CreateAppointmentCommand

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Commands/CreateAppointment/`

**Dosyalar:** Request, Handler, Validator, Response

#### Request

```csharp
public class CreateAppointmentCommandRequest : IRequest<ApiResponse<CreateAppointmentCommandResponse>>
{
    public int BusinessId { get; set; }
    public int BusinessServiceId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? CustomerNotes { get; set; }
}
```

#### Validator Kuralları

- `BusinessId > 0`
- `BusinessServiceId > 0`
- `AppointmentDate >= DateOnly.FromDateTime(DateTime.Today)` — geçmiş tarih yasak
- `StartTime < EndTime` — başlangıç < bitiş
- `CustomerNotes?.MaxLength(500)`

#### Handler İş Mantığı

1. Validasyon
2. CurrentUser kontrolü (Customer rolü)
3. `BusinessService` varlığı ve `BusinessId` uyumu kontrol
4. **Kapasite Kontrolü:** İstenen tarih+saat aralığında çakışan Pending/Confirmed randevu sayısı `>= BusinessService.MaxConcurrentCustomers` ise `ConflictResult("Bu hizmet için seçilen saat dilimi dolu.")`
5. `Appointment` oluştur ve kaydet
6. `CreatedResult` döndür

#### Response

```csharp
public class CreateAppointmentCommandResponse
{
    public int Id { get; set; }
    public AppointmentStatus Status { get; set; }
}
```

---

### 9. Application — Customer: CancelAppointmentByCustomerCommand

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Commands/CancelAppointmentByCustomer/`

#### Request

```csharp
public class CancelAppointmentByCustomerCommandRequest : IRequest<ApiResponse<CancelAppointmentByCustomerCommandResponse>>
{
    public int AppointmentId { get; set; }
}
```

#### Handler İş Mantığı

1. Randevuyu getir
2. `AppUserId == currentUserId` kontrolü (yetkisiz erişimi engelle)
3. Durum `Pending` veya `Confirmed` değilse hata döndür ("Bu randevu iptal edilemez.")
4. `Status = Cancelled` olarak güncelle ve kaydet

---

### 10. Application — Business Owner: ConfirmAppointmentCommand

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Commands/ConfirmAppointment/`

#### Handler İş Mantığı

1. Randevuyu getir
2. Randevu işletmenin o kullanıcıya ait business'ına mı ait? Kontrol et
3. Durum `Pending` değilse hata döndür
4. `Status = Confirmed` olarak güncelle

---

### 11. Application — Business Owner: CompleteAppointmentCommand

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Commands/CompleteAppointment/`

Benzer yapı; sadece `Confirmed` → `Completed` geçişine izin verir.

---

### 12. Application — Business Owner: CancelAppointmentByBusinessCommand

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Commands/CancelAppointmentByBusiness/`

`BusinessNotes` alanına işletme açıklaması yazma imkânı sunar.

```csharp
public class CancelAppointmentByBusinessCommandRequest : IRequest<ApiResponse<CancelAppointmentByBusinessCommandResponse>>
{
    public int AppointmentId { get; set; }
    public string? BusinessNotes { get; set; }
}
```

---

### 13. Application — Queries

#### 13a. GetMyAppointments (Customer)

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Queries/GetMyAppointments/`

- JWT'den müşteri Id'si alınır
- Tüm randevuları döndürür (opsiyonel: status filtresi)
- Include: Business (adı), BusinessService (hizmet adı)

**Response DTO:**
```csharp
public class AppointmentSummaryDto
{
    public int Id { get; set; }
    public string BusinessName { get; set; }
    public string ServiceTitle { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
}
```

#### 13b. GetAppointmentById

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Queries/GetAppointmentById/`

- Customer kendi randevusunu, BusinessOwner işletmesine ait randevuyu görebilir

#### 13c. GetBusinessAppointments (Business Owner)

**Klasör:** `Core/Randevu365.Application/Features/Appointments/Queries/GetBusinessAppointments/`

- JWT'den business owner Id'si alınır, ilgili işletmenin tüm randevuları listelenir
- Opsiyonel filtre: `AppointmentDate`, `Status`

---

### 14. Presentation — Controller Güncellemeleri

#### CustomerController

**Dosya:** `Presentation/Randevu365.Api/Controllers/CustomerController.cs`

```csharp
// Appointments
[HttpPost("appointment/create")]
public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommandRequest request)
{
    var response = await _mediator.Send(request);
    return StatusCode(response.StatusCode, response);
}

[HttpPatch("appointment/{appointmentId}/cancel")]
public async Task<IActionResult> CancelAppointment(int appointmentId)
{
    var response = await _mediator.Send(new CancelAppointmentByCustomerCommandRequest { AppointmentId = appointmentId });
    return StatusCode(response.StatusCode, response);
}

[HttpGet("appointments")]
public async Task<IActionResult> GetMyAppointments()
{
    var response = await _mediator.Send(new GetMyAppointmentsQueryRequest());
    return StatusCode(response.StatusCode, response);
}

[HttpGet("appointment/{appointmentId}")]
public async Task<IActionResult> GetAppointmentById(int appointmentId)
{
    var response = await _mediator.Send(new GetAppointmentByIdQueryRequest { AppointmentId = appointmentId });
    return StatusCode(response.StatusCode, response);
}
```

#### BusinessController

**Dosya:** `Presentation/Randevu365.Api/Controllers/BusinessController.cs`

```csharp
// Appointments
[HttpGet("appointments")]
public async Task<IActionResult> GetBusinessAppointments([FromQuery] DateOnly? date, [FromQuery] AppointmentStatus? status)
{
    var response = await _mediator.Send(new GetBusinessAppointmentsQueryRequest { Date = date, Status = status });
    return StatusCode(response.StatusCode, response);
}

[HttpPatch("appointment/{appointmentId}/confirm")]
public async Task<IActionResult> ConfirmAppointment(int appointmentId)
{
    var response = await _mediator.Send(new ConfirmAppointmentCommandRequest { AppointmentId = appointmentId });
    return StatusCode(response.StatusCode, response);
}

[HttpPatch("appointment/{appointmentId}/complete")]
public async Task<IActionResult> CompleteAppointment(int appointmentId)
{
    var response = await _mediator.Send(new CompleteAppointmentCommandRequest { AppointmentId = appointmentId });
    return StatusCode(response.StatusCode, response);
}

[HttpPatch("appointment/{appointmentId}/cancel")]
public async Task<IActionResult> CancelAppointmentByBusiness(int appointmentId, [FromBody] CancelAppointmentByBusinessCommandRequest request)
{
    request.AppointmentId = appointmentId;
    var response = await _mediator.Send(request);
    return StatusCode(response.StatusCode, response);
}
```

---

## Migration / Altyapı Özeti

| Adım | Gerekli mi? | Açıklama |
|---|---|---|
| Yeni paket | Hayır | Mevcut bağımlılıklar yeterli |
| DI kaydı | Hayır | MediatR handler'ları otomatik taranır |
| EF Migration | **Evet** | `Appointments` tablosu eklenir |
| Seed data | Hayır | — |

---

## Doğrulama

1. `dotnet build` → 0 hata
2. `dotnet ef database update` → migration başarılı
3. Swagger → **Müşteri akışı:**
   - `POST /api/customer/appointment/create` — yeni randevu oluştur, `201` bekle
   - `GET /api/customer/appointments` — oluşturulan randevuyu listede gör
   - `PATCH /api/customer/appointment/{id}/cancel` — randevuyu iptal et, durum `Cancelled` olmalı
4. Swagger → **İşletme akışı:**
   - `GET /api/business/appointments` — randevuyu listele
   - `PATCH /api/business/appointment/{id}/confirm` — onayla, durum `Confirmed` olmalı
   - `PATCH /api/business/appointment/{id}/complete` — tamamla, durum `Completed` olmalı
5. Kapasite testi: `MaxConcurrentCustomers = 1` olan bir hizmet için aynı tarih/saat aralığına 2 randevu oluşturmaya çalış — ikinci istek `409 Conflict` döndürmeli
6. Yetki testi: Müşteri başkasının randevusunu iptal etmeye çalışırsa `401/403` almalı

---

## Notlar

- Durum geçişleri: `Pending → Confirmed → Completed` (normal akış), `Pending/Confirmed → Cancelled` (iptal)
- Çakışma kontrolü (`StartTime < mevcut.EndTime && EndTime > mevcut.StartTime`) koşulu, overlapping aralıkları yakalar
- İleride `GetAvailableSlots` endpoint'i eklenebilir: belirli tarih için boş slot'ları döndürür
- `BusinessHour` ile uyum kontrolü (işletmenin o gün açık olup olmadığı) opsiyonel olarak CreateAppointment validator'ına eklenebilir
