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
using Ionic.Zlib;

namespace ApkShellext2 {
    public class IpaReader : AppPackageReader {
        private string strAppRoot;
        private Dictionary<string,object> infoPlistDic;
        private Dictionary<string, object> itunesMetadataDic;
        private ZipFile zip;

        private const string iTunesMetadataPath = @"iTunesMetadata.plist";
        private const string infoPlistPath = @"(Payload/.*\.app/)Info\.plist";
        private const string CFBundleIcons = @"CFBundleIcons";
        private const string CFBundlePrimaryIcon = @"CFBundlePrimaryIcon";
        private const string CFBundleIconFiles = @"CFBundleIconFiles";
        private const string CFBundleDisplayName = @"CFBundleDisplayName";
        private const string CFBundleIdentifier = @"CFBundleIdentifier";
        private const string CFBundleShortVersionString = @"CFBundleShortVersionString";
        private const string CFBundleVersion = @"CFBundleVersion";

        public const string flagAppId = @"itemId";
        public const string flagCopyright = @"copyright";
        

        public IpaReader(string path) {
            FileName = path;
            openStream(new FileStream(path,FileMode.Open, FileAccess.Read,FileShare.Read));
        }

        public IpaReader(Stream stream) {
            openStream(stream);
        }

        private void openStream(Stream stream) {
            zip = new ZipFile(stream);
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
            zip.GetInputStream(infoPlist).Read(infoBytes, 0, (int)infoPlist.Size);

            infoPlistDic = (Dictionary<string, object>)Plist.readPlist(infoBytes);
        }

        public string[] getStrings(Dictionary<string, object> dic, string[] keys) {
            for (int i = 0; i < keys.Length - 1; i++) {
               if (dic.ContainsKey(keys[i])) {
                   dic = (Dictionary<string, object>)dic[keys[i]];
                } else {
                    throw new Exception("Given ID is not valid");
                }
            }
            if (dic.ContainsKey(keys[keys.Length - 1])) {
                if (dic[keys[keys.Length - 1]] is string) {
                    return new string[] { dic[keys[keys.Length - 1]] as string };
                } if (dic[keys[keys.Length-1]] is int) {
                    return new string[] {dic[keys[keys.Length - 1]].ToString()};
                } else { // is list
                    object[] arr = ((List<object>)dic[keys[keys.Length - 1]]).ToArray();
                    return Array.ConvertAll<object, string>(arr, x => x.ToString());
                }
            } else {
                return null;
            }
        }

        public Bitmap getImage(string[] keys) {
            string[] images = getStrings(infoPlistDic, keys);
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

        public override AppPackageReader.AppType Type {
            get {
                return AppType.iOSApp;
            }
        }

        public override string AppName {
            get {
                return getStrings(infoPlistDic, new string[] {
                    CFBundleDisplayName})[0];
            }
        }

        public override string Version {
            get {
                return getStrings(infoPlistDic, new string[] {
                    CFBundleShortVersionString})[0];
            }
        }

        public override string Revision {
            get {
                return getStrings(infoPlistDic, new string[] {
                    CFBundleVersion})[0];
            }
        }

        public override string PackageName {
            get {
                return getStrings(infoPlistDic, new string[] {
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

        public override string Publisher {
            get {
                ZipEntry itunesMetadata = zip.GetEntry(iTunesMetadataPath);
                if (itunesMetadata == null)
                    return "";
                byte[] itunesMetadataBytes = new byte[itunesMetadata.Size];
                zip.GetInputStream(itunesMetadata).Read(itunesMetadataBytes, 0, (int)itunesMetadata.Size);
                itunesMetadataDic = (Dictionary<string, object>)Plist.readPlist(itunesMetadataBytes);
                return getStrings(itunesMetadataDic, new string [] {flagCopyright})[0];
            }
        }

        public override string AppID {
            get {
                ZipEntry itunesMetadata = zip.GetEntry(iTunesMetadataPath);
                if (itunesMetadata == null)
                    return "";
                byte[] itunesMetadataBytes = new byte[itunesMetadata.Size];
                zip.GetInputStream(itunesMetadata).Read(itunesMetadataBytes, 0, (int)itunesMetadata.Size);
                itunesMetadataDic = (Dictionary<string, object>)Plist.readPlist(itunesMetadataBytes);
                return getStrings(itunesMetadataDic, new string[] { flagAppId })[0];
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
