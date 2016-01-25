namespace Umehluko.Tools.Utils.Extensions
{
    /// <summary>
    /// The is numeric extension.
    /// </summary>
    public static class IsNumericExtension
    {
        /// <summary>
        /// The is numeric.
        /// </summary>
        /// <param name="str">
        /// The string value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsNumeric(this string str)
        {
            double myNum;

            return double.TryParse(str, out myNum);
        }
    }
}