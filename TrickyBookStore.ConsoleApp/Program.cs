using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TrickyBookStore.Common;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Customers;

namespace TrickyBookStore.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .RegisterBookStoreServices()
                .BuildServiceProvider();

            var customerService = serviceProvider.GetService<ICustomerService>();
            var customer = customerService.GetCustomerById(5);

            // Calculate subcription fixed price
            PriceCalulator priceCalulator = new PriceCalulator(customer);
            Console.WriteLine("Fixed Subcription Price: " + priceCalulator.CalculateFixedPrice().ToString());

            Console.ReadLine();
        }
    }

    class PriceCalulator
    {
        public double FixedPrice { get; private set; }
        public Customer Customer { get; private set; }

        public PriceCalulator(Customer customer)
        {
            Customer = customer;
            FixedPrice = 0;
        }
        
        public double CalculateFixedPrice()
        {
            FixedPrice = Customer.Subscriptions.Sum(sub => sub.PriceDetails[Constants.PriceDetailsType.FixedPrice]);
            return FixedPrice;
        }
    }
}
