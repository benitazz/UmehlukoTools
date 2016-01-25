#region

using System;

using Umehluko.Tools.Utils.Common;

#endregion

namespace Umehluko.Tools.UI.Helper
{
    /// <summary>
    /// The financial year helper.
    /// </summary>
    public class FinancialYearHelper
    {
        /// <summary>
        /// The get financial year end.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFinancialYearEnd(DateTime dateTime)
        {
            var financialYearEnd = dateTime.AddYears(1);

            financialYearEnd = financialYearEnd.AddDays(-1);

            return financialYearEnd.ToString(Constant.DateFormat);
        }

        /// <summary>
        /// Gets the financial year start.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFinancialYearStart(DateTime dateTime)
        {
            if (dateTime.Month == 4 && dateTime.Month == 1)
            {
                return dateTime.ToString(Constant.DateFormat);
            }

            var financialYearStart = new DateTime(dateTime.Year, 04, 01);

            return financialYearStart.ToString(Constant.DateFormat);
        }
    }
}