using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.Extensions;
using SharpShell.ServerRegistration;
using SharpShell.SharpInfoTipHandler;
using System.IO;

namespace ApkShellext2 {
    [Guid("946435a5-fe96-416d-99db-e94ee9fb46c8")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".apk")]
    public class ApkInfoTipHandler : SharpInfoTipHandler {
        /// <summary>
        /// Gets info for the selected item (SelectedItemPath).
        /// </summary>
        /// <param name="infoType">Type of info to return.</param>
        /// <param name="singleLine">if set to <c>true</c>, put the info in a single line.</param>
        /// <returns>
        /// Specified info for the selected file.
        /// </returns>
        protected override string GetInfo(RequestedInfoType infoType, bool singleLine) {
            try {
                ApkQuickReader reader = new ApkQuickReader(SelectedItemPath);
                string splitor = singleLine ? " " : Environment.NewLine;
                return "AppName : " + reader.getAttribute("application", "label") + splitor
                        + "Package : " + reader.getAttribute("manifest", "package") + splitor
                        + "VersionCode : " + reader.getAttribute("manifest", "versionCode") + splitor
                        + "VersionName : " + reader.getAttribute("manifest", "versionName");
            } catch (Exception ex) {
                Log("Error happend during GetInfo : " + ex.Message);
                return "apk file is broken";
            }
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

        protected override void Log(string message)
        {
            Logging.Log(Path.GetFileName(SelectedItemPath) + "[" + DateTime.Now.ToString() + "]" + message);
        }
    }
}
