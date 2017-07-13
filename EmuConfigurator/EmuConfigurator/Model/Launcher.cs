using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator.Model
{
    class Launcher
    {
        private Process launchProcess;
        private ProcessStartInfo processInfo;

        public Launcher(Profile profile, Emulator emulator, string romFile)
        {
            Emulator launchEmu = emulator.applyProfile(profile).setRomFile(romFile);

            processInfo = new ProcessStartInfo(launchEmu.LaunchCommand, launchEmu.getLaunchPropsString());
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";
        }

        public string launch()
        {
            try
            {
                launchProcess = Process.Start(processInfo);
                launchProcess.WaitForExit();
                return null;
            } catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
