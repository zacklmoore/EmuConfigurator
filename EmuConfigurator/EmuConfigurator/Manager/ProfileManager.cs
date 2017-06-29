using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator.Manager
{
    static class ProfileManager
    {
        public static Profile loadProfile(String id)
        {
            String profileDir = SettingManager.getSettingValue("profileDirectory");
            String error = "";

            String profilePath = getProfilePath(id);

            if(profilePath != null)
            {
                if (System.IO.File.Exists(profilePath))
                {
                    Profile returnProf = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(System.IO.File.ReadAllText(profilePath));

                    if (returnProf != null)
                    {
                        return returnProf;
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

        public static bool profileExists(String id)
        {
            String profilePath = getProfilePath(id);

            if (profilePath != null)
            {
                if (System.IO.File.Exists(profilePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static string getProfilePath(String id)
        {
            String profileDir = SettingManager.getSettingValue("profileDirectory");
            String error = "";

            if (profileDir != null)
            {
                if (System.IO.Directory.Exists(profileDir))
                {
                    String profilePath = System.IO.Path.Combine(profileDir, id + ".json");
                    return profilePath;
                }
                else
                {
                    error = "Failed to find or access profiles directory.";
                }
            }
            else
            {
                error = "No value specified for profile direcotry";
            }

            Console.WriteLine(error);

            return null;
        }
    }
}
