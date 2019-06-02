using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.SharpThumbnailHandler;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using SharpShell.Extensions;
using SharpShell.ServerRegistration;
using System.IO;
using ApkShellext2.Properties;
using ApkQuickReader;
using System.Configuration;

namespace ApkShellext2 {
    [Guid("d747c5a7-2f66-4b7d-8301-8531838e4ed3")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".apk")]
    public class ApkThumbnailHandler : SharpThumbnailHandler {
        protected override Bitmap GetThumbnailImage(uint width) {
            Bitmap m_icon = null;

            Log("Thumbnail is using setting file from: " + ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

            //if (!Settings.Default.EnableThumbnail) {
            //    return null;
            //}
            if (Utility.getRegistrySetting("EnableThumbnail") == 0) {
                return null;
            }

            try {
                int outputSize = (int) width;
                using (AppPackageReader reader = new ApkReader(SelectedItemStream)) {
                    Log("Reading stream from " + reader.AppName);
                    m_icon = reader.Icon;
                }

                if (m_icon == null)
                    throw new Exception("Cannot find Icon from Stream, draw default");
                if (m_icon.Height < outputSize &&
                    //!Settings.Default.StretchThumbnail)
                    (Utility.getRegistrySetting("StretchThumbnail", 1) ==0))
                    outputSize = m_icon.Height;

                Log("Got icon, resizing...");
                //if (Settings.Default.ShowOverLayIcon) {
                if (Utility.getRegistrySetting("ShowOverlayIcon")==1) {
                    Log("Draw overlay");
                    m_icon = Utility.CombineBitmap(m_icon,
                           Utility.AppTypeIcon(AppPackageReader.AppType.AndroidApp),
                           new Rectangle(0, 0, outputSize , outputSize),
                           new Rectangle(0, (int)outputSize / 2, (int)outputSize / 2, (int)outputSize / 2),
                           new Size(outputSize, outputSize));
                } else {
                    m_icon = Utility.ResizeBitmap(m_icon, outputSize);
                }
#if DEBUG
                string p = Path.GetTempFileName();
                m_icon.Save(p);
                Log("Save m_icon to " + p);
#endif
                return m_icon;
            } catch (Exception ex){
                Log("Error in reading icon from stream, draw default");
                Log(ex.Message);
                // read error, draw the default icon
                //m_icon = Utility.AppTypeIcon(AppPackageReader.AppType.AndroidApp);
                return null;
            }
        }

        [CustomRegisterFunction]
        public static void postDoRegister(Type type, RegistrationType registrationType) {
            Console.WriteLine("Registering " + type.FullName );

            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"\CLSID\.apk")) {
                if (key != null) {
                    // disable the shadow under thumbnail, make it more looks like an icon
                    key.SetValue("Treatment", 0);
                }
            }

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
            Utility.Log(this, "", message);
        }
    }
}
