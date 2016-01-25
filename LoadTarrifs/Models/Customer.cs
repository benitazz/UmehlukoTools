using SimpleMvvmToolkit;

namespace Umehluko.Tools.UI.Models
{
    public class Customer : ModelBase<Customer>
    {
        private int customerId;
        public int CustomerId
        {
            get { return this.customerId; }
            set
            {
                this.customerId = value;
                this.NotifyPropertyChanged(m => m.CustomerId);
            }
        }

        private string customerName;
        public string CustomerName
        {
            get { return this.customerName; }
            set
            {
                this.customerName = value;
                this.NotifyPropertyChanged(m => m.CustomerName);
            }
        }

        private string city;
        public string City
        {
            get { return this.city; }
            set
            {
                this.city = value;
                this.NotifyPropertyChanged(m => m.City);
            }
        }
    }
}
