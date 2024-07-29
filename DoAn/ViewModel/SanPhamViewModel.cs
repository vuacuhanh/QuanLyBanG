using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DoAn.ViewModel
{
    public class SanPhamViewModel
    {
        public int ID { set; get; }

   
        public string TenSP { set; get; }
        public string TenAnhChinh { set; get; }
       
        public string MoTa { set; get; }

    
        public decimal? gia { set; get; }
       
        public int? SoLuongTon { set; get; }



    }

}