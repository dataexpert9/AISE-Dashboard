using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Controllers
{
    public class WhatsappController : Controller
    {
        // GET: Dashboard/Whatsapp
        public ActionResult Index()
        {

            WhatsappFileUploadBindingModel model = new WhatsappFileUploadBindingModel();

            model.SetSharedData(User);
            return View(model);
        }

        public ActionResult DownloadSample()
        {
            //using (var client = new WebClient())
            //{
            string SampleFileFolderPath = ConfigurationManager.AppSettings["SampleFiles"]+"\\"+ "WhatsappSample.xlsx";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SampleFileFolderPath);


            //    byte[] bytes = System.IO.File.ReadAllBytes(FullPath);
            //    string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

            //    return Json(base64);
            //    //client.DownloadFile(path, FileName);
            //}

            var fs = new FileStream(path, FileMode.Open);

            return File(fs, "application/vnd.ms-excel", path);



        }


        public ActionResult ManageWhatsapp()
        {
            ExcelFileModel model = new ExcelFileModel();

            model.SetSharedData(User);

            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Whatsapp/GetUploadedFiles", User, null, true, false, null, "UserId=" + model.UserId));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                model.Whatsapp= response.GetValue("Result").ToObject<List<WhatsappModelViewModel>>();
            }


            foreach (var file in model.Whatsapp)
            {
                FileStatus fileStatus = (FileStatus)file.Status;
                file.StatusText = fileStatus.ToString();
            }
            model.SetSharedData(User);
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeStatus(ChangeFileStatus selectedFile)
        {
            try
            {
                if (selectedFile == null)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden, "Select a file to change status");
                }

                var apiResponse = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Whatsapp/ChangeFileStatus", User, selectedFile));

                if (apiResponse == null || apiResponse is Error)
                    return new HttpStatusCodeResult(500, "Internal Server Error");
                else
                {
                    return Json("Success");
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(Utility.LogError(ex), "Internal Server Error");
            }
        }


        public ActionResult History()
        {
           
            return View(Global.sharedDataModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(WhatsappFileUploadBindingModel model)
        {


            MultipartFormDataContent content;

            byte[] fileData = null;




            if (model.File !=null && model.File.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(model.File.InputStream))
                {

                    fileData = binaryReader.ReadBytes(model.File.ContentLength);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "File is required.";

                return RedirectToAction("Index", "Whatsapp");
            }

            ByteArrayContent fileContent;
            JObject response;

            bool firstCall = true;
            callAgain: content = new MultipartFormDataContent();
            if (fileData.Length > 0)
            {
                fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = model.File.FileName };
                content.Add(fileContent);
            }

            model.SetSharedData(User);

            if (model.UserId > 0)
                content.Add(new StringContent(model.UserId.ToString()), "UserId");

            response = await ApiCall.CallApi("api/Whatsapp/UploadWhatsappFile", User, isMultipart: true, multipartContent: content);

            if (response.ToString().Contains("UnAuthorized"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
            }

            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                var result = response.GetValue("Result").ToObject<ImagePathViewModel>();


                TempData["SuccessMessage"] = "File has been uploaded successfully.";

                return RedirectToAction("ManageWhatsapp","Whatsapp");


            }

        }


        public JsonResult DeleteFile(int FileId = 0)
        {
            ContextModels model = new ContextModels();
            model.SetSharedData(User);


            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/DeleteEntity", User, null, true, false, null, "EntityType=" + (int)EntityTypes.ExcelFile, "Id=" + FileId, "User_Id=" + model.UserId));

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

    }
}
