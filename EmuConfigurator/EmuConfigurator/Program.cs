using System;

namespace EmuConfigurator
{
    class Program
    {
        static Profile loadedProfile;
        static LaunchProfile loadedLauncher;
        static String romPath;

        static void Main(string[] args)
        {
            //Setup Launch Options
            LaunchOptions.resetOptions();

            //Load Settings
            Manager.SettingManager.loadSettingsFile();

            //Process Launch Options
            if(args.Length == 0)
            {
                System.Console.WriteLine("No profile or options provided. Run with --help for help.");
            } else
            {
                LaunchOptions.processOptions(args);
            }

            //Handle Launch Options
            handleOptions();

            //Wait for input
            Console.WriteLine("Press any key to continue. ALSO REMOVE THIS AFTER DEVELOPMENT!!!!!!!!");
            Console.ReadKey();
        }

        private static void handleOptions()
        {
            if(LaunchOptions.getOptionValue(LaunchOptions.Option.HELP) == true)
            {
                System.Console.Write("\n\n" + LaunchOptions.printHelp() + "\n");

                return;
            } else if (LaunchOptions.getOptionValue(LaunchOptions.Option.JSON) != null)
            {
                String status = handleJson();

                if (status != null)
                {
                    System.Console.WriteLine("Error when loading JSON file. Error: " + status);
                }
            } else if (LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE) != null)
            {
                String status = handleProfile();

                if(status != null)
                {
                    System.Console.WriteLine("Error when executing profile. Error: " + status);
                }
            }
        }

        private static String handleJson()
        {
            String pathString = LaunchOptions.getOptionValue(LaunchOptions.Option.JSON);

            if (System.IO.File.Exists(pathString))
            {
                loadedLauncher = Newtonsoft.Json.JsonConvert.DeserializeObject<LaunchProfile>(System.IO.File.ReadAllText(pathString));
            } else
            {
                return @"Unable to open file: " + pathString;
            }

            return null;
        }

        private static String handleProfile()
        {
            if (LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE) != null)
            {
                loadedProfile = Manager.ProfileManager.loadProfile(LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE));
            } else if(loadedLauncher != null)
            {
                loadedProfile = Manager.ProfileManager.loadProfile(loadedLauncher.ProfileId);
            }
            
            return null;
        }
    }
}