using System;
using System.Linq;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Books;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.PurchaseTransactions;

namespace TrickyBookStore.Services.Payment
{
    internal class PaymentService : IPaymentService
    {
        ICustomerService CustomerService { get; }
        IPurchaseTransactionService PurchaseTransactionService { get; }

        public PaymentService(ICustomerService customerService, 
            IPurchaseTransactionService purchaseTransactionService)
        {
            CustomerService = customerService;
            PurchaseTransactionService = purchaseTransactionService;
        }

        public double GetPaymentAmount(
            long customerId, 
            DateTimeOffset fromDate, 
            DateTimeOffset toDate)
        {
            var targetCustomer = CustomerService.GetCustomerById(customerId);
            var transactions = PurchaseTransactionService
                .GetPurchaseTransactions(customerId, fromDate, toDate);

            var fixedPrice = CalculateFixedPrice(targetCustomer);
            return fixedPrice;
        }

        private double CalculateFixedPrice(Customer customer)
        {
            return customer.Subscriptions.Sum(sub => 
                sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);
        }
    }
}
