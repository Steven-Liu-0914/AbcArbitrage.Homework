// Copyright (C) Abc Arbitrage Asset Management - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Olivier Coanet <o.coanet@abc-arbitrage.com>, 2020-10-01

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace AbcArbitrage.Homework.Routing
{
    public class SubscriptionIndex : ISubscriptionIndex
    {
        public List<Subscription> Data_Subscription;

        public SubscriptionIndex()
        {
            Data_Subscription = new List<Subscription>();
        }

        public void AddSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            // TODO
            foreach (Subscription sub in subscriptions)
            {
                Data_Subscription.Add(sub);
            }
        }

        public void RemoveSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            // TODO          
            foreach (Subscription sub in subscriptions)
            {
                var filterList = Data_Subscription.Where(x => x.MessageTypeId.Equals(sub.MessageTypeId));
                if (filterList.Any())
                {
                    if (sub.ContentPattern.Parts.Any())
                    {
                        for (int i = 0; i < sub.ContentPattern.Parts.Count; i++)
                        {
                            string condition = sub.ContentPattern.Parts[i];
                            if (condition == "*")
                            {
                                filterList = filterList.Where(x => !string.IsNullOrEmpty(x.ContentPattern.Parts[i]));
                            }
                            else
                            {
                                filterList = filterList.Where(x => x.ContentPattern.Parts.Count >= i + 1 ? x.ContentPattern.Parts[i] == condition : false);
                            }

                            if (filterList.Any())
                            {
                                break;
                            }
                        }
                    }
                }

                if (filterList.Any())
                {
                    foreach (Subscription sub_to_remove in filterList.ToList())
                    {
                        Data_Subscription.Remove(sub_to_remove);
                    }
                }
            }
        }

        public void RemoveSubscriptionsForConsumer(ClientId consumer)
        {
            // TODO
            List<Subscription> _to_remove = Data_Subscription.Where(x => x.ConsumerId.Equals(consumer)).ToList();
            foreach (Subscription sub in _to_remove)
            {
                Data_Subscription.Remove(sub);
            }
        }

        public IEnumerable<Subscription> FindSubscriptions(MessageTypeId messageTypeId, MessageRoutingContent routingContent)
        {
            // TODO
            List<Subscription> result = new List<Subscription>();

            if (!routingContent.Equals(MessageRoutingContent.Empty) && routingContent.Parts != null)
            {
                //Get valid filter conditions first to exclude empty or null part.
                var valid_Parts = routingContent.Parts.Where(x => !string.IsNullOrEmpty(x));

                if (valid_Parts.Any())
                {
                    List<string> all_possible_parts_combines = GetPossiblePartsByCondition(valid_Parts.ToList());

                    //After getting all possible combinations, we could quick use linq to find those list in possible combinations.
                    //When parts in existing data length is more than the condition parts length, we can only take the top X
                    var Matched_messageType_routingContent = Data_Subscription.Where(x => x.MessageTypeId.Equals(messageTypeId) &&
                    ((all_possible_parts_combines.Contains
                    (string.Join(".", x.ContentPattern.Parts.Take(x.ContentPattern.Parts.Count > valid_Parts.Count() ? valid_Parts.Count() : x.ContentPattern.Parts.Count))))
                    || !x.ContentPattern.Parts.Where(x => !string.IsNullOrEmpty(x)).Any()));

                    if (Matched_messageType_routingContent.Any())
                    {
                        foreach (var sub in Matched_messageType_routingContent)
                        {
                            if (!result.Contains(sub))
                            {
                                result.Add(sub);
                            }
                        }
                    }
                }
            }
            else // to improve performance, quickly return result only match with messageTypeId
            {
                var Matched_messageType = Data_Subscription.Where(x => x.MessageTypeId.Equals(messageTypeId));

                if (Matched_messageType.Any())
                {
                    result = Matched_messageType.ToList();
                }
            }

            return result;
        }


        private List<string> GetPossiblePartsByCondition(List<string> Parts)
        {
            List<string> result = new List<string>();

            for (int index = 0; index < Parts.Count; index++)
            {
                string[] conditions = Parts.Take(index + 1).ToArray();

                int n = conditions.Length;
                List<List<string>> combinations = new List<List<string>>();

                // {1} => [1]/[*] == > 2^1
                // {1,2} => [*][*] / [*][2] / [1][*] / [1][2] => 2^2
                // {1,2,3} => [*][*][*] / [*][*][3]/ [*][2][*] / [*][2][3] / [1][*][*] / [1][*][3]/ [1][2][*] / [1][2][3] => 2^3
                int count_combination = (int)Math.Pow(2, n);

                for (int i = 0; i < count_combination; i++)
                {
                    List<string> subset = new List<string>();
                    for (int j = 0; j < n; j++)
                    {
                        if ((i & (int)Math.Pow(2, j)) > 0) // If the number after moving to left X times is not empty, then we insert the number
                        {
                            subset.Add(conditions[j].ToString());
                        }
                        else
                        {
                            subset.Add("*"); // else we use the * to take the position in array
                        }
                    }
                    combinations.Add(subset);
                }


                foreach (var subset in combinations)
                {
                    string combineString = string.Join(".", subset);
                    result.Add(combineString);
                }
            }

            return result;
        }
    }
}
