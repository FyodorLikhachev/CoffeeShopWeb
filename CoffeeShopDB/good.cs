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
    
    public partial class good
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public good()
        {
            this.material_good_links = new HashSet<material_good_links>();
            this.order_positions = new HashSet<order_positions>();
        }
    
        public int good_id { get; set; }
        public string good_name { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public byte[] photo { get; set; }
        public System.DateTime active_from { get; set; }
        public Nullable<System.DateTime> active_to { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<material_good_links> material_good_links { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_positions> order_positions { get; set; }
    }
}
