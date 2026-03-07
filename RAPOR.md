# RANDEVU365 API - EKSIKLER VE YAPILMASI GEREKENLER RAPORU

> **Tarih:** 2026-03-07
> **Proje:** Randevu365 API - Randevu Yonetim Sistemi
> **Teknoloji:** .NET 10, PostgreSQL, EF Core 10, MediatR CQRS, JWT Auth, SignalR, Docker
> **Mimari:** Clean Architecture (Domain, Application, Infrastructure, Persistence, Presentation)

Bu rapor, projenin mevcut durumunu derinlemesine analiz ederek tum eksikleri, hatalari ve iyilestirme alanlarini **en onemsizden en onemliye** dogru siralar.

---

## MEVCUT DURUM OZETI

| Kategori | Miktar |
|----------|--------|
| Domain Entity | 16 (AppUser, Business, Appointment, BusinessService, vb.) |
| Enum | 6 (AppointmentStatus, BusinessCategory, Roles, SlotPackage/Payment) |
| Controller | 6 (Auth, Customer, Business, Admin, Message, Slot) |
| CQRS Handler | ~45 (Command + Query) |
| MediatR Behavior | 2 (Validation, Logging) |
| API Endpoint | ~40 |

---

## TODO LISTESI (En Onemsizden En Onemliye)

---

### ONCELIK 1 - DUSUK (Kozmetik & Kod Temizligi)

- [ ] **TODO-001** | `Randevu365.sln.DotSettings.user` dosyasi repository'de tracked durumda. Bu kullaniciya ozel IDE ayar dosyasi `.gitignore`'a eklenmeli ve git cache'den temizlenmeli.

- [ ] **TODO-002** | `Presentation/Randevu365.Api/Randevu365.Api.csproj` icinde bos `<Folder>` tanimlari var (`FeaturePlans/FeatureChanges/`, `plans/`). Bu klasorler ya kullanilmali ya da csproj'dan temizlenmeli.

- [ ] **TODO-003** | `CustomerController.cs` icinde `comment/delete` ve `rating/delete` endpoint'leri `[HttpPost]` ile tanimli. RESTful standartlara gore `[HttpDelete]` kullanilmali.
  - Dosya: `Presentation/Randevu365.Api/Controllers/CustomerController.cs:99,139`

- [ ] **TODO-004** | `CustomerController.cs` icinde `comment/update` endpoint'i `[HttpPost]` ile tanimli. RESTful standartlara gore `[HttpPut]` veya `[HttpPatch]` kullanilmali.
  - Dosya: `Presentation/Randevu365.Api/Controllers/CustomerController.cs:92`

- [ ] **TODO-005** | `BusinessController.cs` icinde `detail/update` ve `detail/create` gibi endpoint'ler `[HttpPost]` kullaniyorlar. RESTful uyumlu HTTP metodlari tercih edilmeli (`PUT`, `PATCH`, `POST`).
  - Dosya: `Presentation/Randevu365.Api/Controllers/BusinessController.cs:138,145`

- [ ] **TODO-006** | `BusinessHour` entity'sinde `Day`, `OpenTime`, `CloseTime` alanlari `string` tipinde. `Day` icin `DayOfWeek` enum, saat alanlari icin `TimeOnly` tipi kullanilmali. Bu, gecersiz veri girisini onler ve tip guvenligini arttirir.
  - Dosya: `Core/Randevu365.Domain/Entities/BusinessHour.cs:8-10`

- [ ] **TODO-007** | `WriteRepository.UpdateAsync()` ve `HardDeleteAsync()` metotlarinda senkron islemler `Task.Run()` ile sarmalanmis. Bu anti-pattern gereksiz thread pool kullaniyor. Dogrudan senkron islem yapilip `Task.CompletedTask` dondurulebilir veya metot senkron hale getirilebilir.
  - Dosya: `Infrastructure/Randevu365.Persistence/Repositories/WriteRepository.cs:28-35`

