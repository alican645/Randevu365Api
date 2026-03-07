# Randevu365 API - Proje Roadmap & Eksik Analizi

## Context
Randevu365, kuaför/güzellik/sağlık/fitness gibi kategorilerdeki işletmelerin randevu yönetimini sağlayan bir API projesi. Clean Architecture (Domain, Application, Infrastructure, Persistence, Presentation) ile .NET 10, PostgreSQL, MediatR CQRS, JWT Auth, SignalR kullanılarak geliştirilmiş. Projenin temel iskeleti ve birçok özellik mevcut ancak production-ready olması için önemli eksikler var.

## Mevcut Durum Özeti
- **16 Domain Entity**, **6 Enum**, **39 CQRS Handler** (17 Command + 22 Query)
- **5 Controller**, **~35 endpoint**
- JWT Auth, SignalR Chat, File Upload, Audit Log, Seed Data mevcut
- PostgreSQL + EF Core 10, Generic Repository + UnitOfWork pattern

---

## ROADMAP - Kolaydan Zora

---

### FAZA 1: Proje Altyapısı & Hijyen (Kolay)

#### 1.1 .gitignore Eklenmesi
- [ ] Root dizine `.gitignore` ekle
- [ ] `bin/`, `obj/`, `.idea/`, `*.user`, `appsettings.Development.json`, `uploads/` klasörlerini ignore et
- [ ] Mevcut tracked binary dosyaları git cache'den temizle

#### 1.2 README.md Oluşturulması
- [ ] Proje açıklaması, kurulum adımları, API endpoint listesi
- [ ] Veritabanı kurulum talimatları (PostgreSQL)
- [ ] Ortam değişkenleri ve secret yönetimi bilgisi

#### 1.3 EntityBase.cs Temizliği
- [ ] `Core/Randevu365.Domain/Entities/Base/EntityBase.cs` boş dosya - kaldır veya kullan

#### 1.4 IAppDbContext Eksik DbSet'lerin Eklenmesi
- [ ] `IAppDbContext` şu anda sadece 5 DbSet içeriyor; Appointments, BusinessServices, BusinessSlots, BusinessHours, Conversations, Messages, AuditLogs, BusinessComments, BusinessRatings, BusinessLogos DbSet'leri eksik
- [ ] Dosya: `Core/Randevu365.Application/Interfaces/IAppDbContext.cs`

#### 1.5 JWT SecretKey Yapılandırması
- [ ] `appsettings.json` içinde `Jwt:SecretKey` alanı eksik
- [ ] User Secrets veya environment variable ile secret yönetimi dokümante et

---

### FAZA 2: Temel Eksik Özellikler (Kolay-Orta)

#### 2.1 Şifre Sıfırlama (ForgotPassword & ResetPassword)
- [ ] `Features/ForgotPassword/` - Boş klasör, implementasyon yok
- [ ] `Features/ResetPassword/` - Boş klasör, implementasyon yok
- [ ] ForgotPasswordCommandRequest/Handler: Email ile token gönderimi
- [ ] ResetPasswordCommandRequest/Handler: Token ile şifre güncelleme
- [ ] Gereksinim: Email servisi (bkz. Faza 4.1)

#### 2.2 Kullanıcı Profil Güncelleme
- [ ] AppUserInformation güncelleme endpoint'i yok
- [ ] Şifre değiştirme endpoint'i yok
- [ ] Profil fotoğrafı yükleme/güncelleme

#### 2.3 Müşteri Tarafı Randevu Yönetimi
- [ ] Müşterinin kendi randevularını listeleme query'si yok
- [ ] Müşterinin randevusunu iptal etme command'i yok (sadece business tarafı var)
- [ ] Randevu geçmişi görüntüleme

#### 2.4 Favoriler Sistemi
- [ ] `Features/Favorites/` - Boş klasör
- [ ] `Favorite` entity oluştur (AppUser + Business ilişkisi)
- [ ] Migration ekle
- [ ] AddFavorite, RemoveFavorite command'leri
- [ ] GetMyFavorites query'si
- [ ] CustomerController'a endpoint'ler ekle

#### 2.5 SoftDelete Implementasyonu
- [ ] `IWriteRepository<T>` interface'inde SoftDelete tanımlı ama `WriteRepository` implementasyonu eksik
- [ ] Dosya: `Infrastructure/Randevu365.Persistence/Repositories/WriteRepository.cs`
- [ ] Global query filter ile soft-deleted kayıtları otomatik filtrele

---

### FAZA 3: Validation & Cross-Cutting Concerns (Orta)

#### 3.1 MediatR Pipeline Behavior - FluentValidation Entegrasyonu
- [ ] `Application/Behaviors/` klasörü boş
- [ ] `ValidationBehavior<TRequest, TResponse>` pipeline behavior oluştur
- [ ] FluentValidation'ı MediatR pipeline'ına entegre et (DI registration)
- [ ] Handler'lardaki manuel validation kodlarını kaldır
- [ ] Dosya: `Core/Randevu365.Application/Registration.cs`

