using System;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpInfoTipHandler;

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
            ApkQuickReader reader = new ApkQuickReader(SelectedItemPath);
            string splitor = singleLine ? " " : Environment.NewLine;
            return "AppName : " + reader.getAttribute("application", "label") + splitor
                    + "Package : " + reader.getAttribute("manifest", "package") + splitor
                    + "VersionCode : " + reader.getAttribute("manifest", "versionCode") + splitor
                    + "VersionName : " + reader.getAttribute("manifest", "versionName");
        }
    }
}
