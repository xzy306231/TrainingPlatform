using Examination.BLL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examination.BLL
{
    public class MyExamination
    {
        /// <summary>
        /// 获取我的考试信息列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="strStatus"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="UserNumber"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public object GetMyExamination(pf_examinationContext db, string keyWord, string strStatus, string StartTime, string EndTime, string UserNumber, int PageIndex, int PageSize)
        {
            try
            {
                IQueryable<t_examination_manage> query = null;
                if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
                {
                    query = from s in db.t_examination_record
                            join e in db.t_examination_manage on s.examination_id equals e.id into se
                            from _se in se.DefaultIfEmpty()
                            where s.delete_flag == 0 && s.user_number == UserNumber
                            && (_se.start_time >= DateTime.Parse(StartTime))
                            && (_se.end_time <= DateTime.Parse(EndTime))
                            && ((string.IsNullOrEmpty(strStatus) ? _se.exam_status == "2" : true)
                                 || (_se.exam_status == "1" && _se.publish_flag == 1)
                                 || ((_se.correct_status == "2" || _se.correct_status == "3") && _se.exam_status == "3"))
                            && (strStatus == "1" ? (_se.exam_status == "1" && _se.publish_flag == 1) : true)
                            && (strStatus == "2" ? _se.exam_status == "2" : true)
                            && (strStatus == "3" ? (_se.exam_status == "3" && (_se.correct_status == "3" || _se.correct_status == "2")) : true)
                            orderby _se.t_create descending
                            select _se;
                }
                else if (!string.IsNullOrEmpty(StartTime) && string.IsNullOrEmpty(EndTime))
                {
                    query = from s in db.t_examination_record
                            join e in db.t_examination_manage on s.examination_id equals e.id into se
                            from _se in se.DefaultIfEmpty()
                            where s.delete_flag == 0 && s.user_number == UserNumber
                            && (_se.start_time >= DateTime.Parse(StartTime))
                            && (string.IsNullOrEmpty(keyWord) ? true : _se.exam_name.Contains(keyWord))
                            && ((string.IsNullOrEmpty(strStatus) ? _se.exam_status == "2" : true)
                                 || (_se.exam_status == "1" && _se.publish_flag == 1)
                                 || ((_se.correct_status == "2" || _se.correct_status == "3") && _se.exam_status == "3"))
                            && (strStatus == "1" ? (_se.exam_status == "1" && _se.publish_flag == 1) : true)
                            && (strStatus == "2" ? _se.exam_status == "2" : true)
                            && (strStatus == "3" ? (_se.exam_status == "3" && (_se.correct_status == "3" || _se.correct_status == "2")) : true)
                            orderby _se.t_create descending
                            select _se;
                }
                else if (string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
                {
                    query = from s in db.t_examination_record
                            join e in db.t_examination_manage on s.examination_id equals e.id into se
                            from _se in se.DefaultIfEmpty()
                            where s.delete_flag == 0 && s.user_number == UserNumber
                            && (_se.end_time <= DateTime.Parse(EndTime))
                            && (string.IsNullOrEmpty(keyWord) ? true : _se.exam_name.Contains(keyWord))
                            && ((string.IsNullOrEmpty(strStatus) ? _se.exam_status == "2" : true)
                                 || (_se.exam_status == "1" && _se.publish_flag == 1)
                                 || ((_se.correct_status == "2" || _se.correct_status == "3") && _se.exam_status == "3"))
                            && (strStatus == "1" ? (_se.exam_status == "1" && _se.publish_flag == 1) : true)
                            && (strStatus == "2" ? _se.exam_status == "2" : true)
                            && (strStatus == "3" ? (_se.exam_status == "3" && (_se.correct_status == "3" || _se.correct_status == "2")) : true)
                            orderby _se.t_create descending
                            select _se;
                }
                else
                {
                    query = from s in db.t_examination_record
                            join e in db.t_examination_manage on s.examination_id equals e.id into se
                            from _se in se.DefaultIfEmpty()
                            where s.delete_flag == 0 && s.user_number == UserNumber
                            && (string.IsNullOrEmpty(keyWord) ? true : _se.exam_name.Contains(keyWord))
                            && ((string.IsNullOrEmpty(strStatus) ? _se.exam_status == "2" : true)
                                 || (_se.exam_status == "1" && _se.publish_flag == 1)
                                 || ((_se.correct_status == "2" || _se.correct_status == "3") && _se.exam_status == "3"))
                            && (strStatus == "1" ? (_se.exam_status == "1" && _se.publish_flag == 1) : true)
                            && (strStatus == "2" ? _se.exam_status == "2" : true)
                            && (strStatus == "3" ? (_se.exam_status == "3" && (_se.correct_status == "3" || _se.correct_status == "2")) : true)
                            orderby _se.t_create descending
                            select _se;
                }

                List<Examinations> examinations = new List<Examinations>();
                //进行中
                List<Examinations> examinationing = new List<Examinations>();
                //未开始
                List<Examinations> examinationwill = new List<Examinations>();
                //已结束
                List<Examinations> examinationed = new List<Examinations>();
                foreach (var item in query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList())
                {
                    //状态
                    string sta = "1";
                    float? fscore = 0;
                    var q = from r in db.t_examination_record
                            where r.delete_flag == 0 && r.examination_id == item.id && r.user_number == UserNumber
                            select r;
                    var qf = q.FirstOrDefault();
                    if (qf != null)
                    {
                        fscore = qf.score;
                        sta = qf.record_status;
                    }
                    if (item.exam_status == "2")//进行中
                    {
                        examinationing.Add(new Examinations()
                        {
                            ID = item.id,//ID
                            ContentID = item.content_id,
                            ExamName = item.exam_name,//考试名称
                            StartTime = item.start_time.ToString(),//开始时间
                            EndTime = item.end_time.ToString(),//结束时间
                            ExamDuration = item.exam_duration,//时长
                            ExamDiv = item.exam_div,//区分，1：理论考试，2：实践考试
                            ExamStatus = item.exam_status,//考试管理状态
                            StuExamStatus = sta,//学生状态
                            Score = fscore,//得分 
                            CorrectStatus = item.correct_status//批阅状态
                        });
                    }
                    else if (item.exam_status == "1")//未开始
                    {
                        examinationwill.Add(new Examinations()
                        {
                            ID = item.id,//ID
                            ContentID = item.content_id,
                            ExamName = item.exam_name,//考试名称
                            StartTime = item.start_time.ToString(),//开始时间
                            EndTime = item.end_time.ToString(),//结束时间
                            ExamDuration = item.exam_duration,//时长
                            ExamDiv = item.exam_div,//区分，1：理论考试，2：实践考试
                            ExamStatus = item.exam_status,//考试管理状态
                            StuExamStatus = sta,//学生状态
                            Score = fscore,//得分
                            CorrectStatus = item.correct_status//批阅状态
                        });
                    }
                    else if (item.exam_status == "3")//已结束
                    {
                        examinationed.Add(new Examinations()
                        {
                            ID = item.id,//ID
                            ContentID = item.content_id,
                            ExamName = item.exam_name,//考试名称
                            StartTime = item.start_time.ToString(),//开始时间
                            EndTime = item.end_time.ToString(),//结束时间
                            ExamDuration = item.exam_duration,//时长
                            ExamDiv = item.exam_div,//区分，1：理论考试，2：实践考试
                            ExamStatus = item.exam_status,//考试管理状态
                            StuExamStatus = sta,//学生状态
                            Score = fscore,//得分
                            CorrectStatus = item.correct_status//批阅状态
                        });
                    }
                }
                examinations.AddRange(examinationing);//进行中
                examinations.AddRange(examinationwill);//未开始
                examinations.AddRange(examinationed);//已结束

                return new { code = 200, result = examinations, count = query.Count(), msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 开始考试界面信息
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public object GetTheoryExamAndPaperInfoByExamID(pf_examinationContext db, long examid)
        {
            try
            {
                Examinations examinations = new Examinations();
                var query = from e in db.t_examination_manage
                            join t in db.t_test_papers on e.id equals t.examination_id into et
                            from _et in et.DefaultIfEmpty()
                            where e.delete_flag == 0 && e.id == examid && _et.delete_flag == 0
                            select new { e, _et };
                var qf = query.FirstOrDefault();
                if (qf != null)
                {
                    //获取试卷下试题的数量
                    var queryItem = from p in db.t_questions
                                    where p.test_paper_id == qf._et.id && p.delete_flag == 0
                                    select p;

                    examinations.ExamName = qf.e.exam_name;
                    examinations.StartTime = qf.e.start_time.ToString();
                    examinations.EndTime = qf.e.end_time.ToString();
                    examinations.ExamDuration = qf.e.exam_duration;
                    examinations.ExamExplain = qf.e.exam_explain;
                    examinations.PassScore = qf.e.pass_scores;

                    PaperInfo paperInfo = new PaperInfo();
                    paperInfo.PaperName = qf._et.paper_title;//试卷名称
                    paperInfo.ExamScore = qf._et.exam_score;//试卷分值
                    paperInfo.QuestionCount = queryItem.Count();//试题数量
                    examinations.PaperInfo = paperInfo;
                }
                return new { code = 200, result = examinations, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 开始考试
        /// </summary>
        /// <param name="examinationid"></param>
        /// <param name="usernumber"></param>
        /// <returns></returns>
        public object StartExamination(pf_examinationContext db, long examinationid, string usernumber, TokenModel token, IHttpClientHelper client, RabbitMQClient rabbit)
        {
            try
            {
                double? duration = 0;
                //查找考试时长
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == examinationid
                                select e;
                var queryExamF = queryExam.FirstOrDefault();
                if (queryExamF != null)
                {
                    if (queryExamF.exam_status == "3")
                        return new { code = 401, msg = "本场考试已结束或已被中止！" };

                    if (queryExamF.start_time > DateTime.Now)
                        return new { code = 401, msg = "本场考试还未开始！" };

                    duration = queryExamF.exam_duration * 60 * 60;
                    DateTime now = DateTime.Now;
                    DateTime end = (DateTime)queryExamF.end_time;
                    TimeSpan span = end - now;
                    double dspan = span.TotalSeconds;
                    if (duration > dspan)
                        duration = dspan;
                }

                var query = from r in db.t_examination_record
                            where r.delete_flag == 0
                                 && r.examination_id == examinationid
                                 && r.user_number == usernumber
                            select r;
                var q = query.FirstOrDefault();
                q.start_time = DateTime.Now;//开始考试时间
                q.record_status = "2";//状态变更为进行中。。。
                db.SaveChanges();

                //计算完成率
                var queryExamRecord = from e in db.t_examination_record
                                      where e.delete_flag == 0 && e.plan_id == q.plan_id && e.user_number == usernumber
                                      select e;
                var queryExamRecordSumCount = queryExamRecord.Count();
                var NotStartCount = queryExamRecord.Where(x => x.record_status != "1").ToList().Count;
                var StartCount = queryExamRecordSumCount - NotStartCount;
                var Rate = NotStartCount / (decimal)queryExamRecordSumCount;

                ExamFinishRate examFinishRate = new ExamFinishRate();
                examFinishRate.PlanId = (long)q.plan_id;
                examFinishRate.UserId = token.userId;
                examFinishRate.UserNumber = token.userNumber;
                examFinishRate.Rate = Rate * 100;

                //调用远程服务
                string url = "http://TRAININGPLAN-SERVICE/trainingplan/v1/UpdateExamFinishRate";
                client.PutRequest(url, examFinishRate);

                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 3,
                    logDesc = "开始考试",
                    logSuccessd = 1,
                    moduleName = "我的考试"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, result = q.id, duration = duration, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 继续考试
        /// </summary>
        /// <param name="examinationid"></param>
        /// <param name="usernumber"></param>
        /// <returns></returns>
        public object ContinueExamination(pf_examinationContext db, RabbitMQClient rabbit, long examinationid, string usernumber, TokenModel token)
        {
            try
            {
                double? duration = 0;
                //查找考试时长
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == examinationid
                                select e;
                var queryExamF = queryExam.FirstOrDefault();
                if (queryExamF != null)
                {
                    duration = queryExamF.exam_duration * 60 * 60;
                    DateTime now = DateTime.Now;
                    DateTime end = (DateTime)queryExamF.end_time;
                    TimeSpan span = end - now;
                    double dspan = span.TotalSeconds;
                    if (duration > dspan)
                        duration = dspan;
                }


                //查看考试状态
                var queryRecord = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.examination_id == examinationid
                                  && r.user_number == usernumber
                                  select r;
                var queryRecordF = queryRecord.FirstOrDefault();
                if (queryRecordF != null)
                {
                    if (queryRecordF.record_status == "1")//未开始
                    {
                        return new { code = 200, result = duration, Record = queryRecordF.id, msg = "OK" };
                    }
                    else if (queryRecordF.record_status == "2")//进行中
                    {
                        if (queryRecordF.start_time != null)//进行中，在中途退出了
                        {
                            DateTime dt = DateTime.Now;
                            DateTime startTime = (DateTime)queryRecordF.start_time;
                            TimeSpan ts = dt - startTime;
                            double? SumSecond = ts.TotalSeconds;
                            if (duration < SumSecond)//超时
                            {
                                //强制提交试卷
                                //SubmitStudentTestPaper(queryRecordF.id, token);
                                return new { code = 401, msg = "本场考试已结束！" };
                            }
                            else//未超时
                            {

                                //产生日志消息
                                SysLogModel syslog = new SysLogModel
                                {
                                    opNo = token.userNumber,
                                    opName = token.userName,
                                    opType = 3,
                                    logDesc = "继续考试",
                                    logSuccessd = 1,
                                    moduleName = "我的考试"
                                };
                                rabbit.LogMsg(syslog);
                                return new { code = 200, result = duration - SumSecond, Record = queryRecordF.id, msg = "OK" };
                            }
                        }
                        else
                        {
                            //产生日志消息
                            SysLogModel syslog = new SysLogModel
                            {
                                opNo = token.userNumber,
                                opName = token.userName,
                                opType = 3,
                                logDesc = "继续考试",
                                logSuccessd = 1,
                                moduleName = "我的考试"
                            };
                            rabbit.LogMsg(syslog);
                            return new { code = 200, result = duration, Record = queryRecordF.id, msg = "OK" };
                        }
                    }
                    else//已结束
                    {
                        return new { code = 401, msg = "考试已经结束！" };
                    }
                }
                else
                    return new { code = 400, msg = "记录不存在" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 获取任务考试结果
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        public object GetTaskExamResultInfo(pf_examinationContext db, long ExamID, string UserNumber)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.id == ExamID
                            select e;
                TaskExamInfo task = new TaskExamInfo();
                if (query.FirstOrDefault() != null)
                {
                    var q = query.FirstOrDefault();
                    task.ExamName = q.exam_name;
                    task.StartTime = q.start_time.ToString();
                    task.EndTime = q.end_time.ToString();
                    task.ExamDuration = q.exam_duration;
                    task.ExamExplain = q.exam_explain;

                    var queryTask = from x in db.t_examination_manage
                                    join t in db.t_training_task on x.id equals t.examination_id
                                    where t.delete_flag == 0 && x.id == ExamID
                                    select t;
                    if (queryTask.FirstOrDefault() != null)
                    {
                        var qfTask = queryTask.FirstOrDefault();


                        //查找任务ID下的所有科目
                        List<TrainSubject> trainSubjects = new List<TrainSubject>();
                        var queySubject = from s in db.t_training_subject
                                          where s.delete_flag == 0 && s.task_id == qfTask.id
                                          select s;
                        //训练科目数量
                        task.SubjectCount = queySubject.Count();

                        int i = 0;
                        int nFinishCount = 0;
                        int nPassCount = 0;
                        foreach (var item in queySubject)
                        {
                            //查找知识点
                            var queryTag = from t in db.t_knowledge_tag
                                           join s in db.t_subject_knowledge_ref on t.id equals s.knowledge_tag_id
                                           where s.subject_id == item.id && s.delete_flag == 0
                                           select t;
                            List<Tag> tags = new List<Tag>();
                            foreach (var tag in queryTag)
                            {
                                tags.Add(new Tag()
                                {
                                    ID = tag.src_id,
                                    TagName = tag.tag
                                });
                            }

                            //查找RecordID
                            long record = 0;
                            var queryRecord = from r in db.t_examination_record
                                              where r.delete_flag == 0 && r.examination_id == ExamID && r.user_number == UserNumber
                                              select r;
                            if (queryRecord.FirstOrDefault() != null)
                            {
                                record = queryRecord.FirstOrDefault().id;
                                task.TaskExamResult = queryRecord.FirstOrDefault().score.ToString();//任务，0:未通过，1：通过
                            }

                            //查找训练科目的作答结果
                            var queryTaskLog = from l in db.t_task_log
                                               where l.delete_flag == 0 && l.record_id == record && l.task_id == qfTask.id
                                                    && l.subject_id == item.id
                                               select l;
                            string strTaskResult = "";
                            string strDoFlag = "0";
                            var queryTaskLogF = queryTaskLog.FirstOrDefault();
                            if (queryTaskLogF != null)
                            {
                                strTaskResult = queryTaskLogF.exam_result;//科目，0:未通过，1：通过
                                if (strTaskResult == "1")//通过
                                {
                                    ++nPassCount;
                                }
                                if (queryTaskLogF.do_flag == "1")//做了
                                {
                                    ++nFinishCount;
                                    strDoFlag = "1";
                                }
                            }

                            trainSubjects.Add(new TrainSubject()
                            {

                                ID = ++i,//编号
                                TrainNumber = item.train_number,//训练科目编号
                                TrainName = item.train_name,//训练科目名称
                                TrainDesc = item.train_desc,//训练科目描述
                                PlaneType = item.plane_type,//机型
                                TrainKind = item.train_kind,//训练类别
                                Tags = tags,//知识点
                                TaskExamResult = strTaskResult,//考试训练结果
                                ExpectResult = item.expect_result,//期望结果 
                                SubjectStatus = strDoFlag//0：未完成，1：完成
                            });
                        }
                        task.FinishCount = nFinishCount;//完成数量
                        task.PassCount = nPassCount;//通过数量
                        task.trainSubjects = trainSubjects;

                    }
                }
                return new { code = 200, result = task, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 获取学员作答试卷
        /// </summary>
        /// <param name="ExamManageID"></param>
        /// <returns></returns>
        public object GetStudentExamPaper(pf_examinationContext db, long ExamID, string UserNumber)
        {
            try
            {
                //查找学生作答的记录ID
                long recordid = 0;
                //试卷总得分
                float? sumScore = 0;
                var queryRecord = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.examination_id == ExamID && r.user_number == UserNumber
                                  select r;
                var queryRecordF = queryRecord.FirstOrDefault();
                if (queryRecordF != null)
                {
                    recordid = queryRecordF.id;
                    if (queryRecordF.score != null)
                        sumScore = queryRecordF.score;
                }

                //选项集合
                List<Option> optionsList = new List<Option>();
                //题干集合
                List<Questions> QuestionList = new List<Questions>();
                //试卷
                TestPaper testPaper = new TestPaper();
                var query = from m in db.t_test_papers
                            where m.delete_flag == 0 && m.examination_id == ExamID
                            select m;
                var qq = query.FirstOrDefault();
                if (qq == null)
                    return new { code = 401, msg = "试卷不存在" };

                long? paperid = qq.id;

                //查找试卷下的所有选项
                var queryOption = from t in db.t_test_papers
                                  join q in db.t_questions on t.id equals q.test_paper_id
                                  join o in db.t_question_option on q.id equals o.question_id into qo
                                  from _qo in qo.DefaultIfEmpty()
                                  where t.id == paperid && _qo.delete_flag == 0
                                  orderby _qo.option_number ascending
                                  select new
                                  {
                                      id = _qo == null ? 0 : _qo.id,
                                      questionid = _qo == null ? 0 : _qo.question_id,
                                      optionnum = _qo == null ? "null" : _qo.option_number,
                                      optioncontent = _qo == null ? "null" : _qo.option_content
                                  };
                foreach (var item in queryOption)
                {
                    optionsList.Add(new Option
                    {
                        ID = item.id,
                        QuestionID = item.questionid,
                        OptionNum = item.optionnum,
                        OptionContent = item.optioncontent
                    });
                }

                //查找试卷下的所有题干
                var queryQuestion = from t in db.t_test_papers
                                    join q in db.t_questions on t.id equals q.test_paper_id into tq
                                    from _tq in tq.DefaultIfEmpty()
                                    where t.id == paperid && _tq.delete_flag == 0
                                    orderby _tq.id ascending
                                    select new
                                    {
                                        id = _tq == null ? 0 : _tq.id,
                                        testpaperid = _tq == null ? 0 : _tq.test_paper_id,
                                        questiontype = _tq == null ? "null" : _tq.question_type,
                                        questiontitle = _tq == null ? "null" : _tq.question_title,
                                        questionsort = _tq == null ? 0 : _tq.question_sort,
                                        answeranalyze = _tq == null ? "null" : _tq.answer_analyze,
                                        standanswer = _tq == null ? "null" : _tq.question_answer,
                                        questionscore = _tq == null ? 0 : _tq.question_score
                                    };
                int i = 0;
                foreach (var item in queryQuestion)
                {
                    //筛选题干下的选项
                    var t = optionsList.FindAll(x => x.QuestionID == item.id);
                    //查找学生作答结果
                    string strAnswer = "";
                    int? Score = 0;
                    string strCorrectFlag = "0";
                    var queryAnswerLog = from a in db.t_answer_log
                                         where a.delete_flag == 0 && a.record_id == recordid && a.item_id == item.id
                                         select a;
                    var queryAnswerLogF = queryAnswerLog.FirstOrDefault();
                    if (queryAnswerLogF != null)
                    {
                        strAnswer = queryAnswerLogF.answer_result;
                        Score = queryAnswerLogF.score;
                        strCorrectFlag = queryAnswerLogF.correct_flag;
                    }

                    //添加到题干中
                    QuestionList.Add(new Questions()
                    {
                        ID = item.id,
                        Index = ++i,//题号
                        TestPaperID = item.testpaperid,
                        QuestionTitle = item.questiontitle,
                        QuestionType = item.questiontype,
                        QuestionSort = item.questionsort,
                        OptionsList = t,
                        QuestionAnswer = strAnswer,
                        // AnswerAnalyze = item.answeranalyze,
                        //  StandAnswer = item.standanswer,
                        Score = Score,
                        CorrectFlag = strCorrectFlag,
                        QuestionScore = item.questionscore
                    });
                }

                //查找相应的试卷
                var queryTestPaper = from p in db.t_test_papers
                                     where p.delete_flag == 0 && p.id == paperid
                                     select new { p.id, p.paper_title, p.exam_score };
                var queryTestPaper_F = queryTestPaper.FirstOrDefault();
                if (queryTestPaper_F != null)
                {
                    testPaper.ID = queryTestPaper_F.id;
                    testPaper.PaperTitle = queryTestPaper_F.paper_title;
                    testPaper.QuestionsList = QuestionList;
                    testPaper.ExamScore = sumScore;
                    testPaper.PaperScore = queryTestPaper_F.exam_score;
                    if (queryTestPaper_F.exam_score != null)//分母不能为零
                    {
                        float? f = sumScore / queryTestPaper_F.exam_score;
                        float ff = (float)f * 100;
                        testPaper.CorrectRate = ff.ToString("0.00");
                    }
                }
                return new { code = 200, result = testPaper, msg = "Ok" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 获取学员作答查看界面信息
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        public object GetStuTheoryExamPaperResult(pf_examinationContext db,long ExamID, string UserNumber)
        {
            try
            {
                    //查找学生作答的记录ID
                    long recordid = 0;
                    //试卷总得分
                    float? sumScore = 0;
                    var queryRecord = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.examination_id == ExamID && r.user_number == UserNumber
                                      select r;
                    var queryRecordF = queryRecord.FirstOrDefault();
                    if (queryRecordF != null)
                    {
                        recordid = queryRecordF.id;
                        sumScore = queryRecordF.score;
                    }

                    //选项集合
                    List<Option> optionsList = new List<Option>();
                    //题干集合
                    List<Questions> QuestionList = new List<Questions>();
                    //试卷
                    TestPaper testPaper = new TestPaper();
                    var query = from m in db.t_test_papers
                                where m.delete_flag == 0 && m.examination_id == ExamID
                                select m;
                    var qq = query.FirstOrDefault();
                    if (qq == null)
                        return new { code = 401, msg = "试卷不存在" };

                    long? paperid = qq.id;

                    //查找试卷下的所有选项
                    var queryOption = from t in db.t_test_papers
                                      join q in db.t_questions on t.id equals q.test_paper_id
                                      join o in db.t_question_option on q.id equals o.question_id into qo
                                      from _qo in qo.DefaultIfEmpty()
                                      where t.id == paperid && _qo.delete_flag == 0
                                      orderby _qo.option_number ascending
                                      select new
                                      {
                                          id = _qo == null ? 0 : _qo.id,
                                          questionid = _qo == null ? 0 : _qo.question_id,
                                          optionnum = _qo == null ? "null" : _qo.option_number,
                                          optioncontent = _qo == null ? "null" : _qo.option_content
                                      };
                    foreach (var item in queryOption)
                    {
                        optionsList.Add(new Option
                        {
                            ID = item.id,
                            QuestionID = item.questionid,
                            OptionNum = item.optionnum,
                            OptionContent = item.optioncontent
                        });
                    }

                    //查找试卷下的所有题干
                    var queryQuestion = from t in db.t_test_papers
                                        join q in db.t_questions on t.id equals q.test_paper_id into tq
                                        from _tq in tq.DefaultIfEmpty()
                                        where t.id == paperid && _tq.delete_flag == 0
                                        orderby _tq.id ascending
                                        select new
                                        {
                                            id = _tq == null ? 0 : _tq.id,
                                            testpaperid = _tq == null ? 0 : _tq.test_paper_id,
                                            questiontype = _tq == null ? "null" : _tq.question_type,
                                            questiontitle = _tq == null ? "null" : _tq.question_title,
                                            questionsort = _tq == null ? 0 : _tq.question_sort,
                                            answeranalyze = _tq == null ? "null" : _tq.answer_analyze,
                                            standanswer = _tq == null ? "null" : _tq.question_answer,
                                            questionscore = _tq == null ? 0 : _tq.question_score
                                        };
                    int i = 0;
                    foreach (var item in queryQuestion)
                    {
                        //筛选题干下的选项
                        var t = optionsList.FindAll(x => x.QuestionID == item.id);
                        //查找学生作答结果
                        string strAnswer = "";
                        int? Score = 0;
                        string strCorrectFlag = "0";
                        var queryAnswerLog = from a in db.t_answer_log
                                             where a.delete_flag == 0 && a.record_id == recordid && a.item_id == item.id
                                             select a;
                        var queryAnswerLogF = queryAnswerLog.FirstOrDefault();
                        if (queryAnswerLogF != null)
                        {
                            strAnswer = queryAnswerLogF.answer_result;
                            Score = queryAnswerLogF.score;
                            strCorrectFlag = queryAnswerLogF.correct_flag;
                        }

                        //添加到题干中
                        QuestionList.Add(new Questions()
                        {
                            ID = item.id,
                            Index = ++i,//题号
                            TestPaperID = item.testpaperid,
                            QuestionTitle = item.questiontitle,
                            QuestionType = item.questiontype,
                            QuestionSort = item.questionsort,
                            OptionsList = t,
                            QuestionAnswer = strAnswer,
                            AnswerAnalyze = item.answeranalyze,
                            StandAnswer = item.standanswer,
                            Score = Score,
                            CorrectFlag = strCorrectFlag,
                            QuestionScore = item.questionscore
                        });
                    }

                    //查找相应的试卷
                    var queryTestPaper = from p in db.t_test_papers
                                         where p.delete_flag == 0 && p.id == paperid
                                         select new { p.id, p.paper_title, p.exam_score };
                    var queryTestPaper_F = queryTestPaper.FirstOrDefault();
                    if (queryTestPaper_F != null)
                    {
                        if (sumScore == null)
                            sumScore = 0;
                        testPaper.ID = queryTestPaper_F.id;
                        testPaper.PaperTitle = queryTestPaper_F.paper_title;
                        testPaper.QuestionsList = QuestionList;
                        testPaper.ExamScore = sumScore;
                        testPaper.PaperScore = queryTestPaper_F.exam_score;
                        if (queryTestPaper_F.exam_score != null)//分母不能为零
                        {
                            float? f = sumScore / queryTestPaper_F.exam_score;
                            float ff = (float)f * 100;
                            testPaper.CorrectRate = ff.ToString("0.00");

                        }
                    }

                    return new { code = 200, result = testPaper, msg = "Ok" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public async Task<int> StuSubmitExamPaper(pf_examinationContext db,RabbitMQClient rabbit, AnswerLogList answerLogList, TokenModel token)
        {
            try
            {
                    if (answerLogList.answerLogs.Count > 0)
                    {
                        for (int i = 0; i < answerLogList.answerLogs.Count; i++)
                        {
                            t_answer_log obj = new t_answer_log();
                            obj.record_id = answerLogList.answerLogs[i].RecordID;
                            obj.item_id = answerLogList.answerLogs[i].ItemID;
                            obj.option_id = answerLogList.answerLogs[i].OptionID;
                            obj.answer_result = answerLogList.answerLogs[i].AnswerResult;
                            db.t_answer_log.Add(obj);
                        }

                    }

                    //产生日志消息
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = token.userNumber;
                    syslog.opName = token.userName;
                    syslog.opType = 2;
                    syslog.logDesc = "";
                    syslog.logSuccessd = 1;
                    syslog.moduleName = "我的考试";
                    rabbit.LogMsg(syslog);
                    return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                throw;
            }
        }

        /// <summary>
        /// 学员作答
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="ItemID"></param>
        /// <param name="AnswerResult"></param>
        /// <returns></returns>
        public async Task<object> StuAnswerQuestion(pf_examinationContext db,long RecordID, long ItemID, string AnswerResult)
        {
            try
            {
                    string questionAnswer = "";
                    int questionScore = 0;
                    var queryQuestionType = from q in db.t_questions
                                            where q.delete_flag == 0 && q.id == ItemID
                                            select q;
                    var queryQuestionTypeF = queryQuestionType.FirstOrDefault();
                    //1:单选，2：多选,3:填空，4：判断
                    if (queryQuestionTypeF.question_type == "1" || queryQuestionTypeF.question_type == "2" || queryQuestionTypeF.question_type == "3" || queryQuestionTypeF.question_type == "4")
                    {
                        //题库答案
                        questionAnswer = queryQuestionTypeF.question_answer;
                        if (queryQuestionTypeF.question_type == "2")//多选答案排序
                        {
                            List<char> list = AnswerResult.ToList<char>();
                            list.Sort();
                            AnswerResult = string.Join("", list.ToArray());
                        }
                        if (questionAnswer.Trim() == AnswerResult.Trim())
                        {
                            //试题分值
                            questionScore = queryQuestionTypeF.question_score;
                        }
                    }

                    //查找是否存在作答记录
                    var queryAnswerLog = from g in db.t_answer_log
                                         where g.delete_flag == 0 && g.record_id == RecordID && g.item_id == ItemID
                                         select g;
                    var queryAnswerLogF = queryAnswerLog.FirstOrDefault();
                    if (queryAnswerLogF == null)//不存在,则添加
                    {
                        t_answer_log log = new t_answer_log();
                        log.record_id = RecordID;
                        log.item_id = ItemID;
                        log.score = questionScore;
                        log.answer_result = AnswerResult;
                        log.correct_flag = "1";
                        db.t_answer_log.Add(log);
                    }
                    else//存在则修改
                    {
                        queryAnswerLogF.answer_result = AnswerResult;
                        queryAnswerLogF.score = questionScore;
                        if (queryQuestionTypeF.question_type == "1" || queryQuestionTypeF.question_type == "2" || queryQuestionTypeF.question_type == "3" || queryQuestionTypeF.question_type == "4")
                        {
                            queryAnswerLogF.correct_flag = "1";
                        }

                    }
                    await db.SaveChangesAsync();
                    return new { code = 200, msg = "OK" };
                
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 学员提交试卷
        /// </summary>
        /// <param name="RecordID"></param>
        /// <returns></returns>
        public async Task<object> SubmitStudentTestPaper(pf_examinationContext db,RabbitMQClient rabbit, long RecordID, TokenModel token)
        {
            try
            {
                    t_examination_manage queryExamF = null;
                    var queryRecord = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.id == RecordID
                                      select r;
                    var queryRecordF = queryRecord.FirstOrDefault();
                    if (queryRecordF != null)
                    {
                        int nPassScore = 0;
                        //考试管理ID
                        long examid = (long)queryRecordF.examination_id;
                        var queryExam = from e in db.t_examination_manage
                                        where e.delete_flag == 0 && e.id == examid
                                        select e;
                        queryExamF = queryExam.FirstOrDefault();
                        if (queryExamF != null)
                        {
                            nPassScore = (int)queryExamF.pass_scores;
                        }
                        bool IsExistSubjective = false;
                        //查看试卷下是否存在主观题
                        var queryPaper = (from t in db.t_test_papers
                                          join q in db.t_questions on t.id equals q.test_paper_id
                                          where t.delete_flag == 0 && q.delete_flag == 0 && q.question_type == "5" && t.examination_id == examid
                                          select q).ToList();
                        if (queryPaper.Count > 0)//存在主观题
                            IsExistSubjective = true;

                        //将客观题批改状态修改为已批改
                        var queryAnswer = from g in db.t_answer_log
                                          where g.record_id == RecordID && (from p in db.t_test_papers
                                                                            join q in db.t_questions on p.id equals q.test_paper_id
                                                                            where p.examination_id == examid && (q.question_type == "1" || q.question_type == "2" || q.question_type == "3" || q.question_type == "4")
                                                                            select q.id).Contains((long)g.item_id)
                                          select g;
                        foreach (var item in queryAnswer)
                        {
                            item.correct_flag = "1";
                        }

                        //计算总得分
                        int? sum = db.t_answer_log.Where(o => o.delete_flag == 0 && o.record_id == RecordID).Select(o => o.score).Sum();
                        if (nPassScore <= sum)//及格了
                        {
                            queryRecordF.pass_flag = 1;
                        }
                        queryRecordF.score = sum;
                        queryRecordF.end_time = DateTime.Now;//结束时间
                        if (IsExistSubjective)
                            queryRecordF.record_status = "3";//已结束
                        else
                            queryRecordF.record_status = "4";//已结束
                    }
                    await db.SaveChangesAsync();
                    //产生日志消息
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = token.userNumber;
                    syslog.opName = token.userName;
                    syslog.opType = 3;
                    syslog.logDesc = "提交了试卷：" + queryExamF.exam_name;
                    syslog.logSuccessd = 1;
                    syslog.moduleName = "我的考试";
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
}

public class TaskExamInfo
{
    /// <summary>
    /// 考试名称
    /// </summary>
    public string ExamName { get; set; }
    /// <summary>
    /// 开始时间
    /// </summary>
    public string StartTime { get; set; }
    /// <summary>
    /// 结束时间
    /// </summary>
    public string EndTime { get; set; }
    /// <summary>
    /// 考试说明
    /// </summary>
    public string ExamExplain { get; set; }
    /// <summary>
    /// 任务整体结果
    /// </summary>
    public string TaskExamResult { get; set; }
    /// <summary>
    /// 考试时长
    /// </summary>
    public int? ExamDuration { get; set; }
    /// <summary>
    /// 训练科目数量
    /// </summary>
    public int SubjectCount { get; set; }
    /// <summary>
    /// 训练完成数量
    /// </summary>
    public int FinishCount { get; set; }
    /// <summary>
    /// 训练通过数量
    /// </summary>
    public int PassCount { get; set; }
    public List<TrainSubject> trainSubjects { get; set; }
}

public class AnswerLogList
{
    public List<AnswerLog> answerLogs { get; set; }
}
public class AnswerLog
{
    public long RecordID { set; get; }
    public long ItemID { get; set; }
    public long OptionID { get; set; }
    public string AnswerResult { get; set; }
}

//一套试卷
public class TestPaper
{
    public long ID { get; set; }
    public string PaperTitle { get; set; }
    /// <summary>
    /// 试卷分值
    /// </summary>
    public int? PaperScore { get; set; }
    public int? QuestionCount { get; set; }
    public string PaperConfidential { get; set; }
    public string CorrectRate { get; set; }
    public string ApprovalUserName { get; set; }
    public DateTime? ApprovalDateTime { get; set; }
    public string ApprovalUserNumber { get; set; }
    public string ApprovalStatus { get; set; }
    /// <summary>
    /// 学生考试得分
    /// </summary>
    public float? ExamScore { get; set; }
    public DateTime? CreateTime { get; set; }
    public List<Questions> QuestionsList { get; set; }
}

/// <summary>
/// 一道试题
/// </summary>
public class Questions
{
    public long ID { get; set; }
    public int Index { get; set; }
    public long? TestPaperID { get; set; }
    public string QuestionType { get; set; }
    public int? QuestionSort { get; set; }
    /// <summary>
    /// 试题分值
    /// </summary>
    public int QuestionScore { get; set; }
    /// <summary>
    /// 批阅状态
    /// </summary>
    public string CorrectFlag { get; set; }
    public string Complexity { get; set; }
    public string QuestionTitle { get; set; }
    /// <summary>
    /// 考点分析
    /// </summary>
    public string AnswerAnalyze { get; set; }
    public string QuestionConfidential { get; set; }
    /// <summary>
    /// 学员作答答案
    /// </summary>
    public string QuestionAnswer { get; set; }
    /// <summary>
    /// 标准答案
    /// </summary>
    public string StandAnswer { get; set; }
    /// <summary>
    /// 得分
    /// </summary>
    public int? Score { get; set; }
    public List<Option> OptionsList { get; set; }
    public List<KnowledgeTag> KnowledgeTags { get; set; }
}
//选项
public class Option
{
    public long ID { get; set; }
    public long? QuestionID { get; set; }
    /// <summary>
    /// 选项编号
    /// </summary>
    public string OptionNum { get; set; }
    public string OptionContent { get; set; }
}

public class ExamFinishRate
{
    public long PlanId { get; set; }
    public long UserId { get; set; }
    public string UserNumber { get; set; }
    public decimal Rate { get; set; }
}


