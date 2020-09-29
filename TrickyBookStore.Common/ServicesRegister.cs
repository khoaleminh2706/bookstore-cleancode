using Microsoft.Extensions.DependencyInjection;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.Subscriptions;

namespace TrickyBookStore.Common
{
    public static class ServicesRegister
    {
        public static IServiceCollection RegisterBookStoreServices(this IServiceCollection services)
        {
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ICustomerService, CustomerService>();
            return services;
        }
    }
}
