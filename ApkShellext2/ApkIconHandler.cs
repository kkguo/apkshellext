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
    /// <summary>
    /// This is the icon handler, not only supporting apk, but also other type of apps.
    /// </summary>
    [Guid("1F869CEE-4FDA-35D9-896F-43975A87D1F6")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".apk")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".ipa")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".appxbundle")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".appx")]
    public class ApkIconHandler : SharpIconHandler {
        private Bitmap m_icon = null;

        protected override Icon GetIcon(bool smallIcon, uint iconSize)
        {
            try {
                if (m_icon == null) {
                    using (AppPackageReader reader = AppPackageReader.Read(SelectedItemPath)) {
                        m_icon = reader.Icon;
                    }
                }
            } catch {
                Log("Error in reader icon, draw default");
                // read error, draw the default icon
                m_icon = Utility.ResizeBitmap(getOverlayIcon(SelectedItemPath), (int)iconSize);
                return Icon.FromHandle(m_icon.GetHicon());
            }

            if (Utility.getRegistrySetting(Utility.keyShowOverlay) == 1) {
                return addOverlay(iconSize);
            } else {
                return Icon.FromHandle(m_icon.GetHicon());
            }
        }

        // Overlay icon
        private Icon addOverlay(uint iconSize) {
            using (Bitmap b = new Bitmap((int)iconSize, (int)iconSize)) {
                using (Graphics g = Graphics.FromImage(b)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(m_icon, (int)(iconSize * 0.05), 0, (int)(iconSize * 0.95), (int)(iconSize * 0.95));
                    g.DrawImage(getOverlayIcon(SelectedItemPath), 0, iconSize/2,iconSize/2,iconSize/2);
                    return Icon.FromHandle(b.GetHicon());
                }
            }
        }

        private Bitmap getOverlayIcon(string path) {
            switch (AppPackageReader.getAppType(path)){
                case AppPackageReader.AppType.AndroidApp:
                    return Properties.Resources.Android;
                case AppPackageReader.AppType.iOSApp:
                    return Properties.Resources.Apple;
                case AppPackageReader.AppType.WindowsPhoneApp:
                case AppPackageReader.AppType.WindowsPhoneAppBundle:
                    return Properties.Resources.Windows;
                default:
                    return null;
            }
        }

        [CustomRegisterFunction]
        public static void postDoRegister(Type type, RegistrationType registrationType) {
            Console.WriteLine("Registering " + type.FullName + "Version" + type.Assembly.GetName().Version.ToString());

            // Todo: clean other icon handler as they were integrated
            //"d5ff6172-1ae5-4c4a-a207-5a2dd100891e"
            //"a0ac4758-12d3-4dcf-9d12-03faaa3c0a9d"

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

            #region enable log print when debug
//#if DEBUG
//            try {
//                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"\SOFTWARE\SharpShell")) {
//                    if (key != null) {
//                        key.SetValue("LoggingMode", 4);
//                        key.SetValue("LogPath", @"%temp%\apkshellext.log");
//                    }
//                }
//            } catch (Exception e){
//                Logging.Log("error in enable logging" + e.Message);
//            }
//#endif
            #endregion
        }

        protected override void Log(string message) {
            Logging.Log(Path.GetFileName(SelectedItemPath) + "[" + DateTime.Now.ToString() + "]" + message);
        }
    }
}
