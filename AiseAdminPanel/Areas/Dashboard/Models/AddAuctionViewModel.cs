using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BasketWebPanel.BindingModels;
using System.ComponentModel.DataAnnotations;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddAuctionViewModel : BaseViewModel
    {

        public AddAuctionViewModel()
        {
            AuctionProducts = new List<AuctionProducts>();
        }

        public DateTime CreatedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime StartDateTime { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime ExpiryDateTime { get; set; } = DateTime.Now;

        public int Type { get; set; }

        public List<AuctionProducts> AuctionProducts { get; set; }
    }

    public class AuctionProducts
    {
        public int ProductId { get; set; }
        public double ProductPrice { get; set; }
        public double MinBidPrice { get; set; }
        public double MaxBidPrice { get; set; }

    }
}