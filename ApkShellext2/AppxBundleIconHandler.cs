using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpIconHandler;
using System.Drawing;
using Microsoft.Win32;
using SharpShell.ServerRegistration;
using SharpShell.Diagnostics;
using SharpShell.Extensions;
using System.IO;

namespace ApkShellext2 {
    [Guid("d5ff6172-1ae5-4c4a-a207-5a2dd100891e")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension,".appxbundle")]
    [COMServerAssociation(AssociationType.ClassOfExtension,".appx")]
    public class AppxBundleIconHandler : SharpIconHandler {
        private Bitmap m_icon = null;
        protected override Icon GetIcon(bool smallIcon, uint iconSize) {
            if (Utility.getRegistrySetting(Utility.keyShowAppxIcon, 100) != 1) {
                return null;
            }

            try {
                if (m_icon == null) {
                    if (SelectedItemPath.EndsWith(".appx")) {
                        AppxReader reader = new AppxReader(SelectedItemPath);
                        m_icon = reader.getLogo();
                        reader.Close();
                    } else { // appxbundle
                        AppxBundleReader reader = new AppxBundleReader(SelectedItemPath);
                        m_icon = reader.getLogo();
                        reader.Close();
                    }
                }
            } catch (Exception ex){
                Log(ex.Message);
                return null;
            }
            return Icon.FromHandle(m_icon.GetHicon());
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
            Logging.Log(Path.GetFileName(SelectedItemPath) + "[" + DateTime.Now.ToString() + "]" + message);
        }
    }
}
