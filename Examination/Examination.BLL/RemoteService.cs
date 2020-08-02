using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Examination.BLL
{
    public class RemoteService
    {
        public object AddStuFromRemoteService(pf_examinationContext db,RabbitMQClient rabbit, ExamUserModel examUserModel)
        {
            try
            {
                t_examination_manage queryExaminationF = null;
                string userName = "";
                if (examUserModel.ExaminationListID != null && examUserModel.ExaminationListID.Count > 0)
                {
                    for (int i = 0; i < examUserModel.ExaminationListID.Count; i++)
                    {
                        string strExamDiv = "1";
                        //查询出是什么类别的考试，1：理论，2：实践
                        var queryExamination = from e in db.t_examination_manage
                                               where e.delete_flag == 0 && e.id == examUserModel.ExaminationListID[i]
                                               select e;
                        queryExaminationF = queryExamination.FirstOrDefault();
                        if (queryExaminationF.exam_div == "2")
                            strExamDiv = "2";

                        //更新考试管理人数
                        var queryExam = from e in db.t_statistic_texam
                                        where e.delete_flag == 0 && e.exam_id == examUserModel.ExaminationListID[i]
                                        select e;
                        var queryExamF = queryExam.FirstOrDefault();
                        if (queryExamF != null)
                        {
                            if (queryExamF.total_num == null)
                                queryExamF.total_num = examUserModel.userInfos.Count;
                            else
                                queryExamF.total_num = examUserModel.userInfos.Count + queryExamF.total_num;
                        }

                        if (examUserModel.userInfos != null && examUserModel.userInfos.Count > 0)
                        {
                            for (int j = 0; j < examUserModel.userInfos.Count; j++)
                            {
                                //创建学生参考记录
                                t_examination_record r = new t_examination_record();
                                r.plan_id = examUserModel.PlanID;
                                r.examination_id = examUserModel.ExaminationListID[i];
                                r.user_number = examUserModel.userInfos[j].UserNumber;
                                r.user_name = examUserModel.userInfos[j].UserName;
                                r.department = examUserModel.userInfos[j].Department;
                                //r.score = 0;
                                r.record_status = "1";
                                userName = userName + "," + examUserModel.userInfos[j].UserName;
                                db.t_examination_record.Add(r);
                                db.SaveChanges();

                                //创建学生作答记录
                                long RecordID = r.id;

                                if (strExamDiv == "1")//理论考试
                                {
                                    var queryTestPaper = from p in db.t_test_papers
                                                         where p.delete_flag == 0 && p.examination_id == examUserModel.ExaminationListID[i]
                                                         select p;
                                    var queryTestPaperF = queryTestPaper.FirstOrDefault();
                                    if (queryTestPaperF != null)//如果是实践考试，就不存在试卷咯
                                    {
                                        var queryQuestion = from q in db.t_questions
                                                            where q.delete_flag == 0 && q.test_paper_id == queryTestPaperF.id
                                                            select q;
                                        foreach (var item in queryQuestion)
                                        {
                                            t_answer_log log = new t_answer_log();
                                            log.record_id = RecordID;
                                            log.item_id = item.id;
                                            log.score = 0;//得分
                                            log.answer_result = "";//作答结果
                                            log.correct_flag = "0";//未批阅
                                            db.t_answer_log.Add(log);
                                        }
                                    }
                                }
                                else//实践考试
                                {
                                    var queryTaskSubject = from t in db.t_training_task
                                                           join s in db.t_training_subject on t.id equals s.task_id
                                                           where t.delete_flag == 0 && t.examination_id == examUserModel.ExaminationListID[i]
                                                           select new { t, s };
                                    foreach (var item in queryTaskSubject)
                                    {
                                        //添加任务科目作答日志
                                        t_task_log task = new t_task_log();
                                        task.record_id = RecordID;
                                        task.task_id = item.t.id;
                                        task.subject_id = item.s.id;
                                        db.t_task_log.Add(task);
                                    }
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
                userName = userName.TrimStart(',');
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = examUserModel.userNumber;
                syslog.opName = examUserModel.userName;
                syslog.opType = 2;
                syslog.logDesc = "考试：" + queryExaminationF.exam_name + ",添加了考生：" + userName;
                syslog.logSuccessd = 1;
                syslog.moduleName = "考试管理";
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

        public object DeleteStuFromRemoteService(pf_examinationContext db,RabbitMQClient rabbit, ExamUserModel examUserModel)
        {
            try
            {
                string userNumber = "";
                for (int i = 0; i < examUserModel.userInfos.Count; i++)
                {
                    var queryStu = from s in db.t_examination_record
                                   where s.plan_id == examUserModel.PlanID && s.delete_flag == 0
                                        && s.user_number == examUserModel.userInfos[i].UserNumber
                                   select s;
                    foreach (var item in queryStu)
                    {
                        //删除学员
                        item.delete_flag = 1;
                        userNumber = userNumber + "," + item.user_number;
                    }
                }
                db.SaveChanges();


                var queryRecord1 = from r in db.t_examination_record
                                   where r.plan_id == examUserModel.PlanID
                                   select new { r.examination_id };
                foreach (var item in queryRecord1.Distinct())
                {
                    //更新考试管理人数
                    var queryExam = from e in db.t_statistic_texam
                                    where e.delete_flag == 0 && e.exam_id == item.examination_id
                                    select e;
                    var queryExamF = queryExam.FirstOrDefault();
                    if (queryExamF != null)
                    {
                        var queryRecord = from r in db.t_examination_record
                                          where r.delete_flag == 0 && r.examination_id == item.examination_id
                                          select r;
                        queryExamF.total_num = queryRecord.Count();
                    }
                }

                db.SaveChanges();

                userNumber = userNumber.TrimStart(',');
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = examUserModel.userNumber;
                syslog.opName = examUserModel.userName;
                syslog.opType = 3;
                syslog.logDesc = "考试删除了考生：" + userNumber;
                syslog.logSuccessd = 1;
                syslog.moduleName = "考试管理";
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

        public object DeleteStuPlanIDExamID(pf_examinationContext db, ExamUserModel examUserModel)
        {
            try
            {
                if (examUserModel.ExaminationListID != null && examUserModel.ExaminationListID.Count > 0)
                {
                    //考试恢复使用
                    var queryExamination = from e in db.t_examination_manage
                                           where e.delete_flag == 0 && e.id == examUserModel.ExaminationListID[0]
                                           select e;
                    var queryExaminationF = queryExamination.FirstOrDefault();
                    if (queryExaminationF != null)
                    {
                        queryExaminationF.used_flag = 0;
                        queryExaminationF.publish_flag = 0;
                        queryExaminationF.exam_status = "1";
                        queryExaminationF.start_time = null;
                        queryExaminationF.end_time = null;
                    }

                    //考试管理学员数量清零
                    var queryExam = from e in db.t_statistic_texam
                                    where e.delete_flag == 0 && e.exam_id == examUserModel.ExaminationListID[0]
                                    select e;
                    var queryExamF = queryExam.FirstOrDefault();
                    if (queryExamF != null)
                        queryExamF.total_num = 0;

                    //删除记录
                    var queryRecord = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.examination_id == examUserModel.ExaminationListID[0]
                                      select r;
                    foreach (var item in queryRecord)
                    {
                        item.delete_flag = 1;
                    }
                }
                db.SaveChanges();
                return new { code = 200, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object RecoverExamination(pf_examinationContext db, ExamUserModel examUserModel)
        {
            try
            {
                //删除培训计划下的所有学员
                var queryRecord = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.plan_id == examUserModel.PlanID
                                  select r;
                foreach (var item in queryRecord)
                {
                    item.delete_flag = 1;
                }

                if (examUserModel.ExaminationListID != null && examUserModel.ExaminationListID.Count > 0)
                {

                    for (int i = 0; i < examUserModel.ExaminationListID.Count; i++)
                    {
                        //统计学生人数数据清零
                        var queryStu = from s in db.t_statistic_texam
                                       where s.delete_flag == 0 && s.exam_id == examUserModel.ExaminationListID[i]
                                       select s;
                        var queryStuF = queryStu.FirstOrDefault();
                        if (queryStuF != null)
                        {
                            queryStuF.total_num = 0;
                        }


                        //释放考试管理
                        var queryExam = from e in db.t_examination_manage
                                        where e.delete_flag == 0 && e.id == examUserModel.ExaminationListID[i]
                                        select e;
                        foreach (var item in queryExam)
                        {
                            item.used_flag = 0;
                            item.publish_flag = 0;
                            item.start_time = null;
                            item.end_time = null;
                            if (item.exam_div == "1")//理论考试
                            {
                                item.exam_status = "1";//考试未开始
                                item.correct_status = "1";//阅卷未开始
                            }
                            if (item.exam_div == "2")//实践考试
                            {
                                item.exam_status = "1";//未开始
                                item.correct_status = "2";//阅卷开始
                            }
                        }
                    }
                    db.SaveChanges();
                }
                return new { code = 200, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object PublishExamination(pf_examinationContext db,RabbitMQClient rabbit, List<ExaminationInfo> examination)
        {
            try
            {
                if (examination != null && examination.Count > 0)
                {
                    string examName = "";
                    for (int i = 0; i < examination.Count; i++)
                    {
                        var query = from e in db.t_examination_manage
                                    where e.delete_flag == 0 && e.id == examination[i].ID
                                    select e;
                        foreach (var item in query)
                        {
                            item.publish_flag = 1;
                            examName = examName + "," + item.exam_name;
                        }
                    }
                    if (db.SaveChanges() > 0)
                    {
                        examName = examName.TrimStart(',');
                        //产生日志消息
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = examination[0].userNumber;
                        syslog.opName = examination[0].userName;
                        syslog.opType = 3;
                        syslog.logDesc = "发布了考试：" + examName;
                        syslog.logSuccessd = 1;
                        syslog.moduleName = "考试管理";
                        rabbit.LogMsg(syslog);

                        return new { code = 200, msg = "OK" };
                    }
                    else
                    {
                        return new { code = 200, msg = "OK" };
                    }
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

        public object UpdateExaminationTime(pf_examinationContext db, List<ExaminationInfo> examinations)
        {
            try
            {
                if (examinations != null && examinations.Count > 0)
                {
                    for (int i = 0; i < examinations.Count; i++)
                    {
                        var queryExam = from e in db.t_examination_manage
                                        where e.id == examinations[i].ID && e.delete_flag == 0
                                        select e;
                        var queryExamF = queryExam.FirstOrDefault();
                        queryExamF.start_time = DateTime.Parse(examinations[i].StartTime);
                        queryExamF.end_time = DateTime.Parse(examinations[i].EndTime);
                    }
                    db.SaveChanges();
                }
                return new { code = 200, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object ForceSubmitExamination(pf_examinationContext db, long ExamID)
        {
            try
            {
                int nPassScore = 0;
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == ExamID
                                select e;
                var queryExamF = queryExam.FirstOrDefault();
                if (queryExamF != null)
                {
                    nPassScore = (int)queryExamF.pass_scores;//及格线
                }

                var queryRecord = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.examination_id == ExamID
                                  select r;
                foreach (var item in queryRecord)
                {
                    //将客观题批改状态修改为已批改
                    var queryAnswer = from g in db.t_answer_log
                                      where g.record_id == item.id && (from p in db.t_test_papers
                                                                       join q in db.t_questions on p.id equals q.test_paper_id
                                                                       where p.examination_id == ExamID && (q.question_type == "1" || q.question_type == "2" || q.question_type == "3" || q.question_type == "4")
                                                                       select q.id).Contains((long)g.item_id)
                                      select g;
                    foreach (var item1 in queryAnswer)
                    {
                        item1.correct_flag = "1";
                    }

                    //计算总得分
                    int? sum = db.t_answer_log.Where(o => o.delete_flag == 0 && o.record_id == item.id).Select(o => o.score).Sum();
                    if (nPassScore <= sum)
                    {
                        item.pass_flag = 1;//及格了
                    }
                    item.score = sum;
                    item.end_time = DateTime.Now;//结束时间
                    item.record_status = "3";//已结束
                }
                db.SaveChanges();
                return new { code = 200, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetStuExamResult(pf_examinationContext db, long examId, string userNumber)
        {
            try
            {
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == examId
                                select e;
                var queryExamF = queryExam.FirstOrDefault();
                if (queryExamF.exam_div == "1")//理论
                {
                    var queryRecord = from r in db.t_examination_record
                                      where r.examination_id == examId && r.user_number == userNumber && r.delete_flag == 0
                                      select r;
                    var queryRecordF = queryRecord.FirstOrDefault();
                    if (queryRecordF != null)
                        return new { code = 200, result = queryRecordF.pass_flag, dif = "1", msg = "OK" };
                    else
                        return new { code = 400, msg = "Error" };
                }
                else//实践
                {
                    var queryRecord = from r in db.t_examination_record
                                      where r.examination_id == examId && r.user_number == userNumber && r.delete_flag == 0
                                      select r;
                    var queryRecordF = queryRecord.FirstOrDefault();
                    if (queryRecordF != null)
                        return new { code = 200, result = queryRecordF.score, dif = "2", msg = "OK" };
                    else
                        return new { code = 400, msg = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object QuitExamination(pf_examinationContext db,RabbitMQClient rabbit, ExamUserModel examUserModel)
        {
            try
            {
                t_examination_manage q = null;
                string examName = "";
                if (examUserModel.ExaminationListID != null && examUserModel.ExaminationListID.Count > 0)
                {
                    for (int i = 0; i < examUserModel.ExaminationListID.Count; i++)
                    {
                        var query = from e in db.t_examination_manage
                                    where e.delete_flag == 0 && e.id == examUserModel.ExaminationListID[i]
                                    select e;
                        q = query.FirstOrDefault();
                        examName = examName + "," + q.exam_name;
                        q.exam_status = "3";//中止
                        q.end_time = DateTime.Now;//更新结束时间，提前结束
                        if (q.exam_div == "1")//理论需要判断，这场考试是否有学生作答了
                        {
                            var queryRecord = from r in db.t_examination_record
                                              where r.examination_id == examUserModel.ExaminationListID[i] && r.delete_flag == 0 && (r.record_status == "2" || r.record_status == "3" || r.record_status == "4")
                                              select r;
                            if (queryRecord.Count() > 0)//有学生作答了
                                q.correct_status = "2";//阅卷开始
                            else
                                q.correct_status = "3";//阅卷结束
                        }
                        else
                            q.correct_status = "2";//阅卷开始                         
                    }
                    db.SaveChanges();
                }

                //产生日志消息
                examName = examName.TrimStart(',');
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = examUserModel.userNumber;
                syslog.opName = examUserModel.userName;
                syslog.opType = 3;
                syslog.logDesc = "中止了考试：" + examName;
                syslog.logSuccessd = 1;
                syslog.moduleName = "考试管理";
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

        public object GetPlanExamProgress(pf_examinationContext db, long planId)
        {
            try
            {
                var query = from e in db.t_examination_record
                            where e.delete_flag == 0 && e.plan_id == planId
                            select e;
                var queryRecord = query.Where(x => x.record_status != "1");
                if (query.Count() != 0)
                    return new { result = (decimal)queryRecord.Count() / query.Count() * 100 };
                else
                    return new { result = 0 };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetExamGradeTech(pf_examinationContext db, List<long> list)
        {
            try
            {
                var queryTeacherNumList = db.t_grade_teacher.Where(x => list.Contains((long)x.examination_id)).Select(x => x.grade_teacher_num).ToList();
                return queryTeacherNumList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object GetExamGradeTechNum(pf_examinationContext db, List<long> examIdList)
        {
            try
            {
                Dictionary<long, List<string>> dic = new Dictionary<long, List<string>>();
                var queryTech = db.t_grade_teacher.Where(x => x.delete_flag == 0 && examIdList.Contains((long)x.examination_id)).ToList();
                foreach (var item in queryTech)
                {
                    if (dic.ContainsKey((long)item.examination_id))
                    {
                        if (dic[(long)item.examination_id].Contains(item.grade_teacher_num))
                            continue;
                        else
                            dic[(long)item.examination_id].Add(item.grade_teacher_num);
                    }
                    else
                    {
                        List<string> list = new List<string>();
                        list.Add(item.grade_teacher_num);
                        dic.Add((long)item.examination_id, list);
                    }
                }
                return dic;

            }
            catch (Exception ex)
            {
                return new Dictionary<long, List<string>>();
            }
        }

    }

    public class CorrectRate
    {
        public string UserNumber { get; set; }
        public string correctrate { get; set; }
    }
    public class ExaminationInfo
    {
        public long ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string userNumber { get; set; }
        public string userName { get; set; }
    }


}
