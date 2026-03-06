# Plan: İşyeri Slot Satın Alma Sistemi

## Bağlam ve Problem

- **İlk işyeri ücretsiz** — kayıt olan her business owner bir işyeri açabilir, ekstra bir şey gerekmez.
- **İkinci ve sonraki her işyeri için tek seferlik bir slot satın alınır** — abonelik yok, aylık ücret yok.
  Her ek işyeri için ayrı bir satın alma yapılır.

Örnek akış:
```
Kayıt → İşyeri #1 → Ücretsiz ✓
İşyeri #2 eklemek istiyorum → Slot satın al → Slot aktifleşince işyeri açılır ✓
İşyeri #3 eklemek istiyorum → Yeni bir slot daha satın al → Aktifleşince açılır ✓
```

---

## Veritabanı Tasarımı

### Tek Yeni Tablo: `BusinessSlots`

```csharp
// Core/Randevu365.Domain/Entities/BusinessSlot.cs
public class BusinessSlot : BaseEntity
{
    // Kime ait
    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }

    // Ödeme bilgisi
    public decimal PurchasePrice { get; set; }    // Satın alma anındaki fiyat (TL)
    public SlotPaymentStatus PaymentStatus { get; set; } = SlotPaymentStatus.Pending;
    public SlotPaymentMethod PaymentMethod { get; set; }
    public string? ExternalTransactionId { get; set; }  // EFT dekontu, gateway ID, vb.
    public DateTime? PaidAt { get; set; }               // Ödeme onay tarihi

    // Kullanım durumu
    public bool IsUsed { get; set; } = false;
    public int? UsedForBusinessId { get; set; }    // Hangi işyerine harcandı
    public virtual Business? UsedForBusiness { get; set; }
    public DateTime? UsedAt { get; set; }

    // Admin notu
    public string? Notes { get; set; }
}
```

---

### Yeni Enum'lar

```csharp
// Core/Randevu365.Domain/Enum/SlotPaymentStatus.cs
public enum SlotPaymentStatus
{
    Pending = 0,     // Ödeme bekleniyor (kullanıcı talep oluşturdu)
    Completed = 1,   // Ödeme onaylandı, slot kullanıma hazır
    Failed = 2,      // Ödeme başarısız
    Refunded = 3     // İade edildi
}

// Core/Randevu365.Domain/Enum/SlotPaymentMethod.cs
public enum SlotPaymentMethod
{
    BankTransfer = 1,   // EFT / havale
    CreditCard = 2,     // Kredi kartı (ileride gateway ile)
    Online = 3          // Iyzico, Stripe vb. (ileride)
}
```

---

### Mevcut Entity Değişikliği

```csharp
// Core/Randevu365.Domain/Entities/AppUser.cs — tek satır eklenir
public virtual ICollection<BusinessSlot> BusinessSlots { get; set; } = new List<BusinessSlot>();
```

---

### ER Diyagramı

```
AppUser (1) ──────── (N) BusinessSlot
   │                        │
   └─── (N) Business ◄──────┘ (UsedForBusinessId)
```

---

## İş Kuralları

### Kural 1 — İşyeri Oluşturma Kota Kontrolü

`CreateBusinessCommandHandler` içinde, işyeri oluşturulmadan önce:

```
1. Kullanıcının mevcut aktif işyeri sayısı hesaplanır (IsDeleted = false)
2. businessCount == 0  →  Ücretsiz hak, devam et
3. businessCount >= 1  →  Kullanılmamış (IsUsed = false) ve
                           ödemesi tamamlanmış (PaymentStatus = Completed) bir slot var mı?
                           - Evet → işyerini oluştur, slotu kullanıldı olarak işaretle
                           - Hayır → 402 Payment Required döndür
```

