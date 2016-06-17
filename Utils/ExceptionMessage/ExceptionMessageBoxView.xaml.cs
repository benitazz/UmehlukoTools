#region

using System.ComponentModel;
using System.Windows;

#endregion

namespace Umehluko.Tools.Utils.ExceptionMessage
{
    /// <summary>
    /// Interaction logic for ExceptionMessageBoxView.xaml
    /// </summary>
    public partial class ExceptionMessageBoxView : Window, IExceptionMessageBoxView
    {
        /// <summary>
        /// The m_ message box result.
        /// </summary>
        private MessageBoxResult m_MessageBoxResult = MessageBoxResult.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMessageBoxView"/> class.
        /// </summary>
        public ExceptionMessageBoxView()
        {
            this.InitializeComponent();
        }

        #region IExceptionMessageBoxView Members

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public ExceptionMessageBoxViewModel Model
        {
            get
            {
                return (ExceptionMessageBoxViewModel)this.DataContext;
            }

            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// The show exception message.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        public MessageBoxResult ShowExceptionMessage()
        {
            return this.ShowExceptionMessage(null);
        }

        /// <summary>
        /// The show exception message.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBoxResult"/>.
        /// </returns>
        public MessageBoxResult ShowExceptionMessage(Window owner)
        {
            this.Owner = owner;
            this.m_MessageBoxResult = MessageBoxResult.None;
            this.ShowDialog();

            return this.m_MessageBoxResult;
        }

        #endregion

        /// <summary>
        /// The ok button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_MessageBoxResult = MessageBoxResult.OK;
            this.DialogResult = true;
        }

        /// <summary>
        /// The yes button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_MessageBoxResult = MessageBoxResult.Yes;
            this.DialogResult = true;
        }

        /// <summary>
        /// The no button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_MessageBoxResult = MessageBoxResult.No;
            this.DialogResult = false;
        }

        /// <summary>
        /// The cancel button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_MessageBoxResult = MessageBoxResult.Cancel;
            this.DialogResult = false;
        }

        /// <summary>
        /// The window_ closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.m_MessageBoxResult = MessageBoxResult.Cancel;
            this.DialogResult = false;
        }
    }
}