# BusinessCategory — Frontend Entegrasyon Raporu

**Tarih:** 2026-02-23
**Hedef Kitle:** Frontend Geliştirici
**Kapsam:** `PUT /api/business/update-detail` — BusinessCategory alanı

---

## 1. Özet

`BusinessCategory` alanı, backend'de bir `enum` olarak saklanır ancak API'ye **Türkçe string** olarak gönderilir.
Sayısal (int) değer **gönderilmez**, kabul **edilmez**.

```
✅ "Kuaför"
❌ 1
❌ "Kuafor"   ← aksansız yazım geçersiz
```

---

## 2. Geçerli Değerler

API'ye gönderilebilecek tam ve harf duyarlı değerler:

| Gönderilecek String | Açıklama |
|---------------------|----------|
| `"Kuaför"` | Berber / Kuaför |
| `"Güzellik"` | Güzellik salonu, cilt bakımı |
| `"Sağlık"` | Sağlık klinikleri, poliklinik |
| `"Fitness"` | Spor salonu, pilates |
| `"Diş"` | Diş klinikleri |
| `"Masaj"` | Masaj & SPA |
| `"Veteriner"` | Veteriner klinikleri |

> **Önemli:** Değerler büyük/küçük harf ve Türkçe karakter dahil **birebir** eşleşmelidir.
> `"kuaför"`, `"KUAFÖR"`, `"Kuafor"` → geçersiz, `400` döner.

---

## 3. Endpoint Özeti

```
PUT /api/business/update-detail
Content-Type: multipart/form-data
Authorization: Bearer <token>
```

Bu endpoint dosya (logo, fotoğraf) de aldığı için `application/json` **değil**,
`multipart/form-data` kullanmak zorunludur.

---

## 4. BusinessCategory Alanının Davranışı

| Durum | Sonuç |
|-------|-------|
| Alan hiç gönderilmez | Mevcut kategori **değişmez** |
| `null` gönderilir | Mevcut kategori **değişmez** |
| Geçerli string gönderilir (`"Kuaför"` vb.) | Kategori güncellenir |
| Geçersiz string gönderilir (`"kuafor"`, `"Berber"` vb.) | `400 Bad Request` döner |

---

## 5. Request Örneği

### multipart/form-data (Postman)

```
businessId         = 3                   (Text)
businessName       = Alican Kuaförü      (Text)
businessAddress    = Bağcılar Mah. No:5  (Text)
businessCity       = İstanbul            (Text)
businessPhone      = 05551234567         (Text)
businessEmail      = alican@ornek.com    (Text)
businessCountry    = Türkiye             (Text)
businessCategory   = Kuaför              (Text)   ← bu alan
businessLogo       = <dosya>             (File)
```

### JavaScript / Fetch

```javascript
const formData = new FormData();
formData.append("businessId", "3");
formData.append("businessName", "Alican Kuaförü");
formData.append("businessAddress", "Bağcılar Mah. No:5");
formData.append("businessCity", "İstanbul");
formData.append("businessPhone", "05551234567");
formData.append("businessEmail", "alican@ornek.com");
formData.append("businessCountry", "Türkiye");
formData.append("businessCategory", "Kuaför"); // ← Türkçe string

// Opsiyonel: logo dosyası
if (logoFile) {
  formData.append("businessLogo", logoFile);
}

const response = await fetch("/api/business/update-detail", {
  method: "PUT",
  headers: {
    Authorization: `Bearer ${token}`,
    // Content-Type set etme! FormData otomatik boundary ekler.
  },
  body: formData,
});

const json = await response.json();
if (!json.success) {
  console.error(json.message || json.errors);
}
```

---

## 6. Response Örnekleri

### Başarılı (200 OK)

```json
{
  "success": true,
  "statusCode": 200,
  "message": "İşletme detayları başarıyla güncellendi.",
  "data": {
    "id": 3
  }
}
```

### Geçersiz Kategori (400 Bad Request)

