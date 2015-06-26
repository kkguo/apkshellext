using System.Drawing;
using System;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using SharpShell.Attributes;
using SharpShell.SharpIconHandler;
using SharpShell.SharpInfoTipHandler;
using SharpShell.Interop;
using SharpShell.ServerRegistration;
using SharpShell.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;
using System.Linq;

namespace KKHomeProj.ApkShellext2
{

    [Guid("1F869CEE-4FDA-35D9-896F-43975A87D1F6")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [COMServerAssociation(AssociationType.ClassOfExtension,".apk")]
    public class ApkShellext2 : SharpIconHandler, IQueryInfo
    {
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
            // Todo: 
            // clean up old version registry
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

            // Register InfoTip Handler
             var assocationAttributes = type.GetCustomAttributes(typeof(COMServerAssociationAttribute), true)
                .OfType<COMServerAssociationAttribute>().ToList();

                //  Register the server associations, if there are any.
            if (assocationAttributes.Any()) {
                ServerRegistrationManager.RegisterServerAssociations(type.GUID, SharpShell.ServerType.ShellInfoTipHandler, type.Name, assocationAttributes, registrationType);
            }

            // approve extension
            ServerRegistrationManager.ApproveExtension(type, Environment.Is64BitOperatingSystem ? RegistrationType.OS64Bit : RegistrationType.OS32Bit); 
            // Notify shell to refresh.
            Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Gets the info tip text for an item.
        /// </summary>
        /// <param name="dwFlags">Flags that direct the handling of the item from which you're retrieving the info tip text. This value is commonly zero.</param>
        /// <param name="ppwszTip">The address of a Unicode string pointer that, when this method returns successfully, receives the tip string pointer. Applications that implement this method must allocate memory for ppwszTip by calling CoTaskMemAlloc.
        /// Calling applications must call CoTaskMemFree to free the memory when it is no longer needed.</param>
        /// <returns>
        /// Returns S_OK if the function succeeds. If no info tip text is available, ppwszTip is set to NULL. Otherwise, returns a COM-defined error value.
        /// </returns>
        int IQueryInfo.GetInfoTip(QITIPF dwFlags, out string ppwszTip) {
            //  Do we need to get the tip on a single line?
            var singleLine = dwFlags.HasFlag(QITIPF.QITIPF_SINGLELINE);

            //  Now work out what type of info to get.
            RequestedInfoType infoType;
            if (dwFlags.HasFlag(QITIPF.QITIPF_USENAME))
                infoType = RequestedInfoType.Name;
            else if (dwFlags.HasFlag(QITIPF.QITIPF_LINKUSETARGET))
                infoType = RequestedInfoType.InfoOfShortcutTarget;
            else if (dwFlags.HasFlag(QITIPF.QITIPF_LINKNOTARGET))
                infoType = RequestedInfoType.InfoOfShortcut;
            else
                infoType = RequestedInfoType.InfoTip;

            try {
                //  Get the requested info.
                var infoTip = GetInfo(infoType, singleLine);

                //  Set the tip. The runtime marshals out strings with CoTaskMemAlloc for us.
                ppwszTip = infoTip;
            } catch (Exception exception) {
                //  DebugLog the exception.
                LogError("An exception occured getting the info tip.", exception);

                //  If an exception is thrown, we cannot return any info.
                ppwszTip = string.Empty;
            }

            //  Return success.
            return WinError.S_OK;
        }

        /// <summary>
        /// Gets the information flags for an item. This method is not currently used.
        /// </summary>
        /// <param name="pdwFlags">A pointer to a value that receives the flags for the item. If no flags are to be returned, this value should be set to zero.</param>
        /// <returns>
        /// Returns S_OK if pdwFlags returns any flag values, or a COM-defined error value otherwise.
        /// </returns>
        int IQueryInfo.GetInfoFlags(out int pdwFlags) {
            //  Currently, GetInfoFlags shouldn't be called by the system. Return success if it is.
            pdwFlags = 0;
            return WinError.S_OK;
        }

        /// <summary>
        /// Gets info for the selected item (SelectedItemPath).
        /// </summary>
        /// <param name="infoType">Type of info to return.</param>
        /// <param name="singleLine">if set to <c>true</c>, put the info in a single line.</param>
        /// <returns>
        /// Specified info for the selected file.
        /// </returns>
        protected string GetInfo(RequestedInfoType infoType, bool singleLine) {
            ApkQuickReader reader = new ApkQuickReader(SelectedItemPath);
            string splitor = singleLine ? " " : "\n";
            return    "AppName : " + reader.getAttribute("application", "label") + splitor
                    + "Package : " + reader.getAttribute("manifest", "package") + splitor
                    + "VersionCode : " + reader.getAttribute("manifest", "versionCode") + splitor
                    + "VersionName : " + reader.getAttribute("manifest", "versionName");                   
        }
    }
}
