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
            var totalPrice = customer.Subscriptions
                .Sum(sub => sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);
            return totalPrice;
        }

        private double ProcessTotalPrice(
            IList<Book> books,
            IList<Subscription> subscriptions)
        {
            double price = 0;

            var addictedCategories = subscriptions
                .Where(sub => sub.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                .ToList();

            var standardSubTier = subscriptions
                .Except(addictedCategories)
                .ToList();

            // tính category addicted trước hết
            if (addictedCategories.Count > 0)
            {
                price += AddictedCategoriesPricing(ref books, addictedCategories);
            }

            if (standardSubTier.Count > 0)
            {
                price += StandardSubscriptionPricing(ref books, standardSubTier);
            }

            if (books.Count > 0)
            {
                price += BooksWithoutSubcriptionPricing(ref books);
            }

            return price;
        }

        public double StandardSubscriptionPricing(
            ref IList<Book> books, 
            IList<Subscription> subscriptions)
        {
            double price = 0;
            
            foreach(var subscription in subscriptions)
            {
                if (books.Count == 0)
                {
                    break;
                }

                var oldBooks = books
                    .Where(book => book.IsOld == true)
                    .ToList();
                double discountOldBook = subscription.PriceDetails[Constants.PriceDetailsType.DiscountOldBook];
                price += oldBooks.Sum(book => book.Price * (1 - discountOldBook));

                books = books.Except(oldBooks).ToList();

                int threshold = Convert.ToInt32(
                    subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBookThreshold]
                    );

                var newBooksToApplyDiscount = books
                    .Where(book => book.IsOld == false)
                    .Take(threshold);

                double discountNewBook = subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBook];

                price += newBooksToApplyDiscount
                    .Sum(book => book.Price * (1 - discountNewBook));

                books = books.Except(newBooksToApplyDiscount).ToList();
            }

            return price;
        }

        public double AddictedCategoriesPricing(
            ref IList<Book> books,
            IList<Subscription> subscriptions)
        {
            double price = 0;

            foreach(var subscription in subscriptions)
            {
                if (subscription.BookCategoryId == null)
                {
                    continue;
                }

                // fileter old books of category
                books = books
                    .Where(book => !(book.IsOld == true && book.CategoryId == subscription.BookCategoryId))
                    .ToList();

                int threshold = Convert.ToInt32(
                    subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBook]
                    );
                double discountNewBook = subscription.PriceDetails[Constants.PriceDetailsType.DiscountNewBook];

                var categoryNewBooks = books
                    .Where(book => book.IsOld == false && book.CategoryId == subscription.BookCategoryId)
                    .Take(threshold)
                    .ToList();

                price += categoryNewBooks
                    .Sum(book => book.Price * (1 - discountNewBook));

                books = books.Except(categoryNewBooks).ToList();
            }

            return price;
        }

        public double BooksWithoutSubcriptionPricing(ref IList<Book> books)
        {
            return books.Sum(book => book.Price);
        }
    }
}
