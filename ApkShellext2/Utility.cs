using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;
using SharpShell.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Threading;

namespace ApkShellext2 {
    public static class Utility {
        // Culture Name list:
        //http://timtrott.co.uk/culture-codes/
        public static CultureInfo[] SupportedLanguages = new CultureInfo[] {
            new CultureInfo("en-US"),
            new CultureInfo("zh-CN"),
        };

        /// <summary>
        /// resize bitmap
        /// </summary>
        /// <param name="orignial"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap orignial, int width, int height) {
            // Get better image while stretch
            if (orignial != null) {
                Bitmap b = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage((Image)b)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(orignial, 0, 0, (int)width, (int)height);
                }
                return b;
            } else {
                return null;
            }
        }

        /// <summary>
        /// resize a rectangle bitmap
        /// </summary>
        /// <param name="orignial"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap orignial, int width) {
            return ResizeBitmap(orignial, width, width);
        }

        public static void setRegistrySetting(string key, int value) {
            try {
                string assembly_name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                using (RegistryKey k = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + assembly_name)) {
                    k.SetValue(key, value);
                }
            } catch (Exception ex) {
                Logging.Log("Error happens during write settings :" + ex.Message);
            }
        }

        public static int getRegistrySetting(string key, int defValue=0) {
            try {
                string assembly_name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + assembly_name)) {
                    return (int)k.GetValue(key, (object)defValue);
                }
            } catch (Exception ex) {                
                Logging.Log("Error happens during reading settings :" + ex.Message);
                return defValue;
            }

        }

        public static void HookResolveResourceDll() {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResourceDllResolveEventHandler); 
        }

        public static Assembly ResourceDllResolveEventHandler(object sender, ResolveEventArgs args) {
            AssemblyName MissingAssembly = new AssemblyName(args.Name);
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            string resourceName = "ApkShellext2.Resources." + ci.Name.Replace("-","_") + "." + MissingAssembly.Name + ".dll";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null) return null;
            return Assembly.Load(new BinaryReader(stream).ReadBytes((int)stream.Length));
        }
    }
}
