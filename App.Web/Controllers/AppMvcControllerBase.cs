using App.Core.Domain;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class AppMvcControllerBase :Controller
    {
        public CurrentSession AppSession
        {
            get
            {
                return CurrentSession.GetInstance();
            }
        }

        protected void AutoLogin(User user)
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            //退出当前用户
            authentication.SignOut();

            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName) };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.RoleName)));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            authentication.SignIn(
                new AuthenticationProperties { IsPersistent = false },
                new ClaimsIdentity(claims, "ApplicationCookie")
            );
        }
    }
}