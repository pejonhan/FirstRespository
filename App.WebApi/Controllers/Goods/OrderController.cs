using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Helpers;
using App.Services;
using App.WebApi.Apis;
using App.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace App.Web.Apis
{
    public partial class OrderController : AppApiControllerBase
    {
        public ICommonService _commonService;
        public IOrderService _orderService;
        public OrderController(ICommonService commonService, IOrderService orderService)
        {
            _commonService = commonService;
            _orderService = orderService;
        }

        [Route("api/Orders")]
        public GridResult GetOrders(int page, int rows, string sort = "CreationTime", string order = "desc", string filterRules = "")
        {
            int total;
            var where = "";

            var query = _commonService.GetPageRecords<Order>(where, page, rows, sort, order, out total,
                filterRules);

            return new GridResult { total = total, rows = query };
        }

        [HttpPost]
        [Route("api/GetOrder/{orderSn}")]
        public async Task<IHttpActionResult> SetOrderStatus(string orderSn)
        {
            var result = await _orderService.GetOrderAsync(orderSn);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/SetOrderStatus/{id}")]
        public async Task<IHttpActionResult> SetOrderStatus(Guid id, OrderStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.SetOrderStatus(id, dto);

            return Ok(result);
        }

        [HttpPut]
        [Route("api/Orders/{id}")]
        public async Task<IHttpActionResult> PutOrder(Guid id, OrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.UpdateOrder(id, dto);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/Orders")]
        public async Task<IHttpActionResult> PostOrder(OrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.CreateOrder(dto);

            return Ok(result);
        }
		        [HttpDelete]
        [Route("api/Orders")]
        public async Task<IHttpActionResult> DeleteOrder()
        {
            await _orderService.DeleteAsync(GetRequestDeleteModels());

            return Ok(new OperationResult { success = true });
        }
    }
}
