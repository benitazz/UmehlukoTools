#region

using System;
using System.Linq;

using Umehluko.Tools.DataModel;
using Umehluko.Tools.Utils.Extensions;

#endregion

namespace Umehluko.Tools.UI.Helper
{
    /// <summary>
    /// The database names mapper helper.
    /// </summary>
    public class DatabaseNamesMapperHelper
    {
        /// <summary>
        /// </summary>
        private static readonly GenericUnitOfWork uow = new GenericUnitOfWork();

        /// <summary>
        /// The get mapped publication.
        /// </summary>
        /// <param name="nameToMap">
        /// The name to map.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static Publication GetMappedPublication(string nameToMap)
        {
            return
                uow.Repository<Publication>()
                    .GetAll(
                        p =>
                        p.Description.Contains(nameToMap, StringComparison.InvariantCultureIgnoreCase)
                        || nameToMap.Contains(p.Description))
                    .FirstOrDefault();
        }

        /// <summary>
        /// The get mapped tbuc type.
        /// </summary>
        /// <param name="nameToMap">
        /// The name to map.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static TariffBaseUnitCostType GetMappedTbucType(string nameToMap)
        {
            var tariffBaseUnitCostTypeData =
                uow.Repository<TariffBaseUnitCostType>()
                    .GetAll(
                        p =>
                        p.Description.Contains(nameToMap, StringComparison.InvariantCultureIgnoreCase)
                        || nameToMap.Contains(p.Description, StringComparison.InvariantCulture))
                    .FirstOrDefault();

            return tariffBaseUnitCostTypeData;
        }
    }
}