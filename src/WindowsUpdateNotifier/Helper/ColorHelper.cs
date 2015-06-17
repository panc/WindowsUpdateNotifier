using System;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace WindowsUpdateNotifier
{
    public static class ColorHelper
    {
        private static readonly Color INVALID_COLOR = Color.FromArgb(255, 255, 0, 255);

        public static Color GetWindowsThemeBackgroundColor()
        {
            var color = _GetWindowsThemeBackgroundColor();

            // Somtimes it happens that the first attemt to get the systems theme color 
            // returns an invalid color (always RGBA(255, 0, 255, 255)).
            // If this happens we just try it again

            if (color == INVALID_COLOR)
                color = _GetWindowsThemeBackgroundColor();

            return color;
        }

        private static Color _GetWindowsThemeBackgroundColor()
        {
            if (UacHelper.IsRunningOnWindows7())
                return Color.FromArgb(255, 35, 38, 39);

            var colorSet = GetImmersiveUserColorSetPreference(false, false);
            var elementName = Marshal.StringToHGlobalUni("ImmersiveStartBackground");
            Marshal.FreeCoTaskMem(elementName);

            var type = GetImmersiveColorTypeFromName(elementName);
            var colorDword = GetImmersiveColorFromColorSetEx((uint) colorSet, type, false, 0);

            var colorBytes = new byte[4];
            colorBytes[0] = (byte) ((0xFF000000 & colorDword) >> 24); // A
            colorBytes[1] = (byte) ((0x00FF0000 & colorDword) >> 16); // B
            colorBytes[2] = (byte) ((0x0000FF00 & colorDword) >> 8); // G
            colorBytes[3] = (byte) ((0x000000FF & colorDword) >> 0); // R

            return Color.FromArgb(colorBytes[0], colorBytes[3], colorBytes[2], colorBytes[1]);
        }

        /// <summary>
        /// Retrieves an immersive colour from the specified colour set.
        /// </summary>
        /// <param name="immersiveColorSet">Colour set index.</param>
        /// <param name="immersiveColorType">The colour type. Use <see cref="GetImmersiveColorTypeFromName"/> to get the type from an element name.</param>
        /// <param name="ignoreHighContrast">Set this to true to return colours from the current colour set, even if a high contrast theme is being used.</param>
        /// <param name="highContrastCacheMode">Set this to 1 to force UxTheme to check whether high contrast mode is enabled. If this is set to 0, UxTheme will only perform this check if high contrast mode is currently disabled.</param>
        /// <returns>Returns a colour (0xAABBGGRR).</returns>
        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        private static extern uint GetImmersiveColorFromColorSetEx(uint immersiveColorSet, uint immersiveColorType, bool ignoreHighContrast, uint highContrastCacheMode);

        /// <summary>
        /// Gets the user's colour set preference (or default colour set if the user isn't allowed to modify this setting according to group policy).
        /// </summary>
        /// <param name="forceCheckRegistry">Forces update from registry (HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Accent\ColorSet_Version3).</param>
        /// <param name="skipCheckOnFail">Skip second check if first result is -1.</param>
        /// <returns>User colour set preference.</returns>
        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        private static extern int GetImmersiveUserColorSetPreference(bool forceCheckRegistry, bool skipCheckOnFail);

        /// <summary>
        /// Retrieves an immersive colour type given its name.
        /// </summary>
        /// <param name="name">Pointer to a string containing the name preprended with 9 characters (e.g. "Immersive" + name).</param>
        /// <returns>Colour type.</returns>
        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        private static extern uint GetImmersiveColorTypeFromName(IntPtr name); 
    }
}