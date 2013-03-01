@ECHO OFF
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe WindowsUpdateNotifier.msbuild /t:BuildRelease
pause