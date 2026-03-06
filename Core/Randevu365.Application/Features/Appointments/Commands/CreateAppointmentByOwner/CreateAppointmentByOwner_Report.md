# İşletme Sahibi Tarafından Randevu Oluşturma Raporu (CreateAppointmentByOwner)

Bu doküman, işletme sahibinin kendi paneli üzerinden bir müşteri adına randevu oluşturmasını sağlayan API endpoint'ini açıklar.

## 1. Genel Bilgiler
- **Endpoint:** `POST /api/business/appointment/create-by-owner`
- **Yetki:** `BusinessOwner` rolüne sahip kullanıcılar.
- **Amacı:** İşletme sahibinin telefon, sosyal medya veya yüz yüze gelen randevu taleplerini sistem üzerine manuel olarak işlemesini sağlamak. Bu yöntemle oluşturulan randevular otomatik olarak **Onaylandı (Confirmed)** durumunda kaydedilir.

---

## 2. İstek Yapısı (Request Body)

| Parametre | Tip | Zorunlu mu? | Açıklama |
| :--- | :--- | :---: | :--- |
| `businessId` | `int` | Evet | Randevunun oluşturulacağı işletme ID'si. |
| `businessServiceId`| `int` | Evet | Verilecek hizmetin ID'si. |
| `appUserId` | `int` | Evet | Randevu oluşturulacak müşterinin (AppUser) ID'si. |
| `appointmentDate` | `string (Date)`| Evet | Randevu tarihi (YYYY-MM-DD). |
| `requestedStartTime`| `string (Time)`| Evet | Başlangıç saati (HH:mm:ss). |
| `requestedEndTime` | `string (Time)`| Evet | Bitiş saati (HH:mm:ss). |
| `customerNotes` | `string` | Hayır | Müşteri ile ilgili notlar. |
| `businessNotes` | `string` | Hayır | İşletmenin kendisine özel notları. |

### Örnek İstek (JSON)
```json
{
  "businessId": 5,
  "businessServiceId": 12,
  "appUserId": 25,
  "appointmentDate": "2026-03-10",
  "requestedStartTime": "14:00:00",
  "requestedEndTime": "15:00:00",
  "customerNotes": "Müşteri telefonda özel ricada bulundu.",
  "businessNotes": "Vip oda hazır edilecek."
}
```

---

## 3. Yanıt Yapısı (Response)

Yanıt, `ApiResponse<CreateAppointmentByOwnerCommandResponse>` tipinde döner.

### Örnek Başarılı Yanıt
```json
{
  "isSuccess": true,
  "message": "Randevu işletme sahibi tarafından başarıyla oluşturuldu ve onaylandı.",
  "statusCode": 201,
  "data": {
    "id": 150,
    "status": 1
  }
}
```
*(Status 1 = Confirmed)*

---

## 4. İş Mantığı ve Doğrulama Kuralları (Business Logic)

1.  **Yetki Kontrolü:** İstek yapan kullanıcının (`CurrentUserService.UserId`), belirtilen `businessId`'nin sahibi (`AppUserId`) olup olmadığı kontrol edilir. Başkasına ait işletme için randevu oluşturulamaz (`403 Forbidden`).
2.  **Müşteri Doğrulaması:** Belirtilen `appUserId` sistemde mevcut ve silinmemiş bir kullanıcı olmalıdır.
3.  **Hizmet Doğrulaması:** Seçilen `businessServiceId`'nin gerçekten o işletmeye ait olduğu kontrol edilir.
4.  **Tarih Kontrolü:** Geçmiş bir tarihe randevu oluşturulamaz.
5.  **Kapasite ve Çakışma Kontrolü:** 
    *   Aynı hizmet kolu için o tarih ve saat aralığındaki mevcut (Pending veya Confirmed) randevular sayılır.
    *   Sayı, hizmetin `MaxConcurrentCustomers` (aynı andaki maksimum müşteri) değerine ulaştıysa randevu oluşturulmaz (`409 Conflict`).
6.  **Otomatik Onay:** Randevu oluşturulurken `Status` alanı direkt olarak `Confirmed` (1) yapılır. Ayrıca `ApproveStartTime` ve `ApproveEndTime` alanları, talep edilen saatlerle otomatik olarak doldurulur.

---

## 5. Hata Senaryoları
- **400 Bad Request:** Eksik veri veya validasyon hatası (örn: bitiş saatinin başlangıçtan önce olması).
- **401 Unauthorized:** Kullanıcı girişi yapılmamış.
- **403 Forbidden:** İşletme size ait değil.
- **404 Not Found:** Müşteri, İşletme veya Hizmet bulunamadı.
- **409 Conflict:** Seçilen saat diliminde kapasite dolu.
