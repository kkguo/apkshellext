using PlistCS;
using PNGDecrush;
using SharpShell.Attributes;
using SharpShell.Extensions;
using SharpShell.Diagnostics;
using SharpShell.Exceptions;
using SharpShell.ServerRegistration;
using SharpShell.SharpIconHandler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace ApkShellext2 {
    public class IpaReader : AppPackageReader {
        private string strAppRoot;
        private Dictionary<string,object> dic;
        private ZipFile zip;

        private readonly string infoPlistPath = @"(Payload/.*\.app/)Info\.plist";
        private readonly string CFBundleIcons = @"CFBundleIcons";
        private readonly string CFBundlePrimaryIcon = @"CFBundlePrimaryIcon";
        private readonly string CFBundleIconFiles = @"CFBundleIconFiles";
        private readonly string CFBundleDisplayName = @"CFBundleDisplayName";
        private readonly string CFBundleIdentifier = @"CFBundleIdentifier";
        private readonly string CFBundleShortVersionString = @"CFBundleShortVersionString";
        private readonly string CFBundleVersion = @"CFBundleVersion";

        public IpaReader(string path) {
            FileName = path;
            zip = new ZipFile(FileName);
            ZipEntry infoPlist = null;
            foreach (ZipEntry en in zip) {
                Match m = Regex.Match(en.Name, infoPlistPath);
                if (m.Success) {
                    strAppRoot = m.Groups[1].Value;
                    infoPlist = en;
                    break;
                }
            }
            if (infoPlist == null) {
                throw new EntryPointNotFoundException("cannot find info.plist");
            }
            byte[] infoBytes = new byte[infoPlist.Size];
            zip.GetInputStream(infoPlist).Read(infoBytes,0,(int)infoPlist.Size);

            dic = (Dictionary<string, object>)Plist.readPlist(infoBytes);
        }

        public string[] getStrings(string[] keys) {
            Dictionary<string, object> m_dic = dic;
            for (int i = 0; i < keys.Length - 1; i++) {
               if (m_dic.ContainsKey(keys[i])) {
                   m_dic = (Dictionary<string, object>)m_dic[keys[i]];
                } else {
                    throw new Exception("Given ID is not valid");
                }
            }
            if (m_dic.ContainsKey(keys[keys.Length - 1])) {
                if (m_dic[keys[keys.Length - 1]] is string) {
                    return new string[] { m_dic[keys[keys.Length - 1]] as string };
                } else { // is list
                    object[] arr = ((List<object>)m_dic[keys[keys.Length - 1]]).ToArray();
                    return Array.ConvertAll<object, string>(arr, x => x.ToString());
                }
            } else {
                return null;
            }
        }

        public Bitmap getImage(string[] keys) {
            string[] images = getStrings(keys);
            if (images != null) {
                return getImage(images[0]);
            }
            return null;
        }

        public Bitmap getImage(string name) {
            string fullname = name;
            ZipEntry image;
            if (!name.EndsWith(".png")) {
                fullname = name + ".png";
                image = zip.GetEntry(strAppRoot + fullname);
                if (image == null) {
                    fullname = name + @"@2x" + ".png";
                    image = zip.GetEntry(strAppRoot + fullname);
                }
                if (image == null) {
                    fullname = name + @"@3x" + ".png";
                    image = zip.GetEntry(strAppRoot + fullname);
                }
            } else {
                image = zip.GetEntry(strAppRoot + name);
            }

            byte[] imageBytes = new byte[image.Size];
            zip.GetInputStream(image).Read(imageBytes, 0, (int)image.Size);
            MemoryStream imageOut = new MemoryStream();
            PNGDecrusher.Decrush(new MemoryStream(imageBytes), imageOut);
            return new Bitmap(imageOut);
        }

        public override string AppName {
            get {
                return getStrings(new string[] {
                    CFBundleDisplayName})[0];
            }
        }

        public override string Version {
            get {
                return getStrings(new string[] {
                    CFBundleShortVersionString})[0];
            }
        }

        //public override string Revision {
        //    get {
        //        return getStrings(new string[] {
        //            CFBundleVersion})[0];
        //    }
        //}

        public override string PackageName {
            get {
                return getStrings(new string[] {
                    CFBundleIdentifier})[0];
            }
        }
        public override Bitmap Icon {
            get {
                return getImage(new string[] { 
                    CFBundleIcons, 
                    CFBundlePrimaryIcon, 
                    CFBundleIconFiles });;
            }
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing) {
            if (disposed) return;
            if (disposing) {
                if (zip != null)
                    zip.Close();
            }
            disposed = true;
            base.Dispose(disposing);
        }

        public void Close() {
            Dispose(true);
        }

        ~IpaReader() {
            Dispose(true);
        }
    }
}
