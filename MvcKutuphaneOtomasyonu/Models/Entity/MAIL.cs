//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcKutuphaneOtomasyonu.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class MAIL
    {
        public int MailId { get; set; }
        public string MailKonu { get; set; }
        public string MailMesaj { get; set; }
        public Nullable<int> OgrenciId { get; set; }
        public string OgrenciMail { get; set; }
        public Nullable<int> PersonelId { get; set; }
        public string PersonelMail { get; set; }
        public Nullable<int> KitapId { get; set; }
        public string KitapAd { get; set; }
        public Nullable<bool> Aktif { get; set; }
    
        public virtual OGRENCI OGRENCI { get; set; }
        public virtual PERSONEL PERSONEL { get; set; }
        public virtual KITAP KITAP { get; set; }
    }
}
