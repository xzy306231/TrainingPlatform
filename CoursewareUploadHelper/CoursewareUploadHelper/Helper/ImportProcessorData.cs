using System;
using CoursewareUploadHelper.Entity;
using CoursewareUploadHelper.Entity.Course;
using Tag = CoursewareUploadHelper.Entity.Knowledge.TKnowledgeTag;
using TagCourse = CoursewareUploadHelper.Entity.Course.TKnowledgeTag;
using TagCourseware = CoursewareUploadHelper.Entity.Courseware.TKnowledgeTag;


namespace CoursewareUploadHelper.Helper
{
    public static class ImportProcessorData
    {
        public static int InsertCoursewareCount;//课件插入总条数
        public static int InsertCourseStructCount; //课程结构
        public static int InsertTCourseResourceCount;//课程课件
        public static int InsertCourseCount;//课程插入总条数
        public static int InsertTStructResourceCount;//关联表

        public static int InsertDataCount;


        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 知识体系树插表
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static long InsertTag(string tag, long? parentId)
        {
            var tagName = tag;
            if (int.TryParse(tag.Substring(0, 2), out _))
                tagName = $"{tag.Substring(2)}";//文件名前面有编号，则把编号去掉
            if (tagName.Trim().EndsWith('}'))
                tagName = tagName.Split('{')[0];//存在文件名以{完}结尾
            var result = Program.FSqlTag.Insert<Tag>().AppendData(new Tag { Tag = tagName.Trim(), ParentId = parentId, CreateBy = Program.Teacher.OriginalId, CreateTime = DateTime.Now,DeleteFlag = 0}).ExecuteIdentity();
            Logger.Debug($"知识体系树插表成功,新增id[{result}],标签名[{tag}],父节点为[{parentId}]");
            InsertDataCount++;
            return result;
        }

        /// <summary>
        /// 获取课程知识点
        /// </summary>
        /// <param name="originalId"></param>
        /// <returns></returns>
        public static long GetCourseTagId(long originalId)
        {
            var result = Program.FSqlCourse.Select<TagCourse>().Where(t => t.SrcId == originalId).ToOne();
            if (result != null) return result.Id;
            var resultId = Program.FSqlCourse.Insert<TagCourse>().AppendData(new TagCourse { SrcId = originalId, CreateTime = DateTime.Now, DeleteFlag = 0}).ExecuteIdentity();
            Logger.Debug($"课程知识点表插入成功,新增id[{resultId}],原始id[{originalId}]");
            InsertDataCount++;
            return resultId;
        }

        /// <summary>
        /// 获取课件知识点
        /// </summary>
        /// <param name="originalId"></param>
        /// <returns></returns>
        public static long GetCoursewareTag(long originalId)
        {
            var result = Program.FSqlCourseware.Select<TagCourseware>().Where(t => t.OriginalId == originalId).ToOne();
            if (result != null) return result.Id;
            var resultId = Program.FSqlCourseware.Insert<TagCourseware>().AppendData(new TagCourseware { OriginalId = originalId, TCreate = DateTime.Now}).ExecuteIdentity();
            Logger.Debug($"课件知识点表插入成功,新增id[{resultId}],原始id[{originalId}]");
            InsertDataCount++;
            return resultId;
        }

        /// <summary>
        /// 新建课程
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static long GetNewCourseId(CourseContent info)
        {
            var result = Program.FSqlCourse.Insert<TCourse>()
                .AppendData(new TCourse
                {
                    ApprovalStatus = "1",
                    CourseConfidential = "3",
                    CourseCount = 0,
                    LearningTime = 0,
                    CourseDesc = $"{info.CourseName}",
                    CourseName = $"{info.CourseName}",
                    CreateBy = Program.Teacher.OriginalId,
                    CreateTime = DateTime.Now,
                    DeleteFlag = 0,
                    PublishFlag = 0,
                    UserName = Program.Teacher.UserName,
                    UserNumber = Program.Teacher.UserNumber
                }).ExecuteIdentity();

            InsertDataCount++;//插入条目总数
            InsertCourseCount++;//课程数自增
            Logger.Debug($"课程[{info.CourseName}]插表成功");
            return result;
        }

