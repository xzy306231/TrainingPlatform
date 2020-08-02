using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CoursewareUploadHelper.Entity.Courseware;
using CoursewareUploadHelper.Helper;
using CoursewareUploadHelper.Helper.File;


namespace CoursewareUploadHelper.ImportProcessor
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed partial class ImportProcessor
    {
        /// <summary>
        /// 获取知识点展示信息
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        private string GetTagDisplay(List<TResourceTagRef> tags)
        {
            var result = string.Empty;
            foreach (var tag in tags)
            {
                result += $"{tag.t_knowledge_tag.Tag}|";
            }
            return result.TrimEnd('|');
        }

        #region ::::: 递归找文件和文件夹 :::::

        /// <summary>
        /// 查找课程配置xml
        /// </summary>
        /// <param name="direInfo">课程文件夹</param>
        /// <param name="course"></param>
        /// <param name="nodeInfo"></param>
        /// <returns></returns>
        // ReSharper disable once IdentifierTypo
        private bool GetCbtstructure(DirectoryInfo direInfo, out CourseContent course, CourseNodeInfo nodeInfo)
        {
            course = null;
            var courseName = $"{direInfo.Name}";//课程名
            if (!direInfo.GetFiles().Any(file => file.Name.ToLower().Equals("cbtstructure.xml"))) return false;

            #region ::::: 课程名前面是编号就去掉编号 :::::

            if (int.TryParse(direInfo.Name.Substring(0, 2), out _))
                courseName = $"{direInfo.Name.Substring(2)}";

            #endregion

            nodeInfo.FourthLevelId = ImportProcessorData.InsertTag(courseName, nodeInfo.ThirdLevelId);

            course = new CourseContent
            {
                CourseName = courseName,
                CourseFolderPath = direInfo.FullName,
                XmlFullPath = direInfo.FullName + "/" + "cbtstructure.xml",
                TagNodeInfo = nodeInfo,
                CourseTagInfo = new List<long>
                {
                    ImportProcessorData.GetCourseTagId(nodeInfo.FirstLevelId),
                    ImportProcessorData.GetCourseTagId(nodeInfo.SecondLevelId),
                    ImportProcessorData.GetCourseTagId(nodeInfo.ThirdLevelId),
                    ImportProcessorData.GetCourseTagId(nodeInfo.FourthLevelId)
                },
                CoursewareTagInfo = new List<long>
                {
                    ImportProcessorData.GetCoursewareTag(nodeInfo.FirstLevelId),
                    ImportProcessorData.GetCoursewareTag(nodeInfo.SecondLevelId),
                    ImportProcessorData.GetCoursewareTag(nodeInfo.ThirdLevelId),
                    ImportProcessorData.GetCoursewareTag(nodeInfo.FourthLevelId)
                }
            };
            return true;
        }

        /// <summary>
        /// 递归直到找到课程文件夹
        /// </summary>
        /// <param name="direInfo"></param>
        /// <param name="configs"></param>
        /// <param name="nodeInfo"></param>
        private void RecycleCatchFile(DirectoryInfo direInfo, List<CourseContent> configs, CourseNodeInfo nodeInfo)
        {
            foreach (var directory in direInfo.GetDirectories().OrderBy(x => x.Name, new FileComparer()))
            {
                if (GetCbtstructure(directory, out var course, nodeInfo))
                {
                    configs.Add(course);
                    continue;
                }
                RecycleCatchFile(directory, configs, nodeInfo);
            }
        }

        /// <summary>
        /// 获取课程信息
        /// 当前应该循环到专业文件夹
        /// </summary>
        /// <param name="direInfo">专业文件夹</param>
        /// <param name="nodeInfo"></param>
        /// <returns></returns>
        private List<CourseContent> GetCourseContents(DirectoryInfo direInfo, CourseNodeInfo nodeInfo)
        {
            var thirdLevelInfos = direInfo.GetDirectories().OrderBy(x => x.Name, new FileComparer());
            var allCourseXml = new List<CourseContent>();
            foreach (var info in thirdLevelInfos)
            {
                var thirdId = ImportProcessorData.InsertTag(info.Name, nodeInfo.SecondLevelId);//3级目录，专业id
                var configs = new List<CourseContent>();
                RecycleCatchFile(info, configs,
                    new CourseNodeInfo
                    {
                        FirstLevelId = nodeInfo.FirstLevelId,
                        SecondLevelId = nodeInfo.SecondLevelId,
                        ThirdLevelId = thirdId
                    });
                allCourseXml.AddRange(configs);
            }

            return allCourseXml;
        }

        #endregion

        /// <summary>
        /// 获取xml对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private organizations GetXmlObject(string path)
        {
            organizations result = null;
            var serializer = new XmlSerializer(typeof(organizations));

            try
            {
                using (var reader = new StreamReader(path))
                {
                    result = (organizations)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message ?? $"读取xml路径{path}出错");
            }

            return result;
        }

        /// <summary>
        /// 获取课件的编码
        /// 例01p01
        /// </summary>
        /// <param name="code"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetCoursewareCode(string code, string filePath)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.Error($"文件夹{filePath}下找不到编号");
                return "";
            }

            if (code.Length < 5)
            {
                _logger.Error($"文件夹{filePath}下编号{code}格式不匹配");
                return "";
            }
            var temp = code.Substring(code.Length - 5).Split("p");
            return temp.Aggregate(string.Empty, (current, s) => current + s);
        }

        /// <summary>
        /// 文件大小展示
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string GetFileSizeDisplay(long size)
        {
            var k = size / 1024;
            if (k <= 1024) return $"{Convert.ToInt32(k)}KB";
            var m = k / 1024;
            if (m < 1024) return $"{Convert.ToInt32(m)}MB";
            var g = k / 1024;
            return $"{g:0.0}GB";
        }

        /// <summary>
        /// 文件真实大小
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private long GetFileSize(string path)
        {
            try
            {
                FileInfo objFi = new FileInfo(path);
                return objFi.Length;
            }
            catch (Exception e)
            {
                LogError("获取文件大小失败！");
                _logger.Error($"获取{path}的文件大小失败，失败信息：{e.Message ?? "无"}");
                return 0;
            }
        }

        /// <summary>
        /// 序列号
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private int GetIndex(string title)
        {
            if (string.IsNullOrEmpty(title)) return 0;
            var indexStr = title.Substring(title.Length - 2);
            return int.TryParse(indexStr, out var index) ? index : 0;
        }

        /// <summary>
        /// 获取字幕
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetSubTitle(string path)
        {
            var subTitle = string.Empty;
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(path);
                var nodeList = xmlDoc.GetElementsByTagName("audio_text");
                foreach (var node in nodeList)
                {
                    subTitle += ((XmlElement)node).FirstChild?.Value;
                }
            }
            catch (Exception e)
            {
                LogError(e.Message ?? "xml加载失败");
                _logger.Error($"路径{path}的xml加载失败，失败信息{e.Message ?? "无"}");
            }

            return subTitle;
        }
    }
}
