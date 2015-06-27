# ApkShellext2

Apk shell extension re-write.

Extract icon and other information direct from apk files and showing on windows explorer  
![snapshot](http://kkguo.github.io/apkshellext/images/capture.png)

######[Features]
* Show Icon of apk files on windows explorer
* Show Package information in Infotip
* more

Check the [Wiki](https://github.com/kkguo/apkshellext/wiki/How-to-install-and-uninstall) for how to install / uninstall

Check the __[Project page](http://kkguo.github.io/apkshellext)__ for Download & History

----------------------------------------------------------------------------

#####[Last change on 2015-June-26]
* Rewrite a new APK parser, get resource fast from manifest.xml and resources.arsc without parsing the whole package, 10 times faster than oringinal way in Lteedee.ApkReader.
* Add info tip handler.

----------------------------------------------------------------------------

Referencing code from:
__[SharpShell](https://github.com/dwmkerr/sharpshell)__ / __[SharpZip](https://github.com/icsharpcode/SharpZipLib)__ /  __[Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader)__
