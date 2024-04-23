// Copyright (C) Abc Arbitrage Asset Management - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Olivier Coanet <o.coanet@abc-arbitrage.com>, 2020-10-01

using AbcArbitrage.Homework.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AbcArbitrage.Homework.Routing
{
    public class SubscriptionIndex : ISubscriptionIndex
    {
        public IEnumerable<Subscription> _AllSubscriptions;
        public SubscriptionData _subscriptionData = new SubscriptionData();
        public SubscriptionIndex()
        {
            _AllSubscriptions = _subscriptionData.Get();
        }

        public void AddSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            // TODO
            _AllSubscriptions = _subscriptionData.Add(subscriptions);
        }

        public void RemoveSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            // TODO
            _AllSubscriptions = _subscriptionData.Remove(subscriptions);
        }

        public void RemoveSubscriptionsForConsumer(ClientId consumer)
        {
            // TODO
            _AllSubscriptions = _subscriptionData.RemoveForConsumer(consumer);
        }

        public IEnumerable<Subscription> FindSubscriptions(MessageTypeId messageTypeId, MessageRoutingContent routingContent)
        {
            // TODO
            List<Subscription> result = new List<Subscription>();
            if (Global.Data_Subscription.Where(x => x.MessageTypeId.Equals(messageTypeId)).Any())
            {
                result = Global.Data_Subscription.Where(x => x.MessageTypeId.Equals(messageTypeId)).ToList();
                if (routingContent.Parts != null)
                {
                    if (routingContent.Parts.Any())
                    {
                        result = Global.Data_Subscription.Where(x => x.MessageTypeId.Equals(messageTypeId) && x.ContentPattern.Equals(routingContent)).ToList();
                    }
                }
            }

            return result;
        }
    }
}
