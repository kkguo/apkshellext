using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.IO;
using System.Text.RegularExpressions;


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
                Image = new Bitmap(Properties.Resources.logo, 16, 16)
            };

            var renameMenu = new ToolStripMenuItem {
                Text = "Rename with version"
            };

            renameMenu.Click += (sender, args) => renameWithVersion();

            var playMenu = new ToolStripMenuItem {
                Text = "Check GooglePlay",
                Image = new Bitmap(Properties.Resources.gplay, 16, 16)
            };

            playMenu.Click += (sender, args) => gotoGooglePlay();

            var checkUpdateMenu = new ToolStripMenuItem {
                Text = "About ApkShellext",
                Image = mainMenu.Image
            };

            checkUpdateMenu.Click += (sender, args) => gotoGitHub();

            mainMenu.DropDownItems.Add(renameMenu);
            if (SelectedItemPaths.Count() == 1) {
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
            System.Diagnostics.Process.Start("http://kkguo.github.io/apkshellext");
        }
    }
}
