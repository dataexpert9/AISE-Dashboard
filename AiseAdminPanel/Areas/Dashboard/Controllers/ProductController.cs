using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    [Authorize]
    public class ProductController : Controller
    {
        [HttpPost]
        public JsonResult DeleteImageOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
                Request.RequestContext.HttpContext.Session.Add("AddProductImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("ImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddProductImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }

        [HttpPost]
        public JsonResult DeleteBannerImage()
        {
            Request.RequestContext.HttpContext.Session.Remove("AddBannerImage");
            Request.RequestContext.HttpContext.Session.Add("ImageDeletedOnEdit", true);
            return Json("Session Cleared");
        }




        // banner image session handling 



        [HttpPost]
        public JsonResult DeleteBannerImageOnEdit()
        {
            return Json("Success");
        }

        [HttpPost]
        public JsonResult UploadBannerImage(HttpPostedFileBase file)
        {
            if (Request.Files.Count == 1)
            {
                Request.RequestContext.HttpContext.Session.Remove("AddBannerImage");
                Request.RequestContext.HttpContext.Session.Add("AddBannerImage", Request.Files[0]);

                Request.RequestContext.HttpContext.Session.Remove("BannerImageDeletedOnEdit");
                Request.RequestContext.HttpContext.Session.Add("BannerImageDeletedOnEdit", false);
            }
            return Json("Success");
        }

    

        //Banner image session handling end



        public JsonResult FetchCategories(int storeId) // its a GET, not a POST
        {
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllCategoriesByStoreId", User, GetRequest: true, parameters: "StoreId=" + storeId));
            var responseCategories = response.GetValue("Result").ToObject<List<CategoryBindingModel>>();
            var tempCats = responseCategories.ToList();

            foreach (var cat in responseCategories)
            {
                cat.Name = cat.GetFormattedBreadCrumb(tempCats);
            }

            responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = "None" });

            return Json(responseCategories, JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AddProductViewModel model)
        {
            try
            {
                model.Product.Description = model.Product.Description ?? "";
                if (!ModelState.IsValid)
                {
                    model.SetSharedData(User);
                    model.CategoryOptions = Utility.GetCategoryOptions(User, "None");
                    return View(model);
                }
                bool firstCall = true;

                MultipartFormDataContent content;
                bool ImageDeletedOnEdit = false;
                bool BannerImageDeletedOnEdit = false;
                bool ProductImage = (Request.RequestContext.HttpContext.Session["AddProductImage"] != null);
                bool BannerImage = (Request.RequestContext.HttpContext.Session["AddBannerImage"] != null);

                ByteArrayContent fileContent;
                JObject response;
                callAgain: content = new MultipartFormDataContent();
                if (ProductImage)
                {
             
                    var imgDeleteSessionValue = Request.RequestContext.HttpContext.Session["ImageDeletedOnEdit"];
                    var BannerimgDeleteSessionValue = Request.RequestContext.HttpContext.Session["BannerImageDeletedOnEdit"];

                    if (imgDeleteSessionValue != null)
                    {
                        ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                    }

                    if (BannerimgDeleteSessionValue != null)
                    {
                        BannerImageDeletedOnEdit = Convert.ToBoolean(BannerimgDeleteSessionValue);
                    }


                    byte[] fileDataBannerImage = null;
                    byte[] fileDataInstagramImage = null;

                    var ImageFileBanner = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddBannerImage"];
                    var ImageFileProduct = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductImage"];


                    if (BannerImage)
                        using (var binaryReader = new BinaryReader(ImageFileBanner.InputStream))
                        {
                            fileDataBannerImage = binaryReader.ReadBytes(ImageFileBanner.ContentLength);
                        }

                    if (ProductImage)
                        using (var binaryReader = new BinaryReader(ImageFileProduct.InputStream))
                        {
                            fileDataInstagramImage = binaryReader.ReadBytes(ImageFileProduct.ContentLength);
                        }

                    if (BannerImage)
                    {
                        fileContent = new ByteArrayContent(fileDataBannerImage);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Banner" + Path.GetExtension(ImageFileBanner.FileName) };
                        content.Add(fileContent, "BannerImage");
                    }

                    if (ProductImage)
                    {
                        fileContent = new ByteArrayContent(fileDataInstagramImage);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Product" + Path.GetExtension(ImageFileProduct.FileName) };
                        content.Add(fileContent, "ProductImage");
                    }
                }


                #region Previous Attachment of image for api

                //  bool FileAttached = (Request.RequestContext.HttpContext.Session["AddProductImage"] != null);



                //if (imgDeleteSessionValue != null)
                //{
                //    ImageDeletedOnEdit = Convert.ToBoolean(imgDeleteSessionValue);
                //}
                //byte[] fileData = null;
                //var ImageFile = (HttpPostedFileWrapper)Request.RequestContext.HttpContext.Session["AddProductImage"];
                //if (FileAttached)
                //{
                //    using (var binaryReader = new BinaryReader(ImageFile.InputStream))
                //    {

                //        fileData = binaryReader.ReadBytes(ImageFile.ContentLength);
                //    }
                //}
                //else if (model.Product.ImageUrl == null || ImageDeletedOnEdit)
                //{
                //    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Please Choose an image to upload.");
                //}

                //ByteArrayContent fileContent;
                //JObject response;

                //bool firstCall = true;
                //callAgain: content = new MultipartFormDataContent();
                //if (FileAttached)
                //{
                //    fileContent = new ByteArrayContent(fileData);
                //    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = ImageFile.FileName };
                //    content.Add(fileContent);
                //}
                //if (model.Product.Id > 0)
                //{
                //    content.Add(new StringContent(model.Product.Id.ToString()), "Id");
                //}

                #endregion
                content.Add(new StringContent(model.Product.Name), "Name");
                content.Add(new StringContent(model.Product.Price.ToString()), "Price");
                content.Add(new StringContent(model.Product.SlashPrice.ToString()), "SlashPrice");
                content.Add(new StringContent(model.Product.Category_Id.ToString()), "Category_Id");
                content.Add(new StringContent(model.Product.Store_Id.ToString()), "Store_Id");
                content.Add(new StringContent(model.Product.Description), "Description");
                

                content.Add(new StringContent(Convert.ToString(ImageDeletedOnEdit)), "ImageDeletedOnEdit");
                content.Add(new StringContent(Convert.ToString(BannerImageDeletedOnEdit)), "ImageDeletedOnEdit");

                response = await ApiCall.CallApi("api/Admin/AddProduct", User, isMultipart: true, multipartContent: content);
                if (firstCall && response.ToString().Contains("UnAuthorized"))
                {
                    firstCall = false;
                    goto callAgain;
                }
                else if (response.ToString().Contains("UnAuthorized"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "UnAuthorized Error");
                }
                if (response is Error)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
                }
                else
                {
                    if (model.Product.Id > 0)
                        TempData["SuccessMessage"] = "The product has been updated successfully.";
                    else
                        TempData["SuccessMessage"] = "The product has been added successfully.";
                    return RedirectToAction("ManageProducts");
                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public ActionResult ManageProducts()
        {
            Global.sharedDataModel.SetSharedData(User);
            return View(Global.sharedDataModel);
        }

        public ActionResult SearchProduct()
        {
            SearchProductModel returnModel = new SearchProductModel();

            var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
            if (responseStores == null || responseStores is Error)
            {
            }
            else
            {
                var Stores = responseStores.GetValue("Result").ToObject<List<StoreBindingModel>>();
                IEnumerable<SelectListItem> selectList = from store in Stores
                                                         select new SelectListItem
                                                         {
                                                             Selected = false,
                                                             Text = store.Name,
                                                             Value = store.Id.ToString()
                                                         };
                Stores.Insert(0, new StoreBindingModel { Id = 0, Name = "All" });

                returnModel.StoreOptions = new SelectList(selectList);
            }

            returnModel.SetSharedData(User);
            //returnModel. returnModel.StoreId




            return PartialView("_SearchProduct", returnModel);
        }

        public ActionResult SearchProductResults(SearchProductModel model)
        {
            SearchProductsViewModel returnModel = new SearchProductsViewModel();
            var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/SearchProducts", User, null, true, false, null, "ProductName=" + model.ProductName + "", "ProductPrice=" + model.ProductPrice, "CategoryName=" + model.CategoryName + "", "StoreId=" + Global.StoreId));
            if (response is Error)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, (response as Error).ErrorMessage);
            }
            else
            {
                returnModel = response.GetValue("Result").ToObject<SearchProductsViewModel>();
            }
            returnModel.SetSharedData(User);
            return PartialView("_SearchProductResults", returnModel);
        }

     

    }
}