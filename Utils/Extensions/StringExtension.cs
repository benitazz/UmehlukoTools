#region

using System;
using System.Text;

#endregion

namespace Umehluko.Tools.Utils.Extensions
{
    /// <summary>
    /// The string extension.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="toCheck">
        /// The to check.
        /// </param>
        /// <param name="comp">
        /// The comp.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// The remove special characters.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveSpecialCharacters(this string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}