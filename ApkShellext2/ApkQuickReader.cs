using ApkShellext2;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;

namespace ApkQuickReader {
    public enum RES_TYPE : ushort {
        RES_NULL_TYPE = 0x0000,
        RES_STRING_POOL_TYPE = 0x0001,
        RES_TABLE_TYPE = 0x0002,
        RES_XML_TYPE = 0x0003,

        // Chunk types in RES_XML_TYPE
        RES_XML_FIRST_CHUNK_TYPE = 0x0100,
        RES_XML_START_NAMESPACE_TYPE = 0x0100,
        RES_XML_END_NAMESPACE_TYPE = 0x0101,
        RES_XML_START_ELEMENT_TYPE = 0x0102,
        RES_XML_END_ELEMENT_TYPE = 0x0103,
        RES_XML_CDATA_TYPE = 0x0104,
        RES_XML_LAST_CHUNK_TYPE = 0x017f,
        // This contains a uint32_t array mapping strings in the string
        // pool back to resource identifiers.  It is optional.
        RES_XML_RESOURCE_MAP_TYPE = 0x0180,

        // Chunk types in RES_TABLE_TYPE
        RES_TABLE_PACKAGE_TYPE = 0x0200,
        RES_TABLE_TYPE_TYPE = 0x0201,
        RES_TABLE_TYPE_SPEC_TYPE = 0x0202
    };

    public enum DATA_TYPE : byte {
        // Contains no data.
        TYPE_NULL = 0x00,
        // The 'data' holds an attribute resource identifier.
        TYPE_ATTRIBUTE = 0x02,
        // The 'data' holds a single-precision floating point number.
        TYPE_FLOAT = 0x04,
        // The 'data' holds a complex number encoding a dimension value,
        // such as "100in".
        TYPE_DIMENSION = 0x05,
        // The 'data' holds a complex number encoding a fraction of a
        // container.
        TYPE_FRACTION = 0x06,
        // The 'data' is a raw integer value of the form n..n.
        TYPE_INT_DEC = 0x10,
        // The 'data' is a raw integer value of the form 0xn..n.
        TYPE_INT_HEX = 0x11,
        // The 'data' is either 0 or 1, for input "false" or "true" respectively.
        TYPE_INT_BOOLEAN = 0x12,
        // The 'data' is a raw integer value of the form #aarrggbb.
        TYPE_INT_COLOR_ARGB8 = 0x1c,
        // The 'data' is a raw integer value of the form #rrggbb.
        TYPE_INT_COLOR_RGB8 = 0x1d,
        // The 'data' is a raw integer value of the form #argb.
        TYPE_INT_COLOR_ARGB4 = 0x1e,
        // The 'data' is a raw integer value of the form #rgb.
        TYPE_INT_COLOR_RGB4 = 0x1f,
        TYPE_REFERENCE = 0x01,
        TYPE_STRING = 0x03,
    }

    public class ApkReader : AppPackageReader {
        private ZipFile zip;
        private byte[] resources;
        private byte[] manifest;

        private readonly string AndroidManifextXML = @"androidmanifest.xml";
        private readonly string Resources_arsc = @"resources.arsc";
        private readonly string TagApplication = @"application";
        private readonly string TagManifest = @"manifest";
        private readonly string AttrLabel = @"label";
        private readonly string AttrVersionName = @"versionName";
        private readonly string AttrVersionCode = @"versionCode";
        private readonly string AttrIcon = @"icon";
        private readonly string AttrPackage = @"package";
        private readonly string AttrName = @"name";
        private readonly int ConfigurationDensityPosition = 10;

        private byte[] use_config = null;

