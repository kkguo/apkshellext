@ECHO OFF
ECHO ################################################
ECHO ##            APK Shell Extension             ##
ECHO ##                                            ##
ECHO ##     http://apkshellext.googlecode.com      ##
ECHO ################################################

SYSTEMINFO | FIND /i "x64-based pc"

IF %ERRORLEVEL%==0 (
  ECHO.
  ECHO NOTE: You are using 64bit OS, For WIN7 User, you need "RUN AS ADMINISTRATOR" to run uninstall!
  ECHO.
  "%windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /unregister %~dp0\apkshellext.dll"
) ELSE (
  ECHO 32bit OS
  "%windir%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /unregister %~dp0\apkshellext.dll"
)
ECHO.
ECHO The dll can't be deleted before explorer restart.
CHOICE /M "Restart explorer process?"
IF %ERRORLEVEL%==1 (
  TASKKILL /F /IM explorer.exe
  START explorer.exe
) ELSE (
  ECHO Please delete the dll after restart explorer or reboot!
)
ECHO.
ECHO /-------------------------------------------------------------------\
ECHO  apkshellext is an open-source project,
ECHO  Please visit http://apkshellext.googlecode.com for more information
ECHO \-------------------------------------------------------------------/

PAUSE
@ECHO ON