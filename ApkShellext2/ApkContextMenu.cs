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
            var mainMenu = new ToolStripMenuItem {
                Text = Properties.Resources.menuMain,
            };

            int size = mainMenu.Height;

            bool isapk = SelectedItemPaths.ElementAt(0).EndsWith(".apk");
            bool isipa = SelectedItemPaths.ElementAt(0).EndsWith(".ipa");
            bool isappx = SelectedItemPaths.ElementAt(0).EndsWith(".appx");
            bool isappxbundle = SelectedItemPaths.ElementAt(0).EndsWith(".appxbundle");      

            #region Rename Menu
                int addVerCode = Utility.getRegistrySetting(Utility.keyRenameWithVersionCode);
                ToolStripMenuItem renameMenu = new ToolStripMenuItem() {
                    Text = string.Format(Resources.menuRenameAs, @"%AppName%_%Version%" + ((addVerCode == 1) ? "_%Revision%" : "")),
                    Image = Utility.ResizeBitmap(Properties.Resources.rename, size)
                };

                if (SelectedItemPaths.Count() == 1) {
                    string newName = getNewFileName(SelectedItemPaths.ElementAt(0));
                    if (newName != "") {
                        renameMenu.Text = string.Format(Resources.menuRenameAs, Path.GetFileName(newName));
                    } else {
                        renameMenu.Text = Resources.strReadFileFailed;
                        renameMenu.Enabled = false;
                    }
                }

                renameMenu.Click += (sender, args) => renameWithVersion();

                mainMenu.DropDownItems.Add(renameMenu);
                #endregion

            if (isapk) {
                #region Google Play Menu
                    using (ApkReader reader = new ApkReader(SelectedItemPaths.ElementAt(0))) {
                        var playMenu = new ToolStripMenuItem {
                            Text = string.Format(Resources.menuGotoGooglePlay, reader.AppName),
                            Image = Utility.ResizeBitmap(Properties.Resources.googleplay, size)
                        };

                        playMenu.Click += (sender, args) => gotoGooglePlay();
                        mainMenu.DropDownItems.Add(playMenu);
                        playMenu.Enabled = (SelectedItemPaths.Count() == 1) ||
                (Utility.getRegistrySetting(Utility.keyAlwaysShowGooglePlay) == 1);

                        var amazonMenu = new ToolStripMenuItem {
                            Text = string.Format(Resources.menuGotoAmazonAppStore, reader.AppName),
                            Image = Utility.ResizeBitmap(Properties.Resources.Amazon, size)
                        };

                        amazonMenu.Click += (sender, args) => gotoAmazonAppStore();
                        mainMenu.DropDownItems.Add(amazonMenu);
                        amazonMenu.Enabled = (SelectedItemPaths.Count() == 1) ||
                (Utility.getRegistrySetting(Utility.keyAlwaysShowGooglePlay) == 1);
                    }
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
                Image = Utility.ResizeBitmap(Properties.Resources.logo, size)
            };
            settingsMenu.Click += (sender, args) => showSettings();
            if (mainMenu.DropDownItems.Count > 0) {
                mainMenu.DropDownItems.Add("-");
            }
            mainMenu.DropDownItems.Add(settingsMenu);
            #endregion

            #region Mainmenu Icon
            // if choose multiple files, will create a icon with upto 3 icons together
            try { // draw multiple icons
                int totalIconToDraw = (SelectedItemPaths.Count() > 3) ? 3 : SelectedItemPaths.Count();
                Bitmap b = new Bitmap(size, size);
                for (int i = 0; i <totalIconToDraw; i++) {
                    using (AppPackageReader reader = AppPackageReader.Read(SelectedItemPaths.ElementAt(totalIconToDraw - i - 1)))
                    using (Graphics g = Graphics.FromImage((Image)b)) {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.DrawImage(reader.Icon, i * 3, i * 3, size - 2 * totalIconToDraw, size - 2 * totalIconToDraw);
                    }
                }
                mainMenu.Image = b;                    
            } catch (Exception ex){
                Log("Error happens during extracting icon " + ex.Message);
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
            bool key_RenameWithVersionCode = (Utility.getRegistrySetting(Utility.keyRenameWithVersionCode) == 1);
            string suffix = Path.GetExtension(path);
            string newFileName = "";
            try {
                using (AppPackageReader reader = AppPackageReader.Read(path)) {                
                    newFileName = reader.AppName + "_" + reader.Version;
                    if (key_RenameWithVersionCode) {
                        string revision = reader.Revision;
                        if (revision != "")
                            newFileName = newFileName + "_" + revision;
                    }
                }
                newFileName = Regex.Replace(newFileName, @"[\/:*?""<>|\s-]+", "_"); // remove invalid char
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
                    System.Diagnostics.Process.Start(string.Format(Properties.Resources.urlGooglePlay, package));
                }
            }
        }

        private void gotoAmazonAppStore() {
            foreach (var p in SelectedItemPaths) {
                using (ApkReader reader = new ApkReader(p)) {
                    string package = reader.PackageName;
                    System.Diagnostics.Process.Start(string.Format(Properties.Resources.urlAmazonAppStore, package));
                }
            }
        }

        private void showSettings() {
            Preferences settingForm = new Preferences();
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
            Logging.Log("[" + DateTime.Now.ToString() + "]" + message);
        }
    }
}
