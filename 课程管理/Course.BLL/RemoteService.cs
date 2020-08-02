using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Course.BLL;
using System.Linq;

public class RemoteService
{
    public object GetCourseInfomationByID(pf_course_manage_v1Context db, List<PlanID> idList)
    {
        try
        {
            List<CourseInfomation> courseInfomations = new List<CourseInfomation>();
            if (idList != null && idList.Count > 0)
            {
                for (int i = 0; i < idList.Count; i++)
                {


                    var queryCourse = (from c in db.t_course
                                       where c.delete_flag == 0 && c.id == idList[i].planId
                                       select c).FirstOrDefault();
                    if (queryCourse == null)
                        return new { code = 200, message = "OK" };
                    CourseInfomation courseInfomation = new CourseInfomation();
                    courseInfomation.ID = queryCourse.id;
                    courseInfomation.CourseName = queryCourse.course_name;
                    courseInfomation.CourseDesc = queryCourse.course_desc;
                    courseInfomation.CourseCount = queryCourse.course_count;
                    courseInfomation.LearningTime = queryCourse.learning_time;
                    courseInfomation.ThumbnailPath = queryCourse.thumbnail_path;
                    courseInfomation.CourseConfidential = queryCourse.course_confidential;

                    //查找课程结构
                    var queryStruct = (from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.course_id == idList[i].planId
                                       select s).ToList();
                    List<CourseStructInfomation> courseStructsList = new List<CourseStructInfomation>();
                    foreach (var item in queryStruct)
                    {
                        CourseStructInfomation courseStruct = new CourseStructInfomation();
                        courseStruct.StructID = item.id;
                        courseStruct.ParentID = item.parent_id;
                        courseStruct.CourseNodeName = item.course_node_name;
                        courseStruct.NodeType = item.node_type;
                        courseStruct.ResourceCount = item.resource_count;
                        courseStruct.CreateTime = item.create_time;

                        //查找资源
                        var queryResourceID = (from r in db.t_struct_resource
                                               where r.course_struct_id == item.id
                                               select r).FirstOrDefault();
                        if (queryResourceID != null)
                        {
                            CourseResourceInfomation resourceInfomation = new CourseResourceInfomation();
                            var queryResource = (from s in db.t_course_resource
                                                 where s.delete_flag == 0 && s.id == queryResourceID.course_resouce_id
                                                 select s).FirstOrDefault();
                            if (queryResource != null)
                            {
                                resourceInfomation.ResourceName = queryResource.resource_name;
                                resourceInfomation.ResourceDesc = queryResource.resource_desc;
                                resourceInfomation.ResourceType = queryResource.resource_type;
                                resourceInfomation.ResourceExtension = queryResource.resource_extension;
                                resourceInfomation.GroupName = queryResource.group_name;
                                resourceInfomation.ResourceUrl = queryResource.resource_url;
                                resourceInfomation.ResourceConfidential = queryResource.resource_confidential;

                                //查找自定义课件脚本
                                var queryPage = (from p in db.t_courseware_page_bus
                                                 where p.delete_flag == 0 && p.courseware_resource_id == queryResource.id
                                                 select p).ToList();
                                List<CoursewarePage> pages = new List<CoursewarePage>();
                                foreach (var page in queryPage)
                                {
                                    pages.Add(new CoursewarePage
                                    {
                                        PageScript = page.page_script,
                                        Sort = (int)page.page_sort
                                    });
                                }
                                resourceInfomation.PageList = pages;
                            }
                            courseStruct.resourceInfomation = resourceInfomation;


                        }
                        courseStructsList.Add(courseStruct);
                        courseInfomation.courseStructsList = courseStructsList;
                    }
                    //查找知识点
                    List<KnowledgeTag> TagList = new List<KnowledgeTag>();
                    var queryTag = (from r in db.t_course_know_tag
                                    join t in db.t_knowledge_tag on r.tag_id equals t.id
                                    where r.course_id == idList[i].planId
                                    select t).ToList();
                    foreach (var item in queryTag)
                    {
                        KnowledgeTag tag = new KnowledgeTag();
                        tag.ID = (long)item.src_id;
                        tag.Tag = item.tag;
                        TagList.Add(tag);
                    }
                    courseInfomation.knowledgeTagsList = TagList;
                    courseInfomations.Add(courseInfomation);
                }
            }
            return courseInfomations;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public object TrainingPlanFromProgram(pf_course_manage_v1Context db, long programId)
    {
        try
        {
            ProgramCourseTask programCourseTask = new ProgramCourseTask();
            var queryProgram = (from p in db.t_training_program
                                where p.delete_flag == 0 && p.id == programId
                                select p).FirstOrDefault();
            if (queryProgram != null)
            {
                programCourseTask.RangePurpose = queryProgram.range_purpose;
                programCourseTask.StandardRequest = queryProgram.standard_request;
                programCourseTask.PlaneType = queryProgram.plane_type;
                programCourseTask.TrainType = queryProgram.train_type;
                programCourseTask.ProgramName = queryProgram.train_program_name;
                programCourseTask.EndorsementType = queryProgram.endorsement_type;
                programCourseTask.PlaneType1 = queryProgram.plane_type1;
                programCourseTask.SumDuration = queryProgram.sum_duration;
                programCourseTask.TrainTime = queryProgram.train_time;
                programCourseTask.UpDownTimes = queryProgram.up_down_times;
                programCourseTask.TechnicalGrade = queryProgram.technical_grade;
                programCourseTask.OtherRemark = queryProgram.other_remark;
            }
            var queryTask = (from a in db.t_program_task_ref
                             join b in db.t_training_task on a.taskid equals b.id
                             where a.delete_flag == 0 && a.programid == programId
                             select b).ToList();
            List<TrainingTaskInfomation> trainingTaskInfomation = new List<TrainingTaskInfomation>();
            foreach (var item in queryTask)
            {
                var querySubject = (from s in db.t_subject
                                    where s.delete_flag == 0 && s.task_id == item.id
                                    select s).ToList();
                List<SubjectInfo> subjects = new List<SubjectInfo>();
                foreach (var subject in querySubject)
                {
                    subjects.Add(new SubjectInfo
                    {
                        ID = subject.id,
                        TrainNumber = subject.subject_number,
                        TrainName = subject.subject_name,
                        TrainKind = subject.subject_kind,
                        PlaneType = subject.plane_type,
                        TrainDesc = subject.subject_desc,
                        ExpectResult = subject.expect_result
                    });
                }
                TrainingTaskInfomation trainingTask = new TrainingTaskInfomation();
                trainingTask.ID = (long)item.src_id;
                trainingTask.TaskName = item.task_name;
                trainingTask.TaskDesc = item.task_desc;
                trainingTask.Tag = item.knowledge_tag;
                trainingTask.CourseCount = item.course_count;
                trainingTask.TaskType = item.task_type;
                trainingTask.TypeLevel = item.type_level;
                trainingTask.Level = item.level;
                trainingTask.AirplaneType = item.airplane_type;
                trainingTask.SubjectList = subjects;
                trainingTaskInfomation.Add(trainingTask);
            }
            programCourseTask.trainingTaskInfomation = trainingTaskInfomation;

            List<CourseInfomation> courseInfomationList = new List<CourseInfomation>();
            var queryCourse = (from a in db.t_program_course_ref
                               join b in db.t_course on a.courseid equals b.id
                               where a.delete_flag == 0 && a.programid == programId
                               select b).ToList();
            foreach (var item in queryCourse)
            {
                CourseInfomation courseInfomation = new CourseInfomation();
                courseInfomation.ID = item.id;
                courseInfomation.CourseName = item.course_name;
                courseInfomation.CourseDesc = item.course_desc;
                courseInfomation.CourseCount = item.course_count;
                courseInfomation.LearningTime = item.learning_time;
                courseInfomation.ThumbnailPath = item.thumbnail_path;

                //查找课程结构
                var queryStruct = from s in db.t_course_struct
                                  where s.delete_flag == 0 && s.course_id == item.id
                                  select s;
                List<CourseStructInfomation> courseStructsList = new List<CourseStructInfomation>();
                foreach (var itemStruct in queryStruct)
                {
                    CourseStructInfomation courseStruct = new CourseStructInfomation();
                    courseStruct.StructID = itemStruct.id;
                    courseStruct.ParentID = itemStruct.parent_id;
                    courseStruct.CourseNodeName = itemStruct.course_node_name;
                    courseStruct.NodeType = itemStruct.node_type;
                    courseStruct.ResourceCount = itemStruct.resource_count;

                    //查找资源
                    var queryResourceID = (from r in db.t_struct_resource
                                           where r.course_struct_id == itemStruct.id
                                           select r).FirstOrDefault();
                    if (queryResourceID != null)
                    {
                        CourseResourceInfomation resourceInfomation = new CourseResourceInfomation();
                        var queryResource = (from s in db.t_course_resource
                                             where s.delete_flag == 0 && s.id == queryResourceID.course_resouce_id
                                             select s).FirstOrDefault();
                        if (queryResource != null)
                        {
                            resourceInfomation.ResourceName = queryResource.resource_name;
                            resourceInfomation.ResourceDesc = queryResource.resource_desc;
                            resourceInfomation.ResourceType = queryResource.resource_type;
                            resourceInfomation.ResourceExtension = queryResource.resource_extension;
                            resourceInfomation.GroupName = queryResource.group_name;
                            resourceInfomation.ResourceUrl = queryResource.resource_url;
                        }
                        courseStruct.resourceInfomation = resourceInfomation;
                    }
                    courseStructsList.Add(courseStruct);
                    courseInfomation.courseStructsList = courseStructsList;
                }
                //查找知识点
                List<KnowledgeTag> TagList = new List<KnowledgeTag>();
                var queryTag = (from r in db.t_course_know_tag
                                join t in db.t_knowledge_tag on r.tag_id equals t.id
                                where r.course_id == item.id
                                select t).ToList();
                foreach (var itemTag in queryTag)
                {
                    KnowledgeTag tag = new KnowledgeTag();
                    tag.ID = (long)itemTag.src_id;
                    tag.Tag = itemTag.tag;
                    TagList.Add(tag);
                }
                courseInfomation.knowledgeTagsList = TagList;
                courseInfomationList.Add(courseInfomation);
            }
            programCourseTask.courseInfomation = courseInfomationList;
            return programCourseTask;

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public object GetCourseIDByProgramID(pf_course_manage_v1Context db, long id)
    {
        try
        {
            var queryCourseIDList = db.t_program_course_ref.Where(x => x.delete_flag == 0 && x.programid == id).Select(x => (long)x.courseid).Distinct().ToList();
            return queryCourseIDList;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public object GetTaskIDByProgramID(pf_course_manage_v1Context db, long id)
    {
        try
        {
            var queryTaskIDList = (from r in db.t_program_task_ref
                                   join t in db.t_training_task on r.taskid equals t.id
                                   where r.delete_flag == 0 && r.programid == id
                                   select (long)t.src_id).Distinct().ToList();
            return queryTaskIDList;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
}

public class PlanID
{
    public long planId { get; set; }
}


