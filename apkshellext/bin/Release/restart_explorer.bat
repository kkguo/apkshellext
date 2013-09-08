CHOICE /M "Restart explorer process?"
IF %ERRORLEVEL%==1 (
  TASKKILL /F /IM explorer.exe
  START explorer.exe
)