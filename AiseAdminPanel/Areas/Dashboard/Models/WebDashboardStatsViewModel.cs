using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class WebDashboardStatsViewModel : BaseViewModel
    {
        public WebDashboardStatsViewModel()
        {
            DeviceUsage = new List<DeviceStats>();
        }
        public int TotalModels { get; set; }
        public int ActiveModel { get; set; }
        public int ModelAccuracy { get; set; }
        public int TodayOrders { get; set; }

        public int TotalRequests { get; set; }

        public List<DeviceStats> DeviceUsage { get; set; }
    }

    public class DeviceStats
    {
        public int Count { get; set; }
        public int Percentage { get; set; }
    }
}