using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlTypes;
using TrickyBookStore.Common;
using TrickyBookStore.Services.Payment;

namespace TrickyBookStore.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var year = 2018;
            var month = 1;

            var serviceProvider = new ServiceCollection()
                .RegisterBookStoreServices()
                .BuildServiceProvider();

            var paymentService = serviceProvider.GetService<IPaymentService>();

            var ngayDauThang = NgayDauThang(year, month);
            var amount = paymentService.GetPaymentAmount(
                5, 
                new DateTimeOffset(ngayDauThang), 
                new DateTimeOffset(ngayDauThang.AddMonths(1).AddDays(-1)));

            // Calculate subcription fixed price
            Console.WriteLine("Fixed Subcription Price: " + amount);

            Console.ReadLine();
        }

        static DateTime NgayDauThang(int nam, int thang)
        {
            return new DateTime(nam, thang, 1);
        }
    }
}
