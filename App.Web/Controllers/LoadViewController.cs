using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class LoadViewController : Controller
    {
        // GET: LoadView
        public ActionResult Index()
        {
            var viewUrl = Request["viewUrl"];
            if (!string.IsNullOrEmpty(viewUrl) && viewUrl.StartsWith("app/")) {
                viewUrl = "~/admin/"+ viewUrl;
            }

            return View(viewUrl);
        }
    }
}