#### 3.2 Logging Pipeline Behavior
- [ ] `LoggingBehavior<TRequest, TResponse>` ekle
- [ ] Request/Response bilgilerini logla
- [ ] Execution süresini ölç

#### 3.3 Eksik Validator'lar
- [ ] ConfirmAppointment, RejectAppointment, CompleteAppointment handler'larında inline validation var - ayrı validator sınıflarına taşı
- [ ] CreateBusiness validator eksik
- [ ] Login/Register validator'larının kapsamını kontrol et

#### 3.4 Structured Logging (Serilog)
- [ ] Serilog NuGet paketi ekle
- [ ] Console + File sink konfigürasyonu
- [ ] Request/Response logging middleware
- [ ] Correlation ID desteği

---

### FAZA 4: Altyapı Servisleri (Orta)

#### 4.1 Email Servisi
- [ ] `IEmailService` interface tanımla (Application layer)
- [ ] SMTP veya SendGrid implementasyonu (Infrastructure layer)
- [ ] Randevu onay/red/iptal email bildirimleri
- [ ] Şifre sıfırlama email'i
- [ ] Email template sistemi

#### 4.2 SMS Servisi
- [ ] `ISmsService` interface tanımla
- [ ] Twilio veya NetGSM implementasyonu
- [ ] Randevu hatırlatma SMS'i
- [ ] Doğrulama kodu SMS'i

#### 4.3 Push Notification Servisi
- [ ] `INotificationService` interface tanımla
- [ ] Firebase Cloud Messaging (FCM) implementasyonu
- [ ] Randevu durumu değişikliğinde bildirim
- [ ] Yeni mesaj bildirimi

#### 4.4 RefreshToken Validasyonu
- [ ] `JwtService.ValidateRefreshToken()` metodu stub (null dönüyor)
- [ ] Dosya: `Infrastructure/Randevu365.Infrastructure/Services/JwtService.cs`
- [ ] Refresh token'ı DB'de saklama ve validasyon mantığı
- [ ] Token revocation (çıkış yapma)
- [ ] Refresh token rotation

---

### FAZA 5: İş Mantığı Geliştirmeleri (Orta-Zor)

#### 5.1 Randevu Çakışma Kontrolü
- [ ] Aynı saat aralığında MaxConcurrentCustomers sınırını aşan randevu oluşturulmasını engelle
- [ ] BusinessService'e göre süre hesaplama
- [ ] Müsait slot hesaplama algoritması geliştir

#### 5.2 İşletme Çalışma Saati Kontrolü
- [ ] Randevu oluşturulurken BusinessHour kontrolü yap
- [ ] Çalışma saati dışında randevu alınmasını engelle
- [ ] Tatil günleri / özel kapanış günleri desteği

#### 5.3 Admin Paneli Feature'ları
- [ ] `Features/Admin/` - Boş klasör
- [ ] İşletme onaylama/reddetme
- [ ] Kullanıcı engelleme/silme
- [ ] Sistem istatistikleri (toplam randevu, kullanıcı, işletme)
- [ ] Slot satış raporları
- [ ] Yorum moderasyonu

#### 5.4 Arama & Filtreleme Geliştirmeleri
- [ ] Full-text search desteği (PostgreSQL tsvector)
- [ ] Mesafe bazlı sıralama (PostGIS veya Haversine formülü ile)
- [ ] Çoklu filtre kombinasyonu (kategori + şehir + puan + mesafe)
- [ ] Autocomplete/suggestion endpoint'i

#### 5.5 Raporlama
- [ ] İşletme sahibi için gelir raporu
- [ ] Randevu istatistikleri (günlük/haftalık/aylık)
- [ ] Müşteri analizi (tekrar gelen müşteriler, iptal oranı)
- [ ] Doluluk oranı raporlaması

---

### FAZA 6: Mesajlaşma & İletişim (Orta-Zor)

#### 6.1 Mesajlaşma Feature'larının Tamamlanması
- [ ] `Features/Messaging/` - Boş klasör
- [ ] GetConversations query - Kullanıcının tüm konuşmalarını listele
- [ ] GetMessagesByConversation query - Konuşma detayı (paginasyonlu)
- [ ] SendMessage command (REST alternatifi, SignalR yanında)
- [ ] Okundu bilgisi (read receipt) desteği
- [ ] Mesaj silme

#### 6.2 Bildirim Sistemi
- [ ] `Features/Notifications/` - Boş klasör
- [ ] Notification entity oluştur
- [ ] In-app bildirim listesi
- [ ] Okundu/okunmadı durumu
- [ ] Bildirim tercihleri (hangi bildirimleri almak istiyor)

---

### FAZA 7: Güvenlik & Performans (Zor)

#### 7.1 Rate Limiting
- [ ] API rate limiting middleware ekle (.NET built-in veya AspNetCoreRateLimit)
- [ ] Endpoint bazlı limit tanımla (login, register özellikle)
- [ ] IP ve kullanıcı bazlı throttling

#### 7.2 CORS Politikası Sıkılaştırma
- [ ] `AllowAll` CORS politikası güvensiz - production için origin kısıtlaması ekle
- [ ] Dosya: `Presentation/Randevu365.Api/Program.cs`

