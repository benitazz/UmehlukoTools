#region

using System;

using Umehluko.Tools.Utils.Common;

#endregion

namespace Umehluko.Tools.Utils.Extensions
{
    /// <summary>
    /// The date time extenstion.
    /// </summary>
    public static class DateTimeExtenstion
    {
        /// <summary>
        /// The get formated date.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFormatedDate(this DateTime dateTime)
        {
            return dateTime.ToString(Constant.DateFormat);
        }

        /// <summary>
        /// The get formated date.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFormatedDate(this DateTime? dateTime)
        {
            var date = dateTime == null
                       ? DateTime.Now.ToString(Constant.DateFormat)
                       : dateTime.Value.ToString(Constant.DateFormat);

            return date;
        }
    }
}