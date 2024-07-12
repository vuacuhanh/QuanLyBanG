using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.Models
{
    public class GioHang
    {
        QLBGDataContext db = new QLBGDataContext();
        public int ID_SanPham { get; set; }
        public string TenSanPham { get; set; }
        public string AnhBia { get; set; }
        public string AnhPhu1 { get; set; }
        public string AnhPhu2 { get; set; }
        public double Gia { get; set; }
        public int SoLuong { get; set; }
        public Double ThanhTien { get { return SoLuong * Gia; } set { } }

        public object DonGia { get; internal set; }

        public GioHang(int masanpham)
        {
            ID_SanPham = masanpham;
            SANPHAM sp = db.SANPHAMs.Single(s => s.ID_SanPham == ID_SanPham);
            TenSanPham = sp.TenSanPham;
            AnhBia = sp.HINHANH.AnhChinh.ToString();
            AnhPhu1 = sp.HINHANH.Anh1.ToString();
            AnhPhu2 = sp.HINHANH.Anh2.ToString();
            Gia = double.Parse(sp.DonViGia.ToString());
            SoLuong = 1;
        }
    }
}