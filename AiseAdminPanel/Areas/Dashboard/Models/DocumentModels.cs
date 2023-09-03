using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class DocumentModels : BaseViewModel
    {

        public DocumentModels()
        {
            Documents = new List<DocumentViewModel>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage ="Document Title/Name Is Required.")]
        public string Title { get; set; }

        public int User_Id { get; set; }

        public List<DocumentViewModel> Documents { get; set; }


    }

    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            User = new UserViewModel();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public int User_Id { get; set; }

        public UserViewModel User { get; set; }

    }



}