- [ ] **TODO-008** | `FileService.DeleteFileAsync()` icinde `Task.FromResult()` gereksiz kullaniliyor. Metot zaten senkron islem yapiyor.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/FileService.cs:37-48`

- [ ] **TODO-009** | Tum API response mesajlari Turkce hardcoded. Coklu dil (i18n) destegi icin resource dosyalari ile lokalizasyon altyapisi kurulmali.
  - Etkilenen: Tum Handler dosyalari

- [ ] **TODO-010** | `MockDataUsers.md` dosyasi repository'de test verisi iceriyor. Bu, daha duzgun bir dokumantasyon yapisiyla yonetilmeli veya README'ye tasinmali.

---

### ONCELIK 2 - DUSUK-ORTA (Kod Kalitesi & Standartlar)

- [ ] **TODO-011** | `.editorconfig` dosyasi eksik. Takim genelinde tutarli kod stili icin editorconfig olusturulmali (tab/space, satir sonu, naming conventions vb.).

- [ ] **TODO-012** | Code analyzer'lar (StyleCop, SonarAnalyzer, Roslynator) kullanilmiyor. Statik kod analizi icin NuGet paketleri eklenmeli.

- [ ] **TODO-013** | `Swagger/OpenAPI` yalnizca Development ortaminda aktif (`Program.cs:155-163`). Staging ortami icin de erisim saglanmali veya ayri bir konfigurasyonla yonetilmeli.

- [ ] **TODO-014** | `AppUserInformation` entity'sinde `Height` ve `Weight` alanlari mevcut. Bir randevu sistemi icin bu alanlar gereksiz gorunuyor. Is gereksinimleri dogrultusunda kaldirilamasi veya opsiyonel tutulmasi degerlendirilmeli.
  - Dosya: `Infrastructure/Randevu365.Persistence/Seeds/SeedData.cs:29-40`

- [ ] **TODO-015** | Seed data'da tum business'ler ayni sehirde ("Istanbul") ve ayni ulkede ("Turkiye") olusturuluyor. Daha gercekci ve cesitli test verisi uretilmeli.
  - Dosya: `Infrastructure/Randevu365.Persistence/Seeds/SeedData.cs:64-98`

- [ ] **TODO-016** | `BusinessCategory` enum'unda kategori sayisi sinirli (7 adet). Yeni is alanlari icin genisletilebilir bir yapi (ornegin veritabaninda dinamik kategori tablosu) dusunulmeli.
  - Dosya: `Core/Randevu365.Domain/Enum/BusinessCategory.cs`

- [ ] **TODO-017** | `GetAllBusinessesQueryRequest` paginasyon destegi olmadan tum isletmeleri getiriyor. Buyuk veri setlerinde performans sorunu yaratir.
  - Dosya: `Core/Randevu365.Application/Features/Businesses/Queries/GetAllBusinesses/`

- [ ] **TODO-018** | `AdminController.GetAllUsers()` paginasyon, filtreleme veya siralama destegi olmadan tum kullanicilari getiriyor. N+1 sorgu riski ve bellek sorunlari olusturabilir.
  - Dosya: `Presentation/Randevu365.Api/Controllers/AdminController.cs:28-32`

- [ ] **TODO-019** | `BusinessController` icinde URL route tasarimi tutarsiz. `appointments/{businessId}/{onlyConfirmed}/confirmed` seklinde path parametresi olarak boolean deger geciliyor. Query parameter (`?onlyConfirmed=true`) daha uygun olur.
  - Dosya: `Presentation/Randevu365.Api/Controllers/BusinessController.cs:183`

---

### ONCELIK 3 - ORTA (Eksik Ozellikler - Temel Islevsellik)

- [ ] **TODO-020** | **Isletme Guncelleme (Update Business) endpoint'i eksik.** `CreateBusiness` command var ama isletmenin temel bilgilerini (ad, adres, telefon, kategori vb.) guncelleyen bir endpoint yok.
  - Gerekli: `UpdateBusinessCommandRequest/Handler/Validator`

- [ ] **TODO-021** | **Isletme Silme (Delete Business) endpoint'i eksik.** Bir isletme sahibi kendi isletmesini silemez (soft delete).
  - Gerekli: `DeleteBusinessCommandRequest/Handler`

- [ ] **TODO-022** | **BusinessService (Hizmet) CRUD islemleri eksik.** Isletmeye ait hizmetleri ekleme, guncelleme, silme ve listeleme endpoint'leri yok. Hizmetler sadece migration/seed ile olusturulabiliyor.
  - Gerekli: `CreateBusinessService`, `UpdateBusinessService`, `DeleteBusinessService`, `GetBusinessServicesByBusinessId`

- [ ] **TODO-023** | **BusinessHour (Calisma Saati) CRUD islemleri eksik.** Isletmenin calisma saatlerini ekleme, guncelleme ve listeleme endpoint'leri yok.
  - Gerekli: `CreateBusinessHour`, `UpdateBusinessHour`, `DeleteBusinessHour`, `GetBusinessHours`

- [ ] **TODO-024** | **Kullanici Hesap Silme endpoint'i eksik.** KVKK/GDPR uyumlulugu icin kullanicilarin kendi hesaplarini silebilmesi gerekir.
  - Gerekli: `DeleteAccountCommandRequest/Handler`

- [ ] **TODO-025** | **Isletme Onaylama/Reddetme (Admin) endpoint'i eksik.** Admin panelinde isletme onaylama/reddetme mekanizmasi yok. `Business` entity'sinde `IsApproved` gibi bir alan eksik.
  - Etki: Herkes isletme olusturup yayinlayabiliyor, moderasyon yok.

- [ ] **TODO-026** | **Kullanici Engelleme/Banlama (Admin) endpoint'i eksik.** Kotu niyetli kullanicilari engellemek icin admin tarafinda kullanici yonetim API'leri yok.
  - Gerekli: `BanUserCommand`, `UnbanUserCommand`

- [ ] **TODO-027** | **Yorum Moderasyonu (Admin) eksik.** Admin, uygunsuz yorumlari silebilmeli veya gizleyebilmeli. Mevcut AdminController sadece yorumlari listeliyor.
  - Dosya: `Presentation/Randevu365.Api/Controllers/AdminController.cs:43-48`

- [ ] **TODO-028** | **Mesaj Silme endpoint'i eksik.** Kullanicilar gonderdikleri mesajlari silemez.
  - Dosya: `Presentation/Randevu365.Api/Controllers/MessageController.cs`

- [ ] **TODO-029** | **Okundu Bilgisi (Read Receipt) mesajlasma sisteminde eksik.** `Message` entity'sinde `IsRead` alani veya okunma zamani takibi yok.
  - Dosya: `Core/Randevu365.Domain/Entities/Message.cs`

---

### ONCELIK 4 - ORTA-YUKSEK (Is Mantigi Eksikleri & Hatalar)

- [ ] **TODO-030** | **Randevu olusturulurken calisma saati kontrolu yapilmiyor.** `CreateAppointmentCommandHandler` isletmenin calisma saatlerini (`BusinessHour`) kontrol etmiyor. Gece 3:00'te randevu alinabilir durumda.
  - Dosya: `Core/Randevu365.Application/Features/Appointments/Commands/CreateAppointment/CreateAppointmentCommandHandler.cs:20-76`
  - Oneri: Randevu baslangic/bitis saatleri ilgili gunun `OpenTime`/`CloseTime` araligiyla karsilastirilmali.

- [ ] **TODO-031** | **Randevu olusturulurken hizmet suresi kontrolu eksik.** Musterinin talep ettigi zaman araligi (`RequestedStartTime` - `RequestedEndTime`) ile `BusinessService`'in fiili suresi arasinda uyum kontrolu yapilmiyor. Musteri 5 dakikalik bir hizmet icin 3 saatlik randevu olusturabilir.
  - Dosya: `Core/Randevu365.Application/Features/Appointments/Commands/CreateAppointment/CreateAppointmentCommandHandler.cs`
  - Oneri: `BusinessService`'e `DurationMinutes` alani eklenmeli.

- [ ] **TODO-032** | **ForgotPassword handler'inda `RefreshToken` alani password reset token olarak kullaniliyor.** Ayni alan hem JWT refresh token hem de sifre sifirlama tokeni icin kullanildigi icin, sifre sifirlama istegi yapildiginda kullanicinin mevcut oturumu gecersiz kalir.
  - Dosya: `Core/Randevu365.Application/Features/ForgotPassword/ForgotPasswordCommandHandler.cs:29-30`
  - Oneri: `AppUser` entity'sine ayri `PasswordResetToken` ve `PasswordResetTokenExpiry` alanlari eklenmeli.

- [ ] **TODO-033** | **ResetPassword handler'inda ayni sorun mevcut.** Sifre sifirlandiktan sonra `RefreshToken = null` yapilarak hem reset token hem de refresh token siliniyor.
  - Dosya: `Core/Randevu365.Application/Features/ResetPassword/ResetPasswordCommandHandler.cs:27-28`

- [ ] **TODO-034** | **Ayni musterinin ayni isletmede ayni saatte birden fazla randevu olusturmasi engellenmemiyor.** `CreateAppointmentCommandHandler`'da `AppUserId` bazli tekrar kontrolu yok.
  - Dosya: `Core/Randevu365.Application/Features/Appointments/Commands/CreateAppointment/CreateAppointmentCommandHandler.cs:43-49`

- [ ] **TODO-035** | **Randevu onaylanirken (`ConfirmAppointment`) email bildirimi gonderilmiyor.** `IEmailService` enjekte edilmemis, `SendAppointmentConfirmationEmailAsync` cagirilmiyor.
  - Dosya: `Core/Randevu365.Application/Features/Appointments/Commands/ConfirmAppointment/ConfirmAppointmentCommandHandler.cs`

- [ ] **TODO-036** | **Randevu iptal/reddedilirken email bildirimi gonderilmiyor.** Cancel ve Reject handler'larinda `SendAppointmentCancellationEmailAsync` cagirilmiyor.

- [ ] **TODO-037** | **Gecmis tarihli randevu iptal edebilme hatasi.** `CancelAppointmentCommandHandler`'da randevunun tarihinin gecmis olup olmadiginin kontrolu yapilmiyor.

- [ ] **TODO-038** | **Tamamlanmis/Reddedilmis/Iptal edilmis randevuya tekrar islem yapilabilme riski.** Bazi handler'larda status kontrolu eksik veya yetersiz.

- [ ] **TODO-039** | **Otomatik randevu tamamlama mekanizmasi yok.** Gecmis tarihli `Confirmed` randevular otomatik olarak `Completed` durumuna gecmiyor. Background job ile periyodik kontrol gerekli.

- [ ] **TODO-040** | **`BusinessService` entity'sinde hizmet suresi (`DurationMinutes`) alani eksik.** Bu alan olmadan musait zaman dilimlerini dogru hesaplamak mumkun degil.
  - Dosya: `Core/Randevu365.Domain/Entities/BusinessService.cs`

- [ ] **TODO-041** | **Slot satin alma isleminde gercek odeme entegrasyonu yok.** `RequestSlotCommandHandler` slot olusturuyor ama odeme dogrulamasi yapmadan `PaymentStatus` degistirilebilir durumda.
  - Etki: Sahte odeme ile slot elde edilebilir.

- [ ] **TODO-042** | **Kullanici kayit (Register) handler'inda kullanicinin rolunu kendisinin belirlemesine izin veriliyor.** `request.Role` direkt olarak `AppUser` entity'sine ataniyor. Kullanici kendini `Administrator` roluyle kaydedebilir.
  - Dosya: `Core/Randevu365.Application/Features/Register/RegisterHandler.cs:43`
  - KRITIK GUVENLIK ACIGI

- [ ] **TODO-043** | **Yorum yapabilmek icin o isletmeden randevu almis olma zorunlulugu yok.** Herhangi bir `Customer` rolu olan kullanici, hic gitmedigi isletmeye yorum/puan verebilir.
  - Dosya: `Core/Randevu365.Application/Features/BusinessComments/Commands/AddBusinessComment/`

---

### ONCELIK 5 - YUKSEK (Guvenlik Sorunlari)

- [ ] **TODO-044** | **KRITIK: Register endpoint'inde rol atama acigi.** Kullanici kayit olurken `Role` alanini kendisi seciyor (`request.Role`). Bu, herhangi birinin kendini `Administrator` yapabilmesine izin veriyor. Kayit sirasinda rol ya sabit `Customer` olmali ya da admin tarafindan atanmali.
  - Dosya: `Core/Randevu365.Application/Features/Register/RegisterHandler.cs:43`
  - Aciliyet: **KRITIK**

- [ ] **TODO-045** | **Register endpoint'inde input validation (FluentValidator) eksik.** Email format kontrolu, sifre gucluluyu, zorunlu alan kontrolleri icin validator sinifi yok.
  - Gerekli: `RegisterValidator.cs`

- [ ] **TODO-046** | **Login endpoint'inde input validation eksik.** `LoginRequest` icin FluentValidation validator sinifi yok.
  - Gerekli: `LoginValidator.cs`

- [ ] **TODO-047** | **CORS politikasi potansiyel acik.** `Cors:AllowedOrigins` konfigurasyonu bos oldugunda `AllowAnyOrigin()` aktif oluyor. Production ortaminda bu guvensizdir.
  - Dosya: `Presentation/Randevu365.Api/Program.cs:28-48`
  - Oneri: Production'da `AllowAnyOrigin` fallback'i kaldirmali, zorunlu origin listesi beklemeli.

- [ ] **TODO-048** | **File upload'da dosya tur ve boyut kontrolu yetersiz.** `FileService.UploadFileAsync()` dosya uzantisi ve icerik tipi (MIME type) kontrolu yapmiyor. Zararli dosyalar (`.exe`, `.bat`, `.php`) yuklenebilir.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/FileService.cs:16-35`
  - Oneri: Izin verilen uzantilar beyaz listesi (`AllowedExtensions`) ve maksimum dosya boyutu kontrolu eklenmeli.

