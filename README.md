# ApkShellext2

A windows shell exteions for mobile app files, supporting 
* .apk (android package)
* .ipa (iOS app package)
* .appx .appxbundle (Windows phone 8.1/10 app package)

#### Please help this project
 * Translate into other language : http://translate.apkshellext.com [![Crowdin badge](https://d322cqt584bo4o.cloudfront.net/apkshellext/localized.png)](https://crowdin.com/project/apkshellext) 
 * Website maitainence : I need help to create the jekyll site
 * Tester : I need help to test new features.

Please [drop me a mail](mailto:kkguokk@gmail.com) if you want to help, thanks!

This project is open source and free, visit http://apkshellext.com for Download and more Information.

#### [Features]
 - [x] :boom: Support Windows Phone appx/appxbundle file icon.
 - [x] :boom: Support IPA (iOS app) icon.
 - [x] Show apk file icons of in explorer
 - [x] Show Package information in tip bubble
 - [x] Context menu for renaming apk file, batch renaming, with app name + version
 - [x] Goto apk's google play page.
 - [x] Check new version automatically.
 - [x] Show overlay icon for apk files.
 - [x] Support mulitple languages: 
    - English
    - 中文
    - italiano (by [Vince. M](https://crowdin.com/profile/Widget))
    - :boom:Korean (by [zinc](https://crowdin.com/profile/zinc))
    - :boom:Spain (by [eXDead22](http://translate.apkshellext.com/profile/eXDead22))
    - :boom:Persian (by [Ali.sholug](mailto:ali.sholug@gmail.com))
 - [ ] QR code to download to phone
 - [ ] Hook up adb function with namespace extension.
 - [ ] drag-drop to install / uninstall to phone

#### Check [Wiki](https://github.com/kkguo/apkshellext/wiki) for how to build
#### Feature request & Bug report, please go [Issues](https://github.com/kkguo/apkshellext/issues)

#### [Contributor]
  * [KKGuo](https://github.com/kkguo)(Author) kkguokk@gmail.com
  * Vince. M (Bug report and Italian translation) widget@hotmail.it

#### Credit :
* __[SharpShell](https://github.com/dwmkerr/sharpshell)__ Shell extion library
* __[SharpZip](https://github.com/icsharpcode/SharpZipLib)__ Zip implementation in C#
* __[Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader)__  the oringinal APK reader, not used anymore
* __[PlistCS](https://github.com/animetrics/PlistCS)__  iOS plist file reader
* __[PNGDecrush](https://github.com/MikeWeller/PNGDecrush)__ PNG decrush lib
* __[Ionic.Zlib](https://github.com/jstedfast/Ionic.Zlib)__  Another Zip implementation, used by PNGDecrush
* __[QRCoder](https://github.com/codebude/QRCoder)__ QR code generator

--------------
Originally this project hosted on [GoogleCode](code.google.com/p/apkshellext), now moved to [:octocat:Github](https://github.com/kkguo/apkshellext) and fully re-writen with a native apk reader. The obsolete code is on [master branch](https://github.com/kkguo/apkshellext/tree/master)
