using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanSanGo.Areas.Admin
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string InputCode { get; set; }
        public bool IsVerificationStep { get; set; }
        public string Message { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}