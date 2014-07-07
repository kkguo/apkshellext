using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using KKHomeProj.ShellExtInts;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.IO;
using Iteedee.ApkReader;

namespace KKHomeProj.Android
{
    public class AndroidPackage
    {
        #region Static Members
        public static Icon default_icon = null;
        public static AndroidPackage GetAndroidPackage(string filename)
        {
            return new AndroidPackage(filename);
        }
        #endregion

        public readonly string FileName;
        public bool hasIcon;
        public string IconPath;
        public string PackageName;
        public string VersionName;
        public string VersionCode;
        public string Label;
        public ArrayList UsesPermissions;
        public ArrayList UsesFeatures;

        private Icon m_icon;

        public AndroidPackage(string filename)
        {
            FileName = filename;
            getPackage();
        }

        public void getPackage() {
            //UsesPermissions = new ArrayList();
            //UsesFeatures = new ArrayList();
            //StreamReader sr = new StreamReader((new AndroidToolAapt()).Dump(FileName));

            //Regex r1 = new Regex(@"^package:\sname='(.*)'\sversionCode='(.*)'\sversionName='(.*)'$");
            //Regex r2 = new Regex(@"^application:\slabel='(.*)'\sicon='(.*)'$");
            //Regex r3 = new Regex(@"^uses-permission:'(.*)'$");
            //Regex r4 = new Regex(@"^uses-feature:'(.*)'$");
            //while (!sr.EndOfStream)
            //{
            //    string s = sr.ReadLine();

            //    if (r1.IsMatch(s)) {
            //        PackageName = r1.Match(s).Groups[1].Value;
            //        VersionCode = r1.Match(s).Groups[2].Value;
            //        VersionName = r1.Match(s).Groups[3].Value;
            //    }
            //    else if (r2.IsMatch(s))
            //    {
            //        Label = r2.Match(s).Groups[1].Value;
            //        IconPath = r2.Match(s).Groups[2].Value;
            //        hasIcon = true;
            //    }
            //    else if (r3.IsMatch(s))
            //    {
            //        UsesPermissions.Add(r3.Match(s).Groups[1].Value);
            //    }
            //    else if (r4.IsMatch(s))
            //    {
            //        UsesFeatures.Add(r4.Match(s).Groups[1].Value);
            //    }
            //}
        }        

        ~AndroidPackage() {
            if (m_icon!=null) m_icon.Dispose();
        }

        /// <summary>
        /// Get Icon from current APK package
        /// </summary>
        /// <returns>Icon object</returns>
        public Icon icon
        {
            get
            {
                if (m_icon == null)
                {
                    try
                    {
                        ZipFile zip = new ZipFile(FileName);
                        Bitmap bmp = (Bitmap)Bitmap.FromStream(zip.GetInputStream(zip.FindEntry(IconPath, true)));
                        zip.Close();
                        m_icon = Icon.FromHandle(bmp.GetHicon());
                    }
                    catch
                    {
                        m_icon = default_icon;
                    }
                }
                return m_icon;
            }
        }
    }

    public class AndroidPackage2 : AndroidPackage
    {
        public new static AndroidPackage2 GetAndroidPackage(string filename)
        {
            return new AndroidPackage2(filename);
        }
        public AndroidPackage2(string filename) : base(filename)
        {
            getPackage2();
        }
        ~AndroidPackage2()
        {
    
        }

        public static ApkInfo ReadApkFromPath(string path)
        {
            NativeMethods.Log("ReadApkFromPath: " + path);
            byte[] manifestData = null;
            byte[] resourcesData = null;
            using (ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(File.OpenRead(path)))
            {
                using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    ICSharpCode.SharpZipLib.Zip.ZipFile zipfile = new ICSharpCode.SharpZipLib.Zip.ZipFile(filestream);
                    ICSharpCode.SharpZipLib.Zip.ZipEntry item;
                    while ((item = zip.GetNextEntry()) != null)
                    {
                        if (item.Name.ToLower() == "androidmanifest.xml")
                        {
                            manifestData = new byte[50 * 1024];
                            using (Stream strm = zipfile.GetInputStream(item))
                            {
                                strm.Read(manifestData, 0, manifestData.Length);
                            }

                        }
                        if (item.Name.ToLower() == "resources.arsc")
                        {
                            using (Stream strm = zipfile.GetInputStream(item))
                            {
                                using (BinaryReader s = new BinaryReader(strm))
                                {
                                    resourcesData = s.ReadBytes((int)s.BaseStream.Length);

                                }
                            }
                        }
                    }
                }
            }

            ApkReader apkReader = new ApkReader();
            ApkInfo info = apkReader.extractInfo(manifestData, resourcesData);
            return info;
        }

        public void getPackage2() {
            ApkInfo apkInfo = ReadApkFromPath(FileName);

            UsesPermissions = new ArrayList();
            UsesFeatures = new ArrayList();

            PackageName = apkInfo.packageName;
            VersionCode = apkInfo.versionCode;
            VersionName = apkInfo.versionName;

            Label = apkInfo.label;
            hasIcon = apkInfo.hasIcon;
            if (apkInfo.iconFileName.Count > 0)
                 IconPath = apkInfo.iconFileName[0];
            foreach (string s in apkInfo.Permissions)
            {
                UsesPermissions.Add(s);
            }
        }        
    }
}
