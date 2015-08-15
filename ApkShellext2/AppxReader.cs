using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ApkShellext2 {
    /// <summary>
    /// 
    /// </summary>
    public class AppxReader {
        private string _filename;
        
        public string DisplayName;
        public string Version;

        private string LogoPath;
        private readonly string AppxManifestXml = @"AppxManifest.xml";
        ZipFile zip;

        public AppxReader(Stream stream) {
            zip = new ZipFile(stream);
            Extract();
        }

        public AppxReader(string path) {
            _filename = path;
            zip = new ZipFile(_filename);
            Extract();
        }

        public void Extract() {
            ZipEntry en = zip.GetEntry(AppxManifestXml);
            if (en == null)
                throw new EntryPointNotFoundException("cannot find " + AppxManifestXml);
            byte[] xmlbytes = new byte[en.Size];
            zip.GetInputStream(en).Read(xmlbytes, 0, (int)en.Size);
            using (XmlReader reader = XmlReader.Create(new MemoryStream(xmlbytes))) {
                bool isInProperites = false;
                while (reader.Read()) {
                    if (reader.IsStartElement() && reader.Name == @"Identity") {
                        Version = reader.GetAttribute("Version");
                    }
                    if (reader.IsStartElement() && reader.Name == @"Properties") {
                        isInProperites = true;
                        continue;
                    }
                    if (isInProperites && reader.IsStartElement() && reader.Name == @"DisplayName") {
                        DisplayName = reader.ReadElementContentAsString();
                        continue;
                    }
                    if (isInProperites && reader.IsStartElement() && reader.Name == @"Logo") {
                        LogoPath = reader.ReadElementContentAsString().Replace(@"\",@"/");
                        break;
                    }
                    //if (isInProperites && reader.NodeType == XmlNodeType.EndElement && reader.Name == @"Properties") {
                    //    isInProperites = false;
                    //}
                }
            }
        }

        public Bitmap getLogo() {
            if (LogoPath == "") 
                throw new Exception("Cannot find logo path");
            int dot = LogoPath.LastIndexOf(".");
            string name = LogoPath.Substring(0,dot);
            string extension = LogoPath.Substring(dot+1);
            ZipEntry logo = null ;
            int scale = -1;
            foreach (ZipEntry en in zip) {
                Match m = Regex.Match(en.Name, name + @"(\.scale\-(\d+))?" + @"\." + extension);
                if (m.Success) {
                    if (m.Groups[1].Value == "") { // exactly matching, no scale
                        logo = en;
                        break;
                    } else { // find the biggest scale
                        int newScale = int.Parse(m.Groups[2].Value);
                        if (newScale > scale) {
                            logo = en;
                            scale = newScale;
                        }
                    }
                }
            }
            if (logo != null) {
                //byte[] imageBytes = new byte[logo.Size];
                //zip.GetInputStream(logo).Read(imageBytes, 0, (int)logo.Size);
                return new Bitmap(zip.GetInputStream(logo));
            } else {
                throw new EntryPointNotFoundException("Cannot find Logo file: " + LogoPath);
            }
        }

        public void Close() {
            if (zip != null) {
                zip.Close();
            }
        }
    }
}
