using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;

public class MyTraining
{
    public object GetMyTraining(pf_training_plan_v1Context db, string strStatus, string planName, string StartTime, string EndTime, string UserID, TokenModel token, int pageIndex, int pageSize)
    {
        try
        {
            //我要学的
            List<TrainingPlanModel> Temp = new List<TrainingPlanModel>();
            IQueryable<t_training_plan> query = null;
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                query = from s in db.t_trainingplan_stu
                        join p in db.t_training_plan on s.trainingplan_id equals p.id
                        where s.delete_flag == 0 && s.user_number == UserID && p.delete_flag == 0
                              && p.start_time >= DateTime.Parse(StartTime)
                              && p.end_time <= DateTime.Parse(EndTime)
                                && ((strStatus == "0" ? (new string[] { "2", "3" }).Contains(p.plan_status) : true) || p.publish_flag == 1)
                               && (strStatus == "1" ? (p.publish_flag == 1 && p.plan_status == "1") : true)
                               && (strStatus == "2" ? p.plan_status == strStatus : true)
                               && (strStatus == "3" ? p.plan_status == strStatus : true)
                              && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        select p;
            }
            else if (!string.IsNullOrEmpty(StartTime) && string.IsNullOrEmpty(EndTime))
            {
                query = from s in db.t_trainingplan_stu
                        join p in db.t_training_plan on s.trainingplan_id equals p.id
                        where s.delete_flag == 0 && s.user_number == UserID && p.delete_flag == 0
                              && p.start_time >= DateTime.Parse(StartTime)
                               && ((strStatus == "0" ? (new string[] { "2", "3" }).Contains(p.plan_status) : true) || p.publish_flag == 1)
                               && (strStatus == "1" ? (p.publish_flag == 1 && p.plan_status == "1") : true)
                               && (strStatus == "2" ? p.plan_status == strStatus : true)
                               && (strStatus == "3" ? p.plan_status == strStatus : true)
                              && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        select p;
            }
            else if (string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                query = from s in db.t_trainingplan_stu
                        join p in db.t_training_plan on s.trainingplan_id equals p.id
                        where s.delete_flag == 0 && s.user_number == UserID && p.delete_flag == 0
                              && p.end_time <= DateTime.Parse(EndTime)
                              && ((strStatus == "0" ? (new string[] { "2", "3" }).Contains(p.plan_status) : true) || p.publish_flag == 1)
                               && (strStatus == "1" ? (p.publish_flag == 1 && p.plan_status == "1") : true)
                               && (strStatus == "2" ? p.plan_status == strStatus : true)
                               && (strStatus == "3" ? p.plan_status == strStatus : true)
                              && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        select p;
            }
            else
            {
                query = from s in db.t_trainingplan_stu
                        join p in db.t_training_plan on s.trainingplan_id equals p.id
                        where s.delete_flag == 0 && s.user_number == UserID && p.delete_flag == 0
                               && ((strStatus == "0" ? (new string[] { "2", "3" }).Contains(p.plan_status) : true) || p.publish_flag == 1)
                               && (strStatus == "1" ? (p.publish_flag == 1 && p.plan_status == "1") : true)
                               && (strStatus == "2" ? p.plan_status == strStatus : true)
                               && (strStatus == "3" ? p.plan_status == strStatus : true)
                              && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        select p;
            }
            var list = query.Distinct().ToList();
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Temp.Add(new TrainingPlanModel()
                    {
                        ID = list[i].id,
                        PlanName = list[i].plan_name,
                        PlanDesc = list[i].plan_desc,
                        StartTime = list[i].start_time.ToString(),
                        EndTime = list[i].end_time.ToString(),
                        QuitFlag = list[i].quit_flag,
                        PlanStatus = list[i].plan_status,
                        Div = "1",
                        PublishFlag = list[i].publish_flag,
                        CourseFlag = list[i].course_flag.ToString(),
                        TaskFlag = list[i].task_flag.ToString(),
                        ExamFlag = list[i].exam_flag.ToString()
                    });
                }
            }
            IEnumerable<TrainingPlanModel> resultList = Temp.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            return new { code = 200, result = new { resultList, count = Temp.Count() }, msg = "成功" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    private List<TrainingPlanModel> GetMyTrainingTask(pf_training_plan_v1Context db, string strStatus, string planName, string StartTime, string EndTime, TokenModel token)
    {
        try
        {
            List<TrainingPlanModel> Temp = new List<TrainingPlanModel>();
            IQueryable<t_training_plan> queryMy = null;
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                queryMy = from a in db.t_training_plan
                          where a.delete_flag == 0
                          && a.create_by == token.userId
                          && a.start_time >= DateTime.Parse(StartTime)
                          && a.end_time <= DateTime.Parse(EndTime)
                          && (strStatus == "0" ? true : a.plan_status == strStatus)
                          && (string.IsNullOrEmpty(planName) ? true : a.plan_name.Contains(planName))
                          select a;
            }
            else if (string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                queryMy = from a in db.t_training_plan
                          where a.delete_flag == 0
                          && a.create_by == token.userId
                          && a.end_time <= DateTime.Parse(EndTime)
                          && (strStatus == "0" ? true : a.plan_status == strStatus)
                          && (string.IsNullOrEmpty(planName) ? true : a.plan_name.Contains(planName))
                          select a;
            }
            else if (!string.IsNullOrEmpty(StartTime) && string.IsNullOrEmpty(EndTime))
            {
                queryMy = from a in db.t_training_plan
                          where a.delete_flag == 0
                          && a.create_by == token.userId
                          && a.start_time >= DateTime.Parse(StartTime)
                          && (strStatus == "0" ? true : a.plan_status == strStatus)
                          && (string.IsNullOrEmpty(planName) ? true : a.plan_name.Contains(planName))
                          select a;
            }
            else
            {
                queryMy = from a in db.t_training_plan
                          where a.delete_flag == 0
                          && a.create_by == token.userId
                          && (strStatus == "0" ? true : a.plan_status == strStatus)
                          && (string.IsNullOrEmpty(planName) ? true : a.plan_name.Contains(planName))
                          select a;
            }

            foreach (var item in queryMy)
            {
                Temp.Add(new TrainingPlanModel()
                {
                    ID = item.id,
                    PlanName = item.plan_name,
                    PlanDesc = item.plan_desc,
                    StartTime = item.start_time.ToString(),
                    EndTime = item.end_time.ToString(),
                    QuitFlag = item.quit_flag,
                    PlanStatus = item.plan_status,
                    Div = "2"
                });
            }
            return Temp;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }
    }

    public object GetStuTrainingPlanContent(pf_training_plan_v1Context db, long PlanID, string UserID)
    {
        try
        {
            List<TrainingPlanContent> list = new List<TrainingPlanContent>();

            //配置条件
            List<PlanContent> planContent = new List<PlanContent>();

            var queryCondition1 = (from p in db.t_plan_course_task_exam_ref
                                   join c in db.t_course on p.content_id equals c.id
                                   where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "1"//课程
                                   orderby p.content_sort ascending
                                   select new
                                   {
                                       p.id,
                                       p.dif,
                                       c.course_name,
                                   }).ToList();
            foreach (var item in queryCondition1)
            {
                planContent.Add(new PlanContent()
                {
                    ID = item.id,
                    ConditionName = item.course_name,
                    Dif = item.dif,
                    SelectFlag = "0"
                });
            }

            var queryCondition2 = (from p in db.t_plan_course_task_exam_ref
                                   join t in db.t_task_bus on p.content_id equals t.id
                                   where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "2"//训练任务
                                   orderby p.content_sort ascending
                                   select new
                                   {
                                       p.id,
                                       p.dif,
                                       t.task_name
                                   }).ToList();
            foreach (var item in queryCondition2)
            {
                planContent.Add(new PlanContent()
                {
                    ID = item.id,
                    ConditionName = item.task_name,
                    Dif = item.dif,
                    SelectFlag = "0"
                });
            }

            var queryCondition3 = (from p in db.t_plan_course_task_exam_ref
                                   join t in db.t_examination_manage on p.content_id equals t.id
                                   where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "3"//训练任务
                                   orderby p.content_sort ascending
                                   select new
                                   {
                                       p.id,
                                       p.dif,
                                       t.exam_name
                                   }).ToList();
            foreach (var item in queryCondition3)
            {
                planContent.Add(new PlanContent()
                {
                    ID = item.id,
                    ConditionName = item.exam_name,
                    Dif = item.dif,
                    SelectFlag = "0"
                });
            }

            planContent = planContent.OrderBy(x => x.Sort).ToList();

            var queryCourse = from p in db.t_plan_course_task_exam_ref
                              join c in db.t_course on p.content_id equals c.id
                              where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "1"//课程
                              orderby p.content_sort ascending
                              select new
                              {
                                  p.id,
                                  p.content_sort,
                                  p.dif,
                                  p.teacher_num,
                                  p.teacher_name,
                                  SrcID = c.id,
                                  c.course_name,
                                  c.course_count,
                                  c.learning_time
                              };
            var queryCourseList = queryCourse.ToList();
            if (queryCourseList.Count > 0)
            {
                for (int i = 0; i < queryCourseList.Count; i++)
                {
                    //知识点
                    var qq = from k in db.t_course_know_tag
                             join t in db.t_knowledge_tag on k.tag_id equals t.id
                             where k.course_id == queryCourseList[i].SrcID
                             select new { t.tag };
                    var qqList = qq.ToList();
                    List<string> listTag = new List<string>();
                    if (qqList.Count > 0)
                    {
                        for (int j = 0; j < qqList.Count; j++)
                        {
                            listTag.Add(qqList[j].tag);
                        }
                    }

                    List<PlanContent> planContentTemp = new List<PlanContent>();
                    for (int k = 0; k < planContent.Count; k++)
                    {
                        planContentTemp.Add(new PlanContent()
                        {
                            ID = planContent[k].ID,
                            ConditionName = planContent[k].ConditionName,
                            Dif = planContent[k].Dif,
                            SelectFlag = planContent[k].SelectFlag
                        });
                    }

                    var query = from c in db.t_config_learning_condition
                                where c.delete_flag == 0 && c.content_id == queryCourseList[i].id
                                select new { c.condition_id };
                    var configList = query.ToList();
                    if (configList.Count > 0)
                    {
                        for (int k = 0; k < configList.Count; k++)
                        {
                            PlanContent obj = planContentTemp.Find(x => x.ID == configList[k].condition_id);
                            if (obj != null)
                                obj.SelectFlag = "1";
                        }
                    }
                    //筛选之前的
                    List<PlanContent> tempList = planContentTemp.FindAll(x => x.Sort < queryCourseList[i].content_sort);

                    string strTemp = "0";//未开始
                    var q = from r in db.t_learning_record
                            where r.content_id == queryCourseList[i].id && r.user_number == UserID && r.delete_flag==0
                            select r;
                    var qqq = q.FirstOrDefault();
                    if (qqq != null)
                    {
                        strTemp = qqq.learning_progress;
                    }

                    list.Add(new TrainingPlanContent()
                    {
                        ID = queryCourseList[i].id,
                        SrcID = queryCourseList[i].SrcID,
                        dif = 1,
                        tags = listTag,
                        ContentSort = queryCourseList[i].content_sort,
                        ContentName = queryCourseList[i].course_name,
                        TeacherID = queryCourseList[i].teacher_num,
                        TeacherName = queryCourseList[i].teacher_name,
                        TimeInfo = queryCourseList[i].learning_time,
                        ConditionList = tempList,
                        LearnProgress = strTemp
                    });
                }
            }

            var queryTask = (from p in db.t_plan_course_task_exam_ref
                             join t in db.t_task_bus on p.content_id equals t.id
                             where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "2"//任务
                             orderby p.content_sort ascending
                             select new
                             {
                                 p.id,
                                 p.content_sort,
                                 p.dif,
                                 p.teacher_num,
                                 p.teacher_name,
                                 SrcID = t.id,
                                 knowledge_tag = t.tag_display,//知识点
                                 t.task_name,//训练名称
                                 course_count = t.class_hour//课时数
                             }).ToList();
            foreach (var item in queryTask)
            {
                List<PlanContent> planContentTemp = new List<PlanContent>();
                for (int k = 0; k < planContent.Count; k++)
                {
                    planContentTemp.Add(new PlanContent()
                    {
                        ID = planContent[k].ID,
                        ConditionName = planContent[k].ConditionName,
                        Dif = planContent[k].Dif,
                        SelectFlag = planContent[k].SelectFlag
                    });
                }

                var query = from c in db.t_config_learning_condition
                            where c.delete_flag == 0 && c.content_id == item.id
                            select new { c.condition_id };
                var configList = query.ToList();
                if (configList.Count > 0)
                {
                    for (int k = 0; k < configList.Count; k++)
                    {
                        PlanContent obj = planContentTemp.Find(x => x.ID == configList[k].condition_id);
                        if (obj != null)
                            obj.SelectFlag = "1";
                    }
                }
                string strTag = item.knowledge_tag;
                List<string> listTag = new List<string>();
                if (strTag.Contains(','))
                {
                    string[] TagArray = strTag.Split(',');
                    for (int i = 0; i < TagArray.Length; i++)
                    {
                        listTag.Add(TagArray[i]);
                    }
                }
                //筛选之前的
                List<PlanContent> tempList = planContentTemp.FindAll(x => x.Sort < item.content_sort);

                list.Add(new TrainingPlanContent()
                {
                    ID = item.id,//计划内容ID
                    SrcID = item.SrcID,//源ID
                    dif = 2,//任务
                    ContentSort = item.content_sort,//排序
                    ContentName = item.task_name,//任务名称
                    TeacherID = item.teacher_num,//教员账号
                    TeacherName = item.teacher_name,//教员姓名
                    TimeInfo = (int)item.course_count,//课时数
                    ConditionList = tempList,
                    tags = listTag
                });
            }

            var queryExam = (from p in db.t_plan_course_task_exam_ref
                             join t in db.t_examination_manage on p.content_id equals t.id
                             where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "3"//考试
                             orderby p.content_sort ascending
                             select new
                             {
                                 p.id,
                                 p.content_sort,
                                 p.dif,
                                 p.teacher_num,
                                 p.teacher_name,
                                 t.exam_div,
                                 SrcID = t.src_id,
                                 t.exam_name,//考试名称
                                 t.exam_duration//考试时长
                             }).ToList();
            foreach (var item in queryExam)
            {
                List<PlanContent> planContentTemp = new List<PlanContent>();
                for (int k = 0; k < planContent.Count; k++)
                {
                    planContentTemp.Add(new PlanContent()
                    {
                        ID = planContent[k].ID,
                        ConditionName = planContent[k].ConditionName,
                        Dif = planContent[k].Dif,
                        SelectFlag = planContent[k].SelectFlag
                    });
                }
                var query = from c in db.t_config_learning_condition
                            where c.delete_flag == 0 && c.content_id == item.id
                            select new { c.condition_id };
                var configList = query.ToList();
                if (configList.Count > 0)
                {
                    for (int k = 0; k < configList.Count; k++)
                    {
                        PlanContent obj = planContentTemp.Find(x => x.ID == configList[k].condition_id);
                        if (obj != null)
                            obj.SelectFlag = "1";
                    }
                }
                //筛选之前的
                List<PlanContent> tempList = planContentTemp.FindAll(x => x.Sort < item.content_sort);

                string strTemp = "";//未开始
                var q = from r in db.t_learning_record
                        where r.content_id == item.id && r.user_number == UserID
                        select r;
                var qqq = q.FirstOrDefault();
                if (qqq != null)
                {
                    strTemp = qqq.learning_progress;
                }

                list.Add(new TrainingPlanContent()
                {
                    ID = item.id,//计划内容ID
                    SrcID = item.SrcID,//源ID
                    dif = 3,//任务  
                    TaskExamDiv = item.exam_div,//1：理论，2：实践
                    ContentSort = item.content_sort,//排序
                    ContentName = item.exam_name,//任务名称
                    TeacherID = item.teacher_num,//教员账号
                    TeacherName = item.teacher_name,//教员姓名
                    TimeInfo = (int)item.exam_duration,//课时数
                    ConditionList = tempList,
                    LearnProgress = strTemp
                });
            }

            var queryStu = from s in db.t_trainingplan_stu
                           where s.trainingplan_id == PlanID && s.delete_flag == 0
                           select s.id;
            var queryCount = queryStu.Count();

            return new { code = 200, result = list.OrderBy(x => x.ContentSort).ToList(), Count = queryCount, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    private List<TrainingPlanConditionInfo> GetCourseLearningCondition(pf_training_plan_v1Context db, long PlanID)
    {
        try
        {
            List<TrainingPlanConditionInfo> list = new List<TrainingPlanConditionInfo>();
            var queryCourse = from p in db.t_plan_course_task_exam_ref
                              join c in db.t_config_learning_condition on p.id equals c.content_id into pc
                              from _pc in pc.DefaultIfEmpty()
                              join cc in db.t_course on _pc.condition_id equals cc.id
                              where p.plan_id == PlanID && p.delete_flag == 0 && _pc.delete_flag == 0 && _pc.dif == "1"//课程
                              select new { ContentID = p.content_id, _pc.id, _pc.dif, cc.course_name };
            var queryCourseList = queryCourse.ToList();
            if (queryCourseList.Count > 0)
            {
                for (int i = 0; i < queryCourseList.Count; i++)
                {
                    list.Add(new TrainingPlanConditionInfo()
                    {
                        ID = queryCourseList[i].id,//配置条件表ID
                        ContentID = queryCourseList[i].ContentID,//计划内容ID
                        ConditionName = queryCourseList[i].course_name,
                        Dif = "1"
                    });
                }
            }
            return list;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }

    }
    public object GetCourseInfoByID(pf_training_plan_v1Context db, IConfiguration configuration, long CourseID, long RecordID)
    {
        try
        {
            //课程信息
            var query = from c in db.t_course
                        where c.delete_flag == 0 && c.id == CourseID
                        select new { c.course_name, c.course_count, c.learning_time, c.course_desc, c.thumbnail_path, c.course_confidential };
            var q = query.FirstOrDefault();

            //学习进度
            var progress = from r in db.t_learning_record
                           where r.id == RecordID && r.delete_flag == 0
                           select r.learning_progress;

            //学习时长差值计算
            var queryLearningTimeList = (from s in db.t_course_node_learning_status
                                         where s.delete_flag == 0 && s.record_id == RecordID
                                         select s.learning_time).ToList();
            var learningTime = queryLearningTimeList.Sum();
            //剩余学习时长
            var difTime = (double)(q.learning_time * 3600 - learningTime) / 3600.00;
            if (difTime < 0)
                difTime = 0;

            //最近学习的节点
            long StructID = 0;
            string StructName = "";
            string Extension = "";
            var queryStatus = from s in db.t_course_node_learning_status
                              where s.record_id == RecordID && s.delete_flag == 0
                              orderby s.last_learning_time descending
                              select new { s.id, s.course_id, s.node_id };
            var qs = queryStatus.FirstOrDefault();
            if (qs != null)
            {
                var quer = (from s in db.t_course_struct
                            join t in db.t_struct_resource on s.id equals t.course_struct_id
                            join r in db.t_course_resource on t.course_resouce_id equals r.id into tr
                            from _tr in tr.DefaultIfEmpty()
                            where s.course_id == qs.course_id && s.struct_id == qs.node_id
                            select new { s.id, s.struct_id, s.course_node_name, _tr.resource_extension }).FirstOrDefault();
                if (quer != null)
                {
                    StructID = quer.struct_id;
                    StructName = quer.course_node_name;
                    Extension = quer.resource_extension;
                }
            }
            string thumbnail_path = "";
            if (!string.IsNullOrEmpty(q.thumbnail_path))
                thumbnail_path = configuration["FastDFSUrl"] + q.thumbnail_path;
            return new
            {
                code = 200,
                result = new
                {
                    CourseInfo = new
                    {
                        q.course_name,
                        course_count = q.learning_time.ToString("#0.00"),
                        q.course_desc,
                        thumbnail_path,
                        q.course_confidential
                    },
                    Process = progress.FirstOrDefault(),
                    StructID,
                    StructName,
                    Extension,
                    residueLearningTime = difTime.ToString("#0.00")
                },
                msg = "OK"
            };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    private List<CourseStruct> list;
    public object GetLearningCourseStruct(pf_training_plan_v1Context db, long courseid, long recordID)
    {
        try
        {
            list = new List<CourseStruct>();
            var query = from s in db.t_course_node_learning_status
                        where s.delete_flag == 0 && s.course_id == courseid && s.record_id == recordID && s.course_struct_id == 0
                        orderby s.create_time ascending
                        select s;
            var temp = query.ToList();
            if (temp.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    CourseStruct node = new CourseStruct();
                    node.ID = temp[i].node_id;
                    node.NodeNama = temp[i].node_name;
                    node.NodeType = temp[i].node_type;
                    node.ParentID = temp[i].course_struct_id;
                    node.ResourceExtension = temp[i].resource_extension;
                    node.LearningProcess = temp[i].node_status;
                    node.ResourceCount = temp[i].resource_count;
                    GetChildCourseStruct(db, node, recordID);
                    list.Add(node);
                }
            }
            return new { code = 200, result = list, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    private void GetChildCourseStruct(pf_training_plan_v1Context db, CourseStruct courseStruct, long recordID)
    {
        try
        {
            var query = from s in db.t_course_node_learning_status
                        where s.delete_flag == 0 && s.course_struct_id == courseStruct.ID && s.record_id == recordID
                        orderby s.create_time ascending
                        select s;
            var temp = query.ToList();
            if (temp.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    CourseStruct node = new CourseStruct();
                    node.ID = temp[i].node_id;
                    node.NodeNama = temp[i].node_name;
                    node.NodeType = temp[i].node_type;
                    node.ParentID = temp[i].course_struct_id;
                    node.ResourceExtension = temp[i].resource_extension;
                    node.LearningProcess = temp[i].node_status; ;
                    node.ResourceCount = temp[i].resource_count;
                    GetChildCourseStruct(db, node, recordID);
                    courseStruct.Children.Add(node);
                }
            }

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            // return new { code = 400, msg = "Error" };
        }

    }

    public object CheckLearningCondition(pf_training_plan_v1Context db,long ConditionID, string UserID, IHttpClientHelper client)
    {
        try
        {
                //查找课程的限制条件
                var queryCourseCondition = from c in db.t_config_learning_condition
                                           where c.delete_flag == 0 && c.content_id == ConditionID
                                           select new { c.condition_id };
                var queryCourseConditionList = queryCourseCondition.ToList();
                if (queryCourseConditionList.Count > 0)//存在限制条件
                {
                    for (int i = 0; i < queryCourseConditionList.Count; i++)
                    {
                        //判断是课程、训练任务、课程
                        var queryPlanContent = from p in db.t_plan_course_task_exam_ref
                                               where p.id == queryCourseConditionList[i].condition_id && p.delete_flag == 0
                                               select p;
                        var queryPlanContentF = queryPlanContent.FirstOrDefault();
                        if (queryPlanContentF == null)
                            return new { code = 401, msg = "计划内容不存在！" };

                        if (queryPlanContentF.dif == "1")//课程
                        {
                            var queryLearningRecord = from r in db.t_learning_record
                                                      where r.delete_flag == 0
                                                            && r.content_id == queryCourseConditionList[i].condition_id
                                                      && r.user_number == UserID
                                                      select r;
                            var q = queryLearningRecord.FirstOrDefault();
                            if (q != null)//存在学习记录
                            {
                                if (q.learning_progress != "100")//告诉前端，XX课程还未学习完成
                                {
                                    var query_course = from c in db.t_course
                                                       where c.id == queryPlanContentF.content_id
                                                       select new { c.course_name };
                                    return new { code = 401, message = "请先完成课程：" + query_course.FirstOrDefault().course_name };
                                }
                            }
                            else
                                return new { code = 401, message = "您还不满足学习此课程的条件哦！" };
                        }
                        else if (queryPlanContentF.dif == "2")//训练任务
                        {
                            return new { code = 200, message = "OK" };
                        }
                        else//考试
                        {
                            var queryExam = from e in db.t_examination_manage
                                            where e.id == queryPlanContentF.content_id && e.delete_flag == 0
                                            select e;
                            var queryExamF = queryExam.FirstOrDefault();

                            //调用远程服务
                            string Url = "http://EXAMINATION-SERVICE/examination/v1/GetStuExamResult?";
                            string strFullUrl = Url + "examId=" + queryExamF.src_id + "&userNumber=" + UserID;
                            string strResult = client.GetRequest(strFullUrl).Result;
                            ReponseResult obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ReponseResult>(strResult);
                            if (obj.code == "200")
                            {
                                if (obj.result == "1.0" || obj.result == "1")//通过
                                    return new { code = 200, message = "OK" };
                                else//未通过
                                    return new { code = 401, message = "您的考试：" + queryExamF.exam_name + ",还未通过！" };
                            }
                            return new { code = 200, message = "OK" };
                        }
                    }
                    return new { code = 200, message = "OK" };
                }
                else//不存在限制条件
                    return new { code = 200, message = "OK" };           
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object Add_LearningRecord(pf_training_plan_v1Context db, RabbitMQClient rabbit,long ContentID, string UserID, TokenModel token)
    {
        try
        {

                var query = from r in db.t_learning_record
                            where r.content_id == ContentID && r.user_number == UserID && r.delete_flag == 0
                            select r;
                var q = query.FirstOrDefault();
                if (q == null)//创建
                {
                    t_learning_record obj = new t_learning_record();
                    obj.content_id = ContentID;
                    obj.user_number = UserID;
                    obj.learning_progress = "0";
                    obj.learning_time = DateTime.Now;
                    obj.create_time = DateTime.Now;
                    obj.update_time = DateTime.Now;
                    db.t_learning_record.Add(obj);

                    if (db.SaveChanges() > 0)
                    {
                        long MaxRecord = obj.id;//最大值返回给前端，用户后续记录节点的学习状态

                        //日志消息产生
                        SysLogModel log = new SysLogModel();
                        log.opNo = token.userNumber;
                        log.opName = token.userName;
                        log.opType = 2;
                        log.logDesc = "创建了一条学习记录";
                        log.logSuccessd = 1;
                        log.moduleName = "我的培训";
                        rabbit.LogMsg(log);
                        return new { code = 200, result = MaxRecord, message = "OK" };
                    }
                    else
                        return new { code = 400, message = "Error" };
                }
                else//修改
                {
                    q.update_time = DateTime.Now;
                    if (db.SaveChanges() > 0)
                        return new { code = 200, result = q.id, message = "OK" };
                    else
                        return new { code = 200, result = q.id, message = "OK" };
                }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    public object GetStuFromPlan(pf_training_plan_v1Context db,IConfiguration configuration, long PlanID, DicModel dicDepartment)
    {
        try
        {
                string fastDFS = configuration["FastDFSUrl"];
                var query = from s in db.t_trainingplan_stu
                            where s.delete_flag == 0 && s.trainingplan_id == PlanID
                            select new { s.user_number, s.user_name, s.department, s.photo_path };
                List<PlanStudent> list = new List<PlanStudent>();
                foreach (var item in query)
                {
                    string strDepartment = "";//部门
                    try
                    {
                        if (!string.IsNullOrEmpty(item.department))
                            strDepartment = dicDepartment.Result.Find(x => x.DicCode == item.department).CodeDsc;
                    }
                    catch (Exception)
                    { }

                    string path = "";
                    if (!string.IsNullOrEmpty(item.photo_path))
                        path = fastDFS + item.photo_path;

                    list.Add(new PlanStudent()
                    {
                        user_name = item.user_name,
                        department = strDepartment,
                        photo_path = path
                    });
                }
                return new { code = 200, result = list, msg = "OK" };            
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public object LearningCoursewareByStructNodeID(pf_training_plan_v1Context db,IConfiguration configuration, long RecordID, long NodeID)
    {
        try
        {
                string str = configuration["FastDFSUrl"];
                var q = from s in db.t_course_node_learning_status
                        where s.record_id == RecordID && s.node_id == NodeID && s.delete_flag == 0
                        select s;
                var qf = q.FirstOrDefault();
                if (qf == null)//没有记录
                {

                    var query = from s in db.t_struct_resource
                                join c in db.t_course_resource on s.course_resouce_id equals c.id
                                where s.course_struct_id == NodeID
                                select new { c.id, c.group_name, c.resource_url, c.resource_type, c.resource_name };
                    var qq = query.FirstOrDefault();
                    if (qq != null)
                        return new { code = 200, result = new { NodeInfo = new { id = qq.id, resource_url = str + "/" + qq.group_name + "/" + qq.resource_url, resource_type = qq.resource_type, resource_name = qq.resource_name }, Process = "0" }, message = "OK" };
                    else
                        return new { code = 200, msg = "" };
                }
                else
                {
                    if (qf.node_status == "100")//已完成此节点的学习
                    {
                        var queryStruct = (from a in db.t_course_struct
                                           where a.delete_flag == 0 && a.course_id == qf.course_id && a.struct_id == NodeID
                                           select a).FirstOrDefault();

                        var query = from s in db.t_struct_resource
                                    join c in db.t_course_resource on s.course_resouce_id equals c.id
                                    where s.course_struct_id == queryStruct.id
                                    select new { c.id, c.group_name, c.resource_url, c.resource_type, c.resource_name };
                        var qq = query.FirstOrDefault();
                        return new { code = 200, result = new { NodeInfo = new { id = qq.id, resource_url = str + "/" + qq.group_name + "/" + qq.resource_url, resource_type = qq.resource_type, resource_name = qq.resource_name }, Process = "100" }, message = "OK" };
                    }
                    else//进行中
                    {
                        var queryStruct = (from a in db.t_course_struct
                                           where a.delete_flag == 0 && a.course_id == qf.course_id && a.struct_id == NodeID
                                           select a).FirstOrDefault();

                        var query = from s in db.t_struct_resource
                                    join c in db.t_course_resource on s.course_resouce_id equals c.id
                                    where s.course_struct_id == queryStruct.id
                                    select new { c.id, c.group_name, c.resource_url, c.resource_type, c.resource_name };
                        var qq = query.FirstOrDefault();
                        return new { code = 200, result = new { NodeInfo = new { id = qq.id, resource_url = str + "/" + qq.group_name + "/" + qq.resource_url, resource_type = qq.resource_type, resource_name = qq.resource_name }, Process = qf.node_status }, message = "OK" };
                    }

                }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object LearningCustomCourseWare(pf_training_plan_v1Context db,long courseid, long recordId, long nodeId)
    {
        try
        {
                int pageNumber = 0;
                //查找当前学习的节点
                var queryPageNum = db.t_course_node_learning_status.Where(x => x.record_id == recordId && x.node_id == nodeId).FirstOrDefault();
                if (queryPageNum != null)
                    pageNumber = queryPageNum.learning_page_number;

                //自定义课件脚本

                string courseTitle = "";
                var query = (from a in db.t_course_struct
                             join s in db.t_struct_resource on a.id equals s.course_struct_id
                             join c in db.t_course_resource on s.course_resouce_id equals c.id
                             join p in db.t_courseware_page_bus on c.id equals p.courseware_resource_id
                             where a.struct_id == nodeId && a.course_id == courseid
                             orderby p.page_sort ascending
                             select new { c, p }).ToList();
                List<CoursewarePage> list = new List<CoursewarePage>();
                foreach (var item in query)
                {
                    courseTitle = item.c.resource_name;
                    list.Add(new CoursewarePage
                    {
                        PageScript = item.p.page_script,
                        Sort = (int)item.p.page_sort
                    });
                }
                return new
                {
                    code = 200,
                    result = new
                    {
                        list,
                        pageNumber,
                        courseTitle
                    },
                    message = "OK"
                };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    /// <summary>
    /// 点击返回时候，调用此方法，记录他的学习状态
    /// </summary>
    /// <param name="RecordID"></param>
    /// <param name="SrcID"></param>
    /// <param name="StructID"></param>
    /// <returns></returns>
    public object Add_CourseNodeLearningStatus(pf_training_plan_v1Context db, RabbitMQClient rabbit,long RecordID, long SrcID, long StructID, string NodeStatus, int pageNum, int LearningTime, TokenModel token, IHttpClientHelper client)
    {
        try
        {
                var queryRecord = from r in db.t_learning_record
                                  where r.id == RecordID && r.delete_flag == 0
                                  select r;
                var record = queryRecord.FirstOrDefault();

                //查找培训计划ID
                var queryPlanRef = from pf in db.t_plan_course_task_exam_ref
                                   where pf.delete_flag == 0 && pf.id == record.content_id
                                   select pf;
                long planid = (long)queryPlanRef.FirstOrDefault().plan_id;
                var queryPlanStatus = (from n in db.t_training_plan
                                       where n.delete_flag == 0 && n.id == planid
                                       select n).FirstOrDefault();
                if (queryPlanStatus.plan_status == "3")
                    return new { code = 400, message = "培训计划已结束，不记录学习时长" };

                long StatusID = 0;
                var query = from s in db.t_course_node_learning_status
                            where s.delete_flag == 0
                                  && s.record_id == RecordID
                                  && s.course_id == SrcID
                                  && s.node_id == StructID
                            select s;
                var q = query.FirstOrDefault();
                if (q != null)
                {
                    q.node_status = NodeStatus;
                    q.learning_page_number = pageNum;
                    q.learning_time = q.learning_time + LearningTime;
                    q.attempt_number = q.attempt_number + 1;
                    q.last_learning_time = DateTime.Now;
                    q.update_time = DateTime.Now;
                    StatusID = q.id;
                    db.SaveChanges();
                }

                /*********更新节点进度*************/
                UpdateNodeStatus(db, RecordID, StructID);

                /*********更新记录表（t_learning_record）学习时长*************/
                UpdateLearningTimeLog(db, RecordID, LearningTime, StatusID);

                /*********更新培训计划学生个人课程进度*************/
                UpdateRecordProgress(db, RecordID, SrcID);

                /*********更新课程的平均时长、完成率************/
                UpdateCourseAvgTimeFinishRate(db, planid, RecordID, (long)record.content_id);

                //查询课程数量
                var queryPlanR = from f in db.t_plan_course_task_exam_ref
                                 where f.delete_flag == 0 && f.plan_id == planid && f.dif == "1"
                                 select f;
                int courseCount = queryPlanR.Count();

                /*********更新培训计划完成度*************/
                UpdateTrainingPlanFinishRate(db, client, planid, courseCount);

                /*********更新培训计划统计表进度*******/
                UpdateTrainingPlanStatistic(db, courseCount, planid, token.userNumber, token.userId);

                /*********更新培训计划统计表进度End*******/

                db.SaveChanges();

                //日志消息产生
                SysLogModel g = new SysLogModel();
                g.opNo = token.userNumber;
                g.opName = token.userName;
                g.opType = 3;
                g.moduleName = "我的培训";
                g.logDesc = "学习了时长为：" + LearningTime;
                g.logSuccessd = 1;
                rabbit.LogMsg(g);
               // PubMethod.Log(g);
                return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    /// <summary>
    /// 更新记录表（t_learning_record）学习时长
    /// </summary>
    /// <param name="db"></param>
    /// <param name="RecordID"></param>
    /// <param name="LearningTime"></param>
    /// <param name="StatusID"></param>
    private void UpdateLearningTimeLog(pf_training_plan_v1Context db, long RecordID, int LearningTime, long StatusID)
    {
        try
        {
            //更新记录表（t_learning_record）学习时长
            var queryLearningSumTime = db.t_course_node_learning_status.Where(x => x.delete_flag == 0 && x.record_id == RecordID).Select(x => x.learning_time).ToList().Sum();
            var queryLearnTimeRecord = from r in db.t_learning_record
                                       where r.delete_flag == 0 && r.id == RecordID
                                       select r;
            var queryRecordF = queryLearnTimeRecord.FirstOrDefault();
            queryRecordF.learning_sum_time = queryLearningSumTime;

            //创建学习记录日志
            t_course_node_learning_log log = new t_course_node_learning_log();
            log.attempt_time = LearningTime;
            log.delete_flag = 0;
            log.status_id = StatusID;
            log.t_create = DateTime.Now;
            log.t_modified = DateTime.Now;
            db.t_course_node_learning_log.Add(log);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }
    /// <summary>
    /// 更新培训计划学生个人课程进度
    /// </summary>
    /// <param name="db"></param>
    /// <param name="RecordID"></param>
    /// <param name="SrcID"></param>
    /// <param name="SecLevel"></param>
    private void UpdateRecordProgress(pf_training_plan_v1Context db, long RecordID, long SrcID)
    {
        try
        {
            var queryRecord = from r in db.t_learning_record
                              where r.id == RecordID && r.delete_flag == 0
                              select r;
            var record = queryRecord.FirstOrDefault();

            //查询课程下的所有资源数量(内连接查询)
            var queryCourse = from c in db.t_course_struct
                              join s in db.t_struct_resource on c.id equals s.course_struct_id
                              where c.course_id == SrcID && c.node_type == "2" && c.delete_flag == 0
                              //&& (SecLevel != "0" ? true : (c.resource_confidential == "1" || c.resource_confidential == "2" || c.resource_confidential == null || c.resource_confidential == ""))
                              select c.id;
            var ResourceCount = queryCourse.Count();

            //查找记录中已经学完的节点
            var queryNodeRecord = from n in db.t_course_node_learning_status
                                  where n.delete_flag == 0
                                  && n.record_id == RecordID
                                  && n.course_id == SrcID
                                  && n.resource_extension != null
                                  // && n.node_status == "100"
                                  select n;
            float sumLearn = 0;
            foreach (var item in queryNodeRecord)
            {

                sumLearn = sumLearn + float.Parse(item.node_status);
            }
            float sum = ResourceCount * 100;
            float nProgress = (sumLearn / sum) * 100;//计算百分比前数字

            record.learning_progress = nProgress.ToString();
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }
    /// <summary>
    /// 更新课程的平均时长、完成率
    /// </summary>
    /// <param name="db"></param>
    /// <param name="planid"></param>
    /// <param name="RecordID"></param>
    /// <param name="contentId"></param>
    private void UpdateCourseAvgTimeFinishRate(pf_training_plan_v1Context db, long planid, long RecordID, long contentId)
    {
        try
        {
            /*********更新课程的平均时长Start************/
            var queryPlanContentIDF = (from r in db.t_learning_record
                                       where r.id == RecordID && r.delete_flag == 0
                                       select r).FirstOrDefault();

            //查找培训计划下的所有学员
            var queryPlanStu = from s in db.t_trainingplan_stu
                               where s.delete_flag == 0 && s.trainingplan_id == planid
                               select s.user_number;
            var queryPlanStuCount = queryPlanStu.Count();

            List<string> stulist = queryPlanStu.ToList();
            var queryLearnSumTime = db.t_learning_record.Where(x => x.delete_flag == 0 && x.content_id == queryPlanContentIDF.content_id && stulist.Contains(x.user_number)).Select(x => x.learning_sum_time).ToList().Sum();

            var queryPlanContent = from a in db.t_plan_course_task_exam_ref
                                   where a.delete_flag == 0 && a.id == contentId
                                   select a;
            var queryPlanContentF = queryPlanContent.FirstOrDefault();
            queryPlanContentF.avg_learningtime = (int)queryLearnSumTime / queryPlanStuCount;

            /*********更新课程的平均时长End**************/

            /*********更新课程完成率Start**************/

            var queryLearnSumFinishRate = db.t_learning_record.Where(x => x.delete_flag == 0 && x.content_id == queryPlanContentIDF.content_id && x.learning_progress != null).Select(x => double.Parse(x.learning_progress)).ToList().Sum();
            queryPlanContentF.finish_rate = (decimal)queryLearnSumFinishRate / queryPlanStuCount;

            /*********更新课程完成率End****************/
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }

    /// <summary>
    /// 更新培训计划完成度
    /// </summary>
    /// <param name="db"></param>
    /// <param name="client"></param>
    /// <param name="planid"></param>
    /// <param name="courseCount"></param>
    private void UpdateTrainingPlanFinishRate(pf_training_plan_v1Context db, IHttpClientHelper client, long planid, int courseCount)
    {
        try
        {
            //累加培训计划下所有学员的进度，并更新培训计划进度
            var querySumRecord = from c in db.t_plan_course_task_exam_ref
                                 join r in db.t_learning_record on c.id equals r.content_id
                                 where c.delete_flag == 0 && c.plan_id == planid && r.learning_progress != null && r.delete_flag == 0 && c.dif == "1"
                                 select r;
            float sumprogress = 0;
            foreach (var item in querySumRecord)
            {
                sumprogress = sumprogress + float.Parse(item.learning_progress);
            }

            //培训计划下所有学员的数量
            var queryStu = from s in db.t_trainingplan_stu
                           where s.delete_flag == 0 && s.trainingplan_id == planid
                           select s;
            int stuCount = queryStu.Count();
            decimal p = (decimal)sumprogress / (stuCount * courseCount);



            //更新培训计划进度
            var queryPlan = from pp in db.t_training_plan
                            where pp.delete_flag == 0 && pp.id == planid
                            select pp;
            var queryPlanF = queryPlan.FirstOrDefault();
            int courseFlag = 0;
            int taskFlag = 0;
            int examFlag = 0;
            if (queryPlanF.course_flag == 1)
                courseFlag = 1;//存在课程
            if (queryPlanF.task_flag == 1)
                taskFlag = 1;//存在任务
            if (queryPlanF.exam_flag == 1)
                examFlag = 1;//存在考试

            decimal d = 0;
            decimal t = 0;
            if (examFlag == 1)
            {
                //调用远程服务(考试)
                string url = "http://EXAMINATION-SERVICE/examination/v1/GetPlanExamProgress?";
                string str = client.GetRequest(url + "planId=" + planid).Result;
                ReponseResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReponseResult>(str);
                d = decimal.Parse(result.result);
            }
            if (taskFlag == 1)
            {
                //调用远程服务(任务)
            }
            int count = CacluPlanKindCount(courseFlag, taskFlag, examFlag);
            if (count == 0)
                queryPlanF.finish_rate = 0;
            else
                queryPlanF.finish_rate = (p + d + t) / count;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }
    /// <summary>
    /// 更新培训计划统计表进度
    /// </summary>
    /// <param name="db"></param>
    /// <param name="courseCount"></param>
    /// <param name="planid"></param>
    /// <param name="userNumber"></param>
    /// <param name="userId"></param>
    private void UpdateTrainingPlanStatistic(pf_training_plan_v1Context db, int courseCount, long planid, string userNumber, long userId)
    {
        try
        {
            var querySumStuRecord = from z in db.t_plan_course_task_exam_ref
                                    join r in db.t_learning_record on z.id equals r.content_id
                                    where z.delete_flag == 0 && z.plan_id == planid && r.user_number == userNumber && r.learning_progress != null && r.delete_flag == 0 && z.dif == "1"
                                    select r;
            float sumStuprogress = 0;
            foreach (var item in querySumStuRecord)
            {
                sumStuprogress = sumStuprogress + float.Parse(item.learning_progress);
            }
            var finishRate = sumStuprogress / courseCount;


            var queryStuStatistic = from s in db.t_trainingplan_stustatistic
                                    where s.user_number == userNumber && s.trainingplan_id == planid && s.delete_flag == 0
                                    select s;
            var queryStuStatisticF = queryStuStatistic.FirstOrDefault();
            if (queryStuStatisticF == null)
            {
                t_trainingplan_stustatistic stustatistic = new t_trainingplan_stustatistic();
                stustatistic.trainingplan_id = planid;
                stustatistic.user_number = userNumber;
                stustatistic.user_id = userId;
                stustatistic.course_comrate = (decimal)finishRate;
                db.t_trainingplan_stustatistic.Add(stustatistic);
            }
            else
            {
                queryStuStatisticF.course_comrate = (decimal)finishRate;
            }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }
    /// <summary>
    /// 更新节点进度
    /// </summary>
    /// <param name="db"></param>
    /// <param name="recordId"></param>
    /// <param name="nodeId"></param>
    public void UpdateNodeStatus(pf_training_plan_v1Context db, long recordId, long nodeId)
    {
        try
        {
            long thirdParentID = GetBrotherNodeSumStatus(db, recordId, nodeId);
            if (thirdParentID == 0)
                return;

            long secondParentID = GetBrotherNodeSumStatus(db, recordId, thirdParentID);
            if (secondParentID == 0)
                return;

            long firstParentID = GetBrotherNodeSumStatus(db, recordId, secondParentID);
            if (firstParentID == 0)
                return;

            long parentID = GetBrotherNodeSumStatus(db, recordId, firstParentID);
            if (parentID == 0)
                return;

            long rootNode = GetBrotherNodeSumStatus(db, recordId, parentID);
            if (rootNode == 0)
                return;

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }
    public long GetBrotherNodeSumStatus(pf_training_plan_v1Context db, long recordId, long nodeId)
    {
        try
        {
            //查找它的父节点
            var queryParentID = GetNodeParentId(db, recordId, nodeId);
            if (queryParentID == 0)
                return 0;

            //计算所有兄弟节点的平均值
            var brotherNodeAvgValue = db.t_course_node_learning_status.Where(x => x.delete_flag == 0 && x.record_id == recordId && x.course_struct_id == queryParentID).Select(x => new { nodeStatus = double.Parse(x.node_status) }).Average(x => x.nodeStatus);
            //更新父节点学习进度
            var queryParentStatus = db.t_course_node_learning_status.Where(x => x.delete_flag == 0 && x.record_id == recordId && x.node_id == queryParentID).FirstOrDefault();
            queryParentStatus.node_status = brotherNodeAvgValue.ToString("#0.0");
            db.SaveChanges();
            return queryParentID;
        }
        catch (Exception ex)
        {
            throw;
        }

    }
    private long GetNodeParentId(pf_training_plan_v1Context db, long recordId, long nodeId)
    {
        try
        {
            //查找它的父节点
            var queryParentID = (from s in db.t_course_node_learning_status
                                 where s.delete_flag == 0 && s.record_id == recordId && s.node_id == nodeId
                                 select s.course_struct_id).FirstOrDefault();
            return queryParentID;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }
    private int CacluPlanKindCount(int course, int task, int exam)
    {
        int i = 0;
        if (course == 1)
            ++i;
        if (task == 1)
            ++i;
        if (exam == 1)
            ++i;
        return i;
    }

}


public class ReponseResult
{
    public string code { get; set; }
    public string result { get; set; }
    public string dif { get; set; }
    public string msg { get; set; }
}

public class StuTrainingPlanContent
{
    public long ID { get; set; }
    public long SrcID { get; set; }
    public int? ContentSort { get; set; }
    public string ContentName { get; set; }
    public string LearnProgress { get; set; }
    public int? TimeInfo { get; set; }
    public string TeacherID { get; set; }
    public string TeacherName { get; set; }
    public List<string> Tags { get; set; }
    public List<PlanCondition> planConditions { get; set; }
}

public class PlanCondition
{
    public long ID { get; set; }
    public string ConditionName { get; set; }
    public string SelectFlag { get; set; }
    public string Dif { get; set; }
}

