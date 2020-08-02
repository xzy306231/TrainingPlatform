using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Examination.BLL
{
    public class StatisticData
    {
        /// <summary>
        /// 考试提交率分析
        /// </summary>
        /// <returns></returns>
        public object GetExamSubmitRate(pf_examinationContext db)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            join s in db.t_statistic_texam on e.id equals s.exam_id
                            where e.delete_flag == 0 && s.delete_flag == 0
                            select new { e.exam_name, s.exam_num, s.total_num };
                List<ExaminationSubmitRate> list = new List<ExaminationSubmitRate>();
                foreach (var item in query)
                {
                    if (item.total_num == null || item.total_num == 0 || item.exam_num == null)
                        continue;
                    list.Add(new ExaminationSubmitRate()
                    {
                        ExamName = item.exam_name,
                        Rate = ((int)item.exam_num * 100 / (double)item.total_num).ToString("#0.00")
                    });
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

        /// <summary>
        /// 科目分析
        /// </summary>
        /// <returns></returns>
        public object GetSubjectCorrectRate(pf_examinationContext db)
        {
            try
            {
                var query = from s in db.t_training_subject
                            join j in db.t_statistic_subject on s.id equals j.subject_id
                            where s.delete_flag == 0 && j.delete_flag == 0
                            select new { s.train_name, j.right_rate };


                var querySubjectGroup = query.GroupBy(x => new { x.train_name }).Select(a => new { a.Key.train_name, avg = a.Average(x => x.right_rate) });
                List<ExaminationSubjectRate> list = new List<ExaminationSubjectRate>();
                foreach (var item in querySubjectGroup)
                {
                    list.Add(new ExaminationSubjectRate() { SubjectName = item.train_name, Rate = ((decimal)item.avg).ToString("#0.00") });
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

        /// <summary>
        /// 获取理论考试正确率排名
        /// </summary>
        /// <returns></returns>
        public object GetTheoryExamCorrectRateRank(pf_examinationContext db)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            join r in db.t_examination_record on e.id equals r.examination_id
                            join p in db.t_test_papers on e.id equals p.examination_id
                            where e.delete_flag == 0 && e.exam_div == "1" && r.delete_flag == 0
                            select new { r.user_number, r.user_name, r.department, r.score, p.exam_score };
                List<ExamRank> list = new List<ExamRank>();
                foreach (var item in query)
                {
                    float? nScore = 0;
                    if (item.score != null)
                        nScore = item.score;
                    list.Add(new ExamRank()
                    {

                        UserNumber = item.user_number,
                        UserName = item.user_name,
                        Department = item.department,
                        Score = nScore,//学员得分
                        PaperScore = item.exam_score
                    });
                }
                //分组取平均值
                var temp = list.GroupBy(x => new { x.UserNumber, x.UserName, x.Department }).Select(a => new { a.Key.UserNumber, a.Key.UserName, a.Key.Department, avgScore = a.Average(x => x.Score), avgPaperScore = a.Average(x => x.PaperScore) });
                List<ExamRank> listRate = new List<ExamRank>();
                foreach (var item in temp)
                {
                    listRate.Add(new ExamRank()
                    {
                        UserName = item.UserName,
                        Department = item.Department,
                        CorrectRate = (decimal)item.avgScore / (decimal)item.avgPaperScore
                    });
                }
                var ranks = listRate.OrderByDescending(x => x.CorrectRate).Take(10);
                List<ExamRank> listResult = new List<ExamRank>();
                foreach (var item in ranks)
                {
                    listResult.Add(new ExamRank()
                    {
                        UserName = item.UserName,
                        Department = item.Department,
                        strCorrectRate = ((decimal)item.CorrectRate * 100).ToString("#0.00")
                    });
                }
                return new { code = 200, result = listResult, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 获取实践考试通过率排名
        /// </summary>
        /// <returns></returns>
        public object GetTaskPassRateRank(pf_examinationContext db)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            join r in db.t_examination_record on e.id equals r.examination_id
                            where e.delete_flag == 0 && r.delete_flag == 0 && e.exam_div == "2"
                            group r by new { r.user_number, r.user_name, r.department }
                            into grp
                            select new
                            {
                                grp.Key.user_name,
                                grp.Key.department,
                                avg = grp.Average(x => x.pass_rate)
                            };
                List<ExamRank> list = new List<ExamRank>();
                foreach (var item in query.Take(10).OrderByDescending(x => x.avg))
                {
                    list.Add(new ExamRank()
                    {
                        UserName = item.user_name,
                        Department = item.department,
                        PassRate = ((decimal)item.avg).ToString("#0.00")
                    });
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

        /// <summary>
        /// 获取理论考试知识点掌握度
        /// </summary>
        /// <returns></returns>
        public object GetExamKnowledge(pf_examinationContext db)
        {
            try
            {
                var queryTheory = from e in db.t_statistic_exam_knowledge
                                  where e.delete_flag == 0
                                  group e by new { e.know_name } into grp
                                  select new { grp.Key.know_name, avg = grp.Average(x => x.know_rate) };
                List<KnowlegeRate> knowlegeRates = new List<KnowlegeRate>();
                foreach (var item in queryTheory)
                {
                    knowlegeRates.Add(new KnowlegeRate()
                    {
                        Tag = item.know_name,
                        Rate = item.avg
                    });
                }

                var queryTask = from t in db.t_statistic_subject_knowledge
                                where t.delete_flag == 0
                                group t by new { t.know_name } into grp
                                select new { grp.Key.know_name, avg = grp.Average(x => x.know_rate) };
                foreach (var item in queryTask)
                {
                    knowlegeRates.Add(new KnowlegeRate()
                    {
                        Tag = item.know_name,
                        Rate = item.avg
                    });
                }

                //分组取平均
                var result = knowlegeRates.GroupBy(x => new { x.Tag }).Select(a => new { a.Key.Tag, avg = a.Average(x => x.Rate) });
                List<KnowlegeRate> listRates = new List<KnowlegeRate>();
                foreach (var item in result)
                {
                    listRates.Add(new KnowlegeRate()
                    {
                        Tag = item.Tag,
                        strRate = ((decimal)item.avg).ToString("#0.00")
                    });
                }
                return new { code = 200, result = listRates, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
    }

    public class ExamRank
    {
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public float? Score { get; set; }
        public int? PaperScore { get; set; }
        public decimal? CorrectRate { get; set; }
        public string strCorrectRate { get; set; }
        public string PassRate { get; set; }
    }

    public class ExaminationSubmitRate
    {
        public string ExamName { get; set; }
        public string Rate { get; set; }
    }

    public class ExaminationSubjectRate
    {
        public string SubjectName { get; set; }
        public string Rate { get; set; }
    }
    public class KnowlegeRate
    {
        public string Tag { get; set; }
        public decimal? Rate { get; set; }
        public string strRate { get; set; }
    }
}
