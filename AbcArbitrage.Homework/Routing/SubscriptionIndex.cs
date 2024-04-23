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
        public SubscriptionData _subscriptionData = new SubscriptionData();
        public SubscriptionIndex()
        {
            Global.Data_Subscription = new List<Subscription>();
        }

        public void AddSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            // TODO
            Global.Data_Subscription = _subscriptionData.Add(subscriptions);
        }

        public void RemoveSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            // TODO
            Global.Data_Subscription = _subscriptionData.Remove(subscriptions);
        }

        public void RemoveSubscriptionsForConsumer(ClientId consumer)
        {
            // TODO
            Global.Data_Subscription = _subscriptionData.RemoveForConsumer(consumer);
        }

        public List<List<string>> GetPossiblePartsByCondition(MessageRoutingContent routingContent)
        {
            List<List<string>> result = new List<List<string>>();
            if (routingContent.Parts != null)
            {
                var cleanfilter = routingContent.Parts.Where(x => !string.IsNullOrEmpty(x)).ToList();
                for (int x = 0; x < cleanfilter.Count; x++)
                {
                    string[] conditions = cleanfilter.Take(x + 1).ToArray();

                    int n = conditions.Length;
                    // 生成所有可能的组合
                    List<List<string>> combinations = new List<List<string>>();
                    for (int i = 0; i < (1 << n); i++)
                    {
                        List<string> subset = new List<string>();
                        for (int j = 0; j < n; j++)
                        {
                            if ((i & (1 << j)) > 0)
                            {
                                subset.Add(conditions[j].ToString());
                            }
                            else
                            {
                                subset.Add("*");
                            }
                        }
                        combinations.Add(subset);
                    }

                    // 输出格式化结果
                    foreach (var subset in combinations)
                    {
                        result.Add(subset);
                    }
                }
            }
            return result;
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
                        result.Clear();
                        List<List<string>> possible_results = GetPossiblePartsByCondition(routingContent);
                        foreach (var sub in Global.Data_Subscription.Where(x => x.MessageTypeId.Equals(messageTypeId)))
                        {
                            foreach (var p in possible_results)
                            {
                                var cleanfilter = routingContent.Parts.Where(x => !string.IsNullOrEmpty(x)).ToList();
                                int take_count = sub.ContentPattern.Parts.Count;
                                if (take_count > cleanfilter.Count) { take_count = cleanfilter.Count; }

                                if (string.Join(",", p) == string.Join(",", sub.ContentPattern.Parts.Take(take_count).ToArray()))
                                {
                                    if (!result.Contains(sub))
                                    {
                                        result.Add(sub);
                                    }
                                }
                            }
                        }

                    }
                }
            }

            return result;
        }
    }
}
