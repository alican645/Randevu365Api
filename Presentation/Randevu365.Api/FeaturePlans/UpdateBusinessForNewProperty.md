# Plan: Eş Zamanlı Müşteri Kapasitesi Özelliği

## Bağlam ve Problem

Şu an sistemde bir işletmenin aynı saat diliminde kaç kişiye hizmet verebileceğini tutan bir alan yok. Örneğin 3 berber koltuğu olan bir kuaför, saat 10:00–11:00 arasında aynı anda 3 farklı müşteriye randevu verebilmeli. Bu bilgi olmadan ileride geliştirilecek randevu sistemi, bir slot dolu mu boş mu anlayamaz.

## Tasarım Kararı: `Business` mı, `BusinessService` mi?

| Yaklaşım | Avantaj | Dezavantaj |
|---|---|---|
| `Business.MaxConcurrentCustomers` | Basit, tek alan | Tüm hizmetler için aynı kapasite — gerçekçi değil |
| `BusinessService.MaxConcurrentCustomers` | Hizmet başına kapasite (saç kesim: 3, masaj: 1) | Biraz daha fazla değişiklik |

**Karar: `BusinessService` üzerinde `MaxConcurrentCustomers` alanı.**

Her hizmet kendi kapasitesini taşır. Bu, ileriki randevu sisteminin "bu hizmet bu saatte dolu mu?" sorusunu doğru cevaplamasını sağlar. Varsayılan değer `1` olur (geriye uyumlu, mevcut kayıtları bozmaz).

---

## Değiştirilecek Dosyalar (Uygulama Sırası)

### 1. Domain — Entity Güncelle

**Dosya:** `Core/Randevu365.Domain/Entities/BusinessService.cs`

`MaxConcurrentCustomers` alanı ekle:

```csharp
public class BusinessService : BaseEntity
{
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; } = 1;  // YENİ

    public int BusinessId { get; set; }
    public virtual Business Business { get; set; } = null!;
}
```

---

### 2. Persistence — EF Konfigürasyonu Güncelle

**Dosya:** `Infrastructure/Randevu365.Persistence/Configurations/BusinessServiceConfiguration.cs`

`MaxConcurrentCustomers` için konfigürasyon ekle:

```csharp
builder.Property(x => x.MaxConcurrentCustomers)
    .IsRequired()
    .HasDefaultValue(1);
```

---

### 3. Persistence — Migration Oluştur

EF migration komutu:

```bash
dotnet ef migrations add BusinessServiceMaxConcurrentCustomersAdded \
  --project Infrastructure/Randevu365.Persistence \
  --startup-project Presentation/Randevu365.Api
```

Migration, `BusinessServices` tablosuna `MaxConcurrentCustomers integer NOT NULL DEFAULT 1` sütunu ekler.
Mevcut kayıtlar `1` değerini alır — **veri kaybı yok, breaking change yok.**

---

### 4. Application — CreateBusinessDetail Request Güncelle

**Dosya:** `Core/Randevu365.Application/Features/Businesses/Commands/CreateBusinessDetail/CreateBusinessDetailCommandRequest.cs`

`BusinessServiceCreateDto`'ya alan ekle:

```csharp
public class BusinessServiceCreateDto
{
    public string? ServiceTitle { get; set; }
    public string? ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; } = 1;  // YENİ
}
```

---


### 5. Application — CreateBusinessDetail Handler Güncelle

**Dosya:** `Core/Randevu365.Application/Features/Businesses/Commands/CreateBusinessDetail/CreateBusinessDetailCommandHandler.cs`

`BusinessService` oluşturulurken `MaxConcurrentCustomers` değerini aktar:

```csharp
var services = request.BusinessServices.Select(s => new BusinessService
{
    ServiceTitle = s.ServiceTitle!,
    ServiceContent = s.ServiceContent!,
    MaxConcurrentCustomers = s.MaxConcurrentCustomers,  // YENİ
    BusinessId = business.Id
}).ToList();
```

---

### 6. Application — UpdateBusinessDetail Request Güncelle

