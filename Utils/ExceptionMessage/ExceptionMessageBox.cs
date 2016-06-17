#region

using System;
using System.Text;
using System.Windows;

#endregion

namespace Umehluko.Tools.Utils.ExceptionMessage
{
    /// <summary>
    /// MessageBox to show detail of Exception.
    /// </summary>
    /// <typeparam name="T">
    /// Customize View or Dialog Window to show exception, which implements IExceptionMessageBoxView
    /// </typeparam>
    public class ExceptionMessageBox<T>
        where T : IExceptionMessageBoxView, new()
    {
        /// <summary>
        /// The default caption.
        /// </summary>
        private const string defaultCaption = "Exception Message";

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="p_exception">
        /// The p_exception.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        public static MessageBoxResult Show(Exception p_exception)
        {
            return ShowCore(p_exception.Message, p_exception, defaultCaption, MessageBoxButton.OK, null);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="p_message">
        /// The p_message.
        /// </param>
        /// <param name="p_exception">
        /// The p_exception.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        public static MessageBoxResult Show(string p_message, Exception p_exception)
        {
            return ShowCore(p_message, p_exception, defaultCaption, MessageBoxButton.OK, null);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="p_message">
        /// The p_message.
        /// </param>
        /// <param name="p_exception">
        /// The p_exception.
        /// </param>
        /// <param name="p_owner">
        /// The p_owner.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        public static MessageBoxResult Show(string p_message, Exception p_exception, Window p_owner)
        {
            return ShowCore(p_message, p_exception, defaultCaption, MessageBoxButton.OK, p_owner);
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="p_message">
        /// The p_message.
        /// </param>
        /// <param name="p_exception">
        /// The p_exception.
        /// </param>
        /// <param name="p_caption">
        /// The p_caption.
        /// </param>
        /// <param name="p_button">
        /// The p_button.
        /// </param>
        /// <param name="p_owner">
        /// The p_owner.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        public static MessageBoxResult Show(
            string p_message, 
            Exception p_exception, 
            string p_caption, 
            MessageBoxButton p_button, 
            Window p_owner)
        {
            return ShowCore(p_message, p_exception, p_caption, p_button, p_owner);
        }

        /// <summary>
        /// Core Messagebox Show method.
        /// <remarks>
        /// Basic implementation logic is separated and hide in this method, so that messagebox behavior or messagebox dependencyobject can be changed without impact to Show methods.
        /// </remarks>
        /// </summary>
        /// <param name="p_message">
        /// custom message
        /// </param>
        /// <param name="p_exception">
        /// exception to be shown as additional information
        /// </param>
        /// <param name="p_caption">
        /// caption of messagebox
        /// </param>
        /// <param name="p_button">
        /// buttons on messagebox
        /// </param>
        /// <param name="p_owner">
        /// The p_owner.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        private static MessageBoxResult ShowCore(
            string p_message, 
            Exception p_exception, 
            string p_caption, 
            MessageBoxButton p_button, 
            Window p_owner)
        {
            if (p_message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (p_caption == null)
            {
                throw new ArgumentNullException("caption");
            }

            if (p_owner == null && Application.Current != null)
            {
                p_owner = Application.Current.MainWindow;
            }

            var l_message = GetDetailMessage(p_exception);

            IExceptionMessageBoxView view = new T();

            view.Model = new ExceptionMessageBoxViewModel()
                             {
                                 Caption = p_caption, 
                                 UserMessage = p_message, 
                                 ExceptionDetail = l_message, 
                                 MessageBoxButton = p_button
                             };

            return view.ShowExceptionMessage(p_owner);
        }

        /// <summary>
        /// The get detail message.
        /// </summary>
        /// <param name="p_exception">
        /// The p_exception.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetDetailMessage(Exception p_exception)
        {
            var l_messageBuilder = new StringBuilder();
            l_messageBuilder.AppendLine("An exception has occured. Please see details.");
            l_messageBuilder.Append(Environment.NewLine);

            l_messageBuilder.Append(GetExceptionDetail(p_exception));

            var l_message = l_messageBuilder.ToString();
            return l_message;
        }

        /// <summary>
        /// The get exception detail.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetExceptionDetail(Exception ex)
        {
            var l_exceptionDetailBuffer = new StringBuilder();

            if (ex.InnerException != null)
            {
                l_exceptionDetailBuffer.Append(GetExceptionDetail(ex.InnerException));
            }

            var exceptionType = ex.GetType();

            l_exceptionDetailBuffer.Append('[');
            l_exceptionDetailBuffer.Append(exceptionType.Name);
            l_exceptionDetailBuffer.Append(": ");
            l_exceptionDetailBuffer.Append(ex.Message);
            l_exceptionDetailBuffer.Append(']');
            l_exceptionDetailBuffer.Append(Environment.NewLine);

            l_exceptionDetailBuffer.AppendLine(ex.StackTrace);

            return l_exceptionDetailBuffer.ToString();
        }
    }
}