- [ ] **TODO-049** | **File upload'da path traversal riski.** `file.FileName` dogrudan dosya adinda kullaniliyor. `../` iceren dosya adlariyla sunucu dizin yapisinda yetki disi yazma yapilabilir.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/FileService.cs:26`
  - Oneri: `Path.GetFileName()` ile dosya adi sanitize edilmeli.

- [ ] **TODO-050** | **Uploads klasoru statik dosya olarak servis ediliyor.** `Program.cs:148-153`'te `uploads/` klasoru dogrudan statik dosya olarak sunuluyor. Erisimdenetimi yok, yuklenenmis dosyalar kamuya acik.
  - Dosya: `Presentation/Randevu365.Api/Program.cs:148-153`

- [ ] **TODO-051** | **JWT SecretKey uzunluk kontrolu yok.** HMAC-SHA256 icin minimum 256-bit (32 karakter) anahtar gerekli. Kisa anahtar kullanilirsa runtime hatasi veya guvenlik acigi olusur.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/JwtService.cs:24-25`

- [ ] **TODO-052** | **`JwtService.ValidateRefreshToken()` icinde `.GetAwaiter().GetResult()` kullaniliyor.** Bu senkron bekleme deadlock riski tasir (ozellikle ASP.NET Core'da). Asenkron yapiya cevrilmeli.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/JwtService.cs:61-63`
  - Oneri: `ValidateRefreshTokenAsync()` olarak degistirilmeli.

