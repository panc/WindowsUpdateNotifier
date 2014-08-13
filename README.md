# Windows Update Notifier

Windows 8 (and Windows 8.1) do not offer a desktop notification about new available Windows Updates. You can choose the option "Check for updates but let me choose whether to download them" within the Windows Update Settings, but you will not be informed about new updates via a popup on the desktop (as it was in windows 7). Only on the logon screen you can see a hint in the bottom right corner. 

Microsoft’s official [answer](http://social.technet.microsoft.com/Forums/en-US/w8itprogeneral/thread/d964be4c-6619-473a-a45d-27c2c85e721c) on that matter is that it is a design choice:

> "The reason of removing this feature is based on most of the users’ feedbacks. If a notification were to be displayed, this is suppressed if the user is doing something important, especially when a user watching a movie or playing a game, or in the business environment being interrupted during a PowerPoint presentation."


That's why the Windows Update Notifier has been developed. It informs about updates via a popup and it enables to open the windows update dialog directly by clicking the popup. The application only appears as an icon in the system tray so it will not interfere with your work. Although the application is developed for Windows 8/8.1 it also works on Windows 7 (but it is not necessary as Windows 7 offers this feature by itself).

As the Windows Update Notifier is a portabel application which runs on x86 and x64 systems, it doesn't need to be installed. It can be set to start automatically on windows logon. For further information (instructions, screenshots) see the [documentation] page.


## Features
- Is shown as a icon in the system tray
- Inform about updates via a popup
- Shows the number of updates in the system tray icon
- Supports Windows 7, 8, 8.1
- Search for new updates every hour (by default)
- Open the windows update dialog by clicking the popup (or via the context menu)
- If connection to the update-service can not be established, retry every 30 seconds. for 10 times.
- Four icons states: "searching", "updates found", "no updates available", "not connected"
- Settings dialog for changing update interval and configuring the app as auto startup.
- Select between two popup-styles: The Windows 7 Style and the Metro Style.
- Support for automatically installing Windows Defender updates


## Download
The download of the latest version can be found at: https://wun.codeplex.com/releases