        /// <summary>
        /// 课程-知识点-关联表
        /// </summary>
        /// <param name="info"></param>
        /// <param name="courseId"></param>
        public static void CreateCourseTagRef(CourseContent info, long courseId)
        {
            foreach (var tagId in info.CourseTagInfo)
            {
                var temp = Program.FSqlCourse.Insert<TCourseKnowTag>()
                    .AppendData(new TCourseKnowTag
                    {
                        CourseId = courseId,
                        TagId = tagId
                    }).ExecuteIdentity();
                Logger.Debug($"课程知识点关联表新增id[{temp}]的数据");
                InsertDataCount++;
            }
        }

        /// <summary>
        /// 新建课程结构
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static long GetNewCourseStructId(TCourseStruct ts)
        {
            var result = Program.FSqlCourse.Insert<TCourseStruct>().AppendData(ts).ExecuteIdentity();
            InsertDataCount++;
            InsertCourseStructCount++;//课程结构自增
            Logger.Debug($"课程[{ts.CourseNodeName}]新增节点[{result}]");
            return result;
        }

        /// <summary>
        /// 新建课件资源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long GetNewCoursewareId(Entity.Courseware.TCourseResource source)
        {
            var result = Program.FSqlCourseware.Insert<Entity.Courseware.TCourseResource>()
                .AppendData(source).ExecuteIdentity();
            Logger.Debug($"课件资源表插入资源文件[{result}]成功");
            InsertDataCount++;
            InsertCoursewareCount++;//课件数自增
            return result;
        }

        /// <summary>
        /// 新建课件资源库知识点表数据
        /// </summary>
        /// <param name="originalId"></param>
        /// <returns></returns>
        public static long GetNewCourseTag(long originalId)
        {
            var result = Program.FSqlCourseware.Insert<TagCourseware>()
                .AppendData(new TagCourseware {OriginalId = originalId}).ExecuteIdentity();
            Logger.Debug($"课件知识点表新增id[{result}]的数据");
            InsertDataCount++;
            return result;
        }

        /// <summary>
        /// 课件资源库-课件知识点关联表新增数据
        /// </summary>
        /// <param name="coursewareId"></param>
        /// <param name="tagId"></param>
        public static void CreateCoursewareTagRef(long coursewareId, long tagId)
        {
            var result = Program.FSqlCourseware.Insert<Entity.Courseware.TResourceTagRef>()
                .AppendData(new Entity.Courseware.TResourceTagRef {ResourceId = coursewareId, TagId = tagId, TCreate = DateTime.Now})
                .ExecuteAffrows();
            Logger.Debug($"课件资源库-课件知识点关联表新增{result}条记录");
            InsertDataCount++;
        }

        /// <summary>
        /// 课件资源库-课件知识点关联表新增课程相关知识点
        /// </summary>
        /// <param name="info"></param>
        /// <param name="coursewareId"></param>
        public static void CreateCoursewareTagRefCourse(CourseContent info, long coursewareId)
        {
            for (int i = 0; i < 4; i++)
            {
                var temp = Program.FSqlCourseware.Insert<Entity.Courseware.TResourceTagRef>()
                    .AppendData(new Entity.Courseware.TResourceTagRef
                    {
                        ResourceId = coursewareId,
                        TagId = info.CoursewareTagInfo[i],
                        TCreate = DateTime.Now
                    }).ExecuteAffrows();
                Logger.Debug($"课件资源库知识点关联表新增[{temp}]条数据");
                InsertDataCount++;
            }
        }

        /// <summary>
        /// 课程库课件资源表新增课件
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static long GetNewCourseResource(TCourseResource resource)
        {
            var result = Program.FSqlCourse.Insert<TCourseResource>()
                .AppendData(resource).ExecuteIdentity();
            Logger.Debug($"课件业务表插入资源文件[{result}]成功");
            InsertDataCount++;
            InsertTCourseResourceCount++;
            return result;
        }

        /// <summary>
        /// 课程库课程结构资源关联表新增数据
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        public static long GetNewStructResourceRef(TStructResource tr)
        {
            var result = Program.FSqlCourse.Insert<TStructResource>().AppendData(tr).ExecuteAffrows();
            InsertDataCount++;
            Logger.Debug($"课程结构与资源关联表插入{result}条数据");
            InsertTStructResourceCount++;
            return result;
        }

    }
}
