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

            var mauList = db.MAUs.ToList();
            var sizeList = db.Sizes.Select(s => new { s.ID_Size, s.Size1 }).ToList();

            ViewBag.MauList = new SelectList(mauList, "ID_Mau", "TenMau");
            ViewBag.SizeList = new SelectList(sizeList, "ID_Size", "TenKichThuoc");

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