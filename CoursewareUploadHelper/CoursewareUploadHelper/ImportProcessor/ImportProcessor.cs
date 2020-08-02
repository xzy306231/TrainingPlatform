using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CoursewareUploadHelper.Entity.Course;
using CoursewareUploadHelper.Helper;
using CoursewareUploadHelper.Helper.File;
using Tag = CoursewareUploadHelper.Entity.Knowledge.TKnowledgeTag;


namespace CoursewareUploadHelper.ImportProcessor
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ImportProcessor
    {
        private static readonly ImportProcessor Instance = new ImportProcessor();

        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _basePath;//程序文件夹外层路径

        private readonly Stopwatch _sw;

        /// <summary>
        /// 日志信息
        /// </summary>
        public Action<string> LogDebug { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public Action<string> LogError { get; set; }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private ImportProcessor()
        {
            _basePath = Program.Config["Directory:NewBase"];
            _sw = new Stopwatch();
        }

        /// <summary>
        /// 单例对象
        /// </summary>
        /// <returns></returns>
        public static ImportProcessor GetInstance() => Instance;

        /// <summary>
        /// 处理流程启动
        /// </summary>
        public void Run()
        {
            #region ::::: 知识点根节点 :::::

            long rootId;
            var rootName = Program.Config["TagName:NewRoot"];
            var tagExist = Program.FSqlTag.Select<Tag>()
                .Where(t=>t.ParentId==null || t.ParentId == 0)
                .Where(t=>t.Tag.Equals(rootName)).ToOne();
            if (tagExist != null)
            {
                _logger.Debug($"知识点树根节点已经存在，id为{tagExist.Id},名称为{tagExist.Tag}");
                rootId = tagExist.Id;
            }
            else
            {
                rootId = ImportProcessorData.InsertTag($"01{Program.Config["TagName:Root"]}", 0);
                _logger.Debug($"新增知识点树根节点，id为{rootId},名称为{rootName}");
            }

            #endregion

            _logger.Debug($"开始解析与上传，请等待...");
            var baseDirectory = new DirectoryInfo(_basePath);
            var directories = baseDirectory.GetDirectories().OrderBy(x=>x.Name, new FileComparer());

            _sw.Start();//打开计时器

            foreach (var directory in directories)
            {
                var fileNames = Program.Config["DirectoryName:name"].Split(',');
                if (!fileNames.Any(file => file.Equals(directory.Name))) continue;

                var courseType = directory.Name;//空勤/地勤。。。
                _logger.Debug($"找到[{courseType}]文件夹");

                //1.知识体系添加2级目录
                var secLevelId = ImportProcessorData.InsertTag(courseType, rootId);

                var courseNodeInfo = new CourseNodeInfo { FirstLevelId = rootId, SecondLevelId = secLevelId };

                //2.获取当前2级目录下所有3级目录(专业)和4级目录(课程)信息
                var allCourseXml = GetCourseContents(directory, courseNodeInfo);

                //3.处理课程
                foreach (var info in allCourseXml)
                {
                    _logger.Debug($"找到[{info.CourseName}]");
                    //读xml
                    var xmlObject = GetXmlObject(info.XmlFullPath);
                    if (xmlObject == null)
                    {
                        LogError($"xml文件解析失败，跳过当前课程导入");
                        _logger.Error($"xml文件解析失败，跳过当前课程导入");
                        continue;
                    }

                    _logger.Debug($"获取课程[{info.CourseName}]的xml配置文件，准备创建课程");

                    var coursewareNumb = 0;

                    //3-1.插课程
                    var courseId = ImportProcessorData.GetNewCourseId(info);

                    //3-2.课程-知识点-关联表
                    ImportProcessorData.CreateCourseTagRef(info, courseId);

                    //3-3.课程结构根节点
                    var courseStructRoot = ImportProcessorData.GetNewCourseStructId(new TCourseStruct
                        {
                            CourseId = courseId, CourseNodeName = info.CourseName,
                            CreateName = Program.Teacher.UserName, CreateTime = DateTime.Now,
                            DeleteFlag = 0, NodeSort = 0, NodeType = "1", ParentId = 0,
                            ResourceCount = xmlObject.organization.item.Length - 1,
                        });

                    //3-4.处理课件信息
                    foreach (var item in xmlObject.organization.item)
                    {
                        //3-4-0.子级目录
                        if (item.title.Equals("menu")) continue;

                        //3-4-1.新增课程结构
                        var subRoot = ImportProcessorData.GetNewCourseStructId(new TCourseStruct
                        {
                            CourseId = courseId, CourseNodeName = $"{GetIndex(item.id)}{item.title}",
                            CreateName = Program.Teacher.UserName, CreateTime = DateTime.Now,
                            DeleteFlag = 0, NodeSort = GetIndex(item.id), NodeType = "1",
                            ParentId = courseStructRoot, ResourceCount = item.item.Length
                        });

                        //3-4-2.知识体系中加课件知识点
                        var courseTagIdInTree = ImportProcessorData.InsertTag(item.title, info.TagNodeInfo.FourthLevelId);

                        //3-4-3.课件资源库知识点表新增知识点
                        //todo:
                        info.CoursewareTagInfo.Add(ImportProcessorData.GetNewCourseTag(courseTagIdInTree));

                        coursewareNumb += item.item.Length;

                        //3-4-4.插节点资源
                        foreach (var subItem in item.item)
                        {
                            var sourceware = $"{info.CourseFolderPath}\\cn_content\\{subItem.location.Split('.')[0]}";//.swf为课件，.xml为字幕
                            _logger.Debug($"课件资源所在文件夹为[{sourceware}]");
                            //3-4-4-1.课件资源，字幕
                            var subTitle = GetSubTitle($"{sourceware}.xml");
                            var fileSize = GetFileSize($"{sourceware}.swf");
                            var courcewareId = ImportProcessorData.GetNewCoursewareId(new Entity.Courseware.TCourseResource
                            {
                                CheckStatus = "3", CreatorId = Program.Teacher.OriginalId,
                                CreatorName = Program.Teacher.UserName,
                                FileSize = fileSize, FileSizeDisplay = GetFileSizeDisplay(fileSize),
                                FileSuffix = "swf", ThumbnailPath = subTitle, TransfType = "0",
                                TransformUrl = Program.GetUrlWithoutDrive($"{sourceware}.swf"),
                                ResourceLevel = "3", ResourceName = subItem.title, ResourceType = "2",
                                TCreate = DateTime.Now
                            });
                            //3-4-4-2.关联知识点
                            ImportProcessorData.CreateCoursewareTagRefCourse(info, courcewareId);
                            ImportProcessorData.CreateCoursewareTagRef(courcewareId, info.CoursewareTagInfo.Last());

                            //3-4-4-3.课程课件，字幕
                            var resourceId = ImportProcessorData.GetNewCourseResource(new TCourseResource
                            {
                                CreateBy = Program.Teacher.OriginalId, CreateTime = DateTime.Now,
                                DeleteFlag = 0, ResourceConfidential = "3",
                                //ResourceDesc = subTitle,todo:这边课件替换后需要把字幕替换回来
                                ResourceDesc = GetCoursewareCode(subItem.id, sourceware),
                                ResourceExtension = "swf", ResourceName = subItem.title,
                                ResourceType = "2", SrcId = courcewareId,
                                ResourceUrl = Program.GetUrlWithoutDrive($"{sourceware}.swf")
                            });

                            //3-4-4-4.子节点
                            var subItemId = ImportProcessorData.GetNewCourseStructId(new TCourseStruct
                            {
                                CourseId = courseId, CourseNodeName = subItem.title,
                                CreateName = Program.Teacher.UserName, CreateTime = DateTime.Now,
                                DeleteFlag = 0, NodeSort = GetIndex(subItem.id),
                                NodeType = "2", ParentId = subRoot, ResourceCount = 0
                            });

                            //3-4-4-5.关联表
                            var refId = ImportProcessorData.GetNewStructResourceRef(new TStructResource
                            {
                                CourseResouceId = resourceId, CourseStructId = subItemId,
                                DeleteFlag = 0, TCreate = DateTime.Now
                            });
                        }
                    }

                    //更新课程的课件数量
                    var result = Program.FSqlCourse.Update<TCourseStruct>(courseStructRoot)
                        .Set(c => c.ResourceCount, coursewareNumb).ExecuteAffrows();
                    _logger.Debug($"更新当前课程总课件数{result}");

                }
            }

            _sw.Stop();//计时器关闭
            #region ::::: 日志 :::::

            _logger.Info("所有课件和课程已经导入完成，共导入" +
                         $"{ImportProcessorData.InsertCoursewareCount}个课件资源，" +
                         $"{ImportProcessorData.InsertCourseCount}个课程" +
                         $"{ImportProcessorData.InsertCourseStructCount}个课程结构" +
                         $"{ImportProcessorData.InsertTCourseResourceCount}个课程课件" +
                         $"{ImportProcessorData.InsertTStructResourceCount}条关联数据" +
                         $"总插入条目数：{ImportProcessorData.InsertDataCount}" +
                         $"总消耗时间 {_sw.ElapsedMilliseconds / 1000}秒");

            #endregion
        }

        public void UpdateTag()
        {
            var allTags = Program.FSqlTag.Select<Tag>().ToList();
            _logger.Debug($"知识点总数共{allTags.Count}条");
            var dictTags = new Dictionary<long, string>();
            foreach (var tag in allTags)
            {
                dictTags.Add(tag.Id, tag.Tag);
            }

            #region ::::: 课程知识点 :::::

            _logger.Debug($"开始更新课程知识点...");
            var courseTags = Program.FSqlCourse.Select<TKnowledgeTag>().ToList();
            _logger.Debug($"共有{courseTags.Count}条课程知识点");
            foreach (var courseTag in courseTags)
            {
                courseTag.Tag = dictTags[courseTag.SrcId];
            }
            _logger.Debug($"课程知识点内容替换完成，进行数据更新..");
            Program.FSqlCourse.Update<TKnowledgeTag>().SetSource(courseTags).ExecuteAffrows();
            _logger.Debug($"更新完成");

            #endregion

            #region ::::: 课件知识点 :::::

            _logger.Debug($"开始更新课件知识点");
            var coursewareTags = Program.FSqlCourseware.Select<Entity.Courseware.TKnowledgeTag>().ToList();
            _logger.Debug($"共有{coursewareTags.Count}条课件知识点");
            foreach (var coursewareTag in coursewareTags)
            {
                coursewareTag.Tag = dictTags[coursewareTag.OriginalId];
            }
            _logger.Debug($"课件知识点内容替换完成，进行数据更新..");
            Program.FSqlCourseware.Update<Entity.Courseware.TKnowledgeTag>().SetSource(coursewareTags).ExecuteAffrows();
            _logger.Debug($"更新完成");

            #endregion

            #region ::::: 课件知识点展示 :::::

            _logger.Debug($"开始更新课件知识点展示");
            var coursewareTagRefs = Program.FSqlCourseware.Select<Entity.Courseware.TCourseResource>()
                .IncludeMany(c => c.t_resource_tag_refs, then => then.Include(r => r.t_knowledge_tag))
                .ToList();
            _logger.Debug($"共有{coursewareTagRefs.Count}个课件需要更新知识点展示内容");
            foreach (var coursewareTagRef in coursewareTagRefs)
            {
                coursewareTagRef.ResourceTagsDisplay = GetTagDisplay(coursewareTagRef.t_resource_tag_refs);
            }
            _logger.Debug($"课件知识点展示更新完成，进行数据更新..");
            var resourceResult = Program.FSqlCourseware.Update<Entity.Courseware.TCourseResource>()
                .SetSource(coursewareTagRefs).UpdateColumns(a => a.ResourceTagsDisplay).ExecuteAffrows();
            _logger.Debug($"更新完成,共更新{resourceResult}条数据");

            #endregion
        }


    }
}
