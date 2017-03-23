using App.Core.Common;
using App.Core.Domain;
using App.Core.Dtos;
using App.Services;
using App.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class ExchangeController : AppMvcControllerBase
    {
        public readonly ICommonService _commonService;
        private readonly IDictionaryService _dictionaryService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        public ExchangeController(ICommonService commonService, IDictionaryService dictionaryService, 
            IOrderService orderService, IUserService userService, IAddressService addressService) {
            _commonService = commonService;
            _dictionaryService = dictionaryService;
            _orderService = orderService;
            _userService = userService;
            _addressService = addressService;
        }

        // GET: Exchange
        public async Task<ActionResult> Index()
        {
            if (Session["openid"] == null && Utils.IsWeixin())
            {
                Session["ReturnUrl"] = Request.Url.PathAndQuery;
                return new RedirectResult("/home/WeiXinLogin");
            }

            if (AppSession.UserId == null)
            {
                return new RedirectResult("/account/login?ReturnUrl=%2Fexchange%2Findex");
            }

            ViewBag.Address = new List<AddressDto>();

            if (AppSession.IsLogin) {
                var user = _userService.GetUserById(AppSession.UserId.Value);
                if (user != null) {
                    ViewBag.Mobile = user.Mobile;
                    ViewBag.Address = await _addressService.GetUserAddress(user.Id);
                }
            }

            return View(await _orderService.GetLastAddressAsync(AppSession.UserId));
        }

        public ActionResult Orders(int page=1)
        {
            if (Session["openid"] == null && Utils.IsWeixin())
            {
                Session["ReturnUrl"] = Request.Url.PathAndQuery;
                return new RedirectResult("/home/WeiXinLogin");
            }

            if (AppSession.UserId == null) {
                return new RedirectResult("/account/login?ReturnUrl=%2Fexchange%2Forders");
            }

            var total = 0;
            var pageSize = 10;
            var items = _commonService.GetPageRecords<Order>("c.CreatorUserId=GUID'"+ AppSession.UserId +"'", page, pageSize, "CreationTime", "DESC", out total);
            var model = new OrderViewModel<Order>
            {
                Items=items,
                OrderStatus = _dictionaryService.GetDictionaryText("OrderStatus"),
                Pager=new PagerViewModel
                {
                    Total = total,
                    PageIndex =page,
                    PageSize = pageSize
                }
            };

            return View(model);
        }
    }
}