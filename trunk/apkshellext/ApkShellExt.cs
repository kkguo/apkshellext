/***************************************************************************************************************\
 *
 * 
 * Reference : lc_mtt's blog http://blog.csdn.net/lc_mtt
 *             All-In-One Code Framework http://www.codeproject.com/KB/dotnet/CSShellExtContextMenuHand.aspx?q=context+menu+shell+extension+.net
 *             
 * Changelog :
 *             2011-8-25   Base on v2.0
 *                         Remeber last typed in IP, stored in registry
 *                         Select install path, internal memory or SD card.
 *                         Disconnect
 \**************************************************************************************************************/
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using KKHomeProj.ShellExtInts;
using Microsoft.Win32;

namespace KKHomeProj.ApkShellExt
{
    [Guid("66391a18-f480-413b-9592-a10044de6cf4"),
    ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ApkShellExt : IExtractIcon, IPersistFile, IShellExtInit, IContextMenu, IQueryInfo
    {
        #region Constants
        private const string GUID = "{66391a18-f480-413b-9592-a10044de6cf4}";
        private const string KeyName = "apkshellext";
        private const int BUFF_SIZE = 1024;
        #endregion

        private uint MenuConnectWIFI_ID;
        private string sFileName;
        private uint QITIPF_DEFAULT = 0;
        private ArrayList devices;

        #region IPersistFile 成员

        public void GetClassID(out Guid pClassID)
        {
            pClassID = new Guid(GUID);
        }

        public void GetCurFile(out string ppszFileName)
        {
            throw new NotImplementedException();
        }

        public int IsDirty()
        {
            throw new NotImplementedException();
        }

        public void Load(string pszFileName, int dwMode)
        {
            sFileName = pszFileName;
        }

        public void Save(string pszFileName, bool fRemember)
        {
            throw new NotImplementedException();
        }

        public void SaveCompleted(string pszFileName)
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
                return ((uFlags & ExtractIconOptions.Async) != 0) ? WinError.E_PENDING : WinError.S_OK;
            }
            catch
            {
                pwFlags = ExtractIconFlags.None;
                return WinError.S_FALSE;
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
                return WinError.S_OK;
            }
            catch
            {
                phiconLarge = phiconSmall = IntPtr.Zero;
                return WinError.S_FALSE;
            }
        }

        #endregion

        #region IShellExtInit Members
        public void Initialize(IntPtr pidlFolder, IntPtr pDataObj, IntPtr hKeyProgID)
        {
            if (pDataObj == IntPtr.Zero)
            {
                throw new ArgumentException();
            }

            FORMATETC fe = new FORMATETC();
            fe.cfFormat = (short)CLIPFORMAT.CF_HDROP;
            fe.ptd = IntPtr.Zero;
            fe.dwAspect = DVASPECT.DVASPECT_CONTENT;
            fe.lindex = -1;
            fe.tymed = TYMED.TYMED_HGLOBAL;
            STGMEDIUM stm = new STGMEDIUM();

            // The pDataObj pointer contains the objects being acted upon. In this 
            // example, we get an HDROP handle for enumerating the selected files 
            // and folders.
            IDataObject dataObject = (IDataObject)Marshal.GetObjectForIUnknown(pDataObj);
            dataObject.GetData(ref fe, out stm);

            try
            {
                // Get an HDROP handle.
                IntPtr hDrop = stm.unionmember;
                if (hDrop == IntPtr.Zero)
                {
                    throw new ArgumentException();
                }

                // Determine how many files are involved in this operation.
                uint nFiles = NativeMethods.DragQueryFile(hDrop, UInt32.MaxValue, null, 0);

                // This code sample displays the custom context menu item when only 
                // one file is selected. 
                if (nFiles == 1)
                {
                    // Get the path of the file.
                    StringBuilder fileName = new StringBuilder(260);
                    if (0 == NativeMethods.DragQueryFile(hDrop, 0, fileName,
                        fileName.Capacity))
                    {
                        Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                    }
                    sFileName = fileName.ToString();
                }
                else
                {
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }
            }
            finally
            {
                NativeMethods.ReleaseStgMedium(ref stm);
            }

        }
        #endregion

