using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        // GET: Dashboard/Settings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GeneralSettings()
        {
            GeneralSettingsViewModel model = new GeneralSettingsViewModel();

            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetGeneralSettings", User, GetRequest: true));

            if (response is Error)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.OK, (response as Error).ErrorMessage);
            }
            else
                model = response.GetValue("Result").ToObject<GeneralSettingsViewModel>();

            model.SetSharedData(User);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeneralSettings(GeneralSettingsViewModel model)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/UpdateGeneralSettings", User, model));

                if (response is Error)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.OK, (response as Error).ErrorMessage);
                }

                return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.OK, "Internal Server Error");
            }
        }
    }
}