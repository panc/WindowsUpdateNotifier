using System;
using System.Linq;

namespace WindowsUpdateNotifier
{
    public class CommandLineHelper
    {
        public CommandLineHelper()
            : this(Environment.GetCommandLineArgs())
        {
        }

        public CommandLineHelper(string[] args)
        {
            UseDefaultSettings = args.Any(x => _CheckSwitch(x, "-defaultSettings", "-d"));
            CloseAfterCheck = args.Any(x => _CheckSwitch(x, "-closeAfterCheck", "-c"));
            SettingsFile = "";
            
            var settingsFile = args.FirstOrDefault(x => _CheckArgument(x, "-settingsfile:", "-s:"));
            if (settingsFile != null)
            {
                SettingsFile = settingsFile.ToLower().StartsWith("-s:")
                    ? settingsFile.Substring(3)
                    : settingsFile.Substring(14);
            }
            
            if (UseDefaultSettings && !string.IsNullOrEmpty(SettingsFile))
            {
                Console.WriteLine("'defaultSettings' and 'settingsfile' can not be used together!");
                throw new NotSupportedException();
            }
        }

        public string SettingsFile { get; private set; }

        public bool UseDefaultSettings { get; private set; }

        public bool CloseAfterCheck { get; private set; }

        private bool _CheckSwitch(string argument, string longValue, string shortValue)
        {
            var arg = argument.ToLower();
            return arg == longValue.ToLower() || arg == shortValue.ToLower();
        }

        private bool _CheckArgument(string argument, string longValue, string shortValue)
        {
            var arg = argument.ToLower();
            return arg.StartsWith(longValue.ToLower()) || arg.StartsWith(shortValue.ToLower());
        }
    }
}