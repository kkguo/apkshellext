@ECHO OFF
ECHO ################################################
ECHO ##            APK Shell Extension             ##
ECHO ##                                            ##
ECHO ##     http://apkshellext.googlecode.com      ##
ECHO ################################################

SYSTEMINFO | FIND /i "x64-based pc"

IF %ERRORLEVEL%==0 (
  ECHO.
  ECHO NOTE: You are using 64bit OS, For WIN7 User, you need "RUN AS ADMINISTRATOR" to run install!
  ECHO.
  "%windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /codebase "%~dp0\apkshellext.dll"
) ELSE (
  ECHO You are using 32bit OS
  "%windir%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /codebase "%~dp0\apkshellext.dll"
)

ECHO Done!
ECHO.
ECHO /-------------------------------------------------------------------\
ECHO  apkshellext is an open-source project,
ECHO  Please visit http://apkshellext.googlecode.com for more information
ECHO \-------------------------------------------------------------------/

PAUSE
@ECHO ON