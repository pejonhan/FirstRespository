using App.Core.Domain;
using App.Core.Dtos.Goods;
using App.Core.Helpers;
using App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class ExportController : AppMvcControllerBase
    {
        public readonly ICommonService _commonService;
        public readonly IGoodsPackageService _goodsPackageService;
        private readonly IDictionaryService _dictionaryService;

        public ExportController(ICommonService CommonService, IGoodsPackageService goodsPackageService, IDictionaryService dictionaryService)
        {
            _commonService = CommonService;
            _goodsPackageService = goodsPackageService;
            _dictionaryService = dictionaryService;
        }

        /// <summary>
        /// 导出兑换码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RedeemCodes(Guid id)
        {
            var package = _goodsPackageService.GetGoodsPackage(id);
            var data = _goodsPackageService.GetRedeemCodes(1, 10000, packageId: id);

            new ExportBuilder<RedeemCodeDto>()
                .Column(a => a.CodeNumber)
                .Column(a => a.IsExchange ? "是" : "否")
                .Column(a => a.ExchangeTime.HasValue ? a.ExchangeTime.Value.ToString("yyyy-MM-dd HH:mm") : "")
                .Column(a => a.UserName)
                .Column(a => a.OrderSN)
                .Export((List<RedeemCodeDto>)data.rows, package.PackageName+ ".xls", 
                @"{ 'root':{'rowspan':1,'sheetname':'"+ package.PackageName + @"','head':[
                    { 'title':'兑换码','width':120, 'cellregion':'0,0,0,0' },
                    { 'title': '已兑换', width: 80, 'cellregion':'0,0,1,1' },
                    { 'title': '兑换日期', width: 80, 'cellregion':'0,0,2,2' },
                    { 'title': '兑换人', width: 80, 'cellregion':'0,0,3,3' },
                    { 'title': '相关订单', width: 80, 'cellregion':'0,0,4,4' }
                ]}}");

            return new JsonResult();
        }

        /// <summary>
        /// 导出订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders() {
            var type = Request["type"] ?? "";
            var data = _commonService.GetRecords<Order>(type == "all" ? "" : "c.OrderStatus=0", "c.CreationTime DESC");
            var fileName = (type == "all" ? "所有" : "未发货") + "订单";
            var orderStatus = _dictionaryService.GetDictionaryText("OrderStatus");

            new ExportBuilder<Order>()
                .Column(a => a.OrderSN)
                .Column(a=> a.Amount.ToString() )
                .Column(a => orderStatus.Where(s=>s.Value == a.OrderStatus).Select(s=>s.Key).FirstOrDefault())
                .Column(a => a.Mobile)
                .Column(a => a.RedeemCodes)
                .Column(a => a.Consignee)
                .Column(a => a.ConsigneeSex)
                .Column(a => a.ConsigneePhone)
                .Column(a => a.ConsigneeProvince+a.ConsigneeCity+a.ConsigneeDistrict+a.ConsigneeAddress)
                .Column(a => a.TrackingNumber)
                .Export(data, fileName+".xls",
                @"{ 'root':{'rowspan':1,'sheetname':'" + fileName + @"','head':[
                    { 'title': '订单号','width':120, 'cellregion':'0,0,0,0' },
                    { 'title': '订单金额', width: 80, 'cellregion':'0,0,1,1' },
                    { 'title': '状态', width: 80, 'cellregion':'0,0,2,2' },
                    { 'title': '订货手机', width: 120, 'cellregion':'0,0,3,3' },
                    { 'title': '兑换码', width: 120, 'cellregion':'0,0,4,4' },
                    { 'title': '收货人', width: 100, 'cellregion':'0,0,5,5' },
                    { 'title': '性别', width: 60, 'cellregion':'0,0,6,6' },
                   	{ 'title': '联系电话', width: 120, 'cellregion':'0,0,7,7' },
                    {  'title': '收货地址', width: 220, 'cellregion':'0,0,8,8' },
                    { 'title': '运单号', width: 120, 'cellregion':'0,0,9,9' }
                ]}}");

            return new JsonResult();
        }
    }
}