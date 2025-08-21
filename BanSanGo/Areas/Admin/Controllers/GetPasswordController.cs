using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using BanSanGo.Models;
using BanSanGo.Static;

namespace BanSanGo.Areas.Admin.Controllers
{
    public class GetPasswordController : Controller
    {
        private readonly QuanLySanGoEntities db = new QuanLySanGoEntities();

        private static string verificationCode;
        private static DateTime codeGenerationTime;
        private const int codeValidityDurationInMinutes = 5;

        // GET: Admin/Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Admin/Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email không được để trống.");
                return View();
            }

            var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.Email == email);
            if (nhanVien == null)
            {
                ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                return View();
            }

            verificationCode = GenerateVerificationCode();
            codeGenerationTime = DateTime.Now;
            TienIch.SendVerificationEmail(email, verificationCode);

            TempData["Email"] = email;
            TempData.Keep("Email");
            return RedirectToAction("VerifyCode");
        }

        // GET: Admin/Account/VerifyCode
        public ActionResult VerifyCode()
        {
            return View();
        }

        // POST: Admin/Account/VerifyCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyCode(string inputCode)
        {
            if (string.IsNullOrEmpty(inputCode))
            {
                ModelState.AddModelError("", "Mã xác minh không được để trống.");
                TempData.Keep("Email");
                return View();
            }

            if (DateTime.Now > codeGenerationTime.AddMinutes(codeValidityDurationInMinutes))
            {
                ModelState.AddModelError("", "Mã xác minh đã hết hạn. Vui lòng gửi lại mã.");
                TempData.Keep("Email");
                return View();
            }

            if (inputCode == verificationCode)
            {
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError("", "Mã xác minh không chính xác.");
            TempData.Keep("Email");
            return View();
        }

        // GET: Admin/Account/ResetPassword
        public ActionResult ResetPassword()
        {
            string email = TempData["Email"] as string;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // POST: Admin/Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("", "Email không được để trống.");
                return RedirectToAction("ForgotPassword");
            }

            if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Mật khẩu không được để trống.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.Email == model.Email);
            if (nhanVien != null)
            {
                nhanVien.MatKhau = TienIch.HashPassword(model.NewPassword); // Lưu mật khẩu dưới dạng mã hóa
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Login", "Account"); // Giả sử bạn có trang đăng nhập
            }

            ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
            return View(model);
        }

        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString(); // Tạo mã ngẫu nhiên từ 100000 đến 999999
        }
    }
}
