using Umehluko.Tools.UI.Models;

namespace Umehluko.Tools.UI.Services
{
    /// <summary>
    /// The CustomerServiceAgent interface.
    /// </summary>
    public interface ICustomerServiceAgent
    {
        /// <summary>
        /// The create customer.
        /// </summary>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        Customer CreateCustomer();
    }
}