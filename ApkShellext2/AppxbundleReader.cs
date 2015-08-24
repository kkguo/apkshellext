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
    public class AppxBundleReader :AppPackageReader {
        private ZipFile zip;
        private AppxReader appxReader= null;

        private readonly string AppxBundleManifestXml = @"AppxMetadata/AppxBundleManifest.xml";
        private readonly string ElemIdentity = @"Identity";
        private readonly string ElemProperties = @"Properties";
        private readonly string ElemDisplayName = @"DisplayName";
        private readonly string ElemPackage = @"Package";
        private readonly string AttrVersion = @"Version";
        private readonly string AttrName = @"Name";
        private readonly string AttrPublisher = @"Publisher";
        private readonly string AttrType = @"Type";
        private readonly string ValApplication = @"application";
        private readonly string AttrFileName = @"FileName";

        public AppxBundleReader(string path) {
            FileName = path;
            string appxFileName = "";

            zip = new ZipFile(FileName);
            ZipEntry en = zip.GetEntry(AppxBundleManifestXml);
            if (en == null)
                throw new EntryPointNotFoundException("cannot find " + AppxBundleManifestXml);

            using (XmlReader reader = XmlReader.Create(zip.GetInputStream(en))){
                reader.ReadToFollowing(ElemIdentity);
                reader.MoveToAttribute(AttrName);

                do {
                    reader.ReadToFollowing(ElemPackage);
                    reader.MoveToAttribute(AttrType);
                } while (reader.Value != ValApplication || reader.EOF);

                if (reader.EOF) 
                    throw new Exception("Cannot find application in " + AppxBundleManifestXml);                

                reader.MoveToAttribute(AttrFileName);
                appxFileName = reader.Value;
            }

            en = zip.GetEntry(appxFileName);
            if (en == null)
                throw new EntryPointNotFoundException("cannot find appx " + appxFileName);            

            appxReader = new AppxReader(zip.GetInputStream(en));
        }

        public override Bitmap Icon {
            get {
                return appxReader.Icon;
            }
        }

        public override string AppName {
            get {
                return appxReader.AppName;
            }
        }

        public override string PackageName {
            get {
                return appxReader.PackageName;
            }
        }

        public override string Version {
            get {
                return appxReader.Version;
            }
        }

        public override string Publisher {
            get {
                return appxReader.Publisher;
            }
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing) {
            if (disposed) return;
            if (disposing) {
                if (appxReader != null) {
                    appxReader.Close();
                }
                if (zip != null)
                    zip.Close();
            }
            disposed = true;
            base.Dispose(disposing);
        }

        public void Close() {
            Dispose(true);
        }

        ~AppxBundleReader() {
            Dispose(true);
        }
    }
}
