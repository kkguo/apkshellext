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
using SharpShell.Extensions;
using SharpShell.Interop;
using ApkShellext2.Properties;

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

        #region set/get registry settings
        public const bool UseRegisteryForSettings = true;

        //public const string keyLanguage = @"language";
        //public const string keyMultiSelectShowStore = @"AlwaysShowGooglePlay";
        //public const string keyShowGooglePlay = @"ShowGooglePlay";
        //public const string keyShowAmazonStore = @"ShowAmazonStore";
        //public const string keyShowApkMirror = @"ShowApkMirror";
        //public const string keyShowAppleStore = @"ShowAppleStore";
        //public const string keyShowMSStore = @"ShowMSStore";
        //public const string keyRenameWithVersionCode = @"RenameWithVersionCode";
        //public const string keyShowOverlay = @"ShowOverLayIcon";
        //public const string keyShowIpaIcon = @"ShowIpaIcon";
        //public const string keyShowAppxIcon = @"ShowAppxIcon";
        //public const string keyShowMenuIcon = @"ShowMenuIcon";
        //public const string keyRenamePattern = @"RenamePattern";
        //public const string keyToolTipPattern = @"ToolTipPattern";
        //public const string keyEnableThumbnail = @"EnableThumbnail";
        //public const string keyStretchThumbnail = @"StretchThumbnail";
        //public const string keyLastCheckUpdateTime = @"LastCheckUpdateTime";

        public static void SetRegistrySetting(string key, int value) {
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
                string assembly_name = Assembly.GetExecutingAssembly().GetName().Name;
                using (RegistryKey k = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + assembly_name)) {
                    k.SetValue(key, value);
                }
            } catch (Exception ex) {
                Logging.Log("Error happens during write settings :" + ex.Message);
            }
        }

        static object getRegisterySetting(string key, object defValue) {
            string assembly_name = Assembly.GetExecutingAssembly().GetName().Name;
            try {
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + assembly_name))
                    return k.GetValue(key, defValue);
            }catch (Exception ex) {
                Logging.Log("Error happens during reading settings :" + ex.Message);
                return defValue;
            }
        }

        public static int getRegistrySetting(string key, int defValue = 0) {
            return (int)getRegisterySetting(key,defValue);
        }

        public static string getRegistrySettingString(string key, string defValue = "") {
            return (string)getRegisterySetting(key,defValue);
        }

#pragma warning disable CS0162 // Unreachable code detected
        public static string GetSetting(string key) {
            if (UseRegisteryForSettings) {
                return getRegisterySetting(key, "").ToString();
            } else {
                return Settings.Default[key].ToString();
            }
        }

        public static void SaveSetting(string key, object value) {
            if (UseRegisteryForSettings) {
                setRegistrySettingString(key, value.ToString());
            } else {
                Settings.Default[key] = value;
                Settings.Default.Save();
            }
        }
