using DoAn.Models;
using GoogleAuthentication.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        QLBGDataContext db = new QLBGDataContext();
        public ActionResult Index() //HomeController
        {
            // Kiểm tra xem người dùng đã đăng nhập bằng Google chưa
            if (Session["TaiKhoan"] != null)
            {
                // Lấy thông tin người dùng từ session
                var googleUser = Session["TaiKhoan"] as GoogleProfile;

                // Kiểm tra xem thông tin người dùng có khác null không
                if (googleUser != null)
                {
                    // Lấy tên tài khoản Google
                    string taiKhoan = googleUser.Name; // Sử dụng trường Name

                    // Truyền tên người dùng tới ViewBag
                    ViewBag.TaiKhoan = taiKhoan;
                }
            }

            return View();
        }


        public ActionResult ViewProfile()
        {
            // Lấy thông tin người dùng từ Session hoặc cơ sở dữ liệu
            string taiKhoan = Session["TaiKhoan"] as string;
            var user = db.USERs.FirstOrDefault(u => u.TaiKhoan == taiKhoan);


            // Truyền thông tin người dùng tới view
            return View(user);
        }
        public ActionResult Edit()
        {
            // Lấy thông tin người dùng từ Session hoặc cơ sở dữ liệu
            string taiKhoan = Session["TaiKhoan"] as string;
            var user = db.USERs.FirstOrDefault(u => u.TaiKhoan == taiKhoan);

            // Truyền thông tin người dùng tới view
            return View(user);
        }

        // POST: Home/Edit
        [HttpPost]
        public ActionResult Edit(UserModel updatedProfile)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin người dùng từ cơ sở dữ liệu
                string taiKhoan = Session["TaiKhoan"] as string;
                var user = db.USERs.FirstOrDefault(u => u.TaiKhoan == taiKhoan);

                // Kiểm tra nếu người dùng không tồn tại
                if (user == null)
                {
                    return HttpNotFound(); // hoặc bất kỳ hành động khác phù hợp
                }

                // Kiểm tra giá trị ngày tháng trước khi cập nhật
                if (IsValidDate(updatedProfile.NgaySinh))
                {
                    // Cập nhật thông tin người dùng từ dữ liệu mới nhập
                    user.HoTen = updatedProfile.HoTen;
                    user.Email = updatedProfile.Email;
                    user.SoDienThoai = updatedProfile.SoDienThoai;

                    // Chỉ lấy ngày tháng năm từ updatedProfile.NgaySinh
                    user.NgaySinh = updatedProfile.NgaySinh == DateTime.MinValue ? (DateTime?)null : updatedProfile.NgaySinh.Date;

                    user.DiaChi = updatedProfile.DiaChi;
                    user.GioiTinh = updatedProfile.GioiTinh;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();

                    // Chuyển hướng về trang profile sau khi cập nhật
                    return RedirectToAction("ViewProfile");
                }
                else
                {
                    ModelState.AddModelError("NgaySinh", "Ngày sinh không hợp lệ.");
                }
            }

            // Nếu ModelState không hợp lệ, quay lại trang chỉnh sửa
            return View(updatedProfile);
        }

        // Hàm kiểm tra giá trị ngày tháng hợp lệ
        private bool IsValidDate(DateTime? date)
        {
            return date.HasValue && date >= SqlDateTime.MinValue.Value && date <= SqlDateTime.MaxValue.Value;
        }

        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult contact()
        {
            return View();
        }
        public ActionResult shopsingle()
        {
            return View();
        }


        public ActionResult Shop(int page = 1, int pageSize = 9, string sortOrder = "Featured")
        {
            var totalItems = db.SANPHAMs.Count();
            IQueryable<SANPHAM> sanphams = db.SANPHAMs;

            // Apply sorting based on sortOrder parameter
            switch (sortOrder)
            {
                case "AtoZ":
                    sanphams = sanphams.OrderBy(s => s.TenSanPham);
                    break;
                case "PriceHighLow":
                    sanphams = sanphams.OrderByDescending(s => s.DonViGia);
                    break;
                case "PriceLowHigh":
                    sanphams = sanphams.OrderBy(s => s.DonViGia);
                    break;
                default:
                    // Featured or default sorting
                    sanphams = sanphams.OrderBy(s => s.TenSanPham); // Assuming Featured means sorting by name by default
                    break;
            }

            var model = sanphams.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SortOrder = sortOrder;

            return View(model);
        }


        public ActionResult ListSanPham()
        {
            return View();
        }
    }
}