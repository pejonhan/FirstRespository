using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using App.Core;
using App.Core.DataAccess;
using App.Core.Helpers;
using Newtonsoft.Json;
using App.Services;
using App.Core.Domain;

namespace App.WebApi.Apis
{
    public class InitController : AppApiControllerBase
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly IUserService _userService;
        public InitController(IDictionaryService dictionaryService, IUserService userService) {
            _dictionaryService = dictionaryService;
            _userService = userService;
        }

        [Route("api/init")]
        public HttpResponseMessage GetUserInit()
        {
            var curUser = _userService.GetUserById(AppSession.UserId.Value);
            var initData = "var WebPlus = " + JsonConvert.SerializeObject(new
            {
                config = AppCacheHelper.GetSettings(),
                user = new
                {
                    curUser.UserName, 
                    curUser.Email,
                    curUser.Mobile,
                    curUser.LoginLogs.Count
                },
                dictionarys = GetDictionarys()
            });

            return new HttpResponseMessage { Content = new StringContent(initData, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        [Route("api/get-icons")]
        public List<string> GetIcons()
        {
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath("/App_Data/icons.txt")))
            {
                var icons = new List<string>();
                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    icons.Add(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)));
                }

                return icons;
            }
        }

        private Dictionary<string,Dictionary<int, string>> GetDictionarys() {
            var result = new Dictionary<string,Dictionary<int, string>>();
            foreach (var item in _dictionaryService.GetAll())
            {
                if (string.IsNullOrEmpty(item.Dictionarys)) continue;
                result.Add(item.Key, GetDictionary(item.Dictionarys));
            }
            return result;
        }

        private Dictionary<int, string> GetDictionary(string dictionarys) {
            var result = new Dictionary<int, string>();
            var i = 0;
            foreach (var item in dictionarys.Split(','))
            {
                result.Add(i, item);
                i++;
            }
            return result;
        }
    }
}
