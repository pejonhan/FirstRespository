using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.WebApi.ViewModels
{
    public class ChangePwdViewModel
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string Password { get; set; }
    }
}