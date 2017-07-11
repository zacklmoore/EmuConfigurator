using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator.Manager
{
    static class EmulatorManager
    {
        public static Emulator loadEmulator(String id)
        {
            String error = "";

            String emuPath = getEmulatorPath(id);

            if (emuPath != null)
            {
                if (System.IO.File.Exists(emuPath))
                {
                    Emulator returnEmu = Newtonsoft.Json.JsonConvert.DeserializeObject<Emulator>(System.IO.File.ReadAllText(emuPath));

                    if (returnEmu != null)
                    {
                        return returnEmu;
                    }
                    else
                    {
                        error = "Failed to parse emulator.";
                    }
                }
                else
                {
                    error = "Failed to find or access emulator in emulators directory.";
                }
            }

            Console.WriteLine(error);

            return null;
        }

        public static bool emulatorExists(String id)
        {
            String emuPath = getEmulatorPath(id);

            if (emuPath != null)
            {
                if (System.IO.File.Exists(emuPath))
                {
                    return true;
                }
            }

            return false;
        }

        private static string getEmulatorPath(String id)
        {
            String emuDir = SettingManager.getSettingValue("emulatorDirectory");
            String error = "";

            if (emuDir != null)
            {
                if (System.IO.Directory.Exists(emuDir))
                {
                    try
                    {
                        String emuPath = System.IO.Path.Combine(emuDir, id + ".json");
                        return emuPath;
                    } catch(Exception e)
                    {
                        error = e.Message;
                    }
                }
                else
                {
                    error = "Failed to find or access emulators directory.";
                }
            }
            else
            {
                error = "No value specified for emulators direcotry";
            }

            Console.WriteLine(error);

            return null;
        }

        public static bool createEmulator(String id)
        {
            String emuPath = getEmulatorPath(id);
            String error = "";
            bool returnVal = false;

            if (emuPath != null)
            {
                if (!System.IO.File.Exists(emuPath))
                {
                    Emulator newEmu = new Emulator(true);
                    returnVal = saveEmulator(newEmu, id);
                    return returnVal;
                }
                else
                {
                    error = "Emulator already exists with this id.";
                }
            }
            else
            {
                error = "";
            }

            System.Console.WriteLine(error);

            return returnVal;
        }

        public static bool saveEmulator(Emulator emu, String id)
        {
            String emuPath = getEmulatorPath(id);

            if (emuPath != null)
            {
                String emuDirectoryWithSubDirs = emuPath.Replace('\\', '/').Substring(0, emuPath.LastIndexOf('/'));

                if (!System.IO.Directory.Exists(emuDirectoryWithSubDirs))
                {
                    System.IO.Directory.CreateDirectory(emuDirectoryWithSubDirs);
                }

                System.IO.StreamWriter emuFile = System.IO.File.CreateText(emuPath);
                emuFile.Write(Newtonsoft.Json.JsonConvert.SerializeObject(emu, Newtonsoft.Json.Formatting.Indented));
                emuFile.Dispose();
                return true;
            }

            return false;
        }

        public static void createEmulatorDirectory()
        {
            String emuDir = SettingManager.getSettingValue("emulatorDirectory");

            if (emuDir != null)
            {
                if (!System.IO.Directory.Exists(emuDir))
                {
                    System.IO.Directory.CreateDirectory(emuDir);
                }
            }
        }
    }
}
