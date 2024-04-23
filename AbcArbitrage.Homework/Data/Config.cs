using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcArbitrage.Homework.Data
{
    public class Config
    {
        public static string Data_Folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        public string Subscription_Json_Path = Path.Combine(Data_Folder, "Subscriptions.json");

        public string Client_Json_Path = Path.Combine(Data_Folder, "Clients.json");

        public string ContentPattern_Json_Path = Path.Combine(Data_Folder, "ContentPatterns.json");

        public Config()
        {
            initialling();
        }

        public void initialling()
        {
         
            if (!Directory.Exists(Data_Folder))
            {
                Directory.CreateDirectory(Data_Folder);
            }

          
            if (!File.Exists(Subscription_Json_Path))
            {
                File.Create(Subscription_Json_Path);
            }

         
            if (!File.Exists(Client_Json_Path))
            {
                File.Create(Client_Json_Path);
            }

           
            if (!File.Exists(ContentPattern_Json_Path))
            {
                File.Create(ContentPattern_Json_Path);
            }

            ClearAllData();
        }

        public void ClearAllData()
        {
            File.WriteAllText(Subscription_Json_Path, "");
            File.WriteAllText(Client_Json_Path, "");
            File.WriteAllText(ContentPattern_Json_Path, "");
        }
    }
}