- [ ] **TODO-053** | **Refresh token rotation eksik.** Refresh token kullanildiginda yeni bir refresh token uretilmiyor. Calinan bir refresh token suresiz olarak kullanilabilir.
  - Dosya: `Core/Randevu365.Application/Features/RefreshToken/RefreshTokenHandler.cs`

- [ ] **TODO-054** | **Token revocation (cikayyapma) mekanizmasi yok.** Kullanici cikis yaptiginda mevcut access token ve refresh token gecersiz kilinmiyor. Logout endpoint'i eksik.
  - Oneri: `LogoutCommandRequest/Handler` olusturmali, refresh token'i DB'den silmeli.

- [ ] **TODO-055** | **Brute force korumasi yetersiz.** Rate limiting sadece `auth` politikasi (10 istek/dk) ile yapiliyor, ancak IP bazli degil. Farkli kullanicilar ayni IP'den sinirsiz giris denemesi yapabilir. Ayrica basarisiz giris denemelerini sayma ve hesap kilitleme mekanizmasi yok.
  - Dosya: `Presentation/Randevu365.Api/Program.cs:50-65`

- [ ] **TODO-056** | **XSS korumasi eksik.** Kullanici girdileri (yorum metni, isletme adi, notlar vb.) HTML encode edilmeden DB'ye kaydediliyor. Bu veriler istemcide render edilirken XSS saldirisi riski tasir.
  - Etkilenen: `AddBusinessComment`, `UpdateBusinessComment`, `CreateBusiness`, `SendMessage` handler'lari

