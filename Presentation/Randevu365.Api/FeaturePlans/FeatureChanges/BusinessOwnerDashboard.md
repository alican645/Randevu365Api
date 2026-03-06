# API Değişikliği: Business Owner Dashboard (`GET /api/business/dashboard`)

## Özet

Business Owner için tek bir endpoint'ten sahip bilgilerini ve sahip olduğu tüm işletmelerin özet verisini dönen yeni bir dashboard sorgusu eklendi. Mevcut endpointler etkilenmedi — bu tamamen yeni bir endpoint'tir.

---

## Etkilenen Endpointler

| Endpoint | Değişiklik Türü |
|---|---|
| `GET /api/business/dashboard` | **Yeni endpoint** — BusinessOwner JWT ile çağrılır |

---

## GET `/api/business/dashboard`

### Auth
- **Rol:** `BusinessOwner`
- **JWT gerekli:** Evet (kimlik token'dan okunur, request body/param yoktur)

### Response — 200 OK

```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": null,
  "data": {
    "ownerName": "Ali",
    "ownerSurname": "Aydin",
    "ownerEmail": "ali@example.com",
    "ownerPhone": "05551234567",
    "businesses": [
      {
        "id": 7,
        "businessName": "Örnek Kuaför",
        "businessCity": "İstanbul",
        "latitude": 41.0082,
        "longitude": 28.9784,
        "averageRating": 4.3,
        "todayAppointmentCount": 3,
        "logoUrl": "uploads/business/7/logo/abc123_logo.jpg",
        "firstPhotoPath": "uploads/business/7/photos/xyz456_photo.jpg"
      },
      {
        "id": 12,
        "businessName": "İkinci Şube",
        "businessCity": "Ankara",
        "latitude": null,
        "longitude": null,
        "averageRating": 0,
        "todayAppointmentCount": 0,
        "logoUrl": null,
        "firstPhotoPath": null
      }
    ]
  }
}
```

### Response Alanları

#### Üst seviye (sahip bilgileri)

| Alan | Tip | Açıklama |
|---|---|---|
| `ownerName` | `string` | Kullanıcının adı (`AppUserInformation.Name`) |
| `ownerSurname` | `string` | Kullanıcının soyadı |
| `ownerEmail` | `string` | Giriş e-postası |
| `ownerPhone` | `string` | Telefon numarası |
| `businesses` | `array` | Kullanıcıya ait tüm aktif işletmeler |

#### `businesses[]` öğeleri

| Alan | Tip | Null olabilir | Açıklama |
|---|---|---|---|
| `id` | `int` | Hayır | İşletme ID'si |
| `businessName` | `string` | Hayır | İşletme adı |
| `businessCity` | `string` | Hayır | Şehir |
| `latitude` | `decimal` | **Evet** | Konum eklenmemişse `null` |
| `longitude` | `decimal` | **Evet** | Konum eklenmemişse `null` |
| `averageRating` | `decimal` | Hayır | Ortalama puan; hiç oy yoksa `0` |
| `todayAppointmentCount` | `int` | Hayır | Bugünkü `Pending` veya `Confirmed` randevu sayısı |
| `logoUrl` | `string` | **Evet** | Logo yüklenmemişse `null` |
| `firstPhotoPath` | `string` | **Evet** | Aktif fotoğraf yoksa `null` |

### Hata Durumları

| HTTP | Açıklama |
|---|---|
| `401` | JWT eksik veya geçersiz |
| `403` | Kullanıcı `BusinessOwner` rolünde değil |

---

## İş Kuralları

- `todayAppointmentCount`: Yalnızca **bugüne ait**, **silinmemiş**, statüsü `Pending (0)` veya `Confirmed (1)` olan randevular sayılır. `Cancelled` ve `Completed` randevular **dahil edilmez**.
- `averageRating`: İşletmenin tüm rating kayıtlarının ortalaması. Hiç rating yoksa `0` döner.
- `firstPhotoPath`: `IsActive = true` olan fotoğraflar arasından `Id`'ye göre sıralanmış ilk fotoğraf.
- `businesses`: Yalnızca `IsDeleted = false` olan işletmeler listelenir. Kullanıcının hiç işletmesi yoksa boş dizi (`[]`) döner.

---

## Frontend Güncelleme Kontrol Listesi

- [ ] Uygulama açıldığında / dashboard sayfasına girildiğinde `GET /api/business/dashboard` çağır
- [ ] `ownerName`, `ownerSurname`, `ownerEmail`, `ownerPhone` alanlarını profil bölümünde göster
- [ ] `businesses[]` dizisini iterasyonla kartlar halinde render et
- [ ] Her kart için:
  - [ ] `businessName` ve `businessCity` başlık olarak göster
  - [ ] `averageRating` yıldız/skor göstergesiyle göster (0 ise "Henüz değerlendirilmedi")
  - [ ] `todayAppointmentCount` rozet/sayaç olarak göster
  - [ ] `logoUrl` null kontrolü yap — null ise varsayılan placeholder göster
  - [ ] `firstPhotoPath` null kontrolü yap — arka plan/kapak görseli olarak kullan
  - [ ] `latitude` / `longitude` null kontrolü yap — harita widget'ı yalnızca dolu ise göster
- [ ] `businesses` dizisi boşsa "Henüz işletme eklenmedi" durumu göster
