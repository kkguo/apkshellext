# ApkShellext2

A windows shell exteions for mobile app files, supporting 
* .apk (android package)
* .ipa (iOS app package)
* .appx .appxbundle (Windows phone 8.1/10 app package, .xap is not supported)

#### Please help this project
 * Translate : http://translate.apkshellext.com 
 * jekyll site maitainence.
 * Tester for new features.

Please [drop me a mail](mailto:kkguokk@gmail.com) if you want to help, pull request is welcome.

This project is open source and free, visit http://apkshellext.com for Download and more Information.

#### [Features]
 - [x] Display app icon in explorer:
 - [x] Show Package information in tip bubble
 - [x] Context menu for renaming apk file, batch renaming, with app name + version
 - [x] Go to app store from context menu.
 - [x] Check new version automatically.
 - [x] Show overlay icon for different type of apps.
 - [x] Support upto 15 languages (some are machine translated):
    - English
    - 中文(简体/繁體)
    - italiano (by [Vince. M](https://crowdin.com/profile/Widget))
    - Korean (by [zinc](https://crowdin.com/profile/zinc))
    - Spain (by [eXDead22](http://translate.apkshellext.com/profile/eXDead22))
    - Persian (by [Ali Rex](http://translate.apkshellext.com/profile/ali-sholug))
    - Hebrew (by [gbyx](http://translate.apkshellext.com/profile/gbyx))
    - Portuguese (by [Eduardo Santos](http://translate.apkshellext.com/profile/eduardo650498))
    - German (by [rekire](http://translate.apkshellext.com/profile/rekire)
    - Japanese (by [Noumi](http://translate.apkshellext.com/profile/Noumi)
    - Franch (by [Charles Milette](http://translate.apkshellext.com/profile/charles_milette))
 - [ ] QR code to download to phone
 - [ ] Hook up adb function with namespace extension.
 - [ ] drag-drop to install / uninstall to phone

#### Check [Wiki](https://github.com/kkguo/apkshellext/wiki) for how to build
#### Feature request & Bug report, please go [Issues](https://github.com/kkguo/apkshellext/issues)

#### Credit :
|||
| --- | --- |
| [SharpShell](https://github.com/dwmkerr/sharpshell)                 | Shell exten tion library                           |
| [SharpZip](https://github.com/icsharpcode/SharpZipLib)              | Zip function implementation in C#              |
| [Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader) | the oringinal APK reader, not in use currently     |
| [PlistCS](https://github.com/animetrics/PlistCS)                    | iOS plist file reader                          |
| [PNGDecrush](https://github.com/MikeWeller/PNGDecrush)              | PNG decrush lib                                |
| [Ionic.Zlib](https://github.com/jstedfast/Ionic.Zlib)               | Another Zip implementation, used by PNGDecrush |
| [QRCoder](https://github.com/codebude/QRCoder)                      | QR code generator                              |
| [Ultimate Social](https://www.iconfinder.com/iconsets/ultimate-social) | A free icon set for social medias           |

--------------
Originally this project hosted on [GoogleCode](code.google.com/p/apkshellext), now moved to [:octocat:Github](https://github.com/kkguo/apkshellext) and fully re-writen with a native apk reader. The obsolete code is on [master branch](https://github.com/kkguo/apkshellext/tree/master)
