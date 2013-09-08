@ECHO OFF
ECHO ################################################
ECHO ##            APK Shell Extension             ##
ECHO ##                                            ##
ECHO ##     http://apkshellext.googlecode.com      ##
ECHO ################################################

REM SYSTEMINFO | FIND /i "x64-based pc"
echo %PROCESSOR_IDENTIFIER% | FIND /i "x86"

IF %ERRORLEVEL%==1 (
  ECHO.
  ECHO !!!! ATTENTION !!!!: You are using 64bit OS, 
  ECHO       For WIN7/8 User, you need "RUN AS ADMINISTRATOR"
  ECHO.
  "%windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /unregister %~dp0\apkshellext.dll"
) ELSE (
  ECHO 32bit OS
  "%windir%\Microsoft.NET\Framework\v4.0.30319\regasm.exe" /unregister %~dp0\apkshellext.dll"
)
ECHO.
ECHO The apkshellext.dll can't be deleted before explorer restart.

ECHO.
ECHO /-------------------------------------------------------------------\
ECHO  apkshellext is an open-source project,
ECHO  Please visit http://apkshellext.googlecode.com for more information
ECHO \-------------------------------------------------------------------/

PAUSE
@ECHO ON