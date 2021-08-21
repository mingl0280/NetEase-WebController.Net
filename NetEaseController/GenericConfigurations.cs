using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace NetEaseController
{
    public class GenericConfigurations
    {
        public ReadOnlyCollection<string> Properties => _PropertiesStore.AsReadOnly();

        public object this[string Name]
        {
            get
            {
                if (cfgStore.ContainsKey(Name))
                    return cfgStore[Name];
                return null;
            }
            set => AddOrSetItem(Name, value);
        }

        public string ConfigurationFile { get; set; }

        protected Dictionary<string, object> cfgStore = new Dictionary<string, object>();

        private readonly List<string> _PropertiesStore = new List<string>();

        public void Load()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationFile))
            {
                ConfigurationFile = "app.cfg";
            }

            if (!File.Exists(ConfigurationFile))
                return;
            using (StreamReader CfgIn = new StreamReader(ConfigurationFile))
            {
                while (true)
                {
                    var Str = CfgIn.ReadLine();

                    bool InValueArea = false;
                    string Name = "", Value = "";
                    if (Str != null)
                        foreach (var ChrCurrent in Str)
                        {
                            if (ChrCurrent == ':' && !InValueArea)
                            {
                                InValueArea = true;
                                continue;
                            }

                            if (!InValueArea)
                            {
                                Name += ChrCurrent;
                            }
                            else
                            {
                                Value += ChrCurrent;
                            }
                        }
                    AddOrSetItem(Name, Value);
                    if (CfgIn.EndOfStream)
                        break;
                }
            }
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationFile))
            {
                ConfigurationFile = "app.cfg";
            }

            using (StreamWriter Writer = new StreamWriter(new FileStream(ConfigurationFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)))
            {
                foreach (var Item in cfgStore)
                {
                    Writer.WriteLine($"{Item.Key}:{Item.Value}");
                    Writer.Flush(); ;
                }
            }
        }

        public void AddOrSetItem(string Key, object Value)
        {
            if (cfgStore.ContainsKey(Key))
            {
                cfgStore[Key] = Value;
            }
            else
            {
                _PropertiesStore.Add(Key);
                cfgStore.Add(Key, Value);
            }
        }
    }
}