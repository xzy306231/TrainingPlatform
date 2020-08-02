using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TrainingPlan
{
    #region 培训计划
    public object GetTrainingPlan(pf_training_plan_v1Context db, string strStatus, string planName, string startTime, string endTime, int pageIndex, int pageSize, TokenModel obj)
    {
        try
        {
            IQueryable<t_training_plan> query = null;
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                query = from p in db.t_training_plan
                        where p.delete_flag == 0
                        && p.start_time >= DateTime.Parse(startTime)
                        && p.end_time <= DateTime.Parse(endTime)
                        && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        && (strStatus == "0" ? true : p.plan_status == strStatus)
                        orderby p.create_time descending
                        select p;
            }
            else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                DateTime date = DateTime.Parse(startTime);
                query = from p in db.t_training_plan
                        where p.delete_flag == 0
                        && p.start_time >= DateTime.Parse(startTime)
                        && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        && (strStatus == "0" ? true : p.plan_status == strStatus)
                        orderby p.create_time descending
                        select p;
            }
            else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                query = from p in db.t_training_plan
                        where p.delete_flag == 0
                        && p.end_time <= DateTime.Parse(endTime)
                        && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        && (strStatus == "0" ? true : p.plan_status == strStatus)
                        orderby p.create_time descending
                        select p;
            }
            else
            {
                query = from p in db.t_training_plan
                        where p.delete_flag == 0
                        && (string.IsNullOrEmpty(planName) ? true : p.plan_name.Contains(planName))
                        && (strStatus == "0" ? true : p.plan_status == strStatus)
                        orderby p.create_time descending
                        select p;
            }
            int count = query.Count();
            List<TrainingPlanModel> Temp = new List<TrainingPlanModel>();
            foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
            {
                string str = "0";
                if (item.create_by == obj.userId)
                    str = "1";

                Temp.Add(new TrainingPlanModel()
                {
                    ID = item.id,
                    PlanName = item.plan_name,
                    PlanDesc = item.plan_desc,
                    StartTime = item.start_time.ToString(),
                    EndTime = item.end_time.ToString(),
                    QuitFlag = item.quit_flag,
                    PlanStatus = item.plan_status,
                    MyFlag = str,
                    PublishFlag = item.publish_flag,
                    CourseFlag = item.course_flag.ToString(),
                    TaskFlag = item.task_flag.ToString(),
                    ExamFlag = item.exam_flag.ToString(),
                    CreateName = item.create_name
                });
            }
            return new { code = 200, result = Temp, count = count, msg = "成功" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }


    }
    public object Add_TrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, TrainingPlanModel model, TokenModel token)
    {
        try
        {
            t_training_plan p = new t_training_plan();
            p.plan_name = model.PlanName;
            p.plan_desc = model.PlanDesc;
            p.start_time = DateTime.Parse(model.StartTime);
            p.end_time = DateTime.Parse(model.EndTime);
            p.plan_status = "1";//未开始
            p.quit_flag = 0;
            p.create_time = DateTime.Now;
            p.create_by = model.CreateBy;
            p.create_number = token.userNumber;
            p.create_name = token.userName;
            p.update_time = DateTime.Now;
            db.t_training_plan.Add(p);
            int i = db.SaveChanges();
            if (i > 0)
            {
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 2;
                log.moduleName = "培训计划";
                log.logDesc = "创建了一条培训计划：" + model.PlanName;
                log.logSuccessd = 1;
                rabbit.LogMsg(log);
                long MaxTagID = p.id;
                return new { code = 200, result = MaxTagID, message = "添加成功" };
            }
            else
            {
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 2;
                log.moduleName = "培训计划";
                log.logDesc = "创建了一条培训计划：" + model.PlanName;
                log.logSuccessd = 2;
                rabbit.LogMsg(log);
                return new { code = 200, result = i, message = "添加失败" };
            }

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object Update_TrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, TrainingPlanModel model, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            var query = from p in db.t_training_plan
                        where p.id == model.ID && p.delete_flag == 0
                        select p;
            var q = query.FirstOrDefault();
            q.plan_name = model.PlanName;
            q.plan_desc = model.PlanDesc;
            q.start_time = DateTime.Parse(model.StartTime);
            q.end_time = DateTime.Parse(model.EndTime);

            var queryExam = from c in db.t_plan_course_task_exam_ref
                            join t in db.t_examination_manage on c.content_id equals t.id
                            where c.plan_id == model.ID && c.dif == "3" && c.delete_flag == 0
                            select t;
            List<ExaminationID> list = new List<ExaminationID>();
            foreach (var item in queryExam)
            {
                list.Add(new ExaminationID()
                {
                    ID = (long)item.src_id,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                });
            }
            if (list.Count > 0)
            {
                string strUrl = "http://EXAMINATION-SERVICE/examination/v1/UpdateExaminationTime";
                client.PutRequest(strUrl, list);
            }
            int i = db.SaveChanges();
            if (i > 0)
            {
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 3;
                log.moduleName = "培训计划";
                log.logDesc = "修改了一条培训计划,名称变更为：" + model.PlanName;
                log.logSuccessd = 1;
                rabbit.LogMsg(log);
                // PubMethod.Log(log);
                return new { code = 200, result = i, message = "修改成功！" };
            }
            else
            {
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 3;
                log.moduleName = "培训计划";
                log.logDesc = "修改了一条培训计划,名称变更为：" + model.PlanName;
                log.logSuccessd = 2;
                rabbit.LogMsg(log);
                //PubMethod.Log(log);
                return new { code = 200, result = i, message = "修改失败" };
            }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object Update_QuitTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, long id, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            var query = from p in db.t_training_plan
                        where p.delete_flag == 0 && p.id == id
                        select p;
            var q = query.FirstOrDefault();
            q.quit_flag = 1;
            q.plan_status = "3";
            q.end_time = DateTime.Now;
            int i = db.SaveChanges();
            if (i > 0)
            {
                //调用远程服务，中止问卷
                string url = "http://QUESTIONNAIRE-SERVICE/questionnaire/v1/QuitQuestionByRemote";
                Plan plan = new Plan();
                plan.PlanID = id;
                plan.UserNumber = token.userNumber;
                plan.UserName = token.userName;
                client.PostRequest(url, plan);

                //获取考试管理ID集合
                List<long?> list = GetExamIDByPlanID(db, id);
                if (list != null && list.Count > 0)
                {
                    ExamUserModel examUser = new ExamUserModel();
                    examUser.PlanID = id;
                    examUser.ExaminationListID = list;
                    examUser.userInfos = null;
                    examUser.userNumber = token.userNumber;
                    examUser.userName = token.userName;
                    string Url = "http://EXAMINATION-SERVICE/examination/v1/QuitExamination";
                    //调用远程服务
                    client.PutRequest(Url, examUser);
                }

                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 3;
                log.moduleName = "培训计划";
                log.logDesc = "中止了一条培训计划，计划名称为：" + q.plan_name;
                log.logSuccessd = 1;
                rabbit.LogMsg(log);
                return new { code = 200, result = i, message = "中止成功" };
            }
            else
            {

                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 3;
                log.moduleName = "培训计划";
                log.logDesc = "中止了一条培训计划，计划名称为：" + q.plan_name;
                log.logSuccessd = 2;
                rabbit.LogMsg(log);
                return new { code = 200, result = i, message = "中止失败" };
            }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object Update_PublishTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, long id, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            var quertPlanContentCount = db.t_plan_course_task_exam_ref.Where(x => x.delete_flag == 0 && x.plan_id == id).Count();
            if (quertPlanContentCount == 0)
                return new { code = 400, message = "培训计划下不存在培训计划内容，不能发布！" };

            var queryStudentCount = db.t_trainingplan_stu.Where(x => x.delete_flag == 0 && x.trainingplan_id == id).Count();
            if (queryStudentCount == 0)
                return new { code = 400, message = "培训计划下不存在学员，不能发布！" };

            var query = from p in db.t_training_plan
                        where p.delete_flag == 0 && p.id == id
                        select p;
            var q = query.FirstOrDefault();
            if (q != null)
            {
                q.publish_flag = 1;
                if (db.SaveChanges() > 0)
                {
                    //查找培训计划下是否存在考试
                    var queryExam = from c in db.t_plan_course_task_exam_ref
                                    join e in db.t_examination_manage
                                    on c.content_id equals e.id
                                    where c.plan_id == id && c.dif == "3"
                                    select e;
                    List<ExaminationInfo> list = new List<ExaminationInfo>();
                    foreach (var item in queryExam)
                    {
                        list.Add(new ExaminationInfo()
                        {
                            ID = (long)item.src_id,
                            userNumber = token.userNumber,
                            userName = token.userName
                        });
                    }
                    if (list.Count > 0)//调用远程服务
                    {
                        string Url = "http://EXAMINATION-SERVICE/examination/v1/PublishExamination";
                        client.PutRequest(Url, list);
                    }

                    //日志消息产生
                    SysLogModel log = new SysLogModel();
                    log.opNo = token.userNumber;
                    log.opName = token.userName;
                    log.opType = 3;
                    log.moduleName = "培训计划";
                    log.logDesc = "发布了一条培训计划,计划名称为：" + q.plan_name;
                    log.logSuccessd = 1;
                    rabbit.LogMsg(log);
                    return new { code = 200, message = "OK" };
                }
                else
                {

                    //查找培训计划下是否存在考试
                    var queryExam = from c in db.t_plan_course_task_exam_ref
                                    join e in db.t_examination_manage
                                    on c.content_id equals e.id
                                    where c.plan_id == id && c.dif == "3"
                                    select e;
                    List<ExaminationInfo> list = new List<ExaminationInfo>();
                    foreach (var item in queryExam)
                    {
                        list.Add(new ExaminationInfo()
                        {
                            ID = (long)item.src_id,
                            userNumber = token.userNumber,
                            userName = token.userName
                        });
                    }
                    if (list.Count > 0)//调用远程服务
                    {
                        string Url = "http://EXAMINATION-SERVICE/examination/v1/PublishExamination";
                        client.PutRequest(Url, list);
                    }

                    //日志消息产生
                    SysLogModel log = new SysLogModel();
                    log.opNo = token.userNumber;
                    log.opName = token.userName;
                    log.opType = 3;
                    log.moduleName = "培训计划";
                    log.logDesc = "发布了一条培训计划,计划名称为：" + q.plan_name;
                    log.logSuccessd = 2;
                    rabbit.LogMsg(log);
                    return new { code = 200, message = "此条培训计划已发布" };
                }
            }
            else
                return new { code = 400, message = "Error" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, message = "Error" };
        }
    }
    public object Delete_TrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, long id, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            var query = from p in db.t_training_plan
                        where p.delete_flag == 0 && p.id == id
                        select p;
            var q = query.FirstOrDefault();


            if (q != null)
            {
                q.delete_flag = 1;
                var queryContent = from r in db.t_plan_course_task_exam_ref
                                   where r.delete_flag == 0 && r.plan_id == id
                                   select r;
                foreach (var item in queryContent)
                {
                    item.delete_flag = 1;
                }

                //获取考试管理ID集合
                List<long?> list = GetExamIDByPlanID(db, id);
                if (list != null && list.Count > 0)
                {
                    ExamUserModel examUser = new ExamUserModel();
                    examUser.PlanID = id;
                    examUser.ExaminationListID = list;
                    examUser.userInfos = null;
                    string Url = "http://EXAMINATION-SERVICE/examination/v1/RecoverExamination";
                    //调用远程服务
                    client.PutRequest(Url, examUser);
                }
                int i = db.SaveChanges();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = token.userNumber;
                log.opName = token.userName;
                log.opType = 4;
                log.moduleName = "培训计划";
                log.logDesc = "删除了一条培训计划,培训计划名称为：" + q.plan_name;
                log.logSuccessd = 1;
                rabbit.LogMsg(log);
                return new { code = 200, result = i, message = "OK" };
            }
            else
                return new { code = 200, message = "Error" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object PubLish_TrainingPlan(pf_training_plan_v1Context db, long PlanID)
    {
        try
        {
            var query = from p in db.t_training_plan
                        where p.delete_flag == 0 && p.id == PlanID
                        select p;
            var q = query.FirstOrDefault();
            q.publish_flag = 1;
            if (db.SaveChanges() > 0)
                return new { code = 200, message = "OK" };
            else
                return new { code = 200, message = "Error" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    #endregion

    #region 计划内容
    public object GetTrainingPlanContent(pf_training_plan_v1Context db, long PlanID)
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
                                       p.content_sort
                                   }).ToList();
            foreach (var item in queryCondition1)
            {
                planContent.Add(new PlanContent()
                {
                    ID = item.id,
                    ConditionName = item.course_name,
                    Dif = item.dif,
                    Sort = (int)item.content_sort,
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
                                       t.task_name,
                                       p.content_sort
                                   }).ToList();
            foreach (var item in queryCondition2)
            {
                planContent.Add(new PlanContent()
                {
                    ID = item.id,
                    ConditionName = item.task_name,
                    Dif = item.dif,
                    Sort = (int)item.content_sort,
                    SelectFlag = "0"
                });
            }

            var queryCondition3 = (from p in db.t_plan_course_task_exam_ref
                                   join t in db.t_examination_manage on p.content_id equals t.id
                                   where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "3"//考试
                                   orderby p.content_sort ascending
                                   select new
                                   {
                                       p.id,
                                       p.dif,
                                       t.exam_name,
                                       p.content_sort
                                   }).ToList();
            foreach (var item in queryCondition3)
            {
                planContent.Add(new PlanContent()
                {
                    ID = item.id,
                    ConditionName = item.exam_name,
                    Dif = item.dif,
                    Sort = (int)item.content_sort,
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
                            SelectFlag = planContent[k].SelectFlag,
                            Sort = planContent[k].Sort
                        });
                    }

                    //查找配置条件
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
                        ConditionList = tempList
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
                        Sort = planContent[k].Sort,
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
                                 SrcID = t.src_id,
                                 t.exam_div,
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
                        Sort = planContent[k].Sort,
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

                list.Add(new TrainingPlanContent()
                {
                    ID = item.id,//计划内容ID
                    SrcID = item.SrcID,//源ID
                    dif = 3,//任务
                    ContentSort = item.content_sort,//排序
                    ContentName = item.exam_name,//任务名称
                    TeacherID = item.teacher_num,//教员账号
                    TeacherName = item.teacher_name,//教员姓名
                    TimeInfo = (int)item.exam_duration,//课时数
                    ConditionList = tempList,
                    TaskExamDiv = item.exam_div//1：理论，2：实践
                });
            }

            //培训计划内容
            var plan = from p in db.t_training_plan
                       where p.id == PlanID && p.delete_flag == 0
                       select new { p.plan_name, p.plan_desc, p.start_time, p.end_time };
            var q = plan.FirstOrDefault();

            //人数
            var stuCount = (from s in db.t_trainingplan_stu
                            where s.trainingplan_id == PlanID && s.delete_flag == 0
                            select new { s.id }).Count();

            return new { code = 200, result1 = q, result2 = list.OrderBy(x => x.ContentSort).ToList(), result3 = stuCount, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object Update_ContentSort(pf_training_plan_v1Context db, RabbitMQClient rabbit, List<ContentSort> sortList, string planName, TokenModel token)
    {
        try
        {
            if (sortList.Count > 0)
            {
                for (int i = 0; i < sortList.Count; i++)
                {
                    //修改排序
                    var query = from s in db.t_plan_course_task_exam_ref
                                where s.id == sortList[i].ID && s.delete_flag == 0
                                select s;
                    var q = query.FirstOrDefault();
                    q.content_sort = sortList[i].PlanContentSort;

                    //删除配置条件
                    var queryConfig = from c in db.t_config_learning_condition
                                      where c.delete_flag == 0 && c.content_id == sortList[i].ID
                                      select c;
                    foreach (var item in queryConfig)
                    {
                        item.delete_flag = 1;
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = token.userNumber;
                    syslog.opName = token.userName;
                    syslog.opType = 3;
                    syslog.logDesc = "培训计划:" + planName + "，内容排序变更";
                    syslog.logSuccessd = 1;
                    rabbit.LogMsg(syslog);
                    //PubMethod.Log(syslog);
                    return new { code = 200, msg = "OK" };
                }
                else
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = token.userNumber;
                    syslog.opName = token.userName;
                    syslog.moduleName = "培训计划";
                    syslog.opType = 3;
                    syslog.logDesc = "培训计划:" + planName + "，内容排序变更";
                    syslog.logSuccessd = 2;
                    rabbit.LogMsg(syslog);
                    return new { code = 400, msg = "Error" };
                }
            }
            else
                return new { code = 400, msg = "Error" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public async Task<object> Add_CourseToTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, CourseListID model, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            t_training_plan queryPlanF = null;
            if (model.CourseList.Count > 0)
            {
                int? p = 0;

                var q = (from t in db.t_plan_course_task_exam_ref
                         where t.plan_id == model.CourseList[0].TrainingPlanID && t.delete_flag == 0
                         select t.content_sort).Max();
                if (q == null)
                    p = 0;
                else
                    p = q;

                var queryPlan = from a in db.t_training_plan
                                where a.delete_flag == 0 && a.id == model.CourseList[0].TrainingPlanID
                                select a;
                queryPlanF = queryPlan.FirstOrDefault();
                queryPlan.FirstOrDefault().course_flag = 1;//课程标识位置1

                List<PlanID> planIdList = new List<PlanID>();
                for (int i = 0; i < model.CourseList.Count; i++)
                {
                    planIdList.Add(new PlanID { planId = model.CourseList[i].CourseID });
                }
                string strUrl = "http://COURSE-SERVICE/course/v1/GetCourseInfomationByID";
                string strResult = client.PostRequestResult(strUrl, planIdList);//调用远程服务
                List<CourseInfomation> courseList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CourseInfomation>>(strResult);
                List<long> list = await AddCourseToPlan(db, rabbit, courseList, queryPlanF.plan_name, p, model.CourseList[0].TrainingPlanID, token);

                //异步处理
                AddCourseStructToPlan(rabbit,courseList, queryPlanF.plan_name, p, model.CourseList[0].TrainingPlanID, token);

                db.SaveChanges();

                var queryStuList = (from s in db.t_trainingplan_stu
                                    where s.delete_flag == 0 && s.trainingplan_id == model.CourseList[0].TrainingPlanID
                                    select s.user_number).ToList();

                var queryContent = (from c in db.t_plan_course_task_exam_ref
                                    join a in db.t_course on c.content_id equals a.id
                                    where c.delete_flag == 0 && c.plan_id == model.CourseList[0].TrainingPlanID
                                          && c.dif == "1"
                                          && list.Contains((long)c.content_id)
                                    select new { c, a.learning_time }).ToList();
                List<PlanContentID> listContent = new List<PlanContentID>();
                foreach (var item in queryContent)
                {
                    listContent.Add(new PlanContentID
                    {
                        ContentID = item.c.id,
                        CourseID = (long)item.c.content_id,
                        LearningTime = item.learning_time
                    });
                }
                //初始化学生记录数据
                InitializeStuNodeRecord(queryPlanF.id, listContent, queryStuList);
            }
            return new { code = 200, message = "OK" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    private async Task<List<long>> AddCourseToPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, List<CourseInfomation> courseList, string planName, int? p, long planId, TokenModel token)
    {
        try
        {
            List<long> list = new List<long>();
            string courseName = "";
            if (courseList != null && courseList.Count > 0)
            {
                for (int i = 0; i < courseList.Count; i++)
                {
                    t_course course = new t_course();
                    course.src_id = courseList[i].ID;
                    course.course_name = courseList[i].CourseName;
                    course.course_desc = courseList[i].CourseDesc;
                    course.course_count = courseList[i].CourseCount;
                    course.learning_time = courseList[i].LearningTime;
                    course.thumbnail_path = courseList[i].ThumbnailPath;
                    course.course_confidential = courseList[i].CourseConfidential;
                    db.t_course.Add(course);
                    db.SaveChanges();
                    list.Add(course.id);
                    courseName = courseName + "," + courseList[i].CourseName;
                    long courseId = course.id;
                    courseList[i].dbId = course.id;

                    t_plan_course_task_exam_ref obj = new t_plan_course_task_exam_ref();
                    obj.plan_id = planId;
                    obj.content_id = courseId;
                    obj.dif = "1";//课程
                    obj.content_sort = ++p;//生成排序数值
                    obj.create_by = token.userId;
                    db.t_plan_course_task_exam_ref.Add(obj);
                }
            }
            await db.SaveChangesAsync();

            courseName = courseName.TrimStart(',');
            SysLogModel syslog = new SysLogModel();
            syslog.opNo = token.userNumber;
            syslog.opName = token.userName;
            syslog.opType = 2;
            syslog.moduleName = "培训计划";
            syslog.logDesc = "培训计划:" + planName + ",添加了课程:" + courseName;
            syslog.logSuccessd = 1;
            rabbit.LogMsg(syslog);
            return list;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }
    }
    private async Task AddCourseStructToPlan( RabbitMQClient rabbit, List<CourseInfomation> courseList, string planName, int? p, long planId, TokenModel token)
    {
        try
        {
            using (var db =new pf_training_plan_v1Context())
            {
                if (courseList != null && courseList.Count > 0)
                {
                    for (int i = 0; i < courseList.Count; i++)
                    {
                        if (courseList[i].courseStructsList != null && courseList[i].courseStructsList.Count > 0)
                        {
                            for (int j = 0; j < courseList[i].courseStructsList.Count; j++)
                            {
                                t_course_struct courseStruct = new t_course_struct();
                                courseStruct.course_id = courseList[i].dbId;
                                courseStruct.struct_id = courseList[i].courseStructsList[j].StructID;
                                courseStruct.parent_id = courseList[i].courseStructsList[j].ParentID;
                                courseStruct.course_node_name = courseList[i].courseStructsList[j].CourseNodeName;
                                courseStruct.node_type = courseList[i].courseStructsList[j].NodeType;
                                courseStruct.resource_count = courseList[i].courseStructsList[j].ResourceCount;
                                courseStruct.create_time = courseList[i].courseStructsList[j].CreateTime;
                                db.t_course_struct.Add(courseStruct);
                                await db.SaveChangesAsync();
                                long structId = courseStruct.id;

                                if (courseList[i].courseStructsList[j].resourceInfomation != null)
                                {
                                    t_course_resource courseResource = new t_course_resource();
                                    courseResource.resource_name = courseList[i].courseStructsList[j].resourceInfomation.ResourceName;
                                    courseResource.resource_desc = courseList[i].courseStructsList[j].resourceInfomation.ResourceDesc;
                                    courseResource.resource_type = courseList[i].courseStructsList[j].resourceInfomation.ResourceType;
                                    courseResource.resource_extension = courseList[i].courseStructsList[j].resourceInfomation.ResourceExtension;
                                    courseResource.group_name = courseList[i].courseStructsList[j].resourceInfomation.GroupName;
                                    courseResource.resource_url = courseList[i].courseStructsList[j].resourceInfomation.ResourceUrl;
                                    courseResource.resource_confidential = courseList[i].courseStructsList[j].resourceInfomation.ResourceConfidential;
                                    db.t_course_resource.Add(courseResource);
                                    await db.SaveChangesAsync();
                                    long structResourceID = courseResource.id;

                                    t_struct_resource structResource = new t_struct_resource();
                                    structResource.course_struct_id = structId;
                                    structResource.course_resouce_id = structResourceID;
                                    db.t_struct_resource.Add(structResource);

                                    //添加自定义脚本
                                    if (courseList[i].courseStructsList[j].resourceInfomation.PageList != null && courseList[i].courseStructsList[j].resourceInfomation.PageList.Count > 0)
                                    {
                                        for (int k = 0; k < courseList[i].courseStructsList[j].resourceInfomation.PageList.Count; k++)
                                        {
                                            t_courseware_page_bus page = new t_courseware_page_bus();
                                            page.courseware_resource_id = structResourceID;
                                            page.page_script = courseList[i].courseStructsList[j].resourceInfomation.PageList[k].PageScript;
                                            page.page_sort = courseList[i].courseStructsList[j].resourceInfomation.PageList[k].Sort;
                                            db.Add(page);
                                        }
                                    }
                                }
                            }
                        }
                        if (courseList[i].knowledgeTagsList != null && courseList[i].knowledgeTagsList.Count > 0)
                        {
                            for (int k = 0; k < courseList[i].knowledgeTagsList.Count; k++)
                            {
                                var queryTag = (from t in db.t_knowledge_tag
                                                where t.delete_flag == 0 && t.src_id == courseList[i].knowledgeTagsList[k].ID
                                                select t).FirstOrDefault();
                                if (queryTag != null)//存在
                                {
                                    queryTag.tag = courseList[i].knowledgeTagsList[k].Tag;

                                    t_course_know_tag tagRef = new t_course_know_tag();
                                    tagRef.course_id = courseList[i].dbId;
                                    tagRef.tag_id = queryTag.id;
                                    db.t_course_know_tag.Add(tagRef);
                                }
                                else
                                {
                                    t_knowledge_tag knowledgeTag = new t_knowledge_tag();
                                    knowledgeTag.src_id = courseList[i].knowledgeTagsList[k].ID;
                                    knowledgeTag.tag = courseList[i].knowledgeTagsList[k].Tag;
                                    db.t_knowledge_tag.Add(knowledgeTag);
                                    await db.SaveChangesAsync();
                                    long TagID = knowledgeTag.id;

                                    t_course_know_tag tagRef = new t_course_know_tag();
                                    tagRef.course_id = courseList[i].dbId;
                                    tagRef.tag_id = TagID;
                                    db.t_course_know_tag.Add(tagRef);
                                }
                            }
                        }
                    }
                    await db.SaveChangesAsync();
                }
            }


        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }

    private void AddStuNodeStatus(long planid)
    {
        try
        {
            using (var db = new pf_training_plan_v1Context())
            {
                var queryPlanStu = from s in db.t_trainingplan_stu
                                   where s.delete_flag == 0 && s.trainingplan_id == planid
                                   select s;
                if (queryPlanStu.Count() == 0)
                    return;
                var queryCourse = from c in db.t_plan_course_task_exam_ref
                                  where c.delete_flag == 0 && c.plan_id == planid && c.dif == "1"
                                  select (long)c.content_id;
                //查找课程下的所有课程结构
                var queryCourseStruct = from s in db.t_course_struct
                                        where s.delete_flag == 0 && queryCourse.ToList().Contains(s.course_id)
                                        select s;
                foreach (var stu in queryPlanStu)
                {
                    foreach (var item in queryCourseStruct)
                    {
                        t_course_node_learning_status obj = new t_course_node_learning_status();
                        obj.plan_id = planid;

                    }
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
    public List<long?> Add_TrainingTaskToTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, TrainingTaskList trainingTask, TokenModel token)
    {
        try
        {
            t_training_plan queryPlanF = null;
            string trainName = "";
            List<long?> TaskList = new List<long?>();
            List<long> list = new List<long>();
            if (trainingTask.TrainingTasks != null && trainingTask.TrainingTasks.Count > 0)
            {
                int? p = 0;

                var q = (from t in db.t_plan_course_task_exam_ref
                         where t.plan_id == trainingTask.PlanID && t.delete_flag == 0
                         select t.content_sort).Max();
                if (q == null)
                    p = 0;
                else
                    p = q;

                var queryPlan = from a in db.t_training_plan
                                where a.delete_flag == 0 && a.id == trainingTask.PlanID
                                select a;
                queryPlanF = queryPlan.FirstOrDefault();
                queryPlan.FirstOrDefault().task_flag = 1;//任务标识位置1

                for (int i = 0; i < trainingTask.TrainingTasks.Count; i++)
                {
                    var queryTrainTask = from t in db.t_training_task
                                         where t.delete_flag == 0
                                         && t.src_id == trainingTask.TrainingTasks[i].ID
                                         select t;
                    if (queryTrainTask.FirstOrDefault() == null)
                    {
                        //添加训练任务
                        t_training_task obj = new t_training_task();
                        obj.course_count = trainingTask.TrainingTasks[i].CourseCount;
                        obj.create_number = token.userNumber;
                        obj.knowledge_tag = trainingTask.TrainingTasks[i].KnowledgeTag;
                        obj.task_desc = trainingTask.TrainingTasks[i].TaskDesc;
                        obj.src_id = trainingTask.TrainingTasks[i].ID;
                        obj.task_name = trainingTask.TrainingTasks[i].TaskName;
                        db.t_training_task.Add(obj);
                        db.SaveChanges();
                        trainName = trainName + "," + trainingTask.TrainingTasks[i].TaskName;
                        TaskList.Add(trainingTask.TrainingTasks[i].ID);
                        //long maxid = (from t in db.t_training_task select t.id).Max();
                        long maxid = obj.id;

                        //建立关系
                        t_plan_course_task_exam_ref pc = new t_plan_course_task_exam_ref();
                        pc.plan_id = trainingTask.PlanID;
                        pc.content_id = maxid;
                        pc.content_sort = ++p;
                        pc.dif = "2";//任务
                        pc.delete_flag = 0;
                        pc.create_time = DateTime.Now;
                        pc.create_by = token.userId;
                        pc.update_time = DateTime.Now;
                        db.t_plan_course_task_exam_ref.Add(pc);
                        list.Add(maxid);
                    }
                    else
                    {
                        //查找培训计划内容是否已经存在此条训练任务
                        var query = from c in db.t_plan_course_task_exam_ref
                                    where c.delete_flag == 0 && c.dif == "3" && c.content_id == queryTrainTask.FirstOrDefault().id
                                    && c.plan_id == trainingTask.PlanID
                                    select c;
                        if (query != null)//已经存在不可以添加
                            continue;

                        //建立关系
                        t_plan_course_task_exam_ref pc = new t_plan_course_task_exam_ref();
                        pc.plan_id = trainingTask.PlanID;
                        pc.content_id = queryTrainTask.FirstOrDefault().id;//任务ID
                        pc.content_sort = ++p;
                        pc.dif = "2";//任务
                        pc.delete_flag = 0;
                        pc.create_time = DateTime.Now;
                        pc.create_by = token.userId;
                        pc.update_time = DateTime.Now;
                        trainName = trainName + "," + queryTrainTask.FirstOrDefault().task_name;
                        db.t_plan_course_task_exam_ref.Add(pc);
                        TaskList.Add(queryTrainTask.FirstOrDefault().src_id);
                        list.Add(queryTrainTask.FirstOrDefault().id);
                    }
                }
            }
            db.SaveChanges();

            //日志
            trainName = trainName.TrimStart(',');
            SysLogModel syslog = new SysLogModel();
            syslog.opNo = token.userNumber;
            syslog.opName = token.userName;
            syslog.opType = 2;
            syslog.logDesc = "培训计划:" + queryPlanF.plan_name + ",添加了任务:" + trainName;
            syslog.logSuccessd = 1;
            syslog.moduleName = "培训计划";
            rabbit.LogMsg(syslog);

            var queryStuList = (from s in db.t_trainingplan_stu
                                where s.delete_flag == 0 && s.trainingplan_id == trainingTask.PlanID
                                select s.user_number).ToList();
            var queryContentList = (from c in db.t_plan_course_task_exam_ref
                                    where c.delete_flag == 0 && c.plan_id == trainingTask.PlanID && c.dif == "2" && list.Contains((long)c.content_id)
                                    select c.id).ToList();
            //初始化学生记录数据
            InitializeStuRecord(db, queryContentList, queryStuList);
            return TaskList;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new List<long?>();
        }

    }

    public object Add_ExamToTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, ExaminationList examinationList, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            t_training_plan queryPlanF = null;
            string examName = "";
            string message = string.Empty;
            List<long?> ExamList = new List<long?>();
            //考试管理ID集合
            List<long> examIdList = new List<long>();
            List<long> list = new List<long>();
            List<PlanExamination> ContentList = new List<PlanExamination>();
            if (examinationList.Examinations != null && examinationList.Examinations.Count > 0)
            {
                for (int i = 0; i < examinationList.Examinations.Count; i++)
                {
                    examIdList.Add(examinationList.Examinations[i].ID);
                }
            }
            //查找培训计划下的所有学员
            var queryPlanStuList = db.t_trainingplan_stu.Where(x => x.delete_flag == 0 && x.trainingplan_id == examinationList.PlanID).Select(x => x.user_number).ToList();

            //调用远程服务，查找考试下的阅卷老师
            string strUrl = @"http://EXAMINATION-SERVICE/examination/v1/GetExamGradeTechNum";
            string strResult = client.PostRequestResult(strUrl, examIdList);
            Dictionary<long, List<string>> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<long, List<string>>>(strResult);
            if (examinationList.Examinations != null && examinationList.Examinations.Count > 0)
            {
                int? p = 0;
                var q = (from t in db.t_plan_course_task_exam_ref
                         where t.plan_id == examinationList.PlanID && t.delete_flag == 0
                         select t.content_sort).Max();
                if (q == null)
                    p = 0;
                else
                    p = q;

                //计数
                int Count = 0;
                for (int i = 0; i < examinationList.Examinations.Count; i++)
                {
                    var tempList = dic[examinationList.Examinations[i].ID].Intersect(queryPlanStuList).ToList();//交集
                    if (tempList.Count > 0)//评分人员被添加到了学员中
                    {
                        message += examinationList.Examinations[i].ExamName + "，";
                        continue;
                    }
                    Count++;
                    var quertExam = from e in db.t_examination_manage
                                    where e.delete_flag == 0 && e.src_id == examinationList.Examinations[i].ID
                                    select e;
                    var quertExamF = quertExam.FirstOrDefault();
                    if (quertExamF == null)
                    {
                        t_examination_manage exam = new t_examination_manage();
                        exam.src_id = examinationList.Examinations[i].ID;
                        exam.exam_name = examinationList.Examinations[i].ExamName;
                        exam.exam_div = examinationList.Examinations[i].ExamDiv;
                        exam.exam_duration = examinationList.Examinations[i].Duration;
                        examName = examName + "," + examinationList.Examinations[i].ExamName;
                        db.t_examination_manage.Add(exam);
                        db.SaveChanges();
                        long maxid = exam.id;
                        ExamList.Add(examinationList.Examinations[i].ID);

                        //建立关系
                        t_plan_course_task_exam_ref pc = new t_plan_course_task_exam_ref();
                        pc.plan_id = examinationList.PlanID;
                        pc.content_id = maxid;
                        pc.content_sort = ++p;
                        pc.dif = "3";//考试
                        pc.delete_flag = 0;
                        pc.create_time = DateTime.Now;
                        pc.create_by = token.userId;
                        pc.update_time = DateTime.Now;
                        db.t_plan_course_task_exam_ref.Add(pc);
                        db.SaveChanges();
                        long ContentID = pc.id;
                        ContentList.Add(new PlanExamination()
                        {
                            ID = examinationList.Examinations[i].ID,
                            ContentID = ContentID
                        });
                        list.Add(maxid);
                    }
                    else
                    {
                        //建立关系
                        t_plan_course_task_exam_ref pc = new t_plan_course_task_exam_ref();
                        pc.plan_id = examinationList.PlanID;
                        pc.content_id = quertExamF.id;
                        pc.content_sort = ++p;
                        pc.dif = "3";//考试
                        pc.delete_flag = 0;
                        pc.create_time = DateTime.Now;
                        pc.create_by = token.userId;
                        pc.update_time = DateTime.Now;
                        examName = examName + "," + quertExamF.exam_name;
                        db.t_plan_course_task_exam_ref.Add(pc);
                        db.SaveChanges();
                        long ContentID = pc.id;
                        ExamList.Add(quertExamF.src_id);
                        ContentList.Add(new PlanExamination()
                        {
                            ID = (long)quertExamF.src_id,
                            ContentID = ContentID
                        });
                        list.Add(quertExamF.id);
                    }
                }
                //有满足条件的考试添加到了培训计划下
                if (Count > 0)
                {
                    var queryPlan = from a in db.t_training_plan
                                    where a.delete_flag == 0 && a.id == examinationList.PlanID
                                    select a;
                    queryPlan.FirstOrDefault().exam_flag = 1;//任务标识位置1
                    queryPlanF = queryPlan.FirstOrDefault();
                    var queryContentList = (from c in db.t_plan_course_task_exam_ref
                                            where c.delete_flag == 0 && c.plan_id == examinationList.PlanID && c.dif == "3" && list.Contains((long)c.content_id)
                                            select c.id).ToList();
                    //初始化学生记录数据
                    InitializeStuRecord(db, queryContentList, queryPlanStuList);
                }
            }

            if (ExamList != null && ExamList.Count > 0)
            {
                List<t_trainingplan_stu> stuList = GetStuByPlanID(db, examinationList.PlanID);
                if (stuList != null && stuList.Count > 0)
                {
                    List<ExamUserInfo> userInfoList = new List<ExamUserInfo>();
                    for (int i = 0; i < stuList.Count; i++)
                    {
                        userInfoList.Add(new ExamUserInfo()
                        {
                            UserNumber = stuList[i].user_number,
                            UserName = stuList[i].user_name,
                            Department = stuList[i].department
                        });
                    }
                    ExamUserModel model = new ExamUserModel();
                    model.PlanID = examinationList.PlanID;
                    model.ExaminationListID = ExamList;
                    model.userInfos = userInfoList;
                    model.userNumber = token.userNumber;
                    model.userName = token.userName;

                    //调用远程服务
                    Add_ExamStuToRemoteService(model, client);
                }
                //日志
                examName = examName.TrimStart(',');
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "培训计划:" + queryPlanF.plan_name + ",添加了考试:" + examName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                if (message != "")
                    message = "考试管理：“" + message.TrimEnd('，') + "”的阅卷老师存在于此培训计划下的学员，故不能添加至本培训计划下！";
                return new { code = 200, result = new { ContentList, message }, message = "OK" };
            }
            else
            {
                if (message != "")
                    message = "考试管理：“" + message.TrimEnd('，') + "”的阅卷老师存在于此培训计划下的学员，故不能添加至本培训计划下！";
                return new { code = 200, result = new { ContentList, message }, message = "OK" };
            }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public List<t_trainingplan_stu> GetStuByPlanID(pf_training_plan_v1Context db, long PlanID)
    {
        var queryStu = from s in db.t_trainingplan_stu
                       where s.trainingplan_id == PlanID && s.delete_flag == 0
                       select s;
        return queryStu.ToList();
    }

    public object Add_TeacherToTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, long ContentID, string TeacherID, string TeacherName, string planName, string contentName, TokenModel token)
    {
        try
        {
            var query = from r in db.t_plan_course_task_exam_ref
                        where r.id == ContentID && r.delete_flag == 0
                        select r;
            var q = query.FirstOrDefault();
            //根据计划号，查找已存在的教员
            var queryTeacher = from r in db.t_trainingplan_stu
                               where r.delete_flag == 0
                                     && r.trainingplan_id == q.plan_id
                                     && r.user_number == TeacherID
                               select r;
            if (queryTeacher.FirstOrDefault() != null)//已是学员，不可以设置成教员
            {
                return new { code = 401, message = "该教员在此培训计划下已是学员，不可以再设置成教员哦！" };
            }
            q.teacher_num = TeacherID;
            q.teacher_name = TeacherName;

            var queryPlan = from p in db.t_training_plan
                            where p.id == q.plan_id
                            select p;
            int i = db.SaveChanges();
            if (i > 0)
            {

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "培训计划:" + planName + "内容名：" + contentName + ",指定了教员：" + TeacherName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return new { code = 200, result = i, message = "OK" };
            }
            else
                return new { code = 400, result = i, message = "Error" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public List<string> GetTeacherFromPlan(pf_training_plan_v1Context db, long PlanID)
    {
        try
        {
            var queryTeacher = from t in db.t_plan_course_task_exam_ref
                               where t.plan_id == PlanID && t.delete_flag == 0
                               select t.teacher_num;
            var queryTeacherList = queryTeacher.ToList();
            List<string> list = new List<string>();
            foreach (var item in queryTeacherList)
            {
                if (!string.IsNullOrEmpty(item))
                    list.Add(item);
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

    public long? GetPlanIDByContentID(pf_training_plan_v1Context db, long contentID)
    {
        var query = from p in db.t_plan_course_task_exam_ref
                    where p.delete_flag == 0 && p.id == contentID
                    select p.plan_id;
        return query.FirstOrDefault();
    }
    public long? GetTaskIDByContentID(pf_training_plan_v1Context db, long contentID)
    {
        var query = from c in db.t_plan_course_task_exam_ref
                    join t in db.t_task_bus on c.content_id equals t.id
                    where c.dif == "2" && c.delete_flag == 0 && c.id == contentID
                    select t.id;
        return query.FirstOrDefault();
    }

    public long? GetExamIDByContentID(pf_training_plan_v1Context db, long contentID)
    {
        var query = from c in db.t_plan_course_task_exam_ref
                    join t in db.t_examination_manage on c.content_id equals t.id
                    where c.dif == "3" && c.delete_flag == 0 && c.id == contentID
                    select t.src_id;
        return query.FirstOrDefault();
    }

    public List<long?> GetExamIDByPlanID(pf_training_plan_v1Context db, long PlanID)
    {

        var query = from c in db.t_plan_course_task_exam_ref
                    join t in db.t_examination_manage on c.content_id equals t.id
                    where c.dif == "3" && c.delete_flag == 0 && c.plan_id == PlanID
                    select t.src_id;
        return query.ToList();
    }
    public object Delete_ContentFromTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, long id, string planName, string contentName, TokenModel token)
    {
        try
        {
            var query = from p in db.t_plan_course_task_exam_ref
                        where p.id == id
                        select p;
            var q = query.FirstOrDefault();
            q.delete_flag = 1;
            long taskid = (long)q.content_id;

            long planId = (long)q.plan_id;

            int i = db.SaveChanges();

            var queryPlan = from p in db.t_training_plan
                            where p.delete_flag == 0 && p.id == planId
                            select p;
            var queryPlanF = queryPlan.FirstOrDefault();

            var queryPlanContentList = (from p in db.t_plan_course_task_exam_ref
                                        where p.plan_id == planId && p.delete_flag == 0
                                        select p.dif).ToList();
            if (queryPlanContentList.Contains("1"))
                queryPlanF.course_flag = 1;
            else
                queryPlanF.course_flag = 0;

            if (queryPlanContentList.Contains("2"))
                queryPlanF.task_flag = 1;
            else
                queryPlanF.task_flag = 0;

            if (queryPlanContentList.Contains("3"))
                queryPlanF.exam_flag = 1;
            else
                queryPlanF.exam_flag = 0;

            var queryRecord = from r in db.t_learning_record
                              where r.delete_flag == 0 && r.content_id == id
                              select r;
            foreach (var item in queryRecord)
            {
                item.delete_flag = 1;
            }

            db.SaveChanges();

            SysLogModel syslog = new SysLogModel();
            syslog.opNo = token.userNumber;
            syslog.opName = token.userName;
            syslog.opType = 4;
            syslog.moduleName = "培训计划";
            syslog.logDesc = "培训计划:" + planName + ",其中的计划内容：" + contentName + ",被删除";
            syslog.logSuccessd = 1;
            rabbit.LogMsg(syslog);
            return new { code = 200, result = i, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    #endregion

    #region 课程学习参与条件
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
    public object Add_CourseLearningCondition(pf_training_plan_v1Context db, RabbitMQClient rabbit, ConfigLearningConditionList ModelList, TokenModel token)
    {
        try
        {
            if (ModelList.ConditionList.Count > 0)
            {
                for (int i = 0; i < ModelList.ConditionList.Count; i++)
                {
                    t_config_learning_condition obj = new t_config_learning_condition();
                    obj.condition_id = ModelList.ConditionList[i].ConditionID;
                    obj.content_id = ModelList.ContentID;
                    obj.dif = ModelList.ConditionList[i].Dif;
                    obj.create_by = ModelList.ConditionList[i].CreateBy;
                    obj.create_time = DateTime.Now;
                    obj.update_time = DateTime.Now;
                    db.t_config_learning_condition.Add(obj);
                }
            }
            if (db.SaveChanges() > 0)
            {
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "培训计划:" + ModelList.planName + ",内容:" + ModelList.contentName + "设定了学习条件";
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };
            }
            else
                return new { code = 400, message = "Error" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public object Update_CourseLearningCondition(pf_training_plan_v1Context db, RabbitMQClient rabbit, ConfigLearningConditionList ModelList, TokenModel token)
    {
        try
        {
            var query1 = from c in db.t_config_learning_condition
                         where c.content_id == ModelList.ContentID && c.delete_flag == 0
                         select c;
            var list = query1.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].delete_flag = 1;//删除之前的
            }
            if (ModelList.ConditionList.Count > 0)
            {
                for (int i = 0; i < ModelList.ConditionList.Count; i++)
                {
                    t_config_learning_condition obj = new t_config_learning_condition();
                    obj.content_id = ModelList.ContentID;
                    obj.condition_id = ModelList.ConditionList[i].ConditionID;
                    obj.dif = ModelList.ConditionList[i].Dif;
                    obj.create_by = ModelList.ConditionList[i].CreateBy;
                    obj.create_time = DateTime.Now;
                    obj.update_time = DateTime.Now;
                    db.t_config_learning_condition.Add(obj);//创建修改之后的数据
                }
            }
            if (db.SaveChanges() > 0)
            {
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "培训计划:" + ModelList.planName + ",内容:" + ModelList.contentName + "设定了学习条件";
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };
            }
            else
                return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    #endregion

    #region 培训计划学员管理

    public string GetPlanCreateNum(pf_training_plan_v1Context db, long PlanID)
    {
        try
        {
            var query = from n in db.t_training_plan
                        where n.delete_flag == 0 && n.id == PlanID
                        select n.create_number;
            return query.FirstOrDefault();
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }
    }
    public List<long> GetExaminationIDList(pf_training_plan_v1Context db, long planId)
    {
        try
        {
            var queryExamID = (from r in db.t_plan_course_task_exam_ref
                               join e in db.t_examination_manage on r.content_id equals e.id
                               where r.plan_id == planId && r.dif == "3" && r.delete_flag == 0
                               select (long)e.src_id).ToList();
            return queryExamID;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new List<long>();
        }
    }
    public List<string> GetStuToPlan(pf_training_plan_v1Context db, long PlanID)
    {
        try
        {
            var query = from s in db.t_trainingplan_stu
                        where s.trainingplan_id == PlanID && s.delete_flag == 0
                        select s.user_number;
            return query.ToList();
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }

    }

    public object Add_StuToTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, TrainingPlanStuList trainingPlanStuList, TokenModel token)
    {
        try
        {
            t_training_plan queryPlanF = null;
            string userName = "";
            if (trainingPlanStuList.trainingPlanStusList.Count > 0)
            {
                for (int i = 0; i < trainingPlanStuList.trainingPlanStusList.Count; i++)
                {
                    t_trainingplan_stu obj = new t_trainingplan_stu();
                    obj.trainingplan_id = trainingPlanStuList.trainingPlanStusList[i].TrainingPlanID;
                    obj.user_number = trainingPlanStuList.trainingPlanStusList[i].UserID;
                    obj.user_name = trainingPlanStuList.trainingPlanStusList[i].UserName;
                    obj.education = trainingPlanStuList.trainingPlanStusList[i].Education;
                    obj.department = trainingPlanStuList.trainingPlanStusList[i].Department;
                    obj.airplane = trainingPlanStuList.trainingPlanStusList[i].AirPlane;
                    obj.skill_level = trainingPlanStuList.trainingPlanStusList[i].SkillLevel;
                    obj.actual_duration = trainingPlanStuList.trainingPlanStusList[i].ActualDuration;
                    obj.fly_status = trainingPlanStuList.trainingPlanStusList[i].FlyStatus;
                    obj.create_by = trainingPlanStuList.trainingPlanStusList[i].CreateBy;
                    obj.create_time = DateTime.Now;
                    obj.update_time = DateTime.Now;
                    userName = userName + "," + trainingPlanStuList.trainingPlanStusList[i].UserName;
                    db.t_trainingplan_stu.Add(obj);
                }
                var queryPlan = from p in db.t_training_plan
                                where p.id == trainingPlanStuList.trainingPlanStusList[0].TrainingPlanID
                                select p;
                queryPlanF = queryPlan.FirstOrDefault();
            }
            if (db.SaveChanges() > 0)
            {
                userName = userName.TrimStart(',');
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "为培训计划:" + queryPlanF.plan_name + ",批量添加了学员:" + userName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };
            }
            else
                return new { code = 400, message = "添加失败" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    public bool Add_StuFromRemoteService(pf_training_plan_v1Context db, RabbitMQClient rabbit, long PlanID, List<TrainingPlanSelectDto> list, TokenModel token)
    {
        try
        {
            string userName = "";
            if (list != null && list.Count > 0)
            {
                List<string> userList = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    t_trainingplan_stu obj = new t_trainingplan_stu();
                    obj.trainingplan_id = PlanID;
                    obj.user_id = list[i].ID;
                    obj.user_number = list[i].userNumber;
                    obj.user_name = list[i].userName;
                    obj.education = list[i].educationKey;
                    obj.department = list[i].departmentKey;
                    obj.airplane = list[i].airplaneModelKey;
                    obj.skill_level = list[i].skillLevelKey;
                    obj.actual_duration = list[i].totalDuration.ToString();
                    obj.fly_status = list[i].flyStatusKey;
                    obj.delete_flag = 0;
                    obj.create_time = DateTime.Now;
                    obj.create_by = 1;
                    obj.update_time = DateTime.Now;
                    obj.photo_path = list[i].photoPath;
                    userName = userName + "," + list[i].userName;
                    db.t_trainingplan_stu.Add(obj);

                    t_trainingplan_stustatistic stu = new t_trainingplan_stustatistic();
                    stu.trainingplan_id = PlanID;
                    stu.user_id = list[i].ID;
                    stu.user_number = list[i].userNumber;
                    db.t_trainingplan_stustatistic.Add(stu);

                    userList.Add(list[i].userNumber);
                }
                var queryContentList = (from c in db.t_plan_course_task_exam_ref
                                        where c.delete_flag == 0 && c.plan_id == PlanID
                                        select c.id).ToList();

                var queryPlanInfo = (from p in db.t_training_plan
                                     where p.delete_flag == 0 && p.id == PlanID
                                     select p).FirstOrDefault();

                //初始化数据
                InitializeStuNodeRecord(PlanID, userList);
                db.SaveChanges();

                //更新培训计划中的人数
                var queryStu = from s in db.t_trainingplan_stu
                               where s.delete_flag == 0 && s.trainingplan_id == PlanID
                               select s;

                var queryPlan = from p in db.t_training_plan
                                where p.delete_flag == 0 && p.id == PlanID
                                select p;
                var queryPlanF = queryPlan.FirstOrDefault();
                queryPlanF.stu_count = queryStu.Count();
                db.SaveChanges();

                userName = userName.TrimStart(',');
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "为培训计划:" + queryPlanF.plan_name + ",添加了学员:" + userName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return true;

            }
            else
                return false;

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }
    }

    /// <summary>
    /// 获取培训计划下所有训练任务ID
    /// </summary>
    /// <param name="PlanID"></param>
    /// <returns></returns>
    public List<long?> GetPlanTask(pf_training_plan_v1Context db, long? PlanID)
    {
        try
        {
            var query = from p in db.t_plan_course_task_exam_ref
                        join t in db.t_training_task on p.content_id equals t.id
                        where p.plan_id == PlanID && p.delete_flag == 0 && p.dif == "2"
                        select t.src_id;
            return query.ToList();
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }
    }

    /// <summary>
    /// 获取培训计划下所有的考试管理ID
    /// </summary>
    /// <param name="PlanID"></param>
    /// <returns></returns>
    public List<long?> GetPlanExam(pf_training_plan_v1Context db, long? PlanID)
    {
        try
        {
            var query = from p in db.t_plan_course_task_exam_ref
                        join t in db.t_examination_manage on p.content_id equals t.id
                        where p.plan_id == PlanID && p.delete_flag == 0 && p.dif == "3"
                        select t.src_id;
            return query.ToList();
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return null;
        }
    }

    public object Delete_StuFromTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, StudentIDList studentIDList, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            string stuName = "";
            //计划ID
            long planid = 0;
            if (studentIDList.studentIDList != null && studentIDList.studentIDList.Count > 0)
            {
                //查找所属的培训计划
                var queryPlanID = from s in db.t_trainingplan_stu
                                  where s.delete_flag == 0 && s.id == studentIDList.studentIDList[0]
                                  select s;
                var queryPlanIDF = queryPlanID.FirstOrDefault();
                List<long?> TaskListID = GetPlanTask(db, queryPlanIDF.trainingplan_id);
                List<long?> ExamListID = GetPlanExam(db, queryPlanIDF.trainingplan_id);

                //培训计划下存在任务
                if (TaskListID != null && TaskListID.Count > 0)
                {
                    List<UserInfo> userInfoList = new List<UserInfo>();
                    for (int i = 0; i < studentIDList.studentIDList.Count; i++)
                    {
                        var query = from s in db.t_trainingplan_stu
                                    where s.delete_flag == 0 && s.id == studentIDList.studentIDList[i]
                                    select s;
                        var q = query.FirstOrDefault();
                        q.delete_flag = 1;
                        userInfoList.Add(new UserInfo()
                        {
                            UserId = q.user_id,
                            UserName = q.user_name,
                            Department = q.department
                        });
                    }
                    UserModel userModel = new UserModel();
                    userModel.PlanId = queryPlanIDF.trainingplan_id;
                    userModel.TaskId = TaskListID;
                    userModel.RemoveUsers = userInfoList;
                    //调用远程服务
                    Add_TaskStuToRemoteService(userModel, client);
                }
                if (ExamListID != null && ExamListID.Count > 0)
                {
                    List<ExamUserInfo> examUsers = new List<ExamUserInfo>();
                    for (int i = 0; i < studentIDList.studentIDList.Count; i++)
                    {
                        var query = from s in db.t_trainingplan_stu
                                    where s.delete_flag == 0 && s.id == studentIDList.studentIDList[i]
                                    select s;
                        var q = query.FirstOrDefault();
                        q.delete_flag = 1;
                        examUsers.Add(new ExamUserInfo()
                        {
                            UserNumber = q.user_number,
                            UserName = q.user_name,
                            Department = q.department
                        });
                    }
                    ExamUserModel examUser = new ExamUserModel();
                    examUser.PlanID = queryPlanIDF.trainingplan_id;
                    examUser.userInfos = examUsers;
                    examUser.userName = token.userName;
                    examUser.userNumber = token.userNumber;
                    //调用远程服务
                    Delete_ExamStuToRemoteService(examUser, client);
                }

                for (int i = 0; i < studentIDList.studentIDList.Count; i++)
                {
                    var query = from s in db.t_trainingplan_stu
                                where s.delete_flag == 0 && s.id == studentIDList.studentIDList[i]
                                select s;
                    var q = query.FirstOrDefault();
                    q.delete_flag = 1;
                    stuName = stuName + "," + q.user_name;
                    planid = (long)q.trainingplan_id;

                    //删除统计记录表数据
                    var queryStud = from a in db.t_trainingplan_stustatistic
                                    where a.delete_flag == 0 && a.trainingplan_id == planid && a.user_number == q.user_number
                                    select a;
                    var queryStuF = queryStud.FirstOrDefault();
                    if (queryStuF != null)
                        queryStuF.delete_flag = 1;

                    //查找培训计划的关系表ID
                    var queryContentList = (from a in db.t_plan_course_task_exam_ref
                                            where a.delete_flag == 0 && a.plan_id == planid
                                            select a.id).ToList();
                    //删除记录表数据
                    var queryRecord = from r in db.t_learning_record
                                      where r.delete_flag == 0 && r.user_number == q.user_number && queryContentList.Contains((long)r.content_id)
                                      select r;
                    foreach (var item in queryRecord)
                    {
                        item.delete_flag = 1;
                    }
                }
                db.SaveChanges();
            }

            //更新培训计划下学员的数量
            var queryStu = from s in db.t_trainingplan_stu
                           where s.delete_flag == 0 && s.trainingplan_id == planid
                           select s;

            var queryPlan = from p in db.t_training_plan
                            where p.delete_flag == 0 && p.id == planid
                            select p;
            var queryPlanF = queryPlan.FirstOrDefault();
            queryPlanF.stu_count = queryStu.Count();
            db.SaveChanges();

            stuName = stuName.TrimStart(',');
            SysLogModel syslog = new SysLogModel();
            syslog.opNo = token.userNumber;
            syslog.opName = token.userName;
            syslog.opType = 4;
            syslog.moduleName = "培训计划";
            syslog.logDesc = "培训计划：" + queryPlanF.plan_name + ",删除了学员：" + stuName;
            syslog.logSuccessd = 1;
            rabbit.LogMsg(syslog);
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }
    public object Delete_AllStuFromTrainingPlan(pf_training_plan_v1Context db, RabbitMQClient rabbit, string strEducation, string strPlane, string strSkill, string strFlySta, long PlanID, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            string stuName = "";
            //计划ID
            long planid = 0;

            var query = from s in db.t_trainingplan_stu
                        where s.delete_flag == 0 && s.trainingplan_id == PlanID
                        && (string.IsNullOrEmpty(strEducation) ? true : s.education.Contains(strEducation))
                        && (string.IsNullOrEmpty(strPlane) ? true : s.airplane.Contains(strPlane))
                        && (string.IsNullOrEmpty(strSkill) ? true : s.skill_level.Contains(strSkill))
                        && (string.IsNullOrEmpty(strFlySta) ? true : s.fly_status.Contains(strFlySta))
                        select s;
            var list = query.ToList();
            if (list.Count > 0)
            {
                //查找所属的培训计划
                var queryPlanID = from s in db.t_trainingplan_stu
                                  where s.delete_flag == 0 && s.id == list[0].id
                                  select s;
                var queryPlanIDF = queryPlanID.FirstOrDefault();
                List<long?> TaskListID = GetPlanTask(db, queryPlanIDF.trainingplan_id);
                List<long?> ExamListID = GetPlanExam(db, queryPlanIDF.trainingplan_id);
                if (TaskListID != null && TaskListID.Count > 0)//存在训练任务
                {
                    List<UserInfo> userInfoList = new List<UserInfo>();
                    foreach (var item in list)
                    {
                        var q = from s in db.t_trainingplan_stu
                                where s.delete_flag == 0 && s.id == item.id
                                select s;
                        var qq = q.FirstOrDefault();
                        qq.delete_flag = 1;
                        userInfoList.Add(new UserInfo()
                        {
                            UserId = item.user_id,
                            UserName = item.user_name,
                            Department = item.department
                        });
                    }

                    UserModel userModel = new UserModel();
                    userModel.PlanId = PlanID;
                    userModel.TaskId = TaskListID;
                    userModel.RemoveUsers = userInfoList;

                    //调用远程服务
                    Add_TaskStuToRemoteService(userModel, client);
                }
                if (ExamListID != null && ExamListID.Count > 0)//考试
                {
                    List<ExamUserInfo> examUsers = new List<ExamUserInfo>();
                    foreach (var item in list)
                    {
                        var qy = from s in db.t_trainingplan_stu
                                 where s.delete_flag == 0 && s.id == item.id
                                 select s;
                        var q = qy.FirstOrDefault();
                        q.delete_flag = 1;
                        examUsers.Add(new ExamUserInfo()
                        {
                            UserNumber = q.user_number,
                            UserName = q.user_name,
                            Department = q.department
                        });
                    }
                    ExamUserModel examUser = new ExamUserModel();
                    examUser.PlanID = queryPlanIDF.trainingplan_id;
                    examUser.userInfos = examUsers;
                    examUser.userNumber = token.userNumber;
                    examUser.userName = token.userName;
                    //调用远程服务
                    Delete_ExamStuToRemoteService(examUser, client);
                }

                //删除本地
                foreach (var item in list)
                {
                    var q = from s in db.t_trainingplan_stu
                            where s.delete_flag == 0 && s.id == item.id
                            select s;
                    var qq = q.FirstOrDefault();
                    qq.delete_flag = 1;
                    planid = (long)qq.trainingplan_id;
                    stuName = stuName + "," + qq.user_name;

                    //删除统计记录表数据
                    var queryStud = from a in db.t_trainingplan_stustatistic
                                    where a.delete_flag == 0 && a.trainingplan_id == planid && a.user_number == item.user_number
                                    select a;
                    var queryStuF = queryStud.FirstOrDefault();
                    if (queryStuF != null)
                        queryStuF.delete_flag = 1;

                    //查找培训计划的关系表ID
                    var queryContentList = (from a in db.t_plan_course_task_exam_ref
                                            where a.delete_flag == 0 && a.plan_id == planid
                                            select a.id).ToList();
                    //删除记录表数据
                    var queryRecord = from r in db.t_learning_record
                                      where r.delete_flag == 0 && r.user_number == qq.user_number && queryContentList.Contains((long)r.content_id)
                                      select r;
                    foreach (var item1 in queryRecord)
                    {
                        item1.delete_flag = 1;
                    }
                }
                db.SaveChanges();

                //更新培训计划下学员的数量
                var queryStu = from s in db.t_trainingplan_stu
                               where s.delete_flag == 0 && s.trainingplan_id == planid
                               select s;

                var queryPlan = from p in db.t_training_plan
                                where p.delete_flag == 0 && p.id == planid
                                select p;
                var queryPlanF = queryPlan.FirstOrDefault();
                queryPlanF.stu_count = queryStu.Count();
                db.SaveChanges();

                stuName = stuName.TrimStart(',');
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 4;
                syslog.moduleName = "培训计划";
                syslog.logDesc = "培训计划：" + queryPlanF.plan_name + ",删除了学员：" + stuName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return new { code = 200, msg = "OK" };

            }
            else
                return new { code = 200, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public object GetStuFromTrainingPlan(pf_training_plan_v1Context db, IConfiguration configuration, string strEducation, string strPlane, string strSkill, string strFlySta, long PlanID, DicModel dicDepartment, DicModel dicPlaneType, int PageSize = 30, int PageIndex = 1)
    {
        try
        {
            string fastDFS = configuration["FastDFSUrl"];
            var query = from s in db.t_trainingplan_stu
                        where s.delete_flag == 0 && s.trainingplan_id == PlanID
                        && (string.IsNullOrEmpty(strEducation) ? true : s.education.Contains(strEducation))
                        && (string.IsNullOrEmpty(strPlane) ? true : s.airplane.Contains(strPlane))
                        && (string.IsNullOrEmpty(strSkill) ? true : s.skill_level.Contains(strSkill))
                        && (string.IsNullOrEmpty(strFlySta) ? true : s.fly_status.Contains(strFlySta))
                        select s;
            var count = query.ToList().Count;
            var q = query.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
            List<PlanStudent> list = new List<PlanStudent>();
            foreach (var item in q)
            {
                string strDepartment = "";//部门
                string strAirplane = "";//机型
                try
                {
                    if (!string.IsNullOrEmpty(item.department))
                        strDepartment = dicDepartment.Result.Find(x => x.DicCode == item.department).CodeDsc;
                }
                catch (Exception)
                {
                    // strDepartment = "未知";
                }
                try
                {
                    if (!string.IsNullOrEmpty(item.airplane))
                        strAirplane = dicPlaneType.Result.Find(x => x.DicCode == item.airplane).CodeDsc;
                }
                catch (Exception)
                {
                    //strAirplane = "未知";
                }
                string path = "";
                if (!string.IsNullOrEmpty(item.photo_path))
                    path = fastDFS + item.photo_path;

                list.Add(new PlanStudent()
                {
                    id = item.id,
                    user_name = item.user_name,
                    department = strDepartment,
                    airplane = strAirplane,
                    skill_level = item.skill_level,
                    actual_duration = item.actual_duration,
                    fly_status = item.fly_status,
                    photo_path = path
                });
            }
            return new { code = 200, result = new { Count = count, StuInfo = list }, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    public object GetStuByTrainingPlan(pf_training_plan_v1Context db, long ID)
    {
        try
        {
            var query = from s in db.t_trainingplan_stu
                        where s.delete_flag == 0 && s.trainingplan_id == ID
                        select s.user_number;

            return new { code = 200, result = query.ToList(), msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    public List<UserInfo> GetStuInfoByPlanID(pf_training_plan_v1Context db, long? PlanID)
    {
        List<UserInfo> userInfoList = new List<UserInfo>();
        var query = from s in db.t_trainingplan_stu
                    where s.delete_flag == 0 && s.trainingplan_id == PlanID
                    select s;
        foreach (var item in query)
        {
            userInfoList.Add(new UserInfo()
            {
                UserId = item.user_id,
                UserName = item.user_name,
                Department = item.department
            });
        }
        return userInfoList;
    }

    public List<ExamUserInfo> GetStuByPlanID(pf_training_plan_v1Context db, long? PlanID)
    {
        List<ExamUserInfo> userInfoList = new List<ExamUserInfo>();
        var query = from s in db.t_trainingplan_stu
                    where s.delete_flag == 0 && s.trainingplan_id == PlanID
                    select s;
        foreach (var item in query)
        {
            userInfoList.Add(new ExamUserInfo()
            {
                UserNumber = item.user_number,
                UserName = item.user_name,
                Department = item.department
            });
        }
        return userInfoList;
    }

    /// <summary>
    /// 将学员数据写进远程任务服务
    /// </summary>
    /// <param name="userModel"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public bool Add_TaskStuToRemoteService(UserModel userModel, IHttpClientHelper client)
    {
        string uri = "http://TASKMANAGE-SERVICE/taskManage/v1/updatePersonOfTask";
        if (client.PutRequest(uri, userModel).Result)
            return true;
        else
            return false;
    }
    /// <summary>
    /// 将学员数据写进远程考试服务
    /// </summary>
    /// <param name="userModel"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public bool Add_ExamStuToRemoteService(ExamUserModel userModel, IHttpClientHelper client)
    {
        try
        {
            string uri = "http://EXAMINATION-SERVICE/examination/v1/AddStuFromRemoteService";
            if (client.PostRequest(uri, userModel).Result)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }

    }

    /// <summary>
    /// 将学员数据从远程服务中删除
    /// </summary>
    /// <param name="userModel"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public bool Delete_ExamStuToRemoteService(ExamUserModel userModel, IHttpClientHelper client)
    {
        string uri = "http://EXAMINATION-SERVICE/examination/v1/DeleteStuFromRemoteService";
        if (client.PutRequest(uri, userModel).Result)
            return true;
        else
            return false;
    }

    public bool Delete_ExamStusToRemoteService(ExamUserModel userModel, IHttpClientHelper client)
    {
        string uri = "http://EXAMINATION-SERVICE/examination/v1/DeleteStuPlanIDExamID";
        if (client.PutRequest(uri, userModel).Result)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 初始化记录表数据
    /// </summary>
    /// <param name="contentId"></param>
    /// <param name="userNumber"></param>
    private void InitializeStuRecord(pf_training_plan_v1Context db, List<long> contentId, List<string> userNumber)
    {
        try
        {
            for (int i = 0; i < contentId.Count; i++)
            {
                for (int j = 0; j < userNumber.Count; j++)
                {
                    t_learning_record record = new t_learning_record();
                    record.content_id = contentId[i];
                    record.user_number = userNumber[j];
                    db.t_learning_record.Add(record);
                }
            }
            db.SaveChanges();

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }
    private async Task InitializeStuNodeRecord(long planid, List<string> userNumber)
    {
        try
        {
            using (var db = new pf_training_plan_v1Context())
            {
                var query = (from a in db.t_plan_course_task_exam_ref
                             join c in db.t_course on a.content_id equals c.id
                             where a.delete_flag == 0 && a.dif == "1" && a.plan_id == planid
                             select new { a, c.learning_time }).ToList();
                List<PlanContentID> list = new List<PlanContentID>();
                foreach (var item in query)
                {
                    list.Add(new PlanContentID
                    {
                        ContentID = item.a.id,
                        CourseID = (long)item.a.content_id,
                        LearningTime = item.learning_time
                    });
                }
                InitializeStuNodeRecord(planid, list, userNumber);
            }

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }



    private async Task InitializeStuNodeRecord( long planid, List<PlanContentID> contentId, List<string> userNumber)
    {
        try
        {
            using (var db = new pf_training_plan_v1Context())
            {
                for (int i = 0; i < contentId.Count; i++)
                {
                    for (int j = 0; j < userNumber.Count; j++)
                    {
                        t_learning_record record = new t_learning_record();
                        record.content_id = contentId[i].ContentID;
                        record.user_number = userNumber[j];
                        db.t_learning_record.Add(record);
                        await db.SaveChangesAsync();
                        long recordid = record.id;

                        var queryStruct = from s in db.t_course_struct
                                          join t in db.t_struct_resource on s.id equals t.course_struct_id into st
                                          from _st in st.DefaultIfEmpty()
                                          join r in db.t_course_resource on _st.course_resouce_id equals r.id into tr
                                          from _tr in tr.DefaultIfEmpty()
                                          where s.delete_flag == 0 && s.course_id == contentId[i].CourseID
                                          orderby s.create_time ascending
                                          select new { s, _tr.resource_type, _tr.resource_extension };
                        foreach (var item in queryStruct)
                        {
                            t_course_node_learning_status obj = new t_course_node_learning_status();
                            obj.plan_id = planid;
                            obj.record_id = recordid;
                            obj.user_number = userNumber[j];
                            obj.course_id = contentId[i].CourseID;
                            obj.node_id = item.s.struct_id;
                            obj.course_struct_id = item.s.parent_id;
                            obj.node_type = item.s.node_type;
                            obj.node_name = item.s.course_node_name;
                            obj.resource_count = item.s.resource_count;
                            obj.resource_extension = item.resource_extension;
                            obj.sum_learning_time = 0;
                            obj.node_status = "0";
                            obj.learning_time = 0;
                            obj.attempt_number = 0;
                            obj.create_time = item.s.create_time;
                            db.t_course_node_learning_status.Add(obj);
                        }
                        await db.SaveChangesAsync();
                    }
                }
            }
             
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }
    }

    #endregion

    #region 从大纲生成
    public object Add_TrainingPlanFromProgram(pf_training_plan_v1Context db, RabbitMQClient rabbit, TrainingPlanFromProgram model, TokenModel token, IHttpClientHelper client)
    {
        try
        {
            string url = "http://COURSE-SERVICE/course/v1/TrainingPlanFromProgram?";
            string strResult = client.GetRequest(url + "programId=" + model.ProgramID).Result;
            ProgramCourseTask courseTaskList = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramCourseTask>(strResult);//反序列化

            t_training_program program = new t_training_program();
            program.src_id = model.ProgramID;
            program.range_purpose = courseTaskList.RangePurpose;
            program.standard_request = courseTaskList.StandardRequest;
            program.plane_type = courseTaskList.PlaneType;
            program.train_type = courseTaskList.TrainType;
            program.train_program_name = courseTaskList.ProgramName;
            program.endorsement_type = courseTaskList.EndorsementType;
            program.plane_type1 = courseTaskList.PlaneType1;
            program.sum_duration = courseTaskList.SumDuration;
            program.train_time = courseTaskList.TrainTime;
            program.up_down_times = courseTaskList.UpDownTimes;
            program.technical_grade = courseTaskList.TechnicalGrade;
            program.other_remark = courseTaskList.OtherRemark;
            program.createby = token.userId;
            program.updateby = token.userId;
            program.update_name = token.userName;
            program.create_time = DateTime.Now;
            program.update_time = DateTime.Now;
            db.Add(program);
            db.SaveChanges();
            long programid = program.id; //大纲ID

            t_training_plan p = new t_training_plan();
            p.plan_name = model.PlanName;
            p.plan_status = "1";
            p.plan_desc = model.PlanDesc;
            p.start_time = DateTime.Parse(model.StartTime);
            p.end_time = DateTime.Parse(model.EndTime);
            p.create_time = DateTime.Now;
            p.create_by = model.CreateBy;
            p.update_time = DateTime.Now;
            p.program_id = programid;
            db.t_training_plan.Add(p);
            db.SaveChanges();//保存至数据库
            long MaxPlanID = p.id;

            int nSort = 0;//用于排序

            if (courseTaskList.trainingTaskInfomation != null && courseTaskList.trainingTaskInfomation.Count > 0)
            {
                for (int i = 0; i < courseTaskList.trainingTaskInfomation.Count; i++)
                {
                    t_task_bus taskbus = new t_task_bus();
                    taskbus.delete_flag = 0;
                    taskbus.t_create = DateTime.Now;
                    taskbus.t_modified = DateTime.Now;
                    taskbus.original_id = courseTaskList.trainingTaskInfomation[i].ID;
                    taskbus.plan_id = MaxPlanID;
                    taskbus.task_name = courseTaskList.trainingTaskInfomation[i].TaskName;
                    taskbus.task_desc = courseTaskList.trainingTaskInfomation[i].TaskDesc;
                    taskbus.tag_display = courseTaskList.trainingTaskInfomation[i].Tag;
                    taskbus.class_hour = courseTaskList.trainingTaskInfomation[i].CourseCount;
                    taskbus.task_type_value = courseTaskList.trainingTaskInfomation[i].TaskType;
                    taskbus.type_level_value = courseTaskList.trainingTaskInfomation[i].TypeLevel;
                    taskbus.level_value = courseTaskList.trainingTaskInfomation[i].Level;
                    taskbus.airplane_type_value = courseTaskList.trainingTaskInfomation[i].AirplaneType;
                    db.Add(taskbus);
                    db.SaveChanges();
                    long MaxTaskID = taskbus.id;

                    //建立关系
                    t_plan_course_task_exam_ref obj = new t_plan_course_task_exam_ref();
                    obj.plan_id = MaxPlanID;
                    obj.content_id = MaxTaskID;
                    obj.dif = "2";//任务
                    obj.content_sort = ++nSort;//生成排序数值
                    obj.create_by = token.userId;
                    db.t_plan_course_task_exam_ref.Add(obj);

                    if (courseTaskList.trainingTaskInfomation[i].SubjectList != null && courseTaskList.trainingTaskInfomation[i].SubjectList.Count > 0)
                    {
                        for (int k = 0; k < courseTaskList.trainingTaskInfomation[i].SubjectList.Count; k++)
                        {
                            t_subject_bus subject = new t_subject_bus();
                            subject.delete_flag = 0;
                            subject.number = courseTaskList.trainingTaskInfomation[i].SubjectList[k].TrainNumber;
                            subject.name = courseTaskList.trainingTaskInfomation[i].SubjectList[k].TrainName;
                            subject.original_id = courseTaskList.trainingTaskInfomation[i].SubjectList[k].ID;
                            subject.plane_type_value = courseTaskList.trainingTaskInfomation[i].SubjectList[k].PlaneType;
                            subject.description = courseTaskList.trainingTaskInfomation[i].SubjectList[k].TrainDesc;
                            subject.classify_value = courseTaskList.trainingTaskInfomation[i].SubjectList[k].TrainKind;
                            subject.expect_result = courseTaskList.trainingTaskInfomation[i].SubjectList[k].ExpectResult;
                            subject.task_bus_id = MaxTaskID;
                            db.Add(subject);
                        }
                    }
                }
            }
            if (courseTaskList.courseInfomation != null && courseTaskList.courseInfomation.Count > 0)
            {
                for (int i = 0; i < courseTaskList.courseInfomation.Count; i++)
                {
                    t_course course = new t_course();
                    course.src_id = courseTaskList.courseInfomation[i].ID;
                    course.course_name = courseTaskList.courseInfomation[i].CourseName;
                    course.course_desc = courseTaskList.courseInfomation[i].CourseDesc;
                    course.course_count = courseTaskList.courseInfomation[i].CourseCount;
                    course.learning_time = courseTaskList.courseInfomation[i].LearningTime;
                    course.thumbnail_path = courseTaskList.courseInfomation[i].ThumbnailPath;
                    db.t_course.Add(course);
                    db.SaveChanges();
                    long courseId = course.id;

                    //大纲与课程的关系
                    t_program_course_ref pcr = new t_program_course_ref();
                    pcr.programid = programid;
                    pcr.courseid = courseId;
                    pcr.t_create = DateTime.Now;
                    pcr.t_modified = DateTime.Now;
                    db.Add(pcr);

                    t_plan_course_task_exam_ref obj = new t_plan_course_task_exam_ref();
                    obj.plan_id = MaxPlanID;
                    obj.content_id = courseId;
                    obj.dif = "1";//课程
                    obj.content_sort = ++nSort;//生成排序数值
                    obj.create_by = token.userId;
                    db.t_plan_course_task_exam_ref.Add(obj);

                    if (courseTaskList.courseInfomation[i].courseStructsList != null && courseTaskList.courseInfomation[i].courseStructsList.Count > 0)
                    {
                        for (int k = 0; k < courseTaskList.courseInfomation[i].courseStructsList.Count; k++)
                        {
                            t_course_struct courseStruct = new t_course_struct();
                            courseStruct.course_id = courseId;
                            courseStruct.struct_id = courseTaskList.courseInfomation[i].courseStructsList[k].StructID;
                            courseStruct.parent_id = courseTaskList.courseInfomation[i].courseStructsList[k].ParentID;
                            courseStruct.course_node_name = courseTaskList.courseInfomation[i].courseStructsList[k].CourseNodeName;
                            courseStruct.node_type = courseTaskList.courseInfomation[i].courseStructsList[k].NodeType;
                            courseStruct.resource_count = courseTaskList.courseInfomation[i].courseStructsList[k].ResourceCount;
                            db.t_course_struct.Add(courseStruct);
                            db.SaveChanges();
                            long structId = courseStruct.id;

                            if (courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation != null)
                            {
                                t_course_resource courseResource = new t_course_resource();
                                courseResource.resource_name = courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation.ResourceName;
                                courseResource.resource_desc = courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation.ResourceDesc;
                                courseResource.resource_type = courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation.ResourceType;
                                courseResource.resource_extension = courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation.ResourceExtension;
                                courseResource.group_name = courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation.GroupName;
                                courseResource.resource_url = courseTaskList.courseInfomation[i].courseStructsList[k].resourceInfomation.ResourceUrl;
                                db.t_course_resource.Add(courseResource);
                                db.SaveChanges();

                                long structResourceID = courseResource.id;

                                t_struct_resource structResource = new t_struct_resource();
                                structResource.course_struct_id = structId;
                                structResource.course_resouce_id = structResourceID;
                                db.t_struct_resource.Add(structResource);
                            }
                        }
                    }

                    if (courseTaskList.courseInfomation[i].knowledgeTagsList != null && courseTaskList.courseInfomation[i].knowledgeTagsList.Count > 0)
                    {
                        for (int k = 0; k < courseTaskList.courseInfomation[i].knowledgeTagsList.Count; k++)
                        {
                            var queryTag = (from t in db.t_knowledge_tag
                                            where t.delete_flag == 0 && t.src_id == courseTaskList.courseInfomation[i].knowledgeTagsList[k].ID
                                            select t).FirstOrDefault();
                            if (queryTag != null)//存在
                            {
                                queryTag.tag = courseTaskList.courseInfomation[i].knowledgeTagsList[k].Tag;
                                t_course_know_tag tagRef = new t_course_know_tag();
                                tagRef.course_id = courseId;
                                tagRef.tag_id = queryTag.id;
                                db.t_course_know_tag.Add(tagRef);
                            }
                            else
                            {
                                t_knowledge_tag knowledgeTag = new t_knowledge_tag();
                                knowledgeTag.src_id = courseTaskList.courseInfomation[i].knowledgeTagsList[k].ID;
                                knowledgeTag.tag = courseTaskList.courseInfomation[i].knowledgeTagsList[k].Tag;
                                db.t_knowledge_tag.Add(knowledgeTag);
                                db.SaveChanges();
                                long TagID = knowledgeTag.id;

                                t_course_know_tag tagRef = new t_course_know_tag();
                                tagRef.course_id = courseId;
                                tagRef.tag_id = TagID;
                                db.t_course_know_tag.Add(tagRef);
                            }
                        }
                    }
                }
            }
            db.SaveChanges();
            SysLogModel syslog = new SysLogModel();
            syslog.opNo = token.userNumber;
            syslog.opName = token.userName;
            syslog.opType = 2;
            syslog.moduleName = "培训计划";
            syslog.logDesc = "从大纲生成了培训计划:" + model.PlanName;
            syslog.logSuccessd = 1;
            rabbit.LogMsg(syslog);
            return new { code = 200, result = MaxPlanID, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    #endregion

    #region 学习统计

    public object GetLearningStatisticByCourseID(pf_training_plan_v1Context db, long PlanID, long CourseID)
    {
        try
        {
            //查询培训计划下的总人数
            int querySumCount = (from c in db.t_trainingplan_stu
                                 where c.delete_flag == 0 && c.trainingplan_id == PlanID
                                 select c.id).Count();

            //查找正在学习的人数
            var queryLearning = from l in db.t_learning_record
                                join p in db.t_plan_course_task_exam_ref on l.content_id equals p.id
                                where p.plan_id == PlanID
                                      && p.content_id == CourseID
                                      && p.dif == "1"
                                      && l.learning_progress != "100"
                                      && l.delete_flag == 0
                                select l;
            var LearningCount = queryLearning.Count();


            //查找已完成的学员人数
            var queryFinished = from p in db.t_plan_course_task_exam_ref
                                join l in db.t_learning_record on p.id equals l.content_id into pl
                                from _pl in pl.DefaultIfEmpty()
                                where p.plan_id == PlanID && _pl.learning_progress == "100" && p.content_id == CourseID && p.dif == "1" && _pl.delete_flag == 0
                                select p.id;
            var nFinishedCount = queryFinished.Count();

            CourseManagement cm = new CourseManagement();
            object courseStruct = cm.GetCourseStruct(db, CourseID);

            return new { code = 200, result = new { LearningCount = LearningCount, FinishedCount = nFinishedCount, courseStruct = courseStruct }, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    #endregion
}

public class Plan
{
    public long PlanID { get; set; }
    public string UserName { get; set; }
    public string UserNumber { get; set; }
}
public class ContentSort
{
    public long ID { get; set; }
    public int PlanContentSort { get; set; }
}
public class PlanExamination
{
    public long ID { get; set; }
    public long ContentID { get; set; }
}

public class PlanContent
{
    public long ID { get; set; }
    public string ConditionName { get; set; }
    public string SelectFlag { get; set; }
    public string TeacherName { get; set; }
    public int Sort { get; set; }
    public string Dif { get; set; }
}

public class TrainingPlanFromProgram
{
    public long ID { get; set; }
    public string PlanName { get; set; }
    public string PlanDesc { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public long CreateBy { get; set; }
    public long ProgramID { get; set; }
}

public class TrainingPlanConditionInfo
{
    public long ID { get; set; }
    public long? ContentID { get; set; }
    public string ConditionName { get; set; }
    public string Dif { get; set; }
}

/// <summary>
/// 培训计划内容
/// </summary>
public class TrainingPlanContent
{
    public long ID { get; set; }
    public long? SrcID { get; set; }
    public int dif { get; set; }
    public int? ContentSort { get; set; }
    public string ContentName { get; set; }
    public string TaskExamDiv { get; set; }
    public List<string> tags { get; set; }
    public decimal TimeInfo { get; set; }
    public string LearnProgress { get; set; }
    public string TeacherID { get; set; }
    public string TeacherName { get; set; }
    public List<PlanContent> ConditionList { get; set; }
}
public class StudentIDList
{
    public List<int> studentIDList { get; set; }
}
public class TrainingPlanStuList
{
    public List<TrainingPlanStu> trainingPlanStusList { get; set; }
}
public class TrainingPlanStu
{
    public long TrainingPlanID { get; set; }
    public string UserID { get; set; }
    public string UserName { get; set; }
    public string Education { get; set; }
    public string Department { get; set; }
    public string AirPlane { get; set; }
    public string SkillLevel { get; set; }
    public string ActualDuration { get; set; }
    public string FlyStatus { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public long CreateBy { get; set; }
}
public class ConfigLearningConditionList
{
    public long ContentID { get; set; }
    public string planName { get; set; }
    public string contentName { get; set; }
    public List<ConfigLearningCondition> ConditionList { get; set; }
}
public class ConfigLearningCondition
{

    public long ConditionID { get; set; }
    public string Dif { get; set; }
    public long CreateBy { get; set; }
}
public class CourseListID
{
    public List<CourseInfoID> CourseList { get; set; }
}
public class CourseInfoID
{
    public long TrainingPlanID { get; set; }
    public long CourseID { get; set; }
    public long CreateBy { get; set; }
}
public class TrainingTaskListID
{
    public List<TrainingTaskID> TrainingTaskList { get; set; }
}
public class TrainingTaskID
{
    public long TrainingPlanID { get; set; }
    public long TrainTaskID { get; set; }
    public long CreateBy { get; set; }
}
public class TrainingPlanModel
{
    public long ID { get; set; }
    public string PlanName { get; set; }
    public string PlanDesc { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string PlanStatus { get; set; }
    public string MyFlag { get; set; }
    public sbyte? PublishFlag { get; set; }
    public sbyte? QuitFlag { get; set; }
    public long CreateBy { get; set; }
    public string CreateName { get; set; }
    public string Div { get; set; }
    public string CourseFlag { get; set; }
    public string TaskFlag { get; set; }
    public string ExamFlag { get; set; }
    public int StuCount { get; set; }
}
public class PlanStu
{
    public long PlanID { get; set; }
    public List<string> selectList { get; set; }
    public string eductionKey { get; set; }
    public string airModelKey { get; set; }
    public string skillLevelKey { get; set; }
    public string flyStatusKey { get; set; }
    public bool selectAll { get; set; }
    public double durationStart { get; set; }
    public double durationEnd { get; set; }
}

public class TrainingTaskList
{
    public long PlanID { get; set; }
    public List<TrainingTask> TrainingTasks { get; set; }
}

public class TrainingTask
{
    public long ID { get; set; }
    public string TaskName { get; set; }
    public string TaskDesc { get; set; }
    public string KnowledgeTag { get; set; }
    public int CourseCount { get; set; }
}
public class ExaminationList
{
    public long PlanID { get; set; }
    public List<Examination> Examinations { get; set; }
}

public class Examination
{
    public long ID { get; set; }
    public string ExamName { get; set; }
    public int Duration { get; set; }
    public string ExamDiv { get; set; }
}

public class PlanStudent
{
    public long id { get; set; }
    public string user_name { get; set; }
    public string department { get; set; }
    public string airplane { get; set; }
    public string skill_level { get; set; }
    public string actual_duration { get; set; }
    public string fly_status { get; set; }
    public string photo_path { get; set; }
}
public class ExaminationInfo
{
    public long ID { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string userNumber { get; set; }
    public string userName { get; set; }
}

public class ExaminationID
{
    public long ID { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}
public class PlanID
{
    public long planId { get; set; }
}
public class PlanContentID
{
    public long ContentID { get; set; }
    public long CourseID { get; set; }
    public decimal LearningTime { get; set; }
}

