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
            const int BUFF_SIZE = 1024;
            Bitmap bmp = null;
            string icon_path;
            ZipFile zip = null;
            FileStream fs = null;

            try
            {
                string aapt = Path.GetTempPath() + @"aapt.exe";
                string mgwz = Path.GetTempPath() + @"mgwz.dll";
                bool extract_aapt = !File.Exists(aapt);
                bool extract_mgwz = !File.Exists(mgwz);

                //extract aapt.exe and mgwz.dll
                if (extract_aapt | extract_mgwz)
                {
                    Stream inStream=null;
                    int read_count;

                    try
                    {
                        zip = new ZipFile(new MemoryStream(Properties.Resources.aapt));
                        byte[] buff = new byte[BUFF_SIZE];
                        if (extract_aapt)
                        {
                            inStream = zip.GetInputStream(zip.GetEntry(@"aapt.exe"));
                            fs = new FileStream(aapt, FileMode.Create);
                            while ((read_count = inStream.Read(buff, 0, BUFF_SIZE)) > 0)
                            {
                                fs.Write(buff, 0, read_count);
                            }
                            inStream.Close();
                            fs.Close();
                            inStream.Dispose();
                            fs.Dispose();
                        };
                        if (extract_mgwz)
                        {
                            inStream = zip.GetInputStream(zip.GetEntry(@"mgwz.dll"));
                            fs = new FileStream(mgwz, FileMode.Create);
                            while ((read_count = inStream.Read(buff, 0, BUFF_SIZE)) > 0)
                            {
                                fs.Write(buff, 0, read_count);
                            }
                            inStream.Close();
                            fs.Close();
                            inStream.Dispose();
                            fs.Dispose();
                        };
                        zip.Close();
                    }
                    catch (ICSharpCode.SharpZipLib.Zip.ZipException e)
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
                Process p = new Process();
                p.StartInfo.FileName = aapt;
                p.StartInfo.Arguments = @"dump badging " + "\"" + sFileName + "\"";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WorkingDirectory = Path.GetTempPath();
                p.Start();
                icon_path = p.StandardOutput.ReadLine();
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
                zip = new ZipFile(sFileName);
                bmp = (Bitmap)Bitmap.FromStream(zip.GetInputStream(zip.FindEntry(icon_path, true)));
                zip.Close();
            } catch {
                if (zip != null) {
                    zip.Close();
                }
                //MessageBox.Show(e.Message);
                bmp = new Bitmap(Properties.Resources.deficon);
            }
            return Icon.FromHandle(bmp.GetHicon());
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

            rk = root.CreateSubKey(@".apk\DefaultIcon");
            rk.SetValue("", "%1");
            rk.Close();

            rk = root.CreateSubKey(@".apk\shellex\IconHandler");
            rk.SetValue("", GUID);
            rk.Close();

            // for 64bit windows
            rk = root.CreateSubKey(@"Wow6432Node\.apk");
            rk.SetValue("", "Android Package File");
            rk.Close();

            rk = root.CreateSubKey(@"Wow6432Node\.apk\DefaultIcon");
            rk.SetValue("", "%1");
            rk.Close();

            rk = root.CreateSubKey(@"Wow6432Node\.apk\shellex\IconHandler");
            rk.SetValue("", GUID);
            rk.Close();

            root.Close();

            ////////////////////////////////////////
            root = Registry.LocalMachine;
            try
            {
                rk = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                rk.SetValue(GUID, "Android Package file, shell extention");
                rk.Close();

                rk = root.OpenSubKey(@"Wow6432Node\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                rk.SetValue(GUID, "Android Package file, shell extention");
                rk.Close();
            } catch { }
            root.Close();
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
                root.DeleteSubKeyTree(@"Wow6432Node\.apk");
                root.Close();

                root = Registry.LocalMachine;
                rk = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                rk.DeleteValue(GUID);
                rk.Close();
                rk = root.OpenSubKey(@"Wow6432Node\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\");
                rk.DeleteValue(GUID);
                rk.Close();

                root.Close();
            } catch {}
        }
    }
}
// vim: expandtab tabstop=4 softtabstop=4 shiftwidth=4
