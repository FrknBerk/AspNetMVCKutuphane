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
    
    public partial class KITAP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KITAP()
        {
            this.IADE = new HashSet<IADE>();
            this.ISLEM = new HashSet<ISLEM>();
            this.MAIL = new HashSet<MAIL>();
        }
    
        public int KId { get; set; }
        public string Ad { get; set; }
        public Nullable<int> YazarId { get; set; }
        public Nullable<int> KategoriId { get; set; }
        public string BasımTarihi { get; set; }
        public Nullable<int> Sayfa { get; set; }
        public string Yayınevi { get; set; }
        public string Resim { get; set; }
        public Nullable<bool> Aktif { get; set; }
        public Nullable<int> PersonelId { get; set; }
        public Nullable<int> YoneticiId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IADE> IADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ISLEM> ISLEM { get; set; }
        public virtual KATEGORI KATEGORI { get; set; }
        public virtual PERSONEL PERSONEL { get; set; }
        public virtual YAZAR YAZAR { get; set; }
        public virtual YONETICI YONETICI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIL> MAIL { get; set; }
    }
}
