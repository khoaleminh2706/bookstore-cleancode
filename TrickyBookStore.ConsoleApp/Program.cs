using Microsoft.Extensions.DependencyInjection;
using System;
using TrickyBookStore.Common;
using TrickyBookStore.Services.Payment;

namespace TrickyBookStore.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Welcome...");
            Console.Write("Please input year: ");
            int.TryParse(Console.ReadLine(), out int year);
            Console.Write("Please input month: ");
            int.TryParse(Console.ReadLine(), out int month);
            Console.Write("Please input customerId: ");
            long.TryParse(Console.ReadLine(), out long customerId);

            var serviceProvider = new ServiceCollection()
                .RegisterBookStoreServices()
                .BuildServiceProvider();

            var paymentService = serviceProvider.GetService<IPaymentService>();

            var firstDayOfMonth = FirstDayOfMonth(year, month);
            var amount = paymentService.GetPaymentAmount(
                customerId, 
                new DateTimeOffset(firstDayOfMonth), 
                new DateTimeOffset(firstDayOfMonth.AddMonths(1).AddDays(-1)));

            Console.WriteLine("Total Price: " + amount);

            Console.ReadLine();
        }

        static DateTime FirstDayOfMonth(int year, int month)
        {
            return new DateTime(year, month, 1);
        }
    }
}
