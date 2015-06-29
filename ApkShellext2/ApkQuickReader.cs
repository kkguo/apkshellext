using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;

namespace ApkShellext2 {
    class ApkQuickReader {
        public string FileName { get; set; }

        enum RES_TYPE : short {
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

        enum DATA_TYPE : byte {
            TYPE_REFERENCE = 0x01,
            TYPE_STRING = 0x03,
        }

        private byte[] resources;
        private byte[] manifest;

        enum CHUNKS {
            MANIFEST,
            RESOURCES
        }
        public ApkQuickReader(string filename) {
            FileName = filename;
            using (ZipFile zipfile = new ZipFile(filename)) {

                ZipEntry en = zipfile.GetEntry("androidmanifest.xml");
                if (en != null) {
                    BinaryReader s = new BinaryReader(zipfile.GetInputStream(en));
                    manifest = s.ReadBytes((int)en.Size);
                }

                en = zipfile.GetEntry("resources.arsc");
                if (en != null) {
                    BinaryReader s = new BinaryReader(zipfile.GetInputStream(en));
                    resources = s.ReadBytes((int)en.Size);
                }
            }


        }

        public string getAttribute(string tag, string attr) {
            return QuickSearchManifest(tag, attr);
        }

        public Bitmap getImage(string path) {
            using (ZipFile zipfile = new ZipFile(this.FileName)) {
                ZipEntry en = zipfile.GetEntry(path);
                if (en != null) {
                    try {
                        return (Bitmap)Bitmap.FromStream(zipfile.GetInputStream(en));
                    } catch {
                        return null;
                    }
                } else {
                    return null;
                }
            }
        }

