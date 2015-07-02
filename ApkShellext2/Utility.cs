using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;
using SharpShell.Diagnostics;

namespace ApkShellext2 {
    public static class Utility {
        /// <summary>
        /// Notifies the system of an event that an application has performed. An application should use this function if it performs an action that may affect the Shell.
        /// </summary>
        /// <param name="wEventId">Describes the event that has occurred. Typically, only one event is specified at a time. If more than one event is specified, the values contained in the dwItem1 and dwItem2 parameters must be the same, respectively, for all specified events. This parameter can be one or more of the following values:</param>
        /// <param name="uFlags">Flags that, when combined bitwise with SHCNF_TYPE, indicate the meaning of the dwItem1 and dwItem2 parameters. The uFlags parameter must be one of the following values.</param>
        /// <param name="dwItem1">Optional. First event-dependent value.</param>
        /// <param name="dwItem2">Optional. Second event-dependent value.</param>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(Int32 wEventId, UInt32 uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public static Bitmap ResizeBitmap(Bitmap orignial, int iconSize) {
            // Get better image while stretch
            if (orignial != null) {
                Bitmap b = new Bitmap(iconSize, iconSize);
                using (Graphics g = Graphics.FromImage((Image)b)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(orignial, 0, 0, (int)iconSize, (int)iconSize);
                }
                return b;
            } else {
                return null;
            }
        }

        public static void setRegistrySetting(string key, int value) {
            try {
                using (RegistryKey k = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ApkShellext2")) {
                    k.SetValue(key, value);
                }
            } catch (Exception ex) {
                Logging.Log("Error happens during write settings :" + ex.Message);
            }
        }

        public static int getRegistrySetting(string key, int defValue=0) {
            try {
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ApkShellext2")) {
                    return (int)k.GetValue(key, (object)defValue);
                }
            } catch (Exception ex) {                
                Logging.Log("Error happens during reading settings :" + ex.Message);
                return defValue;
            }

        }

    }
}
