#region

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using BenMVVM;

using SimpleMvvmToolkit;

// Toolkit namespace

// Toolkit extension methods
#endregion

namespace Umehluko.Tools.UI.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MainPageViewModel : ViewModelBase<MainPageViewModel>
    {
        #region Initialization and Cleanup

        // Default ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.IsBusy = false;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        /// <summary>
        /// The error notice.
        /// </summary>
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties

        // Add properties using the mvvmprop code snippet

        /// <summary>
        /// The banner text.
        /// </summary>
        private string bannerText = "Tariffs Management";

        /// <summary>
        /// The is busy.
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// The child view model.
        /// </summary>
        private object childViewModel;

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;
                this.NotifyPropertyChanged(b => b.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the child view model.
        /// </summary>
        public object ChildViewModel
        {
            get
            {
                return this.childViewModel;
            }

            set
            {
                this.childViewModel = value;
                this.NotifyPropertyChanged(c => c.ChildViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the banner text.
        /// </summary>
        public string BannerText
        {
            get
            {
                if (this.IsInDesignMode())
                {
                    return "Banner";
                }

                return this.bannerText;
            }

            set
            {
                this.bannerText = value;
                this.NotifyPropertyChanged(m => m.BannerText);
            }
        }

        /// <summary>
        ///   Gets the selected entity command.
        /// </summary>
        /// <value>
        ///   The selected entity command.
        /// </value>
        public ICommand SelectedTabChangedCommnad
        {
            get
            {
                return new RelayCommand(this.SelectedTabChanged);
            }
        }

        /// <summary>
        /// The selected tab changed.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        private void SelectedTabChanged(object t)
        {
        }

        /*ObservableCollection<ViewModelDetailBase> _workspaces;
        public ObservableCollection<ViewModelDetailBase> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<ViewModelDetailBase>();
                }
                return _workspaces;
            }
            set
            {
                _workspaces = value;

            }
        }*/

        #endregion

        #region Methods

        // TODO: Add methods that will be called by the view
        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent
        #endregion

        #region Helpers

        // Helper method to notify View of an error
        /// <summary>
        /// The notify error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            this.Notify(this.ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}