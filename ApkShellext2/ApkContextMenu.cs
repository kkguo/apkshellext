using ApkQuickReader;
using ApkShellext2.Properties;
using Microsoft.Win32;
using QRCoder;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.Extensions;
using SharpShell.ServerRegistration;
using SharpShell.SharpContextMenu;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using System.Globalization;
using System.Diagnostics;

namespace ApkShellext2 {
    [Guid("dcb629fc-f86f-456f-8e24-98b9b2643a9b")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".apk")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".ipa")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".appxbundle")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".appx")]
    public class ApkContextMenu : SharpContextMenu {
        /// <summary>
        /// Determines whether this instance can a shell context show menu, given the specified selected file list.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance should show a shell context menu for the specified file list; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanShowMenu() {
            return true;
        }

        /// <summary>
        /// Creates the context menu. This can be a single menu item or a tree of them.
        /// </summary>
        /// <returns>The context menu for the shell context menu.</returns>
        protected override ContextMenuStrip CreateMenu() {
            Utility.Localize();

            var menu = new ContextMenuStrip();

            string newVerAvai = "";
            if (Utility.NewVersionAvailible() && Utility.GetSetting("ShowNewVersion") == "True")
                newVerAvai = " (" + string.Format(Resources.strNewVersionAvailible, Utility.GetSetting("LatestVersion")) + ")";
            var mainMenu = new ToolStripMenuItem {
                Text = Properties.Resources.menuMain + newVerAvai
            };

            int size = mainMenu.Height + 1;

            bool hasapk = false, hasipa = false, hasappx = false, hasappxbundle = false;
            foreach (var p in SelectedItemPaths) {
                if (p.EndsWith(AppPackageReader.extAPK))
                    hasapk = true;
                if (p.EndsWith(AppPackageReader.extIPA))
                    hasipa = true;
                if (p.EndsWith(AppPackageReader.extAPPX))
                    hasappx = true;
                if (p.EndsWith(AppPackageReader.extAPPXBUNDLE))
                    hasappxbundle = true;
            };

            bool singleSelected = SelectedItemPaths.Count() == 1;
            string appname = "";
            if (singleSelected) {
                using (AppPackageReader reader = AppPackageReader.Read(SelectedItemPaths.ElementAt(0)))
                    appname = reader.AppName;
            }

            #region Rename Menu
            ToolStripMenuItem renameMenu = new ToolStripMenuItem() {
                Image = Utility.ResizeBitmap(Properties.NonLocalizeResources.iconRename, size)
            };

            if (singleSelected) {
                string newName = getNewFileName(SelectedItemPaths.ElementAt(0));
                if (newName != "") {
                    renameMenu.Text = string.Format(Resources.menuRenameAs, Path.GetFileName(newName));
                } else {
                    renameMenu.Text = Resources.strReadFileFailed;
                    renameMenu.Enabled = false;
                }
            } else {
                string renamePattern = Utility.GetSetting("RenamePattern");
                renameMenu.Text = string.Format(Resources.menuRenameAs, renamePattern);
            }

            renameMenu.Click += (sender, args) => renameWithVersion();

            mainMenu.DropDownItems.Add(renameMenu);
            mainMenu.DropDownItems.Add("-");
            
            #endregion

            if (hasapk) {
                #region APK Menu
                ToolStripMenuItem storeMenu = new ToolStripMenuItem {
                    Text = Resources.strSearchStore,
                    Image = Utility.ResizeBitmap(NonLocalizeResources.iconGooglePlay, size)
                };

                ToolStripMenuItem subMenu = null;
                appname = singleSelected ? appname : AppPackageReader.extAPK;

                if (Utility.GetSetting("ShowGooglePlay")=="True") {
                    subMenu = new ToolStripMenuItem {
                        Text = string.Format(Resources.menuGotoGooglePlay, appname),
                        Image = Utility.ResizeBitmap(NonLocalizeResources.iconGooglePlay, size)
                    };

                    subMenu.Click += (sender, args) => gotoGooglePlay();
                    storeMenu.DropDownItems.Add(subMenu);
                    subMenu.Enabled = singleSelected || (Utility.GetSetting("ShowAppStoreWhenMultiSelected") == "True");
                    
                }

                if (Utility.GetSetting("ShowAmazonStore")== "True") {
                    subMenu = new ToolStripMenuItem {
                        Text = string.Format(Resources.menuGotoAmazonAppStore, appname),
                        Image = Utility.ResizeBitmap(NonLocalizeResources.iconAmazonStore, size)
                    };

                    subMenu.Click += (sender, args) => gotoAmazonAppStore();
                    storeMenu.DropDownItems.Add(subMenu);
                    subMenu.Enabled = singleSelected || Utility.GetSetting("ShowAppStoreWhenMultiSelected")=="True";
                }

                if (Utility.GetSetting("ShowApkMirror", "False") == "True") {
                    subMenu = new ToolStripMenuItem {
                        Text = string.Format(Resources.menuGotoApkMirror, appname),
                        Image = Utility.ResizeBitmap(NonLocalizeResources.iconApkMirror, size)
                    };
                    subMenu.Click += (sender, args) => gotoApkMirror();
                    storeMenu.DropDownItems.Add(subMenu);
                    subMenu.Enabled = (singleSelected || Utility.GetSetting("ShowAppStoreWhenMultiSelected") == "True");
                }

                if (storeMenu.DropDownItems.Count>0) {
                    mainMenu.DropDownItems.Add(storeMenu);
                }
                #endregion
            }
            if (hasipa && Utility.GetSetting("ShowAppleStore")=="True") {
                #region AppleStore Menu
                appname = singleSelected ? appname : AppPackageReader.extIPA;
                var appleMenu = new ToolStripMenuItem {
                    Text = string.Format(Resources.menuGotoAppleAppStore, appname),
                    Image = Utility.ResizeBitmap(NonLocalizeResources.iconAppStore, size)
                };
                appleMenu.Click += (sender, args) => gotoAppleStore();
                mainMenu.DropDownItems.Add(appleMenu);
                appleMenu.Enabled = singleSelected ||
                Utility.GetSetting("ShowAppStoreWhenMultiSelected")== "True";
                #endregion
            }

            if ((hasappx || hasappxbundle) && Utility.GetSetting("ShowMSStore")=="True") {
                #region MS Store Menu
                appname = singleSelected ? appname :
                    (AppPackageReader.extAPPX + @"/" + AppPackageReader.extAPPXBUNDLE);
                var msMenu = new ToolStripMenuItem {
                    Text = string.Format(Resources.menuGotoMicrosoftStore, appname),
                    Image = Utility.ResizeBitmap(NonLocalizeResources.iconMSStore, size)
                };
                msMenu.Click += (sender, args) => gotoMicrosoftStore();
                mainMenu.DropDownItems.Add(msMenu);
                msMenu.Enabled = singleSelected ||
                Utility.GetSetting("ShowAppStoreWhenMultiSelected")== "True";
                #endregion
            }

            #region Barcode
            /* Not enabled yet
            var barcodeMenu = new ToolStripMenuItem();
            barcodeMenu.Text = Resources.strScanToDownload;
            barcodeMenu.Image = Utility.ResizeBitmap(Resources.QR_Scan, size);
            barcodeMenu.Enabled = false;
            if (SelectedItemPaths.Count() == 1) {
                barcodeMenu.Enabled = true;
                var barcode = new ToolStripMenuItem();

                string md5;
                using (MD5 md5Hash = MD5.Create()) {
                    md5 = Utility.GetMd5Hash(md5Hash, SelectedItemPaths.First());
                }

                try {
                    // Notice service regarding the path
                    using (var client = new WebClient()) {
                        var values = new NameValueCollection();
                        client.QueryString.Add("md5", md5);
                        client.QueryString.Add("path", SelectedItemPaths.First());
                        client.BaseAddress = "http://localhost:42728";
                        client.UploadValues("", values);
                    }

                    string url = @"http://" + Utility.LocalIPAddress() + @":42728/" + md5;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.M);
                    barcode.Image = qrCode.GetGraphic(5);

                    barcodeMenu.DropDownItems.Add(barcode);
                    barcode.Click += (sender,args) => gotoDownload();
                } catch (Exception ex) {
                    Log(ex.Message);
                    barcodeMenu.Enabled = false;
                }
            }
            mainMenu.DropDownItems.Add(barcodeMenu);
            */
            #endregion

            #region Preferences Menu
            var settingsMenu = new ToolStripMenuItem {
                Text = Resources.menuPreferences,
                Image = Utility.ResizeBitmap(Properties.NonLocalizeResources.logo, size)
            };
            if (mainMenu.DropDownItems.Count > 1) {
                mainMenu.DropDownItems.Add("-");
            }
            settingsMenu.Click += (sender, args) => showSettings();            
            mainMenu.DropDownItems.Add(settingsMenu);
            #endregion

            #region Mainmenu Icon
            if (Utility.GetSetting("ShowMenuIcon")=="True") {
                // if choose multiple files, will create a icon with upto 3 icons together
                try { // draw multiple icons
                    int totalIconToDraw = (SelectedItemPaths.Count() > 3) ? 3 : SelectedItemPaths.Count();
                    Bitmap b = new Bitmap(size, size);
                    for (int i = 0; i < totalIconToDraw; i++) {
                        using (AppPackageReader reader = AppPackageReader.Read(SelectedItemPaths.ElementAt(totalIconToDraw - i - 1)))
                        using (Graphics g = Graphics.FromImage((Image)b)) {
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            g.DrawImage(reader.Icon, i * 3, i * 3, size - 2 * totalIconToDraw, size - 2 * totalIconToDraw);
                        }
                    }
                    mainMenu.Image = b;
                } catch (Exception ex) {
                    mainMenu.Image = Utility.ResizeBitmap(Properties.NonLocalizeResources.logo, size);
                    Log("Error happens during extracting icon " + ex.Message);
                }
            } else {
                mainMenu.Image = Utility.ResizeBitmap(Properties.NonLocalizeResources.logo, size);
            }
            #endregion

            menu.Items.Add(mainMenu);
            return menu;
        }

        //private void gotoDownload() {
        //    string md5;
        //    using (MD5 md5Hash = MD5.Create()) {
        //        md5 = Utility.GetMd5Hash(md5Hash, SelectedItemPaths.First());
        //    }
        //    System.Diagnostics.Process.Start(@"http://" + Utility.LocalIPAddress() + @":42728/" + md5);
        //}

        /// <summary>
        /// get the new filename for renaming function
        /// </summary>
        /// <returns></returns>
        private string getNewFileName(string path) {
            //bool key_RenameWithVersionCode = Settings.DefaultRenameWithVersionCode) == 1);
            string suffix = Path.GetExtension(path);
            string newFileName = "";
            string renamePattern = Utility.GetSetting("RenamePattern", Resources.strRenamePatternDefault);
            bool isapk = SelectedItemPaths.ElementAt(0).EndsWith(".apk");
            bool isipa = SelectedItemPaths.ElementAt(0).EndsWith(".ipa");

            try {
                using (AppPackageReader reader = AppPackageReader.Read(path)) {                    
                    newFileName = ReplaceVariables(renamePattern,reader);
                }

                newFileName = Regex.Replace(newFileName, @"[\/:*?""<>|-]+", ""); // remove invalid chars
                if (Utility.GetSetting("ReplaceSpace")=="True") {
                    newFileName = Regex.Replace(newFileName, @"\s+", "_");
                }
                string oldFileName = Path.GetFileName(path);
                string folderPath = Path.GetDirectoryName(path);
                string tmpFileName = newFileName + suffix;

                int i = 0;
                while (File.Exists(folderPath + @"\" + tmpFileName) &&
                    (tmpFileName != oldFileName)) {
                    i++;
                    tmpFileName = newFileName + "_(" + i.ToString() + ")" + suffix;
                }
                newFileName = folderPath + @"\" + tmpFileName;
            } catch {
                newFileName = "";
            }
            return newFileName;
        }

        public static string ReplaceVariables(string ori, AppPackageReader reader) {
            string newStr = ori;
            if (newStr.Contains(NonLocalizeResources.varAppName))
                newStr = newStr.Replace(NonLocalizeResources.varAppName, reader.AppName);
            if (newStr.Contains(NonLocalizeResources.varPackage))
                newStr = newStr.Replace(NonLocalizeResources.varPackage, reader.PackageName);
            if (newStr.Contains(NonLocalizeResources.varPublisher))
                newStr = newStr.Replace(NonLocalizeResources.varPublisher, reader.Publisher);
            if (newStr.Contains(NonLocalizeResources.varVersion))
                newStr = newStr.Replace(NonLocalizeResources.varVersion, reader.Version);
            if (newStr.Contains(NonLocalizeResources.varRevision))
                newStr = newStr.Replace(NonLocalizeResources.varRevision, reader.Revision);
            if (newStr.Contains(NonLocalizeResources.varFileSize))
                newStr = newStr.Replace(NonLocalizeResources.varFileSize, Utility.getFileSize(reader.FileName));
            if (newStr.Contains(NonLocalizeResources.varOS))
                newStr = newStr.Replace(NonLocalizeResources.varOS, 
                    reader.Type == AppPackageReader.AppType.AndroidApp ? "Android" 
                    : (reader.Type == AppPackageReader.AppType.iOSApp ? "iOS" 
                    : "Windows"));
            if (newStr.Contains(NonLocalizeResources.varLastModify))
                newStr = newStr.Replace(NonLocalizeResources.varLastModify,
                File.GetLastWriteTime(reader.FileName).ToString("dd/MM/yy HH:mm:ss"));
            if (newStr.Contains(NonLocalizeResources.varDebuggable))
                newStr = newStr.Replace(NonLocalizeResources.varDebuggable,
                    (reader.Type == AppPackageReader.AppType.AndroidApp)?
                    (((ApkReader)reader).Debuggable?"Debug":"Release"):"");
            return newStr;
        }

        private void renameWithVersion() {
            foreach (var path in SelectedItemPaths) {
                try {
                    string newFilename = getNewFileName(path);
                    File.Move(path, newFilename);
                } catch (Exception e) {
                    Log("Exception happens during rename: " + e.Message);
                }
            }
        }

        private void gotoGooglePlay() {
            foreach (var p in SelectedItemPaths) {
                using (ApkReader reader = new ApkReader(p)) {
                    string package = reader.PackageName;
                    Process.Start(string.Format(Properties.NonLocalizeResources.urlGooglePlay, package));
                }
            }
        }

        private void gotoAmazonAppStore() {
            foreach (var p in SelectedItemPaths) {
                using (ApkReader reader = new ApkReader(p)) {
                    string package = reader.PackageName;
                    Process.Start(string.Format(Properties.NonLocalizeResources.urlAmazonAppStore, package));
                }
            }
        }

        private void gotoApkMirror() {
            foreach (var p in SelectedItemPaths) {
                using (ApkReader reader = new ApkReader(p)) {
                    string package = reader.PackageName;
                    Process.Start(string.Format(Properties.NonLocalizeResources.urlApkMirror, reader.Publisher, package));
                }
            }
        }

        private void gotoAppleStore() {
            foreach (var p in SelectedItemPaths) {
                if (p.EndsWith(AppPackageReader.extIPA)) {
                    using (IpaReader reader = AppPackageReader.Read(p) as IpaReader) {
                        try {
                            Process.Start(string.Format(Properties.NonLocalizeResources.urlAppleStore, reader.AppID));
                        } catch (Exception ex) {
                            Log(Path.GetFileName(p) + ex.Message + Environment.NewLine + "Cannot get appid.");
                        }
                    }
                }
            }
        }

        private void gotoMicrosoftStore() {
            foreach (var p in SelectedItemPaths) {
                if (p.EndsWith(AppPackageReader.extAPPX) || p.EndsWith(AppPackageReader.extAPPXBUNDLE)) {
                    using (AppPackageReader reader = AppPackageReader.Read(p)) {
                        string package = reader.PackageName;
                        CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
                        Process.Start(string.Format(Properties.NonLocalizeResources.urlMicrosoftStore,reader.AppID));
                    }
                }
            }
        }

        private void showSettings() {
            Preferences settingForm = new Preferences();
            settingForm.currentFile = SelectedItemPaths.ElementAt(0);
            settingForm.ShowDialog();
        }

        [CustomRegisterFunction]
        public static void postDoRegister(Type type, RegistrationType registrationType) {
            Console.WriteLine("Registering " + type.FullName);

            #region Clean up older versions registry
            try {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"\CLSID\" +
                    type.GUID.ToRegistryString() + @"\InprocServer32")) {
                    if (key != null && key.GetSubKeyNames().Count() != 0) {
                        Console.WriteLine("Found old version in registry, cleaning up ...");
                        foreach (var k in key.GetSubKeyNames()) {
                            if (k != type.Assembly.GetName().Version.ToString()) {
                                Registry.ClassesRoot.DeleteSubKeyTree(@"\CLSID\" +
                        type.GUID.ToRegistryString() + @"\InprocServer32\" + k);
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Logging.Error("Cleaning up older version but see exception. "
                     + e.Message);
            }
            #endregion
        }

        protected override void Log(string message) {
            string s = (SelectedItemPaths.Count() >0) ?Path.GetFileName(SelectedItemPaths.ElementAt(0)): "";
            Utility.Log(this, s, message);
        }
    }
}
