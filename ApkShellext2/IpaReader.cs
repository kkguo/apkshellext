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
    public class IpaReader : IDisposable {
        private string _filename;
        private string strAppRoot;
        private Dictionary<string,object> dic;
        private ZipFile zip;

        public string FileName { get { return _filename; } }

        public IpaReader(string path) {
            _filename = path;
            zip = new ZipFile(_filename);
            ZipEntry infoPlist = null;
            foreach (ZipEntry en in zip) {
                Match m = Regex.Match(en.Name, @"(Payload/.*\.app/)Info\.plist");
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

        public string[] getStrings(string[] ID) {
            Dictionary<string, object> m_dic = dic;
            for (int i = 0; i < ID.Length - 1; i++) {
               if (m_dic.ContainsKey(ID[i])) {
                   m_dic = (Dictionary<string, object>)m_dic[ID[i]];
                } else {
                    throw new Exception("Given ID is not valid");
                }
            }
            if (m_dic.ContainsKey(ID[ID.Length - 1])) {
                object [] arr = ((List<object>)m_dic[ID[ID.Length-1]]).ToArray();
                return Array.ConvertAll<object,string>(arr, x => x.ToString());
            } else {
                return null;
            }
        }

        public Bitmap getImage(string[] ID) {
            string[] images = getStrings(ID);
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

        void IDisposable.Dispose() {
            if (zip != null) {
                zip.Close();
            }
        }
    }
}