**Dosya:** `Core/Randevu365.Application/Features/Businesses/Commands/UpdateBusinessDetail/UpdateBusinessDetailCommandRequest.cs`

`BusinessServiceUpdateDto`'ya alan ekle:

```csharp
public class BusinessServiceUpdateDto
{
    public string? ServiceTitle { get; set; }
    public string? ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; } = 1;  // YENİ
}
```

---

### 7. Application — UpdateBusinessDetail Handler Güncelle

**Dosya:** `Core/Randevu365.Application/Features/Businesses/Commands/UpdateBusinessDetail/UpdateBusinessDetailCommandHandler.cs`

`BusinessService` oluşturulurken `MaxConcurrentCustomers` değerini aktar:

```csharp
var services = request.BusinessServices.Select(s => new BusinessService
{
    ServiceTitle = s.ServiceTitle!,
    ServiceContent = s.ServiceContent!,
    MaxConcurrentCustomers = s.MaxConcurrentCustomers,  // YENİ
    BusinessId = business.Id
}).ToList();
```

---

### 8. Application — GET Response DTO'larını Güncelle

#### 8a. İşletme sahibinin kendi detay bilgisi

**Dosya:** `Core/Randevu365.Application/Features/BusinessProfile/Queries/GetBusinessDetailInfoByCustomerOwnerId/GetBusinessDetailInfoByCustomerOwnerIdQueryResponse.cs`

```csharp
public class BusinessServiceDetailDto
{
    public required string ServiceTitle { get; set; }
    public required string ServiceContent { get; set; }
    public int MaxConcurrentCustomers { get; set; }  // YENİ
}
```

**Handler:** `GetBusinessDetailInfoByCustomerOwnerIdQueryHandler.cs`

```csharp
BusinessServices = business.BusinessServices?.Select(s => new BusinessServiceDetailDto
{
    ServiceTitle = s.ServiceTitle,
    ServiceContent = s.ServiceContent,
    MaxConcurrentCustomers = s.MaxConcurrentCustomers  // YENİ
}).ToList() ?? new List<BusinessServiceDetailDto>()
```

#### 8b. İşletme profili (müşteri görünümü)

`GetBusinessProfileByBusinessOwnerId` ve `GetBusinessProfileByUserId` handler'larında da `BusinessServiceDetailDto` varsa aynı şekilde güncelle.

---

## Migration / Altyapı Özeti

| Adım | Gerekli mi? | Açıklama |
|---|---|---|
| Yeni paket | Hayır | Mevcut bağımlılıklar yeterli |
| DI kaydı | Hayır | Yeni servis yok |
| Migration | **Evet** | `MaxConcurrentCustomers` sütunu ekler |
| Seed data | Hayır | DEFAULT 1 mevcut kayıtları karşılar |

---

## Doğrulama

1. `dotnet build` → 0 hata
2. Migration çalıştır: `dotnet ef database update`
3. Swagger → `POST /api/business/detail/create` isteğinde `BusinessServices[0].MaxConcurrentCustomers: 3` gönder, DB'de kaydı kontrol et
4. Swagger → `GET /api/business/detailinfo` → response'da `maxConcurrentCustomers` alanını doğrula
5. `MaxConcurrentCustomers` gönderilmeden oluşturulan hizmetlerin `1` değerini aldığını doğrula (geriye uyumluluk)
6. `PUT /api/business/detail/update` ile değeri güncelle, doğrula

---

## Notlar

- `MaxConcurrentCustomers` ileride randevu sisteminde şu şekilde kullanılacak: bir slot için aktif randevu sayısı `< MaxConcurrentCustomers` ise o slot müsait, aksi hâlde dolu.
- Validatörlere `MaxConcurrentCustomers >= 1` kuralı eklenebilir (opsiyonel ama önerilen).
- `BusinessHour` ile kombinasyon: "Bu saatte kaç slot boş?" hesaplaması — `(CloseTime - OpenTime) / ServiceDuration * MaxConcurrentCustomers`.
