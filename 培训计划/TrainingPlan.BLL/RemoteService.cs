using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RemoteService
{
    public object UpdateStuScore(pf_training_plan_v1Context db,List<ExamResult> examResults)
    {
        try
        {
               if (examResults != null && examResults.Count > 0)
                {
                    //查找考试管理在计划库的ID
                    var queryExam = from e in db.t_examination_manage
                                    where e.delete_flag == 0 && e.src_id == examResults[0].ExamID
                                    select e;
                    var queryExamF = queryExam.FirstOrDefault();
                    if (queryExamF == null)
                        return new { code = 400, msg = "考试管理不存在！" };

                    //查找培训计划内容的ID
                    var queryContent = from c in db.t_plan_course_task_exam_ref
                                       where c.delete_flag == 0 && c.plan_id == examResults[0].PlanID && c.content_id == queryExamF.id
                                       select c;
                    var queryContentF = queryContent.FirstOrDefault();
                    if (queryContentF == null)
                        return new { code = 400, msg = "培训计划内容不存在！" };

                    //添加考试记录
                    for (int i = 0; i < examResults.Count; i++)
                    {
                        var query = from d in db.t_learning_record
                                    where d.delete_flag == 0 && d.content_id == queryContentF.id && d.user_number == examResults[i].UserNumber
                                    select d;
                        if (query.FirstOrDefault() == null)//不存在
                        {
                            t_learning_record r = new t_learning_record();
                            r.content_id = queryContentF.id;
                            r.user_number = examResults[i].UserNumber;
                            r.learning_progress = examResults[i].Score.ToString();
                            db.t_learning_record.Add(r);
                        }
                        else//存在
                        {
                            query.FirstOrDefault().learning_progress = examResults[i].Score.ToString();
                        }
                    }
                    db.SaveChanges();
                }
                return new { code = 200, msg = "OK" };           
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public object UpdatePlanProgress(pf_training_plan_v1Context db,List<PlanProgress> planProgresses)
    {
        try
        {
                if (planProgresses != null && planProgresses.Count > 0)
                {
                    for (int i = 0; i < planProgresses.Count; i++)
                    {
                        //累加培训计划下所有学员的进度，并更新培训计划进度
                        var querySumRecord = from c in db.t_plan_course_task_exam_ref
                                             join r in db.t_learning_record on c.id equals r.content_id
                                             where c.delete_flag == 0 && c.plan_id == planProgresses[i].PlanID && r.delete_flag == 0 && c.dif == "1"
                                             select r;
                        float sumprogress = 0;
                        foreach (var item in querySumRecord)
                        {
                            sumprogress = sumprogress + float.Parse(item.learning_progress);
                        }

                        //查询课程数量
                        var queryPlanR = from f in db.t_plan_course_task_exam_ref
                                         where f.delete_flag == 0 && f.plan_id == planProgresses[i].PlanID && f.dif == "1"
                                         select f;

                        //培训计划下所有学员的数量
                        var queryStu = from s in db.t_trainingplan_stu
                                       where s.delete_flag == 0 && s.trainingplan_id == planProgresses[i].PlanID
                                       select s;
                        int stuCount = queryStu.Count();
                        decimal p = (decimal)sumprogress / (stuCount * queryPlanR.Count());

                        //更新培训计划进度
                        var queryPlan = from pp in db.t_training_plan
                                        where pp.delete_flag == 0 && pp.id == planProgresses[i].PlanID
                                        select pp;
                        var queryPlanF = queryPlan.FirstOrDefault();
                        queryPlanF.finish_rate = (p + planProgresses[i].ProgressValue) / 2;
                    }

                }
                db.SaveChanges();
                return new { code = 200, msg = "OK" };            
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public object UpdateExamFinishRate(pf_training_plan_v1Context db,ExamFinishRate examFinishRate)
    {
        try
        {
                var query = from a in db.t_trainingplan_stustatistic
                            where a.delete_flag == 0 && a.trainingplan_id == examFinishRate.PlanId && a.user_id == examFinishRate.UserId && a.user_number == examFinishRate.UserNumber
                            select a;
                var queryF = query.FirstOrDefault();
                if (queryF == null)
                {
                    t_trainingplan_stustatistic stustatistic = new t_trainingplan_stustatistic();
                    stustatistic.trainingplan_id = examFinishRate.PlanId;
                    stustatistic.user_id = examFinishRate.UserId;
                    stustatistic.user_number = examFinishRate.UserNumber;
                    stustatistic.exam_comrate = examFinishRate.Rate;
                    db.t_trainingplan_stustatistic.Add(stustatistic);
                }
                else
                {
                    queryF.exam_comrate = examFinishRate.Rate;
                }
                db.SaveChanges();
                return new { code = 200, msg = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
}

public class PlanProgress
{
    public long PlanID { get; set; }
    public decimal ProgressValue { get; set; }
}
public class ExamResult
{
    public long PlanID { get; set; }
    public long ExamID { get; set; }
    public string UserNumber { get; set; }
    public float Score { get; set; }
}
public class ExamFinishRate
{
    public long PlanId { get; set; }
    public long UserId { get; set; }
    public string UserNumber { get; set; }
    public decimal Rate { get; set; }
}

