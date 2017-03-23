using App.Core.DataAccess;
using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Dtos.Goods;
using App.Services;
using App.WebApi.Apis;
using App.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace App.Web.Apis
{
    public partial class GoodsPackageController : AppApiControllerBase
    {
        public readonly ICommonService _commonService;
        public readonly IGoodsPackageService _goodsPackageService;
        public readonly IGoodsService _goodsService;
        public GoodsPackageController(ICommonService commonService, IGoodsPackageService goodsPackageService, IGoodsService goodsService)
        {
            _commonService = commonService;
            _goodsPackageService = goodsPackageService;
            _goodsService = goodsService;
        }

        [Route("api/GoodsPackages")]
        public GridResult GetGoodsPackages(string sort = "CreationTime", string order = "asc", string filterRules = "")
        {
            var query = _commonService.GetRecords<GoodsPackage>("", sort, order, filterRules, new string[] { "Goods"});

            return new GridResult { total = query.Count, rows = query };
        }

        [Route("api/GetAllGoodsPackages")]
        public dynamic GetAllGoodsPackages() {
            var query = _commonService.GetRecords<GoodsPackage>("", "c.CreationTime DESC");
            var list = new List<object>();
            list.Add(new { text = "所有套餐", id = "" });
            list.AddRange(query.Select(s => new { text = string.Format("{0}(￥{1})", s.PackageName, s.Price), id = s.Id }).ToList());

            return list;
        }

        [HttpPost]
        [Route("api/PackageGoods/{packageId}")]
        public async Task<IHttpActionResult> LinkPackageGoods(Guid packageId, PackageGoodsViewModel input)
        {
            return Ok(await _goodsPackageService.LinkPackageGoods(packageId,input.GoodsId));
        }

        [HttpDelete]
        [Route("api/RemoveLinkPackageGoods/{packageId}")]
        public async Task<IHttpActionResult> RemoveLinkPackageGoods(Guid packageId)
        {
            return Ok(await _goodsPackageService.RemoveLinkPackageGoods(packageId, GetRequestDeleteModels()));
        }

        [HttpPost]
        [Route("api/GoodsPackages")]
        public async Task<IHttpActionResult> PostGoodsPackages(BulkDto<GoodsPackageDto> dto)
        {
            var result = await _goodsPackageService.SaveGoodsPackages(dto);

            return Ok(result);
        }

        [HttpPost, Route("api/SavePackageDescription/{packageId}")]
        public async Task<IHttpActionResult> SavePackageDescription(Guid packageId, GoodsPackageDto dto) {
            var result = await _goodsPackageService.SavePackageDescription(packageId, dto.Description);

            return Ok(result);
        }

        [Route("api/RedeemCodes")]
        public GridResult<RedeemCodeDto> GetRedeemCodes(int page, int rows, string sort = "r.CreationTime", string order = "asc", Guid? packageId = null, string filterRules = "")
        {
            //var where = "c.Package.Id != null";
            //if (packageId.HasValue)
            //    where += " and c.Package.Id=GUID'" + packageId.Value +"'";

            //var query = _commonService.GetPageRecords<RedeemCode>(where, page, rows, sort, order, out total,
            //    filterRules,
            //    new string[] { "Package" });

            return _goodsPackageService.GetRedeemCodes(page, rows, sort, order, packageId, filterRules);
        }

        [HttpPost]
        [Route("api/RedeemCodes/{packageId}")]
        public async Task<IHttpActionResult> CreateRedeemCodes(Guid packageId, CreateRedeemCodeInput input) {
            return Ok(await _goodsPackageService.CreateRedeemCodes(packageId, input.CreateNumber));
        }

        [HttpPost]
        [Route("api/EmptyRedeemCodes/{packageId}")]
        public async Task<IHttpActionResult> EmptyRedeemCodes(Guid packageId)
        {
            return Ok(await _goodsPackageService.EmptyRedeemCodes(packageId));
        }
    }
}
