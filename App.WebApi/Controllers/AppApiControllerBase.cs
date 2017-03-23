using App.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace App.WebApi.Apis
{
    [Authorize]
    public class AppApiControllerBase : ApiController
    {
        public CurrentSession AppSession
        {
            get
            {
                return CurrentSession.GetInstance();
            }
        }

        public Guid[] GetRequestDeleteModels() {
            var deletes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeleteViewModel>>(HttpContext.Current.Request["input"]);
            return deletes.Select(s => s.Id).ToArray();
        }
    }
}
