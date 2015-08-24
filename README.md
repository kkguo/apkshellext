# ApkShellext2

A windows shell exteions for mobile app files, supporting 
* .apk (android package)
* .ipa (iOS app package)
* .appx .appxbundle (Windows phone 8.1/10 app package)

#### Please help this project
 * Translate into other language : http://translate.apkshellext.com [![Crowdin badge](https://d322cqt584bo4o.cloudfront.net/apkshellext/localized.png)](https://crowdin.com/project/apkshellext) 
 * Website maitainence : I need help to create the jekyll site
 * Tester : I need help to test new features.

Please [drop me a mail](mailto:kkguokk@gmail.com) if you want to help, pull request is welcome.

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

#### Credit :
|||
| --- | --- |
| [SharpShell](https://github.com/dwmkerr/sharpshell)                 | Shell extion library                           |
| [SharpZip](https://github.com/icsharpcode/SharpZipLib)              | Zip function implementation in C#              |
| [Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader) | the oringinal APK reader, not used anymore     |
| [PlistCS](https://github.com/animetrics/PlistCS)                    | iOS plist file reader                          |
| [PNGDecrush](https://github.com/MikeWeller/PNGDecrush)              | PNG decrush lib                                |
| [Ionic.Zlib](https://github.com/jstedfast/Ionic.Zlib)               | Another Zip implementation, used by PNGDecrush |
| [QRCoder](https://github.com/codebude/QRCoder)                      | QR code generator                              |
| [Ultimate Social](https://www.iconfinder.com/iconsets/ultimate-social) | A free icon set for social medias           |

--------------
Originally this project hosted on [GoogleCode](code.google.com/p/apkshellext), now moved to [:octocat:Github](https://github.com/kkguo/apkshellext) and fully re-writen with a native apk reader. The obsolete code is on [master branch](https://github.com/kkguo/apkshellext/tree/master)
