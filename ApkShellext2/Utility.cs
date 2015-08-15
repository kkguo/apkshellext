using Microsoft.Win32;
using SharpShell.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace ApkShellext2 {
    public static class Utility {
        /// <summary>
        /// resize bitmap with high quality
        /// </summary>
        /// <param name="original"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap original, Size size) {
            // Get better image while stretch
            if (original != null) {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b)) {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    g.DrawImage(original, 0, 0, size.Width, size.Height);
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
        public static Bitmap ResizeBitmap(Bitmap original, int width) {
            return ResizeBitmap(original, new Size(width, width));
        }

        # region set/get registry settings
        public static readonly string keyAlwaysShowGooglePlay = @"AlwaysShowGooglePlay";
        public static readonly string keyRenameWithVersionCode = @"RenameWithVersionCode";
        public static readonly string keyShowOverlay = @"ShowOverLayIcon";
        public static readonly string keyShowIpaIcon = @"ShowIpaIcon";
        public static readonly string keyShowAppxIcon = @"ShowAppxIcon";

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

        public static void setRegistrySettingString(string key, string value) {
            try {
                string assembly_name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                using (RegistryKey k = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + assembly_name)) {
                    k.SetValue(key, value);
                }
            } catch (Exception ex) {
                Logging.Log("Error happens during write settings :" + ex.Message);
            }
        }

        public static int getRegistrySetting(string key, int defValue = 0) {
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

        public static string getRegistrySettingString(string key, string defValue = "") {
            try {
                string assembly_name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + assembly_name)) {
                    return k.GetValue(key, (object)defValue) as string;
                }
            } catch (Exception ex) {
                Logging.Log("Error happens during reading settings :" + ex.Message);
                return defValue;
            }
        }
        # endregion

        #region Resolve resource dll by internal resource
        // buffer for loaded resource dll;
        private static int _bufCultureInfoLCID;
        private static byte[] _binResourceDll;

        /// <summary>
        /// This hook up is trying to resolve the resource dll which is including inside 
        /// the main assembly.
        /// The purpose is for keeping a singal dll release including language resource.
        /// The language resource dll will be generated once resouce file get updated,
        /// and will copied into Resources folder after first time build.
        /// So once any language resouce get changed, need 2 times build to get the new 
        /// resource dll included.
        /// </summary>
        public static void HookResolveResourceDll() {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResourceDllResolveEventHandler);
        }

        public static Assembly ResourceDllResolveEventHandler(object sender, ResolveEventArgs args) {
            AssemblyName MissingAssembly = new AssemblyName(args.Name);
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            if (_bufCultureInfoLCID != ci.LCID) {
                string resourceName = "ApkShellext2.Resources." + ci.Name.Replace("-", "_") + "." + MissingAssembly.Name + ".dll";
#if DEBUG
                Logging.Log("List of the resources:");
                foreach (var s in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                    Logging.Log(s);
                }
#endif
                Logging.Log("Extracting resource dll: " + resourceName);
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                    if (stream == null) return null;
                    _binResourceDll = new BinaryReader(stream).ReadBytes((int)stream.Length);
                    _bufCultureInfoLCID = ci.LCID;
                }
            }
            return Assembly.Load(_binResourceDll);
        }

        #endregion

        /// <summary>
        /// Load Resource Dll and set the culture info
        /// Resource Dll is buffered in static byte array in this class
        /// This is needed before any thread loading localize string
        /// </summary>
        public static void Localize() {
            HookResolveResourceDll();
            int lang = getRegistrySetting("language", -1);
            if (lang != -1) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
        }

        /// <summary>
        ///  Enumerate all language resouce dll, get culture code
        /// </summary>
        /// <returns></returns>
        public static CultureInfo[] getSupportedLanguages() {
            List<CultureInfo> result = new List<CultureInfo>();
            result.Add(new CultureInfo("en-US")); //default is en-US
            foreach (var s in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                Match m = Regex.Match(s, @"ApkShellext2\.Resources\.([a-z][a-z]_[A-Z][A-Z])\.ApkShellext2.resources.dll");
                if (m.Success) {
                    CultureInfo ci = new CultureInfo(m.Groups[1].Value.Replace("_", "-"));
                    result.Add(ci);
                }
            }
            return result.ToArray();
        }

        public static string getLatestVersion() {
            try {
                byte[] buf = new byte[1024];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Properties.Resources.urlGithubHomeLatest);
                // execute the request
                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();
                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();
                int count = resStream.Read(buf, 0, buf.Length);
                string s = "";
                if (count != 0) {
                    s = Encoding.ASCII.GetString(buf, 0, count);
                }
                s = Regex.Replace(s, @"\t|\n|\r", "");

                Logging.Log("Get the latest version :" + s);
                return s;
            } catch {
                return "0.0.0.0";
            }
        }

        // http://stackoverflow.com/questions/6803073/get-local-ip-address
        public static string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public static string GetMd5Hash(MD5 md5Hash, string input) {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}
