using App.Core;
using App.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Views
{
    public abstract class AppWebViewPageBase : AppWebViewPageBase<dynamic>
    {

    }

    public abstract class AppWebViewPageBase<TModel> : WebViewPage<TModel>
    {
        protected string AppVersion
        {
            get
            {
                return AppConsts.AppVersion;
            }
        }

        protected CurrentSession AppSession {
            get {
                return CurrentSession.GetInstance();
            }
        }

        protected string GetConfig(string key) {
            return AppCacheHelper.GetSetting(key);
        }
    }
}