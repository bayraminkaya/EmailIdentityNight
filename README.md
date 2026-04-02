# 📧 EmailIdentityNight

ASP.NET Core MVC ve Identity kütüphanesi kullanılarak geliştirilmiş, tam özellikli bir e-posta yönetim sistemi. Syndash admin teması ile modern ve responsive bir arayüze sahiptir.

---

## 🚀 Özellikler

### 🔐 Kimlik Doğrulama
- **Kayıt (Register):** Ad, soyad, kullanıcı adı, e-posta ve şifre ile kayıt
- **E-Posta Doğrulama:** Kayıt sonrası 6 haneli kod ile e-posta doğrulama
- **Giriş (Login):** Kullanıcı adı + şifre ile giriş, `EmailConfirmed` kontrolü
- **Şifremi Unuttum:** E-posta ile şifre sıfırlama linki gönderme
- **Şifre Sıfırlama:** Token bazlı güvenli şifre yenileme
- **Kullanım Koşulları:** Kayıt ekranında modal ile T&C onayı

### 📬 Mailbox Modülü
- **Gelen Kutusu:** Gelen mesajları listeleme, okundu/okunmadı takibi
- **Gönderilenler:** Gönderilen mesajları listeleme
- **Yıldızlılar:** Yıldızlanan mesajları filtreleme
- **Çöp Kutusu:** Silinen mesajları görüntüleme
- **Kategoriler:** İş, Aile, Eğitim, Sosyal kategorilere göre filtreleme
- **Compose (Yeni Mesaj):** Gmail benzeri sağ alt köşe popup, Summernote Text Editor
- **Mesaj Detayı:** HTML içerik render, yanıtlama, yıldızlama, silme
- **Arama:** Anlık mesaj arama (konu, gönderici, içerik)
- **Pagination:** Sayfa başına 13 mesaj

### 👤 Profil Sayfası
- Profil bilgilerini güncelleme (ad, soyad, e-posta, şifre)
- Profil fotoğrafı yükleme
- Hakkımda bölümü
- **8 İstatistik Widget:** Toplam mesaj, gönderilen, alınan, okunmamış, yıldızlı, iş/aile e-postası, silinen

---

## 🛠️ Teknolojiler

| Teknoloji | Açıklama |
|-----------|----------|
| ASP.NET Core MVC 9 | Web framework |
| ASP.NET Core Identity | Kimlik doğrulama |
| Entity Framework Core | ORM / Code First |
| SQL Server | Veritabanı |
| MailKit / MimeKit | E-posta gönderme (SMTP) |
| Summernote | Rich Text Editor |
| Syndash Bootstrap Theme | UI Teması |

---

## 📸 Ekran Görüntüleri

### Kayıt Sayfası
![Register](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164123.png)

### Kullanım Koşulları ve Gizlilik Politikası
![Verify](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164137.png)

### Giriş Sayfası
![Login](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164157.png)

### Şifremi Unuttum Sayfası
![Inbox](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164250.png)

### Şifremi Unuttum Sayfası Başarı ile Mail gönderilmesi
![Detail](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164713.png)

### Şifre Sıfırlamasının Başarı ile Mail Kutusuna Gelmesi
![Profile](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164725.png)

### Gelen Kutusu
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164811.png)

### Gönderilenler Kutusu
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164831.png)

### Kategoriler(Eğitim)
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20164907.png)

### Mail İçeriği Görüntüleme
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20165031.png)

### Yeni Mesaj Oluşturma
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20165302.png)

### Profilim Sayfası
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20165353.png)

### E Posta Doğrulama Sayfası
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20165603.png)

### E Posta Doğrulamanın Maile gelmesi
![ForgotPassword](Project2EmailNight/Images/Ekran%20görüntüsü%202026-04-02%20165633.png)

---

## ⚙️ Kurulum

### Gereksinimler
- .NET 9 SDK
- SQL Server
- Gmail hesabı (App Password ile)

### Adımlar

**1. Repoyu klonla:**
```bash
git clone https://github.com/bayraminkaya/EmailIdentityNight.git
cd EmailIdentityNight
```

**2. `appsettings.json` dosyasını oluştur:**

Projede `appsettings.json` dosyası `.gitignore`'a eklendiğinden repoda bulunmamaktadır. Aşağıdaki şablonu kullanarak kendi dosyanı oluştur:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "EmailSettings": {
    "SenderEmail": "GMAIL_ADRESIN",
    "SenderName": "Identity Admin",
    "AppPassword": "GOOGLE_APP_PASSWORD"
  }
}
```

> **Not:** Google App Password oluşturmak için: [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)

**3. Veritabanını oluştur:**
```bash
cd Project2EmailNight
dotnet ef database update
```

**4. Projeyi çalıştır:**
```bash
dotnet run
```

---

## 📁 Proje Yapısı
```
Project2EmailNight/
├── Context/
│   └── EmailContext.cs
├── Controllers/
│   ├── RegisterController.cs
│   ├── LoginController.cs
│   ├── PasswordController.cs
│   ├── MessageController.cs
│   ├── ProfileController.cs
│   └── EmailController.cs
├── Dtos/
│   ├── UserRegisterDto.cs
│   ├── UserLoginDto.cs
│   ├── UserEditDto.cs
│   ├── VerifyEmailDto.cs
│   ├── ForgotPasswordDto.cs
│   └── ResetPasswordDto.cs
├── Entities/
│   ├── AppUser.cs
│   └── Message.cs
├── Models/
│   └── CustomIdentityValidator.cs
├── Views/
│   ├── Register/
│   ├── Login/
│   ├── Password/
│   ├── Message/
│   └── Profile/
└── wwwroot/
    ├── vertical/    ← Syndash teması
    └── images/      ← Profil fotoğrafları
```

---

## 🔒 Güvenlik

- Şifreler ASP.NET Core Identity ile hash'lenerek saklanır
- E-posta doğrulaması olmadan giriş yapılamaz
- Şifre sıfırlama token bazlı ve tek kullanımlıktır
- SMTP kimlik bilgileri `appsettings.json`'da tutulur ve `.gitignore`'a eklenmiştir
- Anti-Forgery Token tüm POST işlemlerinde kullanılmaktadır

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

---

## 👨‍💻 Geliştirici

**Bayram İnkaya**  
[GitHub](https://github.com/bayraminkaya)
