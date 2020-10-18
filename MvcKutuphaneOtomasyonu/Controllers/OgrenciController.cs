using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using MvcKutuphaneOtomasyonu.Models.Entity;
using PagedList;
using PagedList.Mvc;

namespace MvcKutuphaneOtomasyonu.Controllers
{
    public class OgrenciController : Controller
    {
        KutuphaneDbEntities ogrenci = new KutuphaneDbEntities();
        // GET: Ogrenci
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(OGRENCI ogr)
        {
            //Sisteme öğrenci kayıdını gerçekleştiriyor.
            var Ogrencigiris = ogrenci.OGRENCI.FirstOrDefault(x => x.Mail == ogr.Mail && x.Sifre == ogr.Sifre && x.DurumId == 3 && x.Aktif == true);
            if (Ogrencigiris != null)
            {
                FormsAuthentication.SetAuthCookie(Ogrencigiris.Mail, false);
                Session["Mail"] = Ogrencigiris.Mail.ToString();
                return RedirectToAction("AnaSayfa", "Ogrenci");
            }
            else
            {
                return View();
            }
        }

        public ActionResult AnaSayfa(string p, int sayfa = 1)
        {
            //Kitap arama işlemini gerçekleştirir.
            var kitaplar = from k in ogrenci.KITAP select k;
            if (!string.IsNullOrEmpty(p))
            {
                kitaplar = kitaplar.Where(x => x.Ad.Contains(p));
            }
            return View(kitaplar.Where(x => x.Aktif == true).ToList().ToPagedList(sayfa, 4));
        }
        public ActionResult Kategori(string p, int sayfa = 1)
        {
            //Kategori arama işlemlerini gerçekleştiriyoruz
            var kategoriler = from k in ogrenci.KATEGORI select k;
            if (!string.IsNullOrEmpty(p))
            {
                kategoriler = kategoriler.Where(x => x.KategoriAd.Contains(p));//Contains içermek 
            }
            return View(kategoriler.ToList().ToPagedList(sayfa, 7));
        }

