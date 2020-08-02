using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

public class StatisticData
{
    #region 培训统计

    /// <summary>
    /// 获取当前正在进行的培训计划数量
    /// </summary>
    /// <returns></returns>
    public object GetCurrentTrainingPlanCount(pf_training_plan_v1Context db)
    {
        try
        {

            var query = from s in db.t_training_plan
                        where s.delete_flag == 0 && s.plan_status == "2"
                        select s;
            return new { code = 200, result = query.Count(), message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    /// <summary>
    /// 获取培训计划信息
    /// </summary>
    /// <returns></returns>
    public object GetTrainingPlanInfo(pf_training_plan_v1Context db)
    {
        try
        {
            var query = from p in db.t_training_plan
                        where p.delete_flag == 0 && p.plan_status != "3"
                        orderby p.create_time descending
                        select p;
            List<TrainPlan> planList = new List<TrainPlan>();
            int i = 0;
            foreach (var item in query)
            {
                planList.Add(new TrainPlan()
                {
                    Index = ++i,
                    PlanName = item.plan_name,
                    StartTime = item.start_time.ToString(),
                    EndTime = item.end_time.ToString(),
                    PlanStatus = item.plan_status,
                    StuCount = item.stu_count
                });
            }
            return new { code = 200, result = new { PlanList = planList, count = query.Count() }, message = "OK" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    /// <summary>
    /// 按年份查找培训计划中人数
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public object GetTrainingPlanInfoByYear(pf_training_plan_v1Context db,int year)
    {
        try
        {
                var query = from p in db.t_training_plan
                            where p.delete_flag == 0 && p.create_time.Value.Year == year
                            orderby p.create_time descending
                            select p;
                List<TrainPlan> planList = new List<TrainPlan>();
                int i = 0;
                foreach (var item in query)
                {
                    planList.Add(new TrainPlan()
                    {
                        Index = ++i,
                        PlanName = item.plan_name,
                        StartTime = item.start_time.ToString(),
                        EndTime = item.end_time.ToString(),
                        PlanStatus = item.plan_status,
                        StuCount = item.stu_count
                    });
                }
                return new { code = 200, result = new { PlanList = planList, count = query.Count() }, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    /// <summary>
    /// 获取培训计划完成率
    /// </summary>
    /// <returns></returns>
    public object GetTrainingPlanFinishRate(pf_training_plan_v1Context db,bool IsAsc, string FeildName)
    {
        try
        {
                var query = from p in db.t_training_plan
                            where p.delete_flag == 0
                            orderby p.create_time descending
                            select p;
                List<TrainPlan> list = new List<TrainPlan>();
                decimal finishRate = 0;
                foreach (var item in query)
                {
                    list.Add(new TrainPlan()
                    {
                        PlanName = item.plan_name,
                        FinishRate = item.finish_rate
                    });
                    finishRate = finishRate + item.finish_rate;
                }

                if (IsAsc && FeildName != "")
                    list = list.OrderBy(x => x.FinishRate).ToList();
                else if (IsAsc == false && FeildName != "")
                    list = list.OrderByDescending(x => x.FinishRate).ToList();

                return new { code = 200, result = new { list = list, finishRate = (finishRate / query.Count()).ToString("#0.00") }, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    #endregion

    #region 理论教学

    /// <summary>
    /// 获取培训计划平均学习时长
    /// </summary>
    /// <returns></returns>
    public object GetAvgLearningTime(pf_training_plan_v1Context db)
    {
        try
        {
                var query = from p in db.t_training_plan
                            join r in db.t_plan_course_task_exam_ref on p.id equals r.plan_id
                            join l in db.t_learning_record on r.id equals l.content_id
                            where p.delete_flag == 0 && r.delete_flag == 0 && l.delete_flag == 0 && r.dif == "1"
                            select new { p.id, p.plan_name, learning_time = l.learning_sum_time };

                var queryResult = from q in query
                                  group q by new { q.id, q.plan_name }
                                  into r
                                  select new
                                  {
                                      ID = r.Key.id,
                                      PlanName = r.Key.plan_name,
                                      LearningTime = r.Sum(x => x.learning_time)
                                  };
                List<PlanAvgLearningTime> list = new List<PlanAvgLearningTime>();
                foreach (var item in queryResult)
                {
                    var queryStu = from s in db.t_trainingplan_stu
                                   where s.delete_flag == 0 && s.trainingplan_id == item.ID
                                   select s.id;
                    list.Add(new PlanAvgLearningTime()
                    {
                        ID = item.ID,
                        PlanName = item.PlanName,
                        AvgTime = ((double)item.LearningTime / (queryStu.Count() * 3600.00)).ToString("#0.00")
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

    /// <summary>
    /// 获取课程学习完成率
    /// </summary>
    /// <returns></returns>
    public object GetCourseFinishRate(pf_training_plan_v1Context db,bool IsAsc, string FeildName)
    {
        try
        {
                List<CourseFinishRate> courseFinishRates = new List<CourseFinishRate>();
                var queryCourseFinishRate = from c in db.t_course
                                            join r in db.t_plan_course_task_exam_ref on c.id equals r.content_id
                                            where c.delete_flag == 0 && r.dif == "1" && r.delete_flag == 0
                                            select new { c.id, c.course_name, r.finish_rate };
                var CourseFinishRate = queryCourseFinishRate.GroupBy(x => new { x.id, x.course_name }).Select(x => new { x.Key.id, x.Key.course_name, finish_rate = x.Average(aaa => aaa.finish_rate) });
                foreach (var item in CourseFinishRate)
                {
                    courseFinishRates.Add(new CourseFinishRate()
                    {
                        CourseID = item.id,
                        CourseName = item.course_name,
                        Rate = item.finish_rate.ToString("#0.00")
                    });
                }
                if (IsAsc && !string.IsNullOrEmpty(FeildName))
                    courseFinishRates = courseFinishRates.OrderBy(arg => arg.Rate).ToList();
                else if (IsAsc == false && !string.IsNullOrEmpty(FeildName))
                    courseFinishRates = courseFinishRates.OrderByDescending(arg => arg.Rate).ToList();

                return new { code = 200, result = courseFinishRates, msg = "OK" };
            
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    /// <summary>
    /// 获取知识点掌握度
    /// </summary>
    /// <returns></returns>
    public object GetKnowledgeDegree(pf_training_plan_v1Context db)
    {
        try
        {
                List<CourseFinishRate> courseFinishRates = new List<CourseFinishRate>();
                var queryCourseFinishRate = from c in db.t_course
                                            join r in db.t_plan_course_task_exam_ref on c.id equals r.content_id
                                            where c.delete_flag == 0 && r.dif == "1"
                                            select new { c.id, c.course_name, r.finish_rate };
                var CourseFinishRate = queryCourseFinishRate.GroupBy(x => new { x.id, x.course_name }).Select(x => new { x.Key.id, x.Key.course_name, finish_rate = x.Average(aaa => aaa.finish_rate) });
                foreach (var item in CourseFinishRate)
                {
                    courseFinishRates.Add(new CourseFinishRate()
                    {
                        CourseID = item.id,
                        CourseName = item.course_name,
                        Rate = item.finish_rate.ToString()
                    });
                }
                //知识点掌握度
                List<KnowledgeDegree1> tags = new List<KnowledgeDegree1>();
                foreach (var item in courseFinishRates)
                {
                    var queryTag = from r in db.t_course_know_tag
                                   join t in db.t_knowledge_tag on r.tag_id equals t.id
                                   where r.course_id == item.CourseID
                                   select t;
                    foreach (var t in queryTag)
                    {
                        tags.Add(new KnowledgeDegree1()
                        {
                            ID = t.id,
                            KnowledgeName = t.tag,
                            Rate = decimal.Parse(item.Rate)
                        });
                    }
                }
                //取平均值
                var queryTagGroup = tags.GroupBy(x => new { x.ID, x.KnowledgeName }).Select(x => new { x.Key.ID, x.Key.KnowledgeName, avg = x.Average(a => a.Rate) });
                return new { code = 200, result = queryTagGroup.ToList(), message = "OK" };
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

public class TrainPlan
{
    public int Index { get; set; }
    public string PlanName { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int StuCount { get; set; }
    public string PlanStatus { get; set; }
    public decimal FinishRate { get; set; }
}

public class PlanAvgLearningTime
{
    public long ID { get; set; }
    public string PlanName { get; set; }
    public string AvgTime { get; set; }
}

public class CourseFinishRate
{
    public long? PlanID { get; set; }
    public long CourseID { get; set; }
    public string CourseName { get; set; }
    public decimal LearnProgress { get; set; }
    public string Rate { get; set; }
}

public class KnowledgeDegree1
{
    public long ID { get; set; }
    public string KnowledgeName { get; set; }
    public decimal Rate { get; set; }

}

