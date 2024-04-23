using AbcArbitrage.Homework.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcArbitrage.Homework.Data
{
    public class SubscriptionData
    {

        public List<Subscription> Get()
        {
            return Global.Data_Subscription;
        }

        public List<Subscription> Add(IEnumerable<Subscription> subscription)
        {
            foreach (Subscription sub in subscription)
            {
                //if (Global.Data_Subscription.Any(x => x.ConsumerId.Equals(sub.ConsumerId) && x.MessageTypeId.Equals(sub.MessageTypeId)))
                //{
                //    var ExistingSub = Global.Data_Subscription.Where(x => x.ConsumerId.Equals(sub.ConsumerId) && x.MessageTypeId.Equals(sub.MessageTypeId)).First();
                    
                //}
                Global.Data_Subscription.Add(sub);
            }

            //SaveDataToJson();

            return Global.Data_Subscription;
        }

        public List<Subscription> Remove(IEnumerable<Subscription> subscription)
        {

            foreach (Subscription sub in subscription)
            {
                Global.Data_Subscription.Remove(sub);
            }

            //SaveDataToJson();
            return Global.Data_Subscription;
        }

        public List<Subscription> RemoveForConsumer(ClientId consumer)
        {
            List<Subscription> _to_remove = Global.Data_Subscription.Where(x => x.ConsumerId.Equals(consumer)).ToList();
            foreach (Subscription sub in _to_remove)
            {
                Global.Data_Subscription.Remove(sub);
            }

            // SaveDataToJson();
            return Global.Data_Subscription;
        }


    }
}
