using System;
using System.Windows.Input;

using SimpleMvvmToolkit;

using Umehluko.Tools.UI.Models;
using Umehluko.Tools.UI.Services;

// Toolkit namespace

// Toolkit extension methods

namespace Umehluko.Tools.UI.ViewModels
{
    /// <summary>
    /// This class extends ViewModelDetailBase which implements IEditableDataObject.
    /// <para>customerId
    /// Specify type being edited <strong>DetailType</strong> as the second type argument
    /// and as a parameter to the seccond ctor.
    /// </para>
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class CustomerViewModel : ViewModelDetailBase<CustomerViewModel, Customer>
    {
        #region Initialization and Cleanup

        // Add a member for ICustomerServiceAgent
        private ICustomerServiceAgent serviceAgent;

        // Default ctor
        public CustomerViewModel() { }

        // Ctor that accepts ICustomerServiceAgent
        public CustomerViewModel(ICustomerServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;
        }

        #endregion

        #region Notifications

        // TODO: Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties

        // TODO: Add properties using the mvvmprop code snippet

        #endregion

        #region Methods

        // Set the model to a new customer
        public void NewCustomer()
        {
            base.Model = this.serviceAgent.CreateCustomer();
        }

        #endregion

        #region Commands

        public ICommand NewCustomerCommand
        {
            get
            {
                return new DelegateCommand(this.NewCustomer);
            }
        }

        #endregion

        #region Completion Callbacks

        // TODO: Optionally add callback methods for async calls to the service agent

        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            this.Notify(this.ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}