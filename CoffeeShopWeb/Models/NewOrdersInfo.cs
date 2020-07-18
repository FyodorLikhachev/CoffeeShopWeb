using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShopWeb.Models
{
    public class NewOrdersInfo
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string OrderTime { get; set; }
        public decimal OrderSum { get; set; }
        public string PaymentType { get; set; }
    }
}