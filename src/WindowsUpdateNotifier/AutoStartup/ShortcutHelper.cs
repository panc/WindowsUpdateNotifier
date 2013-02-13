using System;
using System.IO;
using System.Reflection;

namespace WindowsUpdateNotifier
{
    public static class ShortcutHelper
    {
        public static bool IsSetAsAutoStartup()
        {
            // Check the users startup folder for an existing shortcut.
            // Do not check the common startup folder for now as the user needs admin-rights to modifiy the folder...
            var path = _GetShortcutLocation(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            return string.IsNullOrWhiteSpace(path) == false;
        }

        public static void CreateStartupShortcut()
        {
            var link = new ShellLink { Target = Assembly.GetExecutingAssembly().Location };

            link.Save(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                Path.GetFileNameWithoutExtension(link.Target) + ".lnk"));
        }

        public static void DeleteStartupShortcut()
        {
            var path = _GetShortcutLocation(Environment.GetFolderPath(Environment.SpecialFolder.Startup));

            if (File.Exists(path))
                File.Delete(path);
        }

        private static string _GetShortcutLocation(string folder)
        {
            var exePath = Assembly.GetExecutingAssembly().Location;

            foreach (var file in Directory.GetFiles(folder))
            {
                if (file.EndsWith("lnk") == false)
                    continue;

                var link = new ShellLink(file);
                if (link.Target == exePath)
                    return link.ShortCutFile;
            }

            return string.Empty;
        }
    }
}