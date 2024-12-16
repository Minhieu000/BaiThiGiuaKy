namespace BaiThiGiuaKy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SanPham")]
    public partial class SanPham
    {
        [Key]
        [StringLength(6)]
        public string MaSP { get; set; }

        [StringLength(30)]
        public string TenSP { get; set; }

        public DateTime NgayNhap { get; set; }

        [Required]
        [StringLength(3)]
        public string MaLoai { get; set; }

        public virtual LoaiSP LoaiSP { get; set; }
    }
}
