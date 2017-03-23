using App.Core;
using App.Core.Common;
using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Helpers;
using App.Services;
using App.Services.Impls;
using App.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace App.WebApi.Apis
{
    
    // 系统及权限接口
    public class SystemController : AppApiControllerBase
    {
        public readonly ISystemService _systemService;
        public readonly ICommonService _commonService;
        public SystemController(ISystemService SystemService, ICommonService CommonService)
        {
            this._commonService = CommonService;
            this._systemService = SystemService;
        }

        [Route("api/all-buttons")]
        [HttpGet]
        public List<Button> GetAllButtons()
        {
            return _systemService.GetAllButton();
        }

        [Route("api/buttons")]
        public GridResult GetButton(int page, int rows, string sort = "CreationTime", string order = "asc", string filterRules = "")
        {
            int total;
            var where = "";

            var query = _commonService.GetPageRecords<Button>(where, page, rows, sort, order, out total,
                filterRules);

            return new GridResult { total = total, rows = query };
        }

        [HttpPost]
        [Route("api/buttons")]
        public async Task<IHttpActionResult> PostButton(BulkDto<ButtonDto> dto)
        {
            var result = await _systemService.SaveButtons(dto);

            return Ok(result);
        }

        [Route("api/CalendarEvents")]
        public dynamic GetCalendarEvent(DateTime start, DateTime end)
        {
            return _systemService.GetCalendarEvents(start, end)
                .Select(s => new { id = s.Id, start = s.StartDate, end = s.EndDate, title = s.Subject, color = s.Color });
        }

        [HttpPut]
        [Route("api/CalendarEvents/{id}")]
        public async Task<IHttpActionResult> PutCalendarEvent(Guid id, CalendarEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _systemService.UpdateCalendarEvent(id, dto);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/CalendarEvents")]
        public async Task<IHttpActionResult> PostCalendarEvent(CalendarEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _systemService.CreateCalendarEvent(dto);

            return Ok(result);
        }

        [HttpDelete]
        [Route("api/CalendarEvents")]
        public async Task<IHttpActionResult> DeleteCalendarEvent(DeleteViewModel model)
        {
            await _systemService.DeleteCalendarEventAsync(model.Id);

            return Ok(new OperationResult { success = true });
        }

        [Route("api/LoginLogs")]
        public GridResult GetLoginLog(int page, int rows, string sort = "CreationTime", string order = "desc", string filterRules = "")
        {
            int total;
            var where = "c.User.Id=GUID'" + AppSession.UserId + "'";

            var query = _commonService.GetPageRecords<LoginLog>(where, page, rows, sort, order, out total,
                filterRules, new[] { "User" });

            return new GridResult { total = total, rows = query };

        }

        [Route("api/navs")]
        public List<MenuTree> GetNavTrees()
        {
            var data = _systemService.GetAllMenuByUser();

            var result = new List<MenuTree>();
            Recursion(new MenuTree { id = null, children = new List<MenuTree>() }, result, data);
            return result;
        }

        [Route("api/Menus")]
        public List<MenuTree> GetMenu()
        {
            var data = _commonService.GetRecords<MenuPermission>("", "c.SortOrder DESC");
            var result = new List<MenuTree>();
            RecursionAll(new MenuTree { id = null, children = new List<MenuTree>() }, result, data.ToList());
            return result;
        }

        [HttpPut]
        [Route("api/Menus/{id}")]
        public async Task<IHttpActionResult> PutMenu(Guid id, MenuPermissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _systemService.UpdateMenu(id, dto);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/Menus")]
        public async Task<IHttpActionResult> PostMenu(MenuPermissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _systemService.CreateMenu(dto);

            return Ok(result);
        }

        [Route("api/save-menu-Buttons/{id}")]
        public async Task<IHttpActionResult> SaveMenuButtons(Guid id, BulkKeyDto<Guid> dto)
        {
            var result = await _systemService.SaveMenuButtons(id, dto);

            return Ok(result);
        }

        [Route("api/drop-menus")]
        [HttpPost]
        public IHttpActionResult DropMenus(DropViewModel model)
        {
            if (model.point == "append")
            {

            }

            if (model.point == "top")
            {

            }

            if (model.point == "bottom")
            {

            }

            return Ok(new OperationResult { success = true });
        }

        [HttpDelete]
        [Route("api/Menus")]
        public async Task<IHttpActionResult> DeleteMenu(DeleteViewModel model)
        {
            await _systemService.DeleteMenuAsync(model.Id);

            return Ok(new OperationResult { success = true });
        }

        [Route("api/Roles")]
        public List<Role> GetRole()
        {
            return _commonService.GetRecords<Role>("", "CreationTime", "DESC", includes: new string[] { "Permissions" });
            
        }

        [Route("api/AllRoles")]
        public List<Role> GetAllRoles()
        {
            return _systemService.GetAllRole();
        }

        [HttpPost]
        [Route("api/Roles")]
        public async Task<IHttpActionResult> PostRole(BulkDto<RoleDto> dto)
        {
            var result = await _systemService.SaveRoles(dto);

            return Ok(result);
        }

        [Route("api/save-role-permissions/{id}")]
        public IHttpActionResult SaveRoleButtons(Guid id, BulkKeyDto<Guid> dto)
        {
            var result = _systemService.SaveRoleButtons(id, dto);

            return Ok(result);
        }

        [Route("api/save-user-permissions/{id}")]
        public IHttpActionResult SaveUserButtons(Guid id, BulkKeyDto<Guid> dto)
        {
            var result = _systemService.SaveUserButtons(id, dto);

            return Ok(result);
        }

        [Route("api/Settings")]
        public GridResult GetSetting(int page, int rows, string sort = "CreationTime", string order = "asc", string filterRules = "")
        {
            int total;
            var where = "";

            var query = _commonService.GetPageRecords<Setting>(where, page, rows, sort, order, out total,
                filterRules);

            return new GridResult { total = total, rows = query };
        }

        [HttpPost]
        [Route("api/Settings")]
        public async Task<IHttpActionResult> PostSetting(BulkDto<SettingDto> dto)
        {
            var result = await _systemService.SaveSettings(dto);

            return Ok(result);
        }

        [Route("api/Models")]
        public GridResult GetModels() {
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("/App_Data/models.txt")))
            {
                var icons = new List<ModelViewModel>();
                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("#")) continue;
                    var modelText = line.Split(',');
                    icons.Add(new ModelViewModel { Id= modelText[0],
                        ModelName = modelText[0],
                        ModelDescription = modelText[1],
                        ModelGroup=modelText[2],
                        UISaveType = modelText[3],
                        ExistField = modelText[4]
                    });
                }

                return new GridResult { total = icons.Count, rows = icons };
            }
        }

        [HttpPost]
        [Route("api/Models")]
        public OperationResult CodeGeneration(GenerationViewModel gen) {
            var projectDirectory = GetSolutionDir();//ConfigurationManager.AppSettings["ProjectDirectory"];
            var hash = new Dictionary<string,object>();
            var properties = Type.GetType("App.Core.Domain." + gen.model.ModelName+ ",App.Core").GetProperties();

            hash.Add("dtoFields", properties);
            hash.Add("properties", properties);
            hash.Add("model", gen.model);

            var nvelocity = NVelocityHelper.GetInstance().GetTemplate(gen.option, "/admin/GenerationTemplates/", hash);

            var outputPath = string.Format(projectDirectory + gen.outputPath, gen.model.ModelGroup, gen.model.ModelName);

            FileOperate.WriteFile(outputPath, nvelocity.ToString());

            return new OperationResult { success = true,message="位置："+ outputPath };
        }

        #region Helpers
        /// <summary>
        /// 获取解决方案路径
        /// </summary>
        /// <returns></returns>
        private string GetSolutionDir() {
            var projectDirs = AppDomain.CurrentDomain.BaseDirectory.Split('\\');
            var solutionDir = "";
            var i = 0;
            foreach (var item in projectDirs)
            {
                if (i < projectDirs.Length - 2) solutionDir += item+"\\";
                i++;
            }
            return solutionDir;
        }
            //递归
        private void Recursion(MenuTree parentNode, IList<MenuTree> result, IList<MenuPermission> list)
        {
            foreach (var item in from c in list.OrderBy(s => s.SortOrder).OrderBy(s => s.CreationTime) where c.ParentId == parentNode.id && c.MenuType != "button" select c)
            {
                var buttons = list.Where(m => m.ParentId == item.Id && m.MenuType == "button").OrderBy(s => s.SortOrder).OrderBy(s => s.CreationTime).Select(s => new ButtonDto() { ButtonLink = s.MenuLink, ButtonName = s.MenuText, IconCls = s.IconCls, Id = s.Id }).ToList();
                var child = new MenuTree { id = item.Id, text = item.MenuText, ParentId = item.ParentId, MenuLink = item.MenuLink, MenuText = item.MenuText, Buttons = buttons, IconCls = item.IconCls, SortOrder = item.SortOrder, IsActivated=item.IsActivated, expanded = true, children = new List<MenuTree>() };
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

        private void RecursionAll(MenuTree parentNode, IList<MenuTree> result, IList<MenuPermission> list)
        {
            foreach (var item in from c in list.OrderBy(s => s.SortOrder).OrderBy(s=>s.CreationTime) where c.ParentId == parentNode.id select c)
            {
                var buttons = list.Where(m => m.ParentId == item.Id && m.MenuType == "button").OrderBy(s => s.SortOrder).OrderBy(s => s.CreationTime).Select(s => new ButtonDto() { ButtonLink = s.MenuLink, ButtonName = s.MenuText, IconCls = s.IconCls, Id = s.Id }).ToList();
                var child = new MenuTree { id = item.Id, text = item.MenuText, ParentId = item.ParentId, MenuLink = item.MenuLink, MenuText = item.MenuText,Buttons=buttons,MenuType=item.MenuType, IconCls=item.IconCls, SortOrder = item.SortOrder, IsActivated = item.IsActivated, expanded = true, children = new List<MenuTree>() };
                if (item.ParentId == null)
                {
                    result.Add(child);
                }
                else
                {
                    child.leaf = true;
                    parentNode.children.Add(child);
                }
                RecursionAll(child, result, list);
            }
        }

        private string GetExiseField(PropertyInfo[] propertys) {
            foreach (var item in propertys)
            {
                DescriptionAttribute attr = Attribute.GetCustomAttribute(item,
                    typeof(DescriptionAttribute), false) as DescriptionAttribute;
                if (attr != null && attr.Description == "exise")
                {
                    return item.Name;
                }
            }

            return "";
        }
        #endregion
    }
}
