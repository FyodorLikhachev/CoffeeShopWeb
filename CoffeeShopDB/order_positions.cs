//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoffeeShopDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class order_positions
    {
        public int order_position_id { get; set; }
        public int order_id { get; set; }
        public int employee_id { get; set; }
        public int good_id { get; set; }
        public int quantity { get; set; }
    
        public virtual employee employee { get; set; }
        public virtual good good { get; set; }
        public virtual order order { get; set; }
    }
}
