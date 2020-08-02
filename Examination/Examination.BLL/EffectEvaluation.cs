using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Examination.BLL
{
    public class EffectEvaluation
    {
        public object GetPersonalExamReportResult(pf_examinationContext db, string userNumber, long planId)
        {
            try
            {
                ReportResult reportResult = new ReportResult();
                //查找计划下总人数
                var queryStuCount = (from r in db.t_examination_record
                                     where r.delete_flag == 0 && r.plan_id == planId
                                     select r).Count();
                if (queryStuCount == 0)
                    return new { code = 200, result = "", msg = "Ok" };
                //查找理论考试人数
                var queryTheoryExamCount = (from e in db.t_examination_manage
                                            join r in db.t_examination_record on e.id equals r.examination_id
                                            where e.exam_div == "1" && r.plan_id == planId && e.delete_flag == 0
                                            select e.id).Count();
                //查找培训计划下所有的理论考试管理ID集合
                var queryTheoryExamList = (from e in db.t_examination_manage
                                           join r in db.t_examination_record on e.id equals r.examination_id
                                           where e.exam_div == "1" && r.plan_id == planId && e.delete_flag == 0
                                           select e.id).Distinct().ToList();
                //查找培训计划下所有的实践考试管理ID集合
                var queryTaskExamList = (from e in db.t_examination_manage
                                         join r in db.t_examination_record on e.id equals r.examination_id
                                         where e.exam_div == "2" && r.plan_id == planId && e.delete_flag == 0
                                         select e.id).Distinct().ToList();
                //查找实践考试人数
                var queryTaskExamCount = (from e in db.t_examination_manage
                                          join r in db.t_examination_record on e.id equals r.examination_id
                                          where e.exam_div == "2" && r.plan_id == planId && e.delete_flag == 0
                                          select e.id).Count();

                //查找培训计划下训练科目的数量
                var querySubjectCount = (from t in db.t_training_task
                                         join s in db.t_training_subject on t.id equals s.task_id
                                         where t.delete_flag == 0 && queryTaskExamList.Contains((long)t.examination_id)
                                         select s).Count();
                List<CorrectRate> correctRates = new List<CorrectRate>();
                #region 正确率
                List<CorrectRate> correctTheoryRates = new List<CorrectRate>();
                /******************理论正确率Start******************/
                var queryTheoryCorrect = from e in db.t_examination_manage
                                         join t in db.t_test_papers on e.id equals t.examination_id
                                         join r in db.t_examination_record on e.id equals r.examination_id
                                         where r.delete_flag == 0 && r.plan_id == planId && e.exam_div == "1"
                                         select new { r.user_number, r.score, t.exam_score };
                foreach (var item in queryTheoryCorrect)
                {
                    float nscore = 0;
                    if (item.score != null)
                        nscore = (float)item.score;
                    string strRate = (nscore / (int)item.exam_score * 100).ToString("#0.00");
                    correctTheoryRates.Add(new CorrectRate
                    {
                        UserNumber = item.user_number,
                        correctrate = strRate
                    });
                }
                //分组、求和
                var groupcorrectTheoryRates = correctTheoryRates.GroupBy(x => new { x.UserNumber }).Select(x => new { x.Key.UserNumber, sum = x.Sum(a => decimal.Parse(a.correctrate)) }).ToList();

                decimal stuTheorySum = 0;
                if (groupcorrectTheoryRates.Find(x => x.UserNumber == userNumber) != null)
                {
                    stuTheorySum = groupcorrectTheoryRates.Find(x => x.UserNumber == userNumber).sum;
                }
                var allStuTheorySum = groupcorrectTheoryRates.Sum(x => x.sum);
                //理论个人正确率
                if (queryTheoryExamList.Count != 0)
                {
                    reportResult.ExamTheoryStuCorrectRate = (stuTheorySum / queryTheoryExamList.Count).ToString("#0.00");
                }
                else
                    reportResult.ExamTheoryStuCorrectRate = "0";
                //理论平均正确率
                if (queryTheoryExamList.Count * queryTheoryExamCount != 0)
                {
                    reportResult.ExamTheoryAvgCorrectRate = (allStuTheorySum / (queryTheoryExamList.Count * queryTheoryExamCount)).ToString("#0.00");
                }
                else
                    reportResult.ExamTheoryAvgCorrectRate = "0";

                correctRates.AddRange(correctTheoryRates);
                /******************理论正确率End******************/

                /******************实践正确率Start******************/
                List<CorrectRate> correctTaskRates = new List<CorrectRate>();
                var queryTaskCorrect = (from e in db.t_examination_manage
                                        join r in db.t_examination_record on e.id equals r.examination_id
                                        join g in db.t_task_log on r.id equals g.record_id
                                        where e.exam_div == "2" && r.plan_id == planId && r.delete_flag == 0
                                        select new { r.user_number, score = int.Parse(g.exam_result) }).GroupBy(x => new { x.user_number }).Select(x => new { x.Key.user_number, sum = x.Sum(a => a.score) }).ToList();
                foreach (var item in queryTaskCorrect)
                {
                    string strRate = (item.sum * 100 / (double)querySubjectCount).ToString("#0.00");
                    correctTaskRates.Add(new CorrectRate
                    {
                        UserNumber = item.user_number,
                        correctrate = strRate
                    });
                }
                //分组，求和
                var groupcorrectTaskRates = correctTaskRates.GroupBy(x => new { x.UserNumber }).Select(x => new { x.Key.UserNumber, sum = x.Sum(a => decimal.Parse(a.correctrate)) }).ToList();
                decimal stuTaskSum = 0;
                if (groupcorrectTaskRates.Find(x => x.UserNumber == userNumber) != null)
                {
                    stuTaskSum = groupcorrectTaskRates.Find(x => x.UserNumber == userNumber).sum;
                }
                var allStuTaskSum = groupcorrectTaskRates.Sum(x => x.sum);
                //实践个人正确率
                if (queryTaskExamList.Count != 0)
                {
                    reportResult.ExamTaskStuCorrectRate = (stuTaskSum / queryTaskExamList.Count).ToString("#0.00");
                }
                else
                    reportResult.ExamTaskStuCorrectRate = "0";
                //实践平均正确率
                if (queryTaskExamList.Count * queryTaskExamCount != 0)
                {
                    reportResult.ExamTaskAvgCorrectRate = (allStuTaskSum / (queryTaskExamList.Count * queryTaskExamCount)).ToString("#0.00");
                }
                else
                    reportResult.ExamTaskAvgCorrectRate = "0";

                correctRates.AddRange(correctTaskRates);
                /******************实践正确率End******************/


                //分组
                var groupcorrectRates = correctRates.GroupBy(x => new { x.UserNumber }).Select(x => new { x.Key.UserNumber, avg = x.Average(a => float.Parse(a.correctrate)) }).ToList();
                //降序
                groupcorrectRates = groupcorrectRates.OrderByDescending(x => x.avg).ToList();

                //正确率
                if (groupcorrectRates.Find(x => x.UserNumber == userNumber) != null)
                    reportResult.ExamCorrectRate = groupcorrectRates.Find(x => x.UserNumber == userNumber).avg.ToString();
                else
                    reportResult.ExamCorrectRate = "0";

                //正确率排名
                int CorrectRateRank = 0;
                if (groupcorrectRates.FindIndex(x => x.UserNumber == userNumber) != -1)
                {
                    CorrectRateRank = groupcorrectRates.FindIndex(x => x.UserNumber == userNumber) + 1;
                    reportResult.ExamCorrectRateRank = CorrectRateRank.ToString();
                }
                else
                    reportResult.ExamCorrectRateRank = "0";

                //超出的学生百分比
                if (groupcorrectRates.FindIndex(x => x.UserNumber == userNumber) != -1)
                {
                    var pc = groupcorrectRates.Count - (groupcorrectRates.FindIndex(x => x.UserNumber == userNumber) + 1);
                    var pm = groupcorrectRates.Count;
                    //正确率超出的百分比
                    reportResult.ExamCorrectRateExceedPercent = (pc * 100 / pm).ToString("#0.00");
                }
                else
                    reportResult.ExamCorrectRateExceedPercent = "0";

                //与平均值比较结果
                float difExam = 0;
                if (groupcorrectRates.Find(x => x.UserNumber == userNumber) != null)
                {
                    difExam = groupcorrectRates.Find(x => x.UserNumber == userNumber).avg - groupcorrectRates.Average(x => x.avg);
                    if (difExam >= 0)
                        reportResult.ExamCorrectLevelFlag = "超出";
                    else
                        reportResult.ExamCorrectLevelFlag = "低于";
                }
                else
                    reportResult.ExamCorrectLevelFlag = "超出";

                //比较的差值
                reportResult.ExamDifCorrectRate = Math.Abs(difExam).ToString("#0.00");

                //平均正确率
                float avgCorrectRate = 0;
                if (groupcorrectRates.Count > 0)
                {
                    avgCorrectRate = groupcorrectRates.Average(x => x.avg);
                    reportResult.ExamCorrectAvgRate = avgCorrectRate.ToString("#0.00");
                }
                else
                    reportResult.ExamCorrectAvgRate = "0";

                //添加平均值
                groupcorrectRates.Add(new { UserNumber = "", avg = avgCorrectRate });
                //重新排序
                groupcorrectRates = groupcorrectRates.OrderByDescending(x => x.avg).ToList();
                //查找出索引值（排名）
                int avgcorrectrate = groupcorrectRates.FindIndex(x => x.UserNumber == "" && x.avg == avgCorrectRate) + 1;
                //与平均排名比较
                int correctRankResult = avgcorrectrate - CorrectRateRank;

                #endregion


                List<CorrectRate> passRate = new List<CorrectRate>();

                #region 通过率

                /******************理论通过率Start******************/
                List<CorrectRate> passTheoryRate = new List<CorrectRate>();
                //分组累加
                var queryTheoryPass = (from e in db.t_examination_manage
                                       join r in db.t_examination_record on e.id equals r.examination_id
                                       where r.delete_flag == 0 && r.plan_id == planId && e.exam_div == "1" && e.delete_flag == 0
                                       select new { r.user_number, pass = (int)r.pass_flag }).GroupBy(x => new { x.user_number }).Select(x => new { x.Key.user_number, sum = x.Sum(a => (int)a.pass) }).ToList();
                foreach (var item in queryTheoryPass)
                {
                    double d = (double)item.sum * 100 / queryTheoryExamList.Count;
                    passTheoryRate.Add(new CorrectRate
                    {
                        UserNumber = item.user_number,
                        correctrate = d.ToString("#0.00")
                    });
                }
                //个人通过考试的数量
                if (passTheoryRate.Find(x => x.UserNumber == userNumber) != null)
                    reportResult.ExamTheoryStuPassRate = passTheoryRate.Find(x => x.UserNumber == userNumber).correctrate;
                else
                    reportResult.ExamTheoryStuPassRate = "0";

                //所有考试通过的数量
                var querySumPassCount = (from r in db.t_examination_record
                                         where r.delete_flag == 0 && r.plan_id == planId && r.pass_flag == 1
                                         select r).Count();
                if (queryStuCount != 0)
                    reportResult.ExamTheoryAvgPassRate = (querySumPassCount * 100 / queryStuCount).ToString("#0.00");
                else
                    reportResult.ExamTheoryAvgPassRate = "0";

                /******************理论通过率End/******************/


                /******************实践通过率Start*****************/
                var queryTaskPass = from e in db.t_examination_manage
                                    join r in db.t_examination_record on e.id equals r.examination_id
                                    where e.delete_flag == 0 && e.exam_div == "2" && r.delete_flag == 0 && r.plan_id == planId
                                    select new { r.user_number, r.pass_rate };
                //分组降序排列
                var groupPassRate = (from g in queryTaskPass
                                     group g by new { g.user_number } into grp
                                     select new { grp.Key.user_number, avg = grp.Average(x => (decimal)x.pass_rate) }).OrderByDescending(arg => arg.avg).ToList();
                //个人实践通过率
                if (groupPassRate.Find(x => x.user_number == userNumber) != null)
                    reportResult.ExamTaskStuPassRate = groupPassRate.Find(x => x.user_number == userNumber).avg.ToString("#0.00");
                else
                    reportResult.ExamTaskStuPassRate = "0";

                //实践平均通过率
                if (groupPassRate.Count != 0)
                {
                    reportResult.ExamTaskAvgPassRate = groupPassRate.Average(x => x.avg).ToString("#0.00");
                }
                else
                    reportResult.ExamTaskAvgPassRate = "0";
                /*****************实践通过率End******************/

                //将理论通过率添加至分组集中
                foreach (var item in passTheoryRate)
                {
                    groupPassRate.Add(new { user_number = item.UserNumber, avg = decimal.Parse(item.correctrate) });
                }
                //再次分组排序
                var grouppassRate = groupPassRate.GroupBy(x => new { x.user_number }).Select(x => new { x.Key.user_number, avg = x.Average(a => a.avg) }).ToList();

                int passRateRankResult = 0;
                if (grouppassRate.Count > 0)
                {
                    //通过率
                    if (grouppassRate.Find(arg => arg.user_number == userNumber) != null)
                        reportResult.ExamPassRate = grouppassRate.Find(arg => arg.user_number == userNumber).avg.ToString("#0.00");
                    else
                        reportResult.ExamPassRate = "0";
                    //通过率排名
                    int PassRateRank = 0;
                    if (grouppassRate.FindIndex(arg => arg.user_number == userNumber) != -1)
                    {
                        PassRateRank = grouppassRate.FindIndex(arg => arg.user_number == userNumber) + 1;
                        reportResult.ExamPassRateRank = PassRateRank.ToString();
                    }
                    else
                        reportResult.ExamPassRateRank = "0";

                    //超出百分比
                    if (grouppassRate.FindIndex(x => x.user_number == userNumber) != -1)
                    {
                        var pc1 = grouppassRate.Count - (grouppassRate.FindIndex(x => x.user_number == userNumber) + 1);
                        var pm1 = grouppassRate.Count;
                        reportResult.ExamPassRatePercent = (pc1 * 100 / pm1).ToString("#0.00");
                    }
                    else
                        reportResult.ExamPassRatePercent = "0";

                    //与平均值比较结果
                    decimal difPass = 0;
                    if (grouppassRate.Find(arg => arg.user_number == userNumber) != null)
                    {
                        difPass = grouppassRate.Find(arg => arg.user_number == userNumber).avg - grouppassRate.Average(x => x.avg);
                        if (difPass >= 0)
                            reportResult.ExamPassRateFlag = "超出";
                        else
                            reportResult.ExamPassRateFlag = "低于";
                    }
                    else
                        reportResult.ExamPassRateFlag = "超出";

                    //差值比较结果
                    reportResult.ExamPassDifRate = Math.Abs(difPass).ToString("#0.00");

                    //平均通过率
                    decimal avgpassRate = grouppassRate.Average(x => x.avg);
                    reportResult.ExamPassAvgRate = avgpassRate.ToString("#0.00");
                    //添加平均值
                    grouppassRate.Add(new { user_number = "", avg = avgpassRate });
                    //重新排序
                    grouppassRate = grouppassRate.OrderByDescending(x => x.avg).ToList();
                    //查找出索引值（排名）
                    int avgpassRateRank = grouppassRate.FindIndex(x => x.user_number == "" && x.avg == avgpassRate) + 1;
                    //与平均排名比较
                    passRateRankResult = avgpassRateRank - PassRateRank;
                }

                #endregion

                #region 知识点掌握度
                //理论
                KnowledgeDegreeList knowledgeTheoryDegreeList = new KnowledgeDegreeList();
                //实践
                KnowledgeDegreeList knowledgeTaskDegreeList = new KnowledgeDegreeList();

                //查找理论考试下所有知识点掌握度
                List<KnowledgeDegree> knowledgeTheoryDegrees = new List<KnowledgeDegree>();
                var TheoryRecordList = (from r in db.t_examination_record
                                        join e in db.t_examination_manage on r.examination_id equals e.id
                                        where r.delete_flag == 0 && r.plan_id == planId && r.user_number == userNumber && e.exam_div == "1"
                                        select r).ToList();
                var examination_theory_list = TheoryRecordList.Select(x => x.examination_id).ToList();
                var record_theory_list = TheoryRecordList.Select(x => x.id).ToList(); ;
                var queryTheoryExamKnowledge = (from ek in db.t_statistic_exam_knowledge
                                                where ek.delete_flag == 0 && examination_theory_list.Contains(ek.exam_id)
                                                select ek).ToList();
                foreach (var item in queryTheoryExamKnowledge)
                {
                    //知识点的总分值
                    var queryList = (from p in db.t_test_papers
                                     join q in db.t_questions on p.id equals q.test_paper_id
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id
                                     where examination_theory_list.Contains(p.examination_id) && r.knowledge_tag_id == item.know_id
                                     select new { q.id, q.question_score }).ToList();
                    var sumScore = queryList.Sum(x => x.question_score);

                    //知识点的得分
                    var qId = queryList.Select(x => x.id).ToList();
                    var queryStuScoreList = (from t in db.t_answer_log
                                             where t.delete_flag == 0 && record_theory_list.Contains((long)t.record_id) && qId.Contains((long)t.item_id)
                                             select new { t.score }).ToList();
                    var stuSumScore = queryStuScoreList.Sum(x => x.score);
                    var rate = stuSumScore * 100 / (double)sumScore;

                    knowledgeTheoryDegrees.Add(new KnowledgeDegree
                    {
                        TagName = item.know_name,
                        Rate = (double)rate,
                        Degree = ((double)rate).ToString("#0.00")
                    });
                }
                var groupknowledgeTheoryDegrees = knowledgeTheoryDegrees.GroupBy(x => new { x.TagName }).Select(arg => new { arg.Key.TagName, avg = arg.Average(x => x.Rate) }).OrderByDescending(x => x.avg).ToList();
                if (groupknowledgeTheoryDegrees.Count >= 10)//知识点超过十个
                {
                    List<KnowledgeDegree> TheoryGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TheoryBadDegree = new List<KnowledgeDegree>();
                    //掌握程度好的
                    var goodlist = groupknowledgeTheoryDegrees.Take(5).ToList();
                    foreach (var item in goodlist)
                    {
                        TheoryGoodDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                    }
                    reportResult.TheoryGoodDegree = TheoryGoodDegree;
                    //掌握程度差的
                    var badlist = groupknowledgeTheoryDegrees.TakeLast(5).ToList();
                    foreach (var item in badlist)
                    {
                        TheoryBadDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                    }
                    reportResult.TheoryBadDegree = TheoryBadDegree;
                }
                else
                {
                    List<KnowledgeDegree> TheoryGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TheoryBadDegree = new List<KnowledgeDegree>();
                    if (groupknowledgeTheoryDegrees.Count % 2 == 0)//偶数
                    {
                        //掌握程度好的
                        var goodlist = groupknowledgeTheoryDegrees.Take(groupknowledgeTheoryDegrees.Count / 2).ToList();
                        foreach (var item in goodlist)
                        {
                            TheoryGoodDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TheoryGoodDegree = TheoryGoodDegree;

                        //掌握程度差的
                        var badlist = groupknowledgeTheoryDegrees.TakeLast(groupknowledgeTheoryDegrees.Count / 2).ToList();
                        foreach (var item in badlist)
                        {
                            TheoryBadDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TheoryBadDegree = TheoryBadDegree;
                    }
                    else//奇数
                    {
                        //掌握程度好的
                        var goodlist = groupknowledgeTheoryDegrees.Take((groupknowledgeTheoryDegrees.Count + 1) / 2).ToList();
                        foreach (var item in goodlist)
                        {
                            TheoryGoodDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TheoryGoodDegree = TheoryGoodDegree;

                        //掌握程度差的
                        var badlist = groupknowledgeTheoryDegrees.TakeLast((groupknowledgeTheoryDegrees.Count - 1) / 2).ToList();
                        foreach (var item in badlist)
                        {
                            TheoryBadDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TheoryBadDegree = TheoryBadDegree;
                    }
                }

                //查找实践考试下所有知识点掌握度
                List<KnowledgeDegree> knowledgeTaskDegrees = new List<KnowledgeDegree>();
                var TaskRecordList = (from r in db.t_examination_record
                                      join e in db.t_examination_manage on r.examination_id equals e.id
                                      where r.delete_flag == 0 && r.plan_id == planId && r.user_number == userNumber && e.exam_div == "2"
                                      select r).ToList();
                var examination_task_list = TaskRecordList.Select(x => x.examination_id).ToList();
                var record_task_list = TaskRecordList.Select(x => x.id).ToList(); ;
                var queryTaskExamKnowledge = (from ek in db.t_statistic_subject_knowledge
                                              where ek.delete_flag == 0 && examination_task_list.Contains(ek.exam_id)
                                              select ek).ToList();
                foreach (var item in queryTaskExamKnowledge)
                {
                    //知识点的总分值
                    var queryList = (from p in db.t_training_task
                                     join q in db.t_training_subject on p.id equals q.task_id
                                     join r in db.t_subject_knowledge_ref on q.id equals r.knowledge_tag_id
                                     where examination_task_list.Contains(p.examination_id) && r.knowledge_tag_id == item.know_id
                                     select new { q.id, score = 100 }).ToList();
                    var sumScore = queryList.Sum(x => x.score);

                    //知识点的得分
                    var qId = queryList.Select(x => x.id).ToList();
                    var queryStuScoreList = (from t in db.t_task_log
                                             where t.delete_flag == 0 && record_task_list.Contains((long)t.record_id) && qId.Contains((long)t.subject_id) && t.exam_result == "1"
                                             select new { t.exam_result, score = 100 }).ToList();
                    var stuSumScore = queryStuScoreList.Sum(x => x.score);
                    double? rate = 0;
                    if (sumScore != 0)
                        rate = stuSumScore * 100 / (double)sumScore;

                    knowledgeTaskDegrees.Add(new KnowledgeDegree()
                    {
                        TagName = item.know_name,
                        Rate = rate,
                        Degree = ((double)rate).ToString("#0.00")
                    });
                }
                var groupknowledgeTaskDegrees = knowledgeTaskDegrees.GroupBy(x => new { x.TagName }).Select(arg => new { arg.Key.TagName, avg = arg.Average(x => x.Rate) }).OrderByDescending(x => x.avg).ToList();
                if (groupknowledgeTaskDegrees.Count >= 10)
                {
                    List<KnowledgeDegree> TaskGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TaskBadDegree = new List<KnowledgeDegree>();
                    var goodlist = groupknowledgeTaskDegrees.Take(5).ToList();
                    foreach (var item in goodlist)
                    {
                        TaskGoodDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                    }
                    reportResult.TaskGoodDegree = TaskGoodDegree;

                    var badlist = groupknowledgeTaskDegrees.TakeLast(5).ToList();
                    foreach (var item in badlist)
                    {
                        TaskBadDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                    }
                    reportResult.TaskBadDegree = TaskBadDegree;
                }
                else//少于10个
                {
                    List<KnowledgeDegree> TaskGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TaskBadDegree = new List<KnowledgeDegree>();
                    if (groupknowledgeTaskDegrees.Count % 2 == 0)//偶数
                    {
                        var goodlist = groupknowledgeTaskDegrees.Take(groupknowledgeTaskDegrees.Count / 2).ToList();
                        foreach (var item in goodlist)
                        {
                            TaskGoodDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TaskGoodDegree = TaskGoodDegree;

                        var badlist = groupknowledgeTaskDegrees.TakeLast(groupknowledgeTaskDegrees.Count / 2).ToList();
                        foreach (var item in badlist)
                        {
                            TaskBadDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TaskBadDegree = TaskBadDegree;
                    }
                    else//奇数
                    {
                        var goodlist = groupknowledgeTaskDegrees.Take((groupknowledgeTaskDegrees.Count + 1) / 2).ToList();
                        foreach (var item in goodlist)
                        {
                            TaskGoodDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TaskGoodDegree = TaskGoodDegree;

                        var badlist = groupknowledgeTaskDegrees.TakeLast((groupknowledgeTaskDegrees.Count - 1) / 2).ToList();
                        foreach (var item in badlist)
                        {
                            TaskBadDegree.Add(new KnowledgeDegree { TagName = item.TagName, Degree = ((double)item.avg).ToString("#0.00") });
                        }
                        reportResult.TaskBadDegree = TaskBadDegree;
                    }
                }

                #endregion

                //总体情况
                double globalExamResult = (correctRankResult + passRateRankResult) / 2.0;
                if (globalExamResult >= 0)
                    reportResult.ExamGlobalResult = "1";
                else
                    reportResult.ExamGlobalResult = "0";

                return new { code = 200, Result = reportResult, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetPlanExamReportResult(pf_examinationContext db, long planId)
        {
            try
            {
                #region 正确率
                ExamCorrectRate examCorrectRate = new ExamCorrectRate();
                var queryExam = db.t_examination_record.Where(x => x.plan_id == planId && x.delete_flag == 0).Select(x => x.examination_id).Distinct().ToList();
                if (queryExam.Count == 0)
                    return new { code = 200, result = "", msg = "OK" };

                var queryCorrectRate = from e in db.t_statistic_texam
                                       where e.delete_flag == 0 && queryExam.Contains(e.exam_id)
                                       select e.avg_rightrate;
                var listCorrectList = queryCorrectRate.ToList();
                var avgCorrectRate = listCorrectList.Sum() / listCorrectList.Count;

                //本次平均正确率
                examCorrectRate.AvgExamCorrectRate = ((decimal)avgCorrectRate).ToString("#0.00");

                //计算所有的平均正确率
                var queryExamList = db.t_examination_manage.Where(x => x.delete_flag == 0).Select(x => x.id).ToList();
                var queryAllCorrectRate = from e in db.t_statistic_texam
                                          where e.delete_flag == 0 && queryExamList.Contains(e.exam_id)
                                          select e.avg_rightrate;
                var listAllCorrectList = queryAllCorrectRate.ToList();
                var avgAllCorrectRate = listAllCorrectList.Sum() / listAllCorrectList.Count;

                //正确率差值
                var CorrectDifRate = avgCorrectRate - avgAllCorrectRate;
                if (CorrectDifRate >= 0)
                    examCorrectRate.ExceedCorrectRateFlag = "超出";
                else
                    examCorrectRate.ExceedCorrectRateFlag = "低于";

                examCorrectRate.AllAvgExamCorrectRate = ((decimal)avgAllCorrectRate).ToString("#0.00");
                examCorrectRate.ExceedCorrectRate = Math.Abs((decimal)CorrectDifRate).ToString("#0.00");

                /*******本次理论正确率**********/
                var queryTheoryExam = from e in db.t_examination_manage
                                      join r in db.t_examination_record on e.id equals r.examination_id
                                      where e.delete_flag == 0 && e.exam_div == "1" && r.plan_id == planId
                                      select e.id;
                var listTheoryCorrectList = queryTheoryExam.Distinct().ToList();

                var queryTheoryCorrectRate = (from e in db.t_statistic_texam
                                              where e.delete_flag == 0 && listTheoryCorrectList.Contains(e.exam_id)
                                              select e.avg_rightrate).ToList();
                decimal? TheoryAvgCorrectRate = 0;
                if (queryTheoryCorrectRate.Count != 0)
                {
                    TheoryAvgCorrectRate = queryTheoryCorrectRate.Sum() / queryTheoryCorrectRate.Count;
                    examCorrectRate.TheoryAvgCorrectRate = ((decimal)TheoryAvgCorrectRate).ToString("#0.00");
                }
                else
                    examCorrectRate.TheoryAvgCorrectRate = "0";

                var queryTheoryExamList = db.t_examination_manage.Where(x => x.delete_flag == 0 && x.exam_div == "1").Select(x => x.id).ToList();
                var queryAllTheoryCorrectRate = from e in db.t_statistic_texam
                                                where e.delete_flag == 0 && queryTheoryExamList.Contains(e.exam_id)
                                                select e.avg_rightrate;
                var listAllTheoryCorrectRate = queryAllTheoryCorrectRate.ToList();
                var avgAllTheoryCorrectRate = listAllTheoryCorrectRate.Sum() / listAllTheoryCorrectRate.Count;
                //理论正确率差值
                var AllTheoryCorrectDifRate = TheoryAvgCorrectRate - avgAllTheoryCorrectRate;
                if (AllTheoryCorrectDifRate >= 0)
                    examCorrectRate.TheoryExceedCorrectRateFlag = "超出";
                else
                    examCorrectRate.TheoryExceedCorrectRateFlag = "低于";
                examCorrectRate.TheoryAllAvgCorrectRate = ((decimal)avgAllTheoryCorrectRate).ToString("#0.00");
                examCorrectRate.TheoryExceedCorrectRate = Math.Abs((decimal)AllTheoryCorrectDifRate).ToString("#0.00");

                /*******本次实践正确率**********/
                var queryTaskExam = from e in db.t_examination_manage
                                    join r in db.t_examination_record on e.id equals r.examination_id
                                    where e.delete_flag == 0 && e.exam_div == "2" && r.plan_id == planId
                                    select e.id;
                var listTaskCorrectList = queryTaskExam.Distinct().ToList();

                var queryTaskCorrectRate = (from e in db.t_statistic_texam
                                            where e.delete_flag == 0 && listTaskCorrectList.Contains(e.exam_id)
                                            select e.avg_rightrate).ToList();
                decimal? TaskAvgCorrectRate = 0;
                if (queryTaskCorrectRate.Count() != 0)
                {
                    TaskAvgCorrectRate = queryTaskCorrectRate.Sum() / queryTaskCorrectRate.Count;
                    examCorrectRate.TaskAvgCorrectRate = ((decimal)TaskAvgCorrectRate).ToString("#0.00");
                }
                else
                    examCorrectRate.TaskAvgCorrectRate = "0";

                var queryTaskExamList = db.t_examination_manage.Where(x => x.delete_flag == 0 && x.exam_div == "2").Select(x => x.id).ToList();
                var queryAllTaskCorrectRate = from e in db.t_statistic_texam
                                              where e.delete_flag == 0 && queryTaskExamList.Contains(e.exam_id)
                                              select e.avg_rightrate;
                var listAllTaskCorrectRate = queryAllTaskCorrectRate.ToList();
                decimal? avgAllTaskCorrectRate = 0;
                if (listAllTaskCorrectRate.Count != 0)
                    avgAllTaskCorrectRate = listAllTaskCorrectRate.Sum() / listAllTaskCorrectRate.Count;
                else
                    avgAllTaskCorrectRate = 0;
                //实践正确率差值
                var AllTaskCorrectDifRate = TaskAvgCorrectRate - avgAllTaskCorrectRate;
                if (AllTaskCorrectDifRate >= 0)
                    examCorrectRate.TaskExceedCorrectRateFlag = "超出";
                else
                    examCorrectRate.TaskExceedCorrectRateFlag = "低于";
                examCorrectRate.TaskAllAvgCorrectRate = ((decimal)avgAllTaskCorrectRate).ToString("#0.00");
                examCorrectRate.TaskExceedCorrectRate = Math.Abs((decimal)AllTaskCorrectDifRate).ToString("#0.00");
                #endregion

                #region 通过率

                ExamPassRate examPassRate = new ExamPassRate();
                var queryPassRate = from e in db.t_statistic_texam
                                    where e.delete_flag == 0 && queryExam.Contains(e.exam_id)
                                    select e.pass_rate;
                var listPassList = queryPassRate.ToList();
                var avgPassRate = listPassList.Sum() / listPassList.Count;
                //本次平均通过率
                examPassRate.AvgExamPassRate = ((decimal)avgPassRate).ToString("#0.00");

                //计算所有的平均通过率
                var queryAllPassRate = from e in db.t_statistic_texam
                                       where e.delete_flag == 0 && queryExamList.Contains(e.exam_id)
                                       select e.pass_rate;
                var listAllPassList = queryAllPassRate.ToList();
                var avgAllPassRate = listAllPassList.Sum() / listAllPassList.Count;

                //通过率差值
                var PassDifRate = avgPassRate - avgAllPassRate;
                if (PassDifRate >= 0)
                    examPassRate.ExceedPassRateFlag = "超出";
                else
                    examPassRate.ExceedPassRateFlag = "低于";
                examPassRate.ExceedPassRate = Math.Abs((decimal)PassDifRate).ToString("#0.00");
                examPassRate.AllAvgExamPassRate = ((decimal)avgAllPassRate).ToString("#0.00");

                /*******本次理论通过率**********/
                var queryTheoryPassRate = (from e in db.t_statistic_texam
                                           where e.delete_flag == 0 && listTheoryCorrectList.Contains(e.exam_id)
                                           select e.pass_rate).ToList();
                decimal? TheoryAvgPassRate = 0;
                if (queryTheoryPassRate.Count != 0)
                {
                    TheoryAvgPassRate = queryTheoryPassRate.Sum() / queryTheoryPassRate.Count;
                    examPassRate.TheoryAvgPassRate = ((decimal)TheoryAvgPassRate).ToString("#0.00");
                }
                else
                    examPassRate.TheoryAvgPassRate = "0";

                var queryAllTheoryPassRate = from e in db.t_statistic_texam
                                             where e.delete_flag == 0 && queryTheoryExamList.Contains(e.exam_id)
                                             select e.pass_rate;
                var listAllTheoryPassRate = queryAllTheoryPassRate.ToList();
                var avgAllTheoryPassRate = listAllTheoryPassRate.Sum() / listAllTheoryPassRate.Count;
                //理论通过率差值
                var AllTheoryPassDifRate = TheoryAvgPassRate - avgAllTheoryPassRate;
                if (AllTheoryPassDifRate >= 0)
                    examPassRate.TheoryExceedPassRateFlag = "超出";
                else
                    examPassRate.TheoryExceedPassRateFlag = "低于";
                examPassRate.TheoryExceedPassRate = Math.Abs((decimal)AllTheoryPassDifRate).ToString("#0.00");
                examPassRate.TheoryAllAvgPassRate = ((decimal)avgAllTheoryPassRate).ToString("#0.00");

                /*******本次实践通过率**********/
                var queryTaskPassRate = (from e in db.t_statistic_texam
                                         where e.delete_flag == 0 && listTaskCorrectList.Contains(e.exam_id)
                                         select e.pass_rate).ToList();
                decimal? TaskAvgPassRate = 0;
                if (queryTaskPassRate.Count != 0)
                {
                    TaskAvgPassRate = queryTaskPassRate.Sum() / queryTaskPassRate.Count;
                    examPassRate.TaskAvgPassRate = ((decimal)TaskAvgPassRate).ToString("#0.00");
                }
                else
                    examPassRate.TaskAvgPassRate = "0";

                var queryAllTaskPassRate = from e in db.t_statistic_texam
                                           where e.delete_flag == 0 && queryTaskExamList.Contains(e.exam_id)
                                           select e.pass_rate;
                var listAllTaskPassRate = queryAllTaskPassRate.ToList();
                decimal? avgAllTaskPassRate = 0;
                if (listAllTaskPassRate.Count != 0)
                {
                    avgAllTaskPassRate = listAllTaskPassRate.Sum() / listAllTaskPassRate.Count;
                }
                //实践通过率差值
                var AllTaskPassDifRate = TaskAvgPassRate - avgAllTaskPassRate;
                if (AllTaskPassDifRate >= 0)
                    examPassRate.TaskExceedPassRateFlag = "超出";
                else
                    examPassRate.TaskExceedPassRateFlag = "低于";
                examPassRate.TaskExceedPassRate = Math.Abs((decimal)AllTaskPassDifRate).ToString("#0.00");
                examPassRate.TaskAllAvgPassRate = ((decimal)avgAllTaskPassRate).ToString("#0.00");

                //考试测评整体结果
                var GlobalResult = CorrectDifRate + PassDifRate;
                string examGlobalResult = "1";
                if (GlobalResult >= 0)
                    examGlobalResult = "1";
                else
                    examGlobalResult = "0";
                #endregion

                #region 知识点掌握度
                KnowledgeDegreeList knowledgeDegreeList = new KnowledgeDegreeList();
                /*********理论知识点*********/
                var queryTheoryKnowledge = (from r in db.t_examination_record
                                            join e in db.t_examination_manage on r.examination_id equals e.id
                                            join k in db.t_statistic_exam_knowledge on e.id equals k.exam_id
                                            where e.delete_flag == 0 && e.exam_div == "1" && r.plan_id == planId && r.delete_flag == 0
                                            select k).Distinct();
                List<KnowledgeDegree> listTheory = new List<KnowledgeDegree>();
                foreach (var item in queryTheoryKnowledge)
                {
                    decimal d = 0;
                    if (item.know_rate != null)
                        d = (decimal)item.know_rate;
                    listTheory.Add(new KnowledgeDegree
                    {
                        TagName = item.know_name,
                        Degree = d.ToString()
                    });
                }

                //分组取平均
                var listTheoryGroup = listTheory.GroupBy(x => new { x.TagName }).Select(x => new { x.Key.TagName, avg = x.Average(a => decimal.Parse(a.Degree)) }).OrderByDescending(x => x.avg).ToList();
                if (listTheoryGroup.Count >= 10)//大于十个知识点只取前五个与后五个
                {
                    List<KnowledgeDegree> TheoryGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TheoryBadDegree = new List<KnowledgeDegree>();
                    //取前五个
                    var goodlist = listTheoryGroup.Take(5).ToList();
                    for (int i = 0; i < goodlist.Count; i++)
                    {
                        TheoryGoodDegree.Add(new KnowledgeDegree
                        {
                            TagName = goodlist[i].TagName,
                            Degree = goodlist[i].avg.ToString()
                        });
                    }
                    knowledgeDegreeList.TheoryGoodDegree = TheoryGoodDegree;
                    //取最后五个
                    var badlist = listTheoryGroup.TakeLast(5).ToList();
                    for (int i = 0; i < badlist.Count; i++)
                    {
                        TheoryBadDegree.Add(new KnowledgeDegree
                        {
                            TagName = badlist[i].TagName,
                            Degree = badlist[i].avg.ToString()
                        });
                    }
                    knowledgeDegreeList.TheoryBadDegree = TheoryBadDegree;
                }
                else
                {
                    List<KnowledgeDegree> TheoryGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TheoryBadDegree = new List<KnowledgeDegree>();
                    if (listTheoryGroup.Count % 2 == 0)//偶数
                    {
                        var goodlist = listTheoryGroup.Take(listTheoryGroup.Count / 2).ToList();
                        for (int i = 0; i < goodlist.Count; i++)
                        {
                            TheoryGoodDegree.Add(new KnowledgeDegree
                            {
                                TagName = goodlist[i].TagName,
                                Degree = goodlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TheoryGoodDegree = TheoryGoodDegree;

                        var badlist = listTheoryGroup.TakeLast(listTheoryGroup.Count / 2).ToList();
                        for (int i = 0; i < badlist.Count; i++)
                        {
                            TheoryBadDegree.Add(new KnowledgeDegree
                            {
                                TagName = badlist[i].TagName,
                                Degree = badlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TheoryBadDegree = TheoryBadDegree;
                    }
                    else//奇数
                    {
                        var goodlist = listTheoryGroup.Take((listTheoryGroup.Count + 1) / 2).ToList();
                        for (int i = 0; i < goodlist.Count; i++)
                        {
                            TheoryGoodDegree.Add(new KnowledgeDegree
                            {
                                TagName = goodlist[i].TagName,
                                Degree = goodlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TheoryGoodDegree = TheoryGoodDegree;

                        var badlist = listTheoryGroup.TakeLast((listTheoryGroup.Count - 1) / 2).ToList();
                        for (int i = 0; i < badlist.Count; i++)
                        {
                            TheoryBadDegree.Add(new KnowledgeDegree
                            {
                                TagName = badlist[i].TagName,
                                Degree = badlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TheoryBadDegree = TheoryBadDegree;
                    }
                }

                /*********实践知识点*********/
                var queryTaskKnowledge = (from r in db.t_examination_record
                                          join e in db.t_examination_manage on r.examination_id equals e.id
                                          join k in db.t_statistic_subject_knowledge on e.id equals k.exam_id
                                          where e.delete_flag == 0 && e.exam_div == "2" && r.plan_id == planId && r.delete_flag == 0
                                          select k).Distinct();
                List<KnowledgeDegree> listTask = new List<KnowledgeDegree>();
                foreach (var item in queryTaskKnowledge)
                {
                    decimal d = 0;
                    if (item.know_rate != null)
                        d = (decimal)item.know_rate;
                    listTask.Add(new KnowledgeDegree
                    {
                        TagName = item.know_name,
                        Degree = d.ToString()
                    });
                }
                //分组取平均
                var listTaskGroup = listTask.GroupBy(x => new { x.TagName }).Select(x => new { x.Key.TagName, avg = x.Average(a => decimal.Parse(a.Degree)) }).OrderByDescending(x => x.avg).ToList();
                if (listTaskGroup.Count >= 10)
                {
                    List<KnowledgeDegree> TaskGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TaskBadDegree = new List<KnowledgeDegree>();
                    //取前五个
                    var goodlist = listTaskGroup.Take(5).ToList();
                    for (int i = 0; i < goodlist.Count; i++)
                    {
                        TaskGoodDegree.Add(new KnowledgeDegree
                        {
                            TagName = goodlist[i].TagName,
                            Degree = goodlist[i].avg.ToString()
                        });
                    }
                    knowledgeDegreeList.TaskGoodDegree = TaskGoodDegree;
                    //取后五个
                    var badlist = listTaskGroup.TakeLast(5).ToList();
                    for (int i = 0; i < badlist.Count; i++)
                    {
                        TaskBadDegree.Add(new KnowledgeDegree
                        {
                            TagName = badlist[i].TagName,
                            Degree = badlist[i].avg.ToString()
                        });
                    }
                    knowledgeDegreeList.TaskBadDegree = TaskBadDegree;
                }
                else
                {
                    List<KnowledgeDegree> TaskGoodDegree = new List<KnowledgeDegree>();
                    List<KnowledgeDegree> TaskBadDegree = new List<KnowledgeDegree>();
                    if (listTaskGroup.Count % 2 == 0)//偶数
                    {
                        var goodlist = listTaskGroup.Take(listTaskGroup.Count / 2).ToList();
                        for (int i = 0; i < goodlist.Count; i++)
                        {
                            TaskGoodDegree.Add(new KnowledgeDegree
                            {
                                TagName = goodlist[i].TagName,
                                Degree = goodlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TaskGoodDegree = TaskGoodDegree;

                        var badlist = listTaskGroup.TakeLast(listTaskGroup.Count / 2).ToList();
                        for (int i = 0; i < badlist.Count; i++)
                        {
                            TaskBadDegree.Add(new KnowledgeDegree
                            {
                                TagName = badlist[i].TagName,
                                Degree = badlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TaskBadDegree = TaskBadDegree;
                    }
                    else//奇数
                    {
                        var goodlist = listTaskGroup.Take((listTaskGroup.Count + 1) / 2).ToList();
                        for (int i = 0; i < goodlist.Count; i++)
                        {
                            TaskGoodDegree.Add(new KnowledgeDegree
                            {
                                TagName = goodlist[i].TagName,
                                Degree = goodlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TaskGoodDegree = TaskGoodDegree;

                        var badlist = listTaskGroup.TakeLast((listTaskGroup.Count - 1) / 2).ToList();
                        for (int i = 0; i < badlist.Count; i++)
                        {
                            TaskBadDegree.Add(new KnowledgeDegree
                            {
                                TagName = badlist[i].TagName,
                                Degree = badlist[i].avg.ToString()
                            });
                        }
                        knowledgeDegreeList.TaskBadDegree = TaskBadDegree;
                    }
                }

                #endregion

                return new { code = 200, result = new { examCorrectRate = examCorrectRate, examPassRate = examPassRate, knowledgeDegreeList = knowledgeDegreeList, examGlobalResult = examGlobalResult }, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
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
        /// <summary>
        /// 理论平均正确率
        /// </summary>
        public string ExamTheoryAvgCorrectRate { get; set; }
        /// <summary>
        /// 理论个人正确率
        /// </summary>
        public string ExamTheoryStuCorrectRate { get; set; }
        /// <summary>
        /// 实践平均正确率
        /// </summary>
        public string ExamTaskAvgCorrectRate { get; set; }
        /// <summary>
        /// 实践个人正确率
        /// </summary>
        public string ExamTaskStuCorrectRate { get; set; }

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
        /// <summary>
        /// 个人理论通过率
        /// </summary>
        public string ExamTheoryStuPassRate { get; set; }
        /// <summary>
        /// 理论平均通过率
        /// </summary>
        public string ExamTheoryAvgPassRate { get; set; }
        /// <summary>
        /// 个人实践通过率
        /// </summary>
        public string ExamTaskStuPassRate { get; set; }
        /// <summary>
        /// 实践平均通过率
        /// </summary>
        public string ExamTaskAvgPassRate { get; set; }

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
        public double? Rate { get; set; }
    }

    public class ExamCorrectRate
    {
        public string AvgExamCorrectRate { get; set; }
        public string AllAvgExamCorrectRate { get; set; }
        public string ExceedCorrectRate { get; set; }
        public string ExceedCorrectRateFlag { get; set; }


        public string TheoryAvgCorrectRate { get; set; }
        public string TheoryAllAvgCorrectRate { get; set; }
        public string TheoryExceedCorrectRate { get; set; }
        public string TheoryExceedCorrectRateFlag { get; set; }


        public string TaskAvgCorrectRate { get; set; }
        public string TaskAllAvgCorrectRate { get; set; }
        public string TaskExceedCorrectRate { get; set; }
        public string TaskExceedCorrectRateFlag { get; set; }
    }
    public class ExamPassRate
    {
        public string AvgExamPassRate { get; set; }
        public string AllAvgExamPassRate { get; set; }
        public string ExceedPassRate { get; set; }
        public string ExceedPassRateFlag { get; set; }


        public string TheoryAvgPassRate { get; set; }
        public string TheoryAllAvgPassRate { get; set; }
        public string TheoryExceedPassRate { get; set; }
        public string TheoryExceedPassRateFlag { get; set; }


        public string TaskAvgPassRate { get; set; }
        public string TaskAllAvgPassRate { get; set; }
        public string TaskExceedPassRate { get; set; }
        public string TaskExceedPassRateFlag { get; set; }
    }

    public class KnowledgeDegreeList
    {

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

    }
}
