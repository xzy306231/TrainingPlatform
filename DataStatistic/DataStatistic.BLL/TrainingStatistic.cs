using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace DataStatistic.BLL
{
    /// <summary>
    /// 培训统计
    /// </summary>
    public class TrainingStatistic
    {
        public object GetPFStatisticData(pf_datastatisticContext db)
        {
            try
            {
                var query = from s in db.t_statistic_tdata
                            where s.delete_flag == 0
                            select s;
                var queryF = query.FirstOrDefault();
                if (queryF == null)
                    return new { code = 200, msg = "OK" };
                PFStatisticData data = new PFStatisticData();
                data.TrainingTNum = queryF.training_tnum;
                data.TrainingPNum = queryF.training_pnum;
                data.InTrainingPNum = queryF.intraining_pnum;
                data.ComTrainingPNum = queryF.comtraining_pnum;
                data.ComTrainingRate = queryF.comtraining_rate;
                data.LearningComRate = queryF.learning_comrate;
                data.MaxLearningTime = (queryF.max_learningtime / 3600).ToString("#0.00");
                data.MinLearningTime = (queryF.min_learningtime / 3600).ToString("#0.00");
                data.AvgLearningTime = (queryF.avg_learningtime / 3600).ToString("#0.00");

                if (queryF.sum_learningtime != 0)
                {
                    data.SumLearningTime = queryF.sum_learningtime;
                    data.AvgLearningTimePercent = (queryF.avg_learningtime * 100 / queryF.sum_learningtime).ToString("#0.00");
                    data.MaxLearningTimePercent = (queryF.max_learningtime * 100 / queryF.sum_learningtime).ToString("#0.00");
                    data.MinLearningTimePercent = (queryF.min_learningtime * 100 / queryF.sum_learningtime).ToString("#0.00");
                }
                else
                {
                    data.SumLearningTime = 0;
                    data.AvgLearningTimePercent = "0.00";
                    data.MaxLearningTimePercent = "0.00";
                    data.MinLearningTimePercent = "0.00";
                }
                data.TaskComRate = queryF.task_comrate;
                data.TaskPassRate = queryF.task_passrate;
                data.AvgTaskTime = queryF.avg_tasktime;
                data.SubjectComRate = queryF.subject_comrate;
                data.SubjectPassRate = queryF.subject_passrate;
                data.ExamSubRate = queryF.exam_subrate;
                data.ExamRightRate = queryF.exam_rightrate;
                data.ExamPassRate = queryF.exam_passrate;
                return new { code = 200, result = data, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetTrainingTimesByYear(pf_datastatisticContext db, string year)
        {
            try
            {
                var query = from s in db.t_statistic_trainingtime
                            where s.delete_flag == 0 && s.t_year == year
                            select s;
                List<TrainingTime> trainingTimes = new List<TrainingTime>();
                foreach (var item in query)
                {
                    trainingTimes.Add(new TrainingTime()
                    {
                        Year = item.t_year,
                        Month = item.t_month,
                        TrainNum = item.training_num
                    });
                }
                return new { code = 200, result = trainingTimes, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetTrainingYearTimes(pf_datastatisticContext db)
        {
            try
            {
                var query = from s in db.t_statistic_trainingtime
                            where s.delete_flag == 0
                            group s by s.t_year into g
                            select new { Year = g.Key, Total = g.Sum(x => x.training_num) };
                List<TrainingTime> trainingTimes = new List<TrainingTime>();
                foreach (var item in query)
                {
                    trainingTimes.Add(new TrainingTime()
                    {
                        Year = item.Year,
                        TrainNum = item.Total
                    });
                }
                return new { code = 200, result = trainingTimes, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }


    }
    public class PFStatisticData
    {
        /// <summary>
        /// 总培训次数
        /// </summary>
        public int TrainingTNum { get; set; }
        /// <summary>
        /// 培训总人次
        /// </summary>
        public int TrainingPNum { get; set; }
        /// <summary>
        /// 正在参与培训人次
        /// </summary>
        public int InTrainingPNum { get; set; }
        /// <summary>
        /// 完成培训人次
        /// </summary>
        public int ComTrainingPNum { get; set; }
        /// <summary>
        /// 培训完成率
        /// </summary>
        public decimal ComTrainingRate { get; set; }
        /// <summary>
        /// 学习完成率
        /// </summary>
        public decimal LearningComRate { get; set; }
        /// <summary>
        /// 最长学习时间
        /// </summary>
        public string MaxLearningTime { get; set; }
        /// <summary>
        /// 最长学习时长百分比
        /// </summary>
        public string MaxLearningTimePercent { get; set; }
        /// <summary>
        /// 最短学习时间
        /// </summary>
        public string MinLearningTime { get; set; }
        /// <summary>
        /// 最短学习时间百分比
        /// </summary>
        public string MinLearningTimePercent { get; set; }
        /// <summary>
        /// 平均学习时间
        /// </summary>
        public string AvgLearningTime { get; set; }
        /// <summary>
        /// 平均学习时长百分比
        /// </summary>
        public string AvgLearningTimePercent { get; set; }
        /// <summary>
        /// 总的学习时长
        /// </summary>
        public decimal SumLearningTime { get; set; }
        /// <summary>
        /// 任务完成率
        /// </summary>
        public decimal TaskComRate { get; set; }
        /// <summary>
        /// 任务通过率
        /// </summary>
        public decimal TaskPassRate { get; set; }
        /// <summary>
        /// 平均任务时长
        /// </summary>
        public decimal AvgTaskTime { get; set; }
        /// <summary>
        /// 科目完成率
        /// </summary>
        public decimal SubjectComRate { get; set; }
        /// <summary>
        /// 科目通过率
        /// </summary>
        public decimal SubjectPassRate { get; set; }
        /// <summary>
        /// 考试提交率
        /// </summary>
        public decimal ExamSubRate { get; set; }
        /// <summary>
        /// 考试正确率
        /// </summary>
        public decimal ExamRightRate { get; set; }
        /// <summary>
        /// 考试通过率
        /// </summary>
        public decimal ExamPassRate { get; set; }

    }

    public class TrainingTime
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public int TrainNum { get; set; }
    }
}
