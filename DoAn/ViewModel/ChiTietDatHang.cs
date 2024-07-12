using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.ViewModel
{
    public class ChiTietDatHang
    {
        internal int ID_SanPham;
        internal object DonViGia;

        public int ID_Dat { set; get; }
        public int ID_CTDatHang { set; get; }
        public int? ID_Khach { set; get; }

        public DateTime? ngayDat { set; get; }
        public int? Soluong { set; get; }
        public decimal? gia { set; get; }

    }
}