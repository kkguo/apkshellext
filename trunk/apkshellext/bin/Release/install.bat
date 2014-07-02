@ECHO OFF
ECHO ################################################
ECHO ##            APK Shell Extension             ##
ECHO ##                                            ##
ECHO ##     http://apkshellext.googlecode.com      ##
ECHO ################################################

@echo off

REM === check and get the UAC for administrator privilege ===
REM === code from https://sites.google.com/site/eneerge/scripts/batchgotadmin
REM === 
:: BatchGotAdmin
:-------------------------------------
REM  --> Check for permissions
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"

REM --> If error flag set, we do not have admin.
if '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    echo UAC.ShellExecute "%~s0", "", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
    if exist "%temp%\getadmin.vbs" ( del "%temp%\getadmin.vbs" )
    pushd "%CD%"
    CD /D "%~dp0"
:--------------------------------------

REM SYSTEMINFO | FIND /i "x64-based pc"
echo %PROCESSOR_IDENTIFIER% | FIND /i "x86"

IF %ERRORLEVEL%==1 (
  "%windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" /codebase "%~dp0\apkshellext.dll"
) ELSE (
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