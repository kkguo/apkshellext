REM CHOICE /M "Restart explorer process?"
REM IF %ERRORLEVEL%==1 (
  TASKKILL /F /IM explorer.exe
  REM pause
  START %WINDIR%\explorer.exe
REM )