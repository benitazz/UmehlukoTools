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
    
    public partial class MedicalItem
    {
        public MedicalItem()
        {
            this.Tariffs = new HashSet<Tariff>();
        }
    
        public int MedicalItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public int TreatmentCodeID { get; set; }
        public int NAPPICodeID { get; set; }
        public byte IsActive { get; set; }
        public int MedicalItemTypeID { get; set; }
        public decimal DefaultQuantity { get; set; }
        public Nullable<decimal> MinServiceIntervalDays { get; set; }
        public int AcuteMedicalAuthNeededTypeID { get; set; }
        public int ChronicMedicalAuthNeededTypeID { get; set; }
        public string LastChangedBy { get; set; }
        public Nullable<System.DateTime> LastChangedDate { get; set; }
        public bool IsAllowSameDayTreatment { get; set; }
    
        public virtual ICollection<Tariff> Tariffs { get; set; }
    }
}
