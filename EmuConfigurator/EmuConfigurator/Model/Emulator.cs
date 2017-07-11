using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator
{
    class Emulator
    {
        private string launchCommand;
        private Dictionary<String, String> launchProps;

        public string LaunchCommand { get => launchCommand; set => launchCommand = value; }
        public Dictionary<String, String> LaunchProps { get => launchProps; set => launchProps = value; }

        public Emulator()
        {
            
        }

        public Emulator(bool isNew)
        {
            launchCommand = "<insert launch command here>";
            launchProps = new Dictionary<string, string>();
            launchProps.Add("-example", "%ROM%");
        }

        public Emulator applyProfile(Profile profile)
        {
            //Apply settings overrides
            if(profile != null)
            {
                foreach(KeyValuePair<String, String> prop in profile.SettingOverrides)
                {
                    launchProps[prop.Key] = prop.Value;
                }
            }

            return this;
        }

        public Emulator setRomFile(String romFile)
        {
            //Add rom file to launchCommand
            Dictionary<String, String> changeProps = new Dictionary<string, string>();

            if (launchProps.ContainsValue("%ROM%"))
            {
                foreach(KeyValuePair<String, String> entry in launchProps)
                {
                    if(entry.Value.CompareTo("%ROM%") == 0)
                    {
                        changeProps[entry.Key] = romFile;
                    }
                }

                foreach(KeyValuePair<String, String> entry in changeProps)
                {
                    launchProps[entry.Key] = entry.Value;
                }
            }

            return this;
        }

        public string getLaunchPropsString()
        {
            String returnString = "";

            foreach (KeyValuePair<String, String> entry in launchProps)
            {
                if(entry.Key != null && entry.Value != null)
                {
                    returnString += entry.Key.Trim() + " " + entry.Value.Trim() + " ";
                }
            }

            return returnString.TrimEnd();
        }
    }
}
