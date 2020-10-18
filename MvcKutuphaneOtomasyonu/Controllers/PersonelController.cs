using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcKutuphaneOtomasyonu.Models.Entity;
using PagedList;
using PagedList.Mvc;

namespace MvcKutuphaneOtomasyonu.Controllers
{
    public class PersonelController : Controller
    {
        KutuphaneDbEntities personel = new KutuphaneDbEntities();//Veritabanımıza bağlantı işlemi gerçekleştirdik
        // GET: Personel
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(PERSONEL p)
        {
            //Personel kullanıcımız giriş yapması için Index adında ActionResult oluşturduk.
            var personelgiris = personel.PERSONEL.FirstOrDefault(x => x.Mail == p.Mail && x.Sifre == p.Sifre && x.DurumId == 2 && x.Aktif == true);//burda kontrol işlemi yapıyoruz mail ve şifre doğru girilmiş mi
            if(personelgiris != null)
            {
                //Personel giriş yaptı ise
                FormsAuthentication.SetAuthCookie(personelgiris.Mail, false);
                Session["pmail"] = personelgiris.Mail.ToString();
                return RedirectToAction("AnaSayfa", "Personel");//Giriş yaptıktan sonra Personel Controller daki AnaSayfa yönlendiriyoruz
            }
            else
            {
                return View();
            }
        }

        public ActionResult AnaSayfa()
        {
            var deger1 = personel.OGRENCI.Count();//AnaSayfa mızda OGRENCİ tablosundaki toplam öğrenci sayısını gösteriyoruz
            ViewBag.dgr1 = deger1;
            var deger2 = personel.KITAP.Count();//Count ile toplam kitap sayısını gösteriyoruz
            ViewBag.dgr2 = deger2;
            var deger3 = personel.KATEGORI.Count();//Toplam Kategori sayısını getiriyoruz
            ViewBag.dgr3 = deger3;
            var deger4 = personel.KITAP.Where(x => x.Aktif == false).Count();//Burdada ödünç verilmiş kitaplara gösteriyoruz
            ViewBag.dgr4 = deger4;
            return View();
        }

        public ActionResult KategoriListele(int sayfa = 1)
        {
            var kategorilistele = personel.KATEGORI.Where(x => x.Aktif == true).ToList().ToPagedList(sayfa, 7);//Kategori tablosunda Aktif durumumuz true ise listeleme yapıyoruz
            return View(kategorilistele);//ve bunu döndürüyoruz return ile
        }
        [HttpGet]
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(KATEGORI k)
        {
            k.Aktif = true;
            personel.KATEGORI.Add(k);//Add ile KATEGORI tablosunda ekleme işlemleri yapıyoruz
            personel.SaveChanges();//Ekleme yaptığımız işlemleri kayıt ediyoruz
            Response.Redirect("/Personel/KategoriListele/");
            return View();
        }
        public ActionResult KategoriGetir(int id)
        {
            //İd ye göre kategori tablosundaki değerleri getiriyor.
            var kategorigetir = personel.KATEGORI.Find(id);
            return View("KategoriGetir", kategorigetir);
        }
        public ActionResult KategoriGuncelle(KATEGORI k)
        {
            //Kategori id göre güncelleme işlemi yapıyoruz
            var kategoriguncelle = personel.KATEGORI.Find(k.KategoriId);
            kategoriguncelle.KategoriAd = k.KategoriAd;
            kategoriguncelle.Aktif = true;
            personel.SaveChanges();
            return RedirectToAction("KategoriListele");
        }

