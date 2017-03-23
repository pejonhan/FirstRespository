using System.Linq;
using System.Runtime.Caching;
using System.Web.Helpers;
using System.Web.Http;
using App.Core.Domain.BaseObject;

namespace App.WebApi.Apis
{
    public class CommonController : AppApiControllerBase
    {
        /// <summary>
        /// 公共接口--清除缓存
        /// </summary>
        /// <returns></returns>
        [Route("api/common-clear-cache")]
        public IHttpActionResult ClearAllCache()
        {
            var cache = MemoryCache.Default;
            var items = cache.AsEnumerable();
            foreach (var item in items.Where(item => item.Key.StartsWith("app")))
            {
                WebCache.Remove(item.Key);
            }

            return Ok(new OperationResult() { success = true });
        }
    }
}
