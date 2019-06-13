# ApkShellext2![logo](https://github.com/kkguo/apkshellext/blob/ApkShellext2/ApkShellext2/Resources/Apkshellext_icons/apkshell_b64.png?raw=true)  
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/apkshellext/localized.svg)](http://translate.apkshellext.com/project/apkshellext)

A Windows shell extension for mobile app files supporting 
* .apk (android package)
* .ipa (iOS app package)
* .appx .appxbundle (Windows phone 8.1/10 app package, .xap is not supported)

This is the code repository, please visit the project page http://apkshellext.com for Download and more Information.

#### Please help this project

 * Translators please read this first : [Translate Wiki page] (https://github.com/kkguo/apkshellext/wiki/Translation-and-Multi-language-support#1-translate) 
 * Project page is under re-construction, [Thanasis Georgiou](https://github.com/sakisds) is taking control.
 * Beta testers are always needed. Be prepared for your computer to explode.

#### Say something to this project
 * :email: issue@apkshellext.com
 * Join us on Telegram[![telegram](https://github.com/kkguo/apkshellext/blob/ApkShellext2/ApkShellext2/Resources/telegram_s.png)@apkshellext](https://telegram.me/joinchat/BrcZsQAtOKWeA7ThTyq3Ug)
 * Feature request & Bug report, please go [Issues](https://github.com/kkguo/apkshellext/issues)
 * if you see it doesn't work on certain app file (apk/ipa), please upload that package. If there is copy right issue, please paste link.
 * Pull request is always welcome.

#### [Features]
 - [x] Display app icon in explorer with best resolution.
 - [x] Customize-able Info-Tip for showing package information.
 - [x] Context menu for batch renaming, use customize-able patterns.
 - [x] Go to app store from context menu.
 - [x] Auto-check new version.
 - [x] Show overlay icon for different type of apps.
 - [x] Support multiple languages: check https://crowdin.com/project/apkshellext for credit
    
#### [Todo]
 - [ ] Adaptive-icon support
 - [ ] protobuf support
 - [ ] QR code to download to phone
 - [ ] Hook up adb function with namespace extension.
 - [ ] drag-drop to install / uninstall to phone

#### Check [Wiki](https://github.com/kkguo/apkshellext/wiki) for how to build from source code

#### Credit :
|||
| --- | --- |
| [SharpShell](https://github.com/dwmkerr/sharpshell)                 | Shell extension library                           |
| [SharpZip](https://github.com/icsharpcode/SharpZipLib)              | Zip function implementation in C#              |
| [Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader) | the original APK reader, not in use currently     |
| [PlistCS](https://github.com/animetrics/PlistCS)                    | iOS plist file reader                          |
| [PNGDecrush](https://github.com/MikeWeller/PNGDecrush)              | PNG decrush lib                                |
| [Ionic.Zlib](https://github.com/jstedfast/Ionic.Zlib)               | Another Zip implementation, used by PNGDecrush |
| [QRCoder](https://github.com/codebude/QRCoder)                      | QR code generator                              |
| [Ultimate Social](https://www.iconfinder.com/iconsets/ultimate-social) | A free icon set for social medias           |

--------------
Originally this project hosted on [GoogleCode](code.google.com/p/apkshellext), now moved to [:octocat:Github](https://github.com/kkguo/apkshellext) and fully re-writen with a native apk reader. The obsolete code is on [master branch](https://github.com/kkguo/apkshellext/tree/master)