```json
{
  "success": false,
  "statusCode": 400,
  "errors": [
    "Geçersiz kategori. Geçerli değerler: Kuaför, Güzellik, Sağlık, Fitness, Diş, Masaj, Veteriner"
  ]
}
```

---

## 7. TypeScript Tip Tanımları

```typescript
// Geçerli kategori değerleri
export type BusinessCategory =
  | "Kuaför"
  | "Güzellik"
  | "Sağlık"
  | "Fitness"
  | "Diş"
  | "Masaj"
  | "Veteriner";

// Dropdown için kullanılabilir liste
export const BUSINESS_CATEGORIES: BusinessCategory[] = [
  "Kuaför",
  "Güzellik",
  "Sağlık",
  "Fitness",
  "Diş",
  "Masaj",
  "Veteriner",
];

// Update request tipi
export interface UpdateBusinessDetailRequest {
  businessId: number;
  businessName: string;
  businessAddress: string;
  businessCity: string;
  businessPhone: string;
  businessEmail: string;
  businessCountry: string;
  businessCategory?: BusinessCategory; // opsiyonel
  businessLogo?: File;
  businessHours?: BusinessHourDto[];
  businessServices?: BusinessServiceDto[];
  businessPhotos?: File[];
  photoIdsToDelete?: number[];
}

export interface BusinessHourDto {
  day: string;
  openTime: string;
  closeTime: string;
}

export interface BusinessServiceDto {
  serviceTitle: string;
  serviceContent: string;
  maxConcurrentCustomers: number;
}
```

---

## 8. FormData Builder Yardımcı Fonksiyonu

```typescript
function buildUpdateBusinessFormData(
  request: UpdateBusinessDetailRequest
): FormData {
  const form = new FormData();

  form.append("businessId", String(request.businessId));
  form.append("businessName", request.businessName);
  form.append("businessAddress", request.businessAddress);
  form.append("businessCity", request.businessCity);
  form.append("businessPhone", request.businessPhone);
  form.append("businessEmail", request.businessEmail);
  form.append("businessCountry", request.businessCountry);

  if (request.businessCategory) {
    form.append("businessCategory", request.businessCategory);
  }

  if (request.businessLogo) {
    form.append("businessLogo", request.businessLogo);
  }

  request.businessHours?.forEach((h, i) => {
    form.append(`businessHours[${i}].day`, h.day);
    form.append(`businessHours[${i}].openTime`, h.openTime);
    form.append(`businessHours[${i}].closeTime`, h.closeTime);
  });

  request.businessServices?.forEach((s, i) => {
    form.append(`businessServices[${i}].serviceTitle`, s.serviceTitle);
    form.append(`businessServices[${i}].serviceContent`, s.serviceContent);
    form.append(
      `businessServices[${i}].maxConcurrentCustomers`,
      String(s.maxConcurrentCustomers)
    );
  });

  request.businessPhotos?.forEach((file) => {
    form.append("businessPhotos", file);
  });

  request.photoIdsToDelete?.forEach((id, i) => {
    form.append(`photoIdsToDelete[${i}]`, String(id));
  });

  return form;
}
```

---

## 9. Sık Yapılan Hatalar

| Hata | Neden | Çözüm |
|------|-------|-------|
| `400` — Geçersiz kategori | Aksansız yazıldı (`Kuafor`) | `"Kuaför"` olarak gönder |
| `400` — Geçersiz kategori | Küçük harf (`"kuaför"`) | İlk harf büyük olmalı |
| `400` — Geçersiz kategori | Int gönderildi (`1`) | String kullan (`"Kuaför"`) |
| Kategori hiç değişmiyor | Alan gönderilmedi | `businessCategory` key'ini forma ekle |
| `415 Unsupported Media Type` | `Content-Type: application/json` | `multipart/form-data` kullan, header'ı kendin set etme |

---

*Bu rapor `Presentation/Randevu365.Api/FeaturePlans/BusinessCategoryFrontendReport.md` konumunda saklanmaktadır.*
*Son güncelleme: 2026-02-23*
