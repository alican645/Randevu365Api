# Slot Sistemi — Frontend Entegrasyon Raporu

**Tarih:** 2026-02-22
**Hedef Kitle:** Frontend Geliştirici
**Kapsam:** Slot satın alma, listeleme ve işletme oluşturma entegrasyonu

---

## İçindekiler

1. [Genel Bakış ve Kavram](#1-genel-bakış-ve-kavram)
2. [Önemli Değişiklik: Otomatik Onay](#2-önemli-değişiklik-otomatik-onay)
3. [Enum Referansları](#3-enum-referansları)
4. [API Endpoint'leri](#4-api-endpointleri)
- [GET /api/slot/price](#41-get-apislotprice)
- [POST /api/slot/request](#42-post-apislotrequest)
- [GET /api/slot/my](#43-get-apislotmy)
5. [Genel API Response Yapısı](#5-genel-api-response-yapısı)
6. [İş Akışları (User Flow)](#6-iş-akışları-user-flow)
7. [İşletme Oluşturma ile Entegrasyon](#7-işletme-oluşturma-ile-entegrasyon)
8. [HTTP Status Code Rehberi](#8-http-status-code-rehberi)
9. [Örnek Kod Parçacıkları](#9-örnek-kod-parçacıkları)
10. [UI/UX Önerileri](#10-uiux-önerileri)

---

## 1. Genel Bakış ve Kavram

Platform'da her işletme sahibi **ilk işletmesini ücretsiz** oluşturabilir. İkinci ve sonraki her işletme için önceden bir **slot** satın alınmış olması gerekir.

```
İlk işletme   →  Slot gerekmez, direkt oluşturulur
2. işletme    →  1 kullanılmamış slot gerekir
3. işletme    →  1 kullanılmamış slot gerekir
...
```

Slot bir "işletme oluşturma hakkı"dır. Satın alınır, bakiyede tutulur, işletme oluşturulurken tüketilir.

---

## 2. Önemli Değişiklik: Otomatik Onay

> **Bu sistem güncellenmiştir.** Eski dokümanlardaki "admin onayı" akışı artık geçerli değildir.

### Eski Davranış (Geçersiz)
```
RequestSlot → PaymentStatus: Pending → Admin onaylar → Completed → Kullanılabilir
```

### Yeni Davranış (Güncel)
```
RequestSlot → PaymentStatus: Completed → Direkt kullanılabilir
```

`POST /api/slot/request` isteği başarılı olduğunda slot **anında aktif** olur. Kullanıcıyı "onay bekleniyor" ekranına yönlendirmeye gerek yoktur. Satın alma tamamlandığında işletme oluşturma ekranına geçilebilir.

---

## 3. Enum Referansları

### SlotPackageType

| İsim | Değer (int) | Slot Adedi | Fiyat | Slot Başına |
|------|-------------|------------|-------|-------------|
| `Single` | `1` | 1 slot | 299,00 ₺ | 299,00 ₺ |
| `Triple` | `3` | 3 slot | 799,00 ₺ | ~266,33 ₺ |
| `Bundle5` | `5` | 5 slot | 1.299,00 ₺ | 259,80 ₺ |

> Fiyatlar `/api/slot/price` endpoint'inden dinamik olarak alınmalıdır. Hardcode edilmemelidir.

### SlotPaymentMethod

| İsim | Değer (int) | Açıklama |
|------|-------------|----------|
| `BankTransfer` | `1` | EFT / Havale |
| `CreditCard` | `2` | Kredi kartı |
| `Online` | `3` | Online ödeme gateway |

> `ExternalTransactionId`: Banka havalesi yapıldıysa dekont referans numarası buraya gönderilir. Diğer yöntemlerde opsiyoneldir.

### SlotPaymentStatus

| İsim | Değer (int) | Açıklama |
|------|-------------|----------|
| `Pending` | `0` | Beklemede (artık görülmemeli) |
| `Completed` | `1` | Aktif, kullanılabilir |
| `Failed` | `2` | Başarısız |
| `Refunded` | `3` | İade edildi |

---

## 4. API Endpoint'leri

Tüm endpoint'ler **JWT Bearer token** gerektirir. `BusinessOwner` rolü zorunludur.

```
Authorization: Bearer <token>
Content-Type: application/json
```

---

### 4.1 GET /api/slot/price

Mevcut paket fiyatlarını döner. Fiyat gösterimi için kullanılır.

**Request:** Parametresiz

**Response (200 OK):**

```json
{
"success": true,
"statusCode": 200,
"message": null,
"data": {
"packages": [
{
"packageType": 1,
"quantity": 1,
"totalPrice": 299.00,
"pricePerSlot": 299.00
},
{
"packageType": 3,
"quantity": 3,
"totalPrice": 799.00,
"pricePerSlot": 266.33
},
{
"packageType": 5,
"quantity": 5,
"totalPrice": 1299.00,
"pricePerSlot": 259.80
}
]
}
}
```

**Response Alanları:**

| Alan | Tür | Açıklama |
|------|-----|----------|
| `packageType` | `int` | Paket tipi enum değeri (1/3/5) |
| `quantity` | `int` | Pakette kaç slot var |
| `totalPrice` | `decimal` | Toplam paket fiyatı |
| `pricePerSlot` | `decimal` | Slot başına düşen fiyat |

---

### 4.2 POST /api/slot/request

Slot paketi satın alır. Başarılı olduğunda slotlar **anında aktif** (`Completed`) olur.

**Request Body:**

```json
{
"packageType": 3,
"paymentMethod": 1,
"externalTransactionId": "TXN-20260222-001"
}
```

| Alan | Tür | Zorunlu | Açıklama |
|------|-----|---------|----------|
| `packageType` | `int` | Evet | `1`, `3` veya `5` |
| `paymentMethod` | `int` | Evet | `1`, `2` veya `3` |
| `externalTransactionId` | `string` | Hayır | Dekont / referans numarası |

**Response (201 Created):**

```json
{
"success": true,
"statusCode": 201,
"message": "Slot paketi başarıyla satın alındı ve aktif edildi.",
"data": {
"packageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
"packageType": 3,
"quantity": 3,
"totalPrice": 799.00,
"pricePerSlot": 266.33,
"paymentStatus": 1
}
}
```

**Response Alanları:**

| Alan | Tür | Açıklama |
|------|-----|----------|
| `packageId` | `Guid` | Paket grubu ID'si (aynı paketteki tüm slotlar bu ID'yi paylaşır) |
| `packageType` | `int` | Satın alınan paket tipi |
| `quantity` | `int` | Oluşturulan slot adedi |
| `totalPrice` | `decimal` | Ödenen toplam tutar |
| `pricePerSlot` | `decimal` | Slot başına fiyat |
| `paymentStatus` | `int` | `1` (Completed) — slot kullanıma hazır |

**Hata Durumları:**

| HTTP | Açıklama |
|------|----------|
| `401` | Token eksik veya geçersiz |
| `403` | Kullanıcının `BusinessOwner` rolü yok |

---

### 4.3 GET /api/slot/my

Oturumdaki kullanıcının tüm slotlarını listeler.

**Request:** Parametresiz

**Response (200 OK):**

```json
{
"success": true,
"statusCode": 200,
"message": null,
"data": {
"slots": [
{
"id": 12,
"purchasePrice": 266.33,
"paymentStatus": 1,
"isUsed": false,
"paidAt": "2026-02-22T10:30:00Z",
"createdAt": "2026-02-22T10:30:00Z",
"packageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
"packageType": 3
},
{
"id": 13,
"purchasePrice": 266.33,
"paymentStatus": 1,
"isUsed": true,
"paidAt": "2026-02-22T10:30:00Z",
"createdAt": "2026-02-22T10:30:00Z",
"packageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
"packageType": 3
}
]
}
}
```

**SlotItemDto Alanları:**

| Alan | Tür | Açıklama |
|------|-----|----------|
| `id` | `int` | Slotun unique ID'si |
| `purchasePrice` | `decimal` | Satın alma anındaki slot başına fiyat |
| `paymentStatus` | `int` | `0`=Pending, `1`=Completed, `2`=Failed, `3`=Refunded |
| `isUsed` | `bool` | `true` ise bir işletmeye bağlanmış, kullanılmış |
| `paidAt` | `DateTime?` | Ödeme tamamlanma zamanı (UTC, null olabilir) |
| `createdAt` | `DateTime` | Slotun oluşturulma zamanı (UTC) |
| `packageId` | `Guid?` | Aynı paketten alınan slotları gruplamak için |
| `packageType` | `int?` | Hangi paketten alındığı |

**Frontend Filtreleme Önerileri:**

```
Kullanılabilir slot sayısı = paymentStatus == 1 (Completed) && isUsed == false
Kullanılmış slot sayısı   = isUsed == true
```

---

## 5. Genel API Response Yapısı

Tüm endpoint'ler aşağıdaki `ApiResponse<T>` wrapper'ını kullanır:

```json
{
"success": true | false,
"statusCode": 200 | 201 | 400 | 401 | 402 | 403 | 404 | 409,
"message": "İnsan okunabilir mesaj veya null",
"data": { ... } | null
}
```

Hata durumlarında `data` null, `success` false olur. Kullanıcıya gösterilecek mesaj için `message` alanını kullanın.

---

## 6. İş Akışları (User Flow)

### 6.1 İlk Kez Slot Satın Alma

```
1. GET /api/slot/price
→ Paket seçenekleri ve fiyatları göster

2. Kullanıcı paket seçer (Single / Triple / Bundle5)

3. Kullanıcı ödeme yöntemini seçer (BankTransfer / CreditCard / Online)
- BankTransfer seçtiyse: dekont referans numarası girilebilir (opsiyonel)

4. POST /api/slot/request
{
"packageType": <1|3|5>,
"paymentMethod": <1|2|3>,
"externalTransactionId": "<opsiyonel>"
}

5. Response 201 → paymentStatus: 1 (Completed)
→ "Slotunuz aktif edildi!" başarı bildirimi göster
→ İşletme oluşturma sayfasına yönlendir
```

### 6.2 İşletme Oluşturma (Slot Tüketimi)

```
1. Kullanıcı işletme oluşturmak ister

2. POST /api/business
- 1. işletme ise → direkt başarılı
- 2.+ işletme ise:
- Kullanılabilir slot varsa → işletme oluşturulur, slot tüketilir
- Kullanılabilir slot yoksa → 402 Payment Required döner

3. 402 gelirse → kullanıcıyı slot satın alma sayfasına yönlendir
```

### 6.3 Slot Bakiye Kontrolü

Kullanıcı profil sayfasında veya işletme oluşturma öncesinde slot bakiyesi gösterilebilir:

```
GET /api/slot/my
→ slots.filter(s => s.paymentStatus === 1 && !s.isUsed).length
→ "X kullanılabilir slotonuz var"
```

---

## 7. İşletme Oluşturma ile Entegrasyon

Slot sistemi `POST /api/business` endpoint'iyle doğrudan entegre çalışır. Frontend'in bilinmesi gereken davranış:

| Durum | API Yanıtı | Frontend Aksiyonu |
|-------|-----------|-------------------|
| İlk işletme oluşturma | `201 Created` | Başarı ekranı göster |
| Slot varken oluşturma | `201 Created` | Başarı ekranı göster |
| Slot yokken oluşturma | `402 Payment Required` | Slot satın alma sayfasına yönlendir |

**402 Response Örneği:**

```json
{
"success": false,
"statusCode": 402,
"message": "Yeni bir işletme oluşturmak için slot satın almanız gerekiyor.",
"data": null
}
```

---

## 8. HTTP Status Code Rehberi

Slot API'lerinden dönebilecek status code'ları ve anlamları:

| Code | Anlam | Tipik Senaryo |
|------|-------|---------------|
| `200` | Başarılı | GET istekleri |
| `201` | Oluşturuldu | RequestSlot başarılı |
| `401` | Yetkisiz | Token eksik / süresi dolmuş |
| `402` | Ödeme Gerekli | İşletme oluşturmada slot yetersiz |
| `403` | Yasak | Yanlış rol (BusinessOwner değil) |
| `404` | Bulunamadı | Geçersiz slot ID |
| `409` | Çakışma | Zaten onaylanmış slot tekrar onaylanmaya çalışıldı |

---

## 9. Örnek Kod Parçacıkları

### TypeScript — Tip Tanımları

```typescript
export enum SlotPackageType {
Single = 1,
Triple = 3,
Bundle5 = 5,
}

export enum SlotPaymentMethod {
BankTransfer = 1,
CreditCard = 2,
Online = 3,
}

export enum SlotPaymentStatus {
Pending = 0,
Completed = 1,
Failed = 2,
Refunded = 3,
}

export interface SlotPackageDto {
packageType: SlotPackageType;
quantity: number;
totalPrice: number;
pricePerSlot: number;
}

export interface SlotItemDto {
id: number;
purchasePrice: number;
paymentStatus: SlotPaymentStatus;
isUsed: boolean;
paidAt: string | null;
createdAt: string;
packageId: string | null;
packageType: SlotPackageType | null;
}

export interface RequestSlotRequest {
packageType: SlotPackageType;
paymentMethod: SlotPaymentMethod;
externalTransactionId?: string;
}

export interface RequestSlotResponse {
packageId: string;
packageType: SlotPackageType;
quantity: number;
totalPrice: number;
pricePerSlot: number;
paymentStatus: SlotPaymentStatus;
}
```

### TypeScript — API Çağrıları

```typescript
// Fiyatları getir
async function getSlotPrices(): Promise<SlotPackageDto[]> {
const res = await fetch('/api/slot/price', {
headers: { Authorization: `Bearer ${token}` },
});
const json = await res.json();
return json.data.packages;
}

// Slot satın al
async function requestSlot(payload: RequestSlotRequest): Promise<RequestSlotResponse> {
const res = await fetch('/api/slot/request', {
method: 'POST',
headers: {
Authorization: `Bearer ${token}`,
'Content-Type': 'application/json',
},
body: JSON.stringify(payload),
});

const json = await res.json();

if (!json.success) {
throw new Error(json.message ?? 'Slot satın alınamadı.');
}

// json.data.paymentStatus === 1 (Completed) — direkt aktif
return json.data;
}

// Slotlarımı listele
async function getMySlots(): Promise<SlotItemDto[]> {
const res = await fetch('/api/slot/my', {
headers: { Authorization: `Bearer ${token}` },
});
const json = await res.json();
return json.data.slots;
}

// Kullanılabilir slot sayısı
function getAvailableSlotCount(slots: SlotItemDto[]): number {
return slots.filter(
(s) => s.paymentStatus === SlotPaymentStatus.Completed && !s.isUsed
).length;
}
```

### cURL — Hızlı Test

```bash
# Fiyatları getir
curl -X GET http://localhost:5000/api/slot/price \
-H "Authorization: Bearer <TOKEN>"

# Triple paket satın al (BankTransfer)
curl -X POST http://localhost:5000/api/slot/request \
-H "Authorization: Bearer <TOKEN>" \
-H "Content-Type: application/json" \
-d '{
"packageType": 3,
"paymentMethod": 1,
"externalTransactionId": "EFT-20260222-001"
}'

# Single paket satın al (Online)
curl -X POST http://localhost:5000/api/slot/request \
-H "Authorization: Bearer <TOKEN>" \
-H "Content-Type: application/json" \
-d '{
"packageType": 1,
"paymentMethod": 3
}'

# Slotlarımı listele
curl -X GET http://localhost:5000/api/slot/my \
-H "Authorization: Bearer <TOKEN>"
```

---

## 10. UI/UX Önerileri

### Paket Seçim Ekranı

- `/api/slot/price` verisini fiyatlar için kullanın, hardcode yazmayın
- Tasarruf yüzdesini hesaplayıp gösterin: `Bundle5` Single'a göre ~%13 daha ucuz
- Önerilen paketi vurgulayın (`Triple` en popüler olabilir)

```
[ Single ]     [ Triple ⭐ ]     [ Bundle5 ]
1 Slot          3 Slot           5 Slot
299,00 ₺        799,00 ₺        1.299,00 ₺
     %11 Tasarruf     %13 Tasarruf
```

### Satın Alma Sonrası

- `paymentStatus === 1` (Completed) geldikten sonra kullanıcıya "Slotunuz aktif edildi!" toast/banner gösterin
- "Admin onayını bekliyorsunuz" tarzı mesaj **göstermeyin** — sistem artık anında aktif ediyor
- Başarı sonrası kullanıcıyı işletme oluşturma sayfasına yönlendirin

### Slot Bakiye Gösterimi

```
Kullanılabilir Slot: 2
[+ Slot Satın Al]
```

- `/api/slot/my` → `paymentStatus === 1 && !isUsed` filtresiyle gösterin
- İşletme oluşturma butonunun yanında veya profil sayfasında gösterilebilir

### 402 Hata Yönetimi

`POST /api/business` çağrısında `402` gelirse:

```
"Yeni işletme oluşturmak için slot gerekiyor."
[Slot Satın Al →]
```

Kullanıcıyı slot satın alma sayfasına yönlendiren bir eylem butonu sunun.

---

*Bu rapor `Presentation/Randevu365.Api/FeaturePlans/SlotSystemFrontendReport.md` konumunda saklanmaktadır.*
*Son güncelleme: 2026-02-22 — Otomatik Slot Onayı sistemi yansıtılmıştır.*
