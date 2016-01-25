#region

using System;
using System.IO;

#endregion

namespace Umehluko.Tools.Utils.Extensions
{
    /// <summary>
    /// The file extension.
    /// </summary>
    public static class FileExtension
    {
        /// <summary>
        /// The write to file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="textValue">
        /// The text value.
        /// </param>
        /// <exception cref="UnauthorizedAccessException">Access is denied. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public static void WriteToFile(this string path, string textValue)
        {
            /*if (!File.Exists(path))
            {
                return;
            }*/

            using (var writer = new StreamWriter(path, true))
            {
                writer.WriteLine(textValue);
            }
        }
    }
}