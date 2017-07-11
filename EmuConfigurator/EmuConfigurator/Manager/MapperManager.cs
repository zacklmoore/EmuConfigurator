using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator.Manager
{
    static class MapperManager
    {
        public static RomProfileMapper loadMapper(String id)
        {
            String error = "";

            String mapPath = getRomProfileMapperPath(id);

            if (mapPath != null)
            {
                if (System.IO.File.Exists(mapPath))
                {
                    RomProfileMapper returnMap = Newtonsoft.Json.JsonConvert.DeserializeObject<RomProfileMapper>(System.IO.File.ReadAllText(mapPath));

                    if (returnMap != null)
                    {
                        return returnMap;
                    }
                    else
                    {
                        error = "Failed to parse RomProfileMapper.";
                    }
                }
                else
                {
                    error = "Failed to find or access RomProfileMapper in RomProfileMappers directory.";
                }
            }

            Console.WriteLine(error);

            return null;
        }

        public static bool RomProfileMapperExists(String id)
        {
            String mapPath = getRomProfileMapperPath(id);

            if (mapPath != null)
            {
                if (System.IO.File.Exists(mapPath))
                {
                    return true;
                }
            }

            return false;
        }

        private static string getRomProfileMapperPath(String id)
        {
            String emuDir = SettingManager.getSettingValue("romProfileMapperDirecotry");
            String error = "";

            if (emuDir != null)
            {
                if (System.IO.Directory.Exists(emuDir))
                {
                    try
                    {
                        String mapPath = System.IO.Path.Combine(emuDir, id + ".json");
                        return mapPath;
                    } catch(Exception e)
                    {
                        error = e.Message;
                    }
                }
                else
                {
                    error = "Failed to find or access RomProfileMappers directory.";
                }
            }
            else
            {
                error = "No value specified for RomProfileMappers direcotry";
            }

            Console.WriteLine(error);

            return null;
        }

        public static bool createRomProfileMapper(String id)
        {
            String mapPath = getRomProfileMapperPath(id);
            String error = "";
            bool returnVal = false;

            if (mapPath != null)
            {
                if (!System.IO.File.Exists(mapPath))
                {
                    RomProfileMapper newMap = new RomProfileMapper(true);
                    returnVal = saveRomProfileMapper(newMap, id);
                    return returnVal;
                }
                else
                {
                    error = "RomProfileMapper already exists with this id.";
                }
            }
            else
            {
                error = "";
            }

            System.Console.WriteLine(error);

            return returnVal;
        }

        public static bool saveRomProfileMapper(RomProfileMapper emu, String id)
        {
            String mapPath = getRomProfileMapperPath(id);

            if (mapPath != null)
            {
                String emuDirectoryWithSubDirs = mapPath.Replace('\\', '/').Substring(0, mapPath.LastIndexOf('/'));

                if (!System.IO.Directory.Exists(emuDirectoryWithSubDirs))
                {
                    System.IO.Directory.CreateDirectory(emuDirectoryWithSubDirs);
                }

                System.IO.StreamWriter emuFile = System.IO.File.CreateText(mapPath);
                emuFile.Write(Newtonsoft.Json.JsonConvert.SerializeObject(emu, Newtonsoft.Json.Formatting.Indented));
                emuFile.Dispose();
                return true;
            }

            return false;
        }

        public static void createRomProfileMapperDirectory()
        {
            String mapDir = SettingManager.getSettingValue("romProfileMapperDirecotry");

            if (mapDir != null)
            {
                if (!System.IO.Directory.Exists(mapDir))
                {
                    System.IO.Directory.CreateDirectory(mapDir);
                }
            }
        }
    }
}
