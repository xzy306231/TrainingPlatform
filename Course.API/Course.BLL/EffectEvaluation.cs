using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Course.BLL
{
    public class EffectEvaluation
    {
        public object GetEndTrainingPlan(string keyWord, string startTime, string endTime, int pageIndex, int pageSize)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    IQueryable<t_training_plan> query = null;
                    if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        query = db.t_training_plan.Where(x => x.delete_flag == 0
                                        && x.plan_status == "3"
                                        && (string.IsNullOrEmpty(keyWord) ? true : x.plan_name.Contains(keyWord))
                                        && (string.IsNullOrEmpty(startTime) ? true : (DateTime.Parse(startTime) <= x.start_time))
                                        && (string.IsNullOrEmpty(endTime) ? true : (DateTime.Parse(endTime) >= x.end_time)));
                    }
                    else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        query = db.t_training_plan.Where(x => x.delete_flag == 0
                                        && x.plan_status == "3"
                                        && (string.IsNullOrEmpty(keyWord) ? true : x.plan_name.Contains(keyWord))
                                        && (string.IsNullOrEmpty(endTime) ? true : (DateTime.Parse(endTime) <= x.end_time)));
                    }
                    else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                    {
                        query = db.t_training_plan.Where(x => x.delete_flag == 0
                                        && x.plan_status == "3"
                                        && (string.IsNullOrEmpty(keyWord) ? true : x.plan_name.Contains(keyWord))
                                        && (string.IsNullOrEmpty(startTime) ? true : (DateTime.Parse(startTime) <= x.start_time)));
                    }
                    else
                    {
                        query = db.t_training_plan.Where(x => x.delete_flag == 0
                                        && x.plan_status == "3"
                                        && (string.IsNullOrEmpty(keyWord) ? true : x.plan_name.Contains(keyWord)));
                    }

                    int nCount = query.Count();
                    query = query.OrderByDescending(x => x.create_time).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                    int Index = pageSize * (pageIndex - 1);
                    List<EffectEvaluationPlan> list = new List<EffectEvaluationPlan>();
                    foreach (var item in query)
                    {
                        int i = ++Index;
                        string str = i.ToString().PadLeft(2, '0');
                        list.Add(new EffectEvaluationPlan
                        {
                            Index = str,
                            ID = item.id,
                            PlanName = item.plan_name,
                            StartTime = item.start_time.ToString(),
                            EndTime = item.end_time.ToString(),
                            StuCount = item.stu_count,
                            PlanDesc = item.plan_desc,
                            CourseFlag = item.course_flag.ToString(),
                            ExamFalg = item.exam_flag.ToString(),
                            TaskFlag = item.task_flag.ToString()
                        });
                    }
                    return new { code = 200, result = new { list = list, count = nCount }, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        #region 个人报告
        public object GetPersonalPlanInfo(string userNumber, long planId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    ReportResult reportResult = new ReportResult();
                    var queryPlanF = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == planId).FirstOrDefault();
                    reportResult.PlanName = queryPlanF.plan_name;
                    reportResult.StartTime = queryPlanF.start_time.ToString();
                    reportResult.EndTime = queryPlanF.end_time.ToString();
                    reportResult.StuCount = queryPlanF.stu_count;
                    reportResult.PlanDesc = queryPlanF.plan_desc;
                    return new { code = 200, result = reportResult, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                // 它项目根目录在 /app 你程序写入日志有问题
                //路径有问题？你应用程序在app里面   你的日志都写错地方了
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetStuByPlanID(long planId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryStu = from s in db.t_trainingplan_stu
                                   where s.delete_flag == 0 && s.trainingplan_id == planId
                                   select new { s.uesr_number, s.user_id, s.user_name };
                    return new { code = 200, result = queryStu.ToList(), message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetPersonalTheoryReportResult(string userNumber, long planId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    ReportResult reportResult = new ReportResult();
                    //培训计划下的人数
                    var planStuCount = db.t_trainingplan_stu.Where(x => x.delete_flag == 0 && x.trainingplan_id == planId).Count();
                    //培训计划下课程数量
                    var planCourseCount = db.t_plan_course_task_ref.Where(x => x.delete_flag == 0 && x.dif == "1").Count();

                    #region 学习时长指标
                    var queryPlanLearningTime = from p in db.t_training_plan
                                                join a in db.t_plan_course_task_ref on p.id equals a.plan_id
                                                join r in db.t_learning_record on a.id equals r.content_id
                                                where p.id == planId && a.dif == "1" && p.delete_flag == 0 && a.delete_flag == 0 && r.delete_flag == 0 && r.learning_progress != null
                                                select new { r.user_id, learningtime = r.learning_sum_time };
                    if (queryPlanLearningTime.Count() == 0)
                        return new { code = 200, message = "计划下不存在任何学习记录数据" };

                    // var aaa = queryPlanLearningTime.ToList();
                    //分组
                    var queryPlanLearningTimeGroup = queryPlanLearningTime.OrderByDescending(x => x.learningtime).ToList();
                    //转成小时
                    List<PlanStuLearningTime> PlanLearningTimeGroup = new List<PlanStuLearningTime>();
                    foreach (var item in queryPlanLearningTimeGroup)
                    {
                        PlanLearningTimeGroup.Add(new PlanStuLearningTime() { user_id = item.user_id, learningHours = (double)item.learningtime / 3600 });
                    }

                    //学生学习时长
                    if (PlanLearningTimeGroup.Find(x => x.user_id == userNumber) != null)
                        reportResult.TheorySumLearningTime = PlanLearningTimeGroup.Find(x => x.user_id == userNumber).learningHours.ToString("#0.00");
                    else
                        reportResult.TheorySumLearningTime = "0";

                    //学习时长排名
                    int TimeRank = 0;
                    if (PlanLearningTimeGroup.FindIndex(x => x.user_id == userNumber) != -1)
                    {
                        TimeRank = PlanLearningTimeGroup.FindIndex(x => x.user_id == userNumber) + 1;
                        reportResult.TheoryLearningTimeRank = TimeRank.ToString();
                    }
                    else
                        reportResult.TheoryLearningTimeRank = "0";

                    //超出的学生百分比
                    if (PlanLearningTimeGroup.FindIndex(x => x.user_id == userNumber) != -1)
                    {
                        //分子
                        var tpc = planStuCount - (PlanLearningTimeGroup.FindIndex(x => x.user_id == userNumber) + 1);
                        //分母
                        var tpm = planStuCount;
                        reportResult.TheoryLearningTimeExceedPercent = (tpc * 100 / (double)tpm).ToString("#0.00");
                    }
                    else
                        reportResult.TheoryLearningTimeExceedPercent = "0";

                    //计算差值
                    double dif = 0;
                    if (PlanLearningTimeGroup.Find(x => x.user_id == userNumber) != null)
                    {
                        dif = PlanLearningTimeGroup.Find(x => x.user_id == userNumber).learningHours - (PlanLearningTimeGroup.Sum(x => x.learningHours) / planStuCount);
                        if (dif >= 0)
                            reportResult.TheoryLevelFlag = "超出";
                        else
                            reportResult.TheoryLevelFlag = "低于";
                    }
                    else
                        reportResult.TheoryLevelFlag = "超出";

                    //超出低于水平时间
                    reportResult.TheoryDifHours = Math.Abs(dif).ToString("#0.00");

                    //平均学习时长
                    double sumLearningHours = PlanLearningTimeGroup.Sum(x => x.learningHours);
                    double avgtimedouble = sumLearningHours / planStuCount;

                    reportResult.TheoryAllLearningTime = sumLearningHours.ToString("#0.00");
                    reportResult.TheoryAvgLearningTimePercent = (avgtimedouble * 100 / sumLearningHours).ToString("#0.00");
                    reportResult.TheoryAvgLearningTime = avgtimedouble.ToString("#0.00");

                    //添加平均值
                    PlanLearningTimeGroup.Add(new PlanStuLearningTime() { user_id = "", learningHours = avgtimedouble });
                    //重新排序
                    PlanLearningTimeGroup = PlanLearningTimeGroup.OrderByDescending(x => x.learningHours).ToList();
                    //查找出索引值（排名）
                    int avgTimeRank = PlanLearningTimeGroup.FindIndex(x => x.learningHours == avgtimedouble) + 1;
                    //与平均排名比较
                    int TimeRankResult = avgTimeRank - TimeRank;


                    #endregion

                    #region 理论学习完成率

                    var queryPlanFinishRate = from p in db.t_training_plan
                                              join a in db.t_plan_course_task_ref on p.id equals a.plan_id
                                              join r in db.t_learning_record on a.id equals r.content_id
                                              where p.id == planId && a.dif == "1" && p.delete_flag == 0 && a.delete_flag == 0 && r.delete_flag == 0 && r.learning_progress != null
                                              select new { r.user_id, r.learning_progress };
                    //分组降序
                    List<TheoryFinishRate> listPlanFinishRate = new List<TheoryFinishRate>();
                    foreach (var item in queryPlanFinishRate)
                    {
                        listPlanFinishRate.Add(new TheoryFinishRate { UserNumber = item.user_id, Progress = double.Parse(item.learning_progress) });
                    }
                    var groupPlanFinishRate = listPlanFinishRate.GroupBy(a => new { a.UserNumber }).Select(a => new { a.Key.UserNumber, avg = a.Average(x => x.Progress) }).OrderByDescending(x => x.avg).ToList();

                    //理论学习完成率
                    if (groupPlanFinishRate.Find(x => x.UserNumber == userNumber) != null)
                        reportResult.TheorySumFininshRate = groupPlanFinishRate.Find(x => x.UserNumber == userNumber).avg.ToString("#0.00");
                    else
                        reportResult.TheorySumFininshRate = "0";


                    //完成率排名
                    int FinishRateRank = 0;
                    if (groupPlanFinishRate.FindIndex(x => x.UserNumber == userNumber) != -1)
                    {
                        FinishRateRank = groupPlanFinishRate.FindIndex(x => x.UserNumber == userNumber) + 1;
                        reportResult.TheoryFinishRateRank = FinishRateRank.ToString();
                    }
                    else
                        reportResult.TheoryFinishRateRank = "0";

                    //超出的学生百分比
                    if (groupPlanFinishRate.FindIndex(x => x.UserNumber == userNumber) != -1)
                    {
                        var pc = groupPlanFinishRate.Count - (groupPlanFinishRate.FindIndex(x => x.UserNumber == userNumber) + 1);
                        var pm = planStuCount;
                        reportResult.TheoryFinishRateExceedPercent = ((pc * 100) / (double)pm).ToString("#0.00");
                    }
                    else
                        reportResult.TheoryFinishRateExceedPercent = "0";

                    //计算差值
                    double dif1 = 0;
                    if (groupPlanFinishRate.Find(x => x.UserNumber == userNumber) != null)
                    {
                        dif1 = groupPlanFinishRate.Find(x => x.UserNumber == userNumber).avg - (groupPlanFinishRate.Sum(x => x.avg) / planStuCount);
                        if (dif1 >= 0)
                            reportResult.TheoryRateLevelFlag = "超出";
                        else
                            reportResult.TheoryRateLevelFlag = "低于";
                    }
                    else
                        reportResult.TheoryRateLevelFlag = "超出";
                    //超出低于的完成率
                    reportResult.TheoryDifFinishRate = Math.Abs(dif1).ToString("#0.00");

                    //平均完成率
                    double avgFinishRate = groupPlanFinishRate.Sum(x => x.avg) / planStuCount;
                    reportResult.TheoryAvgFinishRate = avgFinishRate.ToString("#0.00");
                    //添加平均值
                    groupPlanFinishRate.Add(new { UserNumber = "", avg = avgFinishRate });
                    //重新排序
                    groupPlanFinishRate = groupPlanFinishRate.OrderByDescending(x => x.avg).ToList();
                    //查找出索引值（排名）
                    int avgFinishRateRank = groupPlanFinishRate.FindIndex(x => x.avg == avgFinishRate && x.UserNumber == "") + 1;
                    //与平均值比较排名
                    int FinishRateRankResult = avgFinishRateRank - FinishRateRank;

                    double TheoryLearningGlobalResult = FinishRateRankResult + TimeRankResult;
                    if (TheoryLearningGlobalResult >= 0)
                        reportResult.TheoryGlobalResult = "1";
                    else
                        reportResult.TheoryGlobalResult = "0";

                    #endregion

                    return new { code = 200, result = reportResult, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public void GetPersonalExamReportResult(ReportResult reportResult, string userNumber, long planId, IHttpClientHelper client)
        {
            try
            {
                string url = PubMethod.ReadConfigJsonData("ReportResult");
                string fullUrl = url + "userNumber=" + userNumber + "&planId=" + planId;
                string strResult = client.GetRequest(fullUrl).Result;
                ReportResult exam_reportResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResult>(strResult);
                //正确率
                reportResult.ExamCorrectRate = exam_reportResult.ExamCorrectRate;
                reportResult.ExamCorrectRateRank = exam_reportResult.ExamCorrectRateRank;
                reportResult.ExamCorrectRateExceedPercent = exam_reportResult.ExamCorrectRateExceedPercent;
                reportResult.ExamCorrectAvgRate = exam_reportResult.ExamCorrectAvgRate;
                reportResult.ExamCorrectLevelFlag = exam_reportResult.ExamCorrectLevelFlag;
                reportResult.ExamDifCorrectRate = exam_reportResult.ExamDifCorrectRate;
                //通过率
                reportResult.ExamPassRate = exam_reportResult.ExamPassRate;
                reportResult.ExamPassRateRank = exam_reportResult.ExamPassRateRank;
                reportResult.ExamPassRatePercent = exam_reportResult.ExamPassRatePercent;
                reportResult.ExamPassAvgRate = exam_reportResult.ExamPassAvgRate;
                reportResult.ExamPassRateFlag = exam_reportResult.ExamPassRateFlag;
                reportResult.ExamPassDifRate = exam_reportResult.ExamPassDifRate;
                //知识点掌握度
                reportResult.TheoryGoodDegree = exam_reportResult.TheoryGoodDegree;
                reportResult.TheoryBadDegree = exam_reportResult.TheoryBadDegree;
                reportResult.TaskGoodDegree = exam_reportResult.TaskGoodDegree;
                reportResult.TaskBadDegree = exam_reportResult.TaskBadDegree;
                //整体结果
                reportResult.ExamGlobalResult = exam_reportResult.ExamGlobalResult;
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }

        #endregion

        #region 整体报告
        public object GetPlanReportResult(long planId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryPlanF = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == planId).FirstOrDefault();
                    PlanInfo planInfo = new PlanInfo();
                    planInfo.PlanName = queryPlanF.plan_name;
                    planInfo.StartTime = queryPlanF.start_time.ToString();
                    planInfo.EndTime = queryPlanF.end_time.ToString();
                    planInfo.StuCount = queryPlanF.stu_count;
                    planInfo.PlanDesc = queryPlanF.plan_desc;
                    return new { code = 200, result = planInfo, message = "OK" };
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetPlanTheoryReportResult(long planId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    PlanLearningInfo planLearningInfo = new PlanLearningInfo();

                    #region 学习时长

                    //本次学员人数
                    int stuCount = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == planId).FirstOrDefault().stu_count;
                    planLearningInfo.StuCount = stuCount.ToString();

                    //本次培训计划下课程的数量
                    var queryTheoryCourseCount = db.t_plan_course_task_ref.Where(x => x.delete_flag == 0 && x.plan_id == planId && x.dif == "1").Count();

                    //所有培训计划下课程数量
                    var queryPlanCourseCount = db.t_plan_course_task_ref.Where(x => x.delete_flag == 0 && x.dif == "1").Count();

                    //培训计划数量
                    var queryPlanCount = db.t_training_plan.Where(x => x.delete_flag == 0).Count();

                    //课程培训总人数
                    var querySumStuCount = (from p in db.t_training_plan
                                            join r in db.t_plan_course_task_ref on p.id equals r.plan_id
                                            where p.delete_flag == 0 && r.dif == "1"
                                            select p.stu_count).ToList().Sum();
                    //平均人数
                    double queryAvgCount = querySumStuCount / (double)queryPlanCount;
                    //差值
                    double stuDifCount = stuCount - queryAvgCount;
                    planLearningInfo.StuExceedCount = Math.Abs(stuDifCount).ToString("#0");
                    if (stuDifCount >= 0)
                        planLearningInfo.StuExceedCountFlag = "超出";
                    else
                        planLearningInfo.StuExceedCountFlag = "低于";

                    var queryPlanLearningTime = from p in db.t_training_plan
                                                join a in db.t_plan_course_task_ref on p.id equals a.plan_id
                                                join r in db.t_learning_record on a.id equals r.content_id
                                                where a.dif == "1" && p.delete_flag == 0 && a.delete_flag == 0 && r.delete_flag == 0
                                                select new { p.id, learning_time = r.learning_sum_time };
                    //分组、累加、降序
                    var queryGroupLearningTime = (from g in queryPlanLearningTime
                                                  group g by new { g.id } into grp
                                                  select new { grp.Key.id, sum = grp.Sum(x => x.learning_time) }).OrderByDescending(x => x.sum).ToList();
                    //本次理论时长
                    var thisPlanLearningTime = queryGroupLearningTime.Find(x => x.id == planId).sum / 3600.00;
                    planLearningInfo.LearningHours = ((double)thisPlanLearningTime).ToString("#0.00");

                    //本次培训平均时长
                    var thisAvgPlanLearningTime = thisPlanLearningTime / (stuCount * queryTheoryCourseCount);
                    planLearningInfo.AvgLearningHours = ((double)thisAvgPlanLearningTime).ToString("#0.00");

                    //所有的平均时长
                    var allAvgPlanLearningTime = queryGroupLearningTime.Sum(x => x.sum) / (queryPlanCourseCount * querySumStuCount);

                    //计算超出低于差值
                    var PlanLearningTimeDif = (queryGroupLearningTime.Find(x => x.id == planId).sum / (stuCount* queryTheoryCourseCount)) - allAvgPlanLearningTime;
                    if (PlanLearningTimeDif >= 0)
                        planLearningInfo.LearningExceedFlag = "超出";
                    else
                        planLearningInfo.LearningExceedFlag = "低于";

                    //超出的时长
                    planLearningInfo.LearningExceedHours = Math.Abs((double)PlanLearningTimeDif / 3600).ToString("#0.00");

                    //本次培训计划排名
                    var LearningTimeRank = queryGroupLearningTime.FindIndex(x => x.id == planId) + 1;

                    //所有的平均时长（分母培训计划数量）
                    var avgLearningTime = queryGroupLearningTime.Sum(x => x.sum) / queryPlanCount;
                    //所有的总时长
                    planLearningInfo.AllLearningSumHours = ((int)queryGroupLearningTime.Sum(x => x.sum) / 3600.00).ToString("#0.00");
                    //所有的平均时长
                    planLearningInfo.AllLearningAvgHours = ((int)queryGroupLearningTime.Sum(x => x.sum)/ (((double)queryPlanCount)*3600)).ToString("#0.00");

                    queryGroupLearningTime.Add(new { id = (long)0, sum = avgLearningTime });
                    queryGroupLearningTime = queryGroupLearningTime.OrderByDescending(x => x.sum).ToList();
                    //平均值排名
                    int avgRank = queryGroupLearningTime.FindIndex(x => x.id == 0) + 1;
                    //排名差值
                    int DifRank = avgRank - LearningTimeRank;

                    #endregion

                    #region 完成率

                    PlanFinishRate planFinishRate = new PlanFinishRate();



                    planFinishRate.TheoryCourseCount = queryTheoryCourseCount.ToString();
                    var queryPlanFinishRate = from p in db.t_training_plan
                                              join a in db.t_plan_course_task_ref on p.id equals a.plan_id
                                              join r in db.t_learning_record on a.id equals r.content_id
                                              where a.dif == "1" && p.delete_flag == 0 && a.delete_flag == 0 && r.delete_flag == 0
                                              select new { p.id, r.learning_progress };
                    List<TheoryFinishRate> listPlanFinishRate = new List<TheoryFinishRate>();
                    foreach (var item in queryPlanFinishRate)
                    {
                        double temp = 0;
                        if (string.IsNullOrEmpty(item.learning_progress))
                            temp = 0;
                        else
                            temp = double.Parse(item.learning_progress);

                        listPlanFinishRate.Add(new TheoryFinishRate
                        {
                            PlanID = item.id,
                            Progress = temp
                        });
                    }
                    //重新分组排序
                    var groupPlanFinishRate = listPlanFinishRate.GroupBy(x => new { x.PlanID }).Select(x => new { x.Key.PlanID, sum = x.Sum(a => a.Progress) }).OrderByDescending(x => x.sum).ToList();

                    var sumFinishRate = groupPlanFinishRate.Find(x => x.PlanID == planId).sum;
                    //排名
                    var FinishRank = groupPlanFinishRate.FindIndex(x => x.PlanID == planId) + 1;
                    var avgFinishRate = sumFinishRate / (queryTheoryCourseCount * stuCount);
                    //平均完成课程数
                    planFinishRate.TheoryFinishedCourseCount = (avgFinishRate * queryTheoryCourseCount / 100).ToString("#0.00");
                    //平均完成率
                    planFinishRate.AvgFinishRate = avgFinishRate.ToString("#0.00");

                    //培训平均值
                    var planAvgFinishRate = db.t_training_plan.Where(x => x.delete_flag == 0).Select(x => new { x.finish_rate }).ToList().Average(x => x.finish_rate);
                    var AvgPlanFinishRate = groupPlanFinishRate.Sum(x => x.sum) / (queryPlanCourseCount * querySumStuCount);
                    planFinishRate.AllPlanAvgFinishRate = planAvgFinishRate.ToString("#0.00");

                    double AllAvgPlanFinishRate = groupPlanFinishRate.Sum(x => x.sum) / (queryPlanCourseCount * querySumStuCount);
                    double DifPlanFinishRate = Math.Round(avgFinishRate, 2) - (double)planAvgFinishRate;
                    if (DifPlanFinishRate >= 0)
                        planFinishRate.FinishRateFlag = "超出";
                    else
                        planFinishRate.FinishRateFlag = "低于";

                    planFinishRate.ExceedPercent = Math.Abs(DifPlanFinishRate).ToString("#0.00");

                    //加入平均值
                    groupPlanFinishRate.Add(new { PlanID = (long)0, sum = AvgPlanFinishRate });
                    //重新排序
                    groupPlanFinishRate = groupPlanFinishRate.OrderByDescending(x => x.sum).ToList();
                    //查找平均值排名
                    int AvgPlanFinishRateRank = groupPlanFinishRate.FindIndex(x => x.PlanID == 0);
                    int FinishRankDif = AvgPlanFinishRateRank - FinishRank;

                    int TheoryGlobalResult = 0;
                    var RankResult = DifRank + FinishRankDif;
                    if (RankResult >= 0)
                        TheoryGlobalResult = 1;
                    #endregion

                    return new
                    {
                        code = 200,
                        result = new
                        {
                            planLearningInfo = planLearningInfo,
                            planFinishRate = planFinishRate,
                            TheoryGlobalResult = TheoryGlobalResult
                        },
                        message = "OK"
                    };
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

        #region 报告生成

        public object GetPersonnalReport(long userId, string userNumber, long planId, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryPlanF = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == planId).FirstOrDefault();
                    var planInfo = GetPersonalPlanInfo(userNumber, planId);

                    string theoryResult = "1", examResult = "1", taskResult = "1";
                    object theoryInfo = null, examInfo = null, taskInfo = null, endComment = null;
                    if (queryPlanF.course_flag == 1)
                    {
                        theoryInfo = GetPersonalTheoryReportResult(userNumber, planId);
                        string str = Newtonsoft.Json.JsonConvert.SerializeObject(theoryInfo);
                        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ReponseInfomation>(str);
                        theoryResult = obj.result.TheoryGlobalResult;
                    }

                    try
                    {
                        if (queryPlanF.exam_flag == 1)
                        {
                            var examUrl = PubMethod.ReadConfigJsonData("GetPersonExamReport");
                            var examFullUrl = examUrl + "userNumber=" + userNumber + "&planId=" + planId;
                            examInfo = client.GetRequest(examFullUrl).Result;
                            ReponseInfomation r = Newtonsoft.Json.JsonConvert.DeserializeObject<ReponseInfomation>(examInfo.ToString());
                            examResult = r.result.ExamGlobalResult;
                        }
                    }
                    catch (Exception)
                    {
                        examInfo = null;
                    }

                    try
                    {
                        if (queryPlanF.task_flag == 1)
                        {
                            var taskUrl = PubMethod.ReadConfigJsonData("GetPersonTaskReport");
                            var taskFullUrl = taskUrl + "userId=" + userId + "&planId=" + planId;
                            taskInfo = client.GetRequest(taskFullUrl).Result;
                        }
                    }
                    catch (Exception)
                    {
                        taskInfo = null;
                    }
                    try
                    {
                        var endCommentUrl = PubMethod.ReadConfigJsonData("EndComment");
                        var endCommentFullUrl = endCommentUrl + "level=" + theoryResult+ taskResult + examResult + "&div=1";
                        endComment = client.GetRequest(endCommentFullUrl).Result;
                    }
                    catch (Exception)
                    {
                        endComment = null;
                    }

                    return new { planInfo, theoryInfo, taskInfo, examInfo, endComment };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetPlanReport(long planId, IHttpClientHelper client)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryPlanF = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == planId).FirstOrDefault();
                    var planInfo = GetPlanReportResult(planId);

                    string theoryResult = "1", examResult = "1", taskResult = "1";
                    object theoryInfo = null, examInfo = null, taskInfo = null, endComment = null;
                    if (queryPlanF.course_flag == 1)
                    {
                        theoryInfo = GetPlanTheoryReportResult(planId);
                        string str = Newtonsoft.Json.JsonConvert.SerializeObject(theoryInfo);
                        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<ReponseInfomation>(str);
                        theoryResult = obj.result.TheoryGlobalResult;
                    }
                    try
                    {
                        if (queryPlanF.exam_flag == 1)
                        {
                            var examUrl = PubMethod.ReadConfigJsonData("GetPlanExamReport");
                            var examFullUrl = examUrl + "planId=" + planId;
                            examInfo = client.GetRequest(examFullUrl).Result;
                            ReponseInfomation r = Newtonsoft.Json.JsonConvert.DeserializeObject<ReponseInfomation>(examInfo.ToString());
                            examResult = r.result.ExamGlobalResult;
                        }
                    }
                    catch (Exception)
                    {
                        examInfo = null;
                    }

                    try
                    {
                        if (queryPlanF.task_flag == 1)
                        {
                            var taskUrl = PubMethod.ReadConfigJsonData("GetPlanTaskReport");
                            var taskFullUrl = taskUrl + "planId=" + planId;
                            taskInfo = client.GetRequest(taskFullUrl).Result;
                        }
                    }
                    catch (Exception)
                    {
                        taskInfo = null;
                    }
                    try
                    {
                        var endCommentUrl = PubMethod.ReadConfigJsonData("EndComment");
                        var endCommentFullUrl = endCommentUrl + "level=" + theoryResult + taskResult + examResult + "&div=2";
                        endComment = client.GetRequest(endCommentFullUrl).Result;
                    }
                    catch (Exception)
                    {
                        endComment = null;
                    }

                    return new { planInfo, theoryInfo, examInfo, taskInfo, endComment };
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
    public class ReponseInfomation
    {
        public string code { get; set; }
        public string message { get; set; }
        public ReponseResultValue result { get; set; }
    }
    public class ReponseResultValue
    {
        public string TheoryGlobalResult { get; set; }
        public string ExamGlobalResult { get; set; }
    }
    public class TheoryFinishRate
    {
        public string UserNumber { get; set; }
        public long PlanID { get; set; }
        public double Progress { get; set; }
    }
    public class EffectEvaluationPlan
    {
        public long ID { get; set; }
        public string Index { get; set; }
        public string PlanName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int StuCount { get; set; }
        public string PlanDesc { get; set; }
        public string CourseFlag { get; set; }
        public string TaskFlag { get; set; }
        public string ExamFalg { get; set; }
    }
    public class ReportResult
    {
        #region 基本信息
        /// <summary>
        /// 培训计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 培训计划开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 培训计划结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 学员人数
        /// </summary>
        public int StuCount { get; set; }
        /// <summary>
        /// 培训简介
        /// </summary>
        public string PlanDesc { get; set; }
        #endregion

        #region 理论学习

        #region 学习时长
        /// <summary>
        /// 总学习时长
        /// </summary>
        public string TheorySumLearningTime { get; set; }
        /// <summary>
        /// 学习时长排名
        /// </summary>
        public string TheoryLearningTimeRank { get; set; }
        /// <summary>
        /// 学习时长超过的人数百分比
        /// </summary>
        public string TheoryLearningTimeExceedPercent { get; set; }

        /// <summary>
        /// 培训计划下的总时长
        /// </summary>
        public string TheoryAllLearningTime { get; set; }
        /// <summary>
        /// 平均时长百分比
        /// </summary>
        public string TheoryAvgLearningTimePercent { get; set; }
        /// <summary>
        /// 平均学习时长
        /// </summary>
        public string TheoryAvgLearningTime { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TheoryLevelFlag { get; set; }
        /// <summary>
        /// 水平线小时数
        /// </summary>
        public string TheoryDifHours { get; set; }
        #endregion

        #region 完成率

        /// <summary>
        /// 理论学习完成率
        /// </summary>
        public string TheorySumFininshRate { get; set; }
        /// <summary>
        /// 完成率排名
        /// </summary>
        public string TheoryFinishRateRank { get; set; }
        /// <summary>
        /// 完成率超出百分比
        /// </summary>
        public string TheoryFinishRateExceedPercent { get; set; }
        /// <summary>
        /// 平均完成率
        /// </summary>
        public string TheoryAvgFinishRate { get; set; }
        /// <summary>
        /// 超出、低于标志
        /// </summary>
        public string TheoryRateLevelFlag { get; set; }
        /// <summary>
        /// 与平均值比较结果
        /// </summary>
        public string TheoryDifFinishRate { get; set; }

        #endregion

        /// <summary>
        /// 理论整体结果
        /// </summary>
        public string TheoryGlobalResult { get; set; }

        #endregion

        #region 模拟练习

        #region 练习时长

        /// <summary>
        /// 总练习时长
        /// </summary>
        public string TaskTrainSumTime { get; set; }
        /// <summary>
        /// 练习时长排名
        /// </summary>
        public string TaskTrainTimeRank { get; set; }
        /// <summary>
        /// 超出时长百分比
        /// </summary>
        public string TaskTrainTimeExceedPercent { get; set; }
        /// <summary>
        /// 平均练习时长
        /// </summary>
        public string TaskTrainAvgLearningTime { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TaskTrainLevelFlag { get; set; }
        /// <summary>
        /// 超出低于水平线小时数
        /// </summary>
        public string TaskTrainDifHours { get; set; }

        #endregion

        #region 完成率

        /// <summary>
        /// 总完成率
        /// </summary>
        public string TaskFinishRate { get; set; }
        /// <summary>
        /// 完成率排名
        /// </summary>
        public string TaskFinishRateRank { get; set; }
        /// <summary>
        /// 超出的完成率百分比
        /// </summary>
        public string TaskFinishRateExceedPercent { get; set; }
        /// <summary>
        /// 平均完成率
        /// </summary>
        public string TaskFinishAvgRate { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TaskFinishRateFlag { get; set; }
        /// <summary>
        /// 超出、低于的完成率
        /// </summary>
        public string TaskFinishDifRate { get; set; }

        #endregion

        #region 通过率

        /// <summary>
        /// 科目通过率
        /// </summary>
        public string TaskPassRate { get; set; }
        /// <summary>
        /// 通过率排名
        /// </summary>
        public string TaskPassRateRank { get; set; }
        /// <summary>
        /// 超出的通过率百分比
        /// </summary>
        public string TaskPassRatePercent { get; set; }
        /// <summary>
        /// 平均通过率
        /// </summary>
        public string TaskPassAvgRate { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TaskPassRateFlag { get; set; }
        /// <summary>
        /// 超出、低于的通过率
        /// </summary>
        public string TaskPassDifRate { get; set; }

        #endregion

        /// <summary>
        /// 模拟训练整体结果，0：未达标，1：达标
        /// </summary>
        public string TaskGlobalResult { get; set; }

        #endregion

        #region 考试测评

        #region 正确率
        /// <summary>
        /// 考试测评正确率
        /// </summary>
        public string ExamCorrectRate { get; set; }
        /// <summary>
        /// 学员排名
        /// </summary>
        public string ExamCorrectRateRank { get; set; }
        /// <summary>
        /// 超出的百分比
        /// </summary>
        public string ExamCorrectRateExceedPercent { get; set; }
        /// <summary>
        /// 平均正确率
        /// </summary>
        public string ExamCorrectAvgRate { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string ExamCorrectLevelFlag { get; set; }
        /// <summary>
        /// 与平均值比较的结果
        /// </summary>
        public string ExamDifCorrectRate { get; set; }

        #endregion

        #region 通过率

        /// <summary>
        /// 科目通过率
        /// </summary>
        public string ExamPassRate { get; set; }
        /// <summary>
        /// 通过率排名
        /// </summary>
        public string ExamPassRateRank { get; set; }
        /// <summary>
        /// 超出的通过率百分比
        /// </summary>
        public string ExamPassRatePercent { get; set; }
        /// <summary>
        /// 平均通过率
        /// </summary>
        public string ExamPassAvgRate { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string ExamPassRateFlag { get; set; }
        /// <summary>
        /// 超出、低于的通过率
        /// </summary>
        public string ExamPassDifRate { get; set; }

        #endregion

        #region 知识点掌握度

        /// <summary>
        /// 理论掌握程度好的
        /// </summary>
        public List<KnowledgeDegree> TheoryGoodDegree { get; set; }
        /// <summary>
        /// 理论掌握程度差的
        /// </summary>
        public List<KnowledgeDegree> TheoryBadDegree { get; set; }
        /// <summary>
        /// 实践掌握程度好的
        /// </summary>
        public List<KnowledgeDegree> TaskGoodDegree { get; set; }
        /// <summary>
        /// 实践掌握程度差的
        /// </summary>
        public List<KnowledgeDegree> TaskBadDegree { get; set; }

        #endregion

        /// <summary>
        /// 考试测评整体结果
        /// </summary>
        public string ExamGlobalResult { get; set; }

        #endregion

        #region 建议和结语
        public string EndComment { get; set; }

        #endregion
    }
    public class KnowledgeDegree
    {
        public string TagName { get; set; }
        public string Degree { get; set; }
    }
    public class PlanStuLearningTime
    {
        public string user_id { get; set; }
        public double learningHours { get; set; }
    }

    public class PlanInfo
    {
        public string PlanName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int StuCount { get; set; }
        public string PlanDesc { get; set; }
    }
    public class PlanLearningInfo
    {
        /// <summary>
        /// 学员数量
        /// </summary>
        public string StuCount { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string StuExceedCountFlag { get; set; }
        /// <summary>
        /// 超出低于人数
        /// </summary>
        public string StuExceedCount { get; set; }
        /// <summary>
        /// 理论时长
        /// </summary>
        public string LearningHours { get; set; }
        /// <summary>
        /// 平均时长
        /// </summary>
        public string AvgLearningHours { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string LearningExceedFlag { get; set; }
        /// <summary>
        /// 超出低于时长
        /// </summary>
        public string LearningExceedHours { get; set; }
        /// <summary>
        /// 所有的总时长
        /// </summary>
        public string AllLearningSumHours { get; set; }
        /// <summary>
        /// 所有的平均时长
        /// </summary>
        public string AllLearningAvgHours { get; set; }
    }

    public class PlanFinishRate
    {
        /// <summary>
        /// 理论课程数量
        /// </summary>
        public string TheoryCourseCount { get; set; }
        /// <summary>
        /// 完成的课程数量
        /// </summary>
        public string TheoryFinishedCourseCount { get; set; }
        /// <summary>
        /// 平均完成率
        /// </summary>
        public string AvgFinishRate { get; set; }
        /// <summary>
        /// 超出低于标识
        /// </summary>
        public string FinishRateFlag { get; set; }
        /// <summary>
        /// 超出低于百分比
        /// </summary>
        public string ExceedPercent { get; set; }
        /// <summary>
        /// 所有的完成率平均值
        /// </summary>
        public string AllPlanAvgFinishRate { get; set; }
    }

}
