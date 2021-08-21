using System.IO;

namespace NetEaseController
{
    public class KeyConfigs : GenericConfigurations
    {

        public void FirstInitialize()
        {
            AddOrSetItem("VB_Mute", "VOLUME_MUTE");
            AddOrSetItem("VB_OutToggle", "Ctrl+MEDIA_STOP");
            AddOrSetItem("VB_VolUp", "VOLUME_UP");
            AddOrSetItem("VB_VolDown", "VOLUME_DOWN");
            AddOrSetItem("PlayPause", "Ctrl+F1");
            AddOrSetItem("PrevSong", "Ctrl+F2");
            AddOrSetItem("NextSong", "Ctrl+F3");
            AddOrSetItem("VolUp", "Ctrl+F4");
            AddOrSetItem("VolDown", "Ctrl+F5");
            AddOrSetItem("LikeMusic", "Ctrl+F8");
            Save();
        }

        public KeyConfigs()
        {
            ConfigurationFile = "keys.cfg";
            if (!File.Exists("keys.cfg"))
            {
                FirstInitialize();
            }
            Load();
        }
    }
}