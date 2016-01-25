#region

using System;
using System.Linq;
using Umehluko.Tools.Utils.Extensions;

#endregion

namespace Umehluko.Tools.DataModel.DataAccessHelper
{
    /// <summary>
    /// The data access helper.
    /// </summary>
    public class DataAccessHelper
    {
        /// <summary>
        /// The entities.
        /// </summary>
        private static UmehlukoEntities1 entities;

        /// <summary>
        /// The is medical item exists.
        /// </summary>
        /// <param name="itemCode">
        /// The item Code.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="predicate"/> is null.
        /// </exception>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMedicalItemExists(string itemCode)
        {
            if (entities == null)
            {
                entities = new UmehlukoEntities1();
            }

            return entities.MedicalItems.Any(m => (m.ItemCode == itemCode));

            /* return
                entities.MedicalItems.Any(
                    m =>
                    (m.ItemCode == itemCode)
                    && (m.Name.Equals(description, StringComparison.InvariantCultureIgnoreCase) || description.ToUpper().Contains(m.Name.ToUpper())));*/
        }
        
        /// <summary>
        /// Determines whether [is tariff exists] [the specified item code].
        /// </summary>
        /// <param name="itemCode">The item code.</param>
        /// <param name="tariffBaseUnitCost">The tariff base unit cost.</param>
        /// <returns></returns>
        public static bool IsTariffExists(string itemCode, TariffBaseUnitCost  tariffBaseUnitCost)
        {
            if (entities == null)
            {
                entities = new UmehlukoEntities1();
            }

            return entities.Tariffs.Any(t => t.ItemCode == itemCode && t.TariffBaseUnitCostID == tariffBaseUnitCost.TariffBaseUnitCostID);
        }

        /// <summary>
        /// Gets the tbuc.
        /// </summary>
        /// <param name="correctName">Name of the correct.</param>
        /// <param name="startYearAndMonth">The start year and month.</param>
        /// <returns></returns>
        public static TariffBaseUnitCost GetTbuc(string correctName, string startYearAndMonth)
        {
            if (entities == null)
            {
                entities = new UmehlukoEntities1();
            }

            correctName = correctName.ToUpper();

            return
                entities.TariffBaseUnitCosts.FirstOrDefault(
                    baseUnitCost =>
                    (baseUnitCost.Name.ToUpper().Contains(correctName)
                     || correctName.Contains(
                         baseUnitCost.Name.Substring(baseUnitCost.Name.IndexOf('-') + 2).Trim().ToUpper())));

            // && baseUnitCost.ValidFrom.GetFormatedDate().Equals(startYearAndMonth));
        }
    }
}