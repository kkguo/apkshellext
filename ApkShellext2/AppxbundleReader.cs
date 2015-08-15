using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.IO;
using System.Drawing;

namespace ApkShellext2 {
    /// <summary>
    /// Read AppxBundle
    /// </summary>
    public class AppxBundleReader {
        private string _filename;
        public  string Identity {set;get;}
        private string appxName;

        private ZipFile zip;
        private AppxReader appxReader= null;
        private readonly string AppxBundleManifestXml = @"AppxMetadata/AppxBundleManifest.xml";

        public AppxBundleReader(string path) {
            _filename = path;
            zip = new ZipFile(_filename);
            ZipEntry en = zip.GetEntry(AppxBundleManifestXml);
            if (en == null)
                throw new EntryPointNotFoundException("cannot find " + AppxBundleManifestXml);

            using (XmlReader reader = XmlReader.Create(zip.GetInputStream(en))){
                reader.ReadToFollowing("Identity");
                reader.MoveToAttribute("Name");
                Identity = reader.Value;

                do {
                    reader.ReadToFollowing("Package");
                    reader.MoveToAttribute("Type");
                } while (reader.Value != "application" || reader.EOF);

                if (reader.EOF) 
                    throw new Exception("Cannot find application in " + AppxBundleManifestXml);                

                reader.MoveToAttribute("FileName");
                appxName = reader.Value;
            }

            en = zip.GetEntry(appxName);
            if (en == null)
                throw new EntryPointNotFoundException("cannot find appx "+appxName);            

            appxReader = new AppxReader(zip.GetInputStream(en));
        }

        public Bitmap getLogo() {
            return appxReader.getLogo();
        }

        public void Close() {
            if (appxReader != null) {
                appxReader.Close();
            }
            if (zip != null) {
                zip.Close();
            }
        }
    }
}