        #region IContextMenu Members
        public int QueryContextMenu(IntPtr hMenu, uint iMenu, uint idCmdFirst, uint idCmdLast, uint uFlags)
        {
            if (((uint)CMF.CMF_DEFAULTONLY & uFlags) != 0)
            {
                return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0, 0);
            }

            devices = new ArrayList();
            try
            {
                ExtractResourceZip(Properties.Resources.adb, @"adb.exe");
                ExtractResourceZip(Properties.Resources.adb, @"AdbWinApi.dll");
                ExtractResourceZip(Properties.Resources.adb, @"AdbWinUsbApi.dll");

                Process p = StartProcess(@"adb.exe", @"devices");

                string line = p.StandardOutput.ReadLine();
                Regex r = new Regex(@"^([\w-\.:]*)\s+device$");
                while (!String.IsNullOrWhiteSpace(line))
                {
                    if (r.IsMatch(line))
                    {
                        AndroidDevice d = new AndroidDevice();
                        d.Serialno = (string)r.Match(line).Groups[1].Value;                        
                        devices.Add(d);
                    }
                    line = p.StandardOutput.ReadLine();
                }
                p.Close();
            }
            catch { }

            uint id = 0;
            try
            {
                HMenu submenu = NativeMethods.CreatePopupMenu();
                if (devices.Count > 0)
                {
                    foreach (AndroidDevice d in devices)
                    {
                        HMenu subsubmenu = NativeMethods.CreatePopupMenu();
                        d.menuID_1 = id;
                        NativeMethods.AppendMenu(subsubmenu, MFMENU.MF_STRING, new IntPtr(idCmdFirst + id++), Properties.Resources.menu_InstallToInternalMemory);
                        d.menuID_2 = id;
                        NativeMethods.AppendMenu(subsubmenu, MFMENU.MF_STRING, new IntPtr(idCmdFirst + id++), Properties.Resources.menu_InstallToSDCard);
                        if (d.ConnectFromWIFI)
                        {
                            d.menuID_3 = id;
                            NativeMethods.AppendMenu(subsubmenu, MFMENU.MF_STRING, new IntPtr(idCmdFirst + id++), Properties.Resources.menu_DisconnectWIFI);
                        }

                        NativeMethods.InsertMenu(submenu, 1, MFMENU.MF_BYPOSITION | MFMENU.MF_POPUP, subsubmenu.handle, d.Serialno);
                        //NativeMethods.AppendMenu(submenu, MFMENU.MF_STRING, new IntPtr(idCmdFirst + id++), s);
                    }
                }
                else
                {
                    NativeMethods.AppendMenu(submenu, MFMENU.MF_STRING | MFMENU.MF_DISABLED | MFMENU.MF_GRAYED, new IntPtr(idCmdFirst + id++), Properties.Resources.menu_CannotFindPhone);
                }
                // a separator
                NativeMethods.AppendMenu(submenu, MFMENU.MF_SEPARATOR, new IntPtr(idCmdFirst + id++), "");
                // Connect with WIFI
                NativeMethods.AppendMenu(submenu, MFMENU.MF_STRING, new IntPtr(idCmdFirst + id++), Properties.Resources.menu_ConnectViaWIFI);
                MenuConnectWIFI_ID = id - 1;
                // Insert to popup-menu
                NativeMethods.InsertMenu(new HMenu(hMenu), 1, MFMENU.MF_BYPOSITION | MFMENU.MF_POPUP, submenu.handle, Properties.Resources.menu_InstallToPhone);
            }
            catch {
                return Marshal.GetHRForLastWin32Error();
            }

