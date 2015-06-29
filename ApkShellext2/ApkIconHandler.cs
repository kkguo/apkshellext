using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.Diagnostics;
using SharpShell.Extensions;
using SharpShell.Interop;
using SharpShell.ServerRegistration;
using SharpShell.SharpIconHandler;
using SharpShell.SharpInfoTipHandler;
using SharpShell.SharpContextMenu;
using SharpShell.Helpers;
using System.IO;
using System.Text.RegularExpressions;

namespace ApkShellext2
{

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
                    return betterIcon((int)iconSize);
                } finally {
                    if (m_icon != null) {
                        m_icon.Dispose();
                        m_icon = null;
                    }
                }
            }

            ApkQuickReader reader = new ApkQuickReader(SelectedItemPath);
            string iconPath = reader.getAttribute("application", "icon");
            m_icon = reader.getImage(iconPath);
            return betterIcon((int)iconSize);
        }

        private Icon betterIcon(int iconSize) {
            // Get better image while stretch
            if (m_icon != null) {
                Bitmap b = new Bitmap(iconSize, iconSize);
                using (Graphics g = Graphics.FromImage((Image)b)) {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(m_icon, 0, 0, (int)iconSize, (int)iconSize);
                }
                return Icon.FromHandle(b.GetHicon());
            } else {
                return null;
            }
        }

        [CustomRegisterFunction]
        public static void postDoRegister(Type type, RegistrationType registrationType) {
            Logging.Log("executing Post register function");
            
            #region Clean up old version registry
            string oldVersionClassName = @"KKHomeProj.ApkShellExt.ApkShellExt";
            string oldVersionCLID="";
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(oldVersionClassName+@"\CLSID")) {
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
            #endregion

            // Notify shell to refresh.
            SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        protected override void Log(string message) {
            Logging.Log(Path.GetFileName(SelectedItemPath) + ": " + message);
        }

        /// <summary>
        /// Notifies the system of an event that an application has performed. An application should use this function if it performs an action that may affect the Shell.
        /// </summary>
        /// <param name="wEventId">Describes the event that has occurred. Typically, only one event is specified at a time. If more than one event is specified, the values contained in the dwItem1 and dwItem2 parameters must be the same, respectively, for all specified events. This parameter can be one or more of the following values:</param>
        /// <param name="uFlags">Flags that, when combined bitwise with SHCNF_TYPE, indicate the meaning of the dwItem1 and dwItem2 parameters. The uFlags parameter must be one of the following values.</param>
        /// <param name="dwItem1">Optional. First event-dependent value.</param>
        /// <param name="dwItem2">Optional. Second event-dependent value.</param>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(Int32 wEventId, UInt32 uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
