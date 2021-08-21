using System.IO;

namespace NetEaseController
{
    public class NetEaseConfig : GenericConfigurations
    {
        public void FirstInitialize()
        {
            AddOrSetItem("NEPath", "");
            Save();
        }

        public NetEaseConfig()
        {
            ConfigurationFile = "start_app.cfg";
            if (!File.Exists("start_app.cfg"))
            {
                FirstInitialize();
            }
            Load();
        }

        public static string NePath
        {
            get
            {
                NetEaseConfig Cfg = new NetEaseConfig();
                return Cfg["NEPath"] != null ? (string)Cfg["NEPath"] : "";
            }
            set
            {
                NetEaseConfig Cfg = new NetEaseConfig();
                Cfg.AddOrSetItem("NEPath", value);
                Cfg.Save();
            }
        }
    }
}