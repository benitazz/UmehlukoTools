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
    
    public partial class Publication
    {
        public Publication()
        {
            this.TariffBaseUnitCosts = new HashSet<TariffBaseUnitCost>();
        }
    
        public int PublicationID { get; set; }
        public string PublicationNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TariffTypeID { get; set; }
        public System.DateTime ValidFrom { get; set; }
        public System.DateTime ValidTo { get; set; }
        public byte IsActive { get; set; }
        public string LastChangedBy { get; set; }
        public Nullable<System.DateTime> LastChangedDate { get; set; }
    
        public virtual TariffType TariffType { get; set; }
        public virtual ICollection<TariffBaseUnitCost> TariffBaseUnitCosts { get; set; }
    }
}
