﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KutuphaneDbEntities : DbContext
    {
        public KutuphaneDbEntities()
            : base("name=KutuphaneDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BOLUM> BOLUM { get; set; }
        public virtual DbSet<CEZALAR> CEZALAR { get; set; }
        public virtual DbSet<GIRIS> GIRIS { get; set; }
        public virtual DbSet<IADE> IADE { get; set; }
        public virtual DbSet<ISLEM> ISLEM { get; set; }
        public virtual DbSet<KATEGORI> KATEGORI { get; set; }
        public virtual DbSet<MAIL> MAIL { get; set; }
        public virtual DbSet<OGRENCI> OGRENCI { get; set; }
        public virtual DbSet<OGRENCIKATEGORI> OGRENCIKATEGORI { get; set; }
        public virtual DbSet<OKUL> OKUL { get; set; }
        public virtual DbSet<PERSONEL> PERSONEL { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<YAZAR> YAZAR { get; set; }
        public virtual DbSet<YONETICI> YONETICI { get; set; }
        public virtual DbSet<KITAP> KITAP { get; set; }
    }
}
