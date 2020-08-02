using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Course.BLL
{
    public class MyTrainingTask
    {
        public object GetMyTrainTaskPlanList(string UserID)
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
                            var q = from c in db.t_plan_course_task_ref
                                    where c.delete_flag == 0 && c.plan_id == queryList[i].id && c.dif == "2"//查找是否存在任务
                                    select c;
                            if (q.ToList().Count == 0)
                                continue;

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
        public object GetMyTrainingTask(long PlanID, string startTime, string endTime, string strKeyWord, string UserID, int pageSize, int pageIndex)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    List<MyTrainTask> list = new List<MyTrainTask>();
                    IQueryable<t_training_plan> query = null;
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
                    var pp = query.ToList();
                    foreach (var item in query)
                    {
                        var queryTask = from p in db.t_plan_course_task_ref
                                        join t in db.t_training_task on p.content_id equals t.id
                                        where p.delete_flag == 0 && p.plan_id == item.id && p.dif == "2" && t.delete_flag == 0//任务
                                        && (string.IsNullOrEmpty(strKeyWord) ? true : t.task_name.Contains(strKeyWord))
                                        select new { ContentID = p.id, ID = t.id, SrcID = t.src_id, t.task_name, t.task_desc };
                        foreach (var itemTask in queryTask)
                        {
                            list.Add(new MyTrainTask()
                            {
                                ID = itemTask.ID,
                                SrcID = itemTask.SrcID,
                                TaskName = itemTask.task_name,
                                TaskDesc = itemTask.task_desc,
                                PlanName = item.plan_name,
                                StartTime = item.start_time,
                                EndTime = item.end_time,
                                PlanStatus = item.plan_status
                            });
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
    }

    public class MyTrainTask
    {
        public long ID { get; set; }
        public long? SrcID { get; set; }
        public string TaskName { get; set; }
        public string TaskDesc { get; set; }
        public string PlanName { get; set; }
        public string PlanStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
