# Kullanıcı ve Rol Yönetim Sistemi

## 📋 Genel Bakış

ConstructionCostApp projesine JWT tabanlı authentication ve rol bazlı yetkilendirme sistemi eklendi.

## 🎯 Özellikler

- ✅ JWT Token tabanlı authentication
- ✅ Rol bazlı yetkilendirme (Admin, User)
- ✅ BCrypt ile şifre hashleme
- ✅ Kullanıcı kayıt (Register)
- ✅ Kullanıcı girişi (Login)
- ✅ Mevcut kullanıcı bilgilerini alma
- ✅ Swagger'da JWT token desteği
- ✅ Otomatik seed data (Admin rolü ve admin kullanıcısı)

## 🗂️ Eklenen Dosyalar

### Application Katmanı
- `Application/DTOs/RegisterDto.cs` - Kayıt için DTO
- `Application/DTOs/LoginDto.cs` - Giriş için DTO
- `Application/DTOs/AuthResponseDto.cs` - Authentication yanıt DTO
- `Application/DTOs/UserDto.cs` - Kullanıcı bilgileri DTO
- `Application/Interfaces/IAuthService.cs` - Auth servis interface

### Infrastructure Katmanı
- `Infrastructure/Services/AuthService.cs` - Auth servis implementasyonu
- `Infrastructure/Data/DbSeeder.cs` - Veritabanı seed data

### WebAPI Katmanı
- `WebAPI/Controllers/AuthController.cs` - Authentication controller
- `WebAPI/Auth.http` - HTTP test dosyası

## 🔑 Varsayılan Kullanıcı (Seed Data)

Uygulama ilk çalıştırıldığında otomatik olarak oluşturulur:

```
Email: admin@constructioncost.com
Şifre: Admin123!
Rol: Admin
```

## 📝 Roller

1. **Admin (RoleId: 1)**
   - Tüm yetkilere sahip
   - Sistem yönetimi yapabilir
   - Tüm kullanıcıları ve rolleri yönetebilir

2. **Sistem Kullanıcısı (RoleId: 2)**
   - İç sistem kullanıcısı
   - Proje yönetimi ve takibi yapabilir
   - Teklif ve maliyet hesaplamaları oluşturabilir

3. **Müşteri (RoleId: 3)**
   - Dış müşteri kullanıcısı
   - Kendi projelerini görüntüleyebilir
   - Teklifleri inceleyebilir
   - Sipariş verebilir

4. **Tedarikçi (RoleId: 4)**
   - Tedarikçi kullanıcısı
   - Malzeme ve fiyat bilgilerini yönetebilir
   - Sipariş durumlarını görebilir

## 🚀 API Endpoints

### 1. Kullanıcı Kaydı
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "Kullanıcı Adı",
  "email": "user@example.com",
  "password": "StrongPassword123!",
  "roleId": 2
}

