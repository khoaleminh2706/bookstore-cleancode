using System.Collections.Generic;
using System.Security.Cryptography;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Store
{
    public static class Subscriptions
    {
        public static readonly IEnumerable<Subscription> Data = new List<Subscription>
        {
            new Subscription { Id = 1, SubscriptionType = SubscriptionTypes.Paid, Priority = ...,
                PriceDetails = new Dictionary<string, double>
                {
                    { Constants.PriceDetailsType.FixedPrice, 50 },
                    { Constants.PriceDetailsType.DiscountAmount, 0.05 },
                    { Constants.PriceDetailsType.DiscountThreshold, 3 },
                }
            },
            new Subscription { Id = 2, SubscriptionType = SubscriptionTypes.Free, Priority = ...,
                PriceDetails = new Dictionary<string, double>
                {
                    { Constants.PriceDetailsType.FixedPrice, 0 },
                    { Constants.PriceDetailsType.DiscountAmount, 0.1 },
                    { Constants.PriceDetailsType.DiscountThreshold, 3 },
                }
            },
            new Subscription { Id = 3, SubscriptionType = SubscriptionTypes.Premium, Priority = 1,
                PriceDetails = new Dictionary<string, double>
                {
                   { Constants.PriceDetailsType.FixedPrice, 200 },
                   { Constants.PriceDetailsType.DiscountAmount, 0.15 },
                   { Constants.PriceDetailsType.DiscountThreshold, 3 },
                }
            },
            new Subscription { Id = 4, SubscriptionType = SubscriptionTypes.CategoryAddicted, Priority = ...,
                PriceDetails = new Dictionary<string, double>
                {
                  { Constants.PriceDetailsType.FixedPrice, 75 },
                  { Constants.PriceDetailsType.DiscountAmount, 3 },
                }              
            },
            new Subscription { Id = 5, SubscriptionType = SubscriptionTypes.CategoryAddicted, Priority = ...,
                PriceDetails = new Dictionary<string, double>
                {
                    { Constants.PriceDetailsType.FixedPrice, 75 },
                    { Constants.PriceDetailsType.DiscountAmount, 0.15 },
                    { Constants.PriceDetailsType.DiscountThreshold, 3 },
                },
                BookCategoryId = 1
            },
            new Subscription { Id = 6, SubscriptionType = SubscriptionTypes.CategoryAddicted, Priority = ...,
                PriceDetails = new Dictionary<string, double>
                {
                    { Constants.PriceDetailsType.FixedPrice, 75 },
                    { Constants.PriceDetailsType.DiscountAmount, 0.15 },
                    { Constants.PriceDetailsType.DiscountThreshold, 3 },
                },
                BookCategoryId = 3
            }
        };
    }
}
