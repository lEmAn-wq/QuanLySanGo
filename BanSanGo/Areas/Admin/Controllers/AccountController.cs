using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BanSanGo.Models;
using BanSanGo.Static;

namespace BanSanGo.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLySanGoEntities db = new QuanLySanGoEntities();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string emailOrSDT, string password)
        {
            password = TienIch.HashPassword(password);
            string a = TienIch.HashPassword("1");
            var user = db.NhanViens.SingleOrDefault(nv => (nv.Email == emailOrSDT || nv.SDT == emailOrSDT) && nv.MatKhau == password);
            if (user != null)
            {
                // Đăng nhập thành công
                Session["UserId"] = user.MaNV;
                return RedirectToAction("Index", "Home", new { area = "" }); // Chuyển hướng về vùng mặc định
            }
            // Đăng nhập thất bại
            ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
            return View();
        }

        public ActionResult Logout()
        {
            // Xóa tất cả các session
            Session.Clear();
            TienIch.NhanVien = null;
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        // GET: Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string emailOrSDT, string oldPassword, string newPassword, string confirmPassword)
        {
            oldPassword = TienIch.HashPassword(oldPassword);
            var user = db.NhanViens.SingleOrDefault(nv => (nv.Email == emailOrSDT || nv.SDT == emailOrSDT) && nv.MatKhau == oldPassword);
            if (user != null)
            {
                if (newPassword == confirmPassword)
                {
                    user.MatKhau = TienIch.HashPassword(newPassword);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Nhập lại mật khẩu mới không khớp");
            }
            else
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu hiện tại không đúng");
            }
            return View();
        }

        public ActionResult ThongTinTaiKhoan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaCV = new SelectList(db.ChucVus, "MaCV", "TenCV", nhanVien.MaCV);
            return View(nhanVien);
        }

        // POST: Admin/NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNV,TenNV,NgaySinh,GioiTinh,MaCV,SDT,SoCCCD,Email")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction($"ThongTinTaiKhoan/{nhanVien.MaNV}");
            }
            ViewBag.MaCV = new SelectList(db.ChucVus, "MaCV", "TenCV", nhanVien.MaCV);
            return View(nhanVien);
        }


        //// Lưu mã xác minh vào session hoặc tempdata
        //private void SendVerificationCode(string email)
        //{
        //    var verificationCode = new Random().Next(100000, 999999).ToString();
        //    var expirationTime = DateTime.Now.AddMinutes(1);  // Mã xác minh có hiệu lực trong 1 phút

        //    // Lưu mã xác minh và thời gian hết hạn vào Session
        //    Session["VerificationCode"] = verificationCode;
        //    Session["VerificationCodeExpiration"] = expirationTime;

        //    // Gửi email chứa mã xác minh
        //    TienIch.SendVerificationEmail(email, verificationCode);
        //}

        //========================================================================================================
    }
}