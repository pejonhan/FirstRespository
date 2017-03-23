using App.Core.Common;
using App.Core.Domain.BaseObject;
using App.Core.Helpers;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class CommonController : AppMvcControllerBase
    {
        // GET: Common
        public ActionResult SendMobileCode(string mobile)
        {
            if (!string.IsNullOrEmpty(mobile)) {
                var mobileCode = Utils.GetRandomNum(1000, 9999).ToString();
                Session["mobileCode"] = mobileCode;

                string result = UCPAASHelper.SendSMS(mobile, "26592", mobileCode + ",1");
                if (result.IndexOf("000000") >= 0) {
                    return Json(new OperationResult { success = true });
                }

                return Json(new OperationResult { success = false,message="短信接口异常，验证码发送失败" });
            }

            return Json(new OperationResult { success = false, message="请输入正确的手机号码" });
        }

        public ActionResult CheckMobileCode(string mobileCode) {
            if (Session["mobileCode"] == null) {
                return Json(new OperationResult { success = false,message="手机验证码不存在或已失效，请重新获取。" });
            }

            if (!mobileCode.Equals(Session["mobileCode"])){
                return Json(new OperationResult { success = false, message = "手机验证码不正确，请重新输入。" });
            }

            return Json(new OperationResult { success = true });
        }
    }
}