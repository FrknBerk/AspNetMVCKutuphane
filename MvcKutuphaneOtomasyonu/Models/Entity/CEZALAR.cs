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
    
    public partial class CEZALAR
    {
        public int Id { get; set; }
        public Nullable<int> OgrenciId { get; set; }
        public Nullable<System.DateTime> BaslangıcTarihi { get; set; }
        public Nullable<System.DateTime> BıtısTarihi { get; set; }
        public Nullable<decimal> Para { get; set; }
        public Nullable<int> IslemId { get; set; }
    
        public virtual ISLEM ISLEM { get; set; }
        public virtual OGRENCI OGRENCI { get; set; }
    }
}
