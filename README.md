# ApkShellext2![logo](https://github.com/kkguo/apkshellext/blob/ApkShellext2/ApkShellext2/Resources/Apkshellext_icons/apkshell_b64.png?raw=true)  
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/apkshellext/localized.svg)](http://translate.apkshellext.com/)

A Windows shell extension supporting icon for files of
* .apk (android package)
* .ipa (iOS app package)
* .appx .appxbundle (Windows phone 8.1/10 app package, .xap is not supported)

This is the code repository, please visit the project page http://apkshellext.com for Download and no more Information.

#### Please help this project
 * [<img src="https://c5.patreon.com/external/logo/become_a_patron_button.png" width="170" height="36" > and buy me a beer](https://www.patreon.com/apkshellext) , I will be flattered.
 * Translators please read this first : [Translate Wiki page](https://github.com/kkguo/apkshellext/wiki/Translation-and-Multi-language-support#1-translate) 
 * Project page needs web people to maintain.
 * Beta testers are always needed.
 * Pull request is always welcome.

#### Have issue?
 * check the [Known issue list](https://github.com/kkguo/apkshellext/wiki/Known-issues-and-solution) first.
 * Join discussion on Telegram[![telegram](https://github.com/kkguo/apkshellext/blob/ApkShellext2/ApkShellext2/Resources/telegram_s.png)@apkshellext](https://telegram.me/joinchat/BrcZsQAtOKWeA7ThTyq3Ug)
  * go [Issues](https://github.com/kkguo/apkshellext/issues)
  * :email: issue@apkshellext.com

#### [Features]
 - [x] Display app icon in Windows:registered: File Explorer with best resolution. __DOES NOT work with other file manager due to .NET restriction__
 - [x] Customize-able Info-Tip for showing package information.
 - [x] Context menu for batch renaming, use customize-able patterns.
 - [x] Go to app store from context menu.
 - [x] Auto-check new version.
 - [x] Show overlay icon for different type of apps.
 - [x] Support multiple languages: Thanks to contributors on https://crowdin.com/project/apkshellext
    
#### [Todo]
 - [X] Adaptive-icon support (Beta)
 - [ ] protobuf support
 - [ ] support .NET 4.0??
 - [ ] QR code to download to phone
 - [ ] Hook up adb function with namespace extension.
 - [ ] drag-drop to install / uninstall to phone

#### Check [Wiki](https://github.com/kkguo/apkshellext/wiki) for how to build from source code

#### Credit :
|||
| --- | --- |
| [SharpShell](https://github.com/dwmkerr/sharpshell)                 | Shell extension library                        |
| [SharpZip](https://github.com/icsharpcode/SharpZipLib)              | Zip function implementation in C#              |
| [Iteedee.ApkReader](https://github.com/hylander0/Iteedee.ApkReader) | the original APK reader, not in use currently  |
| [PlistCS](https://github.com/animetrics/PlistCS)                    | iOS plist file reader                          |
| [PNGDecrush](https://github.com/MikeWeller/PNGDecrush)              | PNG decrush lib                                |
| [Ionic.Zlib](https://github.com/jstedfast/Ionic.Zlib)               | Another Zip implementation, used by PNGDecrush |
| [QRCoder](https://github.com/codebude/QRCoder)                      | QR code generator                              |
| [Ultimate Social](https://www.iconfinder.com/iconsets/ultimate-social) | A free icon set for social medias           |
| [SVG](https://github.com/vvvv/SVG)                                  | SVG format renderer                            |
| [WebP-Wrapper](https://github.com/JosePineiro/WebP-wrapper)         | WebP format renderer
| [Thanasis Georgiou](https://github.com/sakisds)                     | Project web page |
--------------
Originally this project hosted on [GoogleCode](code.google.com/p/apkshellext), now moved to [:octocat:Github](https://github.com/kkguo/apkshellext) and fully re-writen with a native apk reader. The active developing code is on [ApkShellext2 branch](https://github.com/kkguo/apkshellext/tree/ApkShellext2). The obsolete code is on [master branch](https://github.com/kkguo/apkshellext/tree/master)
