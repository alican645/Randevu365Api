# BusinessSlot Sistemi — Teknik Rapor

**Tarih:** 2026-02-21
**Kapsam:** `Core/Randevu365.Application/Features/BusinessSlots/` ve ilgili tüm katmanlar

---

## 1. Genel Bakış ve Amaç

BusinessSlot sistemi, platformdaki iş modeli kısıtlamasını teknik olarak uygulayan bileşendir.

**Kural:** Her işletme sahibi **ilk işletmesini ücretsiz** oluşturabilir. İkinci ve sonraki işletmeler için önce bir "slot" satın alması gerekir.

Bu yaklaşım tercih edilmesinin nedenleri:

- Ödeme işlemi (henüz gateway entegre edilmediğinden) manuel banka transferi/EFT üzerinden yapılmakta, admin onayı gerektirmektedir.
- Slot kavramı, ödemeyi işletme oluşturma adımından ayırarak iki aşamalı bir akış sağlar: önce ödeme talebi → admin onayı → işletme oluşturma.
- Gelecekte otomatik online ödeme entegrasyonu yapıldığında yalnızca `ApproveSlot` akışı değiştirilecek, geri kalan mimari aynı kalacaktır.

---

## 2. Domain Katmanı

### 2.1 `BusinessSlot` Entity

**Dosya:** `Core/Randevu365.Domain/Entities/BusinessSlot.cs`

```csharp
public class BusinessSlot : BaseEntity
{
    public int AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }

    public decimal PurchasePrice { get; set; }
    public SlotPaymentStatus PaymentStatus { get; set; } = SlotPaymentStatus.Pending;
    public SlotPaymentMethod PaymentMethod { get; set; }
    public string? ExternalTransactionId { get; set; }
    public DateTime? PaidAt { get; set; }

    public bool IsUsed { get; set; } = false;
    public int? UsedForBusinessId { get; set; }
    public virtual Business? UsedForBusiness { get; set; }
    public DateTime? UsedAt { get; set; }

    public string? Notes { get; set; }
}
```

| Alan | Tür | Amaç |
|---|---|---|
| `AppUserId` | `int` | Slotu satın alan kullanıcı (FK) |
| `PurchasePrice` | `decimal` | Satın alım anındaki fiyat (anlık config'den kopyalanır) |
| `PaymentStatus` | `SlotPaymentStatus` | Ödeme durumu (Pending/Completed/Failed/Refunded) |
| `PaymentMethod` | `SlotPaymentMethod` | Ödeme yöntemi (BankTransfer/CreditCard/Online) |
| `ExternalTransactionId` | `string?` | Banka dekontu referans numarası veya harici işlem ID |
| `PaidAt` | `DateTime?` | Admin onay zamanı (UTC) |
| `IsUsed` | `bool` | Slotun bir işletme için kullanılıp kullanılmadığı |
| `UsedForBusinessId` | `int?` | Slotun bağlandığı işletme ID (FK, opsiyonel) |
| `UsedAt` | `DateTime?` | İşletmeye bağlanma zamanı (UTC) |
| `Notes` | `string?` | Admin notu (onay/red gerekçesi) |

`BaseEntity`'den gelen alanlar: `Id`, `CreatedAt`, `UpdatedAt`, `IsDeleted`.

### 2.2 `SlotPaymentStatus` Enum

**Dosya:** `Core/Randevu365.Domain/Enum/SlotPaymentStatus.cs`

| Değer | Int | Anlam |
|---|---|---|
| `Pending` | 0 | Talep oluşturuldu, admin onayı bekleniyor |
| `Completed` | 1 | Admin onayladı, slot kullanılabilir |
| `Failed` | 2 | Ödeme başarısız (rezerve — henüz komut yok) |
| `Refunded` | 3 | İade edildi (rezerve — henüz komut yok) |

### 2.3 `SlotPaymentMethod` Enum

**Dosya:** `Core/Randevu365.Domain/Enum/SlotPaymentMethod.cs`

| Değer | Int | Anlam |
|---|---|---|
| `BankTransfer` | 1 | EFT / havale |
| `CreditCard` | 2 | Kredi kartı (fiziksel POS — henüz gateway yok) |
| `Online` | 3 | Online ödeme gateway (Iyzico/Stripe — henüz implement edilmedi) |

### 2.4 `AppUser.BusinessSlots` Navigation Property

**Dosya:** `Core/Randevu365.Domain/Entities/AppUser.cs` — satır 17

```csharp
public virtual ICollection<BusinessSlot> BusinessSlots { get; set; } = new List<BusinessSlot>();
```

`AppUser` ile `BusinessSlot` arasında 1:N ilişki kurulmuştur.

---

## 3. Persistence Katmanı

### 3.1 `BusinessSlotConfiguration`

**Dosya:** `Infrastructure/Randevu365.Persistence/Configurations/BusinessSlotConfiguration.cs`

```csharp
builder.Property(x => x.PurchasePrice).HasColumnType("decimal(10,2)").IsRequired();
builder.Property(x => x.PaymentStatus).HasConversion<int>().IsRequired();
builder.Property(x => x.PaymentMethod).HasConversion<int>();
builder.Property(x => x.ExternalTransactionId).HasMaxLength(200);
builder.Property(x => x.Notes).HasMaxLength(500);

builder.HasOne(x => x.AppUser)
    .WithMany(x => x.BusinessSlots)
    .HasForeignKey(x => x.AppUserId)
    .OnDelete(DeleteBehavior.Cascade);

builder.HasOne(x => x.UsedForBusiness)
    .WithOne()
    .HasForeignKey<BusinessSlot>(x => x.UsedForBusinessId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.SetNull);

builder.HasIndex(x => new { x.AppUserId, x.IsUsed, x.PaymentStatus });
```

**Dikkat noktaları:**

- `decimal(10,2)`: En fazla 10 basamak, 2 ondalık hane. Maksimum değer: 99.999.999,99
- Enum'lar `int` olarak saklanır (migration'larda sayısal değer görülür)
- `AppUser` cascade: Kullanıcı silinirse slotları da silinir
- `Business` set-null: İşletme silinirse `UsedForBusinessId` NULL olur; slot kaydı korunur ama artık hangi işletmeyle ilişkili olduğu bilinemez
- **Bileşik index** `(AppUserId, IsUsed, PaymentStatus)`: `CreateBusinessCommandHandler`'daki sorgu tam bu sırayla filtre kullandığından query performansı için kritik

