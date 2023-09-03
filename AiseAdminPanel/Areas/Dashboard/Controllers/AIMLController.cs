using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    [Authorize]
    public class AIMLController : Controller
    {
        // GET: Dashboard/AIML
        public ActionResult Index()
        {
            DocumentModels model = new DocumentModels();
            model.SetSharedData(User);
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Index(DocumentModels model)
        {
            try
            {

                model.SetSharedData(User);
                model.User_Id = model.UserId;
                var response = await ApiCall.CallApi("api/Documents/AddEditDocument", User, model);

                if (response == null)
                {
                    return Json(new { error = true, responseText = "Failed" });
                }
                if (response is Error)
                {
                    string ErrorMessage = (response as Error).ErrorMessage;
                    ModelState.AddModelError("", ErrorMessage);

                    TempData["Message"] = ErrorMessage;
                    TempData["Success"] = "false";

                    return Json(new { error = true, responseText = "Failed", Message = ErrorMessage });
                }
                else
                {
                    var resp = response.GetValue("Result").ToObject<String>();

                    TempData["Message"] = resp;
                    TempData["Success"] = "true";

                    return Json(new { success = true, responseText = "Success", Message = resp }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public ActionResult ManageDocuments(int? UserId = 0)
        {
            DocumentModels returnModel = new DocumentModels();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Documents/GetDocumentsByUserId", User, null, true, false, null, "UserId=" + UserId));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel.Documents = response.GetValue("Result").ToObject<List<DocumentViewModel>>();
            }

            returnModel.SetSharedData(User);
            return PartialView("_ManageDocuments", returnModel);
        }






        public ActionResult ContextIndex(int? ContextId = 0)
        {
            ContextModels model = new ContextModels();
            model.SetSharedData(User);

            if (ContextId != 0)
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetContextById", User, null, true, false, null, "ContextId=" + ContextId.Value));
                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    model.Context = response.GetValue("Result").ToObject<ContextBindingModel>();
                }
            }
            else
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetContextByUserId", User, null, true, false, null, "UserId=" + model.UserId));
                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    model.Context = response.GetValue("Result").ToObject<ContextBindingModel>();
                }


            }
            return View(model);
        }

        public JsonResult DeleteContext(int ContextId = 0)
        {
            ContextModels model = new ContextModels();
            model.SetSharedData(User);


            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)EntityTypes.Context, "Id=" + ContextId, "User_Id=" + model.UserId));

            if (response == null)
            {
                return Json(new { error = true, responseText = "Failed" });
            }
            if (response is Error)
            {
                string ErrorMessage = (response as Error).ErrorMessage;
                ModelState.AddModelError("", ErrorMessage);

                TempData["Message"] = ErrorMessage;
                TempData["Success"] = "false";

                return Json(new { error = true, responseText = "Failed", Message = ErrorMessage });
            }
            else
            {
                var resp = response.GetValue("Result").ToObject<String>();

                TempData["Message"] = resp;
                TempData["Success"] = "true";

                return Json(new { success = true, responseText = "Success", Message = resp }, JsonRequestBehavior.AllowGet);

            }

        }


        public ActionResult ManageContexts(int? UserId = 0)
        {
            ContextViewModel returnModel = new ContextViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Documents/GetContexts", User, null, true, false, null, "UserId=" + UserId));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel.Contexts = response.GetValue("Result").ToObject<List<ContextBindingModel>>();
            }

            returnModel.SetSharedData(User);
            return View("ManageContexts", returnModel);
        }



        [HttpPost, ValidateInput(false)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ContextIndex(ContextModels model)
        {
            try
            {

                model.SetSharedData(User);

                model.Context.User_Id = model.UserId;


                var response = await ApiCall.CallApi("api/Documents/AddEditContext", User, model.Context);

                if (response == null)
                {
                    return Json(new { error = true, responseText = "Failed" });
                }
                if (response is Error)
                {
                    string ErrorMessage = (response as Error).ErrorMessage;
                    ModelState.AddModelError("", ErrorMessage);

                    TempData["Message"] = ErrorMessage;
                    TempData["Success"] = "false";

                    return Json(new { error = true, responseText = "Failed", Message = ErrorMessage });
                }
                else
                {
                    var resp = response.GetValue("Result").ToObject<String>();

                    TempData["Message"] = resp;
                    TempData["Success"] = "true";

                    return Json(new { success = true, responseText = "Success", Message = resp }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}