        /// <summary>
        /// extract the manifext
        /// </summary>
        /// <param name="filename">full path to the file</param>
        /// <param name="culture"></param>
        public ApkReader(string filename, string culture = "") {
            FileName = filename;
            openStream(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public ApkReader(Stream stream, string culture = "") {
            openStream(stream);
        }

        private void openStream(Stream stream) {
            zip = new ZipFile(stream);
            ZipEntry en = zip.GetEntry(AndroidManifextXML);
            BinaryReader s = new BinaryReader(zip.GetInputStream(en));
            manifest = s.ReadBytes((int)en.Size);

            en = zip.GetEntry(Resources_arsc);
            s = new BinaryReader(zip.GetInputStream(en));
            resources = s.ReadBytes((int)en.Size);
        }

        public override string AppName {
            get {
                return getAttribute(TagApplication, AttrLabel);
            }
        }

        public override string Version {
            get {
                return getAttribute(TagManifest, AttrVersionName);
            }
        }
        public override string Revision {
            get {
                return getAttribute(TagManifest, AttrVersionCode);
            }
        }

        public override string Publisher {
            get {
                string appName = getAttribute(TagApplication, AttrName);
                return appName;
            }
        }

        public override Bitmap Icon {
            get {
                return getImage(TagApplication, AttrIcon);
            }
        }

        public override string PackageName {
            get {
                return getAttribute(TagManifest, AttrPackage);
            }
        }

        /// <summary>
        /// get a string from manifest.xml
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string getAttribute(string tag, string attr) {
            return QuickSearchManifestXml(tag, attr);
        }

        /// <summary>
        /// Get a Image object from manifest and resources
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public Bitmap getImage(string tag, string attr) {
            // get the biggest density config
            List<byte[]> configs = getResourceConfigs();
            int bestDesityIndex = 0;
            int bestDensity = configs[0][ConfigurationDensityPosition] + configs[0][ConfigurationDensityPosition + 1] * 256;
            for (int i = 1; i < configs.Count; i++) {
                int density = configs[i][ConfigurationDensityPosition] + configs[i][ConfigurationDensityPosition + 1] * 256;
                if (density > bestDensity) {
                    bestDesityIndex = i;
                    bestDensity = density;
                }
            }
            use_config = configs[bestDesityIndex];
            ZipEntry en = zip.GetEntry(QuickSearchManifestXml(tag, attr));
            use_config = null;
            if (en != null) {
                try {
                    return (Bitmap)Bitmap.FromStream(zip.GetInputStream(en));
                } catch {
                    return null;
                }
            } else {
                return null;
            }
        }

        private uint imageSize;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public void setImageSize(uint size) {
            imageSize = size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private string QuickSearchManifestXml(string tag, string attribute) {
            using (MemoryStream ms = new MemoryStream(manifest))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // skip header

                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip string pool chunk

                long resourceMapPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip resourceMap

                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // skip StartNamespaceChunk

                // XML_START_ELEMENT CHUNK
                while (true) {
                    long chunkPos = ms.Position;
                    short chunkType = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int chunkSize = br.ReadInt32();
                    if (chunkType != (short)RES_TYPE.RES_XML_START_ELEMENT_TYPE) {
                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip current chunk
                        continue;
                    }
                    ms.Seek(8 + 4, SeekOrigin.Current); // skip line number & comment / namespace
                    string tag_s = QuickSearchManifestStringPool(br.ReadUInt32());
                    if (tag_s.ToUpper() != tag.ToUpper()) {
                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);// to next tag
                        continue;
                    }
                    int attributeStart = br.ReadInt16();
                    int attributeSize = br.ReadInt16();
                    int attributeCount = br.ReadInt16();
                    for (int i = 0; i < attributeSize; i++) {
                        ms.Seek(chunkPos + headerSize + attributeStart + attributeSize * i + 4, SeekOrigin.Begin); // ignore the ns                            
                        string name = QuickSearchManifestStringPool(br.ReadUInt32());
                        if (name.ToUpper() == attribute.ToUpper()) {
                            ms.Seek(4 + 2 + 1, SeekOrigin.Current); // skip rawValue/size/0/
                            byte dataType = br.ReadByte();
                            uint data = br.ReadUInt32();
                            if (dataType == (byte)DATA_TYPE.TYPE_STRING) {
                                return QuickSearchManifestStringPool(data);
                            } else if (dataType == (byte)DATA_TYPE.TYPE_REFERENCE) {
                                return QuickSearchResource((UInt32)data,use_config);
                            } else { // I would like to expect we only will recieve TYPE_STRING/TYPE_REFERENCE/any integer type, complex is not considering here,yet
                                return data.ToString();
                            }
                        }
                    }
                    return null;
                }
            }

        }

        /// <summary>
        /// Search in Manifest string pool
        /// </summary>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchManifestStringPool(uint stringID) {
            using (MemoryStream ms = new MemoryStream(manifest))
            using (BinaryReader br = new BinaryReader(ms)) {
                // the first chunk is always stringpool for manifest and resources
                ms.Seek(2, SeekOrigin.Begin);
                short headerSize = br.ReadInt16();
                ms.Seek(headerSize, SeekOrigin.Begin);
                return QuickSearchStringPool(ms, stringID);
            }

        }

        /// <summary>
        /// Search in Resources string pool
        /// </summary>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchResourcesStringPool(uint stringID) {
            using (MemoryStream ms = new MemoryStream(resources))
            using (BinaryReader br = new BinaryReader(ms)) {
                // the first chunk is always stringpool for manifest and resources
                ms.Seek(2, SeekOrigin.Begin);
                short headerSize = br.ReadInt16();
                ms.Seek(headerSize, SeekOrigin.Begin);
                return QuickSearchStringPool(ms, stringID);
            }

        }

