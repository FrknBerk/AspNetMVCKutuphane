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
    
    public partial class ISLEM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ISLEM()
        {
            this.CEZALAR = new HashSet<CEZALAR>();
        }
    
        public int IslemId { get; set; }
        public Nullable<int> OgrenciId { get; set; }
        public Nullable<int> KitapId { get; set; }
        public Nullable<System.DateTime> AlısTarihi { get; set; }
        public Nullable<System.DateTime> VerisTarihi { get; set; }
        public Nullable<System.DateTime> UyeGetirTarihi { get; set; }
        public Nullable<bool> IslemDurumu { get; set; }
        public Nullable<System.DateTime> Uyarı { get; set; }
        public Nullable<int> PersonelId { get; set; }
        public Nullable<int> YoneticiId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CEZALAR> CEZALAR { get; set; }
        public virtual OGRENCI OGRENCI { get; set; }
        public virtual PERSONEL PERSONEL { get; set; }
        public virtual YONETICI YONETICI { get; set; }
        public virtual KITAP KITAP { get; set; }
    }
}