- [ ] **TODO-057** | **SignalR Hub'inda yetkilendirme eksik.** `ChatHub` sinifinda `[Authorize]` attribute'u yok. Kimlik dogrulamasi yapilmamis kullanicilar mesaj gonderebilir.
  - Dosya: `Presentation/Randevu365.Api/Hubs/ChatHub.cs`
  - Oneri: `[Authorize]` attribute'u eklenmeli.

- [ ] **TODO-058** | **SignalR Hub'inda UserId null kontrolu yetersiz.** `_currentUserService.UserId ?? 0` ile default 0 degeri kullaniliyor. Bu, kimliksiz kullanicinin userId=0 ile mesaj gondermesine yol acar.
  - Dosya: `Presentation/Randevu365.Api/Hubs/ChatHub.cs:32-33`

---

### ONCELIK 6 - YUKSEK (Altyapi & Performans)

- [ ] **TODO-059** | **Structured logging (Serilog) entegrasyonu eksik.** Proje varsayilan .NET logging kullaniyorNuGet'teki Serilog paketleri eklenmemis. Console + dosya + (opsiyonel) Seq/Elastic sink'leri ile yapilandirilmali.
  - Etki: Production'da log toplama, arama ve analiz mumkun degil.

- [ ] **TODO-060** | **Response caching yok.** Sik erisilen ve nadiren degisen veriler (kategori listesi, top-rated businesses) icin caching uygulanmiyor. Her istekte DB'ye sorgu yapiliyor.
  - Oneri: In-memory cache veya Redis entegrasyonu. `IMemoryCache` veya `IDistributedCache` kullanimi.

- [ ] **TODO-061** | **N+1 sorgu problemi riski.** Bazi handler'larda `include` parametresi kullanilmadan iliskili entity'ler okunuyor. Lazy loading yoksa null, varsa N+1 sorgu sorunu olusur.

