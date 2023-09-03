using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    //public class AuctionProductBindingModel
    //{
    //    public int Id { get; set; }

    //    public int Product_Id { get; set; }
    //    public DateTime StartDateTime { get; set; }
    //    public DateTime EndDateTime { get; set; }
    //    public bool IsDeleted { get; set; }
    //    public int Type { get; set; }
    //    public double MinBidPrice { get; set; }
    //    public double MaxBidPrice { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public DateTime DeletedDate { get; set; }
    //}

    public class AuctionsBindingModel : BaseViewModel
    {
        public AuctionsBindingModel()
        {
            AuctionProducts = new List<AuctionProductBindingModel>();
        }
        public List<AuctionProductBindingModel> AuctionProducts { get; set; }
    }

    public class AuctionProductBindingModel
    {
        public int Id { get; set; }

        public int Product_Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public double MinBidPrice { get; set; }
        public double MaxBidPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        
        public int? WonUser_Id { get; set; }
        
        public int? WonBid_Id { get; set; }

        public UserBindingModel WonUser { get; set; }

        public AuctionProduct_BidBindingModel WonBid { get; set; }
        
        public ProductBindingModel Product { get; set; }

        public double AuctionPrice { get; set; }

        public string StatusName { get; set; }
    }

    public class AuctionProduct_BidBindingModel
    {
        public int Id { get; set; }
        public int AuctionProduct_Id { get; set; }
        public int User_Id { get; set; }
        public double BidPrice { get; set; }

        public UserBindingModel User { get; set; }
    }
}