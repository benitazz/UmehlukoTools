#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Umehluko.Tools.Utils.Common
{
    /// <summary>
    /// The province names.
    /// </summary>
    public class ProvinceNames
    {
        /// <summary>
        /// The gauteng.
        /// </summary>
        public const string Gauteng = "GAUTENG";

        /// <summary>
        /// The limpopo.
        /// </summary>
        public const string Limpopo = "LIMPOPO";

        /// <summary>
        /// The mpumalanga.
        /// </summary>
        public const string Mpumalanga = "MPUMALANGA";

        /// <summary>
        /// The kwa zulu natal.
        /// </summary>
        public const string KwaZuluNatal = "KWA-ZULUNATAL";

        /// <summary>
        /// The free state.
        /// </summary>
        public const string FreeState = "FREESTATE";

        /// <summary>
        /// The northen cape.
        /// </summary>
        public const string NorthenCape = "NORTHRENCAPE";

        /// <summary>
        /// The eastern cape.
        /// </summary>
        public const string EasternCape = "EASTERNCAPE";

        /// <summary>
        /// The western cape.
        /// </summary>
        public const string WesternCape = "WESTERNCAPE";

        /// <summary>
        /// The north west.
        /// </summary>
        public const string NorthWest = "NORTHWEST";

        /// <summary>
        /// The get provinces.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> GetProvinces()
        {
            return
                new List<string>
                    {
                        Gauteng, 
                        Limpopo, 
                        Mpumalanga, 
                        NorthWest, 
                        NorthenCape, 
                        WesternCape, 
                        EasternCape, 
                        FreeState, 
                        KwaZuluNatal
                    }.OrderBy(e => e).ToList();
        }
    }
}