using Microsoft.Extensions.DependencyInjection;
using TrickyBookStore.Common;
using System;
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
            var customer = customerService.GetCustomerById(1);
            Console.WriteLine(customer.Id);
            Console.ReadLine();
        }
    }
}
