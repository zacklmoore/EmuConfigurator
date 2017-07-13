using System;
using System.Collections.Generic;
using System.Text;

namespace EmuConfigurator
{
    public static class LaunchOptions
    {
        private static List<LaunchOption> options = new List<LaunchOption>();

        public enum Option
        {
            HELP = 0,
            PROFILE = 1,
            ROM = 2,
            JSON = 3,
            CREATE = 4,
            MAPGEN = 5
        };

        public static dynamic getOptionValue(Option option)
        {
            return (options[(int)option].value);
        }

        private class LaunchOption
        {
            public LaunchOption(string c, string h, Type t)
            {
                command = c;
                helpString = h;
                type = t;
            }

            public string command;
            public string helpString;
            public dynamic value;
            public Type type;
        }

        public static String processOptions(String[] args)
        {
            List<String> argList = new List<String>(args);
            String returnString = "";

            foreach(LaunchOption option in options)
            {
                int argIndex = argList.IndexOf("--" + option.command);

                if(argIndex >= 0)
                {
                    if (option.type != null)
                    {
                        //Arg with value
                        if(argIndex < args.Length - 1)
                        {
                            String argValue = args[argIndex + 1];

                            argValue = argValue.Trim();

                            if (argValue.StartsWith("--"))
                            {
                                return ("Error: Value for argument: '--" + option.command + "' is missing.");
                            }

                            try
                            {
                                option.value = Convert.ChangeType(argValue, option.type);
                            } catch(InvalidCastException e)
                            {
                                return ("Error: Value for argument: '--" + option.command + "' cannot be converted to type '" + option.type.FullName + "'." + "\n\n" + e.Message);
                            }
                        }
                    } else
                    {
                        //Arg without value
                        option.value = true;
                    }
                }
            }

            return (returnString);
        }

        public static string printHelp()
        {
            String helpString = "Launch Options:\n\n";
            int counter = 1;

            foreach(LaunchOption option in options)
            {
                helpString += counter + ". --" + option.command + ": " + option.helpString + "\n";
                counter++;
            }

            return helpString;
        }

        public static string debugPrint()
        {
            String debugString = "Launch Option Values (DEBUG):\n\n";
            int counter = 1;

            foreach (LaunchOption option in options)
            {
                debugString += counter + ". --" + option.command + ": " + option.value + " (" + option.type + ")\n";
                counter++;
            }

            return debugString;
        }

        public static void resetOptions()
        {
            options.Clear();

            options.Add(new LaunchOption(
                "help",
                "Shows this page.",
                null));

            options.Add(new LaunchOption(
                "profile",
                "The profile to run.",
                typeof(String)));

            options.Add(new LaunchOption(
                "rom",
                "The rom file to launch.",
                typeof(String)));

            options.Add(new LaunchOption(
               "json",
               "The json file to load options from.",
               typeof(String)));

            options.Add(new LaunchOption(
                "create",
                "The type of file to create. Options (case-insensitive): Emulator, Profile, Mapper",
                typeof(String)));

            options.Add(new LaunchOption(
                "mapgen",
                "Generates rom-profile map files using the specified mapper id. Use id * to generate map files for all mappers.",
                typeof(String)));
        }
    }
}