Pseudo-code:
```csharp
var businessCount = await GetActiveBusinessCount(currentUserId);

if (businessCount >= 1)
{
    var availableSlot = await GetAvailableSlot(currentUserId);
    // IsUsed = false && PaymentStatus = Completed && IsDeleted = false

    if (availableSlot == null)
        return ApiResponse.PaymentRequiredResult(
            "Yeni bir işyeri eklemek için önce işyeri slotu satın almanız gerekiyor.");

    // İşyeri oluşturulduktan sonra:
    availableSlot.IsUsed = true;
    availableSlot.UsedForBusinessId = newBusiness.Id;
    availableSlot.UsedAt = DateTime.UtcNow;
}
```

### Kural 2 — Silinen İşyeri Slotu Geri Vermez

`IsDeleted = true` olan işyeri, slotu **serbest bırakmaz**. Slot harcandı sayılır. Bu kural;
slot satın alımını gerçek bir hak olarak tutar ve silme/yeniden oluşturma döngüsüyle sistemin
atlatılmasını engeller.

### Kural 3 — Slot Fiyatı Sabit Değil, Konfigürasyondan Okunur

Slot fiyatı her zaman `appsettings.json`'dan veya ayrı bir `AppConfig` tablosundan okunur.
Ödeme anındaki fiyat `BusinessSlot.PurchasePrice` alanına yazılır (fiyat sonradan değişse bile
geçmiş kayıtlar tutarlı kalır).

---

## Akış: Slot Satın Alma (MVP — Admin Onaylı)

```
1. Kullanıcı "Yeni işyeri ekle" butonuna basar
2. API → GET /api/slot/price → güncel fiyat döner (ör. 299 TL)
3. Kullanıcı ödeme yapacağını teyit eder
4. API → POST /api/slot/request → BusinessSlot oluşturulur (PaymentStatus = Pending)
5. Kullanıcı EFT yapar, dekont bilgisini girer (ExternalTransactionId)
6. Admin dekontunu doğrular
7. API → POST /api/admin/slot/{id}/approve → PaymentStatus = Completed, PaidAt = now
8. Kullanıcı artık yeni işyeri oluşturabilir
```

---

## Katman Bazlı Dosya Listesi

### Oluşturulacak Dosyalar

```
Core/Randevu365.Domain/
├── Entities/
│   └── BusinessSlot.cs                                     (yeni)
└── Enum/
    ├── SlotPaymentStatus.cs                                (yeni)
    └── SlotPaymentMethod.cs                                (yeni)

Infrastructure/Randevu365.Persistence/
└── Configurations/
    └── BusinessSlotConfiguration.cs                        (yeni)

Core/Randevu365.Application/Features/BusinessSlots/
├── Queries/
│   ├── GetMySlots/                                         (yeni)
│   │   ├── GetMySlotsQueryRequest.cs
│   │   ├── GetMySlotsQueryResponse.cs
│   │   └── GetMySlotsQueryHandler.cs
│   └── GetSlotPrice/                                       (yeni)
│       ├── GetSlotPriceQueryRequest.cs
│       ├── GetSlotPriceQueryResponse.cs
│       └── GetSlotPriceQueryHandler.cs
└── Commands/
    ├── RequestSlot/                                        (yeni)
    │   ├── RequestSlotCommandRequest.cs
    │   ├── RequestSlotCommandResponse.cs
    │   └── RequestSlotCommandHandler.cs
    └── ApproveSlot/          [Admin]                       (yeni)
        ├── ApproveSlotCommandRequest.cs
        ├── ApproveSlotCommandResponse.cs
        └── ApproveSlotCommandHandler.cs

Presentation/Randevu365.Api/Controllers/
└── SlotController.cs                                       (yeni)
```

### Değiştirilecek Dosyalar

```
Core/Randevu365.Domain/Entities/AppUser.cs
  → BusinessSlots navigation property eklenir

Core/Randevu365.Application/Features/Businesses/Commands/CreateBusiness/
  CreateBusinessCommandHandler.cs
  → Slot kota kontrolü eklenir

Infrastructure/Randevu365.Persistence/Context/AppDbContext.cs
  → DbSet<BusinessSlot> eklenir

Core/Randevu365.Application/Common/Responses/ApiResponse.cs
  → PaymentRequiredResult (HTTP 402) static metodu eklenir
```

