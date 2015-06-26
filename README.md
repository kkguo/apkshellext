# ApkShellext2

Apk shell extension re-write.

Extract icon and other information direct from apk files and showing on windows explorer  
![snapshot](https://lh6.googleusercontent.com/8yqB9uwz4DlDNrc4hm1F2V5-LT9ZlEaJtWGAzFmiL6dIHVX-EpEzVys27mCyBKaz8BTjgvJhO7Sj33U=w1256-h843)

more feature is coming...

>#####How to install:
>* Download package (zip): [v0.1 from google drive](https://drive.google.com/open?id=0B6ZEW0Or_P6gam15cTZqOS1tbnc&authuser=0)
>* [Download latest .net framework](https://www.microsoft.com/en-us/download/details.aspx?id=30653) if you don't have it.
>* Execute install.bat
>* You should see icons now
>
>#####How to uninstall:
>* Execute uninstall.bat
>* You may need run restart_explorer.bat before you can delete the dll file.

#####[2015-June-26]
Using home made the APK parser, get resource from manifest and resource without parsing the whole package, and get 10 times performance.
also add info tip handler.

----------------------------------------------------------------------------

Powered by the code from:
* [SharpShell](https://github.com/dwmkerr/sharpshell)  
* [SharpZip](https://github.com/icsharpcode/SharpZipLib)  
* [~~Iteedee.ApkReader~~](https://github.com/hylander0/Iteedee.ApkReader)
