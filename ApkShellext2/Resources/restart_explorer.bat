REM CHOICE /M "Restart explorer process?"
REM IF %ERRORLEVEL%==1 (
  TASKKILL /F /IM explorer.exe
  pause
  START explorer.exe
REM )