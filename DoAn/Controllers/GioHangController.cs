using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq;
using System.Data.Entity;

namespace DoAn.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        QLBGDataContext db = new QLBGDataContext();
        private int soLuong;

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

        public ActionResult ThemGioHang(int ms, string strURL, int quantity = 1)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang SanPham = lstGioHang.Find(s => s.ID_SanPham == ms);

            if (SanPham == null)
            {
                SanPham = new GioHang(ms);
                lstGioHang.Add(SanPham);
            }
            else
            {
                SanPham.SoLuong++;
            }

            // Update the session with the modified shopping cart
            Session["GioHang"] = lstGioHang;

            // Redirect to the shopping cart page
            return RedirectToAction("GioHang");
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


        public ActionResult DatHang()
        {
            // Lấy giỏ hàng hiện tại
            List<GioHang> lstGioHang = LayGioHang();
            // Kiểm tra giỏ hàng có sản phẩm không
            if (lstGioHang == null || lstGioHang.Count == 0)
            {
                return RedirectToAction("Index1", "Home");
            }

            // Lấy ID của khách hàng hiện tại
            int khachHangId = GetCurrentKhachHangId();
            // Tính tổng thành tiền
            double tongThanhTien = ThanhTien();

            // Tạo đối tượng đặt hàng
            DatHang datHang = new DatHang
            {
                ID_KhachHang = khachHangId,
                NgayDat = DateTime.Now,
                TongThanhTien = tongThanhTien,
                CTDatHangs = new List<CTDatHang>()
            };

            // Thêm các chi tiết đặt hàng từ giỏ hàng vào đối tượng đặt hàng
            foreach (var item in lstGioHang)
            {
                CTDatHang chiTietDatHang = new CTDatHang
                {
                    ID_SanPham = item.ID_SanPham,
                    SoLuong = item.SoLuong,
                    DonViGia = (decimal)item.Gia
                };
                datHang.CTDatHangs.Add(chiTietDatHang);
            }

            // Thêm đơn hàng vào cơ sở dữ liệu
            db.DatHangs.InsertOnSubmit(datHang);
            db.SubmitChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["GioHang"] = null;

            // Trả về view đặt hàng thành công
            return View("DatHang");
        }

        private int GetCurrentKhachHangId()
        {
            // Giả sử bạn đã có cách lấy ID khách hàng hiện tại từ session hoặc từ nguồn khác
            // Ở đây tôi giả sử bạn đã lưu ID khách hàng vào session
            return (int)Session["KhachHangId"];
        }

        public int AddDatHang(int khachHangId, double tongThanhTien)
        {
            DatHang datHang = new DatHang
            {
                ID_KhachHang = khachHangId,
                NgayDat = DateTime.Now,
                TongThanhTien = tongThanhTien
            };

            db.DatHangs.InsertOnSubmit(datHang);
            db.SubmitChanges();

            return datHang.ID_DatHang;
        }

        public void AddChiTietDatHang(int idDatHang, int idSanPham, int soLuong, double donViGia)
        {
            CTDatHang ctDatHang = new CTDatHang
            {
                ID_DatHang = idDatHang,
                ID_SanPham = idSanPham,
                SoLuong = soLuong,
                DonViGia = (decimal)donViGia
            };

            db.CTDatHangs.InsertOnSubmit(ctDatHang);
            db.SubmitChanges();
        }



        public List<GioHang> GetGioHang(string sessionId)
        {
            return Session[sessionId] as List<GioHang> ?? new List<GioHang>();
        }

        public GioHang GetSanPhamInGioHang(string sessionId, int idSanPham)
        {
            List<GioHang> lstGioHang = GetGioHang(sessionId);
            return lstGioHang.SingleOrDefault(s => s.ID_SanPham == idSanPham);
        }

        public void AddSanPhamToGioHang(string sessionId, int idSanPham, int soLuong)
        {
            List<GioHang> lstGioHang = GetGioHang(sessionId);
            GioHang sanPham = lstGioHang.SingleOrDefault(s => s.ID_SanPham == idSanPham);

            if (sanPham == null)
            {
                sanPham = new GioHang(idSanPham);
                sanPham.SoLuong = soLuong;
                lstGioHang.Add(sanPham);
            }
            else
            {
                sanPham.SoLuong += soLuong;
            }

            Session[sessionId] = lstGioHang;
        }

        public void UpdateSoLuongSanPham(string sessionId, int idSanPham, int soLuong)
        {
            List<GioHang> lstGioHang = GetGioHang(sessionId);
            GioHang sanPham = lstGioHang.SingleOrDefault(s => s.ID_SanPham == idSanPham);

            if (sanPham != null)
            {
                sanPham.SoLuong = soLuong;
            }

            Session[sessionId] = lstGioHang;
        }

        public void RemoveSanPhamFromGioHang(string sessionId, int idSanPham)
        {
            List<GioHang> lstGioHang = GetGioHang(sessionId);
            GioHang sanPham = lstGioHang.SingleOrDefault(s => s.ID_SanPham == idSanPham);

            if (sanPham != null)
            {
                lstGioHang.Remove(sanPham);
            }

            Session[sessionId] = lstGioHang;
        }

        public int GetTongSoLuong(string sessionId)
        {
            List<GioHang> lstGioHang = GetGioHang(sessionId);
            return lstGioHang.Sum(s => s.SoLuong);
        }

        public double GetThanhTien(string sessionId)
        {
            List<GioHang> lstGioHang = GetGioHang(sessionId);
            return lstGioHang.Sum(s => s.ThanhTien);
        }

        public void ClearGioHang(string sessionId)
        {
            Session[sessionId] = null;
        }

        public List<DatHang> GetAllDatHangs()
        {
            return db.DatHangs.ToList();
        }

        public DatHang GetDatHangById(int id)
        {
            return db.DatHangs.SingleOrDefault(dh => dh.ID_DatHang == id);
        }

    }
}