#### 7.3 Input Sanitization
- [ ] XSS koruması - HTML encode
- [ ] SQL injection koruması (EF Core parametreli sorgular kullanıyor ama raw SQL varsa kontrol et)
- [ ] File upload güvenliği - dosya türü ve boyut kontrolü

#### 7.4 Caching
- [ ] Response caching (sık erişilen veriler: kategoriler, top-rated businesses)
- [ ] In-memory cache veya Redis entegrasyonu
- [ ] Cache invalidation stratejisi

#### 7.5 Health Checks
- [ ] `Infrastructure/Randevu365.Infrastructure/HealthChecks/` - Boş klasör
- [ ] Database health check
- [ ] External service health check'leri
- [ ] `/health` endpoint'i

#### 7.6 API Versioning
- [ ] Microsoft.AspNetCore.Mvc.Versioning paketi
- [ ] URL veya header bazlı versiyon yönetimi
- [ ] Eski endpoint'leri deprecate etme stratejisi

---

### FAZA 8: DevOps & Deployment (Zor)

#### 8.1 Docker Desteği
- [ ] `Dockerfile` oluştur (multi-stage build)
- [ ] `docker-compose.yml` (API + PostgreSQL + Redis)
- [ ] `.dockerignore` dosyası

#### 8.2 CI/CD Pipeline
- [ ] GitHub Actions workflow dosyası
- [ ] Build, test, lint adımları
- [ ] Otomatik deployment (staging/production)

#### 8.3 Environment Yönetimi
- [ ] `appsettings.Production.json`
- [ ] Secret management (Azure Key Vault / AWS Secrets Manager)
- [ ] Environment-specific connection string'ler

#### 8.4 Database Migration Stratejisi
- [ ] Migration'ları CI/CD'ye entegre et
- [ ] Rollback stratejisi belirle
- [ ] Seed data'yı environment'a göre ayır (dev vs prod)

---

### FAZA 9: Test & Kalite (Zor)

#### 9.1 Unit Test Projesi
- [ ] `Tests/Randevu365.UnitTests/` projesi oluştur
- [ ] Domain entity testleri
- [ ] Handler/Command testleri (mock repository ile)
- [ ] Validator testleri

#### 9.2 Integration Test Projesi
- [ ] `Tests/Randevu365.IntegrationTests/` projesi oluştur
- [ ] WebApplicationFactory ile API testleri
- [ ] In-memory veya TestContainers ile DB testleri
- [ ] Authentication/Authorization testleri

#### 9.3 Code Quality
- [ ] `.editorconfig` dosyası (kod stili kuralları)
- [ ] Analyzers (StyleCop, SonarAnalyzer)

---

### FAZA 10: İleri Seviye Özellikler (Çok Zor)

#### 10.1 Ödeme Sistemi Entegrasyonu
- [ ] Slot satışları için gerçek ödeme entegrasyonu (iyzico, Stripe)
- [ ] Ödeme callback/webhook handler
- [ ] Fatura oluşturma
- [ ] İade (refund) işlemleri

#### 10.2 Takvim Entegrasyonu
- [ ] Google Calendar API entegrasyonu
- [ ] iCal export desteği
- [ ] Otomatik takvim senkronizasyonu

#### 10.3 Çoklu Dil Desteği (i18n)
- [ ] API response mesajlarının çoklu dil desteği
- [ ] Şu anda tüm mesajlar Türkçe hardcoded
- [ ] Resource dosyaları ile lokalizasyon

#### 10.4 Background Job Sistemi
- [ ] Hangfire veya Quartz.NET entegrasyonu
- [ ] Randevu hatırlatma job'ları (1 saat önce, 1 gün önce)
- [ ] Otomatik randevu tamamlama (geçmiş randevular)
- [ ] Periyodik raporlama
- [ ] Expired slot temizliği

#### 10.5 Analytics & Monitoring
- [ ] Application Insights / Prometheus metrikleri
- [ ] Request tracing (OpenTelemetry)
- [ ] Dashboard (Grafana)
- [ ] Alerting (hata oranı, response time threshold)

---

## Öncelik Sıralaması (Önerilen)

| Sıra | Faza | Aciliyet | Etki |
|------|------|----------|------|
| 1 | Faza 1 - Altyapı Hijyen | Kritik | Temel |
| 2 | Faza 2.3 - Müşteri Randevu Yönetimi | Yüksek | Yüksek |
| 3 | Faza 3.1 - Validation Pipeline | Yüksek | Orta |
| 4 | Faza 4.4 - RefreshToken Fix | Yüksek | Güvenlik |
| 5 | Faza 5.1-5.2 - Çakışma & Saat Kontrolü | Yüksek | Yüksek |
| 6 | Faza 2.1 - Şifre Sıfırlama | Orta | Orta |
| 7 | Faza 7.2 - CORS Sıkılaştırma | Orta | Güvenlik |
| 8 | Faza 2.4 - Favoriler | Orta | Kullanıcı Deneyimi |
| 9 | Faza 4.1 - Email Servisi | Orta | Orta |
| 10 | Faza 6 - Mesajlaşma | Orta | Orta |
