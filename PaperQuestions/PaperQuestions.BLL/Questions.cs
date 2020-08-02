using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace PaperQuestions.BLL
{
    public class Questions
    {
        #region 题库
        public object GetQuestions(pf_exam_paper_questionsContext db, string questionType, string complexity, string publishFlag, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_questions> queryQuestions = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 //&& _qr.delete_flag == 0
                                     && (string.IsNullOrEmpty(questionType) ? true : q.question_type == questionType)
                                     && (string.IsNullOrEmpty(complexity) ? true : q.complexity == complexity)
                                     && (string.IsNullOrEmpty(publishFlag) ? true : q.publish_flag == publishFlag)
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby PubMethod.GetPropertyValue(q, FieldName) ascending
                                     select q;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 //&& _qr.delete_flag == 0
                                     && (string.IsNullOrEmpty(questionType) ? true : q.question_type == questionType)
                                     && (string.IsNullOrEmpty(complexity) ? true : q.complexity == complexity)
                                     && (string.IsNullOrEmpty(publishFlag) ? true : q.publish_flag == publishFlag)
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby PubMethod.GetPropertyValue(q, FieldName) descending
                                     select q;
                }
                else
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0// && _qr.delete_flag == 0
                                     && (string.IsNullOrEmpty(questionType) ? true : q.question_type == questionType)
                                     && (string.IsNullOrEmpty(complexity) ? true : q.complexity == complexity)
                                     && (string.IsNullOrEmpty(publishFlag) ? true : q.publish_flag == publishFlag)
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby q.t_create descending
                                     select q;
                }
                var count = queryQuestions.Distinct().Count();
                List<Question> list = new List<Question>();
                int i = pageSize * (pageIndex - 1);
                foreach (var item in queryQuestions.Distinct().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList())
                {
                    var queryTag = from r in db.t_question_knowledge_ref
                                   join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                   where r.delete_flag == 0 && r.question_id == item.id
                                   select new { t.src_id, t.tag };
                    List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                    foreach (var item1 in queryTag)
                    {
                        listTag.Add(new KnowledgeTag
                        {
                            ID = (long)item1.src_id,
                            Tag = item1.tag
                        });
                    }
                    list.Add(new Question()
                    {
                        ID = item.id,
                        Number = ++i,
                        QuestionTitle = item.question_title,
                        QuestionType = item.question_type,
                        Complexity = item.complexity,
                        QuestionConfidential = item.question_confidential,
                        PublishFlag = item.publish_flag,
                        CreateTime = item.t_create.ToString(),
                        CreateName = item.create_name,
                        KnowledgeTags = listTag
                    });
                }
                return new { code = 200, result = new { list, count }, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetQuestionByID(pf_exam_paper_questionsContext db, long Id)
        {
            try
            {
                var queryQuestion = from q in db.t_questions
                                    where q.delete_flag == 0 && q.id == Id
                                    select q;
                var queryQuestionF = queryQuestion.FirstOrDefault();
                Question questionInfo = new Question();
                questionInfo.ID = Id;
                questionInfo.QuestionTitle = queryQuestionF.question_title;
                questionInfo.QuestionType = queryQuestionF.question_type;
                questionInfo.Complexity = queryQuestionF.complexity;
                questionInfo.QuestionConfidential = queryQuestionF.question_confidential;
                questionInfo.PublishFlag = queryQuestionF.publish_flag;
                questionInfo.AnswerAnalyze = queryQuestionF.answer_analyze;
                questionInfo.QuestionAnswer = queryQuestionF.question_answer;
                questionInfo.ApprovalStatus = queryQuestionF.approval_status;
                questionInfo.ApprovalRemark = queryQuestionF.approval_remarks;
                questionInfo.CreateName = queryQuestionF.create_name;
                questionInfo.CreateTime = queryQuestionF.t_create.ToString();
                questionInfo.ApprovalUserName = queryQuestionF.approval_user_name;
                questionInfo.ApprovalDateTime = queryQuestionF.approval_date;

                var queryQuestionOption = from o in db.t_question_option
                                          where o.delete_flag == 0 && o.question_id == Id
                                          select o;
                List<OptionInfo> questionOptions = new List<OptionInfo>();
                foreach (var item in queryQuestionOption)
                {
                    questionOptions.Add(new OptionInfo
                    {
                        OptionNum = item.option_number,
                        OptionContent = item.option_content
                    });
                }
                questionInfo.OptionInfoList = questionOptions;
                var queryTag = from r in db.t_question_knowledge_ref
                               join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                               where r.delete_flag == 0 && r.question_id == Id
                               select new { t.src_id, t.tag };
                List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                foreach (var item1 in queryTag)
                {
                    listTag.Add(new KnowledgeTag
                    {
                        ID = (long)item1.src_id,
                        Tag = item1.tag
                    });
                }
                questionInfo.KnowledgeTags = listTag;
                return new { code = 200, result = questionInfo, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object DeleteQuestions(pf_exam_paper_questionsContext db,RabbitMQClient rabbit, QusetionIDList qusetionIDList, TokenModel obj)
        {
            try
            {
                if (qusetionIDList != null && qusetionIDList.questionIDList.Count > 0)
                {
                    for (int i = 0; i < qusetionIDList.questionIDList.Count; i++)
                    {
                        var query = from q in db.t_questions
                                    where q.delete_flag == 0 && q.id == qusetionIDList.questionIDList[i]
                                    select q;
                        foreach (var item in query)
                        {
                            item.delete_flag = 1;
                        }
                    }
                    db.SaveChanges();
                }
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "删除了试题";
                log.logSuccessd = 1;
                log.moduleName = "题库管理";
                rabbit.LogMsg(log);
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object PublishBatchQuestions(pf_exam_paper_questionsContext db,RabbitMQClient rabbit, QusetionIDList qusetionIDList, TokenModel obj)
        {
            try
            {
                string strId = "";
                if (qusetionIDList != null && qusetionIDList.questionIDList.Count > 0)
                {
                    for (int i = 0; i < qusetionIDList.questionIDList.Count; i++)
                    {
                        var query = from q in db.t_questions
                                    where q.delete_flag == 0 && q.id == qusetionIDList.questionIDList[i]
                                    select q;
                        foreach (var item in query)
                        {
                            item.publish_flag = "1";
                            item.approval_status = "2";
                            strId += item.id + ",";
                            //审核消息推送
                            MsgToDo model = new MsgToDo();
                            model.todoType = 3;
                            model.commonId = item.id;
                            model.pubTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            model.msgName = "您有一个新的试题待审核";
                            model.msgBody = "题干ID：" + item.id + ",需要您的审核哦！";
                            model.finishFlag = 1;
                            rabbit.ToDoMsg(model);
                        }
                    }
                    db.SaveChanges();
                }


                //消息推送
                Msg msg = new Msg();
                msg.msgTitle = "审核试题";
                msg.msgBody = "试题ID值：" + strId.TrimEnd(',');
                rabbit.Msg(msg);

                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "发布了试题,ID值："+strId.TrimEnd(',');
                log.logSuccessd = 1;
                log.moduleName = "题库管理";
                rabbit.LogMsg(log);
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object CreateQuestion(pf_exam_paper_questionsContext db,RabbitMQClient rabbit, Question questionInfo, TokenModel obj)
        {
            try
            {
                string strAnswer = "";
                t_questions question = new t_questions();
                question.question_title = questionInfo.QuestionTitle;
                question.question_type = questionInfo.QuestionType;
                question.complexity = questionInfo.Complexity;
                question.question_confidential = questionInfo.QuestionConfidential;
                strAnswer = questionInfo.QuestionAnswer;
                if (questionInfo.QuestionType == "2")//多选
                {
                    List<char> list = strAnswer.ToList<char>();
                    list.Sort();
                    strAnswer = string.Join("", list.ToArray());
                }
                question.question_answer = strAnswer;
                question.answer_analyze = questionInfo.AnswerAnalyze;
                question.publish_flag = "0";
                question.create_name = obj.userName;
                question.create_number = obj.userNumber;
                db.t_questions.Add(question);
                db.SaveChanges();
                long maxId = question.id;
                //选项
                if (questionInfo.OptionInfoList != null && questionInfo.OptionInfoList.Count > 0)
                {
                    for (int i = 0; i < questionInfo.OptionInfoList.Count; i++)
                    {
                        t_question_option option = new t_question_option();
                        option.question_id = maxId;
                        option.option_number = questionInfo.OptionInfoList[i].OptionNum;
                        option.option_content = questionInfo.OptionInfoList[i].OptionContent;
                        option.create_number = obj.userNumber;
                        db.t_question_option.Add(option);
                    }
                    db.SaveChanges();
                }
                //知识点
                if (questionInfo.KnowledgeTags != null && questionInfo.KnowledgeTags.Count > 0)
                {
                    for (int i = 0; i < questionInfo.KnowledgeTags.Count; i++)
                    {
                        var queryTag = from t in db.t_knowledge_tag
                                       where t.delete_flag == 0 && t.src_id == questionInfo.KnowledgeTags[i].ID
                                       select t;
                        var queryTagF = queryTag.FirstOrDefault();
                        if (queryTagF != null)//存在
                        {
                            queryTagF.tag = questionInfo.KnowledgeTags[i].Tag;
                            //建立关系
                            t_question_knowledge_ref tagref = new t_question_knowledge_ref();
                            tagref.question_id = maxId;
                            tagref.knowledge_tag_id = queryTagF.id;
                            db.t_question_knowledge_ref.Add(tagref);
                        }
                        else//不存在
                        {
                            t_knowledge_tag tag = new t_knowledge_tag();
                            tag.src_id = questionInfo.KnowledgeTags[i].ID;
                            tag.tag = questionInfo.KnowledgeTags[i].Tag;
                            db.t_knowledge_tag.Add(tag);
                            db.SaveChanges();
                            long tagId = tag.id;

                            //建立关系
                            t_question_knowledge_ref tagref = new t_question_knowledge_ref();
                            tagref.question_id = maxId;
                            tagref.knowledge_tag_id = tagId;
                            db.t_question_knowledge_ref.Add(tagref);
                        }
                    }
                    db.SaveChanges();
                }
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 2;
                log.logDesc = "创建了试题：" + question.question_title;
                log.logSuccessd = 1;
                log.moduleName = "题库管理";
                rabbit.LogMsg(log);
                return new { code = 200, result = maxId, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object UpdateQuestion(pf_exam_paper_questionsContext db,RabbitMQClient rabbit, Question questionInfo, TokenModel obj)
        {
            try
            {
                var queryQuestion = from q in db.t_questions
                                    where q.delete_flag == 0 && q.id == questionInfo.ID
                                    select q;
                var queryQuestionF = queryQuestion.FirstOrDefault();
                queryQuestionF.question_title = questionInfo.QuestionTitle;
                queryQuestionF.question_type = questionInfo.QuestionType;
                queryQuestionF.complexity = questionInfo.Complexity;
                queryQuestionF.question_confidential = questionInfo.QuestionConfidential;
                queryQuestionF.publish_flag = questionInfo.PublishFlag;
                queryQuestionF.answer_analyze = questionInfo.AnswerAnalyze;
                queryQuestionF.question_answer = questionInfo.QuestionAnswer;

                //删除选项
                var queryOption = from p in db.t_question_option
                                  where p.delete_flag == 0 && p.question_id == questionInfo.ID
                                  select p;
                foreach (var item in queryOption)
                {
                    item.delete_flag = 1;
                }

                //选项
                if (questionInfo.OptionInfoList != null && questionInfo.OptionInfoList.Count > 0)
                {
                    for (int i = 0; i < questionInfo.OptionInfoList.Count; i++)
                    {
                        t_question_option option = new t_question_option();
                        option.question_id = questionInfo.ID;
                        option.option_number = questionInfo.OptionInfoList[i].OptionNum;
                        option.option_content = questionInfo.OptionInfoList[i].OptionContent;
                        db.t_question_option.Add(option);
                    }
                }

                //删除知识点关系表
                var queryTagRef = from r in db.t_question_knowledge_ref
                                  where r.delete_flag == 0 && r.question_id == questionInfo.ID
                                  select r;
                foreach (var item in queryTagRef)
                {
                    item.delete_flag = 1;
                }

                //知识点
                if (questionInfo.KnowledgeTags != null && questionInfo.KnowledgeTags.Count > 0)
                {
                    for (int i = 0; i < questionInfo.KnowledgeTags.Count; i++)
                    {
                        var queryTag = from t in db.t_knowledge_tag
                                       where t.delete_flag == 0 && t.src_id == questionInfo.KnowledgeTags[i].ID
                                       select t;
                        var queryTagF = queryTag.FirstOrDefault();
                        if (queryTagF != null)//存在
                        {
                            queryTagF.tag = questionInfo.KnowledgeTags[i].Tag;
                            //建立关系
                            t_question_knowledge_ref tagref = new t_question_knowledge_ref();
                            tagref.question_id = questionInfo.ID;
                            tagref.knowledge_tag_id = queryTagF.id;
                            db.t_question_knowledge_ref.Add(tagref);
                        }
                        else//不存在
                        {
                            t_knowledge_tag tag = new t_knowledge_tag();
                            tag.src_id = questionInfo.KnowledgeTags[i].ID;
                            tag.tag = questionInfo.KnowledgeTags[i].Tag;
                            db.t_knowledge_tag.Add(tag);
                            db.SaveChanges();

                            long tagId = tag.id;

                            //建立关系
                            t_question_knowledge_ref tagref = new t_question_knowledge_ref();
                            tagref.question_id = questionInfo.ID;
                            tagref.knowledge_tag_id = tagId;
                            db.t_question_knowledge_ref.Add(tagref);
                        }
                    }
                }
                db.SaveChanges();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "更新了试题：" + questionInfo.QuestionTitle;
                log.logSuccessd = 1;
                log.moduleName = "题库管理";
                rabbit.LogMsg(log);
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object ImportQuestions(pf_exam_paper_questionsContext db, Questionmation questionmation, TokenModel token)
        {
            try
            {
                if (questionmation.QuestionList != null && questionmation.QuestionList.Count > 0)
                {
                    for (int i = 0; i < questionmation.QuestionList.Count; i++)
                    {
                        //保存题干数据
                        t_questions question = new t_questions();
                        question.question_type = questionmation.QuestionList[i].QuestionType;
                        question.complexity = questionmation.QuestionList[i].Complexity;
                        question.question_title = questionmation.QuestionList[i].QuestionTitle;
                        question.question_answer = questionmation.QuestionList[i].QuestionAnswer;
                        question.answer_analyze = questionmation.QuestionList[i].AnswerAnalyze;
                        question.question_confidential = questionmation.QuestionList[i].QuestionConfidential;
                        question.create_name = token.userName;
                        question.create_number = token.userNumber;
                        db.t_questions.Add(question);
                        db.SaveChanges();
                        //读出题干最大值
                        long questionid = question.id;

                        if (questionmation.QuestionList[i].KnowledgeTags != null && questionmation.QuestionList[i].KnowledgeTags.Count > 0)
                        {
                            for (int j = 0; j < questionmation.QuestionList[i].KnowledgeTags.Count; j++)
                            {
                                var queryTag = from t in db.t_knowledge_tag
                                               where t.delete_flag == 0
                                                     && t.src_id == questionmation.QuestionList[i].KnowledgeTags[j].ID
                                               select t;
                                if (queryTag.FirstOrDefault() != null)//存在副本知识点
                                {
                                    queryTag.FirstOrDefault().tag = questionmation.QuestionList[i].KnowledgeTags[j].Tag;
                                    //建立关系
                                    t_question_knowledge_ref obj = new t_question_knowledge_ref();
                                    obj.question_id = questionid;
                                    obj.knowledge_tag_id = queryTag.FirstOrDefault().id;
                                    db.t_question_knowledge_ref.Add(obj);
                                }
                                else//不存在
                                {
                                    //新建知识点
                                    t_knowledge_tag tag = new t_knowledge_tag();
                                    tag.src_id = questionmation.QuestionList[i].KnowledgeTags[j].ID;
                                    tag.tag = questionmation.QuestionList[i].KnowledgeTags[j].Tag;
                                    db.t_knowledge_tag.Add(tag);
                                    db.SaveChanges();
                                    //读出知识点最大值
                                    long tagid = tag.id;

                                    //建立关系
                                    t_question_knowledge_ref obj = new t_question_knowledge_ref();
                                    obj.question_id = questionid;
                                    obj.knowledge_tag_id = tagid;
                                    db.t_question_knowledge_ref.Add(obj);
                                }
                            }
                        }

                        if (questionmation.QuestionList[i].OptionInfoList != null && questionmation.QuestionList[i].OptionInfoList.Count > 0)
                        {
                            for (int j = 0; j < questionmation.QuestionList[i].OptionInfoList.Count; j++)
                            {
                                //保存选项
                                t_question_option option = new t_question_option();
                                option.question_id = questionid;
                                option.option_number = questionmation.QuestionList[i].OptionInfoList[j].OptionNum;
                                option.option_content = questionmation.QuestionList[i].OptionInfoList[j].OptionContent;
                                //option.right_flag = questionmation.QuestionList[i].OptionInfoList[j].RightFlag;
                                db.t_question_option.Add(option);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                db.SaveChanges();
                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        #endregion

        #region 审核

        /// <summary>
        /// 未审核
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="IsAsc"></param>
        /// <param name="FieldName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetNotApprovalQuestions(pf_exam_paper_questionsContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_questions> queryQuestions = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && q.approval_status == "2"
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby PubMethod.GetPropertyValue(q, FieldName) ascending
                                     select q;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && q.approval_status == "2"
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby PubMethod.GetPropertyValue(q, FieldName) descending
                                     select q;
                }
                else
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && q.approval_status == "2"
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby q.t_create descending
                                     select q;
                }
                var count = queryQuestions.Distinct().Count();
                List<Question> list = new List<Question>();
                int i = pageSize * (pageIndex - 1);
                foreach (var item in queryQuestions.Distinct().Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    var queryTag = from r in db.t_question_knowledge_ref
                                   join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                   where r.delete_flag == 0 && r.question_id == item.id
                                   select new { t.src_id, t.tag };
                    List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                    foreach (var item1 in queryTag)
                    {
                        listTag.Add(new KnowledgeTag
                        {
                            ID = (long)item1.src_id,
                            Tag = item1.tag
                        });
                    }
                    list.Add(new Question()
                    {
                        ID = item.id,
                        Number = ++i,
                        QuestionTitle = item.question_title,
                        QuestionType = item.question_type,
                        Complexity = item.complexity,
                        ApprovalStatus = item.approval_status,
                        QuestionConfidential = item.question_confidential,
                        PublishFlag = item.publish_flag,
                        CreateTime = item.t_create.ToString(),
                        CreateName = item.create_name,
                        KnowledgeTags = listTag
                    });
                }
                return new { code = 200, result = new { list, count }, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        /// <summary>
        /// 已审核
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="IsAsc"></param>
        /// <param name="FieldName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetApprovaledQuestions(pf_exam_paper_questionsContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_questions> queryQuestions = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && (q.approval_status == "4" || q.approval_status == "3")
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby PubMethod.GetPropertyValue(q, FieldName) ascending
                                     select q;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && (q.approval_status == "4" || q.approval_status == "3")
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby PubMethod.GetPropertyValue(q, FieldName) descending
                                     select q;
                }
                else
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && (q.approval_status == "4" || q.approval_status == "3")
                                     && ((string.IsNullOrEmpty(keyWord) ? true : q.question_title.Contains(keyWord))
                                      || (string.IsNullOrEmpty(keyWord) ? true : _qt.tag.Contains(keyWord)))
                                     orderby q.t_create descending
                                     select q;
                }
                var count = queryQuestions.Distinct().Count();
                List<Question> list = new List<Question>();
                int i = pageSize * (pageIndex - 1);
                foreach (var item in queryQuestions.Distinct().Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    var queryTag = from r in db.t_question_knowledge_ref
                                   join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                   where r.delete_flag == 0 && r.question_id == item.id
                                   select new { t.src_id, t.tag };
                    List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                    foreach (var item1 in queryTag)
                    {
                        listTag.Add(new KnowledgeTag
                        {
                            ID = (long)item1.src_id,
                            Tag = item1.tag
                        });
                    }
                    list.Add(new Question()
                    {
                        ID = item.id,
                        Number = ++i,
                        QuestionTitle = item.question_title,
                        QuestionType = item.question_type,
                        Complexity = item.complexity,
                        ApprovalStatus = item.approval_status,
                        QuestionConfidential = item.question_confidential,
                        ApprovalDateTime = item.approval_date,
                        ApprovalUserName = item.approval_user_name,
                        PublishFlag = item.publish_flag,
                        CreateTime = item.t_create.ToString(),
                        CreateName = item.create_name,
                        KnowledgeTags = listTag
                    });
                }
                return new { code = 200, result = new { list, count }, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="questionApprovals"></param>
        /// <returns></returns>
        public async Task<object> ApprovalQuestions(pf_exam_paper_questionsContext db,RabbitMQClient rabbit, List<PaperQuestionApproval> questionApprovals, TokenModel obj)
        {
            try
            {
                if (questionApprovals != null && questionApprovals.Count > 0)
                {
                    for (int i = 0; i < questionApprovals.Count; i++)
                    {
                        var query = (from q in db.t_questions
                                     where q.delete_flag == 0 && q.id == questionApprovals[i].ID
                                     select q).FirstOrDefault();
                        if (query != null)
                        {
                            query.approval_status = questionApprovals[i].ApprovalResult;
                            query.approval_user_number = questionApprovals[i].ApprovalUserNumber;
                            query.approval_user_name = questionApprovals[i].ApprovalUserName;
                            query.approval_remarks = questionApprovals[i].ApprovalRemark;
                            query.approval_date = DateTime.Now;

                            //审核消息推送
                            MsgToDo model = new MsgToDo();
                            model.todoType = 3;
                            model.commonId = query.id;
                            model.pubTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            model.msgName = "审核人员已审核了您的试题";

                            string strResult = string.Empty;
                            if (query.approval_status == "3")
                                strResult = "通过";
                            else
                                strResult = "拒绝";

                            model.msgBody = "审核结果为：" + strResult + "！";
                            model.finishFlag = 2;
                            rabbit.ToDoMsg(model);
                        }
                    }
                    await db.SaveChangesAsync();
                }
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "审核了试题";
                log.logSuccessd = 1;
                log.moduleName = "题库管理";
                rabbit.LogMsg(log);
                return new { code = 200, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        #endregion
    }

    public class QusetionIDList
    {
        public List<long> questionIDList { get; set; }
    }

    public class PaperQuestionApproval
    {
        public long ID { get; set; }
        public string ApprovalResult { get; set; }
        public string ApprovalRemark { get; set; }
        public string ApprovalUserName { get; set; }
        public string ApprovalUserNumber { get; set; }
    }
}
