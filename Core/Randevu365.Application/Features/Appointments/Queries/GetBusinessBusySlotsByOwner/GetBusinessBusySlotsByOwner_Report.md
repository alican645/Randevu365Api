# İşletme Randevu Doluluk Raporu (GetBusinessBusySlotsByOwner)

Bu doküman, işletme sahibinin kendi işletmesine ait önümüzdeki 14 günlük dolu randevu saatlerini çekebileceği API endpoint'ini açıklar.

## 1. Genel Bilgiler
- **Endpoint:** `GET /api/business/appointments/{businessId}/busy-slots`
- **Yetki:** `BusinessOwner` rolüne sahip kullanıcılar.
- **Amacı:** İşletmenin önümüzdeki iki haftalık süreçteki onaylanmış veya tamamlanmış randevularının tarih ve saatlerini listeleyerek doluluk durumunu göstermek.

---

## 2. İstek Parametreleri (Request)

### URL Parametreleri
| Parametre | Tip | Açıklama |
| :--- | :--- | :--- |
| `businessId` | `int` | İşlem yapılacak işletmenin benzersiz kimliği. |

---

## 3. Yanıt Yapısı (Response)

Yanıt, `ApiResponse<GetBusinessBusySlotsByOwnerQueryResponse>` tipinde döner. Liste, tarihe ve başlangıç saatine göre sıralıdır.

### GetBusinessBusySlotsByOwnerQueryResponseItem
| Alan | Tip | Açıklama |
| :--- | :--- | :--- |
| `appointmentDate` | `string (Date)` | Randevunun tarihi (YYYY-MM-DD). |
| `approveStartTime` | `string (Time)` | Randevunun başlangıç saati (HH:mm:ss). |
| `approveEndTime` | `string (Time)` | Randevunun bitiş saati (HH:mm:ss). |

### Örnek Başarılı Yanıt
```json
{
  "isSuccess": true,
  "message": "İşlem başarılı.",
  "statusCode": 200,
  "data": {
    "items": [
      {
        "appointmentDate": "2026-03-05",
        "approveStartTime": "10:30:00",
        "approveEndTime": "11:00:00"
      },
      {
        "appointmentDate": "2026-03-05",
        "approveStartTime": "14:00:00",
        "approveEndTime": "15:00:00"
      }
    ]
  }
}
```

---

## 4. İş Mantığı ve Kurallar (Business Logic)

1.  **Zaman Aralığı:** Sistem, isteğin yapıldığı andan (`DateTime.UtcNow`) itibaren **14 günlük** (iki haftalık) bir periyodu tarar.
2.  **Güvenlik:**
    *   Kullanıcının giriş yapmış olması gerekir.
    *   Belirtilen `businessId`'ye sahip işletmenin gerçekten o kullanıcıya ait olması kontrol edilir. Aksi takdirde `403 Forbidden` döner.
3.  **Kapsanan Randevular:**
    *   Sadece durumu `Confirmed` (Onaylandı) veya `Completed` (Tamamlandı) olan randevular çekilir.
    *   Silinmiş (`IsDeleted: true`) randevular listeye dahil edilmez.
4.  **Saat Önceliği:**
    *   `ApproveStartTime` ve `ApproveEndTime` değerleri önceliklidir; eğer bunlar null ise `RequestedStartTime` ve `RequestedEndTime` değerleri kullanılır.
5.  **Sıralama:** Sonuçlar önce tarihe (`AppointmentDate`), ardından başlangıç saatine göre artan şekilde sıralanır.

---

## 5. Hata Durumları
- **401 Unauthorized:** Kullanıcı oturumu kapalıysa.
- **403 Forbidden:** İşletme başka bir kullanıcıya aitse.
- **404 Not Found:** İşletme bulunamadıysa.
