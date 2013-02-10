using System;
using System.Linq;

namespace WindowsUpdateNotifier
{
    public class CommandLineHelper
    {
        public CommandLineHelper()
        {
            var args = Environment.GetCommandLineArgs();

            UseDefaultSettings = args.Any(x => _CheckArgument(x, "-defaultSettings", "-d"));
            CloseAfterCheck = args.Any(x => _CheckArgument(x, "-closeAfterCheck", "-c"));
        }

        public bool UseDefaultSettings { get; private set; }

        public bool CloseAfterCheck { get; private set; }

        private bool _CheckArgument(string argument, string longValue, string shortValue)
        {
            var arg = argument.ToLower();
            return arg == longValue.ToLower() || arg == shortValue.ToLower();
        }
    }
}