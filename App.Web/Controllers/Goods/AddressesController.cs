using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Core.DataAccess;
using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Services;

namespace App.Web.Controllers
{
    public class AddressesController : AppMvcControllerBase
    {
        private readonly ICommonService _commonService;
        private readonly IAddressService _addressService;
        public AddressesController(ICommonService commonService,IAddressService addressService) {
            _addressService = addressService;
            _commonService = commonService;
        }

        // GET: Addresses
        public ActionResult Index()
        {
            var items = _commonService.GetRecords<Address>("c.CreatorUserId=GUID'" + AppSession.UserId + "'", "c.CreationTime DESC");

            return View(items);
        }

        // GET: Addresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Addresses/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddressDto address)
        {
            return Json(await _addressService.CreateAddress(address));
        }

        // GET: Addresses/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = await _addressService.GetAddressAsync(id.Value);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Addresses/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id,AddressDto address)
        {
            return Json(await _addressService.UpdateAddress(id, address));
        }

        // POST: Addresses/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            return Json(await _addressService.DeleteAsync(id));
        }
    }
}
