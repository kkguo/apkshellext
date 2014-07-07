using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Iteedee.ApkReader
{
    public class ApkResourceFinder
    {
        private const long HEADER_START = 0;
        static short RES_STRING_POOL_TYPE = 0x0001;
        static short RES_TABLE_TYPE = 0x0002;
        static short RES_TABLE_PACKAGE_TYPE = 0x0200;
        static short RES_TABLE_TYPE_TYPE = 0x0201;
        static short RES_TABLE_TYPE_SPEC_TYPE = 0x0202;

        String[] valueStringPool = null;
        String[] typeStringPool = null;
        String[] keyStringPool = null;

        private int package_id = 0;
        private List<String> resIdList;

        //// Contains no data.
        //static byte TYPE_NULL = 0x00;
        //// The 'data' holds an attribute resource identifier.
        //static byte TYPE_ATTRIBUTE = 0x02;
        //// The 'data' holds a single-precision floating point number.
        //static byte TYPE_FLOAT = 0x04;
        //// The 'data' holds a complex number encoding a dimension value,
        //// such as "100in".
        //static byte TYPE_DIMENSION = 0x05;
        //// The 'data' holds a complex number encoding a fraction of a
        //// container.
        //static byte TYPE_FRACTION = 0x06;
        //// The 'data' is a raw integer value of the form n..n.
        //static byte TYPE_INT_DEC = 0x10;
        //// The 'data' is a raw integer value of the form 0xn..n.
        //static byte TYPE_INT_HEX = 0x11;
        //// The 'data' is either 0 or 1, for input "false" or "true" respectively.
        //static byte TYPE_INT_BOOLEAN = 0x12;
        //// The 'data' is a raw integer value of the form #aarrggbb.
        //static byte TYPE_INT_COLOR_ARGB8 = 0x1c;
        //// The 'data' is a raw integer value of the form #rrggbb.
        //static byte TYPE_INT_COLOR_RGB8 = 0x1d;
        //// The 'data' is a raw integer value of the form #argb.
        //static byte TYPE_INT_COLOR_ARGB4 = 0x1e;
        //// The 'data' is a raw integer value of the form #rgb.
        //static byte TYPE_INT_COLOR_RGB4 = 0x1f;

        // The 'data' holds a ResTable_ref, a reference to another resource
        // table entry.
        static byte TYPE_REFERENCE = 0x01;
        // The 'data' holds an index into the containing resource table's
        // global value string pool.
        static byte TYPE_STRING = 0x03;



        private Dictionary<String, List<String>> responseMap;

        Dictionary<int, List<String>> entryMap = new Dictionary<int, List<String>>();

        public Dictionary<string, List<String>> initialize()
        {
            byte[] data = System.IO.File.ReadAllBytes("resources.arsc");
            return this.processResourceTable(data, new List<string>());
        }
        public Dictionary<string, List<String>> processResourceTable(byte[] data, List<String> resIdList)
        {
            this.resIdList = resIdList;

            responseMap = new Dictionary<string, List<String>>();
            long lastPosition;

            using (MemoryStream ms = new MemoryStream(data))
            {

                using (BinaryReader br = new BinaryReader(ms))
                {

                    short type = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int size = br.ReadInt32();
                    int packageCount = br.ReadInt32();


                    if (type != RES_TABLE_TYPE)
                    {
                        throw new Exception("No RES_TABLE_TYPE found!");
                    }
                    if (size != br.BaseStream.Length)
                    {
                        throw new Exception(
                                        "The buffer size not matches to the resource table size.");
                    }

                    int realStringPoolCount = 0;
                    int realPackageCount = 0;


                    while (true)
                    {
                        long pos = br.BaseStream.Position;
                        short t = br.ReadInt16();
                        short hs = br.ReadInt16();
                        int s = br.ReadInt32();

                        if (t == RES_STRING_POOL_TYPE)
                        {
                            if (realStringPoolCount == 0)
                            {
                                // Only the first string pool is processed.
                                Debug.WriteLine("Processing the string pool ...");


                                byte[] buffer = new byte[s];
                                lastPosition = br.BaseStream.Position;
                                br.BaseStream.Seek(pos, SeekOrigin.Begin);
                                buffer = br.ReadBytes(s);
                                //br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);

                                valueStringPool = processStringPool(buffer);

                            }
                            realStringPoolCount++;

                        }
                        else if (t == RES_TABLE_PACKAGE_TYPE)
                        {
                            // Process the package
                            Debug.WriteLine("Processing package {0} ...", realPackageCount);

                            byte[] buffer = new byte[s];
                            lastPosition = br.BaseStream.Position;
                            br.BaseStream.Seek(pos, SeekOrigin.Begin);
                            buffer = br.ReadBytes(s);
                            //br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                            processPackage(buffer);

                            realPackageCount++;

                        }
                        else
                        {
                            throw new InvalidOperationException("Unsupported Type");
                        }
                        br.BaseStream.Seek(pos + (long)s, SeekOrigin.Begin);
                        if (br.BaseStream.Position == br.BaseStream.Length)
                            break;

                    }

                    if (realStringPoolCount != 1)
                    {
                        throw new Exception("More than 1 string pool found!");
                    }
                    if (realPackageCount != packageCount)
                    {
                        throw new Exception(
                                        "Real package count not equals the declared count.");
                    }

                    return responseMap;

                }
            }

        }

        private void processPackage(byte[] data)
        {
            long lastPosition = 0;
            using (MemoryStream ms = new MemoryStream(data))
            {

                using (BinaryReader br = new BinaryReader(ms))
                {
                    //HEADER
                    short type = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int size = br.ReadInt32();

                    int id = br.ReadInt32();
                    package_id = id;

                    //PackageName
                    char[] name = new char[256];
                    for (int i = 0; i < 256; ++i)
                    {
                        name[i] = br.ReadChar();
                    }
                    int typeStrings = br.ReadInt32();
                    int lastPublicType = br.ReadInt32();
                    int keyStrings = br.ReadInt32();
                    int lastPublicKey = br.ReadInt32();

                    if (typeStrings != headerSize)
                    {
                        throw new Exception("TypeStrings must immediately follow the package structure header.");
                    }

                    Debug.WriteLine("Type strings:");
                    lastPosition = br.BaseStream.Position;
                    br.BaseStream.Seek(typeStrings, SeekOrigin.Begin);
                    byte[] bbTypeStrings = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);

                    typeStringPool = processStringPool(bbTypeStrings);

                    Debug.WriteLine("Key strings:");

                    br.BaseStream.Seek(keyStrings, SeekOrigin.Begin);
                    short key_type = br.ReadInt16();
                    short key_headerSize = br.ReadInt16();
                    int key_size = br.ReadInt32();

                    lastPosition = br.BaseStream.Position;
                    br.BaseStream.Seek(keyStrings, SeekOrigin.Begin);
                    byte[] bbKeyStrings = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                    br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);

                    keyStringPool = processStringPool(bbKeyStrings);



                    // Iterate through all chunks
                    //
                    int typeSpecCount = 0;
                    int typeCount = 0;

                    br.BaseStream.Seek((keyStrings + key_size), SeekOrigin.Begin);

                    while (true)
                    {
                        int pos = (int)br.BaseStream.Position;
                        short t = br.ReadInt16();
                        short hs = br.ReadInt16();
                        int s = br.ReadInt32();

                        if (t == RES_TABLE_TYPE_SPEC_TYPE)
                        {
                            // Process the string pool
                            byte[] buffer = new byte[s];
                            br.BaseStream.Seek(pos, SeekOrigin.Begin);
                            buffer = br.ReadBytes(s);

                            processTypeSpec(buffer);

                            typeSpecCount++;
                        }
                        else if (t == RES_TABLE_TYPE_TYPE)
                        {
                            // Process the package
                            byte[] buffer = new byte[s];
                            br.BaseStream.Seek(pos, SeekOrigin.Begin);
                            buffer = br.ReadBytes(s);

                            processType(buffer);

                            typeCount++;
                        }

                        br.BaseStream.Seek(pos + s, SeekOrigin.Begin);
                        if (br.BaseStream.Position == br.BaseStream.Length)
                            break;
                    }

                    return;

                }
            }

        }
        private void putIntoMap(String resId, String value)
        {
            List<String> valueList = null;
            if (responseMap.ContainsKey(resId.ToUpper()))
                valueList = responseMap[resId.ToUpper()];
            if (valueList == null)
            {
                valueList = new List<String>();
            }
            valueList.Add(value);
            if (responseMap.ContainsKey(resId.ToUpper()))
                responseMap[resId.ToUpper()] = valueList;
            else
                responseMap.Add(resId.ToUpper(), valueList);
            return;

        }

        private void processType(byte[] typeData)
        {
            using (MemoryStream ms = new MemoryStream(typeData))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    short type = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int size = br.ReadInt32();
                    byte id = br.ReadByte();
                    byte res0 = br.ReadByte();
                    short res1 = br.ReadInt16();
                    int entryCount = br.ReadInt32();
                    int entriesStart = br.ReadInt32();

                    Dictionary<String, int> refKeys = new Dictionary<String, int>();

                    int config_size = br.ReadInt32();

                    // Skip the config data
                    br.BaseStream.Seek(headerSize, SeekOrigin.Begin);


                    if (headerSize + entryCount * 4 != entriesStart)
                    {
                        throw new Exception("HeaderSize, entryCount and entriesStart are not valid.");
                    }

                    // Start to get entry indices
                    int[] entryIndices = new int[entryCount];
                    for (int i = 0; i < entryCount; ++i)
                    {
                        entryIndices[i] = br.ReadInt32();
                    }

                    // Get entries
                    for (int i = 0; i < entryCount; ++i)
                    {
                        if (entryIndices[i] == -1)
                            continue;

                        int resource_id = (package_id << 24) | (id << 16) | i;

                        long pos = br.BaseStream.Position;
                        short entry_size = br.ReadInt16();
                        short entry_flag = br.ReadInt16();
                        int entry_key = br.ReadInt32();

                        // Get the value (simple) or map (complex)
                        int FLAG_COMPLEX = 0x0001;

                        if ((entry_flag & FLAG_COMPLEX) == 0)
                        {
                            // Simple case
                            short value_size = br.ReadInt16();
                            byte value_res0 = br.ReadByte();
                            byte value_dataType = br.ReadByte();
                            int value_data = br.ReadInt32();

                            String idStr = resource_id.ToString("X4");
                            String keyStr = keyStringPool[entry_key];
                            String data = null;

                            Debug.WriteLine("Entry 0x" + idStr + ", key: " + keyStr + ", simple value type: ");

                            List<String> entryArr = null;
                            if (entryMap.ContainsKey(int.Parse(idStr, System.Globalization.NumberStyles.HexNumber)))
                                entryArr = entryMap[int.Parse(idStr, System.Globalization.NumberStyles.HexNumber)];

                            if (entryArr == null)
                                entryArr = new List<String>();

                            entryArr.Add(keyStr);
                            if (entryMap.ContainsKey(int.Parse(idStr, System.Globalization.NumberStyles.HexNumber)))
                                entryMap[int.Parse(idStr, System.Globalization.NumberStyles.HexNumber)] = entryArr;
                            else
                                entryMap.Add(int.Parse(idStr, System.Globalization.NumberStyles.HexNumber), entryArr);

                            if (value_dataType == TYPE_STRING)
                            {
                                data = valueStringPool[value_data];
                                Debug.WriteLine(", data: " + valueStringPool[value_data] + "");
                            }
                            else if (value_dataType == TYPE_REFERENCE)
                            {
                                String hexIndex = value_data.ToString("X4");
                                refKeys.Add(idStr, value_data);
                            }
                            else
                            {
                                data = value_data.ToString();
                                Debug.WriteLine(", data: " + value_data + "");
                            }

                            // if (inReqList("@" + idStr)) {
                            putIntoMap("@" + idStr, data);
                        }
                        else
                        {
                            int entry_parent = br.ReadInt32();
                            int entry_count = br.ReadInt32();

                            for (int j = 0; j < entry_count; ++j)
                            {
                                int ref_name = br.ReadInt32();
                                short value_size = br.ReadInt16();
                                byte value_res0 = br.ReadByte();
                                byte value_dataType = br.ReadByte();
                                int value_data = br.ReadInt32();
                            }

                            Debug.WriteLine("Entry 0x"
                                                    + resource_id.ToString("X4") + ", key: "
                                                    + keyStringPool[entry_key]
                                                    + ", complex value, not printed.");
                        }

                    }
                    HashSet<String> refKs = new HashSet<String>(refKeys.Keys);

                    foreach (String refK in refKs)
                    {
                        List<String> values = null;
                        if (responseMap.ContainsKey("@" + refKeys[refK].ToString("X4").ToUpper()))
                            values = responseMap["@" + refKeys[refK].ToString("X4").ToUpper()];

                        if (values != null)
                            foreach (String value in values)
                            {
                                putIntoMap("@" + refK, value);
                            }
                    }
                    return;

                }
            }
        }



        private string[] processStringPool(byte[] data)
        {
            long lastPosition = 0;

            using (MemoryStream ms = new MemoryStream(data))
            {

                using (BinaryReader br = new BinaryReader(ms))
                {
                    short type = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int size = br.ReadInt32();
                    int stringCount = br.ReadInt32();
                    int styleCount = br.ReadInt32();
                    int flags = br.ReadInt32();
                    int stringsStart = br.ReadInt32();
                    int stylesStart = br.ReadInt32();

                    bool isUTF_8 = (flags & 256) != 0;

                    int[] offsets = new int[stringCount];
                    for (int i = 0; i < stringCount; ++i)
                    {
                        offsets[i] = br.ReadInt32();
                    }
                    String[] strings = new String[stringCount];

                    for (int i = 0; i < stringCount; i++)
                    {
                        int pos = stringsStart + offsets[i];
                        lastPosition = br.BaseStream.Position;
                        short len = (short)br.BaseStream.Seek(pos, SeekOrigin.Begin);
                        br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);

                        if (len < 0)
                        {
                            short extendShort = br.ReadInt16();
                        }
                        pos += 2;
                        strings[i] = "";
                        if (isUTF_8)
                        {
                            int start = pos;
                            int length = 0;
                            lastPosition = br.BaseStream.Position;
                            br.BaseStream.Seek(pos, SeekOrigin.Begin);
                            while (br.ReadByte() != 0)
                            {
                                length++;
                                pos++;
                            }
                            br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);

                            byte[] oneData = new byte[length];
                            if (length > 0)
                            {
                                byte[] byteArray = data;
                                for (int k = 0; k < length; k++)
                                {
                                    oneData[k] = byteArray[start + k];
                                }
                            }
                            if (oneData.Length > 0)
                                strings[i] = Encoding.UTF8.GetString(oneData);
                            else
                                strings[i] = "";
                        }
                        else
                        {
                            char c;
                            lastPosition = br.BaseStream.Position;
                            br.BaseStream.Seek(pos, SeekOrigin.Begin);
                            while ((c = br.ReadChar()) != 0)
                            {
                                strings[i] += c;
                                br.ReadChar();
                                pos += 2;
                            }
                            br.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                        }
                        Debug.WriteLine("Parsed value: {0}", strings[i]);


                    }
                    return strings;

                }
            }
        }

        private void processTypeSpec(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {

                using (BinaryReader br = new BinaryReader(ms))
                {
                    short type = br.ReadInt16();
                    short headerSize = br.ReadInt16();
                    int size = br.ReadInt32();
                    byte id = br.ReadByte();
                    byte res0 = br.ReadByte();
                    short res1 = br.ReadInt16();
                    int entryCount = br.ReadInt32();


                    Debug.WriteLine("Processing type spec {0}", typeStringPool[id - 1]);

                    int[] flags = new int[entryCount];
                    for (int i = 0; i < entryCount; ++i)
                    {
                        flags[i] = br.ReadInt32();
                    }

                    return;
                }
            }
        }

    }
}