        private string QuickSearchManifest(string tag, string attribute) {
            using (MemoryStream ms = new MemoryStream(manifest)) {
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
                        string tag_s = QuickSearchStringPool(CHUNKS.MANIFEST, br.ReadUInt32());
                        if (tag_s.ToUpper() != tag.ToUpper()) {
                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);// to next tag
                            continue;
                        }
                        int attributeStart = br.ReadInt16();
                        int attributeSize = br.ReadInt16();
                        int attributeCount = br.ReadInt16();
                        for (int i = 0; i < attributeSize; i++) {
                            ms.Seek(chunkPos + headerSize + attributeStart + attributeSize * i + 4, SeekOrigin.Begin); // ignore the ns                            
                            string name = QuickSearchStringPool(CHUNKS.MANIFEST, br.ReadUInt32());
                            if (name.ToUpper() == attribute.ToUpper()) {
                                ms.Seek(4 + 2 + 1, SeekOrigin.Current); // skip rawValue/size/0/
                                byte dataType = br.ReadByte();
                                uint data = br.ReadUInt32();
                                if (dataType == (byte)DATA_TYPE.TYPE_STRING) {
                                    return QuickSearchStringPool(CHUNKS.MANIFEST, data);
                                } else if (dataType == (byte)DATA_TYPE.TYPE_REFERENCE) {
                                    return QuickSearchResource((UInt32)data);
                                } else { // I would like to expect we only will recieve TYPE_STRING/TYPE_REFERENCE/any integer type, complex is not considering here,yet
                                    return data.ToString();
                                }
                            }
                        }
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Search string pool within 2 tables.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stringID"></param>
        /// <returns></returns>
        private string QuickSearchStringPool(CHUNKS table, uint stringID) {
            byte[] t = (table == CHUNKS.MANIFEST) ? manifest : resources;
            using (MemoryStream ms = new MemoryStream(t)) {
                using (BinaryReader br = new BinaryReader(ms)) {
                    // the first chunk is always stringpool for manifest and resources
                    ms.Seek(2, SeekOrigin.Begin);
                    short headerSize = br.ReadInt16();
                    ms.Seek(headerSize, SeekOrigin.Begin);
                    return QuickSearchStringPool(ms, stringID);
                }
            }
        }

        /// <summary>
        /// Search string pool within existing stream.
        /// </summary>
        /// <param name="ms">the stream position should be at StringPool entrance</param>
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
        /// Find the requested resource in Quickest way,   
        /// This method is NOT HANDLING ANY ERROR, yet!!!!
        /// </summary>
        /// <param name="table">the resource table</param>
        /// <param name="id">resourceID</param>
        /// <param name="config">optional, find the resource for specific config, the config contains 
        /// size, imsi, locale, screenType, input, screenSize, version, screenConfig
        /// if any config value is 0, that item will be ignored
        /// if no config set fits in type chunk, return null value
        /// if no config set provided, returns the value in first type chunk (most likely it's the default config)
        /// </param>
        /// <returns>the resource, in string format, if resource id not found, return null value</returns>
        private string QuickSearchResource(UInt32 id, int[] config = null) {
            string result = null;
            uint PackageID = (id & 0xff000000) >> 24;
            uint TypeID = (id & 0x00ff0000) >> 16;
            uint EntryID = (id & 0x0000ffff);

            using (MemoryStream ms = new MemoryStream(resources)) {
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
                        long chunkPos = ms.Position;
                        ms.Seek(2, SeekOrigin.Current); // jump type/headersize
                        int headerSize = br.ReadInt16();
                        int chunkSize = br.ReadInt32();
                        int packID = br.ReadInt32();

                        if (packID != PackageID) { // check if the resource is in this pack
                            // goto next chunk
                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                            continue;
                        } else {
                            //ms.Seek(128*2, SeekOrigin.Current); // skip name
                            //int typeStringsPos = br.ReadInt32();
                            //ms.Seek(4,SeekOrigin.Current);    // skip lastpublictype
                            //int keyStringsPos = br.ReadInt32();
                            //ms.Seek(4, SeekOrigin.Current);  // skip lastpublickey
                            ms.Seek(chunkPos + headerSize, SeekOrigin.Begin);

                            // skip typestring chunk
                            ms.Seek(4, SeekOrigin.Current);
                            ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end
                            // skip keystring chunk
                            ms.Seek(4, SeekOrigin.Current);
                            ms.Seek(br.ReadInt32() - 8, SeekOrigin.Current); // jump to the end

                            //come to typespec chunks
                            do {
                                chunkPos = ms.Position;
                                short chunkType = br.ReadInt16();
                                ms.Seek(2, SeekOrigin.Current);
                                chunkSize = br.ReadInt32();
                                if (chunkType != (short)RES_TYPE.RES_TABLE_TYPE_SPEC_TYPE) { // find the type spec
                                    ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                    continue;
                                }
                                byte typeid = br.ReadByte();
                                if (typeid != TypeID) {
                                    ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin); // skip current typespec
                                    continue;
                                }
                                ms.Seek(3 + 4, SeekOrigin.Current); // skip res0/res1
                                //int entrycount = br.ReadInt32();         // TypeID < entrycount
                                //int flag = br.ReadInt32();
                                ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);

                                // comes to first type chunk
                                do {
                                    chunkPos = ms.Position;
                                    chunkType = br.ReadInt16();
                                    if (chunkType != (short)RES_TYPE.RES_TABLE_TYPE_TYPE) // entry not found
                                        return null;
                                    headerSize = br.ReadInt16();
                                    chunkSize = br.ReadInt32();
                                    typeid = br.ReadByte();
                                    ms.Seek(3, SeekOrigin.Current); // skip 0
                                    int entryCount = br.ReadInt32();
                                    int entryStart = br.ReadInt32();

                                    // skip config section
                                    ms.Seek(chunkPos + headerSize, SeekOrigin.Begin);
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
                                    if (dataType == (byte)DATA_TYPE.TYPE_STRING) {
                                        return QuickSearchStringPool(CHUNKS.RESOURCES, data);
                                    } else if (dataType == (byte)DATA_TYPE.TYPE_REFERENCE) {
                                        if (data == 0x00000000) {
                                            ms.Seek(chunkPos + chunkSize, SeekOrigin.Begin);
                                            continue; // the entry is null, go to next chunk
                                        }
                                        return QuickSearchResource((UInt32)data);
                                    } else { // I would like to expect we only will recieve TYPE_STRING/TYPE_REFERENCE/any integer type, complex is not considering here,yet
                                        return data.ToString();
                                    }
                                } while (true);
                            } while (true);
                        }
                    }
                }
            }
            return result;
        }
    }
}
