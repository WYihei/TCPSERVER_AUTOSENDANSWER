using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace tcp_auto
{
    internal class Config
    {
        const string configFilePath = "Config.config";
        static Config instance;
        public static Config Instance { get
            {
                if (instance == null)
                {
                    if (File.Exists(configFilePath))
                    {
                        string json = File.ReadAllText(configFilePath);
                        instance = JsonConvert.DeserializeObject<Config>(json);
                    }
                    else
                    {
                        instance = new Config();
                        instance.EchoKeyValuePairs = new List<EchoKeyValuePair>();
                    }
                }
                return instance;
            } }
        Config()
        {
        }
        public string LastIpPort;
        public List<EchoKeyValuePair> EchoKeyValuePairs;
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(configFilePath, json);
        }
    }
}
