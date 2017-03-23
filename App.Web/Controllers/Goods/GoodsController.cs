using App.Core;
using App.Core.Common;
using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Helpers;
using App.Services;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class GoodsController : AppMvcControllerBase
    {
        public readonly IGoodsPackageService _goodsPackageService;
        public readonly IOrderService _orderService;
        public readonly IUserService _userService;
        public readonly ISystemService _systemService;
        private readonly IAddressService _addressService;
        public GoodsController(IGoodsPackageService goodsPackageService, 
            IOrderService orderService, 
            IUserService userService,
            ISystemService systemService,
            IAddressService addressService)
        {
            _goodsPackageService = goodsPackageService;
            _orderService = orderService;
            _userService = userService;
            _systemService = systemService;
            _addressService = addressService;
        }

        public async Task<ActionResult> CheckRedeemCodeAsync(string redeemCode)
        {
            var entity = await _goodsPackageService.GetRedeemCodeBySNAsync(redeemCode);

            if (entity == null) return Json(new OperationResult { success = false, message = "兑换码不存在" });
            if (entity.IsExchange) return Json(new OperationResult { success = false, message = "该兑换码已经兑换过，不能再次使用" });
            if (entity.Package == null) return Json(new OperationResult { success = false, message = "该兑换码对应的套餐已经不存在，请联系客服获取帮助。" });

            return Json(new OperationResult<GoodsPackageDto> { success = true, message= entity.Package.Description });
        }

        public async Task<ActionResult> SaveOrder(OrderDto dto)
        {
            if (!AppSession.IsLogin)
            {
                if (Session["mobileCode"] == null)
                {
                    return Json(new OperationResult { success = false, message = "手机验证码已过期，请点击上一步重新获取" });
                }
                if (Session["mobileCode"].ToString() != dto.MobileCode)
                {
                    return Json(new OperationResult { success = false, message = "手机验证码错误，请点击上一步重新获取" });
                }
            }

            //如果用户不存在则创建
            var user = await _userService.GetUserByUserNameAsync(dto.Mobile);
            if (user == null)
            {
                var role = _systemService.GetRoleByName(AppCacheHelper.GetSetting(AppConsts.SettingKeyUserDefaultRole));
                if (role != null)
                {
                    var password = Session["mobileCode"].ToString();
                    var createdResult = await _userService.CreateUserAsync(new UserDto
                    {
                        UserName = dto.Mobile,
                        Password = password.ToString(),
                        ConfrimPassword = password.ToString(),
                        Mobile = dto.Mobile,
                        OpenId = Session["openid"] == null ? "" : Session["openid"].ToString(),
                        RoleId = new List<Guid> { role.Id }
                    });

                    user = createdResult.model;
                }
            }
            else {
                if (Session["mobileCode"] != null) await _userService.ChangeUserPassword(user.Id, Session["mobileCode"].ToString());
                if(Session["openid"] !=null) await _userService.ChangeUserOpenId(user.Id, Session["openid"].ToString());
            }

            if (!AppSession.IsLogin || (AppSession.IsLogin && AppSession.UserId != user.Id))
                //用该用户登录
                AutoLogin(user);

            var redeemCode = await _goodsPackageService.GetRedeemCodeBySNAsync(dto.RedeemCode);
            if (redeemCode == null) return Json(new OperationResult { success = false, message = "兑换码不存在" });
            if (redeemCode.IsExchange) return Json(new OperationResult { success = false, message = "该兑换码已经兑换过，不能再次使用" });

            var result = await _orderService.CreateOrder(dto,user.Id);
            if (result.success) Session["mobileCode"] = null;
            
            return Json(result);
        }
    }
}