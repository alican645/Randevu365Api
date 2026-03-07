# Randevu365 API

Kuafor, guzellik, saglik ve fitness isletmeleri icin randevu yonetim sistemi API'si.

## Teknolojiler

- .NET 10, C#
- PostgreSQL + EF Core 10
- MediatR (CQRS) + FluentValidation
- JWT Authentication
- SignalR (Gercek zamanli mesajlasma)
- Docker

## Kurulum

### Gereksinimler

- .NET 10 SDK
- PostgreSQL 16+

### Veritabani

```bash
# PostgreSQL'de veritabani olustur
createdb Randevu365Db

# Migration'lari uygula
dotnet ef database update -p Infrastructure/Randevu365.Persistence -s Presentation/Randevu365.Api
```

### Calistirma

```bash
# Development
dotnet run --project Presentation/Randevu365.Api

# Docker ile
docker-compose up -d
```

### Ortam Degiskenleri

| Degisken | Aciklama |
|----------|----------|
| `ConnectionStrings__PostgreSql` | PostgreSQL baglanti dizesi |
| `Jwt__SecretKey` | JWT imzalama anahtari (min 32 karakter) |
| `Jwt__Issuer` | JWT issuer (varsayilan: Randevu365) |
| `Jwt__Audience` | JWT audience (varsayilan: Randevu365Users) |
| `Email__SmtpHost` | SMTP sunucu adresi |
| `Email__SmtpPort` | SMTP port (varsayilan: 587) |
| `Email__SmtpUser` | SMTP kullanici adi |
| `Email__SmtpPass` | SMTP sifresi |

### API Endpoints

**Auth** (`/api/auth`)
- `POST /login` - Giris
- `POST /register` - Kayit
- `POST /refresh-token` - Token yenileme
- `POST /forgot-password` - Sifre sifirlama istegi
- `POST /reset-password` - Sifre sifirlama

**Customer** (`/api/customer`)
- `GET /nearby-businesses` - Yakin isletmeler
- `GET /business/{id}` - Isletme profili
- `GET /my-appointments` - Randevularim
- `POST /appointment/create` - Randevu olustur
- `POST /appointment/{id}/cancel` - Randevu iptal
- `GET /favorites` - Favorilerim
- `POST /favorite/add` - Favori ekle
- `DELETE /favorite/{businessId}` - Favori kaldir
- Yorum ve puanlama endpoint'leri

**Business** (`/api/business`)
- Isletme CRUD islemleri
- Randevu yonetimi (onay, red, tamamla)
- Dashboard ve istatistikler

**Admin** (`/api/admin`)
- Kullanici ve isletme yonetimi

**Messages** (`/api/messages`)
- `GET /conversations` - Konusmalar
- `GET /conversation/{id}` - Mesajlar
- `POST /send` - Mesaj gonder

**Health** (`/health`) - Sistem saglik kontrolu

### Swagger

Development ortaminda: `http://localhost:5000/swagger`
