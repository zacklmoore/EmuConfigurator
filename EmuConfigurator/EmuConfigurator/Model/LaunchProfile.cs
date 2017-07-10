using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator
{
    class LaunchProfile
    {
        private string profileId;
        private string romPath;

        public string ProfileId { get => profileId; set => profileId = value; }
        public string RomPath { get => romPath; set => romPath = value; }
    }
}
