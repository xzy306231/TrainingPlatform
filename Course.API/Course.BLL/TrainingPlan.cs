using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Course.Model;

namespace Course.BLL
{
    public class TrainingPlan
    {
        #region 培训计划
        public object GetTrainingPlan(string strStatus, string planName, string startTime, string endTime, int pageIndex, int pageSize, TokenModel obj)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                            ExamFlag = item.exam_flag.ToString()
                        });
                    }
                    return new { code = 200, result = Temp, count = count, msg = "成功" };

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }


        }
        public object Add_TrainingPlan(TrainingPlanModel model, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                        PubMethod.Log(log);
                        long MaxTagID = (from t in db.t_training_plan select t.id).Max();
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
                        PubMethod.Log(log);
                        return new { code = 200, result = i, message = "添加失败" };
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
        public object Update_TrainingPlan(TrainingPlanModel model, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from p in db.t_training_plan
                                where p.id == model.ID && p.delete_flag == 0
                                select p;
                    var q = query.FirstOrDefault();
                    q.plan_name = model.PlanName;
                    q.plan_desc = model.PlanDesc;
                    q.start_time = DateTime.Parse(model.StartTime);
                    q.end_time = DateTime.Parse(model.EndTime);

                    var queryExam = from c in db.t_plan_course_task_ref
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
                        string strUrl = PubMethod.ReadConfigJsonData("UpdateExamTime");
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
                        PubMethod.Log(log);
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
                        PubMethod.Log(log);
                        return new { code = 200, result = i, message = "修改失败" };
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
        public object Update_QuitTrainingPlan(long id, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                        string url = PubMethod.ReadConfigJsonData("QuitQuestionnaire");
                        Plan plan = new Plan();
                        plan.PlanID = id;
                        client.PostRequest(url, plan);

                        //获取考试管理ID集合
                        List<long?> list = GetExamIDByPlanID(id);
                        if (list != null && list.Count > 0)
                        {
                            ExamUserModel examUser = new ExamUserModel();
                            examUser.PlanID = id;
                            examUser.ExaminationListID = list;
                            examUser.userInfos = null;
                            examUser.userNumber = token.userNumber;
                            examUser.userName = token.userName;
                            string Url = PubMethod.ReadConfigJsonData("QuitPlan");
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
                        PubMethod.Log(log);
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
                        PubMethod.Log(log);
                        return new { code = 200, result = i, message = "中止失败" };
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
        public object Update_PublishTrainingPlan(long id, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
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
                            var queryExam = from c in db.t_plan_course_task_ref
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
                                string Url = PubMethod.ReadConfigJsonData("PublishExam");
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
                            PubMethod.Log(log);
                            return new { code = 200, msg = "OK" };
                        }
                        else
                        {

                            //查找培训计划下是否存在考试
                            var queryExam = from c in db.t_plan_course_task_ref
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
                                string Url = PubMethod.ReadConfigJsonData("PublishExam");
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
                            PubMethod.Log(log);
                            return new { code = 401, msg = "此条培训计划已发布" };
                        }
                    }
                    else
                    {
                        return new { code = 400, msg = "Error" };
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
        public object Delete_TrainingPlan(long id, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from p in db.t_training_plan
                                where p.delete_flag == 0 && p.id == id
                                select p;
                    var q = query.FirstOrDefault();
                    if (q != null)
                    {
                        q.delete_flag = 1;
                        int i = db.SaveChanges();
                        if (i > 0)
                        {
                            //获取考试管理ID集合
                            List<long?> list = GetExamIDByPlanID(id);
                            if (list != null && list.Count > 0)
                            {
                                ExamUserModel examUser = new ExamUserModel();
                                examUser.PlanID = id;
                                examUser.ExaminationListID = list;
                                examUser.userInfos = null;
                                string Url = PubMethod.ReadConfigJsonData("RecoverExamination");
                                //调用远程服务
                                client.PutRequest(Url, examUser);
                            }

                            //日志消息产生
                            SysLogModel log = new SysLogModel();
                            log.opNo = token.userNumber;
                            log.opName = token.userName;
                            log.opType = 4;
                            log.moduleName = "培训计划";
                            log.logDesc = "删除了一条培训计划,培训计划名称为：" + q.plan_name;
                            log.logSuccessd = 1;
                            PubMethod.Log(log);
                            return new { code = 200, result = i, message = "OK" };
                        }
                        else
                            return new { code = 200, result = i, message = "Error" };
                    }
                    else
                    {
                        return new { code = 200, message = "Error" };
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
        public object PubLish_TrainingPlan(long PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from p in db.t_training_plan
                                where p.delete_flag == 0 && p.id == PlanID
                                select p;
                    var q = query.FirstOrDefault();
                    q.publish_flag = 1;
                    if (db.SaveChanges() > 0)
                    {
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        return new { code = 200, message = "Error" };
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

        #endregion

        #region 计划内容
        public object GetTrainingPlanContent(long PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    List<TrainingPlanContent> list = new List<TrainingPlanContent>();

                    //配置条件
                    List<PlanContent> planContent = new List<PlanContent>();

                    var queryCondition1 = from p in db.t_plan_course_task_ref
                                          join c in db.t_course on p.content_id equals c.id
                                          where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "1"//课程
                                          orderby p.content_sort ascending
                                          select new
                                          {
                                              p.id,
                                              p.dif,
                                              c.course_name,
                                              p.content_sort
                                          };
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

                    var queryCondition2 = from p in db.t_plan_course_task_ref
                                          join t in db.t_training_task on p.content_id equals t.id
                                          where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "2"//训练任务
                                          orderby p.content_sort ascending
                                          select new
                                          {
                                              p.id,
                                              p.dif,
                                              t.task_name,
                                              p.content_sort
                                          };
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

                    var queryCondition3 = from p in db.t_plan_course_task_ref
                                          join t in db.t_examination_manage on p.content_id equals t.id
                                          where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "3"//训练任务
                                          orderby p.content_sort ascending
                                          select new
                                          {
                                              p.id,
                                              p.dif,
                                              t.exam_name,
                                              p.content_sort
                                          };
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


                    var queryCourse = from p in db.t_plan_course_task_ref
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
                                          c.course_count
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
                                TimeInfo = queryCourseList[i].course_count,
                                ConditionList = tempList
                            });
                        }
                    }

                    var queryTask = from p in db.t_plan_course_task_ref
                                    join t in db.t_training_task on p.content_id equals t.id
                                    where p.delete_flag == 0 && p.plan_id == PlanID && p.dif == "2"//任务
                                    orderby p.content_sort ascending
                                    select new
                                    {
                                        p.id,
                                        p.content_sort,
                                        p.dif,
                                        p.teacher_num,
                                        p.teacher_name,
                                        SrcID = t.src_id,
                                        t.knowledge_tag,//知识点
                                        t.task_name,//训练名称
                                        t.course_count//课时数
                                    };
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
                    var queryExam = from p in db.t_plan_course_task_ref
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
                                    };
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
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object GetApprovalCourse(QueryCriteria queryCriteria)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    if (queryCriteria.IsAsc && !string.IsNullOrEmpty(queryCriteria.FieldName))
                    {
                        var query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 && c.approval_status == "3" && !(from k in db.t_plan_course_task_ref
                                                                                              where k.plan_id == queryCriteria.PlanID
                                                                                              && k.delete_flag == 0
                                                                                              && k.dif == "1"
                                                                                              select k.content_id).Contains(c.id)
                                          && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName)
                                    select c;
                        var count = query.Distinct().ToList().Count;
                        List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                        if (q.Count > 0)
                        {
                            List<Course_TagList> list = new List<Course_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_course_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.course_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<Tag> tags = new List<Tag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Course_TagList
                                {
                                    ID = q[i].id,
                                    CourseName = q[i].course_name,
                                    CourseDesc = q[i].course_desc,
                                    CourseCount = q[i].course_count,
                                    ImagePath = q[i].thumbnail_path,
                                    CreateTime = q[i].create_time,
                                    CreateName = q[i].user_name,
                                    ApprovalStatus = q[i].approval_status,
                                    ApprovalUserNumber = q[i].approval_user_number,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                        }
                        else
                            return new { code = 200, message = "OK" };
                    }
                    else if (queryCriteria.IsAsc == false && !string.IsNullOrEmpty(queryCriteria.FieldName))
                    {
                        var query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 && c.approval_status == "3" && !(from k in db.t_plan_course_task_ref
                                                                                              where k.plan_id == queryCriteria.PlanID
                                                                                              && k.delete_flag == 0
                                                                                              && k.dif == "1"
                                                                                              select k.content_id).Contains(c.id)
                                          && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName) descending
                                    select c;
                        var count = query.Distinct().ToList().Count;
                        List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                        if (q.Count > 0)
                        {
                            List<Course_TagList> list = new List<Course_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_course_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.course_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<Tag> tags = new List<Tag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Course_TagList
                                {
                                    ID = q[i].id,
                                    CourseName = q[i].course_name,
                                    CourseDesc = q[i].course_desc,
                                    ImagePath = q[i].thumbnail_path,
                                    CourseCount = q[i].course_count,
                                    CreateTime = q[i].create_time,
                                    CreateName = q[i].user_name,
                                    ApprovalStatus = q[i].approval_status,
                                    ApprovalUserNumber = q[i].approval_user_number,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                        }
                        else
                            return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        var query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 && c.approval_status == "3" && !(from k in db.t_plan_course_task_ref
                                                                                              where k.plan_id == queryCriteria.PlanID
                                                                                              && k.delete_flag == 0
                                                                                              && k.dif == "1"
                                                                                              select k.content_id).Contains(c.id)
                                          && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby c.create_time descending
                                    select c;
                        var count = query.Distinct().ToList().Count;
                        List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                        if (q.Count > 0)
                        {
                            List<Course_TagList> list = new List<Course_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_course_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.course_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<Tag> tags = new List<Tag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Course_TagList
                                {
                                    ID = q[i].id,
                                    CourseName = q[i].course_name,
                                    CourseDesc = q[i].course_desc,
                                    ImagePath = q[i].thumbnail_path,
                                    CourseCount = q[i].course_count,
                                    CreateTime = q[i].create_time,
                                    CreateName = q[i].user_name,
                                    ApprovalStatus = q[i].approval_status,
                                    ApprovalUserNumber = q[i].approval_user_number,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                        }
                        else
                            return new { code = 200, message = "OK" };
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
        public object Update_ContentSort(List<ContentSort> sortList, string planName, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    if (sortList.Count > 0)
                    {
                        for (int i = 0; i < sortList.Count; i++)
                        {
                            //修改排序
                            var query = from s in db.t_plan_course_task_ref
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
                            PubMethod.Log(syslog);
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
                            PubMethod.Log(syslog);
                            return new { code = 400, msg = "Error" };
                        }
                    }
                    else
                        return new { code = 400, msg = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object Add_CourseToTrainingPlan(CourseListID model, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    int? j = 0;
                    t_training_plan queryPlanF = null;
                    string courseName = "";
                    if (model.CourseList.Count > 0)
                    {
                        int? p = 0;

                        var q = (from t in db.t_plan_course_task_ref
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
                        List<long> list = new List<long>();
                        for (int i = 0; i < model.CourseList.Count; i++)
                        {
                            var queryCourse = from c in db.t_course
                                              where c.delete_flag == 0 && c.id == model.CourseList[i].CourseID
                                              select c;
                            var queryCourseF = queryCourse.FirstOrDefault();
                            courseName = courseName + "," + queryCourseF.course_name;

                            t_plan_course_task_ref obj = new t_plan_course_task_ref();
                            obj.plan_id = model.CourseList[i].TrainingPlanID;
                            obj.content_id = model.CourseList[i].CourseID;
                            obj.dif = "1";//课程
                            obj.content_sort = ++p;//生成排序数值
                            obj.create_time = DateTime.Now;
                            obj.update_time = DateTime.Now;
                            obj.create_by = model.CourseList[i].CreateBy;
                            db.t_plan_course_task_ref.Add(obj);
                            list.Add(model.CourseList[i].CourseID);
                        }
                        j = db.SaveChanges();

                        var queryStuList = (from s in db.t_trainingplan_stu
                                            where s.delete_flag == 0 && s.trainingplan_id == model.CourseList[0].TrainingPlanID
                                            select s.uesr_number).ToList();

                        var queryContentList = (from c in db.t_plan_course_task_ref
                                                where c.delete_flag == 0 && c.plan_id == model.CourseList[0].TrainingPlanID && c.dif == "1" && list.Contains((long)c.content_id)
                                                select c.id).ToList();
                        //初始化学生记录数据
                        InitializeStuRecord(queryContentList, queryStuList);
                    }
                    if (j > 0)
                    {
                        courseName = courseName.TrimStart(',');
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "培训计划";
                        syslog.logDesc = "培训计划:" + queryPlanF.plan_name + ",添加了课程:" + courseName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, result = j, message = "OK" };
                    }
                    else
                        return new { code = 400, result = j, message = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public List<long?> Add_TrainingTaskToTrainingPlan(TrainingTaskList trainingTask, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    t_training_plan queryPlanF = null;
                    string trainName = "";
                    List<long?> TaskList = new List<long?>();
                    List<long> list = new List<long>();
                    if (trainingTask.TrainingTasks != null && trainingTask.TrainingTasks.Count > 0)
                    {
                        int? p = 0;

                        var q = (from t in db.t_plan_course_task_ref
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
                                long maxid = (from t in db.t_training_task select t.id).Max();

                                //建立关系
                                t_plan_course_task_ref pc = new t_plan_course_task_ref();
                                pc.plan_id = trainingTask.PlanID;
                                pc.content_id = maxid;
                                pc.content_sort = ++p;
                                pc.dif = "2";//任务
                                pc.delete_flag = 0;
                                pc.create_time = DateTime.Now;
                                pc.create_by = token.userId;
                                pc.update_time = DateTime.Now;
                                db.t_plan_course_task_ref.Add(pc);
                                list.Add(maxid);
                            }
                            else
                            {
                                //查找培训计划内容是否已经存在此条训练任务
                                var query = from c in db.t_plan_course_task_ref
                                            where c.delete_flag == 0 && c.dif == "3" && c.content_id == queryTrainTask.FirstOrDefault().id
                                            && c.plan_id == trainingTask.PlanID
                                            select c;
                                if (query != null)//已经存在不可以添加
                                    continue;

                                //建立关系
                                t_plan_course_task_ref pc = new t_plan_course_task_ref();
                                pc.plan_id = trainingTask.PlanID;
                                pc.content_id = queryTrainTask.FirstOrDefault().id;//任务ID
                                pc.content_sort = ++p;
                                pc.dif = "2";//任务
                                pc.delete_flag = 0;
                                pc.create_time = DateTime.Now;
                                pc.create_by = token.userId;
                                pc.update_time = DateTime.Now;
                                trainName = trainName + "," + queryTrainTask.FirstOrDefault().task_name;
                                db.t_plan_course_task_ref.Add(pc);
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
                    PubMethod.Log(syslog);

                    var queryStuList = (from s in db.t_trainingplan_stu
                                        where s.delete_flag == 0 && s.trainingplan_id == trainingTask.PlanID
                                        select s.uesr_number).ToList();
                    var queryContentList = (from c in db.t_plan_course_task_ref
                                            where c.delete_flag == 0 && c.plan_id == trainingTask.PlanID && c.dif == "2" && list.Contains((long)c.content_id)
                                            select c.id).ToList();
                    //初始化学生记录数据
                    InitializeStuRecord(queryContentList, queryStuList);
                    return TaskList;
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new List<long?>();
            }

        }

        public object Add_ExamToTrainingPlan(ExaminationList examinationList, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    t_training_plan queryPlanF = null;
                    string examName = "";
                    List<long?> ExamList = new List<long?>();
                    List<long> list = new List<long>();
                    List<PlanExamination> ContentList = new List<PlanExamination>();
                    if (examinationList.Examinations != null && examinationList.Examinations.Count > 0)
                    {
                        int? p = 0;

                        var q = (from t in db.t_plan_course_task_ref
                                 where t.plan_id == examinationList.PlanID && t.delete_flag == 0
                                 select t.content_sort).Max();
                        if (q == null)
                            p = 0;
                        else
                            p = q;

                        var queryPlan = from a in db.t_training_plan
                                        where a.delete_flag == 0 && a.id == examinationList.PlanID
                                        select a;
                        queryPlan.FirstOrDefault().exam_flag = 1;//任务标识位置1
                        queryPlanF = queryPlan.FirstOrDefault();

                        for (int i = 0; i < examinationList.Examinations.Count; i++)
                        {
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
                                long maxid = (from t in db.t_examination_manage select t.id).Max();
                                ExamList.Add(examinationList.Examinations[i].ID);

                                //建立关系
                                t_plan_course_task_ref pc = new t_plan_course_task_ref();
                                pc.plan_id = examinationList.PlanID;
                                pc.content_id = maxid;
                                pc.content_sort = ++p;
                                pc.dif = "3";//考试
                                pc.delete_flag = 0;
                                pc.create_time = DateTime.Now;
                                pc.create_by = token.userId;
                                pc.update_time = DateTime.Now;
                                db.t_plan_course_task_ref.Add(pc);
                                db.SaveChanges();
                                long ContentID = (from t in db.t_plan_course_task_ref select t.id).Max();
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
                                t_plan_course_task_ref pc = new t_plan_course_task_ref();
                                pc.plan_id = examinationList.PlanID;
                                pc.content_id = quertExamF.id;
                                pc.content_sort = ++p;
                                pc.dif = "3";//考试
                                pc.delete_flag = 0;
                                pc.create_time = DateTime.Now;
                                pc.create_by = token.userId;
                                pc.update_time = DateTime.Now;
                                examName = examName + "," + quertExamF.exam_name;
                                db.t_plan_course_task_ref.Add(pc);
                                db.SaveChanges();
                                long ContentID = (from t in db.t_plan_course_task_ref select t.id).Max();
                                ExamList.Add(quertExamF.src_id);
                                ContentList.Add(new PlanExamination()
                                {
                                    ID = (long)quertExamF.src_id,
                                    ContentID = ContentID
                                });
                                list.Add(quertExamF.id);
                            }
                        }

                        var queryStuList = (from s in db.t_trainingplan_stu
                                            where s.delete_flag == 0 && s.trainingplan_id == examinationList.PlanID
                                            select s.uesr_number).ToList();
                        var queryContentList = (from c in db.t_plan_course_task_ref
                                                where c.delete_flag == 0 && c.plan_id == examinationList.PlanID && c.dif == "3" && list.Contains((long)c.content_id)
                                                select c.id).ToList();
                        //初始化学生记录数据
                        InitializeStuRecord(queryContentList, queryStuList);
                    }

                    if (ExamList != null && ExamList.Count > 0)
                    {
                        List<t_trainingplan_stu> stuList = GetStuByPlanID(examinationList.PlanID);
                        if (stuList != null && stuList.Count > 0)
                        {
                            List<ExamUserInfo> userInfoList = new List<ExamUserInfo>();
                            for (int i = 0; i < stuList.Count; i++)
                            {
                                userInfoList.Add(new ExamUserInfo()
                                {
                                    UserNumber = stuList[i].uesr_number,
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
                            new TrainingPlan().Add_ExamStuToRemoteService(model, client);
                        }
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
                    PubMethod.Log(syslog);
                    return new { code = 200, result = ContentList, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public List<t_trainingplan_stu> GetStuByPlanID(long PlanID)
        {
            using (var db = new pf_course_manageContext())
            {
                var queryStu = from s in db.t_trainingplan_stu
                               where s.trainingplan_id == PlanID && s.delete_flag == 0
                               select s;
                return queryStu.ToList();
            }
        }

        public object Add_TeacherToTrainingPlan(long ContentID, string TeacherID, string TeacherName, string planName, string contentName, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from r in db.t_plan_course_task_ref
                                where r.id == ContentID && r.delete_flag == 0
                                select r;
                    var q = query.FirstOrDefault();
                    //根据计划号，查找已存在的教员
                    var queryTeacher = from r in db.t_trainingplan_stu
                                       where r.delete_flag == 0
                                             && r.trainingplan_id == q.plan_id
                                             && r.uesr_number == TeacherID
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
                        PubMethod.Log(syslog);
                        return new { code = 200, result = i, message = "OK" };
                    }
                    else
                        return new { code = 400, result = i, message = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public List<string> GetTeacherFromPlan(long PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryTeacher = from t in db.t_plan_course_task_ref
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
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }

        public long? GetPlanIDByContentID(long contentID)
        {
            using (var db = new pf_course_manageContext())
            {
                var query = from p in db.t_plan_course_task_ref
                            where p.delete_flag == 0 && p.id == contentID
                            select p.plan_id;
                return query.FirstOrDefault();
            }
        }
        public long? GetTaskIDByContentID(long contentID)
        {
            using (var db = new pf_course_manageContext())
            {
                var query = from c in db.t_plan_course_task_ref
                            join t in db.t_training_task on c.content_id equals t.id
                            where c.dif == "2" && c.delete_flag == 0 && c.id == contentID
                            select t.src_id;
                return query.FirstOrDefault();
            }
        }

        public long? GetExamIDByContentID(long contentID)
        {
            using (var db = new pf_course_manageContext())
            {
                var query = from c in db.t_plan_course_task_ref
                            join t in db.t_examination_manage on c.content_id equals t.id
                            where c.dif == "3" && c.delete_flag == 0 && c.id == contentID
                            select t.src_id;
                return query.FirstOrDefault();
            }
        }

        public List<long?> GetExamIDByPlanID(long PlanID)
        {
            using (var db = new pf_course_manageContext())
            {
                var query = from c in db.t_plan_course_task_ref
                            join t in db.t_examination_manage on c.content_id equals t.id
                            where c.dif == "3" && c.delete_flag == 0 && c.plan_id == PlanID
                            select t.src_id;
                return query.ToList();
            }
        }
        public object Delete_ContentFromTrainingPlan(long id, string planName, string contentName, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from p in db.t_plan_course_task_ref
                                where p.id == id
                                select p;
                    var q = query.FirstOrDefault();
                    q.delete_flag = 1;

                    long planId = (long)q.plan_id;

                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        var queryPlan = from p in db.t_training_plan
                                        where p.delete_flag == 0 && p.id == planId
                                        select p;
                        var queryPlanF = queryPlan.FirstOrDefault();

                        var queryPlanContentList = (from p in db.t_plan_course_task_ref
                                                    where p.plan_id == planId && p.delete_flag == 0
                                                    select p.dif).ToList();
                        if (queryPlanContentList.Contains("1"))
                            queryPlanF.course_flag = 1;
                        if (queryPlanContentList.Contains("2"))
                            queryPlanF.task_flag = 1;
                        if (queryPlanContentList.Contains("3"))
                            queryPlanF.task_flag = 1;

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
                        PubMethod.Log(syslog);
                        return new { code = 200, result = i, message = "OK" };
                    }
                    else
                        return new { code = 400, result = i, message = "删除失败" };
                }
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
        private List<TrainingPlanConditionInfo> GetCourseLearningCondition(long PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    List<TrainingPlanConditionInfo> list = new List<TrainingPlanConditionInfo>();
                    var queryCourse = from p in db.t_plan_course_task_ref
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
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }

        }
        public object Add_CourseLearningCondition(ConfigLearningConditionList ModelList, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                        return new { code = 400, message = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object Update_CourseLearningCondition(ConfigLearningConditionList ModelList, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
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

        public string GetPlanCreateNum(long PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from n in db.t_training_plan
                                where n.delete_flag == 0 && n.id == PlanID
                                select n.create_number;
                    return query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }
        public List<string> GetStuToPlan(long PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from s in db.t_trainingplan_stu
                                where s.trainingplan_id == PlanID && s.delete_flag == 0
                                select s.uesr_number;
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }

        }

        public object Add_StuToTrainingPlan(TrainingPlanStuList trainingPlanStuList, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    t_training_plan queryPlanF = null;
                    string userName = "";
                    if (trainingPlanStuList.trainingPlanStusList.Count > 0)
                    {
                        for (int i = 0; i < trainingPlanStuList.trainingPlanStusList.Count; i++)
                        {
                            t_trainingplan_stu obj = new t_trainingplan_stu();
                            obj.trainingplan_id = trainingPlanStuList.trainingPlanStusList[i].TrainingPlanID;
                            obj.uesr_number = trainingPlanStuList.trainingPlanStusList[i].UserID;
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
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                        return new { code = 400, message = "添加失败" };

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public bool Add_StuFromRemoteService(long PlanID, List<TrainingPlanSelectDto> list, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                            obj.uesr_number = list[i].userNumber;
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
                        var queryContentList = (from c in db.t_plan_course_task_ref
                                                where c.delete_flag == 0 && c.plan_id == PlanID
                                                select c.id).ToList();
                        //初始化数据
                        InitializeStuRecord(queryContentList, userList);

                        if (db.SaveChanges() > 0)
                        {
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
                            PubMethod.Log(syslog);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
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
        public List<long?> GetPlanTask(long? PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from p in db.t_plan_course_task_ref
                                join t in db.t_training_task on p.content_id equals t.id
                                where p.plan_id == PlanID && p.delete_flag == 0 && p.dif == "2"
                                select t.src_id;
                    return query.ToList();
                }
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
        public List<long?> GetPlanExam(long? PlanID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from p in db.t_plan_course_task_ref
                                join t in db.t_examination_manage on p.content_id equals t.id
                                where p.plan_id == PlanID && p.delete_flag == 0 && p.dif == "3"
                                select t.src_id;
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }

        public object Delete_StuFromTrainingPlan(StudentIDList studentIDList, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                        List<long?> TaskListID = GetPlanTask(queryPlanIDF.trainingplan_id);
                        List<long?> ExamListID = GetPlanExam(queryPlanIDF.trainingplan_id);

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
                                    UserNumber = q.uesr_number,
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
                                            where a.delete_flag == 0 && a.trainingplan_id == planid && a.user_number == q.uesr_number
                                            select a;
                            var queryStuF = queryStud.FirstOrDefault();
                            if (queryStuF != null)
                                queryStuF.delete_flag = 1;

                            //查找培训计划的关系表ID
                            var queryContentList = (from a in db.t_plan_course_task_ref
                                                    where a.delete_flag == 0 && a.plan_id == planid
                                                    select a.id).ToList();
                            //删除记录表数据
                            var queryRecord = from r in db.t_learning_record
                                              where r.delete_flag == 0 && r.user_id == q.uesr_number && queryContentList.Contains((long)r.content_id)
                                              select r;
                            foreach (var item in queryRecord)
                            {
                                item.delete_flag = 1;
                            }
                            db.SaveChanges();
                        }
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
                    PubMethod.Log(syslog);
                    return new { code = 200, message = "OK" };

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object Delete_AllStuFromTrainingPlan(string strEducation, string strPlane, string strSkill, string strFlySta, long PlanID, TokenModel token, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
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
                        List<long?> TaskListID = GetPlanTask(queryPlanIDF.trainingplan_id);
                        List<long?> ExamListID = GetPlanExam(queryPlanIDF.trainingplan_id);
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
                                    UserNumber = q.uesr_number,
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
                                            where a.delete_flag == 0 && a.trainingplan_id == planid && a.user_number == item.uesr_number
                                            select a;
                            var queryStuF = queryStud.FirstOrDefault();
                            if (queryStuF != null)
                                queryStuF.delete_flag = 1;

                            //查找培训计划的关系表ID
                            var queryContentList = (from a in db.t_plan_course_task_ref
                                                    where a.delete_flag == 0 && a.plan_id == planid
                                                    select a.id).ToList();
                            //删除记录表数据
                            var queryRecord = from r in db.t_learning_record
                                              where r.delete_flag == 0 && r.user_id == qq.uesr_number && queryContentList.Contains((long)r.content_id)
                                              select r;
                            foreach (var item1 in queryRecord)
                            {
                                item1.delete_flag = 1;
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
                        PubMethod.Log(syslog);
                        return new { code = 200, msg = "OK" };

                    }
                    else
                    {
                        return new { code = 400, msg = "Error" };
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
        public object GetStuFromTrainingPlan(string strEducation, string strPlane, string strSkill, string strFlySta, long PlanID, DicModel dicDepartment, DicModel dicPlaneType, int PageSize = 30, int PageIndex = 1)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    string fastDFS = PubMethod.ReadConfigJsonData("FastDFSUrl");
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
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object GetStuByTrainingPlan(long ID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from s in db.t_trainingplan_stu
                                where s.delete_flag == 0 && s.trainingplan_id == ID
                                select s.uesr_number;

                    return new { code = 200, result = query.ToList(), msg = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public List<UserInfo> GetStuInfoByPlanID(long? PlanID)
        {
            using (var db = new pf_course_manageContext())
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
        }

        public List<ExamUserInfo> GetStuByPlanID(long? PlanID)
        {
            using (var db = new pf_course_manageContext())
            {
                List<ExamUserInfo> userInfoList = new List<ExamUserInfo>();
                var query = from s in db.t_trainingplan_stu
                            where s.delete_flag == 0 && s.trainingplan_id == PlanID
                            select s;
                foreach (var item in query)
                {
                    userInfoList.Add(new ExamUserInfo()
                    {
                        UserNumber = item.uesr_number,
                        UserName = item.user_name,
                        Department = item.department
                    });
                }
                return userInfoList;
            }
        }

        /// <summary>
        /// 将学员数据写进远程任务服务
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool Add_TaskStuToRemoteService(UserModel userModel, IHttpClientHelper client)
        {
            string uri = PubMethod.ReadConfigJsonData("TrainTaskUsers");
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
            string uri = PubMethod.ReadConfigJsonData("AddExamUsers");
            if (client.PostRequest(uri, userModel).Result)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 将学员数据从远程服务中删除
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool Delete_ExamStuToRemoteService(ExamUserModel userModel, IHttpClientHelper client)
        {
            string uri = PubMethod.ReadConfigJsonData("DeleteExamUsers");
            if (client.PutRequest(uri, userModel).Result)
                return true;
            else
                return false;
        }

        public bool Delete_ExamStusToRemoteService(ExamUserModel userModel, IHttpClientHelper client)
        {
            string uri = PubMethod.ReadConfigJsonData("DeleteExamUserByContentID");
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
        public void InitializeStuRecord(List<long> contentId, List<string> userNumber)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    for (int i = 0; i < contentId.Count; i++)
                    {
                        for (int j = 0; j < userNumber.Count; j++)
                        {
                            t_learning_record record = new t_learning_record();
                            record.content_id = contentId[i];
                            record.user_id = userNumber[j];
                            db.t_learning_record.Add(record);
                        }
                    }
                    db.SaveChanges();
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
        public object Add_TrainingPlanFromProgram(TrainingPlanFromProgram model, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    t_training_plan p = new t_training_plan();
                    p.plan_name = model.PlanName;
                    p.plan_status = "1";
                    p.plan_desc = model.PlanDesc;
                    p.start_time = DateTime.Parse(model.StartTime);
                    p.end_time = DateTime.Parse(model.EndTime);
                    p.create_time = DateTime.Now;
                    p.create_by = model.CreateBy;
                    p.update_time = DateTime.Now;
                    db.t_training_plan.Add(p);
                    db.SaveChanges();//保存至数据库
                    long MaxPlanID = (from t in db.t_training_plan select t.id).Max();//获取最大ID数值
                    int nSort = 0;//用于排序

                    //查找课程
                    var queryCourseID = from c in db.t_program_course_ref
                                        where c.delete_flag == 0 && c.programid == model.ProgramID
                                        select new { c.courseid };
                    var queryCourseIDList = queryCourseID.ToList();
                    if (queryCourseIDList.Count > 0)
                    {
                        for (int i = 0; i < queryCourseIDList.Count; i++)
                        {
                            t_plan_course_task_ref pct = new t_plan_course_task_ref();
                            pct.plan_id = MaxPlanID;
                            pct.content_id = queryCourseIDList[i].courseid;
                            pct.content_sort = ++nSort;
                            pct.dif = "1";//课程
                            pct.create_time = DateTime.Now;
                            pct.create_by = model.CreateBy;
                            pct.update_time = DateTime.Now;
                            db.t_plan_course_task_ref.Add(pct);
                        }
                    }

                    //查找训练科目
                    var querySubjectID = from s in db.t_program_subject_ref
                                         where s.delete_flag == 0 && s.programid == model.ProgramID
                                         select new { s.subjectid };
                    var querySubjectIDList = querySubjectID.ToList();
                    if (querySubjectIDList.Count > 0)
                    {
                        for (int i = 0; i < querySubjectIDList.Count; i++)
                        {
                            t_plan_course_task_ref pct = new t_plan_course_task_ref();
                            pct.plan_id = MaxPlanID;
                            pct.content_id = querySubjectIDList[i].subjectid;
                            pct.content_sort = ++nSort;
                            pct.dif = "2";//训练科目
                            pct.create_time = DateTime.Now;
                            pct.create_by = model.CreateBy;
                            pct.update_time = DateTime.Now;
                            db.t_plan_course_task_ref.Add(pct);
                        }
                    }

                    int j = db.SaveChanges();
                    if (j > 0)
                    {
                        var prom = from m in db.t_training_program
                                   where m.id == model.ProgramID
                                   select m;
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "培训计划";
                        syslog.logDesc = "从大纲:" + prom.FirstOrDefault().train_program_name + ",生成了培训计划:" + model.PlanName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, result = MaxPlanID, message = "OK" };
                    }
                    else
                        return new { code = 200, result = MaxPlanID, message = "OK" };
                }
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

        public object GetLearningStatisticByCourseID(long PlanID, long CourseID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    //查询培训计划下的总人数
                    int querySumCount = (from c in db.t_trainingplan_stu
                                         where c.delete_flag == 0 && c.trainingplan_id == PlanID
                                         select c.id).ToList().Count;

                    //查找正在学习的人数
                    var queryLearning = from l in db.t_learning_record
                                        join p in db.t_plan_course_task_ref on l.content_id equals p.id
                                        where p.plan_id == PlanID
                                              && p.content_id == CourseID
                                              && p.dif == "1"
                                              && l.learning_progress != "100"
                                        select l;
                    var LearningCount = queryLearning.ToList().Count;


                    //查找已完成的学员人数
                    var queryFinished = from p in db.t_plan_course_task_ref
                                        join l in db.t_learning_record on p.id equals l.content_id into pl
                                        from _pl in pl.DefaultIfEmpty()
                                        where p.plan_id == PlanID && _pl.learning_progress == "100" && p.content_id == CourseID && p.dif == "1"
                                        select p.id;
                    var nFinishedCount = queryFinished.ToList().Count;

                    CourseManagement cm = new CourseManagement();
                    object courseStruct = cm.GetCourseStruct(CourseID);

                    return new { code = 200, result = new { LearningCount = LearningCount, FinishedCount = nFinishedCount, courseStruct = courseStruct }, msg = "OK" };
                }
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
        public string Div { get; set; }
        public string CourseFlag { get; set; }
        public string TaskFlag { get; set; }
        public string ExamFlag { get; set; }
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
}
