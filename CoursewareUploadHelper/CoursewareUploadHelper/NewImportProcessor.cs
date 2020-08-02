using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CoursewareUploadHelper.Entity.Course;
using CoursewareUploadHelper.Helper;
using CoursewareUploadHelper.Helper.File;
using Tag = CoursewareUploadHelper.Entity.Knowledge.TKnowledgeTag;

namespace CoursewareUploadHelper
{
    public sealed class NewImportProcessor
    {
        private static readonly NewImportProcessor Instance = new NewImportProcessor();

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
        private NewImportProcessor()
        {
            _basePath = Program.Config["Directory:Base"];
            _sw = new Stopwatch();
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <returns></returns>
        public static NewImportProcessor GetInstance() => Instance;

        public void Run()
        {
            #region ::::: 知识点根节点 :::::

            long rootId;
            var rootName = Program.Config["TagName:Root"];
            var tagExist = Program.FSqlTag.Select<Tag>()
                .Where(t => t.ParentId == null || t.ParentId == 0)
                .Where(t => t.Tag.Equals(rootName)).ToOne();
            if (tagExist != null)
            {
                _logger.Debug($"知识点树根节点已经存在，id为{tagExist.Id},名称为{tagExist.Tag}");
                rootId = tagExist.Id;
            }
            else
            {
                rootId = ImportProcessorData.InsertTag($"01{Program.Config["TagName:Root"]}", 0);//1级节点
                _logger.Debug($"新增知识点树根节点，id为{rootId},名称为{rootName}");
            }

            #endregion

            _logger.Debug($"开始解析与上传，请等待...");
            var firstLevelDirectory = new DirectoryInfo(_basePath);
            //先做一层校验，文件夹结构满足条件再继续
            int index = 1;
            if (!CheckDirectoryStruct(firstLevelDirectory, 5, ref index))
            {
                LogError("校验失败，文件夹层级结构不满足条件，请检查调整后再试");
                return;
            }

            _sw.Start();//打开计时器

            #region ::::: 循环2级目录，空/地勤 :::::
            var secLevelDirectories = firstLevelDirectory.GetDirectories().OrderBy(x => x.Name, new FileComparer());

            foreach (var secLevelDirectory in secLevelDirectories)
            {
                var secLevelId = ImportProcessorData.InsertTag(secLevelDirectory.Name, rootId);//2级节点
                var thirdLevelDirectories = secLevelDirectory.GetDirectories().OrderBy(x => x.Name, new FileComparer());

                #region ::::: 循环3级目录，专业 :::::
                foreach (var thirdLevelDirectory in thirdLevelDirectories)
                {
                    var thirdLevelId = ImportProcessorData.InsertTag(thirdLevelDirectory.Name, secLevelId);//3级节点
                    var fourthLevelDirectories = thirdLevelDirectory.GetDirectories().OrderBy(x => x.Name, new FileComparer());

                    #region ::::: 循环4级目录，课程 :::::
                    foreach (var fourthLevelDirectory in fourthLevelDirectories)
                    {
                        var courseName = fourthLevelDirectory.Name;
                        if (int.TryParse(courseName.Substring(0, 2), out _)) courseName = $"{courseName.Substring(2)}";

                        _logger.Debug($"获取课程[{courseName}],准备创建课程");
                        var fourthLevelId = ImportProcessorData.InsertTag(courseName, thirdLevelId);
                        var info = new CourseContent
                        {
                            CourseName = courseName, CourseFolderPath = fourthLevelDirectory.FullName,
                            TagNodeInfo = new CourseNodeInfo
                            {
                                FirstLevelId = rootId, SecondLevelId = secLevelId, ThirdLevelId = thirdLevelId,
                                FourthLevelId = fourthLevelId
                            },
                            CourseTagInfo = new List<long>()
                            {
                                ImportProcessorData.GetCourseTagId(rootId),
                                ImportProcessorData.GetCourseTagId(secLevelId),
                                ImportProcessorData.GetCourseTagId(thirdLevelId),
                                ImportProcessorData.GetCourseTagId(fourthLevelId)
                            },
                            CoursewareTagInfo = new List<long>
                            {
                                ImportProcessorData.GetCoursewareTag(rootId),
                                ImportProcessorData.GetCoursewareTag(secLevelId),
                                ImportProcessorData.GetCoursewareTag(thirdLevelId),
                                ImportProcessorData.GetCoursewareTag(fourthLevelId)
                            }
                        };
                        var coursewareNumb = 0;
                        //1.插课程
                        var courseId = ImportProcessorData.GetNewCourseId(info);
                        //2.课程-知识点-关联表
                        ImportProcessorData.CreateCourseTagRef(info, courseId);
                        //3.课程结构根节点
                        var courseStructRoot = ImportProcessorData.GetNewCourseStructId(new TCourseStruct
                        {
                            CourseId = courseId, CourseNodeName = info.CourseName,
                            CreateName = Program.Teacher.UserName, CreateTime = DateTime.Now,
                            DeleteFlag = 0, NodeSort = 0, NodeType = "1", ParentId = 0,
                            ResourceCount = fourthLevelDirectory.GetDirectories().Length
                        });

                        #region ::::: 循环五级目录 课程子节点 :::::
                        //4.处理课件信息
                        var fifthLevelDirectories = fourthLevelDirectory.GetDirectories().OrderBy(x => x.Name, new FileComparer()).ToList();

                        for (int i = 0; i < fifthLevelDirectories.Count; i++)
                        {
                            var title = fifthLevelDirectories[i].Name;
                            //子节点下的课件信息
                            var swfItems = fifthLevelDirectories[i].GetFiles()
                                .Where(e => e.Extension.ToLower().Equals(".swf"))
                                .OrderBy(x => x.Name, new FileComparer()).ToList();
                            //4.1新增课程结构
                            var subRoot = ImportProcessorData.GetNewCourseStructId(new TCourseStruct
                            {
                                CourseId = courseId, DeleteFlag = 0,CourseNodeName = title,
                                CreateName = Program.Teacher.UserName, CreateTime = DateTime.Now,
                                NodeSort = i + 1, NodeType = "1", ParentId = courseStructRoot,
                                ResourceCount = swfItems.Count
                            });
                            //4.2知识体系中加课件知识点
                            var courseTagIdInTree = ImportProcessorData.InsertTag(title, info.TagNodeInfo.FourthLevelId);
                            //4.3课件资源库知识点表新增知识点
                            info.CoursewareTagInfo.Add(ImportProcessorData.GetNewCourseTag(courseTagIdInTree));
                            coursewareNumb += swfItems.Count;

                            #region ::::: 循环六级目录 课件 :::::
                            //4.4插节点资源
                            for (int j = 0; j < swfItems.Count; j++)
                            {
                                //4.4.1课件资源、字幕
                                var subTitle = GetSubTitle(swfItems[j]);
                                var fileSize = swfItems[j].Length;
                                var resourceName = swfItems[j].Name;
                                var coursewareId = ImportProcessorData.GetNewCoursewareId(
                                    new Entity.Courseware.TCourseResource
                                    {
                                        CheckStatus = "3",
                                        CreatorId = Program.Teacher.OriginalId,
                                        CreatorName = Program.Teacher.UserName,
                                        FileSuffix = "swf",
                                        FileSize = fileSize,
                                        FileSizeDisplay = GetFileSizeDisplay(fileSize),
                                        ThumbnailPath = subTitle,
                                        TransfType = "0",
                                        TransformUrl = Program.GetUrlWithoutDrive(swfItems[j].FullName),
                                        ResourceLevel = "3",
                                        ResourceName = resourceName,
                                        ResourceType = "2",
                                        TCreate = DateTime.Now
                                    });
                                //4.4.2 关联知识点
                                ImportProcessorData.CreateCoursewareTagRefCourse(info, coursewareId);
                                ImportProcessorData.CreateCoursewareTagRef(coursewareId, info.CoursewareTagInfo.Last());

                                //4.4.3 课程课件、字幕
                                var resourceId = ImportProcessorData.GetNewCourseResource(new TCourseResource
                                {
                                    CreateBy = Program.Teacher.OriginalId,
                                    CreateTime = DateTime.Now,
                                    DeleteFlag = 0,
                                    ResourceConfidential = "3",
                                    ResourceDesc = subTitle,
                                    ResourceExtension = "swf",
                                    ResourceName = resourceName,
                                    ResourceType = "2",
                                    SrcId = coursewareId,
                                    ResourceUrl = Program.GetUrlWithoutDrive(swfItems[j].FullName)
                                });

                                //4.4.4 子节点
                                var subItemId = ImportProcessorData.GetNewCourseStructId(new TCourseStruct
                                {
                                    CourseId = courseId,
                                    CourseNodeName = resourceName,
                                    CreateName = Program.Teacher.UserName,
                                    CreateTime = DateTime.Now,
                                    DeleteFlag = 0,
                                    NodeSort = j + 1,
                                    NodeType = "2",
                                    ParentId = subRoot,
                                    ResourceCount = 0
                                });

                                //4.4.5 关联表
                                var refId = ImportProcessorData.GetNewStructResourceRef(new TStructResource
                                {
                                    CourseResouceId = resourceId, CourseStructId = subItemId,
                                    DeleteFlag = 0, TCreate = DateTime.Now
                                });

                            }

                            #endregion
                        }

                        #endregion

                        var result = Program.FSqlCourse.Update<TCourseStruct>(courseStructRoot)
                            .Set(c => c.ResourceCount, coursewareNumb).ExecuteAffrows();
                        _logger.Debug($"更新当前课程总课件数{result}");
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

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

        private bool CheckDirectoryStruct(DirectoryInfo info, int goal, ref int current)
        {
            if (current == goal)
            {
                current = 1;
                var files = info.GetFiles().Where(e => e.Extension.ToLower().Equals(".swf"));
                var fileInfos = files.ToList();
                if (fileInfos.Any())
                {
                    if (fileInfos.Count == info.GetFiles().Count(e => e.Extension.ToLower().Equals(".txt"))) return true;
                    LogError($"文件夹{info.FullName}下swf文件数量和txt文件数量不相等！请检查");
                    return false;
                }
                LogError($"文件夹{info.FullName}下没有swf文件！当前层级为{current}");

                return false;
            }

            var subDirectories = info.GetDirectories();
            if (subDirectories.Length == 0)
            {
                LogError($"文件夹{info.FullName}下没有子文件夹！当前层级为{current}");
                return false;
            }
            foreach (var subDirectory in subDirectories)
            {
                current++;
                if (!CheckDirectoryStruct(subDirectory, goal, ref current)) return false;
            }

            return true;
        }


        private string GetSubTitle(FileSystemInfo file)
        {
            var path = file.FullName.Replace("swf", "txt");
            try
            {
                using (var str = new StreamReader(path))
                {
                    return str.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return "";
            }
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
    }
}
