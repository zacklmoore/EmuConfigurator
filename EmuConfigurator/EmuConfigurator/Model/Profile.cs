using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator
{
    class Profile
    {
        private string emulatorId;
       // private string controlId;
        private Dictionary<String, String> settingOverrides;

        public string EmulatorId { get => emulatorId; set => emulatorId = value; }
      //  public string ControlId { get => controlId; set => controlId = value; }
        public Dictionary<string, string> SettingOverrides { get => settingOverrides; set => settingOverrides = value; }

        public Profile()
        {
            emulatorId = "<insert emulator ID here>";
           // controlId = "<NOT YET IMPLEMENTED>";
            SettingOverrides = new Dictionary<String, String>();
            SettingOverrides.Add("-Example", "value");
        }
    }
}
