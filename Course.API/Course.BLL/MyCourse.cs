using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Course.BLL
{
    public class MyCourse
    {
        public object GetMyTrainingPlanLList(string UserID)
        {
            try
            {
                Dictionary<long, string> dic = new Dictionary<long, string>();
                using (var db = new pf_course_manageContext())
                {
                    //我要学的
                    var query = from s in db.t_trainingplan_stu
                                join p in db.t_training_plan on s.trainingplan_id equals p.id
                                where s.delete_flag == 0 && p.delete_flag == 0 && s.uesr_number == UserID
                                       && (p.plan_status == "2" || p.plan_status == "3") 
                                       || (p.plan_status == "1" && p.publish_flag == 1)

                                select new { p.id, p.plan_name };
                    var queryList = query.ToList();
                    if (queryList.Count > 0)
                    {
                        for (int i = 0; i < queryList.Count; i++)
                        {
                            if (dic.ContainsKey(queryList[i].id))
                                continue;
                            else
                                dic.Add(queryList[i].id, queryList[i].plan_name);
                        }
                    }

                    //我要教的
                    var queryTeach = from p in db.t_training_plan
                                     join r in db.t_plan_course_task_ref on p.id equals r.plan_id
                                     where p.delete_flag == 0 && r.delete_flag == 0 && r.teacher_num == UserID
                                     select new { p.id, p.plan_name };
                    var queryTeachList = queryTeach.ToList();
                    if (queryTeachList.Count > 0)
                    {
                        for (int i = 0; i < queryTeachList.Count; i++)
                        {
                            if (dic.ContainsKey(queryTeachList[i].id))
                                continue;
                            else
                                dic.Add(queryTeachList[i].id, queryTeachList[i].plan_name);
                        }
                    }
                }
                List<MyCoursePlan> list = new List<MyCoursePlan>();
                foreach (var item in dic)
                {
                    list.Add(new MyCoursePlan() { Key = item.Key, Value = item.Value });
                }
                return new { code = 200, result = list, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object GetMyCourse(long PlanID, string startTime, string endTime, string strKeyWord, string UserID, int pageSize, int pageIndex)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    string strFds = PubMethod.ReadConfigJsonData("FastDFSUrl");
                    List<MyCourseModel> list = new List<MyCourseModel>();
                    IQueryable<t_training_plan> query = null;
                    //我要学的
                    if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        
                        query = from s in db.t_trainingplan_stu
                                join p in db.t_training_plan on s.trainingplan_id equals p.id
                                where s.delete_flag == 0
                                      && p.delete_flag == 0
                                      && s.uesr_number == UserID
                                      && DateTime.Parse(startTime) <= p.start_time
                                      && DateTime.Parse(endTime) >= p.end_time
                                && (PlanID == 0 ? ((p.plan_status == "1" && p.publish_flag == 1) || p.plan_status == "2" || p.plan_status == "3") : p.id == PlanID)
                                select p;
                    }
                    else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                    {
                        query = from s in db.t_trainingplan_stu
                                join p in db.t_training_plan on s.trainingplan_id equals p.id
                                where s.delete_flag == 0
                                      && p.delete_flag == 0
                                      && s.uesr_number == UserID
                                      && DateTime.Parse(startTime) <= p.start_time
                                && (PlanID == 0 ? ((p.plan_status == "1" && p.publish_flag == 1) || p.plan_status == "2" || p.plan_status == "3") : p.id == PlanID)
                                select p;
                    }
                    else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        query = from s in db.t_trainingplan_stu
                                join p in db.t_training_plan on s.trainingplan_id equals p.id
                                where s.delete_flag == 0
                                      && p.delete_flag == 0
                                      && s.uesr_number == UserID
                                      && DateTime.Parse(endTime) >= p.end_time
                                && (PlanID == 0 ? ((p.plan_status == "1" && p.publish_flag == 1) || p.plan_status == "2" || p.plan_status == "3") : p.id == PlanID)
                                select p;
                    }
                    else
                    {
                        query = from s in db.t_trainingplan_stu
                                join p in db.t_training_plan on s.trainingplan_id equals p.id
                                where s.delete_flag == 0
                                      && p.delete_flag == 0
                                      && s.uesr_number == UserID
                                && (PlanID == 0 ? ((p.plan_status == "1" && p.publish_flag == 1) || p.plan_status == "2" || p.plan_status == "3") : p.id == PlanID)
                                select p;
                    }

                    var q = query.ToList();//查找我的培训计划
                    if (q.Count > 0)
                    {
                        for (int i = 0; i < q.Count; i++)
                        {
                            var queryCourse = from p in db.t_plan_course_task_ref
                                              join c in db.t_course on p.content_id equals c.id
                                              where p.delete_flag == 0 && p.plan_id == q[i].id && p.dif == "1" && c.delete_flag == 0//课程
                                              && (string.IsNullOrEmpty(strKeyWord) ? true : c.course_name.Contains(strKeyWord))
                                              select new { ContentID = p.id, SrcID = c.id, c.course_name, c.thumbnail_path };
                            var queryCourseList = queryCourse.ToList();
                            if (queryCourseList.Count > 0)
                            {
                                for (int j = 0; j < queryCourseList.Count; j++)
                                {
                                    string path = "";
                                    if (!string.IsNullOrEmpty(queryCourseList[j].thumbnail_path))
                                    {
                                        path = strFds + queryCourseList[j].thumbnail_path;
                                    }

                                    list.Add(new MyCourseModel()
                                    {
                                        PlanID = q[i].id,//计划号
                                        ContentID = queryCourseList[j].ContentID,//关系表ID
                                        CourseID = queryCourseList[j].SrcID,
                                        CourseName = queryCourseList[j].course_name,
                                        PlanName = q[i].plan_name,
                                        StartTime = q[i].start_time,
                                        EndTime = q[i].end_time,
                                        ImagePath = path,
                                        PlanStatus = q[i].plan_status,//计划状态
                                        Dif = "1"//我要学的
                                    });
                                }
                            }
                        }
                    }

                    //我要教的
                    if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        var queryTeach = from p in db.t_training_plan
                                         join r in db.t_plan_course_task_ref on p.id equals r.plan_id
                                         where p.delete_flag == 0 && r.delete_flag == 0 && r.teacher_num == UserID
                                         && p.start_time <= DateTime.Parse(startTime)
                                         && p.end_time >= DateTime.Parse(endTime)
                                         && (PlanID == 0 ? true : p.id == PlanID)
                                         select new { p, r };
                        var queryTeachCourse = from t in queryTeach
                                               join c in db.t_course on t.r.content_id equals c.id
                                               where c.delete_flag == 0 && (string.IsNullOrEmpty(strKeyWord) ? true : c.course_name.Contains(strKeyWord))
                                               select new
                                               {
                                                   TrainingPlanID = t.p.id,
                                                   t.p.plan_name,
                                                   t.p.start_time,
                                                   t.p.end_time,
                                                   c.id,
                                                   c.course_name,
                                                   c.thumbnail_path
                                               };
                        var queryTeachCourseList = queryTeachCourse.ToList();
                        if (queryTeachCourseList.Count > 0)
                        {
                            for (int i = 0; i < queryTeachCourseList.Count; i++)
                            {
                                string path = "";
                                if (!string.IsNullOrEmpty(queryTeachCourseList[i].thumbnail_path))
                                {
                                    path = strFds + queryTeachCourseList[i].thumbnail_path;
                                }
                                list.Add(new MyCourseModel()
                                {
                                    PlanID = queryTeachCourseList[i].TrainingPlanID,//计划号
                                    CourseID = queryTeachCourseList[i].id,
                                    CourseName = queryTeachCourseList[i].course_name,
                                    PlanName = queryTeachCourseList[i].plan_name,
                                    StartTime = queryTeachCourseList[i].start_time,
                                    EndTime = queryTeachCourseList[i].end_time,
                                    ImagePath = path,
                                    Dif = "2"//我要教的 
                                });
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                    {
                        var queryTeach = from p in db.t_training_plan
                                         join r in db.t_plan_course_task_ref on p.id equals r.plan_id
                                         where p.delete_flag == 0 && r.delete_flag == 0 && r.teacher_num == UserID
                                         && p.start_time <= DateTime.Parse(startTime)
                                         && (PlanID == 0 ? true : p.id == PlanID)
                                         select new { p, r };
                        var queryTeachCourse = from t in queryTeach
                                               join c in db.t_course on t.r.content_id equals c.id
                                               where c.delete_flag == 0 && (string.IsNullOrEmpty(strKeyWord) ? true : c.course_name.Contains(strKeyWord))
                                               select new
                                               {
                                                   TrainingPlanID = t.p.id,
                                                   t.p.plan_name,
                                                   t.p.start_time,
                                                   t.p.end_time,
                                                   c.id,
                                                   c.course_name,
                                                   c.thumbnail_path
                                               };
                        var queryTeachCourseList = queryTeachCourse.ToList();
                        if (queryTeachCourseList.Count > 0)
                        {
                            for (int i = 0; i < queryTeachCourseList.Count; i++)
                            {
                                string path = "";
                                if (!string.IsNullOrEmpty(queryTeachCourseList[i].thumbnail_path))
                                {
                                    path = strFds + queryTeachCourseList[i].thumbnail_path;
                                }
                                list.Add(new MyCourseModel()
                                {
                                    PlanID = queryTeachCourseList[i].TrainingPlanID,//计划号
                                    CourseID = queryTeachCourseList[i].id,
                                    CourseName = queryTeachCourseList[i].course_name,
                                    PlanName = queryTeachCourseList[i].plan_name,
                                    StartTime = queryTeachCourseList[i].start_time,
                                    EndTime = queryTeachCourseList[i].end_time,
                                    ImagePath = path,
                                    Dif = "2"//我要教的 
                                });
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        var queryTeach = from p in db.t_training_plan
                                         join r in db.t_plan_course_task_ref on p.id equals r.plan_id
                                         where p.delete_flag == 0 && r.delete_flag == 0 && r.teacher_num == UserID
                                         && p.end_time >= DateTime.Parse(endTime)
                                         && (PlanID == 0 ? true : p.id == PlanID)
                                         select new { p, r };
                        var queryTeachCourse = from t in queryTeach
                                               join c in db.t_course on t.r.content_id equals c.id
                                               where c.delete_flag == 0 && (string.IsNullOrEmpty(strKeyWord) ? true : c.course_name.Contains(strKeyWord))
                                               select new
                                               {
                                                   TrainingPlanID = t.p.id,
                                                   t.p.plan_name,
                                                   t.p.start_time,
                                                   t.p.end_time,
                                                   c.id,
                                                   c.course_name,
                                                   c.thumbnail_path
                                               };
                        var queryTeachCourseList = queryTeachCourse.ToList();
                        if (queryTeachCourseList.Count > 0)
                        {
                            for (int i = 0; i < queryTeachCourseList.Count; i++)
                            {
                                string path = "";
                                if (!string.IsNullOrEmpty(queryTeachCourseList[i].thumbnail_path))
                                {
                                    path = strFds + queryTeachCourseList[i].thumbnail_path;
                                }
                                list.Add(new MyCourseModel()
                                {
                                    PlanID = queryTeachCourseList[i].TrainingPlanID,//计划号
                                    CourseID = queryTeachCourseList[i].id,
                                    CourseName = queryTeachCourseList[i].course_name,
                                    PlanName = queryTeachCourseList[i].plan_name,
                                    StartTime = queryTeachCourseList[i].start_time,
                                    EndTime = queryTeachCourseList[i].end_time,
                                    ImagePath = path,
                                    Dif = "2"//我要教的 
                                });
                            }
                        }
                    }
                    else
                    {
                        var queryTeach = from p in db.t_training_plan
                                         join r in db.t_plan_course_task_ref on p.id equals r.plan_id
                                         where p.delete_flag == 0 && r.delete_flag == 0 && r.teacher_num == UserID
                                         && (PlanID == 0 ? true : p.id == PlanID)
                                         select new { p, r };
                        var queryTeachCourse = from t in queryTeach
                                               join c in db.t_course on t.r.content_id equals c.id
                                               where c.delete_flag == 0 && (string.IsNullOrEmpty(strKeyWord) ? true : c.course_name.Contains(strKeyWord))
                                               select new
                                               {
                                                   TrainingPlanID = t.p.id,
                                                   t.p.plan_name,
                                                   t.p.start_time,
                                                   t.p.end_time,
                                                   c.id,
                                                   c.course_name,
                                                   c.thumbnail_path
                                               };
                        var queryTeachCourseList = queryTeachCourse.ToList();
                        if (queryTeachCourseList.Count > 0)
                        {
                            for (int i = 0; i < queryTeachCourseList.Count; i++)
                            {
                                string path = "";
                                if (!string.IsNullOrEmpty(queryTeachCourseList[i].thumbnail_path))
                                {
                                    path = strFds + queryTeachCourseList[i].thumbnail_path;
                                }
                                list.Add(new MyCourseModel()
                                {
                                    PlanID = queryTeachCourseList[i].TrainingPlanID,//计划号
                                    CourseID = queryTeachCourseList[i].id,
                                    CourseName = queryTeachCourseList[i].course_name,
                                    PlanName = queryTeachCourseList[i].plan_name,
                                    StartTime = queryTeachCourseList[i].start_time,
                                    EndTime = queryTeachCourseList[i].end_time,
                                    ImagePath = path,
                                    Dif = "2"//我要教的 
                                });
                            }
                        }
                    }

                    var ResultList = list.Skip(pageSize * (pageIndex - 1)).Take(pageSize);//分页
                    return new { code = 200, result = new { count = list.Count, result = ResultList }, messsage = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        private List<CourseStructRecord> list;
        public object GetStuCourseLearningRecord(string UserID, long PlanID, long CourseID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryContent = from c in db.t_plan_course_task_ref
                                       where c.delete_flag == 0 && c.plan_id == PlanID && c.content_id == CourseID
                                       && c.dif == "1"
                                       select new { c.id };
                    var ContentID = queryContent.FirstOrDefault();
                    if (ContentID == null)
                    {
                        return new { code = 400, message = "Error" };
                    }
                    else
                    {
                        //课程信息
                        var queryCourse = from c in db.t_course
                                          where c.delete_flag == 0 && c.id == CourseID
                                          select new { c.course_name, c.course_count, c.course_desc, c.thumbnail_path };
                        var q = queryCourse.FirstOrDefault();

                        //学习进度
                        var queryRecordProgress = from r in db.t_learning_record
                                                  where r.delete_flag == 0
                                                  && r.content_id == ContentID.id
                                                  && r.user_id == UserID
                                                  select new { r.id, r.learning_progress };
                        long RecordID = queryRecordProgress.FirstOrDefault().id;//记录ID
                        string strRecordProgress = queryRecordProgress.FirstOrDefault().learning_progress;

                        //最近学的
                        long StructID = 0;
                        string StructName = "";
                        var queryStatus = from s in db.t_course_node_learning_status
                                          where s.record_id == RecordID && s.delete_flag == 0
                                          orderby s.last_learning_time descending
                                          select new { s.src_id, s.course_struct_id };
                        var qs = queryStatus.FirstOrDefault();
                        if (qs != null)
                        {
                            var quer = (from s in db.t_course_struct
                                        where s.course_id == qs.src_id && s.id == qs.course_struct_id
                                        select new { s.id, s.course_node_name }).FirstOrDefault();
                            StructID = quer.id;
                            StructName = quer.course_node_name;
                        }

                        list = new List<CourseStructRecord>();

                        var query = from s in db.t_course_struct
                                    join p in db.t_course_node_learning_status
                                    on new { ID = s.id, CourseID = s.course_id } equals new { ID = p.course_struct_id, CourseID = p.src_id } into sp
                                    from _sp in sp.DefaultIfEmpty()
                                    where s.delete_flag == 0
                                          && s.course_id == CourseID
                                          && s.parent_id == 0
                                    select new
                                    {
                                        s,
                                        _sp.node_status,
                                        _sp.learning_time,
                                        _sp.attempt_number,
                                        _sp.last_learning_time
                                    };
                        var queryList = query.ToList();
                        if (queryList.Count > 0)
                        {
                            for (int i = 0; i < queryList.Count; i++)
                            {
                                CourseStructRecord node = new CourseStructRecord();
                                node.ID = queryList[i].s.id;
                                node.NodeNamae = queryList[i].s.course_node_name;
                                node.ParentID = queryList[i].s.parent_id;
                                node.Sort = queryList[i].s.node_sort;
                                node.NodeStatus = queryList[i].node_status;
                                node.LearningTime = queryList[i].learning_time;
                                node.AttemptNum = queryList[i].attempt_number;
                                node.LastLearnTime = queryList[i].last_learning_time;
                                GetChildCourseStruct(node, CourseID, RecordID);
                                list.Add(node);
                            }
                        }

                        return new { code = 200, result = new { CourseInfomation = q, RecordProgress = strRecordProgress, StructID = StructID, StructName = StructName, RecordID = RecordID, list = list }, message = "OK" };

                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        private void GetChildCourseStruct(CourseStructRecord courseStruct, long CourseID, long RecordID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from s in db.t_course_struct
                                join p in db.t_course_node_learning_status
                                on new { ID = s.id, CourseID = s.course_id } equals new { ID = p.course_struct_id, CourseID = p.src_id } into sp
                                from _sp in sp.DefaultIfEmpty()
                                where s.delete_flag == 0
                                      && s.course_id == CourseID
                                      && s.parent_id == courseStruct.ID
                                select new
                                {
                                    s,
                                    _sp.node_status,
                                    _sp.learning_time,
                                    _sp.attempt_number,
                                    _sp.last_learning_time
                                };
                    var queryList = query.ToList();
                    if (queryList.Count > 0)
                    {
                        for (int i = 0; i < queryList.Count; i++)
                        {
                            CourseStructRecord node = new CourseStructRecord();
                            node.ID = queryList[i].s.id;
                            node.NodeNamae = queryList[i].s.course_node_name;
                            node.ParentID = queryList[i].s.parent_id;
                            node.Sort = queryList[i].s.node_sort;
                            node.NodeStatus = queryList[i].node_status;
                            node.LearningTime = queryList[i].learning_time;
                            node.AttemptNum = queryList[i].attempt_number;
                            node.LastLearnTime = queryList[i].last_learning_time;
                            GetChildCourseStruct(node, CourseID, RecordID);//递归
                            courseStruct.Children.Add(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                //return new { code = 400, msg = "Error" };
            }

        }

        public object LearningContinue(long RecordID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var latestTime = (from r in db.t_course_node_learning_status
                                      where r.delete_flag == 0 && r.record_id == RecordID
                                      select r.last_learning_time).Max();
                    var query = from r in db.t_course_node_learning_status
                                where r.last_learning_time == latestTime
                                select r.course_struct_id;
                    return query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
    }

    public class MyCoursePlan
    {
        public long Key { get; set; }
        public string Value { get; set; }
    }

    public class CourseStructRecord
    {
        public CourseStructRecord()
        {
            Children = new List<CourseStructRecord>();
            ParentID = 0;
        }
        public long ID { get; set; }
        public string NodeNamae { get; set; }
        public long ParentID { get; set; }
        public int Sort { get; set; }
        public string NodeStatus { get; set; }
        public int? LearningTime { get; set; }
        public int? AttemptNum { get; set; }
        public DateTime? LastLearnTime { get; set; }
        public List<CourseStructRecord> Children { get; set; }
    }

    public class MyCourseModel
    {
        public long PlanID { get; set; }
        public string PlanStatus { get; set; }
        public long ContentID { get; set; }
        public long CourseID { get; set; }
        public string CourseName { get; set; }
        public string PlanName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Dif { get; set; }
        public string ImagePath { get; set; }
    }
}
