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
    
    public partial class employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public employee()
        {
            this.accounts = new HashSet<account>();
            this.order_positions = new HashSet<order_positions>();
            this.transactions = new HashSet<transaction>();
        }
    
        public int employee_id { get; set; }
        public int sales_point_id { get; set; }
        public string position { get; set; }
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string middle_name { get; set; }
        public System.DateTime active_from { get; set; }
        public Nullable<System.DateTime> active_to { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<account> accounts { get; set; }
        public virtual sales_points sales_points { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_positions> order_positions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<transaction> transactions { get; set; }
    }
}
