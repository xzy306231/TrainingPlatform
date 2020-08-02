using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace DataStatistic.BLL
{
    public class Exam
    {
        public object GetExamCorrectRank(pf_datastatisticContext db, DicModel dicDepartment)
        {
            try
            {
                var query = from r in db.t_statistic_exam_correct_rank
                            where r.delete_flag == 0
                            orderby r.correct_rate descending
                            select r;
                List<ExamCorrectRate> list = new List<ExamCorrectRate>();
                foreach (var item in query.Take(10))
                {
                    string strDepartment = "";//部门
                    try
                    {
                        if (!string.IsNullOrEmpty(item.department))
                            strDepartment = dicDepartment.Result.Find(x => x.DicCode == item.department).CodeDsc;
                    }
                    catch (Exception)
                    { }
                    list.Add(new ExamCorrectRate
                    {
                        user_name = item.user_name,
                        department = strDepartment,
                        correct_rate = item.correct_rate
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

        public object GetExamPassRate(pf_datastatisticContext db, DicModel dicDepartment)
        {
            try
            {
                var query = from r in db.t_statistic_exam_pass_rank
                            where r.delete_flag == 0
                            orderby r.pass_rate descending
                            select r;
                List<ExamPassRate> list = new List<ExamPassRate>();
                foreach (var item in query.Take(10))
                {
                    string strDepartment = "";//部门
                    try
                    {
                        if (!string.IsNullOrEmpty(item.department))
                            strDepartment = dicDepartment.Result.Find(x => x.DicCode == item.department).CodeDsc;
                    }
                    catch (Exception)
                    { }

                    list.Add(new ExamPassRate
                    {
                        user_name = item.user_name,
                        department = strDepartment,
                        pass_rate = item.pass_rate
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

        public object GetExamKnowledge(pf_datastatisticContext db)
        {
            try
            {
                var query = from t in db.t_statistic_exam_knowledge
                            where t.delete_flag == 0
                            select t;
                return new { code = 200, result = query.ToList(), message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
    }

    public class ExamCorrectRate
    {
        public string user_name { get; set; }
        public string department { get; set; }
        public decimal? correct_rate { get; set; }
    }

    public class ExamPassRate
    {
        public string user_name { get; set; }
        public string department { get; set; }
        public decimal? pass_rate { get; set; }
    }
}
