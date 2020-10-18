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
    public class YoneticiController : Controller
    {
        KutuphaneDbEntities yonetici = new KutuphaneDbEntities();//Veritabanımıza bağlantı işlemi gerçekleştirdik
        // GET: Yonetici
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(YONETICI y)
        {
            //Yonetici kullanıcımız giriş yapması için Index adında ActionResult oluşturduk.
            var ygiris = yonetici.YONETICI.FirstOrDefault(x => x.Mail == y.Mail && x.Sifre == y.Sifre && x.DurumId == 1);
            if(ygiris != null)
            {
                FormsAuthentication.SetAuthCookie(ygiris.Mail, false);
                Session["ymail"] = ygiris.Mail.ToString();
                return RedirectToAction("PersonelListele","Yonetici");
            }
            else
            {
                return View();
            }
        }

        public ActionResult BolumListele(int sayfa = 1)
        {
            //bu ActionResult ımızda BOLUM tablosundaki değerlerimizi listeleme işlemi gerçekleştiriyoruz
            var bolumListele = yonetici.BOLUM.Where(x => x.Aktif== true).ToList().ToPagedList(sayfa, 7);
            return View(bolumListele);
        }

        [HttpGet]
        public ActionResult BolumEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BolumEkle(BOLUM b)
        {
            if (!ModelState.IsValid)//Bölüm ekleme işlemi yaparken kontrol ediyoruz BOLUM tablosundaki değerler boş girilmişmi
            {
                return View("BolumEkle");
            }
            yonetici.BOLUM.Add(b);
            yonetici.SaveChanges();
            Response.Redirect("/Yonetici/BolumListele/");
            return View();
        }
        public ActionResult BolumGetir(int id)
        {
            //id ye göre BOLUM tablosundan id seçtiğimiz alanlar geliyor
            var bolum = yonetici.BOLUM.Find(id);
            return View("BolumGetir", bolum);
        }
        public ActionResult BolumGuncelle(BOLUM b)
        {
            //Bölüm tablosundaki değerlerimizi bu ActionResult umuz ile güncelleştirme işlemi gerçekleştiriyoruz
            var bolumguncelle = yonetici.BOLUM.Find(b.BolumId);
            bolumguncelle.BolumAd = b.BolumAd;
            bolumguncelle.EkleyenId = b.EkleyenId;
            bolumguncelle.DurumId = b.DurumId;
            yonetici.SaveChanges();
            return RedirectToAction("BolumListele");
        }
        public ActionResult BolumSil(int id)
        {
            //Bölüm silme işlemini BOLUM tablosunda Aktif kısmını false yapıyoruz İlişkili tablolarda veri silmek hataya yol açar
            var bolumsil = yonetici.BOLUM.Find(id);
            bolumsil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("BolumListele");
        }
        public ActionResult KitapListele(int sayfa = 1)
        {
            var kitapListele = yonetici.KITAP.ToList().ToPagedList(sayfa, 7);
            return View(kitapListele);
        }

        [HttpGet]
        public ActionResult KitapEkle()
        {
            //Veritabanımızda kayıtlı olan Yazarları ve Kategorileri Listeliyoruz
            List<SelectListItem> yazar = (from i in yonetici.YAZAR.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.YazarAd + i.YazarSoyad,
                                              Value = i.YazarId.ToString()
                                          }).ToList();
            ViewBag.yzr = yazar;
            List<SelectListItem> kategori = (from i in yonetici.KATEGORI.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.KategoriAd,
                                                 Value = i.KategoriId.ToString()
                                             }).ToList();
            ViewBag.ktgr = kategori;
            return View();
        }
        [HttpPost]
        public ActionResult KitapEkle(KITAP k)
        {
            if (!ModelState.IsValid)
            {
                return View("KitapEkle");
            }
            var yazar = yonetici.YAZAR.Where(x => x.YazarId == k.YAZAR.YazarId).FirstOrDefault();
            k.YAZAR = yazar;
            var kategori = yonetici.KATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).FirstOrDefault();
            k.KATEGORI = kategori;
            var ok = yonetici.OGRENCIKATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).FirstOrDefault();
            var kontrol = yonetici.OGRENCIKATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).ToList();//kontrol işlemi gerçekleştiriyoruz Kitap ekleme işlemindeki kategori id OGRENCIKATEGORI tablosundaki kategori id sine eşit ise mail gönder
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
                yonetici.KITAP.Add(k);
                yonetici.SaveChanges();
                return RedirectToAction("KitapListele");

            }
            else if (kontrol == null)//bu kısımda ise OGRENCIKATEGORI deki id ile ekleme yaptığımız kitap tablosundaki kategori eşit değilse bile kitabı ekle
            {
                yonetici.KITAP.Add(k);
                yonetici.SaveChanges();
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
            var kitap = yonetici.KITAP.Find(id);
            List<SelectListItem> yazar = (from i in yonetici.YAZAR.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.YazarAd + i.YazarSoyad,
                                              Value = i.YazarId.ToString()
                                          }).ToList();
            ViewBag.dgr = yazar;
            List<SelectListItem> kategori = (from i in yonetici.KATEGORI.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.KategoriAd,
                                                 Value = i.KategoriId.ToString()
                                             }).ToList();
            ViewBag.dgr1 = kategori;
            return View("KitapGetir", kitap);
        }

        public ActionResult KitapGuncelle(KITAP k)
        {
            var kitapguncelle = yonetici.KITAP.Find(k.KId);
            kitapguncelle.Ad = k.Ad;
            var yazar = yonetici.YAZAR.Where(x => x.YazarId == k.YAZAR.YazarId).FirstOrDefault();
            kitapguncelle.YazarId = yazar.YazarId;
            var kategori = yonetici.KATEGORI.Where(x => x.KategoriId == k.KATEGORI.KategoriId).FirstOrDefault();
            kitapguncelle.KategoriId = kategori.KategoriId;
            kitapguncelle.BasımTarihi = k.BasımTarihi;
            kitapguncelle.Resim = k.Resim;
            yonetici.SaveChanges();
            return RedirectToAction("KitapListele");
        }
        public ActionResult KitapSil(int id)
        {
            var kitapsil = yonetici.KITAP.Find(id);
            kitapsil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("KitapListele");
        }
        public ActionResult KategoriListele(int sayfa = 1)
        {
            var kategoriListele = yonetici.KATEGORI.ToList().ToPagedList(sayfa, 7);// Kategori tablosunda Aktif durumumuz true ise listeleme yapıyoruz
            return View(kategoriListele);//ve bunu döndürüyoruz return ile
        }
        [HttpGet]
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(KATEGORI k)
        {
            if (!ModelState.IsValid)
            {
                return View("KategoriEkle");
            }
            yonetici.KATEGORI.Add(k);//Add ile KATEGORI tablosunda ekleme işlemleri yapıyoruz
            yonetici.SaveChanges();//Ekleme yaptığımız işlemleri kayıt ediyoruz
            Response.Redirect("/Yonetici/KategoriListele/");
            return View();
        }
        public ActionResult KategoriGetir(int id)
        {
            //Kategori id göre güncelleme işlemi yapıyoruz
            var kategori = yonetici.KATEGORI.Find(id);
            return View("KategoriGetir", kategori);
        }
        public ActionResult KategoriGuncelle(KATEGORI k)
        {
            var kategoriguncelle = yonetici.KATEGORI.Find(k.KategoriId);
            kategoriguncelle.KategoriAd = k.KategoriAd;
            yonetici.SaveChanges();
            return RedirectToAction("KategoriListele");
        }

        public ActionResult KategoriSil(int id)
        {
            //Kategori sil işleminde Aktif durumunu false yapıyoruz çünkü kategori tablosu ilişki içinde olduğu için
            var kategorisil = yonetici.KATEGORI.Find(id);
            kategorisil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("KategoriListele");
        }
        [HttpGet]
        public ActionResult OduncVer()
        {
            //Odunc verme işleminde ÖGRENCI tablosunda hangi öğrenciye kitap vereceğiz PERSONEL tablosunda hangi personel verecek kitabı KITAP tablosunda hangi kitabı ödünç vereceğiz. HttpGet ile değerleri getiriyoruz
            List<SelectListItem> deger1 = (from i in yonetici.OGRENCI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad + " " + i.Soyad,
                                               Value = i.OgrenciId.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from i in yonetici.KITAP.Where(x => x.Aktif == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad,
                                               Value = i.KId.ToString()
                                           }).ToList();
            List<SelectListItem> deger3 = (from i in yonetici.PERSONEL.ToList()
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

            var d1 = yonetici.OGRENCI.Where(x => x.OgrenciId == ı.OGRENCI.OgrenciId).FirstOrDefault();//OGRENCI tablosunda öğrencimizi seçiyoruz
            var d2 = yonetici.KITAP.Where(x => x.KId == ı.KITAP.KId).FirstOrDefault();//KITAP tablosundan kitabımızı seçiyoruz
            var d3 = yonetici.PERSONEL.Where(x => x.PersonelId == ı.PERSONEL.PersonelId).FirstOrDefault();//PERSONEL tablosundan personelimizi seçiyoruz
            ı.OGRENCI = d1;
            ı.KITAP = d2;
            ı.PERSONEL = d3;
            yonetici.ISLEM.Add(ı);
            yonetici.SaveChanges();
            return RedirectToAction("Odunc", "Personel");
        }
        public ActionResult Odunciade(int id)
        {
            //id ye göre oödünç kitabımızı getiryoruz ve aşağıda public ActionResult OduncGuncelle ile ödünç iade işlemini gerçekleştiriyoruz
            var oduncgetir = yonetici.ISLEM.Find(id);
            return View("Odunciade", oduncgetir);
        }
        public ActionResult OduncGuncelle(ISLEM ı)
        {
            var ıslem = yonetici.ISLEM.Find(ı.IslemId);
            ıslem.UyeGetirTarihi = ı.UyeGetirTarihi;
            ıslem.IslemDurumu = true;
            yonetici.SaveChanges();
            return RedirectToAction("Index");

        }
        public ActionResult Odunc(int sayfa = 1)
        {
            //ISLEM tablosunda IslemDurumu false olanları listeleme işlemi yapıyoruz
            var degerler = yonetici.ISLEM.Where(x => x.IslemDurumu == false).ToList().ToPagedList(sayfa, 7);
            return View(degerler);
        }

        public ActionResult OduncSayfa(int sayfa = 1)
        {
            //ISLEM tablosunda IslemDurumu false olanları listeleme işlemi yapıyoruz
            var degerler = yonetici.ISLEM.Where(x => x.IslemDurumu == true).ToList().ToPagedList(sayfa, 7);
            return View(degerler);
        }

        public ActionResult OkulListele(int sayfa = 1)
        {
            //OKUL veritabanındaki değerleri listeliyoruz
            var okulListele = yonetici.OKUL.Where(x => x.Aktif == true).ToList().ToPagedList(sayfa, 7);
            return View(okulListele);
        }

        [HttpGet]
        public ActionResult OkulEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OkulEkle(OKUL o)
        {
            //OKUL veritabanına ekleme işlemi gerçekleştiriyoruz
            if (!ModelState.IsValid)
            {
                return View("OkulEkle");
            }
            o.Aktif = true;
            yonetici.OKUL.Add(o);
            yonetici.SaveChanges();
            Response.Redirect("/Yonetici/OkulListele/");
            return View();
        }
        public ActionResult OkulGetir(int id)
        {

            //id değerine göre OKUL tablosundan id ye göre değerler getiriliyor
            var okul = yonetici.OKUL.Find(id);
            return View("OkulGetir", okul);
        }
        public ActionResult OkulGuncelle(OKUL o)
        {
            var okulguncelle = yonetici.OKUL.Find(o.OkulId);
            okulguncelle.OkulAdı = o.OkulAdı;
            okulguncelle.Aktif = true;
            yonetici.SaveChanges();
            return RedirectToAction("OkulListele");
        }

        public ActionResult OkulSil(int id)
        {
            var okulsil = yonetici.OKUL.Find(id);
            okulsil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("OkulListele");
        }
        public ActionResult OgrenciListele(int sayfa = 1)
        {
            //OGRENCI tablosundaki öğrencileri listeleme işlemi gerçekleştiriyoruz
            var ogrenciListele = yonetici.OGRENCI.ToList().ToPagedList(sayfa, 7);
            return View(ogrenciListele);
        }

        [HttpGet]
        public ActionResult OgrenciEkle()
        {
            //HttpGet ile OKUL tablosundaki BOLUM tablosundaki GIRIS tablosundaki değerleri getiriyoruz
            IEnumerable<SelectListItem> degerler = (from i in yonetici.GIRIS.ToList()
                                                    select new SelectListItem
                                                    {
                                                        Text = i.DurumAd,
                                                        Value = i.DurumId.ToString()
                                                    }).ToList();
            ViewBag.dgr = degerler;
            IEnumerable<SelectListItem> degerler1 = (from i in yonetici.OKUL.Where(x => x.Aktif == true).ToList()
                                                     select new SelectListItem
                                                     {
                                                         Text = i.OkulAdı,
                                                         Value = i.OkulId.ToString()
                                                     }).ToList();
            ViewBag.dgr1 = degerler1;
            IEnumerable<SelectListItem> degerler2 = (from i in yonetici.BOLUM.Where(x => x.Aktif == true).ToList()
                                                     select new SelectListItem
                                                     {
                                                         Text = i.BolumAd,
                                                         Value = i.BolumId.ToString()
                                                     }).ToList();
            ViewBag.dgr2 = degerler2;
            return View();
        }
        [HttpPost]
        public ActionResult OgrenciEkle(OGRENCI o)
        {
            if (!ModelState.IsValid)//OgrenciEkle işlemi OGRENCI tablosundaki değerler boş geçilemez
            {
                return View("OgrenciListele");
            }
            yonetici.OGRENCI.Add(o);
            yonetici.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }
        public ActionResult OgrenciGetir(int id)
        {
            //id değerine göre OGRENCI tablosundan OKUL,BOLUM,GIRIS tablosudan id ye göre değerler getiriliyor
            var ogrencigetir = yonetici.OGRENCI.Find(id);
            List<SelectListItem> gırıs = (from i in yonetici.GIRIS.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.DurumAd,
                                              Value = i.DurumId.ToString()
                                          }).ToList();
            ViewBag.dgr = gırıs;
            List<SelectListItem> okul = (from i in yonetici.OKUL.ToList()
                                         select new SelectListItem
                                         {
                                             Text = i.OkulAdı,
                                             Value = i.OkulId.ToString()
                                         }).ToList();
            ViewBag.dgr1 = okul;
            List<SelectListItem> bolum = (from i in yonetici.BOLUM.ToList()
                                          select new SelectListItem
                                          {
                                              Text = i.BolumAd,
                                              Value = i.BolumId.ToString()
                                          }).ToList();
            ViewBag.dgr2 = bolum;
            return View("OgrenciGetir", ogrencigetir);
        }
        public ActionResult OgrenciGuncelle(OGRENCI o)
        {
            var ogrenciguncelle = yonetici.OGRENCI.Find(o.OgrenciId);
            ogrenciguncelle.Ad = o.Ad;
            ogrenciguncelle.Soyad = o.Soyad;
            ogrenciguncelle.MezuniyetDurumu = o.MezuniyetDurumu;
            var okul = yonetici.OKUL.Where(x => x.OkulId == o.OKUL.OkulId).SingleOrDefault();
            ogrenciguncelle.OkulId = okul.OkulId;
            var bolum = yonetici.BOLUM.Where(x => x.BolumId == o.BOLUM.BolumId).SingleOrDefault();
            ogrenciguncelle.BolumId = bolum.BolumId;
            ogrenciguncelle.Telefon = o.Telefon;
            ogrenciguncelle.Adres = o.Adres;
            ogrenciguncelle.Mail = o.Mail;
            ogrenciguncelle.Sifre = o.Sifre;
            var durum = yonetici.GIRIS.Where(x => x.DurumId == o.GIRIS.DurumId).SingleOrDefault();
            ogrenciguncelle.DurumId = durum.DurumId;
            yonetici.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }
        public ActionResult OgrenciSil(int id)
        {
            var ogrencisil = yonetici.OGRENCI.Find(id);
            ogrencisil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }

        public ActionResult PersonelListele(int sayfa = 1)
        {
            //PERSONEL tablosundaki değerleri listeleme işlemi gerçekleştiriyoruz
            var personelListele = yonetici.PERSONEL.Where(x => x.Aktif==true).ToList().ToPagedList(sayfa, 7);
            return View(personelListele);
        }
        [HttpGet]
        public ActionResult PersonelEkle()
        {
            List<SelectListItem> degerler = (from i in yonetici.GIRIS.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.DurumAd,
                                                 Value = i.DurumId.ToString()
                                             }).ToList();
            ViewBag.dgr = degerler;
            return View();
        }
        [HttpPost]
        public ActionResult PersonelEkle(PERSONEL p)
        {
            if (!ModelState.IsValid)
            {
                return View("PersonelEkle");
            }
            else
            {
                var durum = yonetici.GIRIS.Where(x => x.DurumId == p.GIRIS.DurumId).FirstOrDefault();
                p.GIRIS = durum;
                yonetici.PERSONEL.Add(p);
                yonetici.SaveChanges();
                return RedirectToAction("PersonelListele");
            }
        }
        public ActionResult PersonelGetir(int id)
        {
            var personelgetir = yonetici.PERSONEL.Find(id);
            List<SelectListItem> degerler = (from i in yonetici.GIRIS.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.DurumAd,
                                                 Value = i.DurumId.ToString()
                                             }).ToList();
            ViewBag.dgr = degerler;
            return View("PersonelGetir", personelgetir);
        }
        public ActionResult PersonelGuncelle(PERSONEL p)
        {
            var personelguncelle = yonetici.PERSONEL.Find(p.PersonelId);
            personelguncelle.Ad = p.Ad;
            personelguncelle.Soyad = p.Soyad;
            personelguncelle.Mail = p.Mail;
            personelguncelle.Sifre = p.Sifre;
            var durum = yonetici.GIRIS.Where(x => x.DurumId == p.GIRIS.DurumId).FirstOrDefault();
            personelguncelle.DurumId = durum.DurumId;
            yonetici.SaveChanges();
            return RedirectToAction("PersonelListele");
        }

        public ActionResult PersonelSil(int id)
        {
            var personelsil = yonetici.PERSONEL.Find(id);
            personelsil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("PersonelListele");
        }

        public ActionResult YazarListele(int sayfa = 1)
        {
            //YAZAR tablosunda Aktif edeğeri true olanı listele
            var yazarlistele = yonetici.YAZAR.ToList().ToPagedList(sayfa, 7);
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
            if (!ModelState.IsValid)
            {
                return View("YazarEkle");
            }
            yonetici.YAZAR.Add(y);
            yonetici.SaveChanges();
            Response.Redirect("/Yonetici/YazarListele/");
            return View();
        }
        public ActionResult YazarGetir(int id)
        {
            //id ile YAZAR tablosunda id sini aldığımız yazar ile YAZAR tablosundaki değerleri getirme işlemi yapyıyoruz. public ActionResult YAzarGuncelle ile güncellem işlemi gerçekleştiriyoruz
            var yazar = yonetici.YAZAR.Find(id);
            return View("YazarGetir", yazar);
        }
        public ActionResult YazarGuncelle(YAZAR y)
        {
            var yazarguncelle = yonetici.YAZAR.Find(y.YazarId);
            yazarguncelle.YazarAd = y.YazarAd;
            yazarguncelle.YazarSoyad = y.YazarSoyad;
            yonetici.SaveChanges();
            return RedirectToAction("YazarListele");
        }
        public ActionResult YazarSil(int id)
        {
            var yazarsil = yonetici.YAZAR.Find(id);
            yazarsil.Aktif = false;
            yonetici.SaveChanges();
            return RedirectToAction("YazarListele");
        }

        public ActionResult YoneticiListele(int sayfa = 1)
        {
            var yoneticiListele = yonetici.YONETICI.ToList().ToPagedList(sayfa, 7);
            return View(yoneticiListele);
        }
        [HttpGet]
        public ActionResult YoneticiEkle()
        {
            List<SelectListItem> degerler = (from i in yonetici.GIRIS.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.DurumAd,
                                                 Value = i.DurumId.ToString()
                                             }).ToList();
            ViewBag.dgr = degerler;
            return View();
        }
        [HttpPost]
        public ActionResult YoneticiEkle(YONETICI y)
        {
            //Yonetici Ekleme işlemi gerçekleştiriyoruz
            if (!ModelState.IsValid)
            {
                return View("YoneticiEkle");
            }
            else
            {
                var durum = yonetici.GIRIS.Where(x => x.DurumId == y.GIRIS.DurumId).FirstOrDefault();
                y.GIRIS = durum;
                yonetici.YONETICI.Add(y);
                yonetici.SaveChanges();
                return RedirectToAction("YoneticiListele");
            }
        }
        public ActionResult YoneticiGetir(int id)
        {
            //id ile YONETICI tablosunda id sini aldığımız yonetici ile YONETICI tablosundaki değerleri getirme işlemi yapyıyoruz. public ActionResult YoneticiGuncelleW ile güncellem işlemi gerçekleştiriyoruz
            var yoneticigetir = yonetici.YONETICI.Find(id);
            List<SelectListItem> degerler = (from i in yonetici.GIRIS.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.DurumAd,
                                                 Value = i.DurumId.ToString()
                                             }).ToList();
            ViewBag.dgr = degerler;
            return View("YoneticiGetir", yoneticigetir);
        }

        public ActionResult YoneticiGuncelle(YONETICI y)
        {
            var yoneticiguncelle = yonetici.YONETICI.Find(y.YoneticiId);
            yoneticiguncelle.Ad = y.Ad;
            yoneticiguncelle.Soyad = y.Soyad;
            yoneticiguncelle.Mail = y.Mail;
            yoneticiguncelle.Sifre = y.Sifre;
            var durum = yonetici.GIRIS.Where(x => x.DurumId == y.GIRIS.DurumId).FirstOrDefault();
            yoneticiguncelle.DurumId = durum.DurumId;
            yonetici.SaveChanges();
            return RedirectToAction("YoneticiListele");
        }

        public ActionResult YoneticiSil(int id)
        {
            var yoneticisil = yonetici.YONETICI.Find(id);
            yonetici.YONETICI.Remove(yoneticisil);
            yonetici.SaveChanges();
            return RedirectToAction("YoneticiListele");
        }

        public ActionResult Logout()
        {
            //Giriş yaptığımız sisteme SignOut ile çıkış yapıyoruz. Çıkış gerçekleştirdikten sonra bizi Personel Controller daki Index sayfasına yönlendiriyor
            Session.Remove("ymail");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Yonetici");
        }

    }
}