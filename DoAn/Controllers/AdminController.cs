using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn.Models;
using DoAn.ViewModel;
using System.Net;
namespace DoAn.Controllers
{
    public class AdminController : Controller
    {
        
        QLBGDataContext db = new QLBGDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowListProduct(int page = 1, int pageSize = 6)
        {
            var listED = from a in db.SANPHAMs
                         join b in db.HINHANHs on a.ID_Anh equals b.ID_Anh

                         select new SanPhamViewModel()
                         {
                             ID = a.ID_SanPham,
                             TenSP = a.TenSanPham,
                             TenAnhChinh = b.AnhChinh,
                             MoTa = a.Mota,
                             gia = a.DonViGia,
                             SoLuongTon = a.SoLuongTon
                         };

            var result = listED.ToList();
            var totalItems = result.Count();
            var model = result.OrderBy(s => s.ID).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var sanPham = db.SANPHAMs.SingleOrDefault(sp => sp.ID_SanPham == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            return View(sanPham);
        }

        // Action xử lý khi người dùng gửi form chỉnh sửa sản phẩm
        [HttpPost]
        public ActionResult Edit(SANPHAM sanPham)
        {
            if (ModelState.IsValid)
            {
                // Lấy sản phẩm từ cơ sở dữ liệu
                var existingSanPham = db.SANPHAMs.SingleOrDefault(sp => sp.ID_SanPham == sanPham.ID_SanPham);

                if (existingSanPham != null)
                {
                    // Cập nhật thông tin sản phẩm
                    existingSanPham.TenSanPham = sanPham.TenSanPham;
                    existingSanPham.ID_ThuongHieu = sanPham.ID_ThuongHieu;
                    existingSanPham.ID_DanhMuc = sanPham.ID_DanhMuc;
                    existingSanPham.ID_Anh = sanPham.ID_Anh;
                    existingSanPham.ID_Size = sanPham.ID_Size;
                    existingSanPham.ID_Mau = sanPham.ID_Mau;
                    existingSanPham.Mota = sanPham.Mota;
                    existingSanPham.DonViGia = sanPham.DonViGia;
                    existingSanPham.SoLuongTon = sanPham.SoLuongTon;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();

                    // Chuyển hướng đến trang chi tiết sản phẩm hoặc trang danh sách sản phẩm
                    return RedirectToAction("ShowListProduct", "Admin");
                }
            }

            // Nếu ModelState không hợp lệ, trả về lại view để hiển thị thông báo lỗi
            return View(sanPham);
        }
        public ActionResult Delete(int id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var sanPham = db.SANPHAMs.SingleOrDefault(sp => sp.ID_SanPham == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            return View(sanPham);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var sanPham = db.SANPHAMs.SingleOrDefault(sp => sp.ID_SanPham == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            var relatedCTDathang = db.CT_DATHANGs.Where(ct => ct.ID_SanPham == id);
            db.CT_DATHANGs.DeleteAllOnSubmit(relatedCTDathang);
            // Xóa sản phẩm và lưu thay đổi vào cơ sở dữ liệu
            db.SANPHAMs.DeleteOnSubmit(sanPham);
            db.SubmitChanges();

            // Chuyển hướng đến trang danh sách sản phẩm hoặc trang chính
            return RedirectToAction("ShowListProduct");
        }
        public ActionResult Create()
        {

            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(SANPHAM sanPham)
        {

            if (db.SANPHAMs.Any(a => a.ID_SanPham == sanPham.ID_SanPham))
            {
                ViewBag.TB = "Trùng id!!!";
                return View();

            }

            if (ModelState.IsValid)
            {
                // Thêm sản phẩm vào cơ sở dữ liệu
                db.SANPHAMs.InsertOnSubmit(sanPham);
                db.SubmitChanges();

                // Chuyển hướng đến trang danh sách sản phẩm hoặc trang chính
                return RedirectToAction("ShowListProduct");
            }

            // Nếu có lỗi, trả về view với model để hiển thị lại thông tin đã nhập
            return View(sanPham);
        }
        public ActionResult ShowDatHang(int page = 1, int pageSize = 6)
        {
            var listED = from a in db.DATHANGs
                         join b in db.CT_DATHANGs on a.ID_DatHang equals b.ID_DatHang

                         select new ChiTietDatHang()
                         {
                             ID_Dat = a.ID_DatHang,
                             ID_CTDatHang = b.ID_CTDatHang,
                             ID_Khach = a.ID_KhachHang,
                             ngayDat = a.NgayDat,
                             Soluong = b.SoLuong,
                             gia = b.DonViGia
                         };

            var result = listED.ToList();
            var totalItems = result.Count();
            var model = result.OrderBy(s => s.ID_CTDatHang).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(model);
        }
        public ActionResult XoaDonDatHang(int id)
        {
            // Lấy đơn đặt hàng từ cơ sở dữ liệu
            var donDatHang = db.DATHANGs.SingleOrDefault(ddh => ddh.ID_DatHang == id);

            if (donDatHang == null)
            {
                // Trả về một trang thông báo lỗi hoặc chuyển hướng đến trang danh sách đơn đặt hàng
                return HttpNotFound();
            }

            // Lấy danh sách chi tiết đơn đặt hàng tương ứng và xóa chúng
            var chiTietDonDatHangs = db.CT_DATHANGs.Where(ct => ct.ID_DatHang == id);
            db.CT_DATHANGs.DeleteAllOnSubmit(chiTietDonDatHangs);

            // Xóa đơn đặt hàng và lưu thay đổi vào cơ sở dữ liệu
            db.DATHANGs.DeleteOnSubmit(donDatHang);
            db.SubmitChanges();

            // Chuyển hướng đến trang danh sách đơn đặt hàng hoặc trang chính
            return RedirectToAction("ShowDatHang"); // Thay thế "Index" và "Home" bằng tên action và controller mong muốn
        }
        public ActionResult ShowDanhMuc()
        {
            return View(db.DANHMUCs.ToList());
        }
        public ActionResult CreateDM()
        {
            return View();
        }

        // POST: DanhMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDM(DANHMUC danhMuc)
        {
            if (db.DANHMUCs.Any(a => a.ID_DanhMuc == danhMuc.ID_DanhMuc))
            {
                ViewBag.TB = "Trùng id!!!";
                return View();
            }
            if (ModelState.IsValid)
            {
                // Thêm đối tượng danh mục vào cơ sở dữ liệu
                db.DANHMUCs.InsertOnSubmit(danhMuc);
                db.SubmitChanges();
                ViewBag.TB = null;
                return RedirectToAction("ShowDanhMuc"); // Chuyển hướng đến trang danh sách danh mục hoặc trang chính
            }

            return View(danhMuc);
        }
        public ActionResult EditDM(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DANHMUC danhMuc = db.DANHMUCs.SingleOrDefault(d => d.ID_DanhMuc == id);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDM(DANHMUC model)
        {
            if (ModelState.IsValid)
            {
                DANHMUC danhMuc = db.DANHMUCs.SingleOrDefault(d => d.ID_DanhMuc == model.ID_DanhMuc);

                if (danhMuc != null)
                {
                    danhMuc.TenDanhMuc = model.TenDanhMuc;
                    db.SubmitChanges();
                    return RedirectToAction("ShowDanhMuc");
                }
            }

            return View(model);
        }
        public ActionResult DeleteDM(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DANHMUC danhMuc = db.DANHMUCs.SingleOrDefault(d => d.ID_DanhMuc == id);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

            return View(danhMuc);
        }

        [HttpPost, ActionName("DeleteDM")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedDM(int id)
        {
            DANHMUC danhMuc = db.DANHMUCs.SingleOrDefault(d => d.ID_DanhMuc == id);

            if (danhMuc != null)
            {
                var pr = db.SANPHAMs.Where(ct => ct.ID_DanhMuc == id);
                db.SANPHAMs.DeleteAllOnSubmit(pr);
                var relatedItems = db.CT_DATHANGs.Where(ct => ct.SANPHAM.ID_DanhMuc == id);
                db.CT_DATHANGs.DeleteAllOnSubmit(relatedItems);

                db.DANHMUCs.DeleteOnSubmit(danhMuc);
                db.SubmitChanges();
                return RedirectToAction("ShowDanhMuc");
            }

            return HttpNotFound();
        }
        public ActionResult ShowUser()
        {
            return View(db.USERs.ToList());
        }
        public ActionResult ShowDetailUser(int id)
        {
            var user = db.USERs.SingleOrDefault(u => u.ID_KhachHang == id);
            if (user.Role == false)
            {
                Session["role"] = "Người dùng";
            }
            else
            {
                Session["role"] = "Admin";
            }

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
        public ActionResult EditUser(int id)
        {
            var user = db.USERs.SingleOrDefault(u => u.ID_KhachHang == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditUser(USER user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem user có tồn tại trong CSDL không
                var existingUser = db.USERs.SingleOrDefault(u => u.ID_KhachHang == user.ID_KhachHang);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin người dùng từ form chỉnh sửa
                existingUser.HoTen = user.HoTen;
                existingUser.NgaySinh = user.NgaySinh;
                existingUser.DiaChi = user.DiaChi;
                existingUser.GioiTinh = user.GioiTinh;
                existingUser.Email = user.Email;
                existingUser.SoDienThoai = user.SoDienThoai;
                existingUser.Role = user.Role;

                // Lưu thay đổi vào CSDL
                db.SubmitChanges();

                return RedirectToAction("ShowUser");
            }

            return View(user);
        }
    }
}