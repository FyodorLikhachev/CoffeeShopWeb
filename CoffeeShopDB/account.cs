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
    
    public partial class account
    {
        public int account_id { get; set; }
        public int employee_id { get; set; }
        public string login_value { get; set; }
        public string password_value { get; set; }
    
        public virtual employee employee { get; set; }
    }
}