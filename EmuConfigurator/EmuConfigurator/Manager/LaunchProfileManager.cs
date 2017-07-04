using EmuConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator.Manager
{
    class LaunchProfileManager
    {
        public static LaunchProfile loadProfile(String path)
        {
            String error = "";

            if (path != null)
            {
                if (System.IO.File.Exists(path))
                {
                    LaunchProfile returnLauncher = Newtonsoft.Json.JsonConvert.DeserializeObject<LaunchProfile>(System.IO.File.ReadAllText(path));

                    if (returnLauncher != null)
                    {
                        return returnLauncher;
                    }
                    else
                    {
                        error = "Failed to parse profile.";
                    }
                }
                else
                {
                    error = "Failed to find or access profile in profiles directory.";
                }
            }

            Console.WriteLine(error);

            return null;
        }

        public static bool validateProfile(LaunchProfile prof)
        {
            if (!ProfileManager.profileExists(prof.ProfileId))
            {
                return false;
            }

            return true;
        }
    }
}
