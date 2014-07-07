using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Iteedee.ApkReader
{
    public class ApkReader
    {
        //private static Logger log = Logger.getLogger("APKReader");

        private static int VER_ID = 0;
        private static int ICN_ID = 1;
        private static int LABEL_ID = 2;
        String[] VER_ICN = new String[3];

        // Some possible tags and attributes
        String[] TAGS = { "manifest", "application", "activity" };
        String[] ATTRS = { "android:", "a:", "activity:", "_:" };

        Dictionary<String, object> entryList = new Dictionary<String, object>();

        List<String> tmpFiles = new List<String>();

        public String fuzzFindInDocument(XmlDocument doc, String tag, String attr)
        {
            foreach (String t in TAGS)
            {
                XmlNodeList nodelist = doc.GetElementsByTagName(t);
                for (int i = 0; i < nodelist.Count; i++)
                {
                    XmlNode element = (XmlNode)nodelist.Item(i);
                    if (element.NodeType == XmlNodeType.Element)
                    {
                        XmlAttributeCollection map = element.Attributes;
                        for (int j = 0; j < map.Count; j++)
                        {
                            XmlNode element2 = map.Item(j);
                            if (element2.Name.EndsWith(attr))
                            {
                                return element2.Value;
                            }
                        }
                    }
                }
            }
            return null;
        }


        private XmlDocument initDoc(String xml)
        {
            XmlDocument retval = new XmlDocument();
            retval.LoadXml(xml);
            retval.DocumentElement.Normalize();
            return retval;
        }


        private void extractPermissions(ApkInfo info, XmlDocument doc)
        {
            ExtractPermission(info, doc, "uses-permission", "name");
            ExtractPermission(info, doc, "permission-group", "name");
            ExtractPermission(info, doc, "service", "permission");
            ExtractPermission(info, doc, "provider", "permission");
            ExtractPermission(info, doc, "activity", "permission");
        }
        private bool readBoolean(XmlDocument doc, String tag, String attribute)
        {
            String str = FindInDocument(doc, tag, attribute);
            bool ret = false;
            try
            {
                ret = Convert.ToBoolean(str);
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        private void extractSupportScreens(ApkInfo info, XmlDocument doc)
        {
            info.supportSmallScreens = readBoolean(doc, "supports-screens", "android:smallScreens");
            info.supportNormalScreens = readBoolean(doc, "supports-screens", "android:normalScreens");
            info.supportLargeScreens = readBoolean(doc, "supports-screens", "android:largeScreens");

            if (info.supportSmallScreens || info.supportNormalScreens || info.supportLargeScreens)
                info.supportAnyDensity = false;
        }
        public ApkInfo extractInfo(byte[] manifest_xml, byte[] resources_arsx)
        {
            string manifestXml = string.Empty;
            APKManifest manifest = new APKManifest();
            try
            {
                manifestXml = manifest.ReadManifestFileIntoXml(manifest_xml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(manifestXml);
            return extractInfo(doc, resources_arsx);

        }
        public ApkInfo extractInfo(XmlDocument manifestXml, byte[] resources_arsx)
        {
            ApkInfo info = new ApkInfo();
            VER_ICN[VER_ID] = "";
            VER_ICN[ICN_ID] = "";
            VER_ICN[LABEL_ID] = "";
            try
            {
                XmlDocument doc = manifestXml;
                if (doc == null)
                    throw new Exception("Document initialize failed");
                info.resourcesFileName = "resources.arsx";
                info.resourcesFileBytes = resources_arsx;
                // Fill up the permission field
                extractPermissions(info, doc);

                // Fill up some basic fields
                info.minSdkVersion = FindInDocument(doc, "uses-sdk", "minSdkVersion");
                info.targetSdkVersion = FindInDocument(doc, "uses-sdk", "targetSdkVersion");
                info.versionCode = FindInDocument(doc, "manifest", "versionCode");
                info.versionName = FindInDocument(doc, "manifest", "versionName");
                info.packageName = FindInDocument(doc, "manifest", "package");
                info.label = FindInDocument(doc, "application", "label");
                if (info.label.StartsWith("@"))
                    VER_ICN[LABEL_ID] = info.label;
                else
                    VER_ICN[LABEL_ID] = String.Format("@{0}", Convert.ToInt32(info.label).ToString("X4"));

                // Fill up the support screen field
                extractSupportScreens(info, doc);

                if (info.versionCode == null)
                    info.versionCode = fuzzFindInDocument(doc, "manifest",
                                    "versionCode");

                if (info.versionName == null)
                    info.versionName = fuzzFindInDocument(doc, "manifest",
                                    "versionName");
                else if (info.versionName.StartsWith("@"))
                    VER_ICN[VER_ID] = info.versionName;

                String id = FindInDocument(doc, "application", "android:icon");
                if (null == id)
                {
                    id = fuzzFindInDocument(doc, "manifest", "icon");
                }

                if (null == id)
                {
                    Debug.WriteLine("icon resId Not Found!");
                    return info;
                }

                // Find real strings
                if (!info.hasIcon && id != null)
                {
                    if (id.StartsWith("@android:"))
                        VER_ICN[ICN_ID] = "@"
                                        + (id.Substring("@android:".Length));
                    else
                        VER_ICN[ICN_ID] = String.Format("@{0}", Convert.ToInt32(id).ToString("X4"));

                    List<String> resId = new List<String>();

                    for (int i = 0; i < VER_ICN.Length; i++)
                    {
                        if (VER_ICN[i].StartsWith("@"))
                            resId.Add(VER_ICN[i]);
                    }

                    ApkResourceFinder finder = new ApkResourceFinder();
                    info.resStrings = finder.processResourceTable(info.resourcesFileBytes, resId);

                    if (!VER_ICN[VER_ID].Equals(""))
                    {
                        List<String> versions = null;
                        if (info.resStrings.ContainsKey(VER_ICN[VER_ID].ToUpper()))
                            versions = info.resStrings[VER_ICN[VER_ID].ToUpper()];
                        if (versions != null)
                        {
                            if (versions.Count > 0)
                                info.versionName = versions[0];
                        }
                        else
                        {
                            throw new Exception(
                                            "VersionName Cant Find in resource with id "
                                                            + VER_ICN[VER_ID]);
                        }
                    }

                    List<String> iconPaths = null;
                    if (info.resStrings.ContainsKey(VER_ICN[ICN_ID].ToUpper()))
                        iconPaths = info.resStrings[VER_ICN[ICN_ID].ToUpper()];
                    if (iconPaths != null && iconPaths.Count > 0)
                    {
                        info.iconFileNameToGet = new List<String>();
                        info.iconFileName = new List<string>();
                        foreach (String iconFileName in iconPaths)
                        {
                            if (iconFileName != null)
                            {
                                if(iconFileName.Contains(@"/"))
                                {
                                    info.iconFileNameToGet.Add(iconFileName);
                                    info.iconFileName.Add(iconFileName);
                                    info.hasIcon = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Icon Cant Find in resource with id "
                                        + VER_ICN[ICN_ID]);
                    }

                    if (!VER_ICN[LABEL_ID].Equals(""))
                    {
                        List<String> labels = null;
                        if (info.resStrings.ContainsKey(VER_ICN[LABEL_ID]))
                            labels = info.resStrings[VER_ICN[LABEL_ID]];
                        if (labels.Count > 0)
                        {
                            info.label = labels[0];
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return info;
        }




        private void ExtractPermission(ApkInfo info, XmlDocument doc, String keyName, String attribName)
        {
            XmlNodeList usesPermissions = doc.GetElementsByTagName(keyName);
            if (usesPermissions != null)
            {
                for (int s = 0; s < usesPermissions.Count; s++)
                {
                    XmlNode permissionNode = usesPermissions.Item(s);
                    if (permissionNode.NodeType == XmlNodeType.Element)
                    {
                        XmlNode node = permissionNode.Attributes.GetNamedItem(attribName);
                        if (node != null)
                            info.Permissions.Add(node.Value);
                    }
                }
            }
        }
        private String FindInDocument(XmlDocument doc, String keyName,
                String attribName)
        {
            XmlNodeList usesPermissions = doc.GetElementsByTagName(keyName);

            if (usesPermissions != null)
            {
                for (int s = 0; s < usesPermissions.Count; s++)
                {
                    XmlNode permissionNode = usesPermissions.Item(s);
                    if (permissionNode.NodeType == XmlNodeType.Element)
                    {
                        XmlNode node = permissionNode.Attributes.GetNamedItem(attribName);
                        if (node != null)
                            return node.Value;
                    }
                }
            }
            return null;
        }

        //  private String normalizeXml(String rawXMl) {
        //        String xml = "";
        //        /* Dealing with ugly content */
        //        foreach (String l in rawXMl.Split(Convert.ToChar("\n"))) {
        //                /* Deal with invalid character & */
        //            string line = l;
        //            line = line.Replace("&", "&amp;");
        //            /* Deal with android:versionName="1.0.3.7-969a */
        //            line = line.Replace((char)0, ' ');
        //            /* Deal with versionName="0.1.8 "Archer"" */
        //            int charCount = Regex.Replace(line, "[^\"]", "").Length;

        //            if (charCount > 2 && !line.Contains("xml version")
        //                            && line.EndsWith("\"")) {
        //                    Regex rx = new Regex("(.+[\\w:=]+)\\\"(.+)\\\"");
        //                    MatchCollection matches = rx.Matches(line);
        //                    if (matches())
        //                    {
        //                        line = matches.group(1) + '"' + matches.group(2).replace('"', '\'')
        //                                            + '"';
        //                    }
        //            }
        //            xml += line + "\n";
        //        }
        //        return xml;
        //}

    }
}
