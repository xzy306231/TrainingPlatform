using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoursewareUploadHelper.Entity.Course;
using CoursewareUploadHelper.Helper.File;

// ReSharper disable All

namespace CoursewareUploadHelper
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ReplaceProcessor
    {
        private static readonly ReplaceProcessor Instance = new ReplaceProcessor();

        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
        private ReplaceProcessor()
        {
            
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <returns></returns>
        public static ReplaceProcessor GetInstance() => Instance;

        private void GetRecycleFileList(DirectoryInfo info, List<FileInfo> fileInfoList)
        {
            if (info.GetDirectories().Length == 0)
            {
                _logger.Debug($"文件夹{info.FullName}下已经没有子文件夹，开始添加课件");
                fileInfoList.AddRange(info.GetFiles().Where(e => e.Extension.ToLower().Equals(".swf")).OrderBy(y=>y.Name, new FileComparer()).ToList());
            }
            else
            {
                _logger.Debug($"文件夹{info.FullName}还有子文件夹，继续递归");
                foreach (var directoryInfo in info.GetDirectories().OrderBy(x => x.Name, new FileComparer()))
                {
                    GetRecycleFileList(directoryInfo, fileInfoList);
                }
            }
        }

        /// <summary>
        /// 运行替换程序
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="coursePath"></param>
        public void Run(long courseId, string coursePath)
        {
            var fileDict = new Dictionary<string, string>();
            var baseDirectory = new DirectoryInfo(coursePath);
            var floders = baseDirectory.GetDirectories().OrderBy(x=>x.Name, new FileComparer()).ToArray();

            if (floders.Length == 0)
            {
                _logger.Debug($"文件夹{baseDirectory.FullName}下没有子文件夹，直接读取文件！");
                var swfs = baseDirectory.GetFiles().Where(e => e.Extension.ToLower().Equals(".swf"))
                    .OrderBy(y => y.Name, new FileComparer()).ToList();
                if (swfs.Count == 0)
                {
                    _logger.Error($"文件夹{baseDirectory.FullName}下没有子文件夹也没有swf课件，错误！");
                    return;
                }

                for (int j = 0; j < swfs.Count(); j++)
                {
                    fileDict.Add($"{Int2String(0)}{Int2String(j)}", swfs[j].FullName);
                }
            }
            else
            {

                for (int i = 0; i < floders.Length; i++)
                {
                    #region ::::: 文件夹下还有文件夹或者没有swf :::::
                    var swfs = new List<FileInfo>();
                    if (floders[i].GetDirectories().Length != 0)
                    {
                        _logger.Debug($"文件夹{floders[i].FullName}下还有子文件夹，递归查找文件！");
                        var tempList = new List<string>();
                        GetRecycleFileList(floders[i], swfs);
                    }
                    else
                    {
                        swfs = floders[i].GetFiles().Where(e => e.Extension.ToLower().Equals(".swf")).OrderBy(y => y.Name, new FileComparer()).ToList();
                        if (!swfs.Any())
                        {
                            _logger.Error($"文件夹{floders[i].FullName}下没有swf文件，替换失败！");
                            LogError($"文件夹{floders[i].FullName}下没有swf文件，替换失败！");
                            return;
                        }
                    }

                    #endregion

                    for (int j = 0; j < swfs.Count(); j++)
                    {
                        fileDict.Add($"{Int2String(i)}{Int2String(j)}", swfs[j].FullName);
                    }
                }
            }

            var resourceInfos = Program.FSqlCourse
                .Select<TCourseStruct, TStructResource, TCourseResource>()
                .Where((csRef, srRef, resource) => csRef.CourseId.Equals(courseId) && csRef.NodeType == "2")
                .LeftJoin((csRef, srRef, resource) => srRef.CourseStructId.Equals(csRef.Id))
                .LeftJoin((csRef, srRef, resource) => resource.Id.Equals(srRef.CourseResouceId))
                .ToList((csRef, srRef, resource) => new {resource.Id, resource.ResourceDesc, resource.ResourceUrl, resource.SrcId});

            foreach (var item in fileDict)
            {
                var temp = resourceInfos.Find(r => r.ResourceDesc.Equals(item.Key));
                if (temp == null)
                {
                    _logger.Error($"文件路径{coursePath}下文件编号为{item.Key}的课件在数据库找不到对应的数据");
                    LogError($"文件路径{coursePath}下文件编号为{item.Key}的课件在数据库找不到对应的数据");
                    continue;
                }

                var courseware = Program.FSqlCourseware.Select<Entity.Courseware.TCourseResource>(temp.SrcId).ToOne();
                if (courseware == null)
                {
                    _logger.Error($"课件资源库中找不到id为{temp.SrcId}的数据");
                    LogError($"课件资源库中找不到id为{temp.SrcId}的数据");
                    continue;
                }

                var updateCoursewareResult = Program.FSqlCourseware
                    .Update<Entity.Courseware.TCourseResource>(courseware.Id)
                    .Set(c => c.TransformUrl, Program.GetUrlWithoutDrive(item.Value))
                    .ExecuteAffrows();

                if (updateCoursewareResult == 0)
                {
                    _logger.Error($"更新课件资源库id为{temp.SrcId}的课件失败！");
                    LogError($"更新课件资源库id为{temp.SrcId}的课件失败！");
                    continue;
                }

                var sourceFile = Program.FSqlCourse.Update<TCourseResource>(temp.Id)
                    .Set(cr => cr.ResourceUrl, Program.GetUrlWithoutDrive(item.Value))
                    .ExecuteAffrows();

                if (sourceFile == 0)
                {
                    _logger.Error($"更新课程课件库id为{temp.SrcId}的课件失败！");
                    LogError($"更新课程课件库id为{temp.SrcId}的课件失败！");
                    continue;
                }

                _logger.Debug("更新成功！");
            }
        }

        /// <summary>
        /// 更新字幕
        /// </summary>
        public void ReplaceSubtitle()
        {
            var coursewares = Program.FSqlCourse.Select<TCourseResource>().ToList(a => new {a.Id, a.SrcId,a.ResourceUrl});
            foreach (var courseware in coursewares)
            {
                try
                {
                    var courseResource =
                        Program.FSqlCourseware.Select<Entity.Courseware.TCourseResource>(courseware.SrcId).ToOne();
                    //课件资源路径更新
                    var sourceResult = Program.FSqlCourseware
                        .Update<Entity.Courseware.TCourseResource>(courseResource.Id)
                        .Set(c => c.TransformUrl, Program.GetUrlWithoutDrive(courseResource.TransformUrl))
                        .ExecuteAffrows();
                    _logger.Debug($"课件资源库id为{courseResource.Id}的资源路径转换成功");
                    //课程资源路径和字幕更新
                    var tempResult = Program.FSqlCourse
                        .Update<TCourseResource>(courseware.Id)
                        .Set(c => c.ResourceDesc, courseResource.ThumbnailPath)
                        .Set(c=>c.ResourceUrl,Program.GetUrlWithoutDrive(courseware.ResourceUrl))
                        .ExecuteAffrows();
                    _logger.Debug($"课程资源库id为{courseware.Id}的字幕更新成功");
                    if (tempResult == 0)
                    {
                        LogError($"课程资源库id为{courseware.Id}的字幕与路径更新失败");
                        _logger.Debug($"课程资源库id为{courseware.Id}的字幕与路径更新失败");
                    }
                }
                catch (Exception e)
                {
                    LogError(e.Message);
                    _logger.Error(e.Message);
                }
            }
        }

        /// <summary>
        /// 创建字幕文件
        /// </summary>
        public async void CreateSubtitleFile()
        {
            var result = await Program.FSqlCourseware.Select<Entity.Courseware.TCourseResource>()
                .ToListAsync(a => new {a.TransformUrl, a.ThumbnailPath});
            var pathPrefix = @"C:/data";
            foreach (var item in result)
            {
                var breakTemp = pathPrefix + item.TransformUrl.Replace(".swf", ".xml");
                if (File.Exists(breakTemp)) break;

                var path = pathPrefix + item.TransformUrl.Replace(".swf", ".txt");
                if (File.Exists(path))
                {
                    _logger.Debug($"文件{path}已存在,跳过");
                    continue;
                }

                try
                {
                    await File.WriteAllTextAsync(path, item.ThumbnailPath);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
                _logger.Debug($"文件{path}创建成功,字幕导入成功");
            }
        }

        private string Int2String(int index)
        {
            var result = (index + 1).ToString();
            if (result.Length < 2)
                result = $"0{result}";
            return result;
        }
    }
}
