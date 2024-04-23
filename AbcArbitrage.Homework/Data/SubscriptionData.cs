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

        public List<Subscription> Data_Subscriptions;

        Config config = new Config();
        public SubscriptionData()
        {         
            string Subscription_Json_Data = File.ReadAllText(config.Subscription_Json_Path);

            List<Subscription>? Data_Subscription = JsonConvert.DeserializeObject<List<Subscription>>(Subscription_Json_Data);

            if (Data_Subscription != null)
            {
                Data_Subscriptions = Data_Subscription;
            }
            else
            {
                Data_Subscriptions = new List<Subscription>();
            }
        }

        public List<Subscription> Get()
        {
            return Data_Subscriptions;
        }

        public List<Subscription> Add(IEnumerable<Subscription> subscription)
        {
            foreach (Subscription sub in subscription)
            {
                Global.Data_Subscription.Add(sub);
            }

            //SaveDataToJson();

            return Global.Data_Subscription;
        }

        public List<Subscription> Remove(IEnumerable<Subscription> subscription)
        {
            
            foreach (Subscription sub in subscription)
            {
                Data_Subscriptions.Remove(sub);
            }
           
            SaveDataToJson();
            return Data_Subscriptions;
        }

        public List<Subscription> RemoveForConsumer(ClientId consumer)
        {
            List<Subscription> _to_remove = Data_Subscriptions.Where(x=>x.ConsumerId.Equals(consumer)).ToList();
            foreach(Subscription sub in _to_remove)
            {
                Data_Subscriptions.Remove(sub);
            }

            SaveDataToJson();
            return Data_Subscriptions;
        }


        private void SaveDataToJson()
        {
            string jsonContent = JsonConvert.SerializeObject(Data_Subscriptions);
            File.WriteAllText(config.Subscription_Json_Path, jsonContent);
        }
    }
}
