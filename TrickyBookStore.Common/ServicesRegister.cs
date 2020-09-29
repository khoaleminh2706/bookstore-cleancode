using Microsoft.Extensions.DependencyInjection;
using TrickyBookStore.Services.Books;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.Payment;
using TrickyBookStore.Services.PurchaseTransactions;
using TrickyBookStore.Services.Subscriptions;

namespace TrickyBookStore.Common
{
    public static class ServicesRegister
    {
        public static IServiceCollection RegisterBookStoreServices(this IServiceCollection services)
        {
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IPurchaseTransactionService, PurchaseTransactionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            return services;
        }
    }
}
