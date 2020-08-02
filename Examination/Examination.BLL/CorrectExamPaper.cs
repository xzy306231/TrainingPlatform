using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examination.BLL
{
    public class CorrectExamPaper
    {
        public object GetCorrectExamPaper(pf_examinationContext db, string UserNumber, string strStatus, string StartTime, string EndTime, string KeyWord, int PageIndex, int PageSize)
        {
            try
            {
                IQueryable<t_examination_manage> query = null;
                if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
                {
                    query = from t in db.t_grade_teacher
                            join e in db.t_examination_manage on t.examination_id equals e.id
                            where t.delete_flag == 0 && t.grade_teacher_num == UserNumber
                            && (strStatus == "0" ? (e.exam_status == "3" && (e.correct_status == "2" || e.correct_status == "3")) : (e.correct_status == strStatus && e.exam_status == "3"))
                            && (e.start_time >= DateTime.Parse(StartTime))
                            && (e.end_time <= DateTime.Parse(EndTime))
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby e.correct_status ascending
                            select e;
                }
                else if (!string.IsNullOrEmpty(StartTime) && string.IsNullOrEmpty(EndTime))
                {
                    query = from t in db.t_grade_teacher
                            join e in db.t_examination_manage on t.examination_id equals e.id
                            where t.delete_flag == 0 && t.grade_teacher_num == UserNumber
                            && (strStatus == "0" ? (e.exam_status == "3" && (e.correct_status == "2" || e.correct_status == "3")) : (e.correct_status == strStatus && e.exam_status == "3"))
                            && (e.start_time >= DateTime.Parse(StartTime))
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby e.correct_status ascending
                            select e;
                }
                else if (string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
                {
                    query = from t in db.t_grade_teacher
                            join e in db.t_examination_manage on t.examination_id equals e.id
                            where t.delete_flag == 0 && t.grade_teacher_num == UserNumber
                            && (strStatus == "0" ? (e.exam_status == "3" && (e.correct_status == "2" || e.correct_status == "3")) : (e.correct_status == strStatus && e.exam_status == "3"))
                            && (e.end_time <= DateTime.Parse(EndTime))
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby e.correct_status ascending
                            select e;
                }
                else
                {
                    query = from t in db.t_grade_teacher
                            join e in db.t_examination_manage on t.examination_id equals e.id
                            where t.delete_flag == 0 && t.grade_teacher_num == UserNumber
                            && (strStatus == "0" ? (e.exam_status == "3" && (e.correct_status == "2" || e.correct_status == "3")) : (e.correct_status == strStatus && e.exam_status == "3"))
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby e.correct_status ascending
                            select e;
                }

                List<ExamInfo> exam = new List<ExamInfo>();

                foreach (var item in query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList())
                {
                    //查询参考人数
                    var queryExam = from r in db.t_examination_record
                                    where r.delete_flag == 0 && r.examination_id == item.id
                                    select r;
                    //查询已批阅人数
                    var queryCorrect = from a in db.t_examination_record
                                       where a.delete_flag == 0 && a.examination_id == item.id && a.record_status == "4"
                                       select a;

                    exam.Add(new ExamInfo()
                    {
                        ID = item.id,
                        ExamName = item.exam_name,
                        CorrectStatus = item.correct_status,
                        StartTime = item.start_time.ToString(),
                        EndTime = item.end_time.ToString(),
                        ExamDiv = item.exam_div,
                        ExamDuration = item.exam_duration,
                        SumExamCount = queryExam.Count(),
                        CorrectCount = queryCorrect.Count()
                    });
                }
                return new { code = 200, result = exam, count = query.Count(), msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetExamStudent(pf_examinationContext db, long examid)
        {
            try
            {
                var query = from s in db.t_examination_record
                            where s.delete_flag == 0 && s.examination_id == examid
                            select s;
                var queryCorrect = from s in db.t_examination_record
                                   where s.delete_flag == 0 && s.examination_id == examid && s.record_status == "4"
                                   select s;
                int SumExamCount = query.Count();
                int CorrectCount = queryCorrect.Count();//已批阅
                int NotCorrectCount = SumExamCount - CorrectCount;//未批阅

                List<UserInfo> list = new List<UserInfo>();
                foreach (var item in query)
                {
                    list.Add(new UserInfo()
                    {
                        UserNumber = item.user_number,
                        UserName = item.user_name,
                        RecordStatus = item.record_status,
                        RecordID = item.id
                    });
                }
                return new { code = 200, result = list, CorrectCount = CorrectCount, NotCorrectCount = NotCorrectCount, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public async Task<object> CorrectQuestion(pf_examinationContext db, long RecordID, long ItemID, int Score)
        {
            try
            {
                //查找及格分
                int? nPassScore = 0;
                var queryExam = from r in db.t_examination_record
                                join e in db.t_examination_manage on r.examination_id equals e.id
                                where r.delete_flag == 0 && r.id == RecordID
                                select e;
                var queryExamF = queryExam.FirstOrDefault();
                if (queryExamF != null)
                    nPassScore = queryExamF.pass_scores;

                var queryAnswer = from g in db.t_answer_log
                                  where g.delete_flag == 0 && g.record_id == RecordID && g.item_id == ItemID
                                  select g;
                var queryAnswerF = queryAnswer.FirstOrDefault();
                if (queryAnswerF != null)
                {
                    queryAnswerF.correct_flag = "1";
                    queryAnswerF.score = Score;
                }
                await db.SaveChangesAsync();
                //计算总得分
                int? sum = db.t_answer_log.Where(o => o.delete_flag == 0 && o.record_id == RecordID).Select(o => o.score).Sum();
                var queryRecord = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.id == RecordID
                                  select r;
                var queryRecordF = queryRecord.FirstOrDefault();
                queryRecordF.score = sum;
                if (nPassScore <= sum)
                    queryRecordF.pass_flag = 1;//及格了
                else
                    queryRecordF.pass_flag = 0;//未及格
                await db.SaveChangesAsync();

                return new { code = 200, result = 1, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object CorrectSubmitTestPaper(pf_examinationContext db, long ExamID, IHttpClientHelper client, TokenModel token, RabbitMQClient rabbit, IConfiguration configuration)
        {
            try
            {
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == ExamID
                                select e;
                var queryExamF = queryExam.FirstOrDefault();
                queryExamF.correct_status = "3";//阅卷结束

                int nSubjectCount = 0;
                if (queryExamF.exam_div == "2")//实践考试
                {
                    var querySubject = from t in db.t_training_task
                                       join s in db.t_training_subject on t.id equals s.task_id
                                       where t.delete_flag == 0 && t.examination_id == ExamID
                                       select t;
                    nSubjectCount = querySubject.Count();

                    var queryRecord = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.examination_id == ExamID
                                      select r;
                    foreach (var item in queryRecord)
                    {
                        var queryTaskLog = from g in db.t_task_log
                                           where g.delete_flag == 0 && g.record_id == item.id && g.exam_result == "1"
                                           select g;
                        var LogCount = queryTaskLog.Count();
                        float f = (float)LogCount / nSubjectCount;
                        item.pass_rate = (decimal)f * 100;
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    //推送消息
                    ExamModel exam = new ExamModel();
                    exam.ID = ExamID;
                    exam.ExamDiv = queryExamF.exam_div;
                    rabbit.ExamMsg(exam);

                    //将得分数据推送到培训计划
                    var queryRecord = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.examination_id == ExamID
                                      select r;
                    List<ExamResult> examResults = new List<ExamResult>();
                    foreach (var item in queryRecord)
                    {
                        examResults.Add(new ExamResult()
                        {
                            PlanID = (long)item.plan_id,
                            ExamID = ExamID,
                            UserNumber = item.user_number,
                            Score = item.score
                        });
                    }
                    string Url = "http://TRAININGPLAN-SERVICE/trainingplan/v1/UpdateStuScore";
                    client.PutRequest(Url, examResults);

                    //产生日志消息
                    SysLogModel syslog = new SysLogModel
                    {
                        opNo = token.userNumber,
                        opName = token.userName,
                        opType = 3,
                        logDesc = "试卷：" + queryExamF.exam_name + ",批阅结束",
                        logSuccessd = 1,
                        moduleName = "考试评分"
                    };
                    rabbit.LogMsg(syslog);
                    return new { code = 200, msg = "OK" };
                }
                else
                    return new { code = 200, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object CorrectTaskSubject(pf_examinationContext db, RabbitMQClient rabbit, long RecordID, long TaskID, long SubjectID, string strResult, string subjectName, TokenModel token)
        {
            try
            {
                var queryTask = from t in db.t_task_log
                                where t.delete_flag == 0 && t.record_id == RecordID
                                && t.task_id == TaskID && t.subject_id == SubjectID
                                select t;
                var queryTaskF = queryTask.FirstOrDefault();
                if (queryTaskF != null)
                {
                    queryTaskF.exam_result = strResult;
                    queryTaskF.do_flag = "1";
                }
                else
                {
                    t_task_log log = new t_task_log();
                    log.record_id = RecordID;
                    log.task_id = TaskID;
                    log.subject_id = SubjectID;
                    log.exam_result = strResult;
                    log.do_flag = "1";//做了
                    db.t_task_log.Add(log);
                }
                if (db.SaveChanges() > 0)
                {

                    //产生日志消息
                    SysLogModel syslog = new SysLogModel
                    {
                        opNo = token.userNumber,
                        opName = token.userName,
                        opType = 3,
                        logDesc = "批阅了科目:" + subjectName,
                        logSuccessd = 1,
                        moduleName = "考试评分"
                    };
                    rabbit.LogMsg(syslog);
                    return new { code = 200, msg = "OK" };
                }
                else
                    return new { code = 200, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object CorrectTask(pf_examinationContext db, RabbitMQClient rabbit, long RecordID, string UserNumber, string TaskComment, string Result, string taskName, TokenModel token)
        {
            try
            {
                var queryRecord = from r in db.t_examination_record
                                  where r.id == RecordID && r.delete_flag == 0 && r.user_number == UserNumber
                                  select r;
                var queryRecordF = queryRecord.FirstOrDefault();
                if (queryRecordF != null)
                {
                    queryRecordF.record_status = "4";//已批阅
                    queryRecordF.task_comment = TaskComment;//评语
                    queryRecordF.score = float.Parse(Result);//0：未通过，1：通过
                }
                if (db.SaveChanges() > 0)
                {

                    //产生日志消息
                    SysLogModel syslog = new SysLogModel
                    {
                        opNo = token.userNumber,
                        opName = token.userName,
                        opType = 3,
                        logDesc = "批阅了任务：" + taskName,
                        logSuccessd = 1,
                        moduleName = "考试评分"
                    };
                    rabbit.LogMsg(syslog);
                    return new { code = 200, msg = "OK" };
                }
                else
                    return new { code = 200, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetTaskSubjectInfo(pf_examinationContext db, long examid, string UserNumber)
        {
            try
            {
                TaskInfo task = new TaskInfo();
                var queryTask = from t in db.t_training_task
                                where t.delete_flag == 0 && t.examination_id == examid
                                select t;
                var queryTaskF = queryTask.FirstOrDefault();
                if (queryTaskF != null)
                {
                    task.ID = queryTaskF.id;
                    task.KindLevel = queryTaskF.kind_level;
                    task.RankLevel = queryTaskF.rank_level;
                    task.TaskName = queryTaskF.task_name;
                    task.TaskType = queryTaskF.task_type;
                    task.PlaneType = queryTaskF.plane_type;

                    var querySubject = from s in db.t_training_subject
                                       where s.task_id == queryTaskF.id && s.delete_flag == 0
                                       select s;
                    List<SubjectInfo> subjects = new List<SubjectInfo>();
                    foreach (var item in querySubject)
                    {
                        SubjectInfo subject = new SubjectInfo();
                        var queryRecord = from r in db.t_examination_record
                                          where r.delete_flag == 0 && r.examination_id == examid && r.user_number == UserNumber
                                          select r;
                        var queryRecordF = queryRecord.FirstOrDefault();
                        if (queryRecordF != null)
                        {
                            task.Comment = queryRecordF.task_comment;
                            task.TaskResult = queryRecordF.score;
                            var queryTaskLog = from t in db.t_task_log
                                               where t.delete_flag == 0 && t.record_id == queryRecordF.id
                                               && t.task_id == queryTaskF.id && t.subject_id == item.id
                                               select t;
                            var queryTaskLogF = queryTaskLog.FirstOrDefault();
                            if (queryTaskLogF != null)
                                subject.SubjectResult = queryTaskLogF.exam_result;
                        }

                        subject.ExpectResult = item.expect_result;
                        subject.ID = item.id;
                        subject.PlaneType = item.plane_type;
                        subject.TrainDesc = item.train_desc;
                        subject.TrainKind = item.train_kind;
                        subject.TrainName = item.train_name;
                        subject.TrainNumber = item.train_number;

                        subjects.Add(subject);
                    }
                    task.subjectInfos = subjects;
                }
                return new { code = 200, result = task, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object UpdateStuCorrectStatus(pf_examinationContext db, RabbitMQClient rabbit, long examid, string UserNumber, TokenModel token)
        {
            try
            {
                var query = from r in db.t_examination_record
                            where r.delete_flag == 0 && r.examination_id == examid && r.user_number == UserNumber
                            select r;
                var queryF = query.FirstOrDefault();
                if (queryF != null)
                {
                    queryF.record_status = "4";
                }
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 3,
                    logDesc = "更新了学生试卷的批阅状态",
                    logSuccessd = 1,
                    moduleName = "考试评分"
                };
                rabbit.LogMsg(syslog);
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
    public class ExamResult
    {
        public long PlanID { get; set; }
        public long ExamID { get; set; }
        public string UserNumber { get; set; }
        public float? Score { get; set; }
    }
    public class ExamInfo
    {
        public long ID { get; set; }
        public string ExamName { get; set; }
        public string StartTime { get; set; }
        public string ExamDiv { get; set; }
        public string EndTime { get; set; }
        public int? ExamDuration { get; set; }
        public string CorrectStatus { get; set; }
        /// <summary>
        /// 参考总人数
        /// </summary>
        public int SumExamCount { get; set; }
        /// <summary>
        /// 已批阅人数
        /// </summary>
        public int CorrectCount { get; set; }
    }
    public class UserInfo
    {
        public long RecordID { get; set; }
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string RecordStatus { get; set; }
    }

    public class TaskInfo
    {
        public long ID { get; set; }
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string KindLevel { get; set; }
        public string RankLevel { get; set; }
        public string PlaneType { get; set; }
        public float? TaskResult { get; set; }
        public string Comment { get; set; }
        public List<SubjectInfo> subjectInfos { get; set; }
    }

    public class SubjectInfo
    {
        public long ID { get; set; }
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string TrainDesc { get; set; }
        public string TrainKind { get; set; }
        public string PlaneType { get; set; }
        public string ExpectResult { get; set; }
        public string SubjectResult { get; set; }
    }
}
