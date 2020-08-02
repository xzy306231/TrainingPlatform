using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Examination.BLL
{
    public class ExaminationManage
    {
        public object GetExamination(pf_examinationContext db, string strStatus, string startTime, string endTime, string strKeyWord, int PageIndex, int PageSize)
        {
            try
            {
                IQueryable<t_examination_manage> query = null;
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            && (e.start_time >= DateTime.Parse(startTime))
                            && (e.end_time <= DateTime.Parse(endTime))
                            orderby e.t_create descending
                            select e;
                }
                else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            && (e.end_time <= DateTime.Parse(endTime))
                            orderby e.t_create descending
                            select e;
                }
                else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            && (e.start_time >= DateTime.Parse(startTime))
                            orderby e.t_create descending
                            select e;
                }
                else
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            orderby e.t_create descending
                            select e;
                }

                List<Examinations> list = new List<Examinations>();
                foreach (var item in query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList())
                {
                    //查询参考人数
                    var queryExam = from r in db.t_examination_record
                                    where r.delete_flag == 0 && r.examination_id == item.id
                                    select r;

                    //完成考试的人数
                    var queryFinish = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.examination_id == item.id && r.record_status != "1"
                                      select r;

                    list.Add(new Examinations()
                    {
                        ID = item.id,
                        EndTime = item.end_time.ToString(),
                        ExamDiv = item.exam_div,
                        ExamDuration = item.exam_duration,
                        ExamExplain = item.exam_explain,
                        ExamName = item.exam_name,
                        PassScore = item.pass_scores,
                        StartTime = item.start_time.ToString(),
                        ExamStatus = item.exam_status,
                        SumExamCount = queryExam.Count(),
                        FinishExamCount = queryFinish.Count(),
                        CorrectStatus = item.correct_status
                    });
                }
                return new { code = 200, result = list, count = query.Count(), msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetExaminationStatistic(pf_examinationContext db, string strStatus, string startTime, string endTime, string strKeyWord, int PageIndex, int PageSize)
        {
            try
            {
                IQueryable<t_examination_manage> query = null;
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.correct_status == "3"
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            && (string.IsNullOrEmpty(startTime) ? true : (e.start_time >= DateTime.Parse(startTime)))
                            && (string.IsNullOrEmpty(endTime) ? true : (e.end_time <= DateTime.Parse(endTime)))
                            orderby e.t_create descending
                            select e;
                }
                else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.correct_status == "3"
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            && (string.IsNullOrEmpty(endTime) ? true : (e.end_time <= DateTime.Parse(endTime)))
                            orderby e.t_create descending
                            select e;
                }
                else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.correct_status == "3"
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            && (string.IsNullOrEmpty(startTime) ? true : (e.start_time >= DateTime.Parse(startTime)))
                            orderby e.t_create descending
                            select e;
                }
                else
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.correct_status == "3"
                            && (string.IsNullOrEmpty(strStatus) ? true : e.exam_status.Contains(strStatus))
                            && (string.IsNullOrEmpty(strKeyWord) ? true : e.exam_name.Contains(strKeyWord))
                            orderby e.t_create descending
                            select e;
                }

                List<Examinations> list = new List<Examinations>();
                foreach (var item in query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList())
                {
                    //查询参考人数
                    var queryExam = from r in db.t_examination_record
                                    where r.delete_flag == 0 && r.examination_id == item.id
                                    select r;

                    //完成考试的人数
                    var queryFinish = from r in db.t_examination_record
                                      where r.delete_flag == 0 && r.examination_id == item.id && r.record_status != "1"
                                      select r;

                    list.Add(new Examinations()
                    {
                        ID = item.id,
                        EndTime = item.end_time.ToString(),
                        ExamDiv = item.exam_div,
                        ExamDuration = item.exam_duration,
                        ExamExplain = item.exam_explain,
                        ExamName = item.exam_name,
                        PassScore = item.pass_scores,
                        StartTime = item.start_time.ToString(),
                        ExamStatus = item.exam_status,
                        SumExamCount = queryExam.Count(),
                        FinishExamCount = queryFinish.Count(),
                        CorrectStatus = item.correct_status
                    });
                }
                return new { code = 200, result = list, count = query.Count(), msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetExaminationToPlan(pf_examinationContext db, string KeyWord, string FieldName, bool IsAsc, int PageIndex, int PageSize)
        {
            try
            {
                List<Examinations> list = new List<Examinations>();
                IQueryable<t_examination_manage> query = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.used_flag == 0 && e.approval_status == "3"
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby PubMethod.GetPropertyValue(e, FieldName) ascending
                            select e;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.used_flag == 0 && e.approval_status == "3"
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby PubMethod.GetPropertyValue(e, FieldName) descending
                            select e;
                }
                else
                {
                    query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.used_flag == 0 && e.approval_status == "3"
                            && (string.IsNullOrEmpty(KeyWord) ? true : e.exam_name.Contains(KeyWord))
                            orderby e.t_create descending
                            select e;
                }
                foreach (var item in query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList())
                {
                    list.Add(new Examinations()
                    {
                        ID = item.id,
                        EndTime = item.end_time.ToString(),
                        ExamDiv = item.exam_div,
                        ExamDuration = item.exam_duration,
                        ExamExplain = item.exam_explain,
                        ExamName = item.exam_name,
                        PassScore = item.pass_scores,
                        StartTime = item.start_time.ToString(),
                        ExamStatus = item.exam_status,
                        CreateName = item.create_name
                    });
                }
                return new { code = 200, result = list, count = query.Count(), msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object UpdateExaminationStatus(pf_examinationContext db,RabbitMQClient rabbit, List<ExaminationID> examinations, TokenModel token)
        {
            try
            {
                string examName = "";
                if (examinations != null && examinations.Count > 0)
                {
                    for (int i = 0; i < examinations.Count; i++)
                    {
                        var query = from e in db.t_examination_manage
                                    where e.delete_flag == 0 && e.used_flag == 0 && e.id == examinations[i].ID
                                    select e;
                        foreach (var item in query)
                        {
                            examName = examName + "," + item.exam_name;
                            item.used_flag = 1;
                            item.start_time = DateTime.Parse(examinations[i].StartTime);
                            item.end_time = DateTime.Parse(examinations[i].EndTime);
                            item.content_id = examinations[i].ContentID;
                        }
                    }
                }
                db.SaveChanges();
                examName = examName.TrimStart(',');
                //产生日志消息 
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 3,
                    logDesc = "考试：" + examName + ",已被使用。",
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                //PubMethod.Log(syslog);
                return new { code = 200, msg = "OK" };


            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetStuExamStatus(pf_examinationContext db, long examId, string userNumber)
        {
            try
            {
                var queryRecord = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.examination_id == examId && r.user_number == userNumber
                                  select r;
                var queryRecordF = queryRecord.FirstOrDefault();
                if (queryRecordF != null)
                    return new { code = 200, result = queryRecordF.record_status, msg = "OK" };
                else
                    return new { code = 400, msg = "Error" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 获取理论考试监控信息
        /// </summary>
        /// <param name="examinationid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetTheoryExaminationStatus(pf_examinationContext db, long examinationid, string strExamStatus, bool IsAsc, string FieldName, int pageIndex, int pageSize, DicModel dicDepartment)
        {
            try
            {
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == examinationid
                                select new { e.exam_name, e.start_time, e.end_time, e.exam_duration, e.exam_explain, e.pass_scores };
                var queryExamFirst = queryExam.FirstOrDefault();

                var queryPaper = from ex in db.t_examination_manage
                                 join p in db.t_test_papers on ex.id equals p.examination_id
                                 where ex.id == examinationid && p.delete_flag == 0
                                 select new { p.id, p.paper_title, p.exam_score };

                //试卷名称
                var queryPaperInfo = queryPaper.FirstOrDefault();

                PaperInfo paperInfo = new PaperInfo();
                if (queryPaperInfo != null)
                {
                    var queryPaperQuestion = from q in db.t_questions
                                             where q.delete_flag == 0 && q.test_paper_id == queryPaperInfo.id
                                             select q;
                    paperInfo.PaperName = queryPaperInfo.paper_title;
                    paperInfo.QuestionCount = queryPaperQuestion.Count();   //试卷试题数量
                    paperInfo.ExamScore = queryPaperInfo.exam_score;
                }

                //参考总人数
                var querySumStu = from r in db.t_examination_record
                                  where r.examination_id == examinationid && r.delete_flag == 0
                                  select r;
                int querySumStuCount = querySumStu.Count();

                //完成人数
                var queryFiniehed = from r in db.t_examination_record
                                    where r.examination_id == examinationid && r.delete_flag == 0 && (r.record_status == "3" || r.record_status == "4")
                                    select r;
                int queryFiniehedCount = queryFiniehed.Count();

                //进行中
                var queryWorking = from r in db.t_examination_record
                                   where r.examination_id == examinationid && r.delete_flag == 0 && r.record_status == "2"
                                   select r;
                int queryWorkingCount = queryWorking.Count();

                //缺考人数
                var queryLack = from r in db.t_examination_record
                                where r.examination_id == examinationid && r.delete_flag == 0 && r.record_status == "1"
                                select r;
                int queryLackCount = queryLack.Count();
                IQueryable<t_examination_record> query = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {
                    query = from r in db.t_examination_record
                            where r.examination_id == examinationid && r.delete_flag == 0
                                    && (strExamStatus == "1" ? (r.record_status == "1" || r.record_status == "2") : true)
                                    && (strExamStatus == "3" ? (r.record_status == "3" || r.record_status == "4") : true)
                            orderby PubMethod.GetPropertyValue(r, FieldName) ascending
                            select r;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    query = from r in db.t_examination_record
                            where r.examination_id == examinationid && r.delete_flag == 0
                                    && (strExamStatus == "1" ? (r.record_status == "1" || r.record_status == "2") : true)
                                    && (strExamStatus == "3" ? (r.record_status == "3" || r.record_status == "4") : true)
                            orderby PubMethod.GetPropertyValue(r, FieldName) descending
                            select r;
                }
                else
                {
                    query = from r in db.t_examination_record
                            where r.examination_id == examinationid && r.delete_flag == 0
                                    && (strExamStatus == "1" ? (r.record_status == "1" || r.record_status == "2") : true)
                                    && (strExamStatus == "3" ? (r.record_status == "3" || r.record_status == "4") : true)
                            orderby r.end_time descending
                            select r;
                }
                int Count = query.Count();
                List<StuExamInfo> list = new List<StuExamInfo>();
                foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    string strDepartment = "";//部门
                    try
                    {
                        if (!string.IsNullOrEmpty(item.department))
                            strDepartment = dicDepartment.Result.Find(x => x.DicCode == item.department).CodeDsc;
                    }
                    catch (Exception)
                    {
                        // strDepartment = "未知";
                    }
                    list.Add(new StuExamInfo()
                    {
                        Name = item.user_name,
                        Department = strDepartment,
                        StartTime = item.start_time,
                        EndTime = item.end_time,
                        ExamStatus = item.record_status
                    });
                }

                return new
                {
                    code = 200,
                    result = new
                    {
                        queryExamInfo = queryExamFirst,//考试管理信息
                        paperInfo = paperInfo,//试卷信息
                        querySumStuCount = querySumStuCount,//参考总人数
                        queryFiniehedCount = queryFiniehedCount,//完成人数
                        queryWorkingCount = queryWorkingCount,//进行中人数
                        queryLackCount = queryLackCount,//缺考人数
                        StuExamInfo = list,
                        Count = Count
                    },
                    msg = "OK"
                };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        ///  根据ID获取理论考试信息
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public object GetTheoryExaminationByID(pf_examinationContext db, long examid)
        {
            try
            {
                Examinations exam = new Examinations();

                var queryExamination = from e in db.t_examination_manage
                                       where e.delete_flag == 0 && e.id == examid
                                       select e;
                var queryExaminationF = queryExamination.FirstOrDefault();
                if (queryExaminationF != null)
                {
                    exam.ExamName = queryExaminationF.exam_name;
                    exam.StartTime = queryExaminationF.start_time.ToString();
                    exam.EndTime = queryExaminationF.end_time.ToString();
                    exam.ExamDiv = queryExaminationF.exam_div;
                    exam.ExamStatus = queryExaminationF.exam_status;
                    exam.ExamDuration = queryExaminationF.exam_duration;
                    exam.ExamExplain = queryExaminationF.exam_explain;
                    exam.PassScore = queryExaminationF.pass_scores;

                    var queryTeacher = from t in db.t_grade_teacher
                                       where t.delete_flag == 0 && t.examination_id == examid
                                       select t;
                    List<TeacherInfo> teacherInfos = new List<TeacherInfo>();
                    foreach (var item in queryTeacher)
                    {
                        teacherInfos.Add(new TeacherInfo()
                        {
                            TeacherNum = item.grade_teacher_num,
                            TeacherName = item.grade_teacher_name
                        });
                    }
                    exam.teacherInfos = teacherInfos;

                    var queryPaper = from p in db.t_test_papers
                                     where p.delete_flag == 0 && p.examination_id == examid
                                     select p;
                    var queryPaperF = queryPaper.FirstOrDefault();
                    if (queryPaperF != null)
                    {
                        List<Question> questions = new List<Question>();
                        var queryQuestion = from q in db.t_questions
                                            where q.delete_flag == 0 && q.test_paper_id == queryPaperF.id
                                            select q;
                        foreach (var item in queryQuestion)
                        {
                            //查找知识点
                            var queryTag = from r in db.t_question_knowledge_ref
                                           join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                           where r.question_id == item.id
                                           select t;
                            List<KnowledgeTag> tags = new List<KnowledgeTag>();
                            foreach (var tag in queryTag)
                            {
                                tags.Add(new KnowledgeTag()
                                {
                                    ID = tag.src_id,
                                    Tag = tag.tag
                                });
                            }

                            //查找选项
                            var queryOption = from o in db.t_question_option
                                              where o.delete_flag == 0 && o.question_id == item.id
                                              select o;
                            List<OptionInfo> options = new List<OptionInfo>();
                            foreach (var option in queryOption)
                            {
                                options.Add(new OptionInfo()
                                {
                                    OptionNum = option.option_number,
                                    OptionContent = option.option_content,
                                    RightFlag = option.right_flag
                                });
                            }

                            questions.Add(new Question()
                            {
                                QuestionType = item.question_type,//题型
                                AnswerAnalyze = item.answer_analyze,//考点分析
                                Complexity = item.complexity,//难易度
                                QuestionTitle = item.question_title,//题干信息
                                QuestionAnswer = item.question_answer,//答案
                                OptionInfoList = options,//选项
                                KnowledgeTags = tags,//知识点
                                Score = item.question_score//分值
                            });
                        }
                        PaperInfomation paper = new PaperInfomation();
                        Paper p = new Paper();
                        p.PaperTitle = queryPaperF.paper_title;
                        p.PaperScore = queryPaperF.exam_score;
                        if (queryPaperF.approval_status == "0" || queryPaperF.approval_status == "3")
                            p.EditFlag = "0";//不能编辑
                        else
                            p.EditFlag = "1";//能编辑
                        p.QuestionList = questions;
                        paper.PaperInfo = p;
                        exam.paperInfomation = paper;

                    }
                }
                //查找考试管理
                return new { code = 200, result = exam, message = "OK" };
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 创建理论考试
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        public object Add_TheoryExamination(pf_examinationContext db,RabbitMQClient rabbit, Examinations examination, IHttpClientHelper client, TokenModel token)
        {
            try
            {
                t_examination_manage q = new t_examination_manage();
                q.exam_name = examination.ExamName;
                q.exam_div = "1";
                q.exam_status = "1";//未开始
                q.exam_duration = examination.ExamDuration;
                q.paper_confidential = examination.PaperConfidential;
                q.exam_explain = examination.ExamExplain;
                q.pass_scores = examination.PassScore;
                q.correct_status = "1";//批改状态
                q.approval_status = "3";
                q.create_num = token.userNumber;
                q.create_name = token.userName;
                db.t_examination_manage.Add(q);

                db.SaveChanges();
                long MaxID = q.id;

                //初始化考试统计管理
                StatisticExam(db, MaxID, examination.paperInfomation.PaperInfo.PaperScore);

                //导入试卷
                TestPaperInfo.TestPaperImportToDB(db, examination.paperInfomation, "0", MaxID, token);

                //存在评分人员
                if (examination.teacherInfos != null && examination.teacherInfos.Count > 0)
                {

                    for (int i = 0; i < examination.teacherInfos.Count; i++)
                    {
                        t_grade_teacher t = new t_grade_teacher();
                        t.examination_id = MaxID;
                        t.grade_teacher_num = examination.teacherInfos[i].TeacherNum;
                        t.grade_teacher_name = examination.teacherInfos[i].TeacherName;
                        t.delete_flag = 0;
                        t.t_create = DateTime.Now;
                        t.t_modified = DateTime.Now;
                        db.t_grade_teacher.Add(t);
                    }
                }
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 2,
                    logDesc = "创建了理论考试：" + examination.ExamName,
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        private void StatisticExam(pf_examinationContext db, long MaxID, int? score)
        {
            try
            {
                //考试管理统计
                t_statistic_texam se = new t_statistic_texam();
                se.exam_id = MaxID;
                se.tscore = score;
                db.t_statistic_texam.Add(se);

                //正确率区间统计
                t_statistic_exam_accrate rate1 = new t_statistic_exam_accrate();
                rate1.exam_id = MaxID;
                rate1.acc_name = "20-0%区间";
                rate1.acc_index = 1;
                db.t_statistic_exam_accrate.Add(rate1);

                t_statistic_exam_accrate rate2 = new t_statistic_exam_accrate();
                rate2.exam_id = MaxID;
                rate2.acc_name = "40-20%区间";
                rate2.acc_index = 2;
                db.t_statistic_exam_accrate.Add(rate2);

                t_statistic_exam_accrate rate3 = new t_statistic_exam_accrate();
                rate3.exam_id = MaxID;
                rate3.acc_name = "60-40%区间";
                rate3.acc_index = 3;
                db.t_statistic_exam_accrate.Add(rate3);

                t_statistic_exam_accrate rate4 = new t_statistic_exam_accrate();
                rate4.exam_id = MaxID;
                rate4.acc_name = "80-60%区间";
                rate4.acc_index = 4;
                db.t_statistic_exam_accrate.Add(rate4);

                t_statistic_exam_accrate rate5 = new t_statistic_exam_accrate();
                rate5.exam_id = MaxID;
                rate5.acc_name = "100-80%区间";
                rate5.acc_index = 5;
                db.t_statistic_exam_accrate.Add(rate5);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                // return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 修改理论考试
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        public object Update_TheoryExamination(pf_examinationContext db,RabbitMQClient rabbit, Examinations examination, TokenModel token)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.id == examination.ID
                            select e;
                var q = query.FirstOrDefault();
                q.exam_name = examination.ExamName;
                q.exam_duration = examination.ExamDuration;
                q.exam_explain = examination.ExamExplain;
                q.pass_scores = examination.PassScore;
                q.paper_confidential = examination.PaperConfidential;
                q.approval_status = "3";

                //删除现有阅卷人员
                var queryTeacher = from t in db.t_grade_teacher
                                   where t.delete_flag == 0 && t.examination_id == examination.ID
                                   select t;
                List<TeacherInfo> teacherInfos = new List<TeacherInfo>();
                foreach (var item in queryTeacher)
                {
                    item.delete_flag = 1;
                }

                //新增评分人员
                if (examination.teacherInfos != null && examination.teacherInfos.Count > 0)
                {

                    for (int i = 0; i < examination.teacherInfos.Count; i++)
                    {
                        t_grade_teacher t = new t_grade_teacher();
                        t.examination_id = examination.ID;
                        t.grade_teacher_num = examination.teacherInfos[i].TeacherNum;
                        t.grade_teacher_name = examination.teacherInfos[i].TeacherName;
                        t.delete_flag = 0;
                        t.t_create = DateTime.Now;
                        t.t_modified = DateTime.Now;
                        db.t_grade_teacher.Add(t);
                    }
                }

                //更改考试统计总分数据
                var queryStatisticExam = from s in db.t_statistic_texam
                                         where s.delete_flag == 0 && s.exam_id == examination.ID
                                         select s;
                var queryStatisticExamF = queryStatisticExam.FirstOrDefault();
                if (queryStatisticExamF != null)
                    queryStatisticExamF.tscore = examination.paperInfomation.PaperInfo.PaperScore;

                //删除现有的试卷
                var queryPaper = from p in db.t_test_papers
                                 where p.delete_flag == 0 && p.examination_id == examination.ID
                                 select p;
                foreach (var item in queryPaper)
                {
                    item.delete_flag = 1;
                }
                //删除试题统计表数据
                var queryQuestion = (from a in db.t_statistic_question
                                     where a.delete_flag == 0 && a.exam_id == examination.ID
                                     select a).ToList();
                foreach (var item in queryQuestion)
                {
                    item.delete_flag = 1;
                }
                //导入试卷
                TestPaperInfo.TestPaperImportToDB(db, examination.paperInfomation, "0", examination.ID, token);
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 3,
                    logDesc = "修改了理论考试：" + examination.ExamName,
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        private object AddTheoryExamFromTestPapers(pf_examinationContext db, Paper paper)
        {
            try
            {
                //将试卷保存至数据库
                t_test_papers _paper = new t_test_papers();
                //_paper.examination_id = examid;//考试管理ID
                _paper.paper_title = paper.PaperTitle;
                _paper.exam_score = paper.PaperScore;
                _paper.paper_confidential = paper.PaperConfidential;
                _paper.question_count = paper.QuestionCount;
                db.t_test_papers.Add(_paper);
                db.SaveChanges();
                long paperid = _paper.id;

                if (paper.QuestionList != null && paper.QuestionList.Count > 0)
                {
                    List<ExamKnowledge> tags = new List<ExamKnowledge>();
                    for (int i = 0; i < paper.QuestionList.Count; i++)
                    {
                        string strAnswer = "";
                        //保存题干数据
                        t_questions question = new t_questions();
                        question.test_paper_id = paperid;
                        question.question_type = paper.QuestionList[i].QuestionType;
                        question.complexity = paper.QuestionList[i].Complexity;
                        question.question_title = paper.QuestionList[i].QuestionTitle;
                        question.answer_analyze = paper.QuestionList[i].AnswerAnalyze;//考点分析
                        strAnswer = paper.QuestionList[i].QuestionAnswer;
                        if (paper.QuestionList[i].QuestionType == "2")//多选
                        {
                            List<char> list = strAnswer.ToList<char>();
                            list.Sort();
                            strAnswer = string.Join("", list.ToArray());
                        }
                        question.question_answer = strAnswer;
                        question.question_score = paper.QuestionList[i].Score;
                        db.t_questions.Add(question);
                        db.SaveChanges();
                        //读出题干最大值
                        long questionid = question.id;

                        //添加统计题干数据
                        t_statistic_question q = new t_statistic_question();
                        //q.exam_id = examid;
                        q.question_id = questionid;
                        db.t_statistic_question.Add(q);
                        db.SaveChanges();
                        long statistic_questionid = q.id;

                        if (paper.QuestionList[i].KnowledgeTags != null && paper.QuestionList[i].KnowledgeTags.Count > 0)
                        {
                            for (int j = 0; j < paper.QuestionList[i].KnowledgeTags.Count; j++)
                            {
                                var queryTag = from t in db.t_knowledge_tag
                                               where t.delete_flag == 0
                                                     && t.src_id == paper.QuestionList[i].KnowledgeTags[j].ID
                                               select t;
                                if (queryTag.FirstOrDefault() != null)//存在副本知识点
                                {
                                    //建立关系
                                    t_question_knowledge_ref obj = new t_question_knowledge_ref();
                                    obj.question_id = questionid;
                                    obj.knowledge_tag_id = queryTag.FirstOrDefault().id;
                                    db.t_question_knowledge_ref.Add(obj);

                                    //添加知识点
                                    tags.Add(new ExamKnowledge()
                                    {
                                        // ExamID = examid,
                                        KnowledgeID = queryTag.FirstOrDefault().id,
                                        KnowledgeName = paper.QuestionList[i].KnowledgeTags[j].Tag
                                    });
                                }
                                else//不存在
                                {
                                    //新建知识点
                                    t_knowledge_tag tag = new t_knowledge_tag();
                                    tag.src_id = paper.QuestionList[i].KnowledgeTags[j].ID;
                                    tag.tag = paper.QuestionList[i].KnowledgeTags[j].Tag;
                                    db.t_knowledge_tag.Add(tag);
                                    db.SaveChanges();
                                    //读出知识点最大值
                                    long tagid = tag.id;

                                    //建立关系
                                    t_question_knowledge_ref obj = new t_question_knowledge_ref();
                                    obj.question_id = questionid;
                                    obj.knowledge_tag_id = tagid;
                                    db.t_question_knowledge_ref.Add(obj);

                                    //添加知识点
                                    tags.Add(new ExamKnowledge()
                                    {
                                        //ExamID = examid,
                                        KnowledgeID = tagid,
                                        KnowledgeName = paper.QuestionList[i].KnowledgeTags[j].Tag
                                    });

                                }

                            }
                        }
                        if (paper.QuestionList[i].OptionInfoList != null && paper.QuestionList[i].OptionInfoList.Count > 0)
                        {
                            for (int j = 0; j < paper.QuestionList[j].OptionInfoList.Count; j++)
                            {
                                //保存选项
                                t_question_option option = new t_question_option();
                                option.question_id = questionid;
                                option.option_number = paper.QuestionList[i].OptionInfoList[j].OptionNum;
                                option.option_content = paper.QuestionList[i].OptionInfoList[j].OptionContent;
                                option.right_flag = paper.QuestionList[i].OptionInfoList[j].RightFlag;
                                db.t_question_option.Add(option);
                                db.SaveChanges();
                                long optionid = option.id;

                                //添加统计选项数据
                                t_statistic_option p = new t_statistic_option();
                                p.statistic_qid = statistic_questionid;
                                p.option_id = optionid;
                                db.t_statistic_option.Add(p);
                            }
                        }
                    }
                }
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetExamPaper(pf_examinationContext db, long ExamID)
        {
            try
            {
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
                                        questionscore = _tq == null ? 0 : _tq.question_score,
                                        complexity = _tq == null ? "null" : _tq.complexity
                                    };
                int i = 0;
                foreach (var item in queryQuestion)
                {
                    //筛选题干下的选项
                    var t = optionsList.FindAll(x => x.QuestionID == item.id);
                    //查找知识点
                    var queryKnowledgeList = (from r in db.t_question_knowledge_ref
                                              join g in db.t_knowledge_tag on r.knowledge_tag_id equals g.id
                                              where r.question_id == item.id && r.delete_flag == 0
                                              select g).ToList();
                    List<KnowledgeTag> KnowledgeTags = new List<KnowledgeTag>();
                    foreach (var tag in queryKnowledgeList)
                    {
                        KnowledgeTags.Add(new KnowledgeTag
                        {
                            ID = tag.src_id,
                            Tag = tag.tag
                        });
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
                        AnswerAnalyze = item.answeranalyze,
                        StandAnswer = item.standanswer,
                        Complexity = item.complexity,
                        QuestionScore = item.questionscore,
                        KnowledgeTags = KnowledgeTags
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
                    testPaper.PaperScore = queryTestPaper_F.exam_score;
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
        /// 获取实践考试监控信息
        /// </summary>
        /// <returns></returns>
        public object GetTaskExaminationStatus(pf_examinationContext db, long ID, string strExamStatus, bool IsAsc, string FieldName, int pageIndex, int pageSize, DicModel dicDepartment)
        {
            try
            {
                TaskExamination task = new TaskExamination();
                //考试管理信息
                var query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.id == ID
                            select e;
                var q = query.FirstOrDefault();
                if (q != null)
                {
                    task.ExamName = q.exam_name;
                    task.StartTime = q.start_time.ToString();
                    task.EndTime = q.end_time.ToString();
                    task.ExamDuration = q.exam_duration;
                    task.ExamExplain = q.exam_explain;
                }

                //训练任务信息
                var queryTask = from t in db.t_training_task
                                where t.examination_id == ID && t.delete_flag == 0
                                select t;
                var qTask = queryTask.FirstOrDefault();
                if (qTask != null)
                {
                    TrainTask trainTask = new TrainTask();
                    trainTask.ID = qTask.src_id;
                    trainTask.TaskName = qTask.task_name;
                    trainTask.TaskType = qTask.task_type;
                    trainTask.KindLevel = qTask.kind_level;
                    trainTask.RankLevel = qTask.rank_level;
                    task.trainTask = trainTask;
                }
                IQueryable<t_examination_record> queryRecord = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {
                    queryRecord = from r in db.t_examination_record
                                  where r.examination_id == ID && r.delete_flag == 0
                                        && (strExamStatus == "1" ? (r.record_status == "1" || r.record_status == "2") : true)
                                        && (strExamStatus == "3" ? (r.record_status == "3") : true)
                                  orderby PubMethod.GetPropertyValue(r, FieldName) ascending
                                  select r;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    queryRecord = from r in db.t_examination_record
                                  where r.examination_id == ID && r.delete_flag == 0
                                        && (strExamStatus == "1" ? (r.record_status == "1" || r.record_status == "2") : true)
                                        && (strExamStatus == "3" ? (r.record_status == "3") : true)
                                  orderby PubMethod.GetPropertyValue(r, FieldName) descending
                                  select r;
                }
                else
                {
                    queryRecord = from r in db.t_examination_record
                                  where r.examination_id == ID && r.delete_flag == 0
                                        && (strExamStatus == "1" ? (r.record_status == "1" || r.record_status == "2") : true)
                                        && (strExamStatus == "3" ? (r.record_status == "3") : true)
                                  orderby r.end_time descending
                                  select r;
                }
                var querySum = from r in db.t_examination_record
                               where r.delete_flag == 0 && r.examination_id == ID
                               select r;
                var Count = querySum.Count();

                var queryFinish = from r in db.t_examination_record
                                  where r.delete_flag == 0 && r.examination_id == ID && r.record_status == "3"
                                  select r;
                var FinishCount = queryFinish.Count();
                var NotFinishCount = Count - FinishCount;

                List<StuExamInfo> stuList = new List<StuExamInfo>();
                foreach (var item in queryRecord.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    string strDepartment = "";//部门
                    try
                    {
                        if (!string.IsNullOrEmpty(item.department))
                            strDepartment = dicDepartment.Result.Find(x => x.DicCode == item.department).CodeDsc;
                    }
                    catch (Exception)
                    {
                    }
                    stuList.Add(new StuExamInfo()
                    {
                        Name = item.user_name,
                        Department = strDepartment,
                        StartTime = item.start_time,
                        EndTime = item.end_time,
                        ExamStatus = item.record_status
                    });
                }

                return new { code = 200, result = new { taskInfo = task, StuInfo = new { stuList = stuList, SumCount = Count, NotFinishCount = NotFinishCount } }, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 根据ID获取实践考试信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object GetTaskExaminationByID(pf_examinationContext db, long ID)
        {
            try
            {
                TaskExamination task = new TaskExamination();
                //考试管理信息
                var query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.id == ID
                            select e;
                var q = query.FirstOrDefault();
                if (q != null)
                {
                    task.ID = q.id;
                    task.ExamName = q.exam_name;
                    task.StartTime = q.start_time.ToString();
                    task.EndTime = q.end_time.ToString();
                    task.ExamDuration = q.exam_duration;
                    task.ExamExplain = q.exam_explain;

                }

                //训练任务信息
                var queryTask = from t in db.t_training_task
                                where t.examination_id == ID && t.delete_flag == 0
                                select t;
                var qTask = queryTask.FirstOrDefault();
                if (qTask != null)
                {
                    TrainTask trainTask = new TrainTask();
                    trainTask.ID = qTask.src_id;
                    trainTask.TaskName = qTask.task_name;
                    trainTask.TaskType = qTask.task_type;
                    trainTask.KindLevel = qTask.kind_level;
                    trainTask.RankLevel = qTask.rank_level;
                    trainTask.PlaneType = qTask.plane_type;
                    task.trainTask = trainTask;


                    //训练科目信息
                    var querySubject = from s in db.t_training_subject
                                       where s.delete_flag == 0 && s.task_id == qTask.id
                                       select s;
                    List<TrainSubject> list = new List<TrainSubject>();
                    foreach (var item in querySubject)
                    {
                        var queryTag = from t in db.t_knowledge_tag
                                       join r in db.t_subject_knowledge_ref on t.id equals r.knowledge_tag_id
                                       where r.subject_id == item.id
                                       select t;
                        List<Tag> tags = new List<Tag>();
                        foreach (var t in queryTag)
                        {
                            Tag tag = new Tag();
                            tag.ID = t.src_id;
                            tag.TagName = t.tag;
                            tags.Add(tag);
                        }

                        TrainSubject subject = new TrainSubject();
                        subject.ID = item.id;
                        subject.TrainNumber = item.train_number;
                        subject.TrainName = item.train_name;
                        subject.TrainDesc = item.train_desc;
                        subject.TrainKind = item.train_kind;
                        subject.PlaneType = item.plane_type;
                        subject.ExpectResult = item.expect_result;
                        subject.Tags = tags;//知识点
                        list.Add(subject);
                    }
                    task.trainTask.trainSubjects = list;
                }

                //阅卷老师信息
                var queryTeacher = from t in db.t_grade_teacher
                                   where t.examination_id == ID && t.delete_flag == 0
                                   select t;
                List<TeacherInfo> teacherList = new List<TeacherInfo>();
                foreach (var item in queryTeacher)
                {
                    teacherList.Add(new TeacherInfo()
                    {
                        TeacherNum = item.grade_teacher_num,
                        TeacherName = item.grade_teacher_name
                    });
                    task.teacherInfos = teacherList;
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
        /// 新增实践考试
        /// </summary>
        /// <param name="taskExamination"></param>
        /// <returns></returns>
        public object Add_TaskExamination(pf_examinationContext db,RabbitMQClient rabbit, TaskExamination taskExamination, TokenModel obj)
        {
            try
            {
                t_examination_manage exam = new t_examination_manage();
                exam.exam_name = taskExamination.ExamName;
                exam.exam_div = "2";//实践考试
                exam.exam_status = "1";//未开始
                exam.correct_status = "2";//阅卷状态2：进行中
                exam.exam_duration = taskExamination.ExamDuration;
                exam.exam_explain = taskExamination.ExamExplain;
                exam.approval_status = "3";
                exam.create_num = obj.userNumber;
                exam.create_name = obj.userName;
                db.t_examination_manage.Add(exam);
                db.SaveChanges();
                //读出最大值
                long examid = exam.id;

                //考试管理统计
                t_statistic_texam se = new t_statistic_texam();
                se.exam_id = examid;
                db.t_statistic_texam.Add(se);

                //正确率区间统计
                t_statistic_exam_accrate rate1 = new t_statistic_exam_accrate();
                rate1.exam_id = examid;
                rate1.acc_name = "20-0%区间";
                rate1.acc_index = 1;
                db.t_statistic_exam_accrate.Add(rate1);

                t_statistic_exam_accrate rate2 = new t_statistic_exam_accrate();
                rate2.exam_id = examid;
                rate2.acc_name = "40-20%区间";
                rate2.acc_index = 2;
                db.t_statistic_exam_accrate.Add(rate2);

                t_statistic_exam_accrate rate3 = new t_statistic_exam_accrate();
                rate3.exam_id = examid;
                rate3.acc_name = "60-40%区间";
                rate3.acc_index = 3;
                db.t_statistic_exam_accrate.Add(rate3);

                t_statistic_exam_accrate rate4 = new t_statistic_exam_accrate();
                rate4.exam_id = examid;
                rate4.acc_name = "80-60%区间";
                rate4.acc_index = 4;
                db.t_statistic_exam_accrate.Add(rate4);

                t_statistic_exam_accrate rate5 = new t_statistic_exam_accrate();
                rate5.exam_id = examid;
                rate5.acc_name = "100-80%区间";
                rate5.acc_index = 5;
                db.t_statistic_exam_accrate.Add(rate5);

                if (taskExamination.teacherInfos != null && taskExamination.teacherInfos.Count > 0)
                {
                    //阅卷老师
                    for (int i = 0; i < taskExamination.teacherInfos.Count; i++)
                    {
                        t_grade_teacher teacher = new t_grade_teacher();
                        teacher.examination_id = examid;
                        teacher.grade_teacher_num = taskExamination.teacherInfos[i].TeacherNum;
                        teacher.grade_teacher_name = taskExamination.teacherInfos[i].TeacherName;
                        db.t_grade_teacher.Add(teacher);
                    }
                }

                if (taskExamination.trainTask != null)
                {
                    //添加训练任务
                    t_training_task task = new t_training_task();
                    task.examination_id = examid;
                    task.src_id = taskExamination.trainTask.ID;
                    task.task_name = taskExamination.trainTask.TaskName;
                    task.task_type = taskExamination.trainTask.TaskType;
                    task.kind_level = taskExamination.trainTask.KindLevel;
                    task.rank_level = taskExamination.trainTask.RankLevel;
                    task.plane_type = taskExamination.trainTask.PlaneType;
                    db.t_training_task.Add(task);
                    db.SaveChanges();
                    if (taskExamination.trainTask.trainSubjects != null && taskExamination.trainTask.trainSubjects.Count > 0)
                    {
                        //读出最大值
                        long taskid = task.id;
                        for (int i = 0; i < taskExamination.trainTask.trainSubjects.Count; i++)
                        {
                            t_training_subject subject = new t_training_subject();
                            subject.task_id = taskid;
                            subject.train_number = taskExamination.trainTask.trainSubjects[i].TrainNumber;
                            subject.train_name = taskExamination.trainTask.trainSubjects[i].TrainName;
                            subject.train_desc = taskExamination.trainTask.trainSubjects[i].TrainDesc;
                            subject.train_kind = taskExamination.trainTask.trainSubjects[i].TrainKind;
                            subject.plane_type = taskExamination.trainTask.trainSubjects[i].PlaneType;
                            subject.expect_result = taskExamination.trainTask.trainSubjects[i].ExpectResult;
                            db.t_training_subject.Add(subject);
                            db.SaveChanges();
                            //读出最大值
                            long subjectid = subject.id;

                            //训练科目统计
                            t_statistic_subject objSubject = new t_statistic_subject();
                            objSubject.exam_id = examid;
                            objSubject.task_id = taskid;
                            objSubject.subject_id = subjectid;
                            db.t_statistic_subject.Add(objSubject);

                            if (taskExamination.trainTask.trainSubjects[i].Tags != null && taskExamination.trainTask.trainSubjects[i].Tags.Count > 0)
                            {
                                List<Knowledge> tags = new List<Knowledge>();
                                for (int j = 0; j < taskExamination.trainTask.trainSubjects[i].Tags.Count; j++)
                                {
                                    var queryTag = from t in db.t_knowledge_tag
                                                   where t.delete_flag == 0 && t.src_id == taskExamination.trainTask.trainSubjects[i].Tags[j].ID
                                                   select t;
                                    if (queryTag.FirstOrDefault() == null)//知识点不存在
                                    {
                                        //新增知识点
                                        t_knowledge_tag tag = new t_knowledge_tag();
                                        tag.src_id = taskExamination.trainTask.trainSubjects[i].Tags[j].ID;
                                        tag.tag = taskExamination.trainTask.trainSubjects[i].Tags[j].TagName;
                                        db.t_knowledge_tag.Add(tag);
                                        db.SaveChanges();

                                        //读出最大值
                                        long tagid = tag.id;

                                        //建立关系
                                        t_subject_knowledge_ref s = new t_subject_knowledge_ref();
                                        s.knowledge_tag_id = tagid;
                                        s.subject_id = subjectid;
                                        db.t_subject_knowledge_ref.Add(s);

                                        //添加知识点
                                        tags.Add(new Knowledge()
                                        {
                                            ExamID = examid,
                                            KnowledgeID = tagid,
                                            KnowledgeName = taskExamination.trainTask.trainSubjects[i].Tags[j].TagName
                                        });
                                    }
                                    else//知识点存在
                                    {
                                        //建立关系
                                        t_subject_knowledge_ref s = new t_subject_knowledge_ref();
                                        s.knowledge_tag_id = queryTag.FirstOrDefault().id;
                                        s.subject_id = subjectid;
                                        db.t_subject_knowledge_ref.Add(s);

                                        //添加知识点
                                        tags.Add(new Knowledge()
                                        {
                                            ExamID = examid,
                                            KnowledgeID = queryTag.FirstOrDefault().id,
                                            KnowledgeName = queryTag.FirstOrDefault().tag
                                        });
                                    }
                                }
                                AddKnowledge(db, tags);
                            }
                        }
                        db.SaveChanges();
                    }
                }

                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = obj.userNumber,
                    opName = obj.userName,
                    opType = 2,
                    logDesc = "创建了实践考试：" + taskExamination.ExamName,
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        private void AddKnowledge(pf_examinationContext db, List<Knowledge> knowledges)
        {
            try
            {
                if (knowledges != null && knowledges.Count > 0)
                {
                    //去重操作
                    knowledges = knowledges.GroupBy(k => k.KnowledgeID).Select(x => x.First()).ToList();

                    for (int i = 0; i < knowledges.Count; i++)
                    {
                        t_statistic_subject_knowledge k = new t_statistic_subject_knowledge();
                        k.exam_id = knowledges[i].ExamID;
                        k.know_id = knowledges[i].KnowledgeID;
                        k.know_name = knowledges[i].KnowledgeName;
                        db.t_statistic_subject_knowledge.Add(k);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }

        /// <summary>
        /// 修改实践考试
        /// </summary>
        /// <param name="taskExamination"></param>
        /// <returns></returns>
        public object Update_TaskExamination(pf_examinationContext db,RabbitMQClient rabbit, TaskExamination taskExamination, TokenModel token)
        {
            try
            {
                var queryExam = from e in db.t_examination_manage
                                where e.delete_flag == 0 && e.id == taskExamination.ID
                                select e;
                var q = queryExam.FirstOrDefault();
                q.exam_name = taskExamination.ExamName;
                q.exam_duration = taskExamination.ExamDuration;
                q.exam_explain = taskExamination.ExamExplain;

                //删除阅卷人员
                var queryTeach = from t in db.t_grade_teacher
                                 where t.delete_flag == 0 && t.examination_id == taskExamination.ID
                                 select t;
                foreach (var item in queryTeach)
                {
                    item.delete_flag = 1;
                }

                //添加新的阅卷人员
                if (taskExamination.teacherInfos != null && taskExamination.teacherInfos.Count > 0)
                {
                    for (int i = 0; i < taskExamination.teacherInfos.Count; i++)
                    {
                        t_grade_teacher teacher = new t_grade_teacher();
                        teacher.examination_id = taskExamination.ID;
                        teacher.grade_teacher_num = taskExamination.teacherInfos[i].TeacherNum;
                        teacher.grade_teacher_name = taskExamination.teacherInfos[i].TeacherName;
                        db.t_grade_teacher.Add(teacher);
                    }
                }

                var queryTask = from t in db.t_training_task
                                where t.delete_flag == 0 && t.examination_id == taskExamination.ID
                                select t;
                var qTask = queryTask.FirstOrDefault();
                if (qTask != null)
                {
                    //删除存在的任务
                    qTask.delete_flag = 1;
                }
                if (taskExamination.trainTask != null)
                {
                    //创建任务
                    t_training_task task = new t_training_task();
                    task.examination_id = taskExamination.ID;
                    task.src_id = taskExamination.trainTask.ID;
                    task.task_name = taskExamination.trainTask.TaskName;
                    task.task_type = taskExamination.trainTask.TaskType;
                    task.kind_level = taskExamination.trainTask.KindLevel;
                    task.rank_level = taskExamination.trainTask.RankLevel;
                    task.plane_type = taskExamination.trainTask.PlaneType;
                    db.t_training_task.Add(task);
                    db.SaveChanges();

                    if (taskExamination.trainTask.trainSubjects != null && taskExamination.trainTask.trainSubjects.Count > 0)
                    {
                        //读出最大值
                        long taskid = task.id;
                        for (int i = 0; i < taskExamination.trainTask.trainSubjects.Count; i++)
                        {
                            t_training_subject subject = new t_training_subject();
                            subject.task_id = taskid;
                            subject.train_number = taskExamination.trainTask.trainSubjects[i].TrainNumber;
                            subject.train_name = taskExamination.trainTask.trainSubjects[i].TrainName;
                            subject.train_desc = taskExamination.trainTask.trainSubjects[i].TrainDesc;
                            subject.train_kind = taskExamination.trainTask.trainSubjects[i].TrainKind;
                            subject.plane_type = taskExamination.trainTask.trainSubjects[i].PlaneType;
                            subject.expect_result = taskExamination.trainTask.trainSubjects[i].ExpectResult;
                            db.t_training_subject.Add(subject);
                            db.SaveChanges();

                            if (taskExamination.trainTask.trainSubjects[i].Tags != null && taskExamination.trainTask.trainSubjects[i].Tags.Count > 0)
                            {
                                //读出最大值
                                long subjectid = subject.id;
                                for (int j = 0; j < taskExamination.trainTask.trainSubjects[i].Tags.Count; j++)
                                {
                                    var queryTag = from t in db.t_knowledge_tag
                                                   where t.delete_flag == 0 && t.src_id == taskExamination.trainTask.trainSubjects[i].Tags[j].ID
                                                   select t;
                                    if (queryTag.FirstOrDefault() == null)//知识点不存在
                                    {
                                        //新增知识点
                                        t_knowledge_tag tag = new t_knowledge_tag();
                                        tag.src_id = taskExamination.trainTask.trainSubjects[i].Tags[j].ID;
                                        tag.tag = taskExamination.trainTask.trainSubjects[i].Tags[j].TagName;
                                        db.t_knowledge_tag.Add(tag);
                                        db.SaveChanges();

                                        //读出最大值
                                        long tagid = tag.id;

                                        //建立关系
                                        t_subject_knowledge_ref s = new t_subject_knowledge_ref();
                                        s.knowledge_tag_id = tagid;
                                        s.subject_id = subjectid;
                                        db.t_subject_knowledge_ref.Add(s);
                                        //db.SaveChanges();
                                    }
                                    else//知识点存在
                                    {
                                        //建立关系
                                        t_subject_knowledge_ref s = new t_subject_knowledge_ref();
                                        s.knowledge_tag_id = queryTag.FirstOrDefault().id;
                                        s.subject_id = subjectid;
                                        db.t_subject_knowledge_ref.Add(s);
                                    }

                                }
                            }
                        }
                        db.SaveChanges();
                    }

                }
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 3,
                    logDesc = "修改了实践考试：" + taskExamination.ExamName,
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 中止考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object Quit_Examination(pf_examinationContext db,RabbitMQClient rabbit, long id, TokenModel token)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            where e.delete_flag == 0 && e.id == id
                            select e;
                var q = query.FirstOrDefault();
                q.exam_status = "3";//中止
                q.end_time = DateTime.Now;//更新结束时间，提前结束

                if (q.exam_div == "1")//理论需要判断，这场考试是否有学生作答了
                {
                    var queryRecord = from r in db.t_examination_record
                                      where r.examination_id == id && r.delete_flag == 0 && (r.record_status == "2" || r.record_status == "3" || r.record_status == "4")
                                      select r;
                    if (queryRecord.Count() > 0)//有学生作答了
                        q.correct_status = "2";//阅卷开始
                    else
                        q.correct_status = "3";//阅卷结束
                }
                else
                    q.correct_status = "2";//阅卷开始
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 3,
                    logDesc = "中止了考试：" + q.exam_name,
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 删除考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object Delete_Examination(pf_examinationContext db,RabbitMQClient rabbit, long id, TokenModel token)
        {
            try
            {
                var query = from e in db.t_examination_manage
                            where e.id == id
                            select e;
                var q = query.FirstOrDefault();
                q.delete_flag = 1;
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel
                {
                    opNo = token.userNumber,
                    opName = token.userName,
                    opType = 4,
                    logDesc = "删除了考试：" + q.exam_name,
                    logSuccessd = 1,
                    moduleName = "考试管理"
                };
                rabbit.LogMsg(syslog);
                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        #region 审核

        public object GetNotApprovalTestPapers(pf_examinationContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_test_papers> query = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            && t.approval_status == "2"
                            orderby PubMethod.GetPropertyValue(t, FieldName) ascending
                            select t;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                              && t.approval_status == "2"
                            orderby PubMethod.GetPropertyValue(t, FieldName) descending
                            select t;
                }
                else
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                              && t.approval_status == "2"
                            select t;
                }

                List<TestPaper> list = new List<TestPaper>();
                foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    list.Add(new TestPaper
                    {
                        ID = item.id,
                        PaperTitle = item.paper_title,
                        PaperConfidential = item.paper_confidential,
                        ExamScore = (int)item.exam_score,
                        CreateTime = item.t_create
                    });
                }
                return new { code = 200, result = new { list, count = query.Count() }, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetApprovaledTestPapers(pf_examinationContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_test_papers> query = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                                  && (t.approval_status == "3" || t.approval_status == "4")
                            orderby PubMethod.GetPropertyValue(t, FieldName) ascending
                            select t;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                                   && (t.approval_status == "3" || t.approval_status == "4")
                            orderby PubMethod.GetPropertyValue(t, FieldName) descending
                            select t;
                }
                else
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                                   && (t.approval_status == "3" || t.approval_status == "4")
                            select t;
                }

                List<TestPaper> list = new List<TestPaper>();
                foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    list.Add(new TestPaper
                    {
                        ID = item.id,
                        PaperTitle = item.paper_title,
                        QuestionCount = item.question_count,
                        PaperConfidential = item.paper_confidential,
                        ExamScore = (float)item.exam_score,
                        ApprovalUserNumber = item.approval_user_number,
                        ApprovalUserName = item.approval_user_name,
                        ApprovalDateTime = item.approval_date,
                        ApprovalStatus = item.approval_status,
                        CreateTime = item.t_create
                    });
                }
                return new { code = 200, result = new { list, count = query.Count() }, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public async Task<object> ApprovalTestPapers(pf_examinationContext db, List<PaperQuestionApproval> paperQuestionApprovals)
        {
            try
            {
                if (paperQuestionApprovals != null && paperQuestionApprovals.Count > 0)
                {
                    for (int i = 0; i < paperQuestionApprovals.Count; i++)
                    {
                        var query = (from t in db.t_test_papers
                                     where t.delete_flag == 0 && t.id == paperQuestionApprovals[i].ID
                                     select t).FirstOrDefault();
                        if (query != null)
                        {
                            query.approval_remarks = paperQuestionApprovals[i].ApprovalRemark;
                            query.approval_status = paperQuestionApprovals[i].ApprovalResult;
                            query.approval_user_name = paperQuestionApprovals[i].ApprovalUserName;
                            query.approval_user_number = paperQuestionApprovals[i].ApprovalUserNumber;

                            var queryExam = (from e in db.t_examination_manage
                                             where e.delete_flag == 0 && e.id == query.examination_id
                                             select e).FirstOrDefault();
                            if (queryExam != null)
                                queryExam.approval_status = paperQuestionApprovals[i].ApprovalResult;
                        }
                    }
                    await db.SaveChangesAsync();
                }
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object AddTheoryExamFromTestPapers(pf_examinationContext db, Examinations examination, TokenModel token)
        {
            try
            {
                t_examination_manage q = new t_examination_manage();
                q.exam_name = examination.ExamName;
                q.exam_div = "1";
                q.exam_status = "1";//未开始
                q.exam_duration = examination.ExamDuration;
                q.paper_confidential = examination.PaperConfidential;
                q.exam_explain = examination.ExamExplain;
                q.pass_scores = examination.PassScore;
                q.approval_status = "3";
                q.correct_status = "1";//批改状态
                q.create_num = token.userNumber;
                q.create_name = token.userName;
                db.t_examination_manage.Add(q);
                db.SaveChanges();
                long MaxID = q.id;
                //存在评分人员
                if (examination.teacherInfos != null && examination.teacherInfos.Count > 0)
                {

                    for (int i = 0; i < examination.teacherInfos.Count; i++)
                    {
                        t_grade_teacher t = new t_grade_teacher();
                        t.examination_id = MaxID;
                        t.grade_teacher_num = examination.teacherInfos[i].TeacherNum;
                        t.grade_teacher_name = examination.teacherInfos[i].TeacherName;
                        t.delete_flag = 0;
                        t.t_create = DateTime.Now;
                        t.t_modified = DateTime.Now;
                        db.t_grade_teacher.Add(t);
                    }
                }
                db.SaveChanges();

                PaperInfomation paperInfomation = new PaperInfomation();
                paperInfomation.PaperInfo = GetTestPaperInfomation(db, examination.PaperID);
                examination.paperInfomation = paperInfomation;

                //初始化考试统计管理
                StatisticExam(db, MaxID, examination.paperInfomation.PaperInfo.ExamScore);

                //导入试卷
                TestPaperInfo.TestPaperImportToDB(db, examination.paperInfomation, examination.ApprovalFlag, MaxID, token);

                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        private Paper GetTestPaperInfomation(pf_examinationContext db, long id)
        {
            try
            {
                //选项集合
                List<OptionInfo> optionsList = new List<OptionInfo>();
                //题干集合
                List<Question> QuestionList = new List<Question>();
                //试卷
                Paper testPaper = new Paper();

                //查找试卷下的所有选项
                var queryOption = from t in db.t_test_papers
                                  join q in db.t_questions on t.id equals q.test_paper_id
                                  join o in db.t_question_option on q.id equals o.question_id into qo
                                  from _qo in qo.DefaultIfEmpty()
                                  where t.id == id && _qo.delete_flag == 0
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
                    optionsList.Add(new OptionInfo
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
                                    where t.id == id && _tq.delete_flag == 0
                                    orderby _tq.id ascending
                                    select new
                                    {
                                        id = _tq == null ? 0 : _tq.id,
                                        testpaperid = _tq == null ? 0 : _tq.test_paper_id,
                                        questiontype = _tq == null ? "null" : _tq.question_type,
                                        questiontitle = _tq == null ? "null" : _tq.question_title,
                                        //questionsort = _tq == null ? 0 : _tq.question_sort,
                                        answeranalyze = _tq == null ? "null" : _tq.answer_analyze,
                                        standanswer = _tq == null ? "null" : _tq.question_answer,
                                        questionscore = _tq == null ? 0 : _tq.question_score,
                                        questionconfidential = _tq == null ? "null" : _tq.question_confidential
                                    };
                int i = 0;
                foreach (var item in queryQuestion)
                {
                    List<KnowledgeTag> knowledgeTags = new List<KnowledgeTag>();
                    //查找知识点
                    var queryTag = from r in db.t_question_knowledge_ref
                                   join g in db.t_knowledge_tag
                                   on r.knowledge_tag_id equals g.id
                                   where r.question_id == item.id && r.delete_flag == 0
                                   select g;
                    foreach (var tag in queryTag)
                    {
                        knowledgeTags.Add(new KnowledgeTag
                        {
                            ID = tag.src_id,
                            Tag = tag.tag
                        });
                    }

                    //筛选题干下的选项
                    var t = optionsList.FindAll(x => x.QuestionID == item.id);
                    //添加到题干中
                    QuestionList.Add(new Question()
                    {
                        ID = item.id,
                        TestPaperID = item.testpaperid,
                        QuestionTitle = item.questiontitle,
                        QuestionType = item.questiontype,
                        OptionInfoList = t,
                        QuestionAnswer = item.standanswer,
                        AnswerAnalyze = item.answeranalyze,
                        QuestionConfidential = item.questionconfidential,
                        Score = item.questionscore,
                        KnowledgeTags = knowledgeTags
                    });
                }

                //查找相应的试卷
                var queryTestPaper = from p in db.t_test_papers
                                     where p.delete_flag == 0 && p.id == id
                                     select new { p.id, p.paper_title, p.exam_score, p.paper_confidential, p.question_count };
                var queryTestPaper_F = queryTestPaper.FirstOrDefault();
                if (queryTestPaper_F != null)
                {
                    testPaper.ID = queryTestPaper_F.id;
                    testPaper.PaperTitle = queryTestPaper_F.paper_title;
                    testPaper.QuestionList = QuestionList;
                    testPaper.PaperScore = queryTestPaper_F.exam_score;
                    testPaper.PaperConfidential = queryTestPaper_F.paper_confidential;
                    testPaper.QuestionCount = queryTestPaper_F.question_count;
                }
                return testPaper;
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }
        public object GetTestPaperInfo(pf_examinationContext db, long id)
        {
            try
            {
                //选项集合
                List<OptionInfo> optionsList = new List<OptionInfo>();
                //题干集合
                List<Question> QuestionList = new List<Question>();
                //试卷
                Paper testPaper = new Paper();

                //查找试卷下的所有选项
                var queryOption = from t in db.t_test_papers
                                  join q in db.t_questions on t.id equals q.test_paper_id
                                  join o in db.t_question_option on q.id equals o.question_id into qo
                                  from _qo in qo.DefaultIfEmpty()
                                  where t.id == id && _qo.delete_flag == 0
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
                    optionsList.Add(new OptionInfo
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
                                    where t.id == id && _tq.delete_flag == 0
                                    orderby _tq.id ascending
                                    select new
                                    {
                                        id = _tq == null ? 0 : _tq.id,
                                        testpaperid = _tq == null ? 0 : _tq.test_paper_id,
                                        questiontype = _tq == null ? "null" : _tq.question_type,
                                        questiontitle = _tq == null ? "null" : _tq.question_title,
                                        //questionsort = _tq == null ? 0 : _tq.question_sort,
                                        answeranalyze = _tq == null ? "null" : _tq.answer_analyze,
                                        standanswer = _tq == null ? "null" : _tq.question_answer,
                                        questionscore = _tq == null ? 0 : _tq.question_score,
                                        questionconfidential = _tq == null ? "null" : _tq.question_confidential
                                    };
                foreach (var item in queryQuestion)
                {
                    List<KnowledgeTag> knowledgeTags = new List<KnowledgeTag>();
                    //查找知识点
                    var queryTag = from r in db.t_question_knowledge_ref
                                   join g in db.t_knowledge_tag
                                   on r.knowledge_tag_id equals g.id
                                   where r.question_id == item.id && r.delete_flag == 0
                                   select g;
                    foreach (var tag in queryTag)
                    {
                        knowledgeTags.Add(new KnowledgeTag
                        {
                            ID = tag.src_id,
                            Tag = tag.tag
                        });
                    }

                    //筛选题干下的选项
                    var t = optionsList.FindAll(x => x.QuestionID == item.id);
                    //添加到题干中
                    QuestionList.Add(new Question()
                    {
                        ID = item.id,
                        TestPaperID = item.testpaperid,
                        QuestionTitle = item.questiontitle,
                        QuestionType = item.questiontype,
                        OptionInfoList = t,
                        QuestionAnswer = item.standanswer,
                        AnswerAnalyze = item.answeranalyze,
                        QuestionConfidential = item.questionconfidential,
                        Score = item.questionscore,
                        KnowledgeTags = knowledgeTags
                    });
                }

                //查找相应的试卷
                var queryTestPaper = from p in db.t_test_papers
                                     where p.delete_flag == 0 && p.id == id
                                     select p;
                var queryTestPaper_F = queryTestPaper.FirstOrDefault();
                if (queryTestPaper_F != null)
                {
                    testPaper.ID = queryTestPaper_F.id;
                    testPaper.PaperTitle = queryTestPaper_F.paper_title;
                    testPaper.QuestionList = QuestionList;
                    testPaper.ExamScore = queryTestPaper_F.exam_score;
                    testPaper.PaperConfidential = queryTestPaper_F.paper_confidential;
                    testPaper.QuestionCount = queryTestPaper_F.question_count;
                    testPaper.ApprovalStatus = queryTestPaper_F.approval_status;
                    testPaper.ApprovalRemark = queryTestPaper_F.approval_remarks;

                }
                return new { code = 200, result = testPaper, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }

        #endregion

    }
    public class PaperQuestionApproval
    {
        public long ID { get; set; }
        public string ApprovalResult { get; set; }
        public string ApprovalRemark { get; set; }
        public string ApprovalUserName { get; set; }
        public string ApprovalUserNumber { get; set; }
    }
    public class Examinations
    {
        public long ID { get; set; }
        public long PaperID { get; set; }
        public string ApprovalFlag { get; set; }
        public long? ContentID { get; set; }
        public string CreateName { get; set; }
        public string ExamName { get; set; }
        public string StartTime { get; set; }
        public string ExamDiv { get; set; }
        public string EndTime { get; set; }
        public string PaperConfidential { get; set; }
        public int? ExamDuration { get; set; }
        public string ExamExplain { get; set; }
        public string ExamStatus { get; set; }
        public string StuExamStatus { get; set; }
        public string CorrectStatus { get; set; }
        public int? PassScore { get; set; }
        public float? Score { get; set; }
        /// <summary>
        /// 参加考试的人数
        /// </summary>
        public int SumExamCount { get; set; }
        /// <summary>
        /// 完成考试的人数
        /// </summary>
        public int FinishExamCount { get; set; }
        public PaperInfo PaperInfo { get; set; }

        public PaperInfomation paperInfomation { get; set; }
        public List<TeacherInfo> teacherInfos { get; set; }

    }

    public class TaskExamination
    {
        public long ID { get; set; }
        public string ExamName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? ExamDuration { get; set; }
        public string ExamExplain { get; set; }
        public string ExamStatus { get; set; }
        public TrainTask trainTask { get; set; }
        public List<TeacherInfo> teacherInfos { get; set; }

    }

    public class TeacherInfo
    {
        public string TeacherNum { get; set; }
        public string TeacherName { get; set; }
    }

    public class PaperInfo
    {
        public string PaperName { get; set; }
        public int? ExamScore { get; set; }
        public int QuestionCount { get; set; }
    }

    public class StuExamInfo
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ExamStatus { get; set; }
    }

    public class TrainTask
    {
        public long? ID { get; set; }
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string KindLevel { get; set; }
        public string RankLevel { get; set; }
        public string PlaneType { get; set; }
        public List<TrainSubject> trainSubjects { get; set; }
    }

    public class TrainSubject
    {
        public long ID { get; set; }
        public long TaskID { get; set; }
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string TrainDesc { get; set; }
        public string TrainKind { get; set; }
        public string PlaneType { get; set; }
        public string ExpectResult { get; set; }

        /// <summary>
        /// 状态完成与未完成
        /// </summary>
        public string SubjectStatus { get; set; }
        public string TaskExamResult { get; set; }
        public List<Tag> Tags { get; set; }
    }

    public class Tag
    {
        public long? ID { get; set; }
        public string TagName { get; set; }
    }

    public class ExaminationID
    {
        public long ID { get; set; }
        /// <summary>
        /// 培训计划内容ID
        /// </summary>
        public long ContentID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
    public class ExamKnowledge
    {
        public long ExamID { get; set; }
        public long KnowledgeID { get; set; }
        public string KnowledgeName { get; set; }
    }
}
