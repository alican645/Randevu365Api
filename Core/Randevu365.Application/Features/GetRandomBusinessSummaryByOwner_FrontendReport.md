# GetRandomBusinessSummaryByOwner — Frontend Entegrasyon Raporu

## Genel Bakış

Business owner'ın sahip olduğu işyerlerinden rastgele birini seçerek temel istatistiklerini
döndürür. Dashboard'un "özet kart" bölümünde kullanılmak üzere tasarlanmıştır.

---

## Endpoint

| Alan | Değer |
|---|---|
| **Method** | `GET` |
| **URL** | `/api/business/random-business-summary` |
| **Auth** | `Bearer token` — Rol: `BusinessOwner` |
| **Body** | Yok |

---

## İstek Örneği

```http
GET /api/business/random-business-summary
Authorization: Bearer <token>
```

---

## Başarılı Yanıt — `200 OK`

```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "businessId": 12,
    "businessName": "Örnek Kuaför",
    "averageRating": 4.35,
    "commentCount": 27,
    "logoUrl": "/uploads/business/12/logo/logo.png",
    "firstPhotoPath": "/uploads/business/12/photos/photo1.jpg",
    "appointmentCount": 143
  }
}
```

### Alan Açıklamaları

| Alan | Tip | Açıklama |
|---|---|---|
| `businessId` | `int` | İşyeri ID'si |
| `businessName` | `string` | İşyeri adı |
| `averageRating` | `decimal` | Tüm ratinglerin ortalaması; rating yoksa `0` döner |
| `commentCount` | `int` | Silinmemiş yorum sayısı |
| `logoUrl` | `string?` | Logo dosya yolu; logo eklenmemişse `null` |
| `firstPhotoPath` | `string?` | En eski aktif fotoğrafın yolu; fotoğraf yoksa `null` |
| `appointmentCount` | `int` | Silinmemiş toplam randevu sayısı |

---

## Hata Yanıtları

| HTTP | `statusCode` | `success` | Durum |
|---|---|---|---|
| `401` | `401` | `false` | Token eksik veya geçersiz |
| `404` | `404` | `false` | Kullanıcıya ait hiç işyeri yok |

### 401 Örneği
```json
{
  "success": false,
  "statusCode": 401,
  "message": "Yetkisiz erişim"
}
```

### 404 Örneği
```json
{
  "success": false,
  "statusCode": 404,
  "message": "Kayıt bulunamadı"
}
```

---

## Önemli Davranış Notları

### Rastgelelik
Kullanıcının birden fazla işyeri varsa, her istekte aralarından **rastgele biri** seçilir.
Dashboard'da "öne çıkan işyeri" efekti yaratmak için kullanılabilir.
Belirli bir işyerini getirmek için `/api/business/getbyid/{id}` tercih edilmeli.

### Null Alanlar
`logoUrl` ve `firstPhotoPath` alanları null gelebilir. Görseller için her zaman
fallback/placeholder kullanılmalı.

```tsx
// Örnek kullanım
<img src={data.logoUrl ?? '/images/default-logo.png'} alt={data.businessName} />
```

### `averageRating` Hassasiyeti
Backend `decimal` döner (örn. `4.35`). Yıldız gösterimi için `Math.round` veya
`toFixed(1)` kullanılabilir.

```tsx
const stars = Math.round(data.averageRating);      // tam yıldız
const label = data.averageRating.toFixed(1);        // "4.4"
```

---

## Öneri: Dashboard Kart Tasarımı

```
┌──────────────────────────────────────────┐
│  [LOGO]   Örnek Kuaför                   │
│           ★★★★½  4.4  (27 yorum)         │
│                                          │
│  [İlk Fotoğraf — 16:9 banner]            │
│                                          │
│  Toplam Randevu: 143                     │
└──────────────────────────────────────────┘
```

### Durum Yönetimi Önerisi

```tsx
const { data, isLoading, isError, error } = useQuery({
  queryKey: ['random-business-summary'],
  queryFn: () => api.get('/api/business/random-business-summary'),
});

if (isLoading) return <Skeleton />;
if (error?.statusCode === 404) return <EmptyBusinessState />;  // işyeri yok
if (isError) return <ErrorState />;
```

---

## Bağımlı Endpointler

| Amaç | Endpoint |
|---|---|
| İşyeri detayına git | `GET /api/business/getbyid/{businessId}` |
| Yorumları listele | `GET /api/business/.../comments` |
| Profil bilgilerini getir | `GET /api/business/myprofile` |
| Dashboard özeti | `GET /api/business/dashboard` |
