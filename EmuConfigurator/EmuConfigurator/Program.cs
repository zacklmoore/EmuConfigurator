using EmuConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EmuConfigurator
{
    class Program
    {
        static Profile loadedProfile;
        static Emulator loadedEmulator;
        static LaunchProfile loadedLaunchProf = new LaunchProfile();
        static Launcher launcher;
        static bool doLaunch = false;

        static void Main(string[] args)
        {
            //Setup Launch Options
            LaunchOptions.resetOptions();

            //Load Settings
            Manager.SettingManager.loadSettingsFile();

            //Check Directories
            Manager.ProfileManager.createProfileDirectory();
            Manager.EmulatorManager.createEmulatorDirectory();
            Manager.MapperManager.createRomProfileMapperDirectory();

            //Process Launch Options
            if(args.Length == 0)
            {
                System.Console.WriteLine("No options provided. Run with --help for help.");
            } else
            {
                LaunchOptions.processOptions(args);
            }

            //Handle Launch Options
            handleOptions();
                        
            //Load Profile
            loadProfile();

            //Load Emulator
            loadEmulator();

            //Build Launcher
            if (loadedProfile != null && loadedEmulator != null)
            {
                launcher = new Launcher(loadedProfile, loadedEmulator, loadedLaunchProf.RomPath);
            }

            //Execute
            if(launcher != null)
            {
                launcher.launch();
            } else if(doLaunch)
            {
                //Pause to show error
                Console.WriteLine("An error occurred. Press any key to continue.");
                Console.WriteLine("DEBUG. WORKING DIR: " + System.IO.Directory.GetCurrentDirectory());
                Console.ReadKey();
            }

            //Wait for input
            /*
            if(launcher == null)
            {
                Console.WriteLine("Press any key to continue. ALSO REMOVE THIS AFTER DEVELOPMENT!!!!!!!!");
                Console.ReadKey();
            }
            */
        }

        private static void handleOptions()
        {
            if (LaunchOptions.getOptionValue(LaunchOptions.Option.HELP) == true)
            {
                System.Console.Write("\n\n" + LaunchOptions.printHelp() + "\n");

                return;
            }
            else if (LaunchOptions.getOptionValue(LaunchOptions.Option.JSON) != null)
            {
                String status = loadJson();
                doLaunch = true;
            }
            else if (LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE) != null)
            {
                loadedLaunchProf.ProfileId = LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE);
                doLaunch = true;
            }
            else if (LaunchOptions.getOptionValue(LaunchOptions.Option.ROM) != null)
            {
                loadedLaunchProf.RomPath = LaunchOptions.getOptionValue(LaunchOptions.Option.ROM);
                doLaunch = true;
            }
            else if (LaunchOptions.getOptionValue(LaunchOptions.Option.CREATE) != null)
            {
                String status = handleCreate();
            }
            else if (LaunchOptions.getOptionValue(LaunchOptions.Option.MAPGEN) != null)
            {
                String status = handleMapGen();
            }
        }

        private static String loadJson()
        {
            String pathString = LaunchOptions.getOptionValue(LaunchOptions.Option.JSON);

            if (System.IO.File.Exists(pathString))
            {
                loadedLaunchProf = Newtonsoft.Json.JsonConvert.DeserializeObject<LaunchProfile>(System.IO.File.ReadAllText(pathString));
            } else
            {
                return @"Unable to open file: " + pathString;
            }

            return null;
        }

        private static String loadProfile()
        {
            if (loadedLaunchProf.ProfileId != null)
            {
                String profileId = loadedLaunchProf.ProfileId.Replace("\"", "").Replace("\\", "/");
                loadedProfile = Manager.ProfileManager.loadProfile(profileId);
            }
            
            return null;
        }

        private static String loadEmulator()
        {
            if (loadedProfile != null)
            {
                String emuId = loadedProfile.EmulatorId.Replace("\"", "").Replace("\\", "/");
                loadedEmulator = Manager.EmulatorManager.loadEmulator(emuId);
            }

            return null;
        }

        private static String handleCreate()
        {
            String createString = LaunchOptions.getOptionValue(LaunchOptions.Option.CREATE);

            if(createString.ToLower().CompareTo("profile") == 0)
            {
                String profName = "";
                do
                {
                    System.Console.WriteLine("Enter New Profile ID: ");
                    profName = System.Console.ReadLine();

                    while (!IsValidFilename(profName))
                    {
                        System.Console.WriteLine("\nInvalid Profile ID.\n\nEnter New Profile ID: ");
                        profName = System.Console.ReadLine().Replace("\"", "").Replace("\\", "/");
                    }
                } while (!Manager.ProfileManager.createProfile(profName));
            }

            if (createString.ToLower().CompareTo("emulator") == 0)
            {
                String emuName = "";
                do
                {
                    System.Console.WriteLine("Enter New Emulator ID: ");
                    emuName = System.Console.ReadLine();

                    while (!IsValidFilename(emuName))
                    {
                        System.Console.WriteLine("\nInvalid Emulator ID.\n\nEnter New Emulator ID: ");
                        emuName = System.Console.ReadLine().Replace("\"", "").Replace("\\", "/");
                    }
                } while (!Manager.EmulatorManager.createEmulator(emuName));
            }

            if (createString.ToLower().CompareTo("mapper") == 0)
            {
                String mapName = "";
                do
                {
                    System.Console.WriteLine("Enter New Mapper ID: ");
                    mapName = System.Console.ReadLine();

                    while (!IsValidFilename(mapName))
                    {
                        System.Console.WriteLine("\nInvalid Mapper ID.\n\nEnter New Mapper ID: ");
                        mapName = System.Console.ReadLine().Replace("\"", "").Replace("\\", "/");
                    }
                } while (!Manager.MapperManager.createRomProfileMapper(mapName));
            }

            return "";
        }

        private static string handleMapGen()
        {
            String mapId = LaunchOptions.getOptionValue(LaunchOptions.Option.MAPGEN).Replace("\"", "").Replace("\\", "/");
            List<string> mapperIdList = new List<string>();
            List<RomProfileMapper> mapperList = new List<RomProfileMapper>();

            if(mapId != null)
            {
                if (mapId.Trim() == "*")
                {
                    mapperIdList.AddRange(System.IO.Directory.GetFiles(Manager.SettingManager.getSettingValue("romProfileMapperDirecotry")));
                }
                else
                {
                    mapperIdList.Add(mapId);
                }

                if (mapperIdList.Count > 0)
                {
                    foreach (String id in mapperIdList)
                    {
                        mapperList.Add(Manager.MapperManager.loadMapper(id));
                    }

                    if(mapperList.Count > 0)
                    {
                        foreach(RomProfileMapper map in mapperList)
                        {
                            map.generateMapFiles();
                        }
                    }
                }
                    
            }

            return "";
        }
        
        private static bool IsValidFilename(string testName)
        {
            Regex containsABadCharacter = new Regex("["
                  + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(testName)) { return false; };
            
            return true;
        }

    }
}