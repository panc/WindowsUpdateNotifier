using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WindowsUpdateNotifier
{
    /// <summary>
    /// Enables extraction of icons for any file type from
    /// the Shell.
    /// </summary>
    public class FileIcon
    {

        #region UnmanagedCode
        private const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32")]
        private static extern int SHGetFileInfo(
           string pszPath,
           int dwFileAttributes,
           ref SHFILEINFO psfi,
           uint cbFileInfo,
           uint uFlags);

        #endregion

        #region Enumerations
        [Flags]
        public enum SHGetFileInfoConstants : int
        {
            SHGFI_ICON = 0x100,                // get icon 
            SHGFI_DISPLAYNAME = 0x200,         // get display name 
            SHGFI_TYPENAME = 0x400,            // get type name 
            SHGFI_ATTRIBUTES = 0x800,          // get attributes 
            SHGFI_ICONLOCATION = 0x1000,       // get icon location 
            SHGFI_EXETYPE = 0x2000,            // return exe type 
            SHGFI_SYSICONINDEX = 0x4000,       // get system icon index 
            SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon 
            SHGFI_SELECTED = 0x10000,          // show icon in selected state 
            SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes 
            SHGFI_LARGEICON = 0x0,             // get large icon 
            SHGFI_SMALLICON = 0x1,             // get small icon 
            SHGFI_OPENICON = 0x2,              // get open icon 
            SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon 
            SHGFI_USEFILEATTRIBUTES = 0x10,     // use passed dwFileAttribute 
            SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040     // Get the index of the overlay
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets/sets the flags used to extract the icon
        /// </summary>
        public SHGetFileInfoConstants Flags { get; set; }

        /// <summary>
        /// Gets/sets the filename to get the icon for
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the icon for the chosen file
        /// </summary>
        public Icon ShellIcon { get; private set; }

        /// <summary>
        /// Gets the display name for the selected file
        /// if the SHGFI_DISPLAYNAME flag was set.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the type name for the selected file
        /// if the SHGFI_TYPENAME flag was set.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        ///  Gets the information for the specified 
        ///  file name and flags.
        /// </summary>
        public void GetInfo()
        {
            ShellIcon = null;
            TypeName = "";
            DisplayName = "";

            var shfi = new SHFILEINFO();
            var shfiSize = (uint)Marshal.SizeOf(shfi.GetType());

            var ret = SHGetFileInfo(
               FileName, 0, ref shfi, shfiSize, (uint)(Flags));
            if (ret != 0)
            {
                if (shfi.hIcon != IntPtr.Zero)
                    ShellIcon = Icon.FromHandle(shfi.hIcon);

                TypeName = shfi.szTypeName;
                DisplayName = shfi.szDisplayName;
            }
//            else
//            {
//                do not throw an exception
//            }
        }

        /// <summary>
        /// Constructs a new, default instance of the FileIcon
        /// class.  Specify the filename and call GetInfo()
        /// to retrieve an icon.
        /// </summary>
        public FileIcon()
        {
            Flags = SHGetFileInfoConstants.SHGFI_ICON |
               SHGetFileInfoConstants.SHGFI_DISPLAYNAME |
               SHGetFileInfoConstants.SHGFI_TYPENAME |
               SHGetFileInfoConstants.SHGFI_ATTRIBUTES |
               SHGetFileInfoConstants.SHGFI_EXETYPE;
        }

        /// <summary>
        /// Constructs a new instance of the FileIcon class
        /// and retrieves the icon, display name and type name
        /// for the specified file.      
        /// </summary>
        /// <param name="fileName">The filename to get the icon, 
        /// display name and type name for</param>
        public FileIcon(string fileName)
            : this()
        {
            FileName = fileName;
            GetInfo();
        }

        /// <summary>
        /// Constructs a new instance of the FileIcon class
        /// and retrieves the information specified in the 
        /// flags.
        /// </summary>
        /// <param name="fileName">The filename to get information
        /// for</param>
        /// <param name="flags">The flags to use when extracting the
        /// icon and other shell information.</param>
        public FileIcon(string fileName, SHGetFileInfoConstants flags)
        {
            FileName = fileName;
            Flags = flags;
            GetInfo();
        }

        #endregion
    }
}