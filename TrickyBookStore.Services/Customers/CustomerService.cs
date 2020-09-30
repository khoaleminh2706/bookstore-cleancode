using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Subscriptions;

namespace TrickyBookStore.Services.Customers
{
    internal class CustomerService : ICustomerService
    {
        ISubscriptionService SubscriptionService { get; }

        public CustomerService(ISubscriptionService subscriptionService)
        {
            SubscriptionService = subscriptionService;
        }

        public Customer GetCustomerById(long id)
        {
            var targetCustomer = Store.Customers.Data.Where(customer => customer.Id == id).FirstOrDefault();

            var subscriptionList = SubscriptionService.GetSubscriptions(targetCustomer.SubscriptionIds.ToArray());

            if (subscriptionList != null && subscriptionList.Count != 0)
            {
                targetCustomer.Subscriptions = subscriptionList;
            }
            else
            {
                throw new System.Exception($"Customer {id} does not have a subcription"); ;
            }

            return targetCustomer;
        }
    }
}
