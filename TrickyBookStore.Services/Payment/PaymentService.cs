using System;
using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;
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
            var transactionList = PurchaseTransactionService
                .GetPurchaseTransactions(customerId, fromDate, toDate);

            var fixedPrice = CalculateFixedPrice(targetCustomer);

            var transactionsTotalPrice = ProcessTotalPrice(transactionList.Select(tran => tran.Book).ToList(), targetCustomer.Subscriptions);

            return fixedPrice + transactionsTotalPrice;
        }

        private double CalculateFixedPrice(Customer customer)
        {
            // tinsh cao nhất + thêm số category addtted accounts
            var addictedCategoriesPrice = customer.Subscriptions
                .Where(sub => sub.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                .Sum(sub => sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);

            var priceFromOtherSubcriptions =  customer.Subscriptions
                .Where(sub => sub.SubscriptionType != SubscriptionTypes.CategoryAddicted)
                .Sum(sub => 
                sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);

            return addictedCategoriesPrice + priceFromOtherSubcriptions;
        }

        private double ProcessTotalPrice(
            List<Book> books, 
            IList<Subscription> subscriptions)
        {
            double totalPrice = 0;

            // get highest subcription
            var notIncludeAdditedCategories = subscriptions
                .Where(sub => sub.SubscriptionType != SubscriptionTypes.CategoryAddicted)
                .ToList()[0];

            // calculate old books
            totalPrice += books.Where(book => book.IsOld == true)
                .Sum(book => book.Price * (1  - notIncludeAdditedCategories.PriceDetails[Constants.PriceDetailsType.DiscountOldBook]));

            // calculate for new books
            int threshold = Convert.ToInt32(notIncludeAdditedCategories.PriceDetails[Constants.PriceDetailsType.DiscountNewBookThreshold]);

            var newBooks = books.Where(book => book.IsOld == false);

            totalPrice += newBooks
                .Sum(book => book.Price * (1 - notIncludeAdditedCategories.PriceDetails[Constants.PriceDetailsType.DiscountNewBook]));

            totalPrice += newBooks
                .Sum(book => book.Price * (1 - notIncludeAdditedCategories.PriceDetails[Constants.PriceDetailsType.DiscountNewBook]));

            return totalPrice;
        }
    }
}