- [ ] **TODO-062** | **Global query filter bypas sorunu.** `IReadRepository.GetAsync()` icinde `!x.IsDeleted` filtresi bazi handler'larda manuel olarak ekleniyor, ancak `AppDbContext.OnModelCreating` icinde zaten global soft delete filtresi var. Bu, cift filtreleme veya kafa karisikligina neden oluyor.
  - Dosya: `Infrastructure/Randevu365.Persistence/Context/AppDbContext.cs:46-56`
  - Etki: Handler'lardaki `&& !x.IsDeleted` kontrolleri gereksiz tekrar.

- [ ] **TODO-063** | **Database connection pooling konfigurasyonu eksik.** `AppDbContext` icin connection pool boyutu, timeout ve retry politikasi tanimlanmamis.
  - Dosya: `Infrastructure/Randevu365.Persistence/Registration.cs`

- [ ] **TODO-064** | **UnitOfWork her cagirisinda yeni Repository instance olusturuyor.** `GetReadRepository<T>()` ve `GetWriteRepository<T>()` her cagirildiginda `new ReadRepository<T>()` / `new WriteRepository<T>()` donduruyor. Repository caching yapilmali.
  - Dosya: `Infrastructure/Randevu365.Persistence/UnitOfWork/UnitOfWork.cs:20-21`
  - Oneri: `ConcurrentDictionary` ile repository instance'larini cache'le.

- [ ] **TODO-065** | **AuditLog yazma islemi her SaveChanges'ta ek bir SaveChanges cagirisi yapiyor.** `OnAfterSaveChangesAsync` icinde `base.SaveChangesAsync()` tekrar cagirilarak 2x DB round-trip yapiliyor.
  - Dosya: `Infrastructure/Randevu365.Persistence/Context/AppDbContext.cs:150-172`
  - Etki: Her yazma isleminde ek performans maliyeti.

---

### ONCELIK 7 - YUKSEK (Test & CI/CD)

- [ ] **TODO-066** | **Hicbir test projesi yok.** Unit test, integration test veya E2E test projesi mevcut degil. Kod degisikliklerinin guvenilirligini dogrulamak imkansiz.
  - Gerekli: `Tests/Randevu365.UnitTests/` projesi (xUnit + Moq/NSubstitute)
  - Gerekli: `Tests/Randevu365.IntegrationTests/` projesi (WebApplicationFactory + TestContainers)

- [ ] **TODO-067** | **CI/CD pipeline yok.** GitHub Actions, Azure DevOps veya baska bir CI/CD araci yapilandirilmamis. Build, test, lint ve deployment otomasyonu eksik.
  - Gerekli: `.github/workflows/ci.yml`

- [ ] **TODO-068** | **Handler birim testleri yazilmali.** Ozellikle kritik is mantigi iceren handler'lar (CreateAppointment, ConfirmAppointment, Register, Login) icin birim testleri zorunludur.
  - Oncelikli: Randevu cakisma kontrolu, yetkilendirme kontrolleri, validasyon mantigi

- [ ] **TODO-069** | **Integration testleri yazilmali.** API endpoint'lerinin uctan uca calismasi, JWT authentication/authorization, veritabani islemleri test edilmeli.

- [ ] **TODO-070** | **Code coverage hedefi belirlenmeli.** Minimum %70-80 code coverage hedefi konulmali ve CI/CD'de enforce edilmeli.

---

### ONCELIK 8 - YUKSEK (Production Hazirlik)

- [ ] **TODO-071** | **`appsettings.json` icinde hassas bilgi kontrolu.** Connection string, JWT key gibi bilgiler environment variable veya User Secrets ile yonetilmeli. Production'da Azure Key Vault / AWS Secrets Manager kullanilmali.

- [ ] **TODO-072** | **Health check endpoint'i genisletilmeli.** Mevcut `/health` endpoint'i sadece PostgreSQL baglantisini kontrol ediyor. Disk alani, bellek kullanimi, dis servis baglantilari (SMTP vb.) icin ek kontroller eklenmeli.
  - Dosya: `Presentation/Randevu365.Api/Program.cs:67-68`

- [ ] **TODO-073** | **API versioning eksik.** Breaking change durumunda geriye donuk uyumluluk saglamak icin API versiyonlama (`/api/v1/`, `/api/v2/`) uygulanmali.
  - Oneri: `Asp.Versioning.Mvc` NuGet paketi

- [ ] **TODO-074** | **Dockerfile icinde preview SDK kullaniliyor.** `mcr.microsoft.com/dotnet/aspnet:10.0-preview` kullaniliyor. Production'da stabil surumler tercih edilmeli veya .NET 10 GA ciktiginda guncellenmeli.
  - Dosya: `Dockerfile:1,4`