#pragma warning restore CS0162 // Unreachable code detected
        #endregion

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
            Log(null, "Localize", "Resolve resource dll " + args.Name);
            AssemblyName MissingAssembly = new AssemblyName(args.Name);
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            if (_bufCultureInfoLCID != ci.LCID) {
                string resourceName = "ApkShellext2.Resources." + ci.Name.Replace("-", "_") + "." + MissingAssembly.Name + ".dll";

                Log(null, "Localize", "Extracting resource dll: " + resourceName);
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                    if (stream == null)
                        return null;
                    _binResourceDll = new BinaryReader(stream).ReadBytes((int)stream.Length);
                    _bufCultureInfoLCID = ci.LCID;
                }
            }
            Log(null, "Localize", "Loading culture dll with " + new CultureInfo(_bufCultureInfoLCID).DisplayName);
            return Assembly.Load(_binResourceDll);
        }

        #endregion

        /// <summary>
        /// Load Resource Dll and set the culture info
        /// Resource Dll is buffered in static byte array in this class
        /// This is needed before any thread loading localize string
        /// </summary>
        public static void Localize() {
            //HookResolveResourceDll();
            int lang = Int16.Parse(Utility.GetSetting("Language"));
            if (lang != -1) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                Log(null, "Localize", "Set current Thread culture to " + Thread.CurrentThread.CurrentCulture.DisplayName);
            }
        }

        /// <summary>
        ///  Enumerate satallite dll folders, get lang code
        /// </summary>
        /// <returns></returns>
        public static CultureInfo[] getSupportedLanguages() {
            List<CultureInfo> result = new List<CultureInfo>();
            result.Add(new CultureInfo("en-US")); //default is en-US
            DirectoryInfo dir = new DirectoryInfo(getInstallPath());
            DirectoryInfo[] sub = dir.GetDirectories("??-??");
            foreach (var d in sub) {
                FileInfo[] f = d.GetFiles("ApkShellext2.resources.dll");
                if (f.Length == 1)
                    result.Add(new CultureInfo(d.Name));
            }
            return result.ToArray();
        }

        public static string getInstallPath() {
            string codebase = new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath;
            return Path.GetDirectoryName(codebase);
        }

        public static void getLatestVersion() {
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
                Utility.SaveSetting("LatestVersion", s);
                Log(null, "Update", "Get the latest version :" + s);
            } catch (Exception ex) {
                Log(null, "Update", "Error During check update:" + ex.Message);
            }
        }

        public static bool NewVersionAvailible() {
            string[] latestV = Utility.GetSetting("LatestVersion").Split(new Char[] { '.' });
            if (latestV.Length != 4)
                return false;
            string[] curV = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split(new Char[] { '.' });
            // version number should be always 4 parts
            for (int i = 0; i < latestV.Length; i++) {
                if (latestV[i] != curV[i]) {
                    if (int.Parse(latestV[i]) > int.Parse(curV[i]))
                        return true;
                    break;
                }
            }
            return false;
        }

        // http://stackoverflow.com/questions/6803073/get-local-ip-address
        //public static string LocalIPAddress() {
        //    IPHostEntry host;
        //    string localIP = "";
        //    host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList) {
        //        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
        //            localIP = ip.ToString();
        //            break;
        //        }
        //    }
        //    return localIP;
        //}

        //public static string GetMd5Hash(MD5 md5Hash, string input) {

        //    // Convert the input string to a byte array and compute the hash. 
        //    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        //    // Create a new Stringbuilder to collect the bytes 
        //    // and create a string.
        //    StringBuilder sBuilder = new StringBuilder();

        //    // Loop through each byte of the hashed data  
        //    // and format each one as a hexadecimal string. 
        //    for (int i = 0; i < data.Length; i++) {
        //        sBuilder.Append(data[i].ToString("x2"));
        //    }

        //    // Return the hexadecimal string. 
        //    return sBuilder.ToString();
        //}

        public static Bitmap AppTypeIcon(AppPackageReader.AppType type) {
            switch (type) {
                case AppPackageReader.AppType.AndroidApp:
                    return Properties.Resources.iconAndroid;
                case AppPackageReader.AppType.iOSApp:
                    return Properties.Resources.iconApple;
                case AppPackageReader.AppType.WindowsPhoneApp:
                case AppPackageReader.AppType.WindowsPhoneAppBundle:
                    return Properties.Resources.Windows;
                default:
                    return null;
            }
        }

        // Overlay icon
        public static Bitmap CombineBitmap(Bitmap backgroud, Bitmap foreground, Rectangle backgroundPos, Rectangle foregroundPos, Size outputSize) {
            Bitmap b = new Bitmap(outputSize.Width, outputSize.Height);
            using (Graphics g = Graphics.FromImage(b)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(backgroud, backgroundPos);
                g.DrawImage(foreground, foregroundPos);
                return b;
            }
        }

        public static string getFileSize(string path) {
            float filesize = new FileInfo(path).Length;
            if (filesize < 1024 * 1024) {  // < 1M
                return string.Format("{0:0.###}K", filesize / 1024);
            } else {
                return string.Format("{0:0.###}M", filesize / 1024 / 1024);
            }
        }

        //public static string[] getSelectedFiles() {
        //    IntPtr handle = User32.GetForegroundWindow();

        //    List<string> selected = new List<string>();

        //    var shell = new Shell32.Shell();
        //    foreach (SHDocVw.InternetExplorer window in shell.Windows()) {
        //        if (window.HWND == (int)handle) {
        //            Shell32.FolderItems items = ((Shell32.IShellFolderViewDual2)window.Document).SelectedItems();
        //            foreach (Shell32.FolderItem item in items) {
        //                selected.Add(item.Path);
        //            }
        //        }
        //    }
        //}

        public static void refreshShell() {
            SharpShell.Interop.Shell32.SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        public static void Log(object caller, string title, string message) {
#if DEBUG
            string output;
            output = "[" + System.DateTime.Now.ToString() + "]";
            if (caller != null)
                output += "<" + caller.GetType().Name + ">";
            if (title != "")
                output += "|" + title + "|";
            SharpShell.Diagnostics.Logging.Log(output + message);
#endif
        }
    }
}
