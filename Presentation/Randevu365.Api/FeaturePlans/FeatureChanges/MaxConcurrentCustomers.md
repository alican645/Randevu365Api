# API Değişikliği: Eş Zamanlı Müşteri Kapasitesi (`MaxConcurrentCustomers`)

## Özet

`BusinessService` nesnesine `maxConcurrentCustomers` alanı eklendi. Bu alan, bir hizmetin aynı saat diliminde kaç farklı müşteriye eş zamanlı olarak verilebileceğini belirtir. Varsayılan değer `1`'dir.

---

## Etkilenen Endpointler

| Endpoint | Değişiklik Türü |
|---|---|
| `GET /api/business/detailinfo` | Response değişti — `businessServices[]` içine `maxConcurrentCustomers` eklendi |
| `POST /api/business/detail/create` | Request değişti — `businessServices[]` içine `maxConcurrentCustomers` eklendi |
| `POST /api/business/detail/update` | Request değişti — `businessServices[]` içine `maxConcurrentCustomers` eklendi |

---

## GET `/api/business/detailinfo` — Response Değişikliği

**Önce:**
```json
{
  "businessServices": [
    {
      "serviceTitle": "Saç Kesimi",
      "serviceContent": "Erkek saç kesimi"
    }
  ]
}
```

**Sonra:**
```json
{
  "businessServices": [
    {
      "serviceTitle": "Saç Kesimi",
      "serviceContent": "Erkek saç kesimi",
      "maxConcurrentCustomers": 3
    }
  ]
}
```

> `maxConcurrentCustomers` her zaman döner. Mevcut kayıtlar için değer `1`'dir.

---

## POST `/api/business/detail/create` & `/api/business/detail/update` — Request Değişikliği

`businessServices` dizisindeki her nesneye opsiyonel `maxConcurrentCustomers` alanı eklendi.

| Alan | Tip | Zorunlu | Varsayılan | Açıklama |
|---|---|---|---|---|
| `serviceTitle` | `string` | Evet | — | Hizmet adı |
| `serviceContent` | `string` | Evet | — | Hizmet açıklaması |
| `maxConcurrentCustomers` | `int` | Hayır | `1` | Aynı anda hizmet verilecek maksimum müşteri sayısı |

**Örnek request body:**
```json
{
  "businessName": "Örnek Kuaför",
  "businessServices": [
    {
      "serviceTitle": "Saç Kesimi",
      "serviceContent": "Erkek saç kesimi",
      "maxConcurrentCustomers": 3
    },
    {
      "serviceTitle": "Sakal Tıraşı",
      "serviceContent": "Sakal düzeltme",
      "maxConcurrentCustomers": 1
    }
  ]
}
```

---

## Geriye Uyumluluk

- **Mevcut kayıtlar:** Veritabanındaki tüm mevcut `BusinessService` kayıtları otomatik olarak `maxConcurrentCustomers = 1` değerini taşır.
- **İstek gönderirken:** `maxConcurrentCustomers` gönderilmezse sunucu `1` kabul eder — eski frontend kodu çalışmaya devam eder, breaking change yoktur.
- **Response okurken:** Alan artık her zaman mevcut olduğundan `null` kontrolüne gerek yoktur.

---

## Kullanım Senaryosu

Bu alan ileride randevu sistemi devreye girdiğinde şu şekilde kullanılacak:

```
Bir slot "müsait" sayılır:
  aktif randevu sayısı < maxConcurrentCustomers
```

Örneğin `maxConcurrentCustomers = 3` olan bir hizmet için saat 10:00–11:00 slotunda zaten 2 randevu varsa, 3. müşteri hâlâ randevu alabilir; 4. müşteri alamaz.

---

## Frontend Güncelleme Kontrol Listesi

- [ ] Hizmet oluşturma/düzenleme formuna "Eş Zamanlı Kapasite" input alanı ekle (sayısal, min: 1)
- [ ] GET response'tan `maxConcurrentCustomers` değerini okuyup forma doldur
- [ ] POST/PUT isteğinde `maxConcurrentCustomers` alanını gönder
- [ ] Alanı göndermezsen sunucu `1` kabul eder (opsiyonel alan)
