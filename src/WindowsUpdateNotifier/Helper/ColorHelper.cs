using System;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace WindowsUpdateNotifier
{
    public static class ColorHelper
    {
        public static Color GetWindowsThemeBackgroundColor()
        {
            var colourset = GetImmersiveUserColorSetPreference(false, false);
            var pElementName = Marshal.StringToHGlobalUni("ImmersiveStartBackground");
            Marshal.FreeCoTaskMem(pElementName);

            var type = GetImmersiveColorTypeFromName(pElementName);
            var colourdword = GetImmersiveColorFromColorSetEx((uint)colourset, type, false, 0);

            var colourbytes = new byte[4];
            colourbytes[0] = (byte)((0xFF000000 & colourdword) >> 24); // A
            colourbytes[1] = (byte)((0x00FF0000 & colourdword) >> 16); // B
            colourbytes[2] = (byte)((0x0000FF00 & colourdword) >> 8); // G
            colourbytes[3] = (byte)(0x000000FF & colourdword); // R

            return Color.FromArgb(colourbytes[0], colourbytes[3], colourbytes[2], colourbytes[1]);
        }

        /// <summary>
        /// Retrieves an immersive colour from the specified colour set.
        /// </summary>
        /// <param name="dwImmersiveColorSet">Colour set index.</param>
        /// <param name="dwImmersiveColorType">The colour type. Use <see cref="GetImmersiveColorTypeFromName"/> to get the type from an element name.</param>
        /// <param name="bIgnoreHighContrast">Set this to true to return colours from the current colour set, even if a high contrast theme is being used.</param>
        /// <param name="dwHighContrastCacheMode">Set this to 1 to force UxTheme to check whether high contrast mode is enabled. If this is set to 0, UxTheme will only perform this check if high contrast mode is currently disabled.</param>
        /// <returns>Returns a colour (0xAABBGGRR).</returns>
        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        private static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);

        /// <summary>
        /// Gets the user's colour set preference (or default colour set if the user isn't allowed to modify this setting according to group policy).
        /// </summary>
        /// <param name="bForceCheckRegistry">Forces update from registry (HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Accent\ColorSet_Version3).</param>
        /// <param name="bSkipCheckOnFail">Skip second check if first result is -1.</param>
        /// <returns>User colour set preference.</returns>
        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        private static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

        /// <summary>
        /// Retrieves an immersive colour type given its name.
        /// </summary>
        /// <param name="pName">Pointer to a string containing the name preprended with 9 characters (e.g. "Immersive" + name).</param>
        /// <returns>Colour type.</returns>
        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        private static extern uint GetImmersiveColorTypeFromName(IntPtr pName); 
    }
}