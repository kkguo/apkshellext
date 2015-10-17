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

namespace ApkShellext2 {
    [Guid("d747c5a7-2f66-4b7d-8301-8531838e4ed3")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".apk")]
    public class ApkThumbnailHandler : SharpThumbnailHandler {
        protected override Bitmap GetThumbnailImage(uint width) {
            Bitmap m_icon = null;
            try {
                using (AppPackageReader reader = AppPackageReader.Read(SelectedItemStream, AppPackageReader.AppType.AndroidApp)) {
                    reader.setFlags("IconSize", width);
                    m_icon = reader.Icon;
                    if (m_icon == null)
                        throw new Exception("Cannot find Icon from Stream, draw default");
                }
                if (Settings.Default.ShowOverLayIcon)
                    m_icon = Utility.CombineBitmap(m_icon,
                           Utility.AppTypeIcon(AppPackageReader.AppType.AndroidApp),
                           new Rectangle((int)(m_icon.Width * 0.05), 0, (int)(m_icon.Width * 0.95), (int)(m_icon.Height * 0.95)),
                           new Rectangle(0, (int)m_icon.Height / 2, (int)m_icon.Width / 2, (int)m_icon.Height / 2),
                           new Size((int)m_icon.Width, (int)m_icon.Height));
            } catch {
                Log("Error in reading icon from stream, draw default");
                // read error, draw the default icon
                m_icon = Utility.AppTypeIcon(AppPackageReader.AppType.AndroidApp);
            }

            return m_icon;
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
    }
}
