using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class SearchBoxesViewModel : BaseViewModel
    {
        public List<Box> Boxes { get; set; }
    }

    public class Box
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IntroUrl { get; set; }

        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public int BoxCategory_Id { get; set; }

        public string CategoryName  { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}