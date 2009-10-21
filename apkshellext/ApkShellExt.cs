/*
 * 
 * 
 * Reference to : lc_mtt's blog http://blog.csdn.net/lc_mtt
 * 
 */
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using KKHomeProj.ShellExtInts;
using Microsoft.CSharp;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace KKHomeProj.ApkShellExt
{
    [Guid("66391a18-f480-413b-9592-a10044de6cf4"),
    ComVisible(true)]
    public class ApkShellExt : IExtractIcon, IPersistFile
    {
        #region Constants
        private const string GUID = "{66391a18-f480-413b-9592-a10044de6cf4}";
        private const string KeyName = "apkshellext";
        private const int S_OK = 0;
        private const int S_FALSE = 1;
        private const uint E_PENDING = 0x8000000A;
        private const uint E_NOTIMPL = 0x80004001;
        #endregion

        private string sFileName;
        private FileStream fs;

        #region IPersistFile 成员

        public uint GetClassID(out Guid pClassID)
        {
            pClassID = new Guid(GUID);
            return S_OK;
        }

        public uint IsDirty()
        {
            throw new NotImplementedException();
        }

        public uint Load(string pszFileName, uint dwMode)
        {
            sFileName = pszFileName;
            return S_OK;
        }

        public uint Save(string pszFileName, bool fRemember)
        {
            throw new NotImplementedException();
        }

        public uint SaveCompleted(string pszFileName)
        {
            throw new NotImplementedException();
        }

        public uint GetCurFile(out string ppszFileName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExtractIcon 成员

        public uint GetIconLocation(ExtractIconOptions uFlags, IntPtr szIconFile, uint cchMax, out int piIndex, out ExtractIconFlags pwFlags)
        {
            piIndex = -1;
            szIconFile = IntPtr.Zero;
            try
            {
                pwFlags = ExtractIconFlags.NotFilename | ExtractIconFlags.PerInstance | ExtractIconFlags.DontCache;
                return ((uFlags & ExtractIconOptions.Async) != 0) ? E_PENDING : S_OK;
            }
            catch
            {
                pwFlags = ExtractIconFlags.None;
                return S_FALSE;
            }
        }

        public uint Extract(string pszFile, uint nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, uint nIconSize)
        {
            try
            {
                Icon ico = GetApkIcon();   
                int s_size = (int)nIconSize >> 16;
                int l_size = (int)nIconSize & 0xffff;
                phiconLarge = (new Icon(ico,l_size,l_size)).Handle;
                phiconSmall = (new Icon(ico,s_size,s_size)).Handle;
                return S_OK;
            }
            catch
            {
                phiconLarge = phiconSmall = IntPtr.Zero;
                return S_FALSE;
            }
        }

        #endregion

        #region Registeration
        [System.Runtime.InteropServices.ComRegisterFunctionAttribute()]
        static void RegisterServer(String str1)
        {
            try
            {
                //register this dll
                RegistryKey root;
                RegistryKey rk;
                root = Registry.LocalMachine;
                rk = root.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Approved", true);
                rk.SetValue(GUID, KeyName);
                rk.Close();
                root.Close();

                //Register APK file type
                RegApk();
            }
            catch
            {
            }
        }

        [System.Runtime.InteropServices.ComUnregisterFunctionAttribute()]
        static void UnregisterServer(String str1)
        {
            try
            {
                //unregister dll
                RegistryKey root;
                RegistryKey rk;
                root = Registry.LocalMachine;
                rk = root.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Approved", true);
                rk.DeleteValue(GUID);
                rk.Close();
                root.Close();

                //unregister file type association
                UnregApk();
            }
            catch
            {
            }
        }

        #endregion

        /// <summary>
        /// Get Icon from current APK package
        /// </summary>
        /// <returns>Icon object</returns>
        private Icon GetApkIcon()
        {       
            Bitmap bmp = null;
            string icon_path;
            try
            {
                string aapt = Path.GetTempPath() + @"aapt.exe";
                if (!File.Exists(aapt))
                {
                    File.WriteAllBytes(aapt, Properties.Resources.aapt);
                }
                string mgwz = Path.GetTempPath() + @"mgwz.dll";
                if (!File.Exists(mgwz))
                {
                    File.WriteAllBytes(mgwz, Properties.Resources.mgwz);
                }
                Process p = new Process();
                p.StartInfo.FileName = aapt;
                p.StartInfo.Arguments = @"dump badging " + sFileName;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WorkingDirectory = Path.GetTempPath();
                p.Start();
                icon_path = p.StandardOutput.ReadLine();
                while (icon_path != string.Empty)
                {
                    icon_path = p.StandardOutput.ReadLine();
                    if (icon_path.Contains("application:"))
                    {
                        int icon_ind = icon_path.IndexOf("icon=") + 5 + 1;//1 for "'"
                        icon_path = icon_path.Substring(icon_ind, icon_path.Length - icon_ind - 1);
                        break;
                    }
                }
                if (string.IsNullOrEmpty(icon_path))
                {
                    throw new Exception("Cannot find icon path!");
                }
                fs = new FileStream(sFileName, FileMode.Open);
                ZipFile zip = new ZipFile(fs);
                bmp = (Bitmap)Bitmap.FromStream(zip.GetInputStream(zip.FindEntry(icon_path, false)));
                fs.Close();

            }
            catch
            {
                if (fs != null)
                {
                    fs.Close();
                }
                bmp = new Bitmap(Properties.Resources.deficon);
            }
            return Icon.FromHandle(bmp.GetHicon());                        
        }

        private static void RegApk()
        {
            RegistryKey root;
            RegistryKey rk;

            root = Registry.ClassesRoot;
            rk = root.CreateSubKey(".apk");
            rk.Close();

            rk = root.CreateSubKey(@".apk\DefaultIcon");
            rk.SetValue("", "%1");
            rk.Close();

            rk = root.CreateSubKey(@".apk\shellex\IconHandler");
            rk.SetValue("", GUID);
            rk.Close();
        }

        private static void UnregApk()
        {
            RegistryKey root;

            root = Registry.ClassesRoot;
            root.DeleteSubKey(".apk");
        }
    }
}
