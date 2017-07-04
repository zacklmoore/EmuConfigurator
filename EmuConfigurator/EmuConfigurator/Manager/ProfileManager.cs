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

        public static bool createProfile(String id)
        {
            String profPath = getProfilePath(id);
            String error = "";
            bool returnVal = false;

            if (profPath != null)
            {
                if (!System.IO.File.Exists(profPath))
                {
                    Profile newProf = new Profile();
                    returnVal = saveProfile(newProf, id);
                    return returnVal;
                }
                else
                {
                    error = "Profile already exists with this id.";
                }
            }
            else
            {
                error = "";
            }

            System.Console.WriteLine(error);

            return returnVal;
        }

        public static bool saveProfile(Profile prof, String id)
        {
            String profPath = getProfilePath(id);

            if (profPath != null)
            {
                System.IO.StreamWriter profFile = System.IO.File.CreateText(profPath);
                profFile.Write(Newtonsoft.Json.JsonConvert.SerializeObject(prof, Newtonsoft.Json.Formatting.Indented));
                profFile.Dispose();
                return true;
            }

            return false;
        }

        public static void createProfileDirectory()
        {
            String profileDir = SettingManager.getSettingValue("profileDirectory");

            if (profileDir != null)
            {
                if (!System.IO.Directory.Exists(profileDir))
                {
                    System.IO.Directory.CreateDirectory(profileDir);
                }
            }
        }
    }
}
