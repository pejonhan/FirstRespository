using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住此次登录！")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "验证码")]
        public string MobileCode { get; set; }
    }

    public class LoginResult {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
        public List<string> Roles { get; set; }
    }
}
