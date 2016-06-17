#region

using System.Windows;

#endregion

namespace Umehluko.Tools.Utils.ExceptionMessage
{
    /// <summary>
    /// The ExceptionMessageBoxView interface.
    /// </summary>
    public interface IExceptionMessageBoxView
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        ExceptionMessageBoxViewModel Model { get; set; }

        /// <summary>
        /// The show exception message.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        MessageBoxResult ShowExceptionMessage();

        /// <summary>
        /// The show exception message.
        /// </summary>
        /// <param name="p_owner">
        /// The p_owner.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        MessageBoxResult ShowExceptionMessage(Window p_owner);
    }
}