        public ActionResult KategoriListele()
        {
            //Öğrencinin ilgilendiği kategorileri listeler
            var degerler = ogrenci.OGRENCIKATEGORI.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult KayitOl()
        {
            IEnumerable<SelectListItem> degerler1 = (from i in ogrenci.OKUL.ToList()
                                                     select new SelectListItem
                                                     {
                                                         Text = i.OkulAdı,
                                                         Value = i.OkulId.ToString()
                                                     }).ToList();
            ViewBag.dgr1 = degerler1;
            IEnumerable<SelectListItem> degerler2 = (from i in ogrenci.BOLUM.ToList()
                                                     select new SelectListItem
                                                     {
                                                         Text = i.BolumAd,
                                                         Value = i.BolumId.ToString()
                                                     }).ToList();
            ViewBag.dgr2 = degerler2;
            return View();
        }
        [HttpPost]
        public ActionResult KayitOl(OGRENCI o)
        {
            //Öğrencinin kayıt olduğu kısımdır.
            if (!ModelState.IsValid)
            {
                return View("KayitOl");
            }
            ogrenci.OGRENCI.Add(o);
            ogrenci.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Kitaplarim()
        {
            //Daha önce ödünç aldığı kitapları görüyor
            var kullanici = (string)Session["Mail"];
            var id = ogrenci.OGRENCI.Where(x => x.Mail == kullanici.ToString()).Select(z => z.OgrenciId).FirstOrDefault();
            var degerler = ogrenci.ISLEM.Where(x => x.OgrenciId == id).ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult KategoriEkle()
        {
            List<SelectListItem> deger1 = (from i in ogrenci.OGRENCI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad + " " + i.Soyad,
                                               Value = i.OgrenciId.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from i in ogrenci.KATEGORI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.KategoriAd,
                                               Value = i.KategoriId.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(OGRENCIKATEGORI ok)
        {
            //Bu kısımda öğrencinin ilgindiği kategorileri kendi OGRENCIKATEGORI tablosuna ekliyoruz
            var d1 = ogrenci.OGRENCI.Where(x => x.OgrenciId == ok.OGRENCI.OgrenciId).FirstOrDefault();
            var d2 = ogrenci.KATEGORI.Where(x => x.KategoriId == ok.KATEGORI.KategoriId).FirstOrDefault();
            ok.OGRENCI = d1;
            ok.KATEGORI = d2;
            ogrenci.OGRENCIKATEGORI.Add(ok);
            ogrenci.SaveChanges();
            return RedirectToAction("KategoriListele", "Ogrenci");
        }

        [HttpGet]
        public ActionResult OduncVer()
        {
            //Odunc verme işleminde ÖGRENCI tablosunda hangi öğrenciye kitap vereceğiz PERSONEL tablosunda hangi personel verecek kitabı KITAP tablosunda hangi kitabı ödünç vereceğiz. HttpGet ile değerleri getiriyoruz
            List<SelectListItem> deger1 = (from i in ogrenci.OGRENCI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad + " " + i.Soyad,
                                               Value = i.OgrenciId.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from i in ogrenci.KITAP.Where(x => x.Aktif == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.Ad,
                                               Value = i.KId.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            return View();
        }
        [HttpPost]
        public ActionResult OduncVer(ISLEM ı)
        {
            var d1 = ogrenci.OGRENCI.Where(x => x.OgrenciId == ı.OGRENCI.OgrenciId).FirstOrDefault();//OGRENCI tablosunda öğrencimizi seçiyoruz
            var d2 = ogrenci.KITAP.Where(x => x.KId == ı.KITAP.KId).FirstOrDefault();//KITAP tablosundan kitabımızı seçiyoruz
            ı.OGRENCI = d1;
            ı.KITAP = d2;
            ogrenci.ISLEM.Add(ı);
            ogrenci.SaveChanges();
            return RedirectToAction("Kitaplarim", "Ogrenci");
        }
     

        [HttpGet]
        public ActionResult Profilim()
        {
            //Profilimde giriş yapan öğrencilerin mailin alıp aldığımız mail ile değerli getiriyoruz
            IEnumerable<SelectListItem> okul = (from i in ogrenci.OKUL.ToList()
                                                select new SelectListItem
                                                {
                                                    Text = i.OkulAdı,
                                                    Value = i.OkulId.ToString()
                                                }).ToList();
            ViewBag.okl = okul;
            IEnumerable<SelectListItem> bolum = (from i in ogrenci.BOLUM.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.BolumAd,
                                                     Value = i.BolumId.ToString()
                                                 }).ToList();
            ViewBag.blm = bolum;
            var uyemail = (string)Session["Mail"];
            var degerler = ogrenci.OGRENCI.FirstOrDefault(x => x.Mail == uyemail);
            return View(degerler);
        }
        public ActionResult Profilim2(OGRENCI o)
        {
            //bu kısımda ise sisteme giriş yapan personel mail ile giriş yapılan mail adresindeki değer PERSONEL tablosundaki mail adresine eşitse bilgileri güncelle 
            var kullanici = (string)Session["Mail"];
            var üye = ogrenci.OGRENCI.FirstOrDefault(x => x.Mail == kullanici);
            üye.Ad = o.Ad;
            üye.Soyad = o.Soyad;
            üye.MezuniyetDurumu = o.MezuniyetDurumu;
            üye.Telefon = o.Telefon;
            üye.Adres = o.Adres;
            üye.Mail = o.Mail;
            üye.Sifre = o.Sifre;
            üye.DurumId = 3;
            üye.Aktif = true;
            ogrenci.SaveChanges();
            return RedirectToAction("Profilim");
        }


        //public ActionResult Error()
        //{
        //    return View();
        //}


        public ActionResult Logout()
        {
            //Giriş yaptığımız sisteme SignOut ile çıkış yapıyoruz. Çıkış gerçekleştirdikten sonra bizi Personel Controller daki Index sayfasına yönlendiriyor
            Session.Remove("Mail");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Ogrenci");
        }






    }
}