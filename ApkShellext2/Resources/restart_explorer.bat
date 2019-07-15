REM CHOICE /M "Restart explorer process?"
REM IF %ERRORLEVEL%==1 (
  TASKKILL /F /IM explorer.exe
  TASKKILL /F /IM dllhost.exe
  REM pause
  START %WINDIR%\explorer.exe
REM )