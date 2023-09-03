using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.BindingModels
{
    public class AuctionBindingModel : BaseViewModel
    {
        public AuctionBindingModel()
        {
            AuctionProducts = new List<AuctionProductBindingModel>();
            Bids = new List<AuctionProduct_BidBindingModel>();
        }

        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime StartDateTime { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime EndDateTime { get; set; } = DateTime.Now;

        [Required]
        public int Type { get; set; }

        public bool IsDeleted { get; set; }

        public ProductBindingModel Product { get; set; }

        public List<AuctionProductBindingModel> AuctionProducts { get; set; }

        public SelectList AuctionTypes { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double AuctionPrice { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a valid price")]
        public double MinBidPrice { get; set; }

        public List<AuctionProduct_BidBindingModel> Bids { get; set; }
    }
}