using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DoAn.Controllers
{
    public class GiayController : Controller
    {
        // GET: GIay
        QLBGDataContext db = new QLBGDataContext();
        public ActionResult XemChiTietSanPham(int mg)
        {
            SANPHAM sanpham = db.SANPHAMs.FirstOrDefault(s => s.ID_SanPham == mg);

            if (sanpham == null)
            {
                return HttpNotFound();
            }

            // Truy vấn danh sách màu sắc và kích thước
            var mauList = db.MAUs.Select(m => new { ID = m.ID_Mau, Ten = m.TenMau }).ToList();
            var sizeList = db.Sizes.Select(s => new { ID = s.ID_Size, Ten = s.Size1 }).ToList();

            // Sử dụng SelectList để tạo danh sách cho ViewBag
            ViewBag.MauList = new SelectList(mauList, "ID", "Ten");
            ViewBag.SizeList = new SelectList(sizeList, "ID", "Ten");

            return View(sanpham);
        }

        public ActionResult Timgiay(string txt_Search)
        {
            if (string.IsNullOrEmpty(txt_Search))
            {
                var allProducts = db.SANPHAMs.ToList();
                return View(allProducts);
            }
            else
            {
                var searchResults = db.SANPHAMs.Where(s => s.TenSanPham.Contains(txt_Search)).ToList();
                return View(searchResults);
            }
        }
        public ActionResult GiayTheoDM(int MaDM)
        {
            var ListSach = db.SANPHAMs.Where(s => s.ID_DanhMuc == MaDM).OrderBy(s => s.DonViGia).ToList();
            if (ListSach.Count == 0)
            {
                ViewBag.TB = "Không có loại giày này";
            }
            return View(ListSach);
        }

    }
}