using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.Extensions;
using SharpShell.ServerRegistration;
using SharpShell.SharpContextMenu;
using System.Collections.Generic;

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
            var menu = new ContextMenuStrip();

            var mainMenu = new ToolStripMenuItem {
                Text = @"APK Shell Extension",
            };

            int size = mainMenu.Height - 1;

            #region Rename Menu

            int addVerCode = Utility.getRegistrySetting("RenameWithVersionCode");
            ToolStripMenuItem renameMenu = new ToolStripMenuItem() {
                Text = @"Rename to <Label>_<verNumber>" + ((addVerCode==1)?"_<verCode>":"") +".apk",
                Image = Utility.ResizeBitmap(Properties.Resources.rename, size)
            };

            if (SelectedItemPaths.Count() == 1) {
                string newName = getNewFileName(SelectedItemPaths.ElementAt(0));
                if (newName != "") {
                    renameMenu.Text = @"Rename to " + Path.GetFileName(newName);
                } else {
                    renameMenu.Text = @"Failed to read apk file.";
                    renameMenu.Enabled = false;
                }
            }

            renameMenu.Click += (sender, args) => renameWithVersion();
            #endregion

            #region Google Play Menu
            var playMenu = new ToolStripMenuItem {
                Text = @"Goto GooglePlay page",
                Image = Utility.ResizeBitmap(Properties.Resources.googleplay, size)
            };

            playMenu.Click += (sender, args) => gotoGooglePlay();
            #endregion

            #region Setting Menu
            var settingsMenu = new ToolStripMenuItem {
                Text = @"Preferences",
                Image = Utility.ResizeBitmap(Properties.Resources.logo, size)
            };
            settingsMenu.Click += (sender, args) => showSettings();
            #endregion

            #region Mainmenu Icon
            // Draw Apk icon to the menu
            // if choose multiple files, will create a icon with upto 3 icons together.             
            try { // draw multiple icons
                ApkQuickReader reader0 = new ApkQuickReader(SelectedItemPaths.ElementAt(0));
                ApkQuickReader reader1 = null;
                ApkQuickReader reader2 = null;
                if (SelectedItemPaths.Count() == 1) {
                    mainMenu.Image = Utility.ResizeBitmap(reader0.getImage("application", "icon"), size);
                } else {
                    reader1 = new ApkQuickReader(SelectedItemPaths.ElementAt(1));
                    if (SelectedItemPaths.Count() > 2) {
                        reader2 = new ApkQuickReader(SelectedItemPaths.ElementAt(2));
                    }
                    Bitmap b = new Bitmap(size, size);
                    using (Graphics g = Graphics.FromImage((Image)b)) {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        if (SelectedItemPaths.Count() > 2) {
                            g.DrawImage(reader2.getImage("application", "icon"), 0, 0, size - 6, size - 6);
                            g.DrawImage(reader1.getImage("application", "icon"), 3, 3, size - 6, size - 6);
                        } else {
                            g.DrawImage(reader1.getImage("application", "icon"), 0, 0, size - 6, size - 6);
                        }
                        g.DrawImage(reader0.getImage("application", "icon"), 5, 5, size - 6, size - 6);
                    }
                    mainMenu.Image = b;
                }
            } catch { }
            #endregion

            mainMenu.DropDownItems.Add(renameMenu);
            mainMenu.DropDownItems.Add(playMenu);
            playMenu.Enabled = (SelectedItemPaths.Count() == 1) ||
                (Utility.getRegistrySetting("AlwaysShowGooglePlay") == 1);

            mainMenu.DropDownItems.Add("-");
            mainMenu.DropDownItems.Add(settingsMenu);
            menu.Items.Add(mainMenu);
            return menu;
        }

        /// <summary>
        /// get the new filename for renaming function
        /// </summary>
        /// <returns></returns>
        private string getNewFileName(string path) {
            bool key_RenameWithVersionCode = (Utility.getRegistrySetting("RenameWithVersionCode")==1);
            string newFileName = "";
            try {
                ApkQuickReader reader = new ApkQuickReader(path);
                newFileName = reader.getAttribute("application", "label")
                     + "_" + reader.getAttribute("manifest", "versionName");
                if (key_RenameWithVersionCode) {
                    string versionCode = reader.getAttribute("manifest", "versionCode");
                    newFileName = newFileName + "_" + versionCode;
                }
                newFileName = Regex.Replace(newFileName, @"[\/:*?""<>|\s]", "_"); // remove invalide char
                string oldFileName = Path.GetFileName(path);
                string folderPath = Path.GetDirectoryName(path);
                string tmpFileName =  newFileName + ".apk";
                
                int i = 0;
                while (File.Exists( folderPath + @"\" + tmpFileName ) && 
                    (tmpFileName != oldFileName)) {
                    i++;
                    tmpFileName = newFileName + "_(" + i.ToString() + ")" + ".apk";
                }
                newFileName = folderPath + @"\" + tmpFileName;
            } catch (Exception ex) {
                newFileName = "";
                Log("Error happens during rename :" + ex.Message);
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
                ApkQuickReader reader = new ApkQuickReader(p);
                string package = reader.getAttribute("manifest", "package");
                System.Diagnostics.Process.Start("https://play.google.com/store/apps/details?id=" + package);
            }
        }

        private void showSettings() {
            Settings settingForm = new Settings();
            settingForm.ShowDialog();
        }

        [CustomRegisterFunction]
        public static void postDoRegister(Type type, RegistrationType registrationType) {
            Console.WriteLine("Registering " + type.FullName);

            #region Clean up older versions registry
            try {
                Console.WriteLine("Found old version in registry, cleaning up ...");
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"\CLSID\" +
                    type.GUID.ToRegistryString() + @"\InprocServer32")) {
                    foreach (var k in key.GetSubKeyNames()) {
                        if (k != type.Assembly.GetName().Version.ToString()) {
                            Registry.ClassesRoot.DeleteSubKeyTree(@"\CLSID\" +
                    type.GUID.ToRegistryString() + @"\InprocServer32\" + k);
                        }
                    }
                }
            } catch (Exception e) {
                Logging.Error("Cleaning up older version but see exception. "
                     + e.Message);
            }
            #endregion
        }
    }
}
