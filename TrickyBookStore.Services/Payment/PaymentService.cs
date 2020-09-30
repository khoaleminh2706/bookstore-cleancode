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

            double transactionsTotalPrice = 0;
            if (transactionList.Count == 0)
            {
                transactionsTotalPrice = 0;
            }
            else
            {
                transactionsTotalPrice = ProcessTotalPrice(transactionList.Select(tran => tran.Book).ToList(), targetCustomer.Subscriptions);
            }

            return fixedPrice + transactionsTotalPrice;
        }

        private double CalculateFixedPrice(Customer customer)
        {
            // tinsh cao nhất + thêm số category addtted accounts
            var addictedCategoriesPrice = customer.Subscriptions
                .Where(sub => sub.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                .Sum(sub => sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);

            var priceFromOtherSubcriptions = customer.Subscriptions
                .Where(sub => sub.SubscriptionType != SubscriptionTypes.CategoryAddicted)
                .Sum(sub =>
                sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);

            return addictedCategoriesPrice + priceFromOtherSubcriptions;
        }

        private double ProcessTotalPrice(
            IList<Book> books,
            IList<Subscription> subscriptions)
        {
            double price = 0;

            var addictedCategories = subscriptions
                .Where(sub => sub.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                .ToList();

            // get highest subcription
            var highestSubTier = subscriptions
                .Where(sub => sub.SubscriptionType != SubscriptionTypes.CategoryAddicted)
                .ToList().FirstOrDefault();
            // tính category addicted trước hết
            if (highestSubTier != null)
            {
                 if (highestSubTier.Priority > addictedCategories[0].Priority)
                {
                    price += StandardSubscriptionPricing(books, highestSubTier);
                    price += AddictedCategoriesPricing(books, addictedCategories);
                }
                else
                {
                    price += AddictedCategoriesPricing(books, addictedCategories);
                    price += StandardSubscriptionPricing(books, highestSubTier);
                }
            }
            else
            {
                price += AddictedCategoriesPricing(books, addictedCategories);
            }


            return price;
        }

        public double StandardSubscriptionPricing(
            IList<Book> books, 
            Subscription subscription)
        {
            double price = 0;
            var newBook = books.Where(book => book.IsOld == false).ToList();
            int threshold = Convert.ToInt32(
                subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBookThreshold]
                );
            double discountNewBook = subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBook];

            price += newBook.Take(threshold)
                .Sum(book => book.Price * (1 - discountNewBook));

            price += newBook.Skip(threshold)
                .Sum(book => book.Price * (1 - discountNewBook));

            return price;
        }

        public double AddictedCategoriesPricing(
            IList<Book> books,
            IList<Subscription> subscriptions)
        {
            double price = 0;

            foreach(var subscription in subscriptions)
            {
                if (subscription.BookCategoryId == null)
                {
                    continue;
                }
                int threshold = Convert.ToInt32(
                    subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBook]
                    );
                double discountNewBook = subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBook];

                var categoryBooks = books.Where(book => book.IsOld == false && book.CategoryId == subscription.BookCategoryId)
                    .ToList();

                price += categoryBooks.Take(threshold)
                    .Sum(book => book.Price * (1 - discountNewBook));
                price += categoryBooks.Skip(threshold)
                    .Sum(book => book.Price * (1 - discountNewBook));
            }

            return price;
        }
    }
}
