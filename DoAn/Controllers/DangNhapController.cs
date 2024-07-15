using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using GoogleAuthentication.Services;
using System.Security.Policy;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DoAn.Controllers
{
    public class DangNhapController : Controller
    {
        QLBGDataContext db = new QLBGDataContext();

        // GET: DangNhap
        public ActionResult DangNhap()
        {

            var ClientID = "490648763973-t0jpi2ld4u5h0sivmsv1s40i4udn5amh.apps.googleusercontent.com";
            var url = "https://localhost:44330/DangNhap/GoogleLoginCallback";
            var response = GoogleAuth.GetAuthUrl(ClientID, url);
            ViewBag.response = response;
            return View();
        }
        public async Task<ActionResult> GoogleLoginCallback(string code)
        {
            try
            {

                var ClientSecret = "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";
                var ClientID = "490648763973-t0jpi2ld4u5h0sivmsv1s40i4udn5amh.apps.googleusercontent.com";
                var url = "https://localhost:44330/DangNhap/GoogleLoginCallback";
                var token = await GoogleAuth.GetAuthAccessToken(code, ClientID, ClientSecret, url); 
                var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
                var googleUser = JsonConvert.DeserializeObject<GoogleProfile>(userProfile);
                Session["TaiKhoan"] = googleUser.Name;
                Session["Img"] = googleUser.Name;


                //return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public ActionResult KiemTraDangNhap(string TaiKhoan, string MatKhau)
        {

            // Kiểm tra xem có thông tin đăng nhập hợp lệ không
            var user = db.USERs.FirstOrDefault(u => u.TaiKhoan == TaiKhoan && u.MatKhau == MatKhau);

            if (string.IsNullOrEmpty(TaiKhoan) && string.IsNullOrEmpty(MatKhau))
            {
                // Đăng nhập không thành công, cả hai trường đều trống
                ModelState.AddModelError("", "Please enter your username and password!.");
            }
            else if (user == null)
            {
                // Đăng nhập không thành công, kiểm tra từng trường và thêm ModelState
                if (string.IsNullOrEmpty(TaiKhoan))
                {
                    ModelState.AddModelError("TaiKhoan", "Please enter your username.");
                }

                if (string.IsNullOrEmpty(MatKhau))
                {
                    ModelState.AddModelError("MatKhau", "Please enter your password.");
                }
                else
                {
                    // Thêm thông báo lỗi tổng quát nếu đã nhập cả hai trường nhưng không đúng
                    ModelState.AddModelError("", "Username or password is incorrect.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, trả về view DangNhap để hiển thị thông báo lỗi
                return View("DangNhap");
            }

            // Đăng nhập thành công, có thể lưu thông tin người dùng vào Session hoặc đối tượng ClaimsIdentity
            // sau đó chuyển hướng đến trang Index của HomeController
            if (user.Role == true)
            {
                // Nếu là Admin, chuyển hướng đến trang quản trị admin
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                Session["TaiKhoan"] = user.TaiKhoan;
                Session["ID_KhachHang"] = user.ID_KhachHang;  // Đảm bảo ID_KhachHang được lưu vào Session
                return RedirectToAction("Index", "Home");

            }
        }


        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string HoTen, string Email, string TaiKhoan, string MatKhau, string NhapLaiMatKhau)
        {

            if (ModelState.IsValid)
            {
                // Create a new user object with role = 0 (User)
                var newUser = new USER
                {
                    HoTen = HoTen,
                    Email = Email,
                    TaiKhoan = TaiKhoan,
                    MatKhau = MatKhau,
                    Role = false // Set default role to 0 for User
                };

                // Add the user to the database
                db.USERs.InsertOnSubmit(newUser);
                db.SubmitChanges();

                // Set a success message in ViewBag
                ViewBag.RegistrationSuccess = "Registration successful! Please login.";

                // Redirect to the registration view with the success message
                return View("DangKy");
            }

            // If there are validation errors, return to the registration page
            return View("DangKy");

        }
        public ActionResult DangXuat()
        {
            // Xóa thông tin đăng nhập từ Session
            Session["TaiKhoan"] = null;

            // Chuyển hướng về trang Index của HomeController
            return RedirectToAction("Index", "Home");
        }
     

    }
}
