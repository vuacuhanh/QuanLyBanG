using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace DoAn.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        QLBGDataContext db = new QLBGDataContext();

        public ActionResult Index1()
        {
            return View();
        }

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int ms, string strURL, int quantity = 1, bool isBuyNow = false)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang SanPham = lstGioHang.Find(s => s.ID_SanPham == ms);

            if (SanPham == null)
            {
                SanPham = new GioHang(ms);
                SanPham.SoLuong = quantity; // set số lượng theo quantity
                lstGioHang.Add(SanPham);
            }
            else
            {
                SanPham.SoLuong += quantity; 
            }

            Session["GioHang"] = lstGioHang;

            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            if (isBuyNow)
            {
                return RedirectToAction("GioHang");
            }

            return Redirect(strURL);
        }

        public int TongSoLuong()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Sum(s => s.SoLuong);
            }
            return tsl;
        }

        private double ThanhTien()
        {
            double tt = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tt += lstGioHang.Sum(s => s.ThanhTien);
            }
            return tt;
        }

        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index1", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = ThanhTien();
            return View(lstGioHang);
        }

        public ActionResult GioHangPar()
        {
            ViewBag.TongSoLuong = TongSoLuong();

            return View();
        }

        public ActionResult TangSoLuong(int id)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang SanPham = lstGioHang.Find(s => s.ID_SanPham == id);
            if (SanPham != null)
            {
                SanPham.SoLuong++;
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult GiamSoLuong(int id)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang SanPham = lstGioHang.Find(s => s.ID_SanPham == id);
            if (SanPham != null && SanPham.SoLuong > 1)
            {
                SanPham.SoLuong--;
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang(int id)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang SanPham = lstGioHang.Find(s => s.ID_SanPham == id);

            if (SanPham != null)
            {
                lstGioHang.Remove(SanPham);
            }

            return RedirectToAction("GioHang");
        }

        // Phương thức Đặt hàng
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang == null || lstGioHang.Count == 0)
            {
                return RedirectToAction("Index1", "Home");
            }

            int idKhachHang = GetCurrentKhachHangId();
            if (idKhachHang == 0)
            {
                return RedirectToAction("DangNhap", "DangNhap");
            }

            var sanPhamList = lstGioHang.Select(s => new {
                ID_SanPham = s.ID_SanPham,
                SoLuong = s.SoLuong,
                DonGia = s.Gia
            });

            string sanPhamListJson = Newtonsoft.Json.JsonConvert.SerializeObject(sanPhamList);

            string connectionString = ConfigurationManager.ConnectionStrings["QLBGConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("sp_DatHang", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ID_KhachHang", idKhachHang);
                command.Parameters.AddWithValue("@NgayDat", DateTime.Now.Date);
                command.Parameters.AddWithValue("@SoLuong", lstGioHang.Sum(s => s.SoLuong));
                command.Parameters.AddWithValue("@SanPhamList", sanPhamListJson);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int idDatHang = Convert.ToInt32(reader["ID_DatHang"]);
                    int idHoaDon = Convert.ToInt32(reader["ID_HoaDon"]);

                    ViewBag.ID_DatHang = idDatHang;
                    ViewBag.ID_HoaDon = idHoaDon;
                    ViewBag.ThongBao = "Đặt hàng thành công!";
                }

                reader.Close();
            }

            Session["GioHang"] = null;

            return View("DatHang");
        }

        private int GetCurrentKhachHangId()
        {
            if (Session["ID_KhachHang"] != null)
            {
                return (int)Session["ID_KhachHang"];
            }
            return 0;
        }

    }
}