        /// <summary>
        /// Search a string pool within existing stream, the stream need to be at the 
        /// start of string pool
        /// </summary>
        /// <param name="ms">Stream, need to be at start of stringPool</param>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchStringPool(MemoryStream ms, uint stringID) {
            using (BinaryReader br = new BinaryReader(ms)) {
                long poolPos = ms.Position; // record start of pool
                ms.Seek(8 + 4 + 4, SeekOrigin.Current); //skip the header/stringCount/styleCount

                // comes to the start of string pool chunk body, 
                int flags = br.ReadInt32();
                bool isUTF_8 = (flags & (1 << 8)) != 0;
                int stringStart = br.ReadInt32();
                ms.Seek(4, SeekOrigin.Current);
                ms.Seek(stringID * 4, SeekOrigin.Current);
                int stringPos = br.ReadInt32();
                ms.Seek(poolPos + stringStart + stringPos, SeekOrigin.Begin);
                if (isUTF_8) {
                    int u16len = br.ReadByte(); // u16len
                    if ((u16len & 0x80) != 0) {// larger than 128
                        u16len = ((u16len & 0x7F) << 8) + br.ReadByte();
                    }

                    int u8len = br.ReadByte(); // u8len
                    if ((u8len & 0x80) != 0) {// larger than 128
                        u8len = ((u8len & 0x7F) << 8) + br.ReadByte();
                    }
                    return Encoding.UTF8.GetString(br.ReadBytes(u8len));
                } else // UTF_16
                {
                    int u16len = br.ReadUInt16();
                    if ((u16len & 0x8000) != 0) {// larger than 32768
                        u16len = ((u16len & 0x7FFF) << 16) + br.ReadUInt16();
                    }

                    return Encoding.Unicode.GetString(br.ReadBytes(u16len * 2));
                }
            }
        }

        /// <summary>
        /// Get culture info from string
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private CultureInfo getCulture(byte[] code) {
            string language;
            string country;
            byte[] decode;
            if (code[1] > 0x80) { // ISO-639-2
                decode = new byte[3];
                decode[0] = (byte)(code[0] & 0x1F);
                decode[1] = (byte)(((code[1] & 0x3) << 3) + (code[0] & 0xE0) >> 1);
                decode[2] = (byte)((code[1] & 0x7C) >> 2);
            } else { //ISO-639-1
                decode = new byte[2];
                decode[0] = code[0];
                decode[1] = code[1];
            }
            language = System.Text.Encoding.ASCII.GetString(decode);
            decode = new byte[2];
            if (code[3] > 0x80) {
                decode[0] = (byte)(code[2] & 0x1F);
                decode[1] = (byte)(((code[3] & 0x3) << 3) + (code[2] & 0xE0) >> 1);
            } else {
                decode[0] = code[0];
                decode[1] = code[1];
            }
            country = System.Text.Encoding.ASCII.GetString(decode);            
            return new CultureInfo(country + "-" + language);
        }

        /// <summary>
        /// get all configuations
        /// </summary>
        /// <returns>list of configurations</returns>
        private List<byte[]> getResourceConfigs() {
            List<byte[]> result = new List<byte[]>();

            using (MemoryStream ms = new MemoryStream(resources))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // jump type/headersize/chunksize
                int packageCount = br.ReadInt32();
                // comes to stringpool chunk, skipit
                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                int stringPoolSize = br.ReadInt32();
                ms.Seek(stringPoolSize - 8, SeekOrigin.Current); // jump to the end

                //Package chunk now
                for (int pack = 0; pack < packageCount; pack++) {
                    long PackChunkPos = ms.Position;
                    ms.Seek(2, SeekOrigin.Current); // jump type/headersize
                    int headerSize = br.ReadInt16();
                    int PackChunkSize = br.ReadInt32();
                    int packID = br.ReadInt32();

                    ms.Seek(PackChunkPos + headerSize, SeekOrigin.Begin);

                    // skip typestring chunk
                    ms.Seek(4, SeekOrigin.Current);
                    ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end
                    // skip keystring chunk
                    ms.Seek(4, SeekOrigin.Current);
                    ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end

                    do {
                        long chunkPos = ms.Position;
                        short chunkType = br.ReadInt16();
                        headerSize = br.ReadInt16();
                        int chunkSize = br.ReadInt32();
                        if (chunkType == (short)RES_TYPE.RES_TABLE_TYPE_TYPE) {
                            ms.Seek(4 + 4 + 4, SeekOrigin.Current); // skip typeid ,0, entrycount, entrystart

                            // read the config section
                            int config_size = br.ReadInt32();
                            byte[] config = br.ReadBytes(config_size - 4);
                            bool match = false;
                            foreach (byte[] conf in result) {
                                match = true;
                                for (int i = 0; i < conf.Length; i++) {
                                    if (conf[i] != config[i]) {
                                        match = false;
                                        break;
                                    }
                                }
                                if (match)
                                    break;
                            };
                            if (match == false)
                                result.Add(config);
                        }
                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip this chunk
                    } while (ms.Position < PackChunkPos + PackChunkSize);
                }
            }
            return result;
        }

        /// <summary>
        /// Find the requested resource, according to config setting, if the config was set.
        /// This method is NOT HANDLING ANY ERROR, yet!!!!
        /// </summary>
        /// <param name="id">resourceID</param>
        /// <returns>the resource, in string format, if resource id not found, return null value</returns>
        private string QuickSearchResource(UInt32 id, byte[] config = null) {
            uint PackageID = (id & 0xff000000) >> 24;
            uint TypeID = (id & 0x00ff0000) >> 16;
            uint EntryID = (id & 0x0000ffff);

            using (MemoryStream ms = new MemoryStream(resources))
            using (BinaryReader br = new BinaryReader(ms)) {
                ms.Seek(8, SeekOrigin.Begin); // jump type/headersize/chunksize
                int packageCount = br.ReadInt32();
                // comes to stringpool chunk, skipit
                long stringPoolPos = ms.Position;
                ms.Seek(4, SeekOrigin.Current);
                int stringPoolSize = br.ReadInt32();
                ms.Seek(stringPoolSize - 8, SeekOrigin.Current); // jump to the end

                //Package chunk now
                for (int pack = 0; pack < packageCount; pack++) {
                    long PackChunkPos = ms.Position;
                    ms.Seek(2, SeekOrigin.Current); // jump type/headersize
                    int headerSize = br.ReadInt16();
                    int PackChunkSize = br.ReadInt32();
                    int packID = br.ReadInt32();

                    if (packID != PackageID) { // check if the resource is in this pack
                        // goto next chunk
                        ms.Seek(PackChunkPos + PackChunkSize, SeekOrigin.Begin);
                        continue;
                    } else {
                        //ms.Seek(128*2, SeekOrigin.Current); // skip name
                        //int typeStringsPos = br.ReadInt32();
                        //ms.Seek(4,SeekOrigin.Current);    // skip lastpublictype
                        //int keyStringsPos = br.ReadInt32();
                        //ms.Seek(4, SeekOrigin.Current);  // skip lastpublickey
                        ms.Seek(PackChunkPos + headerSize, SeekOrigin.Begin);

                        // skip typestring chunk
                        ms.Seek(4, SeekOrigin.Current);
                        ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end
                        // skip keystring chunk
                        ms.Seek(4, SeekOrigin.Current);
                        ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end

                        // come to typespec chunks and type chunks
                        // typespec and type chunks may happen in a row.
                        do {
                            long chunkPos = ms.Position;
                            short chunkType = br.ReadInt16();
                            headerSize = br.ReadInt16();
                            int chunkSize = br.ReadInt32();
                            byte typeid;
                            //if (chunkType == (short)RES_TYPE.RES_TABLE_TYPE_SPEC_TYPE) {
                            //    typeid = br.ReadByte();
                            //    if (typeid == TypeID) {
                            //        // todo: get the flags
                            //    }
                            //} else 
                            if (chunkType == (short)RES_TYPE.RES_TABLE_TYPE_TYPE) {
                                typeid = br.ReadByte();
                                if (typeid == TypeID) {
                                    ms.Seek(3, SeekOrigin.Current); // skip 0
                                    int entryCount = br.ReadInt32();
                                    int entryStart = br.ReadInt32();

                                    // read the config section
                                    int config_size = br.ReadInt32();
                                    byte[] conf = br.ReadBytes(config_size - 4);

                                    if (config != null) {
                                        bool match = true;
                                        for (int i = 0; i < config.Length; i++) {
                                            if (conf[i] != config[i]) {
                                                match = false;
                                                break;
                                            }
                                        }
                                        if (match == false) {// config does not fit, jump to next chunk
                                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                            continue;
                                        }
                                    }

                                    ms.Seek(EntryID * 4, SeekOrigin.Current); // goto index
                                    uint entryIndic = br.ReadUInt32();
                                    if (entryIndic == 0xffffffff) {
                                        ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                        continue; //no entry here, go to next chunk
                                    }
                                    ms.Seek(chunkPos + entryStart + entryIndic, SeekOrigin.Begin);

                                    // get to the entry
                                    ms.Seek(11, SeekOrigin.Current); // skip entry size, flags, key, size, 0
                                    byte dataType = br.ReadByte();
                                    uint data = br.ReadUInt32();
                                    if (config != null)
                                        SharpShell.Diagnostics.Logging.Log(string.Format("Extracting Resource {0} using density {1}:",id,config[ConfigurationDensityPosition]));
                                    if (dataType == (byte)DATA_TYPE.TYPE_STRING) {
                                        return QuickSearchResourcesStringPool(data);
                                    } else if (dataType == (byte)DATA_TYPE.TYPE_REFERENCE) {
                                        // the entry is null, or it's referencing itself, go to next chunk
                                        if (data == 0x00000000 || data == id) {
                                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                            continue;
                                        }
                                        return QuickSearchResource((UInt32)data, config);
                                    } else { // I would like to expect we only will recieve TYPE_STRING/TYPE_REFERENCE/any integer type, complex is not considering here,yet
                                        return data.ToString();
                                    }
                                }
                            //} else {
                            //    // chunk Type is not what we want.
                            }
                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip this chunk
                        } while (ms.Position < PackChunkPos + PackChunkSize);
                        if (config != null) // no config fits, search default
                            return QuickSearchResource(id);
                    }
                }
            }
            return null;
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing) {
            if (disposed) return;
            if (disposing) {
                resources = null;
                manifest = null;
                if (zip != null)
                    zip.Close();
            }
            disposed = true;
            base.Dispose(disposing);
        }

        public void Close() {
            Dispose(true);
        }

        ~ApkReader() {
            Dispose(true);
        }
    }

    /// <summary>
    /// ChunkInfo class
    /// </summary>
    //[DebuggerDisplay("{Type}")]
    //public class ApkChunkInfo : IEnumerable {
    //    public long Offset { get; private set; }
    //    public RES_TYPE Type { get; private set; }
    //    public UInt32 Size;
    //    public List<ApkChunkInfo> subChunks = new List<ApkChunkInfo>();
    //    public ApkChunkInfo parentChunk = null;
    //    public UInt16 headerSize;

    //    public MemoryStream baseStream { get; private set; }

    //    public static ApkChunkInfo FromMemoryStream(MemoryStream stream) {
    //        return new ApkChunkInfo(stream);
    //    }

    //    public void AttachStream(MemoryStream stream) {
    //        if (stream != null) {
    //            baseStream = stream;
    //            baseStream.Seek(Offset, SeekOrigin.Begin);
    //        }
    //    }

    //    private ApkChunkInfo(MemoryStream stream) {
    //        Offset = stream.Position;
    //        AttachStream(stream);
    //        using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true)) {

    //            Type = (RES_TYPE)br.ReadUInt16();
    //            headerSize = br.ReadUInt16();
    //            Size = br.ReadUInt32();

    //            // skip whole header
    //            stream.Seek(Offset + headerSize, SeekOrigin.Begin);
    //            // and come to the chunk body

    //            switch (Type) {
    //                case RES_TYPE.RES_TABLE_TYPE:
    //                case RES_TYPE.RES_TABLE_PACKAGE_TYPE:
    //                case RES_TYPE.RES_XML_TYPE:
    //                    // Get subChunks
    //                    while (stream.Position < (Offset + Size)) {
    //                        ApkChunkInfo sub = new ApkChunkInfo(stream);
    //                        sub.parentChunk = this;
    //                        subChunks.Add(sub);
    //                    }
    //                    break;
    //                default:
    //                    stream.Seek(Offset + Size, SeekOrigin.Begin);
    //                    break;
    //            }
    //            if (stream.Position != Offset + Size) {
    //                throw new Exception("Read through the end of chunk, but not match the ChunkSize");
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// find the first chunk fits specific type in sub chunks 
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public ApkChunkInfo findFirstSubChunk(RES_TYPE type) {
    //        foreach (ApkChunkInfo chunk in subChunks) {
    //            if (chunk.Type == type) {
    //                chunk.AttachStream(baseStream);
    //                return chunk;
    //            }
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    ///  get the next chunk at the same level
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public ApkChunkInfo findNextChunk(RES_TYPE type) {
    //        foreach (ApkChunkInfo chunk in parentChunk.subChunks) {
    //            if (chunk.Offset > Offset && chunk.Type == type) {
    //                return chunk;
    //            }
    //        }
    //        return null;
    //    }

    //    public IEnumerator GetEnumerator() {
    //        throw new NotImplementedException();
    //        //return new ApkChunkEnumerator(BaseStream);
    //    }
    //}
}
