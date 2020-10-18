using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using MvcKutuphaneOtomasyonu.Controllers;
using Owin;
using MvcKutuphaneOtomasyonu.Models.Entity;
using System.Net.Mail;
using System.Net;
using System.Linq;

[assembly: OwinStartup(typeof(MvcKutuphaneOtomasyonu.Startup))]

namespace MvcKutuphaneOtomasyonu
{
    public class Startup
    {
        KutuphaneDbEntities mail = new KutuphaneDbEntities();
        public void Configuration(IAppBuilder app)
        {
            // Uygulamanızı nasıl yapılandıracağınız hakkında daha fazla bilgi için https://go.microsoft.com/fwlink/?LinkId=316888 adresini ziyaret edin
            GlobalConfiguration.Configuration.UseSqlServerStorage(@"Data Source=DESKTOP-508BM63\SQLEXPRESS;Initial Catalog=KutuphaneDb;Integrated Security=True");//Veritabanına bağlanma işlemini gerçekleştiriyoruz
            app.UseHangfireDashboard();//Dashboard tablosunu gösteriyoruz
            RecurringJob.AddOrUpdate(() => mailgonder(), "30 11 * * *", TimeZoneInfo.Local);//RecurringJob işlemi işleri günlük olarak saat 11:30 mailgonder() işlemini yapıyoruz
            app.UseHangfireServer();
        }
        private ISLEM ıslem;
        public async Task mailgonder()
        {
            //Burda ISLEM tarihindeki Uyarı smalldatetime mımız bugünkü güne eşit ise mail gönderme işlemi yapıyoruz
            var kontrol = mail.ISLEM.Where(x => x.Uyarı == DateTime.Now);
            if (kontrol != null)
            {
                MailMessage mailgönder = new MailMessage();
                mailgönder.To.Add("ogrenciberk@gmail.com");//Gönderilecek kişi
                mailgönder.From = new MailAddress("personelberk@gmail.com");//Gönderen kişi
                mailgönder.Subject = " iade tarihine bir gün kaldı";//Mail konusu
                mailgönder.Body = "Sayın öğrencimiz iade tarihine bir gün kaldı uzatmak isterseniz link'e tıklayınız" +
                    "<br/><br/>" + "<br>Bilgilerinize iyi günler";//Mail mesaj kısmı
                mailgönder.IsBodyHtml = true;//Html sayfasını true yapıyoruz

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential("personelberk@gmail.com", "102027242611");//Gönderen kişinin mail adresi ve şifresini alıyoruz
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";//gmail türünde gönderme işlemi gerçekleştiriyoruz
                smtp.EnableSsl = true;
                smtp.Send(mailgönder);//burda gönderme işlemi gerçekleştiriyoruz
            }
        }

    }
}
