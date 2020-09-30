using System.Collections.Generic;
using System.Linq;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Subscriptions
{
    internal class SubscriptionService : ISubscriptionService
    {
        public IList<Subscription> GetSubscriptions(params int[] ids)
        {
            var data =  Store.Subscriptions.Data
                .Where(sub => ids.Contains(sub.Id))
                .OrderBy(sub => sub.Priority)
                .ThenBy(sub => sub.Id)
                .ToList();
            
            if (data.Count == 0)
            {
                data.Add(Store.Subscriptions.Data.First(sub => sub.SubscriptionType == SubscriptionTypes.Free));
            }
            return data;
        }
    }
}