---

## EF Konfigürasyonu

```csharp
// Infrastructure/Randevu365.Persistence/Configurations/BusinessSlotConfiguration.cs
public class BusinessSlotConfiguration : IEntityTypeConfiguration<BusinessSlot>
{
    public void Configure(EntityTypeBuilder<BusinessSlot> builder)
    {
        builder.Property(x => x.PurchasePrice).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(x => x.PaymentStatus).HasConversion<int>().IsRequired();
        builder.Property(x => x.PaymentMethod).HasConversion<int>();
        builder.Property(x => x.ExternalTransactionId).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasOne(x => x.AppUser)
            .WithMany(x => x.BusinessSlots)
            .HasForeignKey(x => x.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Slotun hangi işyerine harcandığı (nullable)
        builder.HasOne(x => x.UsedForBusiness)
            .WithOne()
            .HasForeignKey<BusinessSlot>(x => x.UsedForBusinessId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ödeme durumuna göre sık sorgulanan index
        builder.HasIndex(x => new { x.AppUserId, x.IsUsed, x.PaymentStatus });
    }
}
```

---

## Migration

```bash
dotnet ef migrations add AddBusinessSlotTable \
  --project Infrastructure/Randevu365.Persistence \
  --startup-project Presentation/Randevu365.Api
```

---

## Controller Yapısı

```csharp
[Route("api/slot")]
[ApiController]
public class SlotController : ControllerBase
{
    // Business Owner — güncel slot fiyatını öğren
    [HttpGet("price")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> GetSlotPrice() { ... }

    // Business Owner — slot satın alma talebi oluştur
    [HttpPost("request")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> RequestSlot(RequestSlotCommandRequest request) { ... }

    // Business Owner — kendi slotlarını listele
    [HttpGet("my")]
    [Authorize(Roles = Roles.BusinessOwner)]
    public async Task<IActionResult> GetMySlots() { ... }

    // Admin — ödemeyi onaylayıp slotu aktifleştir
    [HttpPost("{id}/approve")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> ApproveSlot(int id) { ... }
}
```

---

## `ApiResponse` Eklentisi

```csharp
// HTTP 402 Payment Required
public static ApiResponse<T> PaymentRequiredResult(string message = "Ödeme gerekli.")
    => new() { StatusCode = 402, IsSuccess = false, Message = message };
```

---

## Doğrulama Senaryoları

| Senaryo | Beklenen Sonuç |
|---|---|
| Yeni kayıt → 1. işyerini oluşturur | `201 Created` |
| Slotu olmayan kullanıcı → 2. işyerini oluşturmaya çalışır | `402 Payment Required` |
| Slot talebi oluşturulur (Pending) → işyeri oluşturmaya çalışır | `402` (henüz onaylanmadı) |
| Admin slotu onaylar → işyeri oluşturulur | `201 Created`, slot `IsUsed = true` olur |
| Bir işyerini siler → slot geri gelmez, başka işyeri eklemek için yeni slot lazım | `402` |
| 3. işyeri için 2. slotu alır, admin onaylar → 3. işyeri oluşturur | `201 Created` |

---

## Notlar

- **Slot fiyatı** ileride `appsettings.json` veya `AppConfig` tablosundan dinamik okunacak.
  Şimdilik `appsettings.json`'da sabit bir değer (ör. `"SlotPrice": 299.00`) tutulabilir.
- **Faz 2 — Ödeme Gateway:** `ExternalTransactionId` alanı Iyzico/Stripe işlem ID'si için
  hazır bırakıldı. Gateway entegrasyonu eklendiğinde admin onayı adımı otomatikleşir.
- **Slot iadesi:** Admin `SlotPaymentStatus = Refunded` yapabilir. İade edilen slotun
  `IsUsed = false` olup olmadığı kontrol edilmeli (kullanılmış slotun iadesi iş kararıdır).
