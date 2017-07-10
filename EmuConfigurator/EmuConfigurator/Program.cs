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
        static Launcher launcher;
        static LaunchProfile loadedLaunchProf;
        static String romPath;

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
                System.Console.WriteLine("No profile or options provided. Run with --help for help.");
            } else
            {
                LaunchOptions.processOptions(args);
            }

            //Handle Launch Options
            handleOptions();

            //Eexecute
            if(loadedProfile != null && loadedEmulator != null)
            {
                launcher = new Launcher(loadedProfile, loadedEmulator, romPath);

                launcher.launch();
            }
            
            //Wait for input
            if(launcher == null)
            {
                Console.WriteLine("Press any key to continue. ALSO REMOVE THIS AFTER DEVELOPMENT!!!!!!!!");
                Console.ReadKey();
            }
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
            } else if (LaunchOptions.getOptionValue(LaunchOptions.Option.CREATE) != null)
            {
                String status = handleCreate();
            } else if (LaunchOptions.getOptionValue(LaunchOptions.Option.MAPGEN) != null)
            {
                String status = handleMapGen();
            }
        }

        private static String handleJson()
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

        private static String handleProfile()
        {
            if (LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE) != null)
            {
                String profileId = LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE);
                profileId.Replace("\"", "");
                loadedProfile = Manager.ProfileManager.loadProfile(LaunchOptions.getOptionValue(LaunchOptions.Option.PROFILE));
            } else if(loadedLaunchProf != null)
            {
                String profileId = loadedLaunchProf.ProfileId;
                profileId.Replace("\"", "");
                loadedProfile = Manager.ProfileManager.loadProfile(loadedLaunchProf.ProfileId);
            }

            if(loadedProfile != null)
            {
                String emuId = loadedProfile.EmulatorId.Replace("\"", "");
                loadedEmulator = Manager.EmulatorManager.loadEmulator(loadedProfile.EmulatorId);
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
                        profName = System.Console.ReadLine();
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
                        emuName = System.Console.ReadLine();
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
                        mapName = System.Console.ReadLine();
                    }
                } while (!Manager.MapperManager.createRomProfileMapper(mapName));
            }

            return "";
        }

        private static string handleMapGen()
        {
            String mapId = LaunchOptions.getOptionValue(LaunchOptions.Option.MAPGEN);
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