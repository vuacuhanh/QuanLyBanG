using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoAn.Models
{
    public class UserModel
    {
        public int ID_KhachHang { get; set; }

        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }


        [Display(Name = "Email")]
        public string Email { get; set; }


        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }


        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }


        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }
    }
}