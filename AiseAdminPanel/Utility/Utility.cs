﻿using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.Models;
using BasketWebPanel.ViewModels;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel
{
    public static class ExtensionMethods
    {
        public static string GetFormattedBreadCrumb(this CategoryBindingModel category, IEnumerable<CategoryBindingModel> categoryService, string separator = ">>")
        {
            string result = string.Empty;
            if (category.ParentCategoryId == null || category.ParentCategoryId == 0)
            {
                return category.Name;
            }
            var breadcrumb = categoryService.Where(x => x.Id == category.ParentCategoryId).FirstOrDefault();
            if (breadcrumb == null)
            {
                return category.Name;
            }
            var categoryName = breadcrumb.Name;
            result = string.Format("{0} {1} {2}", categoryName, separator, category.Name);

            return result;
        }

        public static string GetFormattedBreadCrumb(this SearchCategoryViewModel category, IEnumerable<SearchCategoryViewModel> categoryService, string separator = ">>")
        {
            string result = string.Empty;
            if (category.ParentCategoryId == null || category.ParentCategoryId == 0)
            {
                return category.Name;
            }
            var breadcrumb = categoryService.Where(x => x.Id == category.ParentCategoryId).FirstOrDefault();
            if (breadcrumb == null)
            {
                return category.Name;
            }
            var categoryName = breadcrumb.Name;
            result = string.Format("{0} {1} {2}", categoryName, separator, category.Name);

            return result;
        }

        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }


    }

    class MyRegularExpressions
    {
        public const string Price = @"^\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$";
        public const string Name = @"^[A-z]+$";
    }

    public class Utility
    {

        public enum FileStatus
        {
            Pending = 0,
            Initiated = 1,
            InProgress = 2,
            Completed = 3,
            Paused=4,
            Failed = 5
        }

        public enum Auctiontypes
        {
            Live = 1,
            Upcoming = 2,
            Closed = 3
        }

        public static DbGeography CreatePoint(double lat, double lon, int srid = 4326)
        {
            string wkt = String.Format("POINT({0} {1})", lon, lat);

            return DbGeography.PointFromText(wkt, srid);
        }

        public enum AuctionStatus
        {
            Initial = 1, //Not Started
            InProgress = 2,
            Completed = 3
        }

        public enum RoleTypes
        {
            User = 0,
            SuperAdmin = 1,
        }

        public enum WeightUnits
        {
            gm = 1,
            kg = 2
        }

        public enum EntityTypes
        {
            Admin=0,
            User=1,
            Context=2,
            ExcelFile=3
        }

        public enum PaymentMethods
        {
            CashOnDelivery,
            CreditCard,
            DebitCard
        }

        public enum BoxCategoryOptions
        {
            Junior = 1,
            Monthly = 2,
            TriMonthly = 3,
            HallOfFame = 4
        }

        public enum PaymentStatuses
        {
            Pending,
            Paid
        }

        public enum OrderStatuses
        {
            Initiated,
            Accepted,
            Rejected,
            [Display(Name = "In Progress")]
            InProgress,
            [Display(Name = "Ready For Delivery")]
            ReadyForDelivery,
            [Display(Name = "Assigned To Deliverer")]
            AssignedToDeliverer,
            [Display(Name = "Deliverer In Progress")]
            DelivererInProgress,
            [Display(Name = "Dispatched")]
            Dispatched,
            [Display(Name = "Completed")]
            Completed
        }

        public static SelectList GetUserStatusOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>();

                options.Add(new SelectListItem { Text = "Active", Value = "false" });
                options.Add(new SelectListItem { Text = "Blocked", Value = "true" });

                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }

        public static SelectList GetDocumentOptions(IPrincipal User, int? UserId=0,int? DocumentId=0)
        {
            try
            {
                var responseDocument = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/Admin/GetAllDocuments", User, true, true,false ,null,"UserId=" + UserId + "", "DocumentId=" + DocumentId));
                if (responseDocument == null || responseDocument is Error)
                {
                    return null;
                }
                else
                {
                    var Documents = responseDocument.GetValue("Result").ToObject<List<DocumentViewModel>>();
                    IEnumerable<SelectListItem> selectList = from document in Documents
                                                             select new SelectListItem
                                                             {
                                                                 Selected = false,
                                                                 Text = document.Title,
                                                                 Value = document.Id.ToString()
                                                             };
                    return new SelectList(selectList);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static SelectList GetStoresOptions(IPrincipal User, string DefaultName = "")
        {
            try
            {
                var responseStores = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllStores", User, GetRequest: true));
                if (responseStores == null || responseStores is Error)
                {
                    return null;
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
                    if (DefaultName != "")
                        Stores.Insert(0, new StoreBindingModel { Id = 0, Name = DefaultName });

                    return new SelectList(selectList);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SelectList GetPaymentMethodOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>();

                foreach (PaymentMethods paymentMethod in Enum.GetValues(typeof(PaymentMethods)))
                {
                    options.Add(new SelectListItem { Text = paymentMethod.ToString(), Value = (((int)paymentMethod) + 1).ToString() });
                }

                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SelectList GetAuctionTypeOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>(); 

                foreach (Auctiontypes auctionType in Enum.GetValues(typeof(Auctiontypes)))
                {
                    options.Add(new SelectListItem { Text = auctionType.ToString(), Value = ((int)auctionType).ToString() });
                }

                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SelectList GetBoxCategoryOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>();

                foreach (BoxCategoryOptions boxCategory in Enum.GetValues(typeof(BoxCategoryOptions)))
                {
                    options.Add(new SelectListItem { Text = boxCategory.ToString(), Value = ((int)boxCategory).ToString() });
                }

                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SelectList GetOrderStatusOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>();

                foreach (OrderStatuses orderStatus in Enum.GetValues(typeof(OrderStatuses)))
                {
                    if (orderStatus == OrderStatuses.AssignedToDeliverer)
                    {
                        options.Add(new SelectListItem { Text = "Assigned To Deliverer", Value = (((int)orderStatus) + 1).ToString() });
                    }
                    else if (orderStatus == OrderStatuses.DelivererInProgress)
                    {
                        options.Add(new SelectListItem { Text = "Deliverer In Progress", Value = (((int)orderStatus) + 1).ToString() });
                    }
                    else if (orderStatus == OrderStatuses.InProgress)
                    {
                        options.Add(new SelectListItem { Text = "In Progress", Value = (((int)orderStatus) + 1).ToString() });
                    }
                    else if (orderStatus == OrderStatuses.ReadyForDelivery)
                    {
                        options.Add(new SelectListItem { Text = "Ready For Delivery", Value = (((int)orderStatus) + 1).ToString() });
                    }
                    else
                        options.Add(new SelectListItem { Text = orderStatus.ToString(), Value = (((int)orderStatus) + 1).ToString() });
                }


                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception)
            {
                throw null;
            }
        }

        public static SelectList GetPaymentStatusOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>();

                foreach (PaymentStatuses paymentStatus in Enum.GetValues(typeof(PaymentStatuses)))
                {
                    options.Add(new SelectListItem { Text = paymentStatus.ToString(), Value = (((int)paymentStatus) + 1).ToString() });
                }


                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception)
            {
                throw null;
            }
        }

        public static SelectList GetWeightOptions(string DefaultName = "")
        {
            try
            {
                List<SelectListItem> options = new List<SelectListItem>();

                foreach (WeightUnits weightUnit in Enum.GetValues(typeof(WeightUnits)))
                {
                    options.Add(new SelectListItem { Text = weightUnit.ToString(), Value = ((int)weightUnit).ToString() });
                }


                if (DefaultName != "")
                    options.Insert(0, new SelectListItem { Text = DefaultName, Value = "0" });

                return new SelectList(options);
            }
            catch (Exception)
            {
                throw null;
            }
        }

        public static SelectList GetCategoryOptions(IPrincipal User, string DefaultName = "", int? CatId = null)
        {
            try
            {
                var response = AsyncHelpers.RunSync<JObject>(() => ApiCall.CallApi("api/GetAllCategories", User, null, true, false, null, "forAdmin=true"));

                if (response == null || response is Error)
                {
                    return null;
                }
                else
                {
                    var responseCategories = response.GetValue("Result").ToObject<CategoriesViewModel>().Categories;
                    var tempCats = responseCategories.ToList();

                    foreach (var cat in responseCategories)
                    {
                        cat.Name = cat.GetFormattedBreadCrumb(tempCats);
                    }
                    responseCategories = responseCategories.OrderBy(x => x.Name).ToList();

                    if (DefaultName != "")
                        responseCategories.Insert(0, new CategoryBindingModel { Id = 0, Name = DefaultName });

                    IEnumerable<SelectListItem> selectList;
                    if (CatId.HasValue)
                    {
                        selectList = from cat in responseCategories
                                     where cat.Id != CatId.Value
                                     select new SelectListItem
                                     {
                                         Selected = false,
                                         Text = cat.Name,
                                         Value = cat.Id.ToString()
                                     };
                    }
                    else
                    {
                        selectList = from cat in responseCategories
                                     select new SelectListItem
                                     {
                                         Selected = false,
                                         Text = cat.Name,
                                         Value = cat.Id.ToString()
                                     };
                    }
                    return new SelectList(selectList);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static HttpStatusCode LogError(Exception ex)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
                {
                    if (ex.Message != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Message" + ex.Message);
                        sw.WriteLine(Environment.NewLine + "StackTrace" + ex.StackTrace);
                    }
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
                        sw.WriteLine(Environment.NewLine + "InnerExceptionStackTrace : " + ex.InnerException.StackTrace);
                    }
                    sw.WriteLine("------******------");
                }
                return HttpStatusCode.InternalServerError;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }


    }

    class Global
    {
        public static SharedViewModel sharedDataModel = new SharedViewModel();

        public static int StoreId = Convert.ToInt32(ConfigurationManager.AppSettings["StoreId"]);

    }


}