**Not:** roleId değerleri:
- 1: Admin
- 2: Sistem Kullanıcısı
- 3: Müşteri
- 4: Tedarikçi
```

**Yanıt:**
```json
{
  "userId": 1,
  "fullName": "Kullanıcı Adı",
  "email": "user@example.com",
  "roleName": "User",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 2. Kullanıcı Girişi
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}
```

**Yanıt:**
```json
{
  "userId": 1,
  "fullName": "Kullanıcı Adı",
  "email": "user@example.com",
  "roleName": "User",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Mevcut Kullanıcı Bilgisi
```http
GET /api/auth/me
Authorization: Bearer {token}
```

**Yanıt:**
```json
{
  "id": 1,
  "fullName": "Kullanıcı Adı",
  "email": "user@example.com",
  "roleName": "Sistem Kullanıcısı"
}
```

### 4. Admin-Only Endpoint (Test)
```http
GET /api/auth/admin-only
Authorization: Bearer {token}
```

## 🔒 Controller'da Yetkilendirme Kullanımı

### Herkes için açık endpoint
```csharp
[HttpGet("public")]
public IActionResult PublicEndpoint()
{
    return Ok("Herkese açık endpoint");
}
```

### Sadece giriş yapmış kullanıcılar için
```csharp
[Authorize]
[HttpGet("protected")]
public IActionResult ProtectedEndpoint()
{
    return Ok("Giriş yapmış kullanıcılar için");
}
```

### Sadece Admin rolü için
```csharp
[Authorize(Roles = "Admin")]
[HttpGet("admin-only")]
public IActionResult AdminOnlyEndpoint()
{
    return Ok("Sadece Admin için");
}
```

### Birden fazla rol için
```csharp
[Authorize(Roles = "Admin,Sistem Kullanıcısı")]
[HttpGet("admin-or-system")]
public IActionResult AdminOrSystemEndpoint()
{
    return Ok("Admin veya Sistem Kullanıcısı için");
}

### Müşteri ve Tedarikçi için
```csharp
[Authorize(Roles = "Müşteri,Tedarikçi")]
[HttpGet("customer-or-supplier")]
public IActionResult CustomerOrSupplierEndpoint()
{
    return Ok("Müşteri veya Tedarikçi için");
}
```

### Controller seviyesinde yetkilendirme
```csharp
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    // Tüm endpoint'ler Admin yetkisi gerektirir
    
    [HttpGet]
    public IActionResult GetAll() { ... }
    
    [AllowAnonymous] // Bu endpoint herkese açık
    [HttpGet("public")]
    public IActionResult PublicEndpoint() { ... }
}
```

## 👤 Kullanıcı Bilgilerine Erişim

Controller'da mevcut kullanıcı bilgilerine erişmek için:

```csharp
[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile()
{
    // Kullanıcı ID'si
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    // Email
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    
    // Tam Ad
    var fullName = User.FindFirst(ClaimTypes.Name)?.Value;
    
    // Rol
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    
    // Rol kontrolü
    if (User.IsInRole("Admin"))
    {
        // Admin işlemleri
    }
    
    return Ok(new { userId, email, fullName, role });
}
```

## 🔧 Swagger'da Kullanım

1. Swagger UI'ı açın (https://localhost:7179/swagger)
2. `/api/auth/login` endpoint'ini kullanarak giriş yapın
3. Dönen `token` değerini kopyalayın
4. Sağ üstteki "Authorize" butonuna tıklayın
5. `Bearer {token}` formatında token'ı yapıştırın
6. "Authorize" butonuna tıklayın
7. Artık korumalı endpoint'leri kullanabilirsiniz

## ⚙️ Yapılandırma

### appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJwtTokenGenerationMustBeAtLeast32Characters!",
    "Issuer": "ConstructionCostApp",
    "Audience": "ConstructionCostAppUsers",
    "ExpiryMinutes": "1440"
  }
}
```

**Önemli:** Production ortamında `SecretKey`'i mutlaka değiştirin ve güvenli bir yerde saklayın (Azure Key Vault, Environment Variables, vb.)

## 📦 Eklenen NuGet Paketleri

- `BCrypt.Net-Next` (Application katmanı)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (WebAPI katmanı)

## 🧪 Test

1. Projeyi çalıştırın:
   ```bash
   dotnet run --project WebAPI
   ```

2. `WebAPI/Auth.http` dosyasını kullanarak test edin
3. Veya Swagger UI'dan test edin

## 🔐 Güvenlik Notları

1. **Şifre Politikası**: Şifre minimum 6 karakter (validasyon kurallarını artırabilirsiniz)
2. **Token Süresi**: Varsayılan 1440 dakika (24 saat)
3. **SecretKey**: Production'da mutlaka güçlü bir key kullanın
4. **HTTPS**: Production'da mutlaka HTTPS kullanın
5. **CORS**: Gerekirse CORS ayarlarını yapın

## 🎯 Sonraki Adımlar

- [ ] Refresh token mekanizması ekle
- [ ] Email doğrulama ekle
- [ ] Şifre sıfırlama ekle
- [ ] İki faktörlü kimlik doğrulama (2FA)
- [ ] Kullanıcı profil yönetimi
- [ ] Rol ve yetki yönetim paneli
- [ ] Kullanıcı aktivite logları

## 📞 Yardım

Herhangi bir sorunuz varsa lütfen iletişime geçin!

