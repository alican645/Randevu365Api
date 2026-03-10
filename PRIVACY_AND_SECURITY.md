# Randevu365 - Gizlilik ve Güvenlik Politikası

## 1. Toplanan Kişisel Veriler

Randevu365 platformu, hizmet sunumu için aşağıdaki kişisel verileri toplamaktadır:

### Kullanıcı (Müşteri) Verileri
- **E-posta adresi** – Kimlik doğrulama, bildirimler ve iletişim amacıyla
- **Şifre** – Hesap güvenliği için (BCrypt algoritması ile hash'lenerek saklanır, düz metin olarak tutulmaz)
- **Ad ve Soyad** – Kullanıcı profili ve randevu tanımlaması için
- **Telefon numarası** – İletişim amacıyla

### İşletme Sahibi Verileri
- Kullanıcı verilerine ek olarak:
- **İşletme adı, adresi, şehri ve ülkesi** – İşletme profili ve arama sonuçları için
- **İşletme telefon numarası ve e-posta adresi** – Müşterilerle iletişim için
- **Konum bilgisi (enlem/boylam)** – Harita üzerinde gösterim ve yakınlık araması için
- **İşletme fotoğrafları ve logosu** – İşletme profil sayfasında görsel tanıtım için

### Randevu ve İşlem Verileri
- **Randevu tarihi ve saati** – Hizmet planlaması için
- **Randevu notları** – Müşteri ve işletme arasında bilgi paylaşımı için
- **Randevu durumu** (beklemede, onaylandı, iptal edildi vb.)
- **Mesajlaşma içerikleri** – Müşteri-işletme iletişimi için
- **Değerlendirme ve yorumlar** – Hizmet kalitesi geri bildirimi için
- **Favori işletmeler** – Kullanıcı tercihleri için

---

## 2. Kimlik Doğrulama ve Yetkilendirme

- **JWT (JSON Web Token)** tabanlı kimlik doğrulama sistemi kullanılmaktadır.
- Access token'lar **HMAC-SHA256** algoritması ile imzalanır.
- **Refresh token** mekanizması ile oturum yenileme desteklenir; refresh token'lar kriptografik olarak güvenli rastgele değerlerle üretilir.
- Token'lar süreli olup, belirlenen süre sonunda geçerliliğini yitirir.
- Kullanıcı rolleri (**Müşteri**, **İşletme Sahibi**, **Admin**) ile yetki tabanlı erişim kontrolü uygulanır.
- E-posta doğrulama kodu ile kayıt işlemi güvence altına alınır (kod 5 dakika geçerlidir).

---

## 3. Şifre Güvenliği

- Kullanıcı şifreleri **BCrypt** algoritması ile hash'lenerek veritabanında saklanır.
- Düz metin şifre hiçbir zaman veritabanında tutulmaz.
- **Şifre sıfırlama** işlemi, e-posta ile gönderilen tek kullanımlık doğrulama kodu üzerinden gerçekleştirilir (15 dakika geçerlidir).
- **Şifre değiştirme** endpoint'i yalnızca oturum açmış kullanıcılar tarafından erişilebilir.

---

## 4. API Güvenliği

### Rate Limiting (İstek Sınırlandırma)
- Genel API endpoint'leri: Dakikada **100 istek** ile sınırlıdır.
- Kimlik doğrulama endpoint'leri (login, register, forgot-password): Dakikada **10 istek** ile sınırlıdır.
- Limit aşımında HTTP **429 Too Many Requests** yanıtı döner.

### CORS (Cross-Origin Resource Sharing)
- Yapılandırılmış origin'ler ile CORS politikası uygulanır.
- Yalnızca izin verilen domainlerden gelen istekler kabul edilir.

### HTTPS
- Tüm HTTP istekleri **HTTPS**'e yönlendirilir, veri aktarımı şifreli olarak gerçekleşir.

---

## 5. Veri Saklama ve Silme

- **Soft delete** (yumuşak silme) mekanizması uygulanmaktadır; veriler fiziksel olarak silinmek yerine `IsDeleted` işareti ile pasif hale getirilir.
- Her kayıt için **oluşturulma tarihi** (`CreatedAt`) ve **güncellenme tarihi** (`UpdatedAt`) tutulur.
- Kullanıcılar hesap silme talebinde bulunabilir.

---

## 6. Denetim Kaydı (Audit Log)

- Veritabanı üzerinde yapılan tüm değişiklikler (oluşturma, güncelleme, silme) **denetim kaydı** altında tutulur.
- Denetim kaydında aşağıdaki bilgiler saklanır:
  - İşlemi yapan kullanıcı kimliği
  - İşlem türü (Create, Update, Delete)
  - Etkilenen tablo adı
  - İşlem zamanı
  - Eski ve yeni değerler
  - Değişen sütunlar

---

## 7. E-posta İletişimi

- E-posta gönderimi **SMTP over TLS** (StartTLS) ile şifreli bağlantı üzerinden gerçekleştirilir.
- E-posta aşağıdaki durumlarda gönderilir:
  - Kayıt sırasında e-posta doğrulama kodu
  - Şifre sıfırlama kodu
  - Randevu onay bildirimi
  - Randevu iptal bildirimi

---

## 8. Dosya Yükleme

- İşletme fotoğrafları ve logoları sunucuya yüklenir ve `/uploads` dizininde saklanır.
- Yüklenen dosyalar statik dosya sunucusu aracılığıyla erişime açılır.

---

## 9. Gerçek Zamanlı İletişim

- Müşteri ve işletme arasındaki mesajlaşma **SignalR** (WebSocket) teknolojisi ile gerçek zamanlı olarak gerçekleşir.
- Mesaj içerikleri veritabanında saklanır.

---

## 10. Veritabanı Güvenliği

- **PostgreSQL** veritabanı kullanılmaktadır.
- Veritabanı bağlantı bilgileri yapılandırma dosyalarında saklanır ve ortam değişkenleri ile yönetilir.
- Veritabanı sağlık kontrolü (health check) mekanizması mevcuttur.

---

## 11. Üçüncü Taraf Hizmetler

| Hizmet | Amaç | Paylaşılan Veri |
|--------|-------|-----------------|
| SMTP (Gmail vb.) | E-posta gönderimi | Alıcı e-posta adresi |
| PostgreSQL | Veri saklama | Tüm uygulama verileri |

---

## 12. Kullanıcı Hakları

Kullanıcılar aşağıdaki haklara sahiptir:

- **Erişim hakkı** – Profilim sayfasından kişisel verilerine erişebilir
- **Düzeltme hakkı** – Profil bilgilerini güncelleyebilir
- **Şifre değiştirme hakkı** – Mevcut şifresini değiştirebilir
- **Silme hakkı** – Hesap silme talebinde bulunabilir
- **Favori yönetimi** – Favori işletmelerini ekleyip kaldırabilir
- **Yorum ve değerlendirme** – Yaptığı yorum ve değerlendirmeleri yönetebilir

---

## 13. Güvenlik Özet Tablosu

| Güvenlik Önlemi | Durum |
|------------------|-------|
| Şifre hashleme (BCrypt) | Aktif |
| JWT kimlik doğrulama | Aktif |
| Refresh token mekanizması | Aktif |
| HTTPS yönlendirmesi | Aktif |
| Rate limiting | Aktif |
| CORS politikası | Aktif |
| E-posta doğrulama | Aktif |
| Audit log (denetim kaydı) | Aktif |
| Soft delete | Aktif |
| TLS ile e-posta gönderimi | Aktif |
| Rol tabanlı yetkilendirme | Aktif |

---

*Bu doküman, Randevu365 projesinin mevcut kod tabanı incelenerek oluşturulmuştur. Son güncelleme: Mart 2026*
