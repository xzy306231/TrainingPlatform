using System;
using System.IO;
using System.Linq;

namespace DataStatistic.BLL
{
    public class HomePage
    {
        public object GetStuStatisticData(pf_datastatisticContext db, string userNumber)
        {
            try
            {
                var query = from s in db.t_statistic_studata
                            where s.delete_flag == 0 && s.stu_number == userNumber
                            select s;
                var queryF = query.FirstOrDefault();
                if (queryF == null)
                    return new { code = 200, message = "OK" };

                StatisticStuData statisticStuData = new StatisticStuData();
                statisticStuData.LearningTime = queryF.learning_time;
                statisticStuData.TrainingTime = queryF.training_time;
                statisticStuData.TrainingNum = queryF.training_num;
                statisticStuData.CompleteTNum = queryF.complete_tnum;
                statisticStuData.CourseNum = queryF.course_num;
                statisticStuData.CompleteCNum = queryF.complete_cnum;
                statisticStuData.TaskNum = queryF.task_num;
                statisticStuData.CompleteTaskNum = queryF.complete_tasknum;
                statisticStuData.ThExamrate = queryF.th_examrate;
                statisticStuData.TaskExamrate = queryF.task_examrate;
                statisticStuData.SumLearningTime = queryF.learning_totaltime;

                var queryData = from d in db.t_statistic_tdata
                                where d.delete_flag == 0
                                select d;
                var queryDataF = queryData.FirstOrDefault();
                if (queryDataF != null)
                {
                    statisticStuData.AvgLearningTime = decimal.Round(queryDataF.avg_learningtime / 3600, 2);
                    statisticStuData.SumTaskTime = queryDataF.sum_tasktime;
                    statisticStuData.AvgTaskTime = queryDataF.avg_tasktime;
                }
                return new { code = 200, result = statisticStuData, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
    }
    public class StatisticStuData
    {
        /// <summary>
        /// 理论学习时长
        /// </summary>
        public decimal LearningTime { get; set; }
        /// <summary>
        /// 模拟训练时长
        /// </summary>
        public decimal TrainingTime { get; set; }
        /// <summary>
        /// 培训总次数
        /// </summary>
        public int TrainingNum { get; set; }
        /// <summary>
        /// 完成培训数
        /// </summary>
        public int CompleteTNum { get; set; }
        /// <summary>
        /// 参与课程数
        /// </summary>
        public int CourseNum { get; set; }
        /// <summary>
        /// 完成课程数
        /// </summary>
        public int CompleteCNum { get; set; }
        /// <summary>
        /// 参与训练数
        /// </summary>
        public int TaskNum { get; set; }
        /// <summary>
        /// 完成训练任务数
        /// </summary>
        public int CompleteTaskNum { get; set; }
        /// <summary>
        /// 理论考试通过率
        /// </summary>
        public decimal ThExamrate { get; set; }
        /// <summary>
        /// 实践考试通过率
        /// </summary>
        public decimal? TaskExamrate { get; set; }
        /// <summary>
        /// 学习总时长
        /// </summary>
        public decimal SumLearningTime { get; set; }
        /// <summary>
        /// 平均学习时长
        /// </summary>
        public decimal AvgLearningTime { get; set; }
        /// <summary>
        /// 总训练时间
        /// </summary>
        public decimal SumTaskTime { get; set; }
        /// <summary>
        /// 平均训练时间
        /// </summary>
        public decimal AvgTaskTime { get; set; }
    }
}
