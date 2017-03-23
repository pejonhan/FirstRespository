using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Helpers;
using App.Services;
using App.WebApi.ViewModels;

namespace App.WebApi.Apis
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public class UserController : AppApiControllerBase
    {
        public readonly ICommonService _commonService;
        public readonly IUserService _userService;
        public readonly ISystemService _systemService;
        
        public UserController(ICommonService CommonService, IUserService UserService, ISystemService systemService)
        {
            _commonService = CommonService;
            _userService = UserService;
            _systemService = systemService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="rows">每页行数</param>
        /// <param name="sort">排序字段</param>
        /// <param name="order">排序方式</param>
        /// <param name="teamId">部门Id</param>
        /// <param name="filterRules">过滤规则</param>
        /// <returns></returns>
        [Route("api/Users")]
        public GridResult GetUser(int page, int rows, string sort = "CreationTime", string order = "asc", Guid? teamId = null, string filterRules = "")
        {
            int total;
            var where = "1=1";
            if (teamId != null)
                where += " and c.Team.Id=GUID'" + teamId + "'";

            var query = _commonService.GetPageRecords<User>(where, page, rows, sort, order, out total,
                filterRules, new[] { "Team", "Roles" });

            return new GridResult { total = total, rows = query };
        }

        /// <summary>
        /// 根据用户Id获取授权菜单Id列表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/GetAllMenuByUserId/{id}")]
        public Guid[] GetAllMenuByUserId(Guid id) {
            return _systemService.GetAllMenuByUserId(id).Select(s => s.Id).ToArray();
        }

        /// <summary>
        /// 根据用户Id更新用户资料
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="dto">用户实体对象</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Users/{id}")]
        public async Task<IHttpActionResult> PutUser(Guid id, UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUser(id, dto);

            return Ok(result);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="dto">用户实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Users")]
        public async Task<IHttpActionResult> PostUser(UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(dto);

            return Ok(result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Users")]
        public async Task<IHttpActionResult> DeleteUser()
        {
            await _userService.DeleteAsync(GetRequestDeleteModels());

            return Ok(new OperationResult());
        }

        /// <summary>
        /// 更改用户密码
        /// </summary>
        /// <param name="dto">用户实体对象</param>
        /// <returns></returns>
        [Route("api/change-user-pwd")]
        public async Task<IHttpActionResult> ChangeUserPassword(ChangePwdDto dto)
        {
            var result = await _userService.ChangeUserPassword(dto);

            return Ok(result);
        }

        /// <summary>
        /// 获取部门树型列表
        /// </summary>
        /// <returns></returns>
        [Route("api/Teams")]
        public List<TeamTree> GetTeam()
        {
            var data = _userService.GeTeams();
            var result = new List<TeamTree>();
            Recursion(new TeamTree { id = null, children = new List<TeamTree>() }, result, data.ToList());
            return result;
        }

        /// <summary>
        /// 根据部门Id修改部门资料
        /// </summary>
        /// <param name="id">部门Id</param>
        /// <param name="dto">部门实体对象</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Teams/{id}")]
        public async Task<IHttpActionResult> PutTeam(Guid id, TeamDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateTeam(id, dto);

            return Ok(result);
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="dto">部门实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Teams")]
        public async Task<IHttpActionResult> PostTeam(TeamDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateTeam(dto);

            return Ok(result);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="model">页面部门对象（包含部门Id和删除Remark）</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Teams")]
        public async Task<IHttpActionResult> DeleteTeam(DeleteViewModel model)
        {
            await _userService.DeleteTeamAsync(model.Id);

            return Ok(new OperationResult { success = true });
        }

        #region Helpers
        //递归
        private void Recursion(TeamTree parentNode, IList<TeamTree> result, IList<Team> list)
        {
            foreach (var item in from c in list where c.ParentId == parentNode.id select c)
            {
                var child = new TeamTree { id = item.Id, text = item.TeamName, ParentId = item.ParentId, TeamName = item.TeamName, TeamDesc = item.TeamDesc, expanded = true, children = new List<TeamTree>() };
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