### 3.2 `AppDbContext` Kaydı

**Dosya:** `Infrastructure/Randevu365.Persistence/Context/AppDbContext.cs` — satır 36

```csharp
public DbSet<BusinessSlot> BusinessSlots { get; set; }
```

Configuration'lar `Assembly`-scan ile otomatik uygulanır (`ApplyConfigurationsFromAssembly`).

### 3.3 Migration

**Dosya:** `Infrastructure/Randevu365.Persistence/Migrations/20260220111535_AddBusinessSlotTable.cs`

`BusinessSlots` tablosunu oluşturan migration. Bileşik index bu migration'da tanımlanmıştır.

---

## 4. Application Katmanı — Queries

### 4.1 `GetSlotPrice`

**Handler:** `Core/Randevu365.Application/Features/BusinessSlots/Queries/GetSlotPrice/GetSlotPriceQueryHandler.cs`

Veritabanı sorgusu yapmadan `IConfiguration` üzerinden fiyatı okur:

```csharp
var priceStr = _configuration["BusinessSlot:SlotPrice"];
var price = decimal.TryParse(priceStr, out var parsed) ? parsed : 0m;
```

- Config anahtarı: `BusinessSlot:SlotPrice`
- Mevcut değer (`appsettings.json`): `299.00`
- Config okunamazsa `0m` döner (sessiz fallback — dikkat edilmeli)
- İşlem DB'ye dokunmadığından senkron (`Task.FromResult`) döner

**Response:** `GetSlotPriceQueryResponse { Price: decimal }`

### 4.2 `GetMySlots`

**Handler:** `Core/Randevu365.Application/Features/BusinessSlots/Queries/GetMySlots/GetMySlotsQueryHandler.cs`

```csharp
var slots = await _unitOfWork.GetReadRepository<BusinessSlot>()
    .GetAllAsync(predicate: x => x.AppUserId == _currentUserService.UserId.Value && !x.IsDeleted);
```

- Soft-delete filtrelemesi yapılır (`!x.IsDeleted`)
- `ICurrentUserService` üzerinden oturumdaki kullanıcı belirlenir
- Tüm slotlar (Pending, Completed, Used) döner — client'ta state'e göre filtreleme yapılabilir

**Response içindeki `SlotItemDto` alanları:**

| Alan | Tür |
|---|---|
| `Id` | `int` |
| `PurchasePrice` | `decimal` |
| `PaymentStatus` | `SlotPaymentStatus` |
| `IsUsed` | `bool` |
| `PaidAt` | `DateTime?` |
| `CreatedAt` | `DateTime` |

