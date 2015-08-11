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

            int size = mainMenu.Height - 1;

            #region Rename Menu

            int addVerCode = Utility.getRegistrySetting(Utility.keyRenameWithVersionCode);
            ToolStripMenuItem renameMenu = new ToolStripMenuItem() {
                Text = string.Format(Resources.menuRenameAs , @"<Label>_<verNumber>" + ((addVerCode == 1) ? "_<verCode>" : "") + ".apk"),
                Image = Utility.ResizeBitmap(Properties.Resources.rename, size)
            };

            if (SelectedItemPaths.Count() == 1) {
                string newName = getNewFileName(SelectedItemPaths.ElementAt(0));
                if (newName != "") {
                    renameMenu.Text = string.Format(Resources.menuRenameAs,Path.GetFileName(newName));
                } else {
                    renameMenu.Text = Resources.strReadApkFailed;
                    renameMenu.Enabled = false;
                }
            }

            renameMenu.Click += (sender, args) => renameWithVersion();
            #endregion

            #region Google Play Menu
            var playMenu = new ToolStripMenuItem {
                Text = Resources.menuGotoGooglePlay,
                Image = Utility.ResizeBitmap(Properties.Resources.googleplay, size)
            };

            playMenu.Click += (sender, args) => gotoGooglePlay();
            #endregion

            #region Preferences Menu
            var settingsMenu = new ToolStripMenuItem {
                Text = Resources.menuPreferences,
                Image = Utility.ResizeBitmap(Properties.Resources.logo, size)
            };
            settingsMenu.Click += (sender, args) => showSettings();
            #endregion

            /* Not enabled yet
            #region Barcode
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
            #endregion
            */

            #region Mainmenu Icon
            // Draw Apk icon to the menu
            // if choose multiple files, will create a icon with upto 3 icons together.             
            try { // draw multiple icons
                ApkReader reader0 = new ApkReader(SelectedItemPaths.ElementAt(0));
                ApkReader reader1 = null;
                ApkReader reader2 = null;
                if (SelectedItemPaths.Count() == 1) {
                    mainMenu.Image = Utility.ResizeBitmap(reader0.getImage("application", "icon"), size);
                } else {
                    reader1 = new ApkReader(SelectedItemPaths.ElementAt(1));
                    if (SelectedItemPaths.Count() > 2) {
                        reader2 = new ApkReader(SelectedItemPaths.ElementAt(2));
                    }
                    Bitmap b = new Bitmap(size, size);
                    using (Graphics g = Graphics.FromImage((Image)b)) {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        if (SelectedItemPaths.Count() > 2) {
                            g.DrawImage(reader2.getImage("application", "icon"), 0, 0, size - 6, size - 6);
                            g.DrawImage(reader1.getImage("application", "icon"), 3, 3, size - 6, size - 6);
                            reader2.Close();
                            reader1.Close();
                        } else {
                            g.DrawImage(reader1.getImage("application", "icon"), 0, 0, size - 6, size - 6);
                            reader1.Close();
                        }
                        g.DrawImage(reader0.getImage("application", "icon"), 5, 5, size - 6, size - 6);
                        reader0.Close();
                    }
                    mainMenu.Image = b;
                }
            } catch { }
            #endregion

            mainMenu.DropDownItems.Add(renameMenu);
            mainMenu.DropDownItems.Add(playMenu);
            playMenu.Enabled = (SelectedItemPaths.Count() == 1) ||
                (Utility.getRegistrySetting(Utility.keyAlwaysShowGooglePlay) == 1);

            //mainMenu.DropDownItems.Add(barcodeMenu);

            mainMenu.DropDownItems.Add("-");
            mainMenu.DropDownItems.Add(settingsMenu);
            menu.Items.Add(mainMenu);
            return menu;
        }

        private void gotoDownload() {
            string md5;
            using (MD5 md5Hash = MD5.Create()) {
                md5 = Utility.GetMd5Hash(md5Hash, SelectedItemPaths.First());
            }
            System.Diagnostics.Process.Start(@"http://" + Utility.LocalIPAddress() + @":42728/" + md5);
        }
        /// <summary>
        /// get the new filename for renaming function
        /// </summary>
        /// <returns></returns>
        private string getNewFileName(string path) {
            bool key_RenameWithVersionCode = (Utility.getRegistrySetting(Utility.keyRenameWithVersionCode) == 1);
            string newFileName = "";
            try {
                using (ApkReader reader = new ApkReader(path)) {
                    newFileName = reader.getAttribute("application", "label")
                         + "_" + reader.getAttribute("manifest", "versionName");
                    if (key_RenameWithVersionCode) {
                        string versionCode = reader.getAttribute("manifest", "versionCode");
                        newFileName = newFileName + "_" + versionCode;
                    }
                }
                newFileName = Regex.Replace(newFileName, @"[\/:*?""<>|\s]", "_"); // remove invalid char
                string oldFileName = Path.GetFileName(path);
                string folderPath = Path.GetDirectoryName(path);
                string tmpFileName = newFileName + ".apk";

                int i = 0;
                while (File.Exists(folderPath + @"\" + tmpFileName) &&
                    (tmpFileName != oldFileName)) {
                    i++;
                    tmpFileName = newFileName + "_(" + i.ToString() + ")" + ".apk";
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
                    string package = reader.getAttribute("manifest", "package");
                    System.Diagnostics.Process.Start(string.Format(Properties.Resources.urlGooglePlay, package));
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
