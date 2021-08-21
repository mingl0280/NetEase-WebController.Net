using System.IO;

namespace NetEaseController
{
    public class ServerConfigs : GenericConfigurations
    {
        public void FirstInitialize()
        {
            AddOrSetItem("ServerAddress", "http://*:10180/");
            Save();
        }

        public ServerConfigs()
        {
            ConfigurationFile = "server.cfg";
            if (!File.Exists("server.cfg"))
            {
                FirstInitialize();
            }
            Load();
        }

        public static string ServerAddress
        {
            get
            {
                ServerConfigs Cfgs = new ServerConfigs();
                if (string.IsNullOrWhiteSpace((string)Cfgs["ServerAddress"]))
                {
                    Cfgs.FirstInitialize();
                    Cfgs.Load();
                }
                return Cfgs["ServerAddress"] != null ? (string)Cfgs["ServerAddress"] : "";
            }
        }
    }
}