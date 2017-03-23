using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using App.Core.Common;
using App.Core.DataAccess;
using App.Core.Domain;
using App.Web.ViewModels;
using Microsoft.Owin.Security;
using App.Services;
using App.Core.Domain.BaseObject;
using App.Core.Helpers;
using App.Core;
using App.Core.Dtos;

namespace App.Web.Controllers
{
    public class AccountController : AppMvcControllerBase
    {
        //test
        public readonly IUserService _userService;
        public readonly ISystemService _systemService;

        public AccountController(IUserService userService, ISystemService systemService) {
            _userService = userService;
            _systemService = systemService;
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? "/exchange/index" : returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await this.ValidateUserName(model.UserName);
                if (user != null)
                {
                    var isValidate = PasswordHash.ValidatePassword(model.Password, user.PasswordHash);
                    if (isValidate)
                    {
                        var authentication = HttpContext.GetOwinContext().Authentication;

                        var claims = new List<Claim> {new Claim(ClaimsIdentity.DefaultNameClaimType, model.UserName)};
                        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));

                        authentication.SignIn(
                            new AuthenticationProperties { IsPersistent = model.RememberMe},
                            new ClaimsIdentity(claims, "ApplicationCookie")
                        );

                        //添加到历史登录
                        await _userService.CreateLoginLog(user.UserId);
                        if (Session["openid"] != null) {
                            await _userService.ChangeUserOpenId(user.UserId, Session["openid"].ToString());
                        }

                        return RedirectToLocal(returnUrl);
                    }
                }

                ModelState.AddModelError(String.Empty, "用户名或密码有误！");
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            authentication.SignOut();

            return Redirect("/account/login?ReturnUrl="+Request["returnUrl"]);
        }

        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (Session["mobileCode"] == null)
            {
                ModelState.AddModelError(String.Empty, "手机验证码已过期");
                return View(model);
            }
            if (Session["mobileCode"].ToString() != model.MobileCode)
            {
                ModelState.AddModelError(String.Empty, "手机验证码错误");
                return View(model);
            }

            //如果用户不存在则创建
            var user = await _userService.GetUserByUserNameAsync(model.Mobile);
            if (user == null)
            {
                var role = _systemService.GetRoleByName(AppCacheHelper.GetSetting(AppConsts.SettingKeyUserDefaultRole));
                if (role != null)
                {
                    var password = Session["mobileCode"].ToString();
                    var createdResult = _userService.CreateUser(new UserDto
                    {
                        UserName = model.Mobile,
                        Password = password.ToString(),
                        ConfrimPassword = password.ToString(),
                        Mobile = model.Mobile,
                        OpenId = Session["openid"] != null ? Session["openid"].ToString() : "",
                        RoleId = new List<Guid> { role.Id }
                    });

                    user = createdResult.model;
                }
            }
            else
            {
                await _userService.ChangeUserPassword(user.Id, Session["mobileCode"].ToString());
            }

            Session["mobileCode"] = null;

            return new RedirectResult("/account/login?success=true");
        }

        #region helpers
        public async Task<LoginResult> ValidateUserName(string userName)
        {
            var entity = await _userService.GetUserByUserNameAsync(userName);
            if (entity != null)
            {
                return new LoginResult
                {
                    UserId = entity.Id,
                    PasswordHash = entity.PasswordHash,
                    Roles = entity.Roles.Select(r=>r.RoleName).ToList()
                };
            }

            return null;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }
        #endregion
    }
}