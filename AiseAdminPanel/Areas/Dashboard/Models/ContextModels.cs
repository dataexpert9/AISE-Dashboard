using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BasketWebPanel.Utility;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class ContextModels : BaseViewModel
    {
        public ContextModels()
        {
            Context = new ContextBindingModel();
        }
        public ContextBindingModel Context { get; set; }

        public int? DocumentId { get; set; }
    }

    public class ContextBindingModel
    {
        public int Id { get; set; }

        public string ContextText { get; set; }

        public int User_Id { get; set; }
    }

    public class ContextViewModel : BaseViewModel
    {
        public ContextViewModel()
        {
            Contexts = new List<ContextBindingModel>();
        }
        public List<ContextBindingModel> Contexts { get; set; }
    }


    public class WhatsappFileUploadBindingModel : BaseViewModel
    {
        public WhatsappFileUploadBindingModel()
        {
        }

        public HttpPostedFileBase File { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public string FileUrl { get; set; }
    }

    public class ExcelFileModel : BaseViewModel
    {
        public ExcelFileModel()
        {
            Whatsapp = new List<WhatsappModelViewModel>();
        }

        public List<WhatsappModelViewModel> Whatsapp { get; set; }
    }

    public class WhatsappModelViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FileUrl { get; set; }

        public FileStatus Status { get; set; }

        public string StatusText { get; set; }

        public UserViewModel User { get; set; }
    }


    public class ImagePathViewModel
    {
        public string Path { get; set; }
    }
}