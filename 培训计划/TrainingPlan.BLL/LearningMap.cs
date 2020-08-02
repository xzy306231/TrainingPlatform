using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class LearningMap
{
    public object GetLearningMapInfo(pf_training_plan_v1Context db, string userNumber, long userId, IHttpClientHelper client)
    {
        try
        {
            //查找人员信息
            string strUrl = @"http://PEOPLEMANAGER-SERVICE/peoplemanager/v1/student/personalInfo?category=student&id=" + userId;
            string strResult = client.GetRequest(strUrl).Result;
            RemoteServiceResult peopleInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteServiceResult>(strResult);

            //查找这个人使用过的大纲
            var queryProgramList = (from s in db.t_trainingplan_stu
                                    join p in db.t_training_plan on s.trainingplan_id equals p.id
                                    join g in db.t_training_program on p.program_id equals g.id
                                    where s.delete_flag == 0 && s.user_number == userNumber && p.delete_flag == 0 && p.program_id != 0
                                    select new { p.id, g }).ToList();
            List<long> programList = new List<long>();
            foreach (var item in queryProgramList)
            {
                if (!programList.Contains(item.g.src_id))
                    programList.Add(item.g.src_id);
            }
            string PlaneType = "";
            List<GrowthPhase> phaseList = new List<GrowthPhase>();
            foreach (var item in programList)
            {
                GrowthPhase growthPhase = new GrowthPhase();
                //大纲信息
                string strProUrl = @"http://COURSE-SERVICE/course/v1/TrainingPlanFromProgram?programId=" + item;
                string strProResult = client.GetRequest(strProUrl).Result;
                ProgramCourseTask programInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramCourseTask>(strProResult);//反序列化

                string EndorsementType = "";
                bool EndorsementTypeFlag = false;

                bool PlaneTypeFlag = false;

                int SumDuration = 0;
                bool SumDurationFlag = false;

                bool TrainTimeFlag = false;
                int TrainTime = 0;

                bool UpDownTimesFlag = false;
                int UpDownTimes = 0;

                bool TechnicalGradeFlag = false;
                string TechnicalGrade = "";

                //进入条件
                if (peopleInfo.result.workInfos != null && peopleInfo.result.workInfos.Count > 0)
                {
                    SumDuration = (int)peopleInfo.result.workInfos[0].actualDuration;
                    if (peopleInfo.result.workInfos[0].actualDuration >= programInfo.SumDuration)
                        SumDurationFlag = true;

                    TrainTime = (int)peopleInfo.result.workInfos[0].trainingDuration;
                    if (peopleInfo.result.workInfos[0].trainingDuration >= programInfo.TrainTime)
                        TrainTimeFlag = true;

                    UpDownTimes = (int)peopleInfo.result.workInfos[0].actualFlightNumber;
                    if (peopleInfo.result.workInfos[0].actualFlightNumber >= programInfo.UpDownTimes)
                        UpDownTimesFlag = true;

                    TechnicalGrade = peopleInfo.result.workInfos[0].skillLevelValue;
                    if (peopleInfo.result.workInfos[0].skillLevelValue == programInfo.TechnicalGrade)
                        TechnicalGradeFlag = true;

                    PlaneType = peopleInfo.result.workInfos[0].airplaneModelValue;
                    if (peopleInfo.result.workInfos[0].airplaneModelValue == programInfo.PlaneType1)
                        PlaneTypeFlag = true;
                }

                if (peopleInfo.result.certificateInfos != null && peopleInfo.result.certificateInfos.Count > 0)
                {
                    var certificateInfo = new CertificateInfos();
                    certificateInfo.Name = programInfo.EndorsementType;
                    EndorsementType = peopleInfo.result.certificateInfos[0].Name;
                    if (peopleInfo.result.certificateInfos.Contains(certificateInfo))
                        EndorsementTypeFlag = true;
                }

                bool passFlag = false;
                if (EndorsementTypeFlag && PlaneTypeFlag && SumDurationFlag && TrainTimeFlag && UpDownTimesFlag && TechnicalGradeFlag)
                    passFlag = true;

                ////////////////课程相关//////////////
                string strUrl1 = @"http://COURSE-SERVICE/course/v1/GetCourseIDByProgramID?id=" + item;
                string strResult1 = client.GetRequest(strUrl1).Result;
                List<long> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(strResult1);//返回原始课程ID集合

                //学时总数
                double sumCourseLearningTime = 0;
                //完成学时
                double coursefinishLearningTime = 0;
                //完成率
                double finishRate = 0;
                var queryCourseList = (from r in db.t_plan_course_task_exam_ref
                                       join c in db.t_course on r.content_id equals c.id
                                       join e in db.t_learning_record on r.id equals e.content_id
                                       where r.delete_flag == 0 && r.dif == "1" && list.Contains((long)c.src_id)
                                             && e.user_number == userNumber && c.delete_flag == 0 
                                       select new { c.course_name, c.learning_time, c.src_id, e.learning_progress, e.learning_sum_time }).Distinct().ToList();
                List<CourseData> dataList = new List<CourseData>();
                foreach (var item1 in queryCourseList)
                {
                    if (dataList.Find(x => x.src_id == item1.src_id) != null)//存在
                    {
                        var courseData = dataList.Find(x => x.src_id == item1.src_id);
                        string strprogress = "0";
                        if (!string.IsNullOrEmpty(item1.learning_progress))
                            strprogress = item1.learning_progress;
                        if (decimal.Parse(courseData.learning_progress) > decimal.Parse(strprogress))//比较大小
                            continue;//不做其他操作，结束本次循环
                        else
                        {
                            dataList.Remove(courseData);//删除
                                                        //重新添加
                            dataList.Add(new CourseData
                            {
                                course_name = item1.course_name,
                                learning_progress = item1.learning_progress,
                                learning_sum_time = (int)item1.learning_sum_time,
                                learning_time = item1.learning_time,
                                src_id = (long)item1.src_id
                            });
                        }
                    }
                    else//不存在，就加入集合
                    {
                        string strprogress = "0";
                        if (!string.IsNullOrEmpty(item1.learning_progress))
                            strprogress = item1.learning_progress;
                        dataList.Add(new CourseData
                        {
                            course_name = item1.course_name,
                            learning_progress = strprogress,
                            learning_sum_time = (int)item1.learning_sum_time,
                            learning_time = item1.learning_time,
                            src_id = (long)item1.src_id
                        });
                    }
                }

                List<ProgramCourse> programCourses = new List<ProgramCourse>();
                foreach (var item1 in dataList)
                {
                    double learningSumTime = 0.0;
                    sumCourseLearningTime += (double)item1.learning_time;
                    learningSumTime = item1.learning_sum_time;

                    double FinishLearningTime = learningSumTime/ 3600;
                    if (FinishLearningTime >= (double)item1.learning_time)
                        FinishLearningTime = (double)item1.learning_time;

                    double pro = 100*(FinishLearningTime  / (double)item1.learning_time);
                    finishRate += pro;
                    coursefinishLearningTime += FinishLearningTime;
                    programCourses.Add(new ProgramCourse
                    {
                        CourseName = item1.course_name,
                        LearningTime = item1.learning_time.ToString("#0.0"),
                        FinishLearningTime = FinishLearningTime.ToString("#0.0"),
                        FinishRate = pro.ToString("#0.0")         
                    });
                }
                //课程完成率
                double tempCourseFinishRate = 0;
                if (programCourses.Count != 0)
                    tempCourseFinishRate = finishRate / programCourses.Count;

                ////////////////任务相关//////////////
                string strUrl2 = @"http://COURSE-SERVICE/course/v1/GetTaskIDByProgramID?id=" + item;
                string strResult2 = client.GetRequest(strUrl2).Result;
                List<long> taskIDlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(strResult2);
                var queryTask = (from r in db.t_plan_course_task_exam_ref
                                 join t in db.t_task_bus on r.content_id equals t.id
                                 join o in db.t_task_bus_score on t.id equals o.task_bus_id
                                 where r.delete_flag == 0 && r.dif == "2" && taskIDlist.Contains(t.original_id) && o.user_id == userId
                                 select new { srcid = t.original_id, t.task_name, t.class_hour, o.result, o.start_time, o.end_time }).ToList();
                List<ProgramTask> programTasks = new List<ProgramTask>();
                foreach (var item2 in queryTask)
                {
                    double temptaskfinishLearningTime = 0;
                    if (item2.start_time != null && item2.end_time != null)
                        temptaskfinishLearningTime = ((DateTime)item2.end_time - (DateTime)item2.start_time).TotalSeconds;
                    programTasks.Add(new ProgramTask
                    {
                        SrcID = item2.srcid,
                        TaskName = item2.task_name,
                        LearningTime = item2.class_hour.ToString(),
                        TaskFinishLearningTime = temptaskfinishLearningTime,
                        TaskResult = (sbyte)item2.result//0：通过，1：未通过
                    });
                }
                //去重之后的任务集合
                List<ProgramTask> programDisTasks = new List<ProgramTask>();
                foreach (var task in programTasks)
                {
                    var obj = programDisTasks.Find(x => x.SrcID == task.SrcID);
                    if (obj != null)//存在
                    {
                        if (obj.TaskResult == 0)//任务结果为通过,结束本次循环
                            continue;
                        else
                        {
                            if (task.TaskResult == 0)
                            {
                                programDisTasks.Remove(obj);
                                programDisTasks.Add(task);
                            }
                        }
                    }
                    else//不存在
                    {
                        programDisTasks.Add(task);
                    }
                }

                //任务完成率
                double tempTaskPassRate = 0;
                //任务数量
                int taskSumCount = programDisTasks.Count;
                //通过数量
                int taskPassCount = programDisTasks.FindAll(x => x.TaskResult == 0).Count;
                //总时长
                double sumTaskLearningTime = programDisTasks.Sum(x => double.Parse(x.LearningTime));
                //完成的学习时长
                double taskfinishLearningTime = programDisTasks.Sum(x => x.TaskFinishLearningTime);
                if (taskSumCount != 0)
                    tempTaskPassRate = (double)taskPassCount * 100 / taskSumCount;

                //阶段完成率
                double tempPhaseFinishRate = 0;
                if (programCourses.Count != 0 && taskSumCount != 0)//都存在
                    tempPhaseFinishRate = (tempCourseFinishRate + tempTaskPassRate) / 2;
                else if (programCourses.Count == 0 && taskSumCount != 0)//不存在课程
                    tempPhaseFinishRate = tempTaskPassRate;
                else if (programCourses.Count != 0 && taskSumCount == 0)//不存在任务
                    tempPhaseFinishRate = tempCourseFinishRate;
                else
                    tempPhaseFinishRate = 0;

                double sumlearningTime = sumCourseLearningTime + sumTaskLearningTime;
                double sumFinishTime = coursefinishLearningTime + taskfinishLearningTime / 3600;

                growthPhase.RangePurpose = programInfo.RangePurpose;
                growthPhase.StandardRequest = programInfo.StandardRequest;
                growthPhase.GrowPhase = programInfo.TrainType;

                growthPhase.EndorsementType = programInfo.EndorsementType;
                growthPhase.CurEndorsementType = EndorsementType;
                growthPhase.EndorsementTypeFlag = EndorsementTypeFlag;

                growthPhase.PlaneType1 = programInfo.PlaneType1;
                growthPhase.CurPlaneType1 = PlaneType;
                growthPhase.PlaneTypeFlag = PlaneTypeFlag;

                growthPhase.TechnicalGrade = programInfo.TechnicalGrade;
                growthPhase.CurTechnicalGrade = TechnicalGrade;
                growthPhase.TechnicalGradeFlag = TechnicalGradeFlag;

                growthPhase.SumDuration = programInfo.SumDuration;
                growthPhase.CurSumDuration = SumDuration;
                growthPhase.SumDurationFlag = SumDurationFlag;

                growthPhase.TrainTime = programInfo.TrainTime;
                growthPhase.CurTrainTime = TrainTime;
                growthPhase.TrainTimeFlag = TrainTimeFlag;

                growthPhase.UpDownTimes = programInfo.UpDownTimes;
                growthPhase.CurUpDownTimes = UpDownTimes;
                growthPhase.UpDownTimesFlag = UpDownTimesFlag;

                growthPhase.OtherRemark = programInfo.OtherRemark;

                growthPhase.PassFlag = passFlag;

                //理论
                growthPhase.FinishRate = tempCourseFinishRate.ToString("#0");
                growthPhase.SumLearningTime = sumCourseLearningTime.ToString("#0.0");
                growthPhase.FinishLearningTime = coursefinishLearningTime.ToString("#0.0");
                growthPhase.programCourses = programCourses;

                //任务
                growthPhase.TaskPassRate = tempTaskPassRate.ToString("#0");//通过率
                growthPhase.TaskSumCount = taskSumCount.ToString();
                growthPhase.TaskPassCount = taskPassCount.ToString();
                growthPhase.TaskSumLearningTime = sumTaskLearningTime.ToString();
                growthPhase.programTasks = programDisTasks;

                double temp = taskfinishLearningTime / 3600;
                temp = temp >= sumTaskLearningTime ? sumTaskLearningTime : temp;
                growthPhase.TaskFinishLearningTime = temp.ToString("#0");

                //总体
                growthPhase.PhaseLearningTime = sumlearningTime.ToString();
                growthPhase.PhaseFinishLearningTime = sumFinishTime.ToString("#0.0");
                growthPhase.PhaseFinishRate = tempPhaseFinishRate.ToString("#0.0");
                phaseList.Add(growthPhase);
            }
            List<Program> programs = new List<Program>();
            programs.Add(new Program
            {
                PlaneType = PlaneType,
                growthPhases = phaseList
            });
            return new { code = 200, result = programs, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

}
public class CourseData
{
    public long src_id { get; set; }
    public string course_name { get; set; }
    public decimal learning_time { get; set; }
    public string learning_progress { get; set; }
    public int learning_sum_time { get; set; }

}
public class Program
{
    public string PlaneType { get; set; }
    public List<GrowthPhase> growthPhases { get; set; }
}
public class GrowthPhase
{
    public string GrowPhase { get; set; }
    public string RangePurpose { get; set; }
    public string StandardRequest { get; set; }
    public string CurEndorsementType { get; set; }
    public string EndorsementType { get; set; }
    public bool EndorsementTypeFlag { get; set; }
    public string CurPlaneType1 { get; set; }
    public string PlaneType1 { get; set; }
    public bool PlaneTypeFlag { get; set; }
    public int? CurSumDuration { get; set; }
    public int? SumDuration { get; set; }
    public bool SumDurationFlag { get; set; }
    public int? CurTrainTime { get; set; }
    public int? TrainTime { get; set; }
    public bool TrainTimeFlag { get; set; }
    public int? CurUpDownTimes { get; set; }
    public int? UpDownTimes { get; set; }
    public bool UpDownTimesFlag { get; set; }
    public string CurTechnicalGrade { get; set; }
    public string TechnicalGrade { get; set; }
    public bool TechnicalGradeFlag { get; set; }
    public string OtherRemark { get; set; }
    public bool PassFlag { get; set; }//0：未通过，1：通过

    //阶段学时
    public string PhaseLearningTime { get; set; }
    public string PhaseFinishLearningTime { get; set; }
    public string PhaseFinishRate { get; set; }

    //理论
    public string FinishRate { get; set; }
    public string SumLearningTime { get; set; }
    public string FinishLearningTime { get; set; }

    //实践
    public string TaskPassRate { get; set; }
    public string TaskSumCount { get; set; }
    public string TaskPassCount { get; set; }
    public string TaskSumLearningTime { get; set; }
    public string TaskFinishLearningTime { get; set; }

    public List<ProgramCourse> programCourses { get; set; }
    public List<ProgramTask> programTasks { get; set; }
}
public class ProgramCourse
{
    public string CourseName { get; set; }
    public string FinishRate { get; set; }
    public string LearningTime { get; set; }
    public string FinishLearningTime { get; set; }
}
public class ProgramTask
{
    public long SrcID { get; set; }
    public string TaskName { get; set; }
    public sbyte TaskResult { get; set; }
    public string LearningTime { get; set; }
    public double TaskFinishLearningTime { get; set; }
}
public class RemoteServiceResult
{
    public int code { get; set; }
    public string message { get; set; }
    public ResultInfo result { get; set; }
}
public class ResultInfo
{
    public List<WorkInfos> workInfos { get; set; }
    public List<CertificateInfos> certificateInfos { get; set; }
}
public class WorkInfos
{
    /// <summary>
    /// 总飞行时间
    /// </summary>
    public double totalDuration { get; set; }
    /// <summary>
    /// 总模拟训练时间
    /// </summary>
    public double trainingDuration { get; set; }
    /// <summary>
    /// 总起落次数
    /// </summary>
    public double actualFlightNumber { get; set; }
    /// <summary>
    /// 总飞行经历时间
    /// </summary>
    public double actualDuration { get; set; }
    /// <summary>
    /// 本机型总起落次数
    /// </summary>
    public double currentActualDuration { get; set; }
    /// <summary>
    /// 本机型总经历时间
    /// </summary>
    public double currentFlightNumber { get; set; }
    /// <summary>
    /// 机型
    /// </summary>
    public string airplaneModelValue { get; set; }
    /// <summary>
    /// 技术等级
    /// </summary>
    public string skillLevelValue { get; set; }
}
public class CertificateInfos
{
    public string Name { get; set; }
}