            // Return an HRESULT value with the severity set to SEVERITY_SUCCESS. 
            // Set the code value to the offset of the largest command identifier 
            // that was assigned, plus one (1).
            return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0, id + 1);
            
        }

        public void InvokeCommand(IntPtr pici)
        {
            //bool isUnicode = false;

            // Determine which structure is being passed in, CMINVOKECOMMANDINFO or 
            // CMINVOKECOMMANDINFOEX based on the cbSize member of lpcmi. Although 
            // the lpcmi parameter is declared in Shlobj.h as a CMINVOKECOMMANDINFO 
            // structure, in practice it often points to a CMINVOKECOMMANDINFOEX 
            // structure. This struct is an extended version of CMINVOKECOMMANDINFO 
            // and has additional members that allow Unicode strings to be passed.
            CMINVOKECOMMANDINFO ici = (CMINVOKECOMMANDINFO)Marshal.PtrToStructure(
                pici, typeof(CMINVOKECOMMANDINFO));
            //CMINVOKECOMMANDINFOEX iciex = new CMINVOKECOMMANDINFOEX();
            //if (ici.cbSize == Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX)))
            //{
            //    if ((ici.fMask & CMIC.CMIC_MASK_UNICODE) != 0)
            //    {
            //        isUnicode = true;
            //        iciex = (CMINVOKECOMMANDINFOEX)Marshal.PtrToStructure(pici,
            //            typeof(CMINVOKECOMMANDINFOEX));
            //    }
            //}

            // Is the command identifier offset supported by this context menu 
            // extension?
            int id = NativeMethods.LowWord(ici.verb.ToInt32());
            if (id == MenuConnectWIFI_ID)
            {
                string s = "";
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(@".apk\shellex\ContextMenuHandlers\" + KeyName);
                if (rk != null)
                {
                    s = (string)rk.GetValue("LastConnectedIP");
                }
                s = Microsoft.VisualBasic.Interaction.InputBox(Properties.Resources.prompt_ConnectViaWIFI,
                     Properties.Resources.menu_ConnectViaWIFI,
                     s);
                if (NativeMethods.isIPAddress(s))
                {
                    Registry.ClassesRoot.CreateSubKey(@".apk\shellex\ContextMenuHandlers\" + KeyName).SetValue("LastConnectedIP", s);

                    Process p = StartProcess(@"adb.exe", "connect " + s);
                    System.Windows.Forms.MessageBox.Show(p.StandardOutput.ReadToEnd());
                    p.Close();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.prompt_NotAnIP);
                }
            }
            else
            {
                bool flag = false;
                foreach (AndroidDevice d in devices)
                {
                    if (d.menuID_1 == id || d.menuID_2 == id)
                    {
                        Process p = StartProcess(@"adb.exe", "-s " + d.Serialno + " install -r " +((d.menuID_2==id)?"-s ":"") + sFileName);
                        System.Windows.Forms.MessageBox.Show(p.StandardOutput.ReadToEnd());
                        p.Close();
                        flag = true;
                        break;
                    }
                    else if (d.menuID_3 == id)
                    {
                        Process p = StartProcess(@"adb.exe", "disconnect " + d.Serialno);
                        p.Close();
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    // If the verb is not recognized by the context menu handler, it 
                    // must return E_FAIL to allow it to be passed on to the other 
                    // context menu handlers that might implement that verb.
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }
            }
        }

        public void GetCommandString(UIntPtr idCmd, uint uFlags, IntPtr pReserved, System.Text.StringBuilder pszName, uint cchMax)
        {
            pszName.Clear();
            //if ((GCS)uFlags == GCS.GCS_HELPTEXTW) {
            //    if (idCmd.ToUInt32() < devices.Count)
            //    {

            //        if (Properties.Resources.menu_comment_InstallToPhone.Length > cchMax - 1)
            //        {
            //            Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER);
            //        }
            //        else
            //        {
            //            pszName.Clear();
            //            pszName.Append(Properties.Resources.menu_comment_InstallToPhone);
            //        }

            //    }
            //    else if (idCmd.ToUInt32() == devices.Count + 1)
            //    {
            //        if (Properties.Resources.menu_comment_ConnectViaWIFI.Length > cchMax -1){
            //            Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER);
            //        }
            //        else
            //        {
            //            pszName.Clear();
            //            pszName.Append(Properties.Resources.menu_comment_ConnectViaWIFI);
            //        }
            //    }
            //}

        }
        #endregion

        #region IQuaryInfo Members
        public uint GetInfoTip(uint dwFlags, out IntPtr pszInfoTip)
        {
            try
            {
                ExtractResourceZip(Properties.Resources.aapt, @"aapt.exe");
                Process p = new Process();
                p.StartInfo.FileName = Path.GetTempPath() + @"aapt.exe";
                p.StartInfo.Arguments = @"dump badging " + "\"" + sFileName + "\"";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WorkingDirectory = Path.GetTempPath();
                p.Start();
                string tip = p.StandardOutput.ReadToEnd();
                
                pszInfoTip = Marshal.StringToCoTaskMemUni(tip);
            }
            catch
            {
                pszInfoTip = IntPtr.Zero;
            }
            return WinError.S_OK;
        }

        public uint GetInfoFlags(out uint dwFlags)
        {
            dwFlags = QITIPF_DEFAULT;
            return WinError.S_OK;
        }
        #endregion

        #region Registeration
        [ComRegisterFunction()]
        public static void Register(Type t)
        {
            try
            {
                //Register APK file type
                RegApk(t.GUID);
            }
            catch
            {
            }
        }

        [ComUnregisterFunction()]
        public static void Unregister(Type t)
        {
            try
            {
                //unregister file type association
                UnregApk(t.GUID);
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
                ExtractResourceZip(Properties.Resources.aapt, @"aapt.exe");

                Process p = StartProcess(@"aapt.exe",@"dump badging " + "\"" + sFileName + "\"");
                string icon_path = p.StandardOutput.ReadLine();
                Regex r = new Regex(@"^application.*icon='([\w/.]*)'$");
                while (!String.IsNullOrEmpty(icon_path))
                {
                    if (r.IsMatch(icon_path))
                    {
                        icon_path = (string)r.Match(icon_path).Groups[1].Value;
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
        private static void ExtractResourceZip(byte[] resource, string fileName,bool OverWriteIfExists=false)
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
        private static void RegApk(Guid guid)
        {
            RegistryKey root;
            RegistryKey rk;
            root = Registry.LocalMachine;
            try
            {
                rk = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\",true);
                rk.SetValue(guid.ToString("B"), "Shell Extention for Android Package files");
                rk.Close();
            }
            catch { }
            root.Close();

            root = Registry.ClassesRoot;
            rk = root.CreateSubKey(@".apk");
            rk.SetValue("", "Android Package File");
            rk.Close();

            rk = root.CreateSubKey(@".apk\shellex\IconHandler");
            rk.SetValue("", guid.ToString("B"));
            rk.Close();
            rk = root.CreateSubKey(@".apk\shellex\ContextMenuHandlers\"+KeyName);
            rk.SetValue("", guid.ToString("B"));
            rk.Close();
            rk = root.CreateSubKey(@".apk\shellex\{00021500-0000-0000-C000-000000000046}");
            rk.SetValue("", guid.ToString("B"));
            rk.Close();
            root.Close();

            ////////////////////////////////////////
            ExtractResourceZip(Properties.Resources.aapt, @"aapt.exe",true);        
            ExtractResourceZip(Properties.Resources.adb,  @"adb.exe",true);
            ExtractResourceZip(Properties.Resources.adb,  @"AdbWinApi.dll",true);
            ExtractResourceZip(Properties.Resources.adb,  @"AdbWinUsbApi.dll", true);
        }

        /// <summary>
        /// unregister
        /// </summary>
        private static void UnregApk(Guid guid)
        {
            try{
                RegistryKey root;
                RegistryKey rk;
                root = Registry.ClassesRoot;
                root.DeleteSubKeyTree(@".apk\shellex\IconHandler");
                root.DeleteSubKeyTree(@".apk\shellex\ContextMenuHandlers\" + KeyName);
                root.DeleteSubKeyTree(@".apk\shellex\{00021500-0000-0000-C000-000000000046}");
                root.Close();

                root = Registry.LocalMachine;
                rk = root.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved\",true);
                rk.DeleteValue(guid.ToString("B"));
                rk.Close();

                root.Close();
            } catch {}
        }

        /// <summary>
        /// start a process under backgroud, the process should be in %TMP%, and working directory will be 
        /// %TMP as well
        /// </summary>
        /// <param name="cmd">command to execute</param>
        /// <param name="arg">arguments pass to the command</param>
        /// <returns>the process</returns>
        private Process StartProcess(string cmd, string arg)
        {
            Process p = new Process();
            p.StartInfo.FileName = Path.GetTempPath() + cmd;
            p.StartInfo.Arguments = arg;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = Path.GetTempPath();
            p.Start();
            return p;
        }
    }
}
// vim: expandtab tabstop=4 softtabstop=4 shiftwidth=4
