//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Umehluko.Tools.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class TariffBaseUnitCostType
    {
        public TariffBaseUnitCostType()
        {
            this.TariffBaseUnitCosts = new HashSet<TariffBaseUnitCost>();
        }
    
        public int TariffBaseUnitCostTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LastChangedBy { get; set; }
        public Nullable<System.DateTime> LastChangedDate { get; set; }
    
        public virtual ICollection<TariffBaseUnitCost> TariffBaseUnitCosts { get; set; }
    }
}