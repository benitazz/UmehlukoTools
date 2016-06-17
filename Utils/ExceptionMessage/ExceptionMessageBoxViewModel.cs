#region

using System.Windows;

#endregion

namespace Umehluko.Tools.Utils.ExceptionMessage
{
    /// <summary>
    /// The exception message box view model.
    /// </summary>
    public class ExceptionMessageBoxViewModel
    {
        // TODO: property change notification for exception messagebox has yet implemented. to be implement when it's needed.

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMessageBoxViewModel"/> class.
        /// </summary>
        public ExceptionMessageBoxViewModel()
        {
            this.Caption = "Exception Message";
            this.UserMessage =
                "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            this.MessageBoxButton = MessageBoxButton.OK;
        }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// User Message
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Gets or sets the exception detail.
        /// </summary>
        public string ExceptionDetail { get; set; }

        /// <summary>
        /// Gets or sets the message box button.
        /// </summary>
        public MessageBoxButton MessageBoxButton { get; set; }

        /// <summary>
        /// Gets the ok visibility.
        /// </summary>
        public Visibility OKVisibility
        {
            get
            {
                if (this.MessageBoxButton == MessageBoxButton.OK || this.MessageBoxButton == MessageBoxButton.OKCancel)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the cancel visibility.
        /// </summary>
        public Visibility CancelVisibility
        {
            get
            {
                if (this.MessageBoxButton == MessageBoxButton.OKCancel
                    || this.MessageBoxButton == MessageBoxButton.YesNoCancel)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the yes visibility.
        /// </summary>
        public Visibility YesVisibility
        {
            get
            {
                if (this.MessageBoxButton == MessageBoxButton.YesNo
                    || this.MessageBoxButton == MessageBoxButton.YesNoCancel)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the no visibility.
        /// </summary>
        public Visibility NoVisibility
        {
            get
            {
                if (this.MessageBoxButton == MessageBoxButton.YesNo
                    || this.MessageBoxButton == MessageBoxButton.YesNoCancel)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }
    }
}