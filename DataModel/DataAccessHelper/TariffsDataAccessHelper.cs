#region

using System;
using System.Linq;

using Umehluko.Tools.Utils.Common;

#endregion

namespace Umehluko.Tools.DataModel.DataAccessHelper
{
    /// <summary>
    /// The data access helper.
    /// </summary>
    public class TariffsDataAccessHelper : IDisposable
    {
        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The entities.
        /// </summary>
        private UmehlukoEntities1 entities = null;

        /// <summary>
        /// Gets the tariff.
        /// </summary>
        /// <param name="itemcode">The itemcode.</param>
        /// <returns></returns>
        public Tariff GetTariff(string itemcode)
        {
            if (this.entities == null)
            {
                this.entities = new UmehlukoEntities1();
            }

            return (from tariff in this.entities.Tariffs where tariff.ItemCode == itemcode select tariff).FirstOrDefault();
        }
        
        /// <summary>
        /// Gets the tariff.
        /// </summary>
        /// <param name="startYearAndMonth">The start year and month.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="tariffAction">The tariff action.</param>
        /// <returns></returns>
        public IQueryable<Tariff> GetTariff(string startYearAndMonth, string fileName, TariffAction tariffAction)
        {
            this.entities = new UmehlukoEntities1();

            var startDate = DateTime.ParseExact(startYearAndMonth, Constant.DateFormat, null);

            return from tariff in this.entities.Tariffs
                   where tariff.ValidFrom == startDate && tariff.TariffTypeID == Constant.CoidTariffTypeId
                   select tariff;
        }

        /// <summary>
        /// Gets the medical items.
        /// </summary>
       public IQueryable<MedicalItem> GetMedicalItems()
        {
            this.entities = new UmehlukoEntities1();

             return from medicalItem in this.entities.MedicalItems
                   //where medicalItem.ItemCode == itemCode
                   select medicalItem;
        }


       

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.entities.Dispose();
                this.entities = null;
            }

            // Free any unmanaged objects here.
            this.disposed = true;
        }
    }
}