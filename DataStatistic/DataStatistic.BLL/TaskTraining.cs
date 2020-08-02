using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace DataStatistic.BLL
{
    public class TaskTraining
    {
        public object GetTaskRank(pf_datastatisticContext db)
        {
            try
            {
                var query = from r in db.t_statistic_taskpass_rank
                            where r.delete_flag == 0
                            orderby r.pass_rate descending
                            select r;
                return new { code = 200, result = query.Take(10).ToList(), message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetTaskKnowledge(pf_datastatisticContext db)
        {
            try
            {
                var query = from t in db.t_statistic_task_knowledge
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
}
