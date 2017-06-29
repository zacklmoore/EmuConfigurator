using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator.Manager
{
    static class SettingManager
    {
        private static String SETTINGS_FILE_PATH = "./settings.json";

        private static Dictionary<String, String> settings = new Dictionary<String, String>();

        private static void resetSettings()
        {
            settings.Clear();

            settings.Add("profileDirectory", "./profiles/");
            settings.Add("emulatorDirectory", "./emulators/");
        }

        public static void loadSettingsFile()
        {
            resetSettings();

            if (System.IO.File.Exists(SETTINGS_FILE_PATH))
            {
                Dictionary<String, String> loaded = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<String, String> >(System.IO.File.ReadAllText(SETTINGS_FILE_PATH));

                foreach(String key in loaded.Keys)
                {
                    settings[key] = loaded[key];
                }
            } else
            {
                System.IO.StreamWriter settingFile = System.IO.File.CreateText(SETTINGS_FILE_PATH);
                settingFile.Write(Newtonsoft.Json.JsonConvert.SerializeObject(settings));
            }
        }

        public static String getSettingValue(String key)
        {
            String result = null;

            settings.TryGetValue(key, out result);

            return result;
        }
    }
}
