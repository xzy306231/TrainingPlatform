using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace DataStatistic.BLL
{
    public class TheoryTeaching
    {
        public object GetLearningTimeRank(pf_datastatisticContext db, DicModel dicDepartment)
        {
            try
            {
                var query = from s in db.t_statistic_learningtime_rank
                            where s.delete_flag == 0
                            orderby s.learning_time descending
                            select s;
                List<LearningTimeRank> list = new List<LearningTimeRank>();
                int i = 0;
                double nFirstLearningTime = 0;
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
                    double hour = (int)item.learning_time / 3600.00;
                    ++i;
                    if (i == 1)
                        nFirstLearningTime = hour;
                    list.Add(new LearningTimeRank()
                    {
                        Index = i,
                        UserName = item.user_name,
                        Department = strDepartment,
                        LearningTime = hour.ToString("#0.00"),
                        Rate = ((int)(hour * 100 / nFirstLearningTime)).ToString("#0.00")
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

        public object GetTheoryKnowledge(pf_datastatisticContext db)
        {
            try
            {
                var query = from t in db.t_statistic_theory_knowledge
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

    public class LearningTimeRank
    {
        public int Index { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public string LearningTime { get; set; }
        public string Rate { get; set; }
    }
}
