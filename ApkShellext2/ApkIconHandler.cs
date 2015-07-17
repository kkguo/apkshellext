using ApkQuickReader;
using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.Extensions;
using SharpShell.ServerRegistration;
using SharpShell.SharpIconHandler;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace ApkShellext2{

    [Guid("1F869CEE-4FDA-35D9-896F-43975A87D1F6")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension,".apk")]
    public class ApkIconHandler : SharpIconHandler {
        private Bitmap m_icon = null;

        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            if (smallIcon) { // this place save one time read zip package
                try {
                    return addOverlay((int)iconSize);
                } finally {
                    if (m_icon != null) {
                        m_icon.Dispose();
                        m_icon = null;
                    }
                }
            }

            try {
                ApkReader reader = new ApkReader(SelectedItemPath);
                m_icon = reader.getImage("application", "icon");
            } catch {
                // read error, draw the default icon
                m_icon = new Bitmap((int)iconSize, (int)iconSize);
                using (Graphics g = Graphics.FromImage(m_icon)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;   
                    g.FillRectangle(Brushes.White, 0, 0, iconSize, iconSize);
                    Rectangle rec = new Rectangle();                    
                    if (Properties.Resources.androidHead.Width > Properties.Resources.androidHead.Height) {
                        rec.Width = (int)iconSize;
                        rec.Height = (int)(Properties.Resources.androidHead.Height * iconSize / Properties.Resources.androidHead.Width);
                        rec.X = 0;
                        rec.Y = (int)(iconSize - rec.Height) / 2;
                    } else {
                        rec.Width = (int)(Properties.Resources.androidHead.Width * iconSize /
                            Properties.Resources.androidHead.Height);
                        rec.Height = (int)iconSize;
                        rec.X = (int)(iconSize - rec.Width) / 2;
                        rec.Y = 0;
                    }
                    g.DrawImage(Properties.Resources.androidHead, rec);
                }
            }
            return addOverlay((int)iconSize);
        }

        private Icon addOverlay(int iconSize) {
            using (Bitmap b = new Bitmap(iconSize, iconSize)) {
                using (Graphics g = Graphics.FromImage(b)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    if (Utility.getRegistrySetting(Utility.keyShowOverlay) == 1) {
                        g.DrawImage(m_icon, (int)(iconSize * 0.05), 0, (int)(iconSize * 0.95), (int)(iconSize * 0.95));
                        int overlaySize = (int)(iconSize * 0.6);
                        overlaySize = (overlaySize >= 64) ? 64 : overlaySize;
                        int targetW = overlaySize;
                        targetW = (targetW >= 16) ? targetW : 16;
                        int targetH = Properties.Resources.androidHead.Height * targetW / Properties.Resources.androidHead.Width;
                        int targetY = iconSize - targetH;
                        g.DrawImage(Properties.Resources.androidHead, 0, targetY, targetW, targetH);
                    } else {
                        g.DrawImage(m_icon, 0, 0, iconSize, iconSize);
                    }
                    return Icon.FromHandle(b.GetHicon());
                }
            }
        }

        [CustomRegisterFunction]
        public static void postDoRegister(Type type, RegistrationType registrationType) {
            Console.WriteLine("Registering " + type.FullName);

            #region Clean up apkshellext registry
            try {
                string oldVersionClassName = @"KKHomeProj.ApkShellExt.ApkShellExt";
                string oldVersionCLID = "";
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(oldVersionClassName + @"\CLSID")) {
                    if (key != null) {
                        Console.WriteLine("Found old version in registry, cleaning up ...");
                        oldVersionCLID = (string)key.GetValue(null);
                        key.Close();
                        Registry.ClassesRoot.DeleteSubKeyTree(oldVersionClassName);
                    }
                }
                if (oldVersionCLID != "") {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"\SOFTWARE\Classes\CLSID\" + oldVersionCLID)) {
                        if (key != null) {
                            key.Close();
                            Registry.LocalMachine.DeleteSubKeyTree(@"\SOFTWARE\Classes\CLSID\" + oldVersionCLID);
                        }
                    }
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\" + oldVersionCLID)) {
                        if (key != null) {
                            key.Close();
                            Registry.LocalMachine.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\" + oldVersionCLID);
                        }
                    }
                }
            } catch (Exception e){
                Logging.Error("Cleaning up apkshellext remaining registry items but see exception" + 
                    e.Message);
            }
            #endregion

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
            Logging.Log(Path.GetFileName(SelectedItemPath) + "[" + DateTime.Now.ToString() + "]" + message);
        }
    }
}