        public ActionResult KategoriSil(int id)
        {
            //Kategori sil işleminde Aktif durumunu false yapıyoruz çünkü kategori tablosu ilişki içinde olduğu için
            var kategorisil = personel.KATEGORI.Find(id);
            kategorisil.Aktif = false;
            personel.SaveChanges();
            return RedirectToAction("KategoriListele");
        }
        public ActionResult KitapListele(int sayfa = 1)
        {
            var kitaplistele = personel.KITAP.Where(x => x.Aktif == true).ToList().ToPagedList(sayfa, 7);
            return View(kitaplistele);
        }
        [HttpGet] //HttpGet ile değerleri getirme işlemi yapıyoruz
        public ActionResult KitapEkle()
        {
            //Veritabanımızda kayıtlı olan Yazarları ve Kategorileri Listeliyoruz
            List<SelectListItem> yazar = (from i in personel.YAZAR.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.YazarAd + " " + i.YazarSoyad,
                                              Value = i.YazarId.ToString()
                                          }).ToList();
            ViewBag.yzr = yazar;
            List<SelectListItem> kategori = (from i in personel.KATEGORI.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.KategoriAd,
                                                 Value = i.KategoriId.ToString()
                                             }).ToList();
            ViewBag.ktgr = kategori;
            return View();


        }
        [HttpPost] //HttpPost işlemi ile değerleri post ediyoruz yani gönderme işlemi diyebiliriz
        public ActionResult KitapEkle(KITAP k)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("KitapEkle");
            //}
            var yazar = personel.YAZAR.Where(x => x.YazarId == k.YAZAR.YazarId).FirstOrDefault();
            k.YAZAR = yazar;
            var kategori = personel.KATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).FirstOrDefault();
            k.KATEGORI = kategori;
            var ok = personel.OGRENCIKATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).FirstOrDefault();
            var kontrol = personel.OGRENCIKATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).ToList();//kontrol işlemi gerçekleştiriyoruz Kitap ekleme işlemindeki kategori id OGRENCIKATEGORI tablosundaki kategori id sine eşit ise mail gönder
            if (kontrol != null)
            {
                MailMessage mailgönder = new MailMessage();
                mailgönder.To.Add(ok.OGRENCI.Mail.ToString());
                mailgönder.From = new MailAddress("personelberk@gmail.com");
                mailgönder.Subject = k.KATEGORI.KategoriAd + "kategorisine yeni bir kitap eklendi...";
                mailgönder.Body = "Sayın Oğrencimiz " + ok.OGRENCI.Ad + " " + ok.OGRENCI.Soyad + " " + k.KATEGORI.KategoriAd + " kategorisinde ilgi duyduğunuz alana " + k.Ad + " adında yeni bir kitap eklendi. <br>" + "Bilgilerinize iyi günler...";
                mailgönder.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential("personelberk@gmail.com", "102027242611");
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;

                smtp.Send(mailgönder);
                personel.KITAP.Add(k);
                personel.SaveChanges();
                return RedirectToAction("KitapListele");

            }
            else if(kontrol == null)//bu kısımda ise OGRENCIKATEGORI deki id ile ekleme yaptığımız kitap tablosundaki kategori eşit değilse bile kitabı ekle
            {
                personel.KITAP.Add(k);
                personel.SaveChanges();
                return RedirectToAction("KitapListele");
            }
            else
            {
                return View();
            }

        }
        public ActionResult KitapGetir(int id)
        {
            //Kitap tablosundaki değerleri id ile getirip Aşağıdaki public ActionResult KitapGuncelle işlemini gerçekleştiriyoruz
            var kitapgetir = personel.KITAP.Find(id);
            IEnumerable<SelectListItem> yazar = (from i in personel.YAZAR.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.YazarAd + i.YazarSoyad,
                                                     Value = i.YazarId.ToString()
                                                 }).ToList();
            ViewBag.yzr = yazar;
            IEnumerable<SelectListItem> kategori = (from i in personel.KATEGORI.ToList()
                                                    select new SelectListItem
                                                    {
                                                        Text = i.KategoriAd,
                                                        Value = i.KategoriId.ToString()
                                                    }).ToList();
            ViewBag.ktgr = kategori;
            return View("KitapGetir", kitapgetir);
        }

        public ActionResult KitapGuncelle(KITAP k)
        {
            var kitapguncelle = personel.KITAP.Find(k.KId);
            kitapguncelle.Ad = k.Ad;
            var yazar = personel.YAZAR.Where(x => x.YazarId == k.YAZAR.YazarId).FirstOrDefault();
            kitapguncelle.YazarId = yazar.YazarId;
            var kategori = personel.KATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).FirstOrDefault();
            kitapguncelle.KategoriId = kategori.KategoriId;
            kitapguncelle.BasımTarihi = k.BasımTarihi;
            kitapguncelle.Sayfa = k.Sayfa;
            kitapguncelle.Yayınevi = k.Yayınevi;
            kitapguncelle.Resim = k.Resim;
            kitapguncelle.Aktif = true;
            personel.SaveChanges();
            return RedirectToAction("KitapListele");
        }

        public ActionResult KitapSil(int id)
        {
            var kitapsil = personel.KITAP.Find(id);
            kitapsil.Aktif = false;
            personel.SaveChanges();
            return RedirectToAction("KitapListele");
        }

        [HttpGet]
        public ActionResult OduncVer()
        {
            //Odunc verme işleminde ÖGRENCI tablosunda hangi öğrenciye kitap vereceğiz PERSONEL tablosunda hangi personel verecek kitabı KITAP tablosunda hangi kitabı ödünç vereceğiz. HttpGet ile değerleri getiriyoruz
            List<SelectListItem> deger1 = (from i in personel.OGRENCI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad + " " + i.Soyad,
                                               Value = i.OgrenciId.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from i in personel.KITAP.Where(x => x.Aktif == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad,
                                               Value = i.KId.ToString()
                                           }).ToList();
            List<SelectListItem> deger3 = (from i in personel.PERSONEL.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad + " " + i.Soyad,
                                               Value = i.PersonelId.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            return View();
        }
        [HttpPost]
        public ActionResult OduncVer(ISLEM ı)
        {

            var d1 = personel.OGRENCI.Where(x => x.OgrenciId == ı.OGRENCI.OgrenciId).FirstOrDefault();//OGRENCI tablosunda öğrencimizi seçiyoruz
            var d2 = personel.KITAP.Where(x => x.KId == ı.KITAP.KId).FirstOrDefault();//KITAP tablosundan kitabımızı seçiyoruz
            var d3 = personel.PERSONEL.Where(x => x.PersonelId == ı.PERSONEL.PersonelId).FirstOrDefault();//PERSONEL tablosundan personelimizi seçiyoruz
            ı.OGRENCI = d1;
            ı.KITAP = d2;
            ı.PERSONEL = d3;
            personel.ISLEM.Add(ı);
            personel.SaveChanges();
            return RedirectToAction("Odunc", "Personel");
        }
        public ActionResult Odunciade(int id)
        {
            //id ye göre oödünç kitabımızı getiryoruz ve aşağıda public ActionResult OduncGuncelle ile ödünç iade işlemini gerçekleştiriyoruz
            var oduncgetir = personel.ISLEM.Find(id);
            return View("Odunciade", oduncgetir);
        }
        public ActionResult OduncGuncelle(ISLEM ı)
        {
            var ıslem = personel.ISLEM.Find(ı.IslemId);
            ıslem.UyeGetirTarihi = ı.UyeGetirTarihi;
            ıslem.IslemDurumu = true;
            personel.SaveChanges();
            return RedirectToAction("Odunc");

        }
        public ActionResult Odunc(int sayfa = 1)
        {
            //ISLEM tablosunda IslemDurumu false olanları listeleme işlemi yapıyoruz
            var degerler = personel.ISLEM.Where(x => x.IslemDurumu == false).ToList().ToPagedList(sayfa, 7);
            return View(degerler);
        }

        public ActionResult OduncSayfa(int sayfa = 1)
        {
            //ISLEM tablosunda IslemDurumu false olanları listeleme işlemi yapıyoruz
            var degerler = personel.ISLEM.Where(x => x.IslemDurumu == true).ToList().ToPagedList(sayfa, 7);
            return View(degerler);
        }

        public ActionResult OgrenciListele(int sayfa = 1)
        {
            //OGRENCI tablosundaki öğrencileri listeleme işlemi gerçekleştiriyoruz
            var ogrenciListele = personel.OGRENCI.ToList().ToPagedList(sayfa, 7);
            return View(ogrenciListele);
        }
        [HttpGet]
        public ActionResult OgrenciEkle()
        {
            //HttpGet ile OKUL tablosundaki BOLUM tablosundaki GIRIS tablosundaki değerleri getiriyoruz
            IEnumerable<SelectListItem> okul = (from i in personel.OKUL.ToList()
                                                select new SelectListItem
                                                {
                                                    Text = i.OkulAdı,
                                                    Value = i.OkulId.ToString()
                                                }).ToList();
            ViewBag.okl = okul;
            IEnumerable<SelectListItem> bolum = (from i in personel.BOLUM.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.BolumAd,
                                                     Value = i.BolumId.ToString()
                                                 }).ToList();
            ViewBag.blm = bolum;
            IEnumerable<SelectListItem> durum = (from i in personel.GIRIS.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.DurumAd,
                                                     Value = i.DurumId.ToString()
                                                 }).ToList();
            ViewBag.drm = durum;
            return View();
        }
        [HttpPost]
        public ActionResult OgrenciEkle(OGRENCI o)
        {
            if (!ModelState.IsValid)//OgrenciEkle işlemi OGRENCI tablosundaki değerler boş geçilemez
            {
                return View("OgrenciEkle");
            }

            personel.OGRENCI.Add(o);
            personel.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }
        public ActionResult OgrenciGetir(int id)
        {
            //id değerine göre OGRENCI tablosundan OKUL,BOLUM,GIRIS tablosudan id ye göre değerler getiriliyor
            var ogrencigetir = personel.OGRENCI.Find(id);
            List<SelectListItem> okul = (from i in personel.OKUL.ToList()
                                         select new SelectListItem
                                         {
                                             Text = i.OkulAdı,
                                             Value = i.OkulId.ToString()
                                         }).ToList();
            ViewBag.okl = okul;
            List<SelectListItem> bolum = (from i in personel.BOLUM.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.BolumAd,
                                              Value = i.BolumId.ToString()
                                          }).ToList();
            ViewBag.blm = bolum;
            List<SelectListItem> durum = (from i in personel.GIRIS.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.DurumAd,
                                              Value = i.DurumId.ToString()
                                          }).ToList();
            ViewBag.drm = durum;
            return View("OgrenciGetir", ogrencigetir);
        }
        public ActionResult OgrenciGuncelle(OGRENCI o)
        {
            var ogrenciguncelle = personel.OGRENCI.Find(o.OgrenciId);
            ogrenciguncelle.Ad = o.Ad;
            ogrenciguncelle.Soyad = o.Soyad;
            ogrenciguncelle.MezuniyetDurumu = o.MezuniyetDurumu;
            var okul = personel.OKUL.Where(x => x.OkulId == o.OKUL.OkulId).FirstOrDefault();
            ogrenciguncelle.OkulId = okul.OkulId;
            var bolum = personel.BOLUM.Where(x => x.BolumId == o.BOLUM.BolumId).FirstOrDefault();
            ogrenciguncelle.BolumId = bolum.BolumId;
            ogrenciguncelle.Telefon = o.Telefon;
            ogrenciguncelle.Adres = o.Adres;
            ogrenciguncelle.Mail = o.Mail;
            ogrenciguncelle.Sifre = o.Sifre;
            var durum = personel.GIRIS.Where(x => x.DurumId == o.GIRIS.DurumId).FirstOrDefault();
            ogrenciguncelle.DurumId = durum.DurumId;
            personel.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }
        public ActionResult OgrenciSil(int id)
        {
            var ogrencisil = personel.OGRENCI.Find(id);
            ogrencisil.Aktif = false;
            personel.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }

        [HttpGet]
        public ActionResult Profilim()
        {
            //Profilimde giriş yapan personelin mailin alıp aldığımız mail ile değerli getiriyoruz
            var uyemail = (string)Session["pmail"];
            var degerler = personel.PERSONEL.FirstOrDefault(x => x.Mail == uyemail);
            return View(degerler);
        }
        public ActionResult Profilim2(PERSONEL p)
        {
            //bu kısımda ise sisteme giriş yapan personel mail ile giriş yapılan mail adresindeki değer PERSONEL tablosundaki mail adresine eşitse bilgileri güncelle 
            var kullanici = (string)Session["pmail"];
            var personelim = personel.PERSONEL.FirstOrDefault(x => x.Mail == kullanici);
            personelim.Ad = p.Ad;
            personelim.Soyad = p.Soyad;
            personelim.Mail = p.Mail;
            personelim.Sifre = p.Sifre;
            personelim.DurumId = 2;
            personelim.Aktif = true;
            personel.SaveChanges();
            return RedirectToAction("Profilim");
        }
        public ActionResult YazarListele(int sayfa = 1)
        {
            //YAZAR tablosunda Aktif edeğeri true olanı listele
            var yazarlistele = personel.YAZAR.Where(x => x.Aktif == true).ToList().ToPagedList(sayfa, 7);
            return View(yazarlistele);
        }
        [HttpGet]
        public ActionResult YazarEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult YazarEkle(YAZAR y)
        {
            //Yazar ekleme işlemi gerçekleştiriyoruz
            y.Aktif = true;
            personel.YAZAR.Add(y);
            personel.SaveChanges();
            Response.Redirect("/Personel/YazarListele/");
            return View();
        }
        public ActionResult YazarGetir(int id)
        {
            //id ile YAZAR tablosunda id sini aldığımız yazar ile YAZAR tablosundaki değerleri getirme işlemi yapyıyoruz. public ActionResult YAzarGuncelle ile güncellem işlemi gerçekleştiriyoruz
            var yazargetir = personel.YAZAR.Find(id);
            return View("YazarGetir", yazargetir);
        }
        public ActionResult YazarGuncelle(YAZAR y)
        {
            var yazarguncelle = personel.YAZAR.Find(y.YazarId);
            yazarguncelle.YazarAd = y.YazarAd;
            yazarguncelle.YazarSoyad = y.YazarSoyad;
            y.Aktif = true;
            personel.SaveChanges();
            return RedirectToAction("YazarListele");
        }

        public ActionResult YazarSil(int id)
        {
            var yazarsil = personel.YAZAR.Find(id);
            yazarsil.Aktif = false;
            personel.SaveChanges();
            return RedirectToAction("YazarListele");
        }

        public ActionResult Logout()
        {
            //Giriş yaptığımız sisteme SignOut ile çıkış yapıyoruz. Çıkış gerçekleştirdikten sonra bizi Personel Controller daki Index sayfasına yönlendiriyor
            Session.Remove("pmail");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Personel");
        }



    }
}