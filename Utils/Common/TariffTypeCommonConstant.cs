#region

using System.Collections.Generic;

#endregion

namespace Umehluko.Tools.Utils.Common
{
    /// <summary>
    /// The tariff type.
    /// </summary>
    public class TariffTypeCommonConstant
    {
        /// <summary>
        /// The base code tariffs.
        /// </summary>
        public const string BaseCodeTariffs = "Base Unit Code Tariffs";

        /// <summary>
        /// The normal tariffs.
        /// </summary>
        public const string NormalTariffs = "Tariffs";

        /// <summary>
        /// The upfs
        /// </summary>
        public const string Upfs = "UPFS Tariffs";

        /// <summary>
        /// The get tariff types.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> GetTariffTypes()
        {
            return new List<string> { BaseCodeTariffs, NormalTariffs, Upfs};
        }
    }
}