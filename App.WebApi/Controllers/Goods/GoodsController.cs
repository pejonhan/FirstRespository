using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Helpers;
using App.Services;
using App.Services.Impls;
using App.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace App.WebApi.Apis
{
    public partial class GoodsController : AppApiControllerBase
    {
        public readonly ICommonService _commonService;
        public readonly IGoodsService _goodsService;
        public GoodsController(ICommonService CommonService, IGoodsService GoodsService)
        {
            this._commonService = CommonService;
            this._goodsService = GoodsService;
        }

        [Route("api/Goods")]
        public GridResult GetGoodss(int page, int rows, string sort = "CreationTime", string order = "asc", Guid? teamId = null, string filterRules = "",string q = "")
        {
            int total;
            var where = "1=1";
            //if (!string.IsNullOrEmpty(q)) {
            //    where += " and (";
            //    foreach (var item in q.Split(','))
            //    {
            //        where += "c.GoodsName like '%" + item + "%' or ";
            //    }
            //    where = where.Remove(where.Length - 4, 4);
            //    where += ")";
            //}

            var query = _commonService.GetPageRecords<Goods>(where, page, rows, sort, order, out total,
                filterRules, new[] { "Brand", "Categorys" });

            return new GridResult { total = total, rows = query };
        }

        [HttpGet]
        [Route("api/Goods/{id}")]
        public async Task<Goods> GetGoods(Guid id) {
            return await _goodsService.GetGoods(id);
        }

        [HttpPut]
        [Route("api/Goods/{id}")]
        public async Task<IHttpActionResult> PutGoods(Guid id, GoodsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _goodsService.UpdateGoods(id, dto);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/Goods")]
        public async Task<IHttpActionResult> PostGoods(GoodsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _goodsService.CreateGoods(dto);

            return Ok(result);
        }
        [HttpDelete]
        [Route("api/Goods")]
        public async Task<IHttpActionResult> DeleteGoods()
        {
            await _goodsService.DeleteAsync(GetRequestDeleteModels());

            return Ok(new OperationResult { success = true });
        }

        [Route("api/GetAllGoodsBrands")]
        public List<GoodsBrand> GetAllGoodsBrands() {
            return _commonService.GetRecords<GoodsBrand>("", "c.CreationTime asc");
        }

        [Route("api/brands")]
        public GridResult GetGoodsBrands(int page, int rows, string sort = "CreationTime", string order = "asc", Guid? teamId = null, string filterRules = "")
        {
            int total;
            var where = "";

            var query = _commonService.GetPageRecords<GoodsBrand>(where, page, rows, sort, order, out total,
                filterRules);

            return new GridResult { total = total, rows = query };
        }

        [HttpPost]
        [Route("api/brands")]
        public async Task<IHttpActionResult> PostBrands(BulkDto<BrandDto> dto)
        {
            var result = await _goodsService.SaveBrands(dto);

            return Ok(result);
        }

        [Route("api/GoodsCategorys")]
        public List<GoodsCategoryTree> GetGoodsCategory()
        {
            var data = _goodsService.GetCategorys();
            var result = new List<GoodsCategoryTree>();
            Recursion(new GoodsCategoryTree { id = null, children = new List<GoodsCategoryTree>() }, result, data.ToList());
            return result;
        }

        [HttpPut]
        [Route("api/GoodsCategorys/{id}")]
        public async Task<IHttpActionResult> PutGoodsCategory(Guid id, GoodsCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _goodsService.UpdateCategory(id, dto);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/GoodsCategorys")]
        public async Task<IHttpActionResult> PostGoodsCategory(GoodsCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _goodsService.CreateCategory(dto);

            return Ok(result);
        }

        [HttpDelete]
        [Route("api/GoodsCategorys")]
        public async Task<IHttpActionResult> DeleteGoodsCategory(DeleteViewModel model)
        {
            await _goodsService.DeleteCategoryAsync(model.Id);

            return Ok(new OperationResult { success = true });
        }

        #region Helpers
        //递归
        private void Recursion(GoodsCategoryTree parentNode, IList<GoodsCategoryTree> result, IList<GoodsCategory> list)
        {
            foreach (var item in from c in list where c.ParentId == parentNode.id select c)
            {
                var child = new GoodsCategoryTree { id = item.Id, text = item.CategoryName, ParentId = item.ParentId, CategoryName = item.CategoryName, expanded = true, children = new List<GoodsCategoryTree>() };
                if (item.ParentId == null)
                {
                    result.Add(child);
                }
                else
                {
                    child.leaf = true;
                    parentNode.children.Add(child);
                }
                Recursion(child, result, list);
            }
        }
        #endregion
    }
}
