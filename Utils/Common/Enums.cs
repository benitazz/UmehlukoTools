#region

using System.ComponentModel;

#endregion

namespace Umehluko.Tools.Utils.Common
{
    /// <summary>
    /// The file type.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// The csv file.
        /// </summary>
        [Description(".csv")]
        CsvFile , 

        /// <summary>
        /// The text file.
        /// </summary>
        [Description(".txt")]
        TextFile
    }

    /// <summary>
    /// The practitioner type.
    /// </summary>
    public enum PracType
    {
        /// <summary>
        /// The doctor.
        /// </summary>
        Doctor = 0, 

        /// <summary>
        /// The specialist.
        /// </summary>
        Specialist = 1
    }

    /// <summary>
    /// The tariff action.
    /// </summary>
    public enum TariffAction
    {
        /// <summary>
        /// The compare.
        /// </summary>
        Compare, 

        /// <summary>
        /// The generate script.
        /// </summary>
        GenerateScript,

        /// <summary>
        /// The hospital tariffs.
        /// </summary>
        Upfs
    }
}