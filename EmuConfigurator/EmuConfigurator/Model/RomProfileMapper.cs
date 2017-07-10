using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator { 
    class RomProfileMapper
    {
        public class RomProfileMap
        {
            public RomProfileMap(string p, string o, bool e, bool d, List<String> r)
            {
                profileId = p;
                outputDir = o;
                retainExtension = e;
                romsAreDirectories = d;
                romPaths = r;
            }

            public string profileId;
            public string outputDir;
            public bool retainExtension;
            public bool romsAreDirectories;
            public List<string> romPaths;
        };

        private List<RomProfileMap> mappings;

        public List<RomProfileMap> Mappings { get => mappings; set => mappings = value; }

        public RomProfileMapper()
        {
            Mappings = new List<RomProfileMap>();
            List<string> exampleMap = new List<string>();
            exampleMap.Add("C:/example/*.rom");
            exampleMap.Add("C:/example 2/example 2.zip");
            Mappings.Add(new RomProfileMap("example", "C:/example_output/", false, false, exampleMap));
        }

        public bool generateMapFiles()
        {
            return false;
        }
    }
}
