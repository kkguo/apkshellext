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
                Text = "APK Shell Extension",
            };

            int size = mainMenu.Height - 1;

            var checkUpdateMenu = new ToolStripMenuItem {
                Text = "ApkShellext2 on github",
                Image = betterIcon(Properties.Resources.GitHub, size)
            };
            

            checkUpdateMenu.Click += (sender, args) => gotoGitHub();

            ToolStripMenuItem renameMenu = new ToolStripMenuItem() {
                Text = "Rename to <label>_<version>.apk"
            };
            renameMenu.Image = betterIcon(Properties.Resources.rename,size);

            // Draw Apk icon to the menu
            // if choose multiple files, will create a icon with upto 3 icons together.
            if (SelectedItemPaths.Count() == 1) {
                try {
                    ApkQuickReader reader = new ApkQuickReader(SelectedItemPaths.ElementAt(0));
                    string newfilename = reader.getAttribute("application", "label")
                         + "_" + reader.getAttribute("manifest", "versionName") + ".apk";
                    Regex rgx = new Regex(@"[\/:*?""<>|\s]");
                    newfilename = rgx.Replace(newfilename, "_");
                    renameMenu.Text = "Rename to " + newfilename;
                    mainMenu.Image = betterIcon(reader.getImage("application", "icon"),size);
                } catch {
                    renameMenu.Text = "Cannot read apk file";
                    renameMenu.Enabled = false;
                    renameMenu.Image = null;
                    mainMenu.DropDownItems.Add(renameMenu);
                    mainMenu.DropDownItems.Add("-");
                    mainMenu.DropDownItems.Add(checkUpdateMenu);
                    menu.Items.Add(mainMenu);
                    return menu;
                }
            } else {
                try { // draw multiple icons
                    ApkQuickReader reader1 = new ApkQuickReader(SelectedItemPaths.ElementAt(0));
                    ApkQuickReader reader2 = new ApkQuickReader(SelectedItemPaths.ElementAt(1));
                    ApkQuickReader reader3 = null;
                    if (SelectedItemPaths.Count() > 2) {
                        reader3 = new ApkQuickReader(SelectedItemPaths.ElementAt(3));
                    }
                    Bitmap b = new Bitmap(size, size);
                    using (Graphics g = Graphics.FromImage((Image)b)) {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        if (SelectedItemPaths.Count() > 2) {
                            g.DrawImage(reader3.getImage("application", "icon"), 0, 0, size - 6, size - 6);
                            g.DrawImage(reader2.getImage("application", "icon"), 3, 3, size - 6, size - 6);
                        } else {
                            g.DrawImage(reader2.getImage("application", "icon"), 0, 0, size - 6, size - 6);
                        }
                        g.DrawImage(reader1.getImage("application", "icon"), 5, 5, size - 6, size - 6);
                    }
                    mainMenu.Image = b;
                } catch {}
            }

            renameMenu.Click += (sender, args) => renameWithVersion();
            mainMenu.DropDownItems.Add(renameMenu);

            if (SelectedItemPaths.Count() == 1) {
                var playMenu = new ToolStripMenuItem {
                    Text = "Check GooglePlay",
                    Image = betterIcon(Properties.Resources.googleplay, size)
                };

                playMenu.Click += (sender, args) => gotoGooglePlay();

                mainMenu.DropDownItems.Add(playMenu);
            }
            mainMenu.DropDownItems.Add("-");
            mainMenu.DropDownItems.Add(checkUpdateMenu);

            menu.Items.Add(mainMenu);
            return menu;
        }

        private void renameWithVersion() {
            foreach (string path in SelectedItemPaths) {
                if (path.EndsWith(".apk", StringComparison.CurrentCultureIgnoreCase)) {
                    try {
                        ApkQuickReader reader = new ApkQuickReader(path);
                        string newfilename = reader.getAttribute("application", "label")
                             + "_" + reader.getAttribute("manifest", "versionName") + ".apk";
                        Regex rgx = new Regex(@"[\/:*?""<>|\s]");
                        if (FolderPath == null || FolderPath == "") {
                            newfilename = Path.GetDirectoryName(path) + @"\" + rgx.Replace(newfilename, "_");
                        } else {
                            newfilename = FolderPath + @"\" + rgx.Replace(newfilename, "_");
                        }

                        File.Move(path, newfilename);
                    } catch (Exception e) {
                        Log(e.Message);
                    }

                }
            }
        }

        private void gotoGooglePlay() {
            foreach (string path in SelectedItemPaths) {
                ApkQuickReader reader = new ApkQuickReader(path);
                string package = reader.getAttribute("manifest", "package");
                System.Diagnostics.Process.Start("https://play.google.com/store/apps/details?id=" + package);
            }
        }

        private void gotoGitHub() {
            System.Diagnostics.Process.Start("http://kkguo.github.io/apkshellext/index.html?version=" + GetType().Assembly.GetName().Version.ToString());
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

        private Bitmap betterIcon(Bitmap orignial, int iconSize) {
            // Get better image while stretch
            if (orignial != null) {
                Bitmap b = new Bitmap(iconSize, iconSize);
                using (Graphics g = Graphics.FromImage((Image)b)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(orignial, 0, 0, (int)iconSize, (int)iconSize);
                }
                return b;
            } else {
                return null;
            }
        }
    }
}
