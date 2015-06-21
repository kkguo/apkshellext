# apkshellext
Shell Extension for APK Files

This is a windows extension for APK Files, enable windows explorer to have ability to show Icon of Android app file.
Tested on WinXP/Win7/Win8 32bit/64bit 
please follow [wiki page](https://github.com/allangoing/apkshellext/wiki/How-to-install-and-use-the-shell-extension-for-APK-files) to install and uninstall.

**2015-6-21**
After move to Github, I decied re-write the extension with more robost existing code, as my code to shell operation is messy and I don't want to see them anymore. 

Now a new reposity is here: 

##>>[ApkShellext2](https://github.com/allangoing/ApkShellext2)<<

**2015-4-03**

Actually I like this place (google code), but time to move : [GitHub](https://github.com/allangoing/apkshellext)
from [google code](https://code.google.com/p/apkshellext)

**2014-7-11**

Make an icon, shows android within windows. ![icon](https://code.google.com/p/apkshellext/logo?cct=1405098752&nonsense=something_that_ends_with.png)

**2014-7-7 V2.4**

Update v2.4

*Changes*
 * use native [Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader) instead of aapt.exe, to resolve path encoding issue, and should have better performance.
 * update newest adb executable.
 * Install to device is not working stable, thinking another way. don't use it for now.

Google code doesn't allow to download anymore, so I hide the tab, I shared the download from [gdrive](https://drive.google.com/folderview?id=0B6ZEW0Or_P6gSzQ3Z1JrZVNUMkk&usp=sharing GOOGLEDRIVE)

I create [the googlecode tracker](https://code.google.com/p/apkshellext/issues/detail?id=28) for new release, please start it and subscribe update.


**2014-7-2**
It's another year passed. 

I was quite busy on collecting my hearthstone cards (LoL), while I really feel nervous when seeing people still raise issue to this page(which means I still have users), I will restart (continue on) this project SOON(c) <- Blizzard reserved this word LoL

**2013-9-9**
Hey, I got time to fix several issue, try the newest [release](https://apkshellext.googlecode.com/files/apkshellext_2_3.zip release).

*I assume you know what is ADB and what is administrator* 

Win7/8 user please use *Administrator privilege !!!*

You don't need to kill n restart explorer anymore, just use install.bat in the zip package.

__Feature__
 * Support WIN8 64bit
 * Easy install & uninstall
__Known issue__
 * Doesn't read Chinese character in the path
 * ADB doesn't list device for some time

__Q & A__
 * *Q:*Why not use cache to speed up?
 * *A:* the windows build-in icon cache mechanism will not allow individual icon for different file of same type (extension). I have to extract the icon every time when you do refresh. for those who has thousands APKs in one folder, please leave a message or drop a mail let me know how critical the situation is, I would like to think about creating a file cache in your temp folder for each icon.
 * *Q:*Cannot delete the dll after uninstall?
 * *A:* Kill the explorer and restart it. The latest release (v2.3) solved this, basically it caused by some process handler leak. But in case, I left a restart_explorer.bat in v2.3 package
 * *Q:*How often this project got updated?
 * *A:* it's a home-brew project, and I cannot guarantee frequently update, let me know if you have idea or you want to help.
 * *Q:*How to connect phone to adb via wifi?
 * *A:* I don't think it's a question for now, search wireless ADB in google play
 * *Q:*Cannot see icon when phone connected as media device
 * *A:* Basically the shell extension needs to read your app file as file, so you have to set your phone as MSD(Massive Storage Device, USB device mode), at least for now. For some brand, you need to install driver to enable ADB. 

**2013-9-8**

So surprise you guys still here, I will restart this small project soon...
So on the list:
 * add google play menu
 * permission details.
 * etc...

**2011-9-1 v2.1**

If you find the explorer no response for long time, please re-plug in your device and retry. Please use cable connect as possible as you can to get more stable.

Support 32bit and 64bit windows7, require .net framework 4.
Please see detail in [wiki page](http://code.google.com/p/apkshellext/wiki/Usage).

First post is on [XDA Dream forum](http://forum.xda-developers.com/showthread.php?t=577735&page=1), but please leave your comment in this project. 

Cannot install? Probably the [Issues list](http://code.google.com/p/apkshellext/issues/list?can=1&q=&colspec=ID+Type+Status+Priority+Milestone+Owner+Summary&cells=tiles) can help.
----
Support :
  * Icon display
  * Context menu to install/uninstall apk files, support ADB via WIFI
  * Query message for apk detail information.

TODO list :
  * show a busy window when adb was busy
  * friendly pack information in tip bobble.
  * Thumbnail for apk files
  * A Property page?
  * Update notification or auto update?
  * more option.
here is a snapshot:

  ![snapshot1](http://apkshellext.googlecode.com/files/snapshot.JPG)
  
  ![win7snapshot](http://apkshellext.googlecode.com/files/Windows_7_snapshot.jpg)
