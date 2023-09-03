using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class AddBoxViewModel : BaseViewModel
    {
        public AddBoxViewModel()
        {
            BoxVideos = new List<Video>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Url]
        [Required(ErrorMessage = "This field is required")]
        public string IntroUrl { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double? Price { get; set; }

        public List<Video> BoxVideos { get; set; }

        public SelectList Categories { get; internal set; }

        public int BoxCategory_Id { get; set; }

    }

    public class Video
    {
        [Url]
        public string VideoUrl { get; set; }
    }
}