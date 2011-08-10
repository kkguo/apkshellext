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
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
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
        private const int BUFF_SIZE = 1024;
        #endregion

        private string sFileName;

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
            ZipFile zip = null;

            try
            {
                ExtractResource(Properties.Resources.aapt, @"aapt.exe");
                ExtractResource(Properties.Resources.aapt, @"mgwz.dll");
                ExtractResource(Properties.Resources.adb, @"adb.exe");
                ExtractResource(Properties.Resources.adb, @"AdbWinApi.dll");

                Process p = new Process();
                p.StartInfo.FileName = Path.GetTempPath() + @"aapt.exe";
                p.StartInfo.Arguments = @"dump badging " + "\"" + sFileName + "\"";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WorkingDirectory = Path.GetTempPath();
                p.Start();
                string icon_path = p.StandardOutput.ReadLine();
                while (!String.IsNullOrEmpty(icon_path))
                {
                    if (icon_path.Contains("application:"))
                    {
                        int icon_ind = icon_path.IndexOf("icon=") + 5 + 1;//1 for "'"
                        icon_path = icon_path.Substring(icon_ind, icon_path.Length - icon_ind - 1);
                        break;
                    }
                    icon_path = p.StandardOutput.ReadLine();
                }
                p.Close();
                if (string.IsNullOrEmpty(icon_path))
                {
                    throw new Exception("Cannot find icon path!");
                }
                else
                {
                    zip = new ZipFile(sFileName);
                    bmp = (Bitmap)Bitmap.FromStream(zip.GetInputStream(zip.FindEntry(icon_path, true)));
                }
            }
            catch
            {
                //MessageBox.Show(e.Message);
                bmp = new Bitmap(Properties.Resources.deficon);
            }
            finally
            {
                if (zip != null)
                {
                    zip.Close();
                }
            }
            return Icon.FromHandle(bmp.GetHicon());
        }

        /// <summary>
        /// extract file from zipped resource, and place to temp folder
        /// </summary>
        /// <param name="resource">resource name</param>
        /// <param name="fileName">output name</param>
        /// <param name="OverWriteIfExists">if true,will overwrite the file even if the file exists</param>
        private static void ExtractResource(byte[] resource, string fileName,bool OverWriteIfExists=false)
        {
            string target = Path.GetTempPath() + fileName;
            if (OverWriteIfExists || !File.Exists(target))
            {
                ZipFile zip = null;
                FileStream fs = null;
                Stream inStream = null;
                try
                {
                    zip = new ZipFile(new MemoryStream(resource));
                    inStream = zip.GetInputStream(zip.GetEntry(fileName));
                    fs = new FileStream(target, FileMode.Create);
                    byte[] buff = new byte[BUFF_SIZE];
                    int read_count;
                    while ((read_count = inStream.Read(buff, 0, BUFF_SIZE)) > 0)
                    {
                        fs.Write(buff, 0, read_count);
                    }
                }
                catch
                { }
                finally
                {
                    if (zip != null)
                    {
                        zip.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                    if (inStream != null)
                    {
                        inStream.Close();
                        inStream.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// register this dll
        /// </summary>
        private static void RegApk()
        {
            RegistryKey root;
            RegistryKey rk;
            root = Registry.ClassesRoot;
            rk = root.CreateSubKey(@".apk");
            rk.SetValue("", "Android Package File");
            rk.Close();

            //rk = root.CreateSubKey(@".apk\DefaultIcon");
            //rk.SetValue("", "%1");
            //rk.Close();

            rk = root.CreateSubKey(@".apk\shellex\IconHandler");
            rk.SetValue("", GUID);
            rk.Close();

            //// for 64bit windows
            //rk = root.CreateSubKey(@"Wow6432Node\.apk");
            //rk.SetValue("", "Android Package File");
            //rk.Close();

            //rk = root.CreateSubKey(@"Wow6432Node\.apk\DefaultIcon");
            //rk.SetValue("", "%1");
            //rk.Close();

            //rk = root.CreateSubKey(@"Wow6432Node\.apk\shellex\IconHandler");
            //rk.SetValue("", GUID);
            //rk.Close();

            root.Close();

            ////////////////////////////////////////
            root = Registry.LocalMachine;
            try
            {
                rk = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                rk.SetValue(GUID, "Shell Extention for Android Package files");
                rk.Close();

                //rk = root.OpenSubKey(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                //rk.SetValue(GUID, "Android Package file, shell extention");
                //rk.Close();
            } catch { }
            root.Close();
            ExtractResource(Properties.Resources.aapt, @"aapt.exe",true);
            ExtractResource(Properties.Resources.aapt, @"mgwz.dll",true);
            ExtractResource(Properties.Resources.adb, @"adb.exe",true);
            ExtractResource(Properties.Resources.adb, @"AdbWinApi.dll",true);
        }

        /// <summary>
        /// unregister
        /// </summary>
        private static void UnregApk()
        {
            try{
                RegistryKey root;
                RegistryKey rk;
                root = Registry.ClassesRoot;
                root.DeleteSubKeyTree(@".apk");
                //root.DeleteSubKeyTree(@"Wow6432Node\.apk");
                root.Close();

                root = Registry.LocalMachine;
                rk = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                rk.DeleteValue(GUID);
                rk.Close();
                //rk = root.OpenSubKey(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                //rk.DeleteValue(GUID);
                //rk.Close();

                root.Close();
            } catch {}
        }

    }
}
// vim: expandtab tabstop=4 softtabstop=4 shiftwidth=4