---

## 5. Application Katmanı — Commands

### 5.1 `RequestSlot`

**Handler:** `Core/Randevu365.Application/Features/BusinessSlots/Commands/RequestSlot/RequestSlotCommandHandler.cs`

**Request alanları:**
- `PaymentMethod`: `SlotPaymentMethod` (zorunlu)
- `ExternalTransactionId`: `string?` (banka dekont no veya harici referans)

**Akış:**
1. `ICurrentUserService.UserId` null ise `401 Unauthorized`
2. `appsettings.json` üzerinden fiyat okunur
3. `BusinessSlot` oluşturulur: `PaymentStatus = Pending`, `IsUsed = false`
4. Kaydedilir
5. `201 Created` döner; mesaj: "Slot talebi oluşturuldu. Admin onayından sonra aktif olacaktır."

**Önemli:** Fiyat, talep anındaki config değerinden kopyalanır. Sonradan config değişse bile eski slot kaydındaki `PurchasePrice` korunur.

### 5.2 `ApproveSlot`

**Handler:** `Core/Randevu365.Application/Features/BusinessSlots/Commands/ApproveSlot/ApproveSlotCommandHandler.cs`

**Request alanları:**
- `SlotId`: `int` (route'dan gelir)
- `Notes`: `string?` (opsiyonel admin notu)

**Akış:**
1. `SlotId` ile slot aranır; soft-delete kontrolü yapılır
2. Bulunamazsa `404 Not Found`
3. Zaten `Completed` ise `409 Conflict`
4. `PaymentStatus = Completed`, `PaidAt = DateTime.UtcNow` atanır
5. `Notes` varsa kaydedilir
6. Kaydedilir
7. `200 OK` döner

**Kısıt:** Sadece `Administrator` rolü bu endpoint'i çağırabilir (controller seviyesinde `[Authorize(Roles = Roles.Administrator)]`).

---

## 6. Presentation Katmanı

**Controller:** `Presentation/Randevu365.Api/Controllers/SlotController.cs`
**Route prefix:** `api/slot`

| Method | Endpoint | Rol | Handler | Açıklama |
|---|---|---|---|---|
| `GET` | `/api/slot/price` | `BusinessOwner` | `GetSlotPriceQueryHandler` | Güncel slot fiyatını döner |
| `POST` | `/api/slot/request` | `BusinessOwner` | `RequestSlotCommandHandler` | Slot talebi oluşturur (Pending) |
| `GET` | `/api/slot/my` | `BusinessOwner` | `GetMySlotsQueryHandler` | Oturumdaki kullanıcının tüm slotlarını listeler |
| `POST` | `/api/slot/{id}/approve` | `Administrator` | `ApproveSlotCommandHandler` | Belirtilen slotu onaylar (Completed) |

`{id}` route parametresi, `ApproveSlotCommandRequest.SlotId`'ye atanır (controller satır 51).

---

## 7. İş Kuralları ve Akış

### 7.1 Slot Yaşam Döngüsü (State Machine)

```
                  RequestSlot
                      │
                      ▼
                  [Pending]
                 /    │    \
   ApproveSlot /     │     \ (henüz yok)
              ▼      │      ▼
         [Completed] │   [Failed]
              │       │
              │       │ (henüz yok)
              │       ▼
              │   [Refunded]
              │
              ▼ (CreateBusiness sırasında)
           IsUsed = true
           UsedForBusinessId = <business.Id>
           UsedAt = DateTime.UtcNow
```

Geçerli geçişler:
- `Pending` → `Completed` (ApproveSlot)
- `Pending` → `Failed` (henüz komut yok)
- `Completed` → `Refunded` (henüz komut yok)
- `Completed` → kullanılmış (`IsUsed = true`) — ApproveSlot değil, CreateBusiness akışı

### 7.2 İşletme Oluşturma Entegrasyonu

**Dosya:** `Core/Randevu365.Application/Features/Businesses/Commands/CreateBusiness/CreateBusinessCommandHandler.cs`

```
Mevcut işletme sayısı < 1  →  slot gerekmez, işletme oluşturulur
Mevcut işletme sayısı >= 1 →  Completed + IsUsed=false slot aranır
                                 Bulunamazsa → 402 Payment Required
                                 Bulunursa   → işletme oluşturulur,
                                               slot IsUsed=true yapılır
```

HTTP `402 Payment Required` dönmesi: `ApiResponse<T>.PaymentRequiredResult(...)` metodu kullanılmaktadır.

### 7.3 "Silinen İşletme Slot İade Etmez" Kuralı

`BusinessSlot → Business` ilişkisinde `OnDelete(DeleteBehavior.SetNull)` tanımlanmıştır. İşletme silindiğinde:
- `UsedForBusinessId` → `NULL`
- `IsUsed` → hâlâ `true`
- Slot yeniden kullanılabilir hale **gelmez**

Bu, kasıtlı bir iş kuralıdır: işletmeyi silmek slot iadesi anlamına gelmez.

### 7.4 Fiyat Konfigürasyonu

`appsettings.json`:
```json
"BusinessSlot": {
  "SlotPrice": 299.00
}
```

- `GetSlotPrice` ve `RequestSlot` handler'ları aynı config anahtarını okur
- Fiyat değişikliği yeni talepleri etkiler; eski `PurchasePrice` kayıtları dokunulmaz
- Config okunamazsa `0m` döner (sessiz fallback — production'da izlenmeli)

---

## 8. Veri Modeli Diyagramı

```
AppUser (1) ──────────────────── (N) BusinessSlot
  │                                        │
  │ Id                              AppUserId (FK)
  │                                 PurchasePrice
  │                                 PaymentStatus
  │                                 PaymentMethod
  │                                 ExternalTransactionId
  │                                 PaidAt
  │                                 IsUsed
  │                                 UsedAt
  │                                 Notes
  │                                        │
  │                                        │ UsedForBusinessId (FK, nullable)
  │                                        │ OnDelete: SetNull
  │                                        ▼
  └──────────── (N) Business (1) ──────────┘
                    Id
                    BusinessName
                    AppUserId (FK)
                    ...
```

- `AppUser` → `BusinessSlot`: 1:N (Cascade delete)
- `BusinessSlot` → `Business`: 1:1 opsiyonel (Set-null on delete)

---

## 9. Güvenlik

| Endpoint | Rol | Kontrol Seviyesi |
|---|---|---|
| `GET /api/slot/price` | `BusinessOwner` | Controller `[Authorize]` attribute |
| `POST /api/slot/request` | `BusinessOwner` | Controller `[Authorize]` attribute + Handler'da `UserId` null kontrolü |
| `GET /api/slot/my` | `BusinessOwner` | Controller `[Authorize]` attribute + Handler'da `UserId` null kontrolü |
| `POST /api/slot/{id}/approve` | `Administrator` | Controller `[Authorize(Roles = Roles.Administrator)]` |

**Kritik noktalar:**

- Slot onayı yalnızca `Administrator` rolüyle yapılabilir; `BusinessOwner` kendi slotunu onaylayamaz.
- `GetMySlots`, her kullanıcının yalnızca kendi slotlarını görmesini garantiler (`AppUserId == currentUserId` filtresi).
- `ApproveSlot` handler'da sahiplik kontrolü yoktur — herhangi bir slotu onaylayabilir. Bu admin rolüyle sınırlandırıldığından yeterlidir, ancak kötü niyetli admin vakası için ekstra kontrol düşünülebilir.

---

## 10. Eksikler / Geliştirme Önerileri

| Konu | Durum | Öneri |
|---|---|---|
| **Bildirim sistemi** | Yok | Slot onaylandığında kullanıcıya push notification veya e-posta gönderilmeli |
| **`FailSlot` komutu** | Yok | Admin reddettiğinde `Failed` state'e geçiş için komut eklenmeli |
| **`RefundSlot` komutu** | Yok | `Completed` slotların iade edilmesi için komut ve iş kuralı belirlenmeli |
| **Online ödeme gateway** | `Online` enum değeri var, implement yok | Iyzico veya Stripe entegrasyonu yapılırsa `ApproveSlot` otomatik tetiklenebilir |
| **Soft-delete sonrası slot iadesi politikası** | Belirsiz | İşletme silinince slot kullanılmış kalıyor (`IsUsed=true`); iş kararı netleştirilmeli |
| **Config fallback uyarısı** | Sessiz `0m` | `SlotPrice` okunamazsa `0m` fiyatla slot oluşuyor; log veya exception fırlatılmalı |
| **Sahiplik doğrulaması (GetMySlots)** | Mevcut | Tamam — `AppUserId` filtresi uygulanıyor |
| **Bileşik index sırası** | `(AppUserId, IsUsed, PaymentStatus)` | `CreateBusiness` sorgu filtresiyle birebir eşleşiyor, iyileştirme gerekmiyor |
