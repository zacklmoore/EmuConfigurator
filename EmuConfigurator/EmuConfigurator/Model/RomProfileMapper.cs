using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator { 
    class RomProfileMapper
    {
        public class RomProfileMap
        {
            public RomProfileMap(string p, string o, bool e, bool d, bool s, List<String> r)
            {
                profileId = p;
                outputDir = o;
                retainExtension = e;
                romsAreDirectories = d;
                recursiveSubDirectories = s;
                romPaths = r;
            }

            public string profileId;
            public string outputDir;
            public bool retainExtension;
            public bool romsAreDirectories;
            public bool recursiveSubDirectories;
            public List<string> romPaths;
        };

        private List<RomProfileMap> mappings;

        public List<RomProfileMap> Mappings { get => mappings; set => mappings = value; }

        public RomProfileMapper()
        {
            
        }

        //Constructor for creating new instances. Value of isNew does not actually matter
        public RomProfileMapper(bool isNew)
        {
            Mappings = new List<RomProfileMap>();
            List<string> exampleMap = new List<string>();
            exampleMap.Add("C:/example/*.rom");
            exampleMap.Add("C:/example 2/example 2.zip");
            Mappings.Add(new RomProfileMap("example", "C:/example_output/", false, false, false, exampleMap));
        }

        public bool generateMapFiles()
        {
            foreach(RomProfileMap map in Mappings)
            {
                //1. Create Output Directory if it doesn't exist
                if (!System.IO.Directory.Exists(map.outputDir))
                {
                    System.IO.Directory.CreateDirectory(map.outputDir);
                }

                //2. Loop through map inputs and get files
                List<string> fileNames = new List<string>();
                foreach(string romPath in map.romPaths)
                {
                    string directory = System.IO.Path.GetDirectoryName(romPath);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(romPath);
                    string fileNameWithExtension = System.IO.Path.GetFileName(romPath);
                    
                    if (System.IO.Directory.Exists(directory))
                    {
                        System.IO.SearchOption option = System.IO.SearchOption.AllDirectories;
                        if (fileName == "*" && map.recursiveSubDirectories)
                        {
                            option = System.IO.SearchOption.AllDirectories;
                        }

                        fileNames.AddRange(System.IO.Directory.GetFiles(directory, fileNameWithExtension, option));
                    }
                }

                //3. Generate Output Files
                foreach(string file in fileNames)
                {
                    //Output Path
                    string extension = System.IO.Path.GetExtension(file);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                    string outFile = fileName + (map.retainExtension ? extension : extension + ".ecmap");

                    //Output Profile
                    LaunchProfile romProfile = new LaunchProfile();
                    romProfile.ProfileId = map.profileId;
                    romProfile.RomPath = file;

                    //Write File
                    System.IO.StreamWriter mapFile = System.IO.File.CreateText(System.IO.Path.Combine(map.outputDir, outFile));
                    mapFile.Write(Newtonsoft.Json.JsonConvert.SerializeObject(romProfile, Newtonsoft.Json.Formatting.Indented));
                    mapFile.Dispose();
                }
            }
            
            return false;
        }
    }
}
