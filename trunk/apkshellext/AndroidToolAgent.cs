using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KKHomeProj.ShellExtInts;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;

namespace KKHomeProj.Android
{
    class AndroidToolAgent
    {
        public static string WorkingPath = Path.GetTempPath();
        private const int BUFFERSIZE = 4096;

        public string Binary;
        public string [] Dependency;
        protected byte[] _resource;
        
        public delegate void GotProcessOutHandler(string s);

        public event EventHandler ExecuteStart;
        public event EventHandler ExecuteEnd;
        public event GotProcessOutHandler GotOutputLine;

        /// <summary>
        /// extract file from zipped resource, and place to temp folder
        /// </summary>
        /// <param name="resource">resource name</param>
        /// <param name="fileName">output name</param>
        /// <param name="OverWriteIfExists">if true,will overwrite the file even if the file exists</param>
        private void ExtractResourceZip(byte[] resource, string fileName, bool OverWriteIfExists = false, int BufferSize = BUFFERSIZE)
        {
            string target = WorkingPath + fileName;
            
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
                    byte[] buff = new byte[BufferSize];
                    int read_count;
                    while ((read_count = inStream.Read(buff, 0, BufferSize)) > 0)
                    {
                        fs.Write(buff, 0, read_count);
                    }
                }
                catch { }
                finally
                {
                    if (zip != null) zip.Close();
                    if (fs != null) fs.Close();
                    if (inStream != null) inStream.Close();
                }
            }
        }
        /// <summary>
        /// Extract binary and dependency files from resource
        /// </summary>
        /// <param name="overwrite">if file exists, overwrite</param>
        protected void Extract(bool overwrite = false)
        {
            ExtractResourceZip(_resource, Binary, overwrite);
            for (int i = 0; i < Dependency.Length; i++)
            {
                ExtractResourceZip(_resource, Dependency[i], overwrite);
            }
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
            p.StartInfo.FileName =WorkingPath + cmd;
            p.StartInfo.Arguments = arg;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = WorkingPath;
            p.EnableRaisingEvents = false;
            p.Start();
            return p;
        }
        /// <summary>
        /// execute command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        /// <returns>result of command</returns>
        protected Stream Execute(string cmd, string arg, bool async = false)
        {
            if (ExecuteStart != null) ExecuteStart(this, new EventArgs());
            Process p = StartProcess(cmd, arg);
            string s = "";
            if (GotOutputLine != null)
            {
                while (!p.StandardOutput.EndOfStream)
                {
                    string ss = p.StandardOutput.ReadLine();
                    GotOutputLine(ss);
                    s += ss;
                }
            }
            else
            {
                s = p.StandardOutput.ReadToEnd();
            }
            p.WaitForExit();
            p.Close();
            if (ExecuteEnd != null) ExecuteEnd(this, new EventArgs());
            return new MemoryStream(ASCIIEncoding.UTF8.GetBytes(s));
        }
    }

    class AndroidToolAapt : AndroidToolAgent
    {
        public AndroidToolAapt()
        {
            Binary = @"aapt.exe";
            Dependency = new string[] { };
            _resource = KKHomeProj.ApkShellExt.Properties.Resources.aapt;
            Extract();
        }

        public Stream Dump(string apkfile)
        {
            return Execute(Binary, "dump badging \"" + apkfile + "\"");
        }
    }

    class AndroidToolAdb : AndroidToolAgent
    {
        public AndroidToolAdb()
        {
            Binary = @"adb.exe";
            Dependency = new string[] { @"AdbWinApi.dll", @"AdbWinUsbApi.dll" };
            _resource = KKHomeProj.ApkShellExt.Properties.Resources.adb;
            Extract();
        }

        public bool   install(string serialno, string apkfile, bool installToSD = false)
        {
            string param = "";
            if (!String.IsNullOrEmpty(serialno)) param += "-s " + serialno;
            param += " install -r " + (installToSD ? "-s " : "") + apkfile;
            StreamReader sr = new StreamReader(Execute(Binary, param));
            while (!sr.EndOfStream)
            {
                if (sr.ReadLine().Trim()=="Success") return true;
            }
            return false;
        }
        public bool   uninstall(string serialno, string packagename)
        {
            if (String.IsNullOrEmpty(packagename)) return false;
            string param = " uninstall " + packagename;
            if (!String.IsNullOrEmpty(serialno)) param = "-s " + serialno + param;
            StreamReader sr = new StreamReader(Execute(Binary, param));
            while (!sr.EndOfStream)
            {
                if (sr.ReadLine().Trim() == "Success") return true;
            }
            return false;
        }
        public Stream Devices()
        {
            return Execute(Binary, "devices");
        }
        public void   Connect(string IP)
        {
            Execute(Binary, "connect " + IP, true);
        }
        public void   Disconnect(string IP = "")
        {
            Execute(Binary, "disconnect " + IP);
        }
        public string GetState(string serialno = "")
        {
            string param = " get-state";
            if (!String.IsNullOrEmpty (serialno)) param = "-s " + serialno + param;
            return (new StreamReader(Execute(Binary, param))).ReadToEnd();
        }
        public void   StartServer()
        {
            Execute(Binary, "start-server");
        }
    }
}