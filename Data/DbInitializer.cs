using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Rolleri oluştur
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }
        if (!roleManager.RoleExistsAsync("EvSahibi").Result)
        {
            roleManager.CreateAsync(new IdentityRole("EvSahibi")).Wait();
        }
        if (!roleManager.RoleExistsAsync("Kullanici").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Kullanici")).Wait();
        }

        // Admin kullanıcısını oluştur
        var adminEmail = "admin@tinyhouse.com";
        var adminUser = userManager.FindByEmailAsync(adminEmail).Result;

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Admin User"
            };

            var result = userManager.CreateAsync(adminUser, "Admin123!").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            }
        }

        // Türk ve Arap isimleri
        var turkIsimleri = new[] { "Ahmet Yılmaz", "Ayşe Demir", "Mehmet Kaya" };
        var arapIsimleri = new[] { "Ahmed Al-Farsi", "Youssef Al-Mansour", "Fatima Al-Harbi" };

        // Ev sahiplerini Türk ve Arap isimleriyle oluştur
        var evSahibi1Email = "ahmet@tinyhouse.com";
        var evSahibi2Email = "ayse@tinyhouse.com";
        var evSahibi3Email = "mehmet@tinyhouse.com";
        var evSahibiArap1Email = "ahmed.arap@tinyhouse.com";
        var evSahibiArap2Email = "youssef.arap@tinyhouse.com";

        var evSahibi1 = userManager.FindByEmailAsync(evSahibi1Email).Result;
        if (evSahibi1 == null)
        {
            evSahibi1 = new ApplicationUser
            {
                UserName = evSahibi1Email,
                Email = evSahibi1Email,
                EmailConfirmed = true,
                FullName = turkIsimleri[0]
            };
            var result = userManager.CreateAsync(evSahibi1, "EvSahibi123!").Result;
            if (result.Succeeded)
                userManager.AddToRoleAsync(evSahibi1, "EvSahibi").Wait();
        }
        var evSahibi2 = userManager.FindByEmailAsync(evSahibi2Email).Result;
        if (evSahibi2 == null)
        {
            evSahibi2 = new ApplicationUser
            {
                UserName = evSahibi2Email,
                Email = evSahibi2Email,
                EmailConfirmed = true,
                FullName = turkIsimleri[1]
            };
            var result = userManager.CreateAsync(evSahibi2, "EvSahibi123!").Result;
            if (result.Succeeded)
                userManager.AddToRoleAsync(evSahibi2, "EvSahibi").Wait();
        }
        var evSahibi3 = userManager.FindByEmailAsync(evSahibi3Email).Result;
        if (evSahibi3 == null)
        {
            evSahibi3 = new ApplicationUser
            {
                UserName = evSahibi3Email,
                Email = evSahibi3Email,
                EmailConfirmed = true,
                FullName = turkIsimleri[2]
            };
            var result = userManager.CreateAsync(evSahibi3, "EvSahibi123!").Result;
            if (result.Succeeded)
                userManager.AddToRoleAsync(evSahibi3, "EvSahibi").Wait();
        }
        var evSahibiArap1 = userManager.FindByEmailAsync(evSahibiArap1Email).Result;
        if (evSahibiArap1 == null)
        {
            evSahibiArap1 = new ApplicationUser
            {
                UserName = evSahibiArap1Email,
                Email = evSahibiArap1Email,
                EmailConfirmed = true,
                FullName = arapIsimleri[0]
            };
            var result = userManager.CreateAsync(evSahibiArap1, "EvSahibi123!").Result;
            if (result.Succeeded)
                userManager.AddToRoleAsync(evSahibiArap1, "EvSahibi").Wait();
        }
        var evSahibiArap2 = userManager.FindByEmailAsync(evSahibiArap2Email).Result;
        if (evSahibiArap2 == null)
        {
            evSahibiArap2 = new ApplicationUser
            {
                UserName = evSahibiArap2Email,
                Email = evSahibiArap2Email,
                EmailConfirmed = true,
                FullName = arapIsimleri[1]
            };
            var result = userManager.CreateAsync(evSahibiArap2, "EvSahibi123!").Result;
            if (result.Succeeded)
                userManager.AddToRoleAsync(evSahibiArap2, "EvSahibi").Wait();
        }

        // Evleri ekle
        SeedEvler(context, evSahibi1, evSahibi2, evSahibi3, evSahibiArap1, evSahibiArap2);

        // Örnek kiracı hesabı
        var tenant = new ApplicationUser
        {
            UserName = "tenant@example.com",
            Email = "tenant@example.com",
            FullName = "Örnek Kiracı",
            PhoneNumber = "5551234567",
            KayitTarihi = DateTime.Now,
            Aktif = true,
            UserType = UserType.Kiraci
        };

        var tenantResult = userManager.CreateAsync(tenant, "Test123!").Result;
        if (tenantResult.Succeeded)
        {
            userManager.AddToRoleAsync(tenant, "Kiraci").Wait();
        }

        // Örnek evler ekle
        if (!context.Evler.Any())
        {
            var evler = new List<Ev>
            {
                new Ev
                {
                    Baslik = "Bolu'da Huzurlu Bir Tatil",
                    Sehir = "Bolu",
                    Ilce = "Merkez",
                    Adres = "Bolu Merkez, Örnek Mahallesi No:1",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1500,
                    ResimUrl = "/images/ev1.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Bolu'nun merkezinde, doğayla iç içe huzurlu bir tatil için ideal ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "İzmir'de Deniz Manzaralı Ev",
                    Sehir = "İzmir",
                    Ilce = "Çeşme",
                    Adres = "Çeşme Merkez, Deniz Mahallesi No:2",
                    KisiSayisi = 6,
                    YatakSayisi = 3,
                    BanyoSayisi = 2,
                    Fiyat = 2500,
                    ResimUrl = "/images/ev2.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Çeşme'nin en güzel plajlarına yakın, deniz manzaralı lüks ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Havuz",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Antalya'da Villa",
                    Sehir = "Antalya",
                    Ilce = "Kemer",
                    Adres = "Kemer Merkez, Villa Mahallesi No:3",
                    KisiSayisi = 8,
                    YatakSayisi = 4,
                    BanyoSayisi = 3,
                    Fiyat = 3500,
                    ResimUrl = "/images/ev3.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Kemer'in en güzel bölgesinde, özel havuzlu lüks villa.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Havuz,Barbekü",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Muğla'da Dağ Evi",
                    Sehir = "Muğla",
                    Ilce = "Fethiye",
                    Adres = "Fethiye Merkez, Dağ Mahallesi No:4",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1200,
                    ResimUrl = "/images/ev4.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Fethiye'nin yükseklerinde, muhteşem manzaralı dağ evi.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Şömine",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Trabzon'da Yayla Evi",
                    Sehir = "Trabzon",
                    Ilce = "Uzungöl",
                    Adres = "Uzungöl Merkez, Yayla Mahallesi No:5",
                    KisiSayisi = 6,
                    YatakSayisi = 3,
                    BanyoSayisi = 2,
                    Fiyat = 1800,
                    ResimUrl = "/images/ev5.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Uzungöl'ün eşsiz doğasında, yayla evi.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Şömine",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Rize'de Çay Bahçeli Ev",
                    Sehir = "Rize",
                    Ilce = "Ayder",
                    Adres = "Ayder Merkez, Çay Mahallesi No:6",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1400,
                    ResimUrl = "/images/ev6.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Ayder Yaylası'nda, çay bahçeli şirin ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Bahçe",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Aydın'da Zeytinlik Ev",
                    Sehir = "Aydın",
                    Ilce = "Kuşadası",
                    Adres = "Kuşadası Merkez, Zeytin Mahallesi No:7",
                    KisiSayisi = 6,
                    YatakSayisi = 3,
                    BanyoSayisi = 2,
                    Fiyat = 2200,
                    ResimUrl = "/images/aydin.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Kuşadası'nda, zeytin ağaçları arasında huzurlu ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Bahçe,Havuz",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Bursa'da Termal Ev",
                    Sehir = "Bursa",
                    Ilce = "Çekirge",
                    Adres = "Çekirge Merkez, Termal Mahallesi No:8",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 2,
                    Fiyat = 1600,
                    ResimUrl = "/images/bursa.jpg",
                    EvSahibiAdi = adminUser.FullName,
                    EvSahibiId = adminUser.Id,
                    Aciklama = "Bursa'nın termal bölgesinde, özel jakuzili ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Termal Su,Jakuzi",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                }
            };

            context.Evler.AddRange(evler);
            context.SaveChanges();
        }
    }

    private static void SeedEvler(ApplicationDbContext context, ApplicationUser evSahibi1, ApplicationUser evSahibi2, ApplicationUser evSahibi3, ApplicationUser evSahibiArap1, ApplicationUser evSahibiArap2)
    {
        if (!context.Evler.Any())
        {
            var evler = new List<Ev>
            {
                new Ev
                {
                    Baslik = "Bolu'da Huzurlu Bir Tatil",
                    Sehir = "Bolu",
                    Ilce = "Merkez",
                    Adres = "Bolu Merkez, Örnek Mahallesi No:1",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1500,
                    ResimUrl = "/images/ev1.jpg",
                    EvSahibiAdi = evSahibiArap1.FullName,
                    EvSahibiId = evSahibiArap1.Id,
                    Aciklama = "Bolu'nun merkezinde, doğayla iç içe huzurlu bir tatil için ideal ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Trabzon'da Deniz Manzaralı Villa",
                    Sehir = "Trabzon",
                    Ilce = "Ortahisar",
                    Adres = "Ortahisar, Yalı Mahallesi No:3",
                    KisiSayisi = 7,
                    YatakSayisi = 4,
                    BanyoSayisi = 2,
                    Fiyat = 3200,
                    ResimUrl = "/images/ev5.jpg",
                    EvSahibiAdi = evSahibiArap2.FullName,
                    EvSahibiId = evSahibiArap2.Id,
                    Aciklama = "Trabzon'un eşsiz deniz manzaralı villası.",
                    Ozellikler = "Wifi,TV,Mutfak,Otopark,Bahçe",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "İzmir'de Deniz Manzaralı Ev",
                    Sehir = "İzmir",
                    Ilce = "Çeşme",
                    Adres = "Çeşme Merkez, Deniz Mahallesi No:2",
                    KisiSayisi = 6,
                    YatakSayisi = 3,
                    BanyoSayisi = 2,
                    Fiyat = 2500,
                    ResimUrl = "/images/ev2.jpg",
                    EvSahibiAdi = evSahibi1.FullName,
                    EvSahibiId = evSahibi1.Id,
                    Aciklama = "Çeşme'nin en güzel plajlarına yakın, deniz manzaralı lüks ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Havuz",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Antalya'da Lüks Villa",
                    Sehir = "Antalya",
                    Ilce = "Lara",
                    Adres = "Lara, Güzeloba Mahallesi No:5",
                    KisiSayisi = 8,
                    YatakSayisi = 4,
                    BanyoSayisi = 3,
                    Fiyat = 4000,
                    ResimUrl = "/images/ev3.jpg",
                    EvSahibiAdi = evSahibi2.FullName,
                    EvSahibiId = evSahibi2.Id,
                    Aciklama = "Antalya'nın en güzel sahiline yakın, havuzlu lüks villa.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Havuz,Klima",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Sapanca Gölü Kenarında Bungalov",
                    Sehir = "Sakarya",
                    Ilce = "Sapanca",
                    Adres = "Sapanca, Göl Mahallesi No:10",
                    KisiSayisi = 3,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1200,
                    ResimUrl = "/images/sapanca.jpg",
                    EvSahibiAdi = evSahibi3.FullName,
                    EvSahibiId = evSahibi3.Id,
                    Aciklama = "Sapanca gölü kenarında doğayla iç içe huzurlu bungalov.",
                    Ozellikler = "Wifi,Mutfak,Şömine",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Fethiye'de Deniz Manzaralı Tiny House",
                    Sehir = "Muğla",
                    Ilce = "Fethiye",
                    Adres = "Fethiye, Çalış Mahallesi No:7",
                    KisiSayisi = 2,
                    YatakSayisi = 1,
                    BanyoSayisi = 1,
                    Fiyat = 1800,
                    ResimUrl = "/images/fethiye.jpg",
                    EvSahibiAdi = evSahibi1.FullName,
                    EvSahibiId = evSahibi1.Id,
                    Aciklama = "Fethiye'nin eşsiz deniz manzarasına sahip tiny house.",
                    Ozellikler = "Wifi,TV,Mutfak,Klima",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Trabzon'da Karadeniz Ev",
                    Sehir = "Trabzon",
                    Ilce = "Uzungöl",
                    Adres = "Uzungöl Mahallesi, Göl Caddesi No:3",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1800,
                    ResimUrl = "/images/trabzon2.jpg",
                    EvSahibiAdi = evSahibi1.FullName,
                    EvSahibiId = evSahibi1.Id,
                    Aciklama = "Uzungöl'ün eşsiz manzarasına sahip, doğayla iç içe bir ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Manzara",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Rize'de Çay Bahçeli Ev",
                    Sehir = "Rize",
                    Ilce = "Ayder",
                    Adres = "Ayder Yaylası, Çay Bahçesi No:4",
                    KisiSayisi = 5,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 2000,
                    ResimUrl = "/images/ev6.jpg",
                    EvSahibiAdi = evSahibi2.FullName,
                    EvSahibiId = evSahibi2.Id,
                    Aciklama = "Ayder Yaylası'nda, çay bahçesi manzaralı şirin bir ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Bahçe",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Artvin'de Orman Ev",
                    Sehir = "Artvin",
                    Ilce = "Şavşat",
                    Adres = "Şavşat Ormanları, Ağaç Sokak No:5",
                    KisiSayisi = 3,
                    YatakSayisi = 1,
                    BanyoSayisi = 1,
                    Fiyat = 1500,
                    ResimUrl = "/images/artvin.jpg",
                    EvSahibiAdi = evSahibi3.FullName,
                    EvSahibiId = evSahibi3.Id,
                    Aciklama = "Şavşat ormanlarının ortasında, huzurlu bir tatil için ideal ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Orman",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                },
                new Ev
                {
                    Baslik = "Giresun'da Fındık Bahçeli Ev",
                    Sehir = "Giresun",
                    Ilce = "Bulancak",
                    Adres = "Bulancak Merkez, Fındık Caddesi No:6",
                    KisiSayisi = 4,
                    YatakSayisi = 2,
                    BanyoSayisi = 1,
                    Fiyat = 1700,
                    ResimUrl = "/images/giresun.jpg",
                    EvSahibiAdi = evSahibiArap2.FullName,
                    EvSahibiId = evSahibiArap2.Id,
                    Aciklama = "Fındık bahçesi manzaralı, Karadeniz'in eşsiz doğasında bir ev.",
                    Ozellikler = "Wifi,TV,Mutfak,Parking,Bahçe",
                    Aktif = true,
                    OlusturmaTarihi = DateTime.Now
                }
            };

            context.Evler.AddRange(evler);
            context.SaveChanges();

            // Fotoğraflar eklenebilir (örnek olarak ilk üç eve)
            var evFotograf1 = new EvFotograf
            {
                Url = "/images/ev1.jpg",
                Aciklama = "Bolu evi dış görünüm",
                YuklemeTarihi = DateTime.Now,
                AnaFotograf = true,
                EvId = evler[0].Id,
                Ev = evler[0]
            };
            var evFotograf2 = new EvFotograf
            {
                Url = "/images/ev2.jpg",
                Aciklama = "İzmir evi dış görünüm",
                YuklemeTarihi = DateTime.Now,
                AnaFotograf = true,
                EvId = evler[1].Id,
                Ev = evler[1]
            };
            var evFotograf3 = new EvFotograf
            {
                Url = "/images/ev3.jpg",
                Aciklama = "Antalya villa dış görünüm",
                YuklemeTarihi = DateTime.Now,
                AnaFotograf = true,
                EvId = evler[2].Id,
                Ev = evler[2]
            };
            context.EvFotograflari.AddRange(evFotograf1, evFotograf2, evFotograf3);
            context.SaveChanges();
        }
    }
} 