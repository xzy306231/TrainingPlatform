using System;
using System.Xml;
using adlcp_rootv1p2;
using adlcp_rootv1p2.imscp;
using adlcp_v1p3;
using adlcp_v1p3.imscp_v1p1;
using Microsoft.Extensions.Logging;

namespace Courseware.Api.Common
{
    /// <summary>
    /// Get some information about the course from the imsmanifest`.xml file
    /// 从imsmanifest ' .xml文件中获得有关该课程的一些信息
    /// </summary>
    public class SCORMUploadHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SCORM_Version { get; set; } // will either be 1.2 or one of the synonyms for SCORM 2004

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; } // this is the creator's version

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PathToManifest { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PathToPackageFolder { get; set; }

        private ILogger Logger { get; }
        public SCORMUploadHelper(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToManifest"></param>
        public void ParseManifest(string pathToManifest)
        {
            string xmlPath = pathToManifest;
            string xmlDirectory = System.IO.Path.GetDirectoryName(pathToManifest);
            if (!System.IO.File.Exists(xmlPath))
            {
                Logger.LogWarning("Manifest file not found!");
            }
            var doc = new adlcp_rootv1p2Doc();
            var root = new adlcp_rootv1p2.imscp.manifestType(doc.Load(xmlPath));
            this.Identifier = root.identifier.Value;
            // SCORM version should be in the metadata. It should have the following values:
            //   Schema: ADL SCORM
            //   SchemaVersion: 1.2
            // for SCORM2004 it will say "1.3" or "CAM 1.3"
            // unfortunately I have seen a lot of manifests without this which forces you to guess

            // SCORM版本应该在元数据中。
            // 它应该有以下值:Schema: ADL SCORM schem: 1.2 .
            // 对于SCORM2004，它将显示“1.3”或“CAM 1.3”，
            // 不幸的是，我已经看到了很多没有这个的清单，这迫使您猜测
            if (root.Hasmetadata())
            {
                adlcp_metadataType meta = root.Getmetadata();

                if (meta.Hasschemaversion())
                {
                    // this is the way we SHOULD get the version but some manifests don't have this
                    // 这是我们应该得到版本的方式，但有些清单没有这个
                    SCORM_Version = meta.schemaversion.Value;
                }
            }
            if (string.IsNullOrEmpty(SCORM_Version))
            {
                // backwards way to get the version
                // 返回得到版本的方法
                SCORM_Version = GetSCORMVersion(root);
            }

            if (SCORM_Version == "1.2")
            {
                Logger.LogInformation("Module is SCORM 1.2");
                versionType versionType;

                // this is the manifest's creator's version of this manifest, not SCORM version
                // 这是这个清单的创造者版本，而不是SCORM版本
                versionType = root.Hasversion() ? root.Getversion() : new versionType("1.0");
                Version = versionType.Value;

                // this will become the course title unless there is one in Organizations (should be!)
                // 这将成为课程名称，除非在组织中有一个(应该是!)
                Title = Identifier;
                if (root.Hasmetadata())
                {
                    adlcp_metadataType md = root.Getmetadata();
                    if (md.HasLOM())
                    {
                        if (md.LOM.Hasgeneral())
                        {
                            if (md.LOM.general.Hasdescription())
                            {
                                Description = md.LOM.general.description.ToString();
                            }
                        }
                    }
                }
                Href = FindDefaultWebPage(root);

            } // end if version == 1.2
            else if (SCORM_Version == "1.3" || SCORM_Version == "CAM 1.3" || SCORM_Version.IndexOf("2004", StringComparison.Ordinal) >= 0)
            {
                Logger.LogInformation("Module is SCORM2004");
                var doc2 = new adlcp_v1p3Doc();
                var root2 = new manifestTypeExtended(doc2.Load(xmlPath));
                Identifier = root.Getidentifier().Value;
                Title = Identifier;
                Version = root.Getversion().Value;
                // Now we start looking for the default web page. Organizations => organization => item
                // get the identifierref for the first item
                // then find that identifier in resources => resource. That resource.href is the default launching page for the sco

                // get all organizations for this manifest. "Organizations" is a container for "Organization" objects
                // 现在我们开始寻找默认的web页面。
                // 获取第一项的identifierref，然后在resources =>资源中找到该标识符。
                // 该资源。href是sco get所有组织对此清单的默认启动页面。“组织”是“组织”对象的容器
                Href = FindDefaultWebPage(root);
            }
            else
            {
                Logger.LogInformation("Manifest version is " + SCORM_Version + ". Must be 1.2 or 1.3");
                return;
            }
            Logger.LogInformation("Parse of manifest completed successfully");
        }

        private string FindDefaultWebPage(adlcp_rootv1p2.imscp.manifestType root)
        {
            if (root.Hasorganizations())
            {
                adlcp_rootv1p2.imscp.organizationsType orgs = root.Getorganizations();
                if (orgs.Hasorganization()) // its possible but fatal to have a blank "organizations" node 拥有一个空白的“组织”节点是可能的，但也是致命的
                {
                    if (orgs.Hasdefault2())
                    {
                        var orgDefault = orgs.default2.ToString();
                        var org = FindDefaultOrg(orgDefault, orgs);
                        string identifier1 = "";
                        if (org.Hasidentifier())
                            identifier1 = org.Getidentifier().ToString();
                        if (org.Hastitle())
                        {
                            Title = org.Gettitle().ToString();
                        }
                        if (org.Hasitem())
                        {
                            int i = org.GetitemCount();
                            if (i > 0)
                            {
                                adlcp_itemType item = org.GetitemAt(0);
                                identifierrefType2 itemIdentifier = item.identifierref;
                                //
                                // find the resource for this item
                                //
                                adlcp_rootv1p2.imscp.resourceType resource = FindDefaultResource(itemIdentifier, root.resources);
                                if (resource.Hashref())
                                {
                                    Href = resource.href.ToString();
                                    return Href;
                                }
                            }
                        }
                    }
                }
            } // end of "organizations" object
            return "";

        }

        private adlcp_rootv1p2.imscp.resourceType FindDefaultResource(identifierrefType2 itemIdentifier, adlcp_rootv1p2.imscp.resourcesType resources)
        {
            var rsEmpty = new adlcp_rootv1p2.imscp.resourceType();
            for (int ii = 0; ii < resources.resourceCount; ii++)
            {
                adlcp_rootv1p2.imscp.resourceType rs = resources.GetresourceAt(ii);

                string identifier = "";
                if (rs.Hasidentifier())
                    identifier = rs.identifier.ToString();
                if (identifier == itemIdentifier.ToString())
                {
                    return rs;
                }
            }
            return rsEmpty;
        }

        /// <summary>
        /// get the SCORM version by examining the namespace declaration
        /// People have misused the "Version" attribute so you can't depend on it.
        /// Version SHOULD be in the metadata but some people don't even include that.
        /// 通过检查名称空间声明，获得SCORM版本，因为人们误用了“version”属性，所以不能依赖它。
        /// 版本应该在元数据中，但是有些人甚至不包括它。
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private string GetSCORMVersion(adlcp_rootv1p2.imscp.manifestType root)
        {
            string version = "unknown";
            XmlNode node = root.getDOMNode();
            foreach (XmlNode attr in node.Attributes)
            {
                if (attr.Name.ToLower() == "xmlns:adlcp")
                {
                    string ns = attr.Value;
                    switch (ns.ToLower())
                    {
                        case "http://www.adlnet.org/xsd/adlcp_rootv1p1":
                            version = "1.1";
                            break;
                        case "http://www.adlnet.org/xsd/adlcp_rootv1p2":
                            version = "1.2";
                            break;
                        case "http://www.adlnet.org/xsd/adlcp_v1p3":
                            version = "1.3";
                            break;
                    }
                    return version;
                }

            }
            return version;
        }

        private adlcp_rootv1p2.imscp.organizationType FindDefaultOrg(string orgDefault, adlcp_rootv1p2.imscp.organizationsType orgs)
        {
            var orgEmpty = new adlcp_rootv1p2.imscp.organizationType();
            foreach (var org in orgs.Myorganizations)
            {
                if (org.identifier.ToString() == orgDefault)
                {
                    return org;
                }
            }
            return orgEmpty;
        }
    }
}
