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
    
    public partial class provider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public provider()
        {
            this.transactions = new HashSet<transaction>();
        }
    
        public int provider_id { get; set; }
        public string provider_name { get; set; }
        public string country { get; set; }
        public int shipment_in { get; set; }
        public System.DateTime active_from { get; set; }
        public Nullable<System.DateTime> active_to { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<transaction> transactions { get; set; }
    }
}
