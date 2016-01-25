using Umehluko.Tools.UI.Models;
using Umehluko.Tools.UI.Services;

namespace LoadTariffs
{
    /// <summary>
    /// The mock customer service agent.
    /// </summary>
    public class MockCustomerServiceAgent : ICustomerServiceAgent
    {
        // Create a fake customer
        /// <summary>
        /// The create customer.
        /// </summary>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Customer CreateCustomer()
        {
            return new Customer { CustomerId = 1, CustomerName = "John Doe", City = "Dallas" };
        }
    }
}