- [ ] **TODO-075** | **Docker Compose'da postgres sifresi plain text.** `POSTGRES_PASSWORD=postgres` guvenli degil. Docker secrets veya environment variable ile yonetilmeli.
  - Dosya: `docker-compose.yml:24`

- [ ] **TODO-076** | **Docker Compose'da Redis servisi eksik.** Caching, rate limiting ve session yonetimi icin Redis eklenmeli.
  - Dosya: `docker-compose.yml`

- [ ] **TODO-077** | **`.dockerignore` dosyasi eksik.** Build context'ine gereksiz dosyalarin (`.git/`, `bin/`, `obj/`, `uploads/`, test dosyalari) dahil edilmesini engellemek icin `.dockerignore` olusturulmali.

- [ ] **TODO-078** | **Database migration CI/CD entegrasyonu yok.** Migration'larin otomatik uygulanmasi, rollback stratejisi ve migration test sureci tanimlanmamis.

- [ ] **TODO-079** | **Logging ve monitoring altyapisi yok.** Application Insights, Prometheus/Grafana veya ELK Stack gibi bir izleme sistemi entegre edilmemis.
  - Oneri: OpenTelemetry ile tracing, metriklerin toplanmasi, alerting kurulumu.

- [ ] **TODO-080** | **Background job altyapisi yok.** Periyodik gorevler (randevu hatirlatma, otomatik tamamlama, expired slot temizligi, rapor uretme) icin Hangfire veya Quartz.NET entegrasyonu eksik.

---

### ONCELIK 9 - KRITIK (Acil Duzeltilmesi Gerekenler)

- [ ] **TODO-081** | **KRITIK GUVENLIK: Kullanici kayit sirasinda rol escalation acigi.** `RegisterHandler.cs:43` satirinda `request.Role` dogrudan kullanicidan aliniyor. Herhangi biri kendini `Administrator` olarak kaydedebilir. Bu, tum uygulamanin guvenligi icin kritik bir tehdit.
  - Dosya: `Core/Randevu365.Application/Features/Register/RegisterHandler.cs:43`
  - Cozum: Kayit sirasinda rol sabit `Customer` olmali. BusinessOwner rolu admin onayi ile, Administrator rolu ise sadece sistem icerisinden atanmali.

- [ ] **TODO-082** | **KRITIK GUVENLIK: File upload path traversal acigi.** `FileService.cs:26`'da `file.FileName` dogrudan kullaniliyor. Saldirgan `../../etc/cron.d/malicious` gibi bir dosya adi ile sunucu dosya sistemine yetki disi yazabilir.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/FileService.cs:26`
  - Cozum: `var safeFileName = Path.GetFileName(file.FileName);` ile sanitize et.

- [ ] **TODO-083** | **KRITIK GUVENLIK: SignalR Hub yetkisiz erisime acik.** `ChatHub` sinifinda `[Authorize]` attribute'u yok. Kimliksiz kullanicilar WebSocket baglantisi kurup mesaj gonderebilir.
  - Dosya: `Presentation/Randevu365.Api/Hubs/ChatHub.cs`
  - Cozum: Sinifa `[Authorize]` attribute'u ekle.

- [ ] **TODO-084** | **KRITIK: `IWriteRepository<T>` interface'inde `SoftDeleteAsync` metodu tanimli degil ama `WriteRepository<T>` implementasyonunda mevcut.** Interface uzerinden erisim mumkun degil, bu da soft delete islemlerini UoW pattern ile yapilamaaz kiliyor.
  - Dosya: `Core/Randevu365.Application/Interfaces/IWriteRepository.cs`
  - Dosya: `Infrastructure/Randevu365.Persistence/Repositories/WriteRepository.cs:42-50`
  - Cozum: `IWriteRepository<T>` interface'ine `Task SoftDeleteAsync(T entity);` ekle.

- [ ] **TODO-085** | **KRITIK: MediatR lisans anahtari kaynak kodda hardcoded.** `Registration.cs:17`'de MediatR lisans anahtari dogrudan kaynak kodda yer aliyor. Bu, kaynak kod paylasiminda lisans anahtarinin ifsa olmasina neden olur.
  - Dosya: `Core/Randevu365.Application/Registration.cs:17`
  - Cozum: Lisans anahtarini environment variable veya User Secrets'a tasi.

- [ ] **TODO-086** | **KRITIK: `appsettings.json` icinde `Include Error Detail=true` aktif.** Connection string'de hata detayi acik. Production ortaminda PostgreSQL hata mesajlari istemciye sizarak veritabani yapisi hakkinda bilgi ifsa eder.
  - Dosya: `Presentation/Randevu365.Api/appsettings.json:9`
  - Cozum: Production appsettings'ten `Include Error Detail=true` kaldirilmali.

- [ ] **TODO-087** | **KRITIK: JWT SecretKey appsettings.json'da placeholder olarak duruyor.** `"CHANGE_THIS_TO_A_SECURE_SECRET_KEY_AT_LEAST_32_CHARS_LONG!"` degeri herkesin gorebilecegi sekilde repository'de yer aliyor. Production'da bu deger degistirilmezse tum JWT tokenlar sahtelenebilir.
  - Dosya: `Presentation/Randevu365.Api/appsettings.json:15`
  - Cozum: SecretKey'i appsettings'ten tamamen kaldir, yalnizca environment variable veya User Secrets ile yonet.

- [ ] **TODO-088** | **KRITIK: `SmtpEmailService` icinde `MailMessage` nesnesi dispose edilmiyor.** `using` olmadan `new MailMessage(...)` olusturuluyor. Yuksek email trafiginde bellek sizintisi (memory leak) riski var.
  - Dosya: `Infrastructure/Randevu365.Infrastructure/Services/SmtpEmailService.cs:40`
  - Cozum: `using var mailMessage = new MailMessage(...)` olarak degistir.

- [ ] **TODO-089** | **KRITIK: Coklu `SaveAsync()` cagrilari transaction olmadan yapiliyor.** `CreateBusinessCommandHandler` ve `RegisterHandler` gibi handler'larda birden fazla `SaveAsync()` cagirisi var. Ikinci kayit basarisiz olursa birinci kayit geri alinmaz, veri tutarsizligi olusur.
  - Dosyalar: `Features/Businesses/Commands/CreateBusiness/CreateBusinessCommandHandler.cs`, `Features/Register/RegisterHandler.cs:39-50`
  - Cozum: `using var transaction = await dbContext.Database.BeginTransactionAsync()` ile sarmala.

- [ ] **TODO-090** | **KRITIK: `pageSize` parametresinde ust sinir kontrolu yok.** Kullanici `pageSize=999999` gondererek tum veritabanini tek istekte cekebilir. DoS saldirisi ve bellek tasma riski.
  - Dosyalar: `BusinessController.cs:52-53`, `MessageController.cs:30`
  - Cozum: `pageSize` icin `[Range(1, 100)]` validasyonu ekle veya handler'da clamp yap.

---

## OZET TABLOSU

| Oncelik | Adet | Kategori |
|---------|------|----------|
| 1 - Dusuk | 10 | Kozmetik & Kod Temizligi |
| 2 - Dusuk-Orta | 9 | Kod Kalitesi & Standartlar |
| 3 - Orta | 10 | Eksik Temel Ozellikler |
| 4 - Orta-Yuksek | 14 | Is Mantigi Eksikleri & Hatalar |
| 5 - Yuksek | 15 | Guvenlik Sorunlari |
| 6 - Yuksek | 7 | Altyapi & Performans |
| 7 - Yuksek | 5 | Test & CI/CD |
| 8 - Yuksek | 10 | Production Hazirlik |
| 9 - Kritik | 10 | Acil Duzeltilmesi Gerekenler |
| **TOPLAM** | **90** | |

---

## ONERILEN AKSIYON PLANI

### Hemen Yapilmali (1-2 Gun)
1. TODO-081: Register rol acigini kapat
2. TODO-082: File upload path traversal duzelt
3. TODO-083: ChatHub'a Authorize ekle
4. TODO-084: IWriteRepository'ye SoftDeleteAsync ekle
5. TODO-085: MediatR lisans anahtarini koddan cikar
6. TODO-086: Connection string'den Include Error Detail kaldir
7. TODO-087: JWT SecretKey'i appsettings'ten cikar
8. TODO-088: MailMessage dispose eksikligi
9. TODO-089: Transaction eksik handler'lar
10. TODO-090: pageSize ust sinir kontrolu ekle

### Kisa Vadede Yapilmali (1-2 Hafta)
1. TODO-044-058: Diger guvenlik sorunlari
2. TODO-030-043: Is mantigi hatalari ve eksikleri
3. TODO-066-070: Test altyapisi kurulumu

### Orta Vadede Yapilmali (2-4 Hafta)
1. TODO-020-029: Eksik CRUD islemleri
2. TODO-059-065: Altyapi iyilestirmeleri
3. TODO-071-080: Production hazirlik

### Uzun Vadede Yapilmali (1-2 Ay)
1. TODO-001-019: Kod temizligi ve standartlar
2. Odeme sistemi entegrasyonu
3. SMS/Push notification servisleri
4. Takvim entegrasyonu (Google Calendar, iCal)
5. Analytics ve monitoring dashboard
