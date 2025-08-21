using BanSanGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace BanSanGo.Static
{
    public class TienIch
    {
        public static NhanVien NhanVien { get; set; }

        public const string sdt = "";
        public const string email = "";
        public const string matKhauUngDung = "";

        public const string thongBaoXoa = "Bạn có chắc muốn xóa nội dung này không";


        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static NhanVien GetInFoStaff(QuanLySanGoEntities db, HttpSessionStateBase session)
        {
            var userId = session["UserId"] as int?;
            if (userId.HasValue)
            {
                return db.NhanViens.SingleOrDefault(nv => nv.MaNV == userId.Value);
            }
            return null;
        }

        public static void SendVerificationEmail(string email, string verificationCode)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(TienIch.email); // Email của bạn
            mail.Subject = "Xác minh email";
            mail.Body = $"Mã xác minh của bạn là: {verificationCode}. Mã chỉ có hiệu lực trong 1 phút.";
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential(TienIch.email, TienIch.matKhauUngDung), // Email và mật khẩu ứng dụng của bạn
                EnableSsl = true
            };

            smtp.Send(mail);

            //// Log lỗi gửi email
            //System.Diagnostics.Debug.WriteLine("Lỗi gửi email: " + ex.Message);
            //throw new Exception("Không thể gửi email xác minh.");
        }

    }
}