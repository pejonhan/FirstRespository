using App.Core.Common;
using System;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;
using App.Services;
using System.Threading.Tasks;
using App.Core.Helpers;
using App.Core;
using App.Core.Dtos;
using System.Collections.Generic;

namespace App.Web.Controllers
{
    public class HomeController : AppMvcControllerBase
    {
        public readonly IUserService _userService;
        public readonly ISystemService _systemService;
        public HomeController(
            IUserService userService,
            ISystemService systemService)
        {
            _userService = userService;
            _systemService = systemService;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WeiXinOAuth() {
            string echoStr = Request.QueryString["echoStr"];

            if (CheckSignature() && !string.IsNullOrEmpty(echoStr))
            {
                return Content(echoStr);
            }

            return Content("fail");
        }

        public ActionResult WeiXinLogin() {
            //获取appId,appSecret的配置信息
            string appId = ConfigurationManager.AppSettings["weixin:appid"];
            string appSecret = ConfigurationManager.AppSettings["weixin:secret"];
            var weixinOAuth = new WeiXinOAuth();
            //微信第一次握手后得到的code 和state
            string _code = Request["code"];
            string _state = Request["state"];
            if (string.IsNullOrEmpty(_code) || _code == "authdeny")
            {
                if (string.IsNullOrEmpty(_code))
                {
                    //发起授权(第一次微信握手)
                    string _authUrl = weixinOAuth.GetWeiXinCode(appId, appSecret, Server.UrlEncode(Request.Url.ToString()));
                    Response.Redirect(_authUrl, true);
                }
                else
                {   // 用户取消授权
                    Response.Redirect("~/Error.html", true);
                }
            }
            else
            {
                //获取微信的Access_Token（第二次微信握手）
                var modelResult = weixinOAuth.GetWeiXinAccessToken(appId, appSecret, _code);
                //获取微信的用户信息(第三次微信握手)
                var _userInfo = weixinOAuth.GetWeiXinUserInfo(modelResult.SuccessResult.access_token, modelResult.SuccessResult.openid);
                //用户信息（判断是否已经获取到用户的微信用户信息）
                if (_userInfo.Result && _userInfo.UserInfo.openid != "")
                {
                    //如果存在则自动登录
                    var user = _userService.GetUserByOpenId(_userInfo.UserInfo.openid);
                    if (user != null)
                    {
                        AutoLogin(user);
                    }

                    Session["openid"] = _userInfo.UserInfo.openid;

                    return new RedirectResult(Session["ReturnUrl"] == null ? "/exchange/" : Session["ReturnUrl"].ToString());
                }
                else
                {
                    throw new Exception("获取用户OpenId失败");
                }
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        /// <returns></returns>
        private bool CheckSignature()
        {
            string signature = Request.QueryString["signature"];
            string timestamp = Request.QueryString["timestamp"];
            string nonce = Request.QueryString["nonce"];

            string[] arrTmp = { "498435433245", timestamp, nonce };
            Array.Sort(arrTmp);
            string tmpStr = string.Join("", arrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            if (tmpStr != null)
            {
                tmpStr = tmpStr.ToLower();
                return tmpStr == signature;
            }
            return false;
        }
    }
}