using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace PaperQuestions.BLL
{
    public partial class ExaminationPapers
    {
        #region 知识点组卷

        public object GetKnowledgeQuestions(pf_exam_paper_questionsContext db, QuestionsQueryParameter parameter)
        {
            try
            {
                IQueryable<t_questions> queryQuestions = null;
                if (parameter.IsAsc && !string.IsNullOrEmpty(parameter.FieldName))
                {
                    queryQuestions = from q in db.t_questions
                                     join s in db.t_question_statistic on q.id equals s.questionid into qs
                                     from _qs in qs.DefaultIfEmpty()
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && q.publish_flag == "1" && q.approval_status == "3"
                                     && (string.IsNullOrEmpty(parameter.QuestionType) ? true : q.question_type == parameter.QuestionType)
                                     && (string.IsNullOrEmpty(parameter.Complexity) ? true : q.complexity == parameter.Complexity)
                                     && ((string.IsNullOrEmpty(parameter.KeyWord) ? true : q.question_title.Contains(parameter.KeyWord))
                                     || (string.IsNullOrEmpty(parameter.KeyWord) ? true : _qt.tag.Contains(parameter.KeyWord)))
                                     && (parameter.NodeID.Count == 0 ? true : parameter.NodeID.Contains((long)_qt.src_id))
                                     orderby PubMethod.GetPropertyValue(q, parameter.FieldName) ascending
                                     select q;

                }
                else if (parameter.IsAsc == false && !string.IsNullOrEmpty(parameter.FieldName))
                {

                    queryQuestions = from q in db.t_questions
                                     join s in db.t_question_statistic on q.id equals s.questionid into qs
                                     from _qs in qs.DefaultIfEmpty()
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && q.publish_flag == "1" && q.approval_status == "3"
                                     && (string.IsNullOrEmpty(parameter.QuestionType) ? true : q.question_type == parameter.QuestionType)
                                     && (string.IsNullOrEmpty(parameter.Complexity) ? true : q.complexity == parameter.Complexity)
                                     && ((string.IsNullOrEmpty(parameter.KeyWord) ? true : q.question_title.Contains(parameter.KeyWord))
                                     || (string.IsNullOrEmpty(parameter.KeyWord) ? true : _qt.tag.Contains(parameter.KeyWord)))
                                    && (parameter.NodeID.Count == 0 ? true : parameter.NodeID.Contains((long)_qt.src_id))
                                     orderby PubMethod.GetPropertyValue(q, parameter.FieldName) descending
                                     select q;

                }
                else
                {
                    queryQuestions = from q in db.t_questions
                                     join r in db.t_question_knowledge_ref on q.id equals r.question_id into qr
                                     from _qr in qr.DefaultIfEmpty()

                                     join t in db.t_knowledge_tag on _qr.knowledge_tag_id equals t.id into qt
                                     from _qt in qt.DefaultIfEmpty()

                                     where q.delete_flag == 0 && q.publish_flag == "1" && q.approval_status == "3"
                                     && (string.IsNullOrEmpty(parameter.QuestionType) ? true : q.question_type == parameter.QuestionType)
                                     && (string.IsNullOrEmpty(parameter.Complexity) ? true : q.complexity == parameter.Complexity)
                                     && ((string.IsNullOrEmpty(parameter.KeyWord) ? true : q.question_title.Contains(parameter.KeyWord))
                                     || (string.IsNullOrEmpty(parameter.KeyWord) ? true : _qt.tag.Contains(parameter.KeyWord)))
                                         && (parameter.NodeID.Count == 0 ? true : parameter.NodeID.Contains((long)_qt.src_id))
                                     orderby q.t_create descending
                                     select q;
                }
                var count = queryQuestions.Distinct().Count();
                var listQuestion = queryQuestions.Distinct().Skip(parameter.PageSize * (parameter.PageIndex - 1)).Take(parameter.PageSize).ToList();
                List<Question> list = new List<Question>();
                foreach (var item in listQuestion)
                {
                    var queryTag = (from r in db.t_question_knowledge_ref
                                    join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                    where r.delete_flag == 0 && r.question_id == item.id
                                    select new { t.src_id, t.tag }).ToList();
                    List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                    foreach (var item1 in queryTag)
                    {
                        listTag.Add(new KnowledgeTag
                        {
                            ID = (long)item1.src_id,
                            Tag = item1.tag
                        });
                    }
                    int nUseCount = 0;
                    var queryUse = (from s in db.t_question_statistic
                                    where s.delete_flag == 0 && s.questionid == item.id
                                    select s).FirstOrDefault();
                    if (queryUse != null)
                        nUseCount = (int)queryUse.use_count;

                    List<OptionInfo> optionInfos = new List<OptionInfo>();
                    var queryOption = (from o in db.t_question_option
                                       where o.delete_flag == 0 && o.question_id == item.id
                                       select o).ToList();
                    foreach (var option in queryOption)
                    {
                        optionInfos.Add(new OptionInfo
                        {
                            QuestionID = item.id,
                            OptionNum = option.option_number,
                            OptionContent = option.option_content
                        });
                    }

                    list.Add(new Question()
                    {
                        ID = item.id,
                        QuestionTitle = item.question_title,
                        QuestionType = item.question_type,
                        Complexity = item.complexity,
                        CreateTime = item.t_create.ToString(),
                        CreateName = item.create_name,
                        KnowledgeTags = listTag,
                        AnswerAnalyze = item.answer_analyze,
                        QuestionAnswer = item.question_answer,
                        UseCount = nUseCount,
                        OptionInfoList = optionInfos
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

        public async Task<object> AddQuestionToBasket(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionAttribute> idList, TokenModel obj)
        {
            try
            {
                //查找排序
                var querySort = from b in db.t_question_basket
                                where b.delete_flag == 0 && b.user_number == obj.userNumber
                                select b;
                int nSort = querySort.Count();
                for (int i = 0; i < idList.Count; i++)
                {
                    int nTypeSort = int.Parse(idList[i].QuestionType);

                    //查找当前题型排序
                    var query = (from q in db.t_question_basket
                                 where q.delete_flag == 0 && q.user_number == obj.userNumber && q.question_type == idList[i].QuestionType
                                 select q).FirstOrDefault();
                    if (query != null)
                        nTypeSort = (int)query.question_type_sort;
                    else
                    {
                        var sort = db.t_question_basket.Where(x => x.delete_flag == 0 && x.user_number == obj.userNumber).Select(x => (int)x.question_type_sort).ToList();
                        nTypeSort = GetTypeSort(sort);
                    }
                    t_question_basket basket = new t_question_basket();
                    basket.question_id = idList[i].ID;
                    basket.question_type = idList[i].QuestionType;
                    basket.user_number = obj.userNumber;
                    basket.complexity = idList[i].Complexity;
                    basket.question_sort = ++nSort;
                    basket.question_type_sort = nTypeSort;
                    db.t_question_basket.Add(basket);
                }
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 2;
                log.logDesc = "将试题添加到了试题篮";
                log.logSuccessd = 1;
                log.moduleName = "知识点组卷";
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
        private int GetTypeSort(List<int> list)
        {
            int i = 0;
            if (!list.Contains(1))
                i = 1;
            if (!list.Contains(2))
                i = 2;
            if (!list.Contains(3))
                i = 3;
            if (!list.Contains(4))
                i = 4;
            if (!list.Contains(5))
                i = 5;
            return i;
        }

        public object CancleSelectedQuestion(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, string userNumber, long questionId, TokenModel obj)
        {
            try
            {
                var query = (from b in db.t_question_basket
                             where b.delete_flag == 0 && b.question_id == questionId && b.user_number == userNumber
                             select b).ToList();
                foreach (var item in query)
                {
                    item.delete_flag = 1;
                }
                db.SaveChanges();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "取消选择了试题";
                log.logSuccessd = 1;
                log.moduleName = "知识点组卷";
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

        #region 我的组卷

        public object GetMyTestPaper(pf_exam_paper_questionsContext db, string startTime, string endTime, string keyWord, string userNumber, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_test_papers> query = null;
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    DateTime start = DateTime.Parse(startTime);
                    DateTime end = DateTime.Parse(endTime);

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.create_number == userNumber
                            && t.t_create >= start && t.t_create <= end
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;

                }
                else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    DateTime start = DateTime.Parse(startTime);

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.create_number == userNumber
                            && t.t_create >= start
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;
                }
                else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    DateTime end = DateTime.Parse(endTime);

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.create_number == userNumber
                            && t.t_create <= end
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;
                }
                else
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.create_number == userNumber
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;

                }
                var queryList = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                List<Paper> list = new List<Paper>();
                foreach (var item in queryList)
                {
                    list.Add(new Paper
                    {
                        ID = item.id,
                        PaperTitle = item.paper_title,
                        PaperConfidential = item.paper_confidential,
                        Complexity = item.complexity,
                        QuestionCount = (int)item.question_count,
                        ExamScore = (int)item.exam_score,
                        UserName = item.create_name,
                        ShareFlag = item.share_flag,
                        CreateTime = item.t_modified
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

        public async Task<object> ShareMyTestPaper(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, long id, TokenModel obj)
        {
            try
            {
                var count = db.t_questions_bus.Where(x => x.delete_flag == 0 && x.test_paper_id == id).Count();
                if (count == 0)
                    return new { code = 401, message = "试卷下不存在试题，不得发布！" };
                var query = (from t in db.t_test_papers
                             where t.delete_flag == 0 && t.id == id
                             select t).FirstOrDefault();
                if (query.exam_score == 0)
                    return new { code = 401, message = "试卷分值不能为零！" };
                query.share_flag = 1;
                query.approval_status = "3";//提交审核
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "提交审核试卷：" + query.paper_title;
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        public async Task<object> CancleShareMyTestPaper(pf_exam_paper_questionsContext db, long id)
        {
            try
            {
                var query = (from t in db.t_test_papers
                             where t.delete_flag == 0 && t.id == id
                             select t).FirstOrDefault();
                query.share_flag = 0;
                await db.SaveChangesAsync();
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public async Task<object> RemoveTestPaper(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, long id, TokenModel obj)
        {
            try
            {
                var query = (from t in db.t_test_papers
                             where t.delete_flag == 0 && t.id == id
                             select t).FirstOrDefault();
                query.delete_flag = 1;
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "删除了试卷：" + query.paper_title;
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        public async Task<object> RemoveQuestionFromTestPaper(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, long id, long paperid, TokenModel obj)
        {
            try
            {
                long srcid = 0;
                var query = from q in db.t_questions_bus
                            where q.delete_flag == 0 && q.id == id
                            select q;
                foreach (var item in query)
                {
                    item.delete_flag = 1;
                    srcid = (long)item.src_id;
                }
                //更新使用次数
                var srcquestion = db.t_questions.Find(srcid);
                srcquestion.use_count = srcquestion.use_count - 1;
                db.SaveChanges();
                await UpdateTestPaperScore(db, paperid, true);

                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "从试卷中删除了试题";
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        public async Task<object> RemoveQuestionsByType(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, string questionType, TokenModel obj)
        {
            try
            {
                long paperid = 0;
                var queryQuestionList = (from q in db.t_questions_bus
                                         where q.delete_flag == 0 && q.question_type == questionType && q.create_number == obj.userNumber
                                         select q).ToList();
                foreach (var item in queryQuestionList)
                {
                    item.delete_flag = 1;
                    paperid = (long)item.test_paper_id;

                    //更新使用次数
                    var srcquestion = db.t_questions.Find(item.src_id);
                    srcquestion.use_count = srcquestion.use_count - 1;
                }
                db.SaveChanges();

                await UpdateTestPaperScore(db, paperid, true);
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "按题型删除了试题";
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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
        public async Task<object> SetQuestionsScore(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionTypeScore> typeScoreList, TokenModel obj)
        {
            try
            {
                if (typeScoreList != null && typeScoreList.Count > 0)
                {
                    long paperid = 0;
                    for (int i = 0; i < typeScoreList.Count; i++)
                    {
                        var query = from b in db.t_questions_bus
                                    where b.delete_flag == 0 && b.create_number == obj.userNumber && b.id == typeScoreList[i].ID
                                    select b;
                        foreach (var item in query)
                        {
                            paperid = (long)item.test_paper_id;
                            item.question_score = typeScoreList[i].Score;
                        }
                    }
                    db.SaveChanges();
                    await UpdateTestPaperScore(db, paperid, false);
                }
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "设置了试题分值";
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        private async Task<object> UpdateTestPaperScore(pf_exam_paper_questionsContext db, long paperid, bool flag)
        {
            try
            {
                var queryQuestion = from q in db.t_questions_bus
                                    where q.test_paper_id == paperid && q.delete_flag == 0
                                    select q;
                //更新试卷中试题数量、试卷分值
                var questionCount = queryQuestion.Count();
                var examScore = queryQuestion.Sum(x => x.question_score);
                var queryPaper = from p in db.t_test_papers
                                 where p.delete_flag == 0 && p.id == paperid
                                 select p;
                foreach (var item in queryPaper)
                {
                    item.question_count = questionCount;
                    item.exam_score = examScore;
                    if (flag)
                    {
                        item.share_flag = 0;
                        item.approval_status = "1";
                    }
                }
                await db.SaveChangesAsync();
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public async Task<object> MyTestPaperQuestionsSort(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionTypeScore> questionsSort, TokenModel obj)
        {
            try
            {
                if (questionsSort != null && questionsSort.Count > 0)
                {
                    for (int i = 0; i < questionsSort.Count; i++)
                    {
                        var query = from q in db.t_questions_bus
                                    where q.delete_flag == 0 && q.id == questionsSort[i].ID
                                    select q;
                        foreach (var item in query)
                        {
                            item.question_sort = questionsSort[i].QuestionSort;
                        }
                    }
                }
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "变更了试题排序";
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        public async Task<object> MyTestPaperQuestionTypeSort(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionTypeScore> questionsSort, TokenModel obj)
        {
            try
            {
                if (questionsSort != null && questionsSort.Count > 0)
                {
                    for (int i = 0; i < questionsSort.Count; i++)
                    {
                        var query = from q in db.t_questions_bus
                                    where q.delete_flag == 0 && q.create_number == obj.userNumber && q.question_type == questionsSort[i].QuestionType
                                    select q;
                        foreach (var item in query)
                        {
                            item.question_type_sort = questionsSort[i].QuestionTypeSort;
                        }
                    }
                }
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "变更了试题排序";
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        public object GetMyTestPaperQuestions(pf_exam_paper_questionsContext db, long id)
        {
            try
            {
                Paper paper = new Paper();
                List<QuestionOrderByType> questionTypeSortList = new List<QuestionOrderByType>();
                //选项集合
                List<OptionInfo> optionsList = new List<OptionInfo>();
                //题干集合
                List<Question> QuestionList = new List<Question>();

                var queryOption = from b in db.t_test_papers
                                  join q in db.t_questions_bus on b.id equals q.test_paper_id
                                  join o in db.t_question_option_bus on q.id equals o.question_id into qo
                                  from _qo in qo.DefaultIfEmpty()
                                  where b.id == id && b.delete_flag == 0 && q.delete_flag == 0 && _qo.delete_flag == 0
                                  orderby _qo.option_number ascending
                                  select new
                                  {
                                      id = _qo == null ? 0 : _qo.id,
                                      questionid = _qo == null ? 0 : _qo.question_id,
                                      optionnum = _qo == null ? "null" : _qo.option_number,
                                      optioncontent = _qo == null ? "null" : _qo.option_content
                                  };
                var queryOptionList = queryOption.ToList();
                foreach (var item in queryOptionList)
                {
                    optionsList.Add(new OptionInfo
                    {
                        ID = item.id,
                        QuestionID = item.questionid,
                        OptionNum = item.optionnum,
                        OptionContent = item.optioncontent
                    });
                }
                int index = 0;
                var queryQuestion = from b in db.t_test_papers
                                    join q in db.t_questions_bus on b.id equals q.test_paper_id into bq
                                    from _bq in bq.DefaultIfEmpty()
                                    where b.id == id && b.delete_flag == 0 && _bq.delete_flag == 0
                                    orderby _bq.question_type_sort, _bq.question_sort ascending
                                    select new
                                    {
                                        paperid = b.id,
                                        srcid = _bq == null ? 0 : _bq.src_id,
                                        questionscore = _bq == null ? 0 : _bq.question_score,
                                        questionsort = _bq.question_sort,
                                        questiontypesort = _bq.question_type_sort,
                                        id = _bq == null ? 0 : _bq.id,
                                        questiontype = _bq == null ? "null" : _bq.question_type,
                                        complexity = _bq == null ? "null" : _bq.complexity,
                                        questiontitle = _bq == null ? "null" : _bq.question_title,
                                        answeranalyze = _bq == null ? "null" : _bq.answer_analyze,
                                        standanswer = _bq == null ? "null" : _bq.question_answer,
                                        createtime = _bq.t_create
                                    };
                var queryQuestionList = queryQuestion.ToList();
                foreach (var item in queryQuestionList)
                {
                    var queryTag = from r in db.t_question_tag_bus_ref
                                   join t in db.t_tag_bus on r.knowledge_tag_id equals t.id
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

                    //筛选题干下的选项
                    var a = optionsList.FindAll(x => x.QuestionID == item.id);
                    int nUseCount = 0;
                    var queryUse = (from s in db.t_question_statistic
                                    where s.delete_flag == 0 && s.questionid == item.srcid
                                    select s).FirstOrDefault();
                    if (queryUse != null)
                        nUseCount = (int)queryUse.use_count;
                    QuestionList.Add(new Question()
                    {

                        Number = ++index,
                        ID = item.id,
                        SrcID = (long)item.srcid,
                        TestPaperID = item.paperid,
                        QuestionScore = item.questionscore,
                        QuestionTitle = item.questiontitle,
                        QuestionType = item.questiontype,
                        Complexity = item.complexity,
                        OptionInfoList = a,
                        UseCount = nUseCount,
                        AnswerAnalyze = item.answeranalyze,
                        QuestionAnswer = item.standanswer,
                        QuestionSort = (int)item.questionsort,
                        QuestionTypeSort = (int)item.questiontypesort,
                        CreateTime = ((DateTime)item.createtime).ToString(),
                        KnowledgeTags = listTag
                    });
                }
                questionTypeSortList = QuestionTypeSort(QuestionList);
                var queryTestPaper = db.t_test_papers.Find(id);
                paper.ID = queryTestPaper.id;
                paper.PaperTitle = queryTestPaper.paper_title;
                paper.PaperConfidential = queryTestPaper.paper_confidential;
                paper.QuestionCount = queryTestPaper.question_count;
                paper.ExamScore = queryTestPaper.exam_score;
                paper.Complexity = queryTestPaper.complexity;
                paper.CreateTime = queryTestPaper.t_create;
                paper.UserName = queryTestPaper.create_name;
                paper.ShareFlag = queryTestPaper.share_flag;
                return new
                {
                    code = 200,
                    result = new
                    {
                        paper,
                        questionTypeSortList,
                        count = queryQuestion.Count()
                    },
                    message = "OK"
                };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public async Task<object> EditTestPaperTitle(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, long paperid, string paperTitle, TokenModel obj)
        {
            try
            {
                var queryTitle = db.t_test_papers.Find(paperid);
                queryTitle.paper_title = paperTitle;
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "试卷标题变更为：" + paperTitle;
                log.logSuccessd = 1;
                log.moduleName = "我的组卷";
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

        public object GetMyTestPaperAnalyze(pf_exam_paper_questionsContext db, long id)
        {
            try
            {
                List<QuestionTypePercent> TypeList = new List<QuestionTypePercent>();
                List<QuestionComplexityPercent> ComplexityList = new List<QuestionComplexityPercent>();
                List<QusetionCountAnalyze> QusetionCountAnalyzeList = new List<QusetionCountAnalyze>();

                var queryCount = db.t_questions_bus.Where(x => x.delete_flag == 0 && x.test_paper_id == id).Count();
                if (queryCount == 0)
                    return new { code = 200, message = "OK" };

                //题量
                //主观题数量
                var querySubjectiveCount = db.t_questions_bus.Where(x => x.question_type == "5" && x.delete_flag == 0 && x.test_paper_id == id).Count();
                //主观题
                QusetionCountAnalyzeList.Add(new QusetionCountAnalyze
                {
                    QuestionCountType = "subjective",
                    Percent = ((decimal)querySubjectiveCount * 100 / queryCount).ToString("#0"),
                    QuestionCount = querySubjectiveCount.ToString()
                });
                //客观题
                var objectiveCount = queryCount - querySubjectiveCount;
                QusetionCountAnalyzeList.Add(new QusetionCountAnalyze
                {
                    QuestionCountType = "objective",
                    Percent = ((decimal)objectiveCount * 100 / queryCount).ToString("#0"),
                    QuestionCount = objectiveCount.ToString()
                });
                QusetionCountAnalyzeList = QusetionCountAnalyzeList.OrderByDescending(x => x.QuestionCount).ToList();

                //题型
                var queryTypeGroup = db.t_questions_bus.Where(x => x.delete_flag == 0 && x.test_paper_id == id).GroupBy(x => new { x.question_type }).Select(x => new { x.Key.question_type, count = x.Count() }).ToList();

                //难易度
                var queryCompxityGroup = db.t_questions_bus.Where(x => x.delete_flag == 0 && x.test_paper_id == id).GroupBy(x => new { x.complexity }).Select(x => new { x.Key.complexity, count = x.Count() }).ToList();

                foreach (var item in queryTypeGroup)
                {
                    decimal p = (decimal)item.count * 100 / queryCount;
                    TypeList.Add(new QuestionTypePercent
                    {
                        QuestionType = item.question_type,
                        Percent = p.ToString("#0")
                    });
                }

                foreach (var item in queryCompxityGroup)
                {
                    decimal p = (decimal)item.count * 100 / queryCount;
                    ComplexityList.Add(new QuestionComplexityPercent
                    {
                        Complexity = item.complexity,
                        Percent = p.ToString("#0")
                    });
                }

                if (TypeList.Find(x => x.QuestionType == "1") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "1", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "2") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "2", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "3") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "3", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "4") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "4", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "5") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "5", Percent = "0" });
                TypeList = TypeList.OrderByDescending(x => x.Percent).ToList();

                if (ComplexityList.Find(x => x.Complexity == null || x.Complexity == "") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "0", Percent = "0" });
                if (ComplexityList.Find(x => x.Complexity == "1") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "1", Percent = "0" });
                if (ComplexityList.Find(x => x.Complexity == "3") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "3", Percent = "0" });
                if (ComplexityList.Find(x => x.Complexity == "5") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "5", Percent = "0" });

                List<KnowledgeTag> TagList = KnowledgeList(db, id);
                var tagCount = TagList.Count;
                if (tagCount == 0)
                    return new
                    {
                        code = 200,
                        result = new
                        {
                            QusetionCountAnalyzeList,
                            TypeList,
                            ComplexityList
                        },
                        message = "OK"
                    };

                var tag = TagList.GroupBy(x => new { x.ID, x.Tag }).Select(a => new { a.Key.Tag, count = a.Count() }).OrderByDescending(arg => arg.count).ToList();
                List<TagPercent> tagList = new List<TagPercent>();
                foreach (var item in tag)
                {
                    decimal t = (decimal)item.count * 100 / tagCount;
                    tagList.Add(new TagPercent
                    {
                        TagName = item.Tag,
                        Percent = t.ToString("#0")
                    });
                }
                var Tags = tagList.Take(6);
                IEnumerable<TagPercent> richTags = null;
                IEnumerable<TagPercent> poolTags = null;
                if (tagList.Count >= 6)
                {
                    richTags = tagList.Take(3);
                    poolTags = tagList.TakeLast(3);
                }
                else
                {
                    int c = tagList.Count;
                    if (c % 2 == 1)
                    {
                        richTags = tagList.Take((c + 1) / 2);
                        poolTags = tagList.TakeLast((c - 1) / 2);
                    }
                    else
                    {
                        richTags = tagList.Take(c / 2);
                        poolTags = tagList.TakeLast(c / 2);
                    }
                }

                return new
                {
                    code = 200,
                    result = new
                    {
                        QusetionCountAnalyzeList,
                        TypeList,
                        ComplexityList,
                        Tag = new
                        {
                            Tags,
                            richTags,
                            poolTags
                        }
                    },
                    message = "OK"
                };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        private List<KnowledgeTag> KnowledgeList(pf_exam_paper_questionsContext db, long id)
        {
            try
            {
                var queryTag = (from a in db.t_questions_bus
                                join r in db.t_question_tag_bus_ref on a.id equals r.question_id
                                join t in db.t_tag_bus on r.knowledge_tag_id equals t.id
                                where r.delete_flag == 0 && a.delete_flag == 0 && a.test_paper_id == id
                                select new { t.src_id, t.tag }).ToList();
                List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                foreach (var item1 in queryTag)
                {
                    listTag.Add(new KnowledgeTag
                    {
                        ID = (long)item1.src_id,
                        Tag = item1.tag
                    });
                }
                return listTag;

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }

        #endregion

        #region 试卷篮

        public object GetBasketQuestions(pf_exam_paper_questionsContext db, string userNumber)
        {
            try
            {
                List<QuestionOrderByType> questionTypeSortList = new List<QuestionOrderByType>();
                //选项集合
                List<OptionInfo> optionsList = new List<OptionInfo>();
                //题干集合
                List<Question> QuestionList = new List<Question>();

                var queryOption = from b in db.t_question_basket
                                  join q in db.t_questions on b.question_id equals q.id
                                  join o in db.t_question_option on q.id equals o.question_id into qo
                                  from _qo in qo.DefaultIfEmpty()
                                  where b.user_number == userNumber && b.delete_flag == 0 && _qo.delete_flag == 0
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
                int index = 0;
                var queryQuestion = from b in db.t_question_basket
                                    join q in db.t_questions on b.question_id equals q.id into bq
                                    from _bq in bq.DefaultIfEmpty()
                                    where b.user_number == userNumber && b.delete_flag == 0
                                    orderby b.question_type_sort, b.question_sort ascending
                                    select new
                                    {
                                        basketid = b.id,
                                        questionscore = b == null ? 0 : b.question_score,
                                        questionsort = b.question_sort,
                                        questiontypesort = b.question_type_sort,
                                        id = _bq == null ? 0 : _bq.id,
                                        questiontype = _bq == null ? "null" : _bq.question_type,
                                        complexity = _bq == null ? "null" : _bq.complexity,
                                        questiontitle = _bq == null ? "null" : _bq.question_title,
                                        answeranalyze = _bq == null ? "null" : _bq.answer_analyze,
                                        standanswer = _bq == null ? "null" : _bq.question_answer
                                    };
                var queryQuestionList = queryQuestion.ToList();
                foreach (var item in queryQuestionList)
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

                    //筛选题干下的选项
                    var a = optionsList.FindAll(x => x.QuestionID == item.id);
                    QuestionList.Add(new Question()
                    {
                        Number = ++index,
                        ID = item.id,
                        BasketID = item.basketid,
                        QuestionScore = item.questionscore,
                        QuestionTitle = item.questiontitle,
                        QuestionType = item.questiontype,
                        Complexity = item.complexity,
                        OptionInfoList = a,
                        AnswerAnalyze = item.answeranalyze,
                        QuestionAnswer = item.standanswer,
                        QuestionSort = item.questionsort,
                        QuestionTypeSort = (int)item.questiontypesort,
                        KnowledgeTags = listTag
                    });
                }
                questionTypeSortList = QuestionTypeSort(QuestionList);
                return new
                {
                    code = 200,
                    result = new
                    {
                        questionTypeSortList,
                        count = queryQuestion.Count()
                    },
                    message = "OK"
                };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        private List<QuestionOrderByType> QuestionTypeSort(List<Question> QuestionList)
        {
            List<QuestionOrderByType> questionTypeSortList = new List<QuestionOrderByType>();
            List<Question> tempList1 = QuestionList.FindAll(x => x.QuestionTypeSort == 1);
            if (tempList1 != null && tempList1.Count > 0)
            {
                if (tempList1[0].QuestionType == "1")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "1",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 1)
                    });
                }
                else if (tempList1[0].QuestionType == "2")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "2",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 1)
                    });
                }
                else if (tempList1[0].QuestionType == "3")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "3",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 1)
                    });
                }
                else if (tempList1[0].QuestionType == "4")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "4",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 1)
                    });
                }
                else if (tempList1[0].QuestionType == "5")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "5",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 1)
                    });
                }
            }

            List<Question> tempList2 = QuestionList.FindAll(x => x.QuestionTypeSort == 2);
            if (tempList2 != null && tempList2.Count > 0)
            {
                if (tempList2[0].QuestionType == "1")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "1",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 2)
                    });
                }
                else if (tempList2[0].QuestionType == "2")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "2",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 2)
                    });
                }
                else if (tempList2[0].QuestionType == "3")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "3",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 2)
                    });
                }
                else if (tempList2[0].QuestionType == "4")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "4",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 2)
                    });
                }
                else if (tempList2[0].QuestionType == "5")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "5",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 2)
                    });
                }
            }

            List<Question> tempList3 = QuestionList.FindAll(x => x.QuestionTypeSort == 3);
            if (tempList3 != null && tempList3.Count > 0)
            {
                if (tempList3[0].QuestionType == "1")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "1",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 3)
                    });
                }
                else if (tempList3[0].QuestionType == "2")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "2",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 3)
                    });
                }
                else if (tempList3[0].QuestionType == "3")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "3",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 3)
                    });
                }
                else if (tempList3[0].QuestionType == "4")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "4",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 3)
                    });
                }
                else if (tempList3[0].QuestionType == "5")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "5",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 3)
                    });
                }
            }

            List<Question> tempList4 = QuestionList.FindAll(x => x.QuestionTypeSort == 4);
            if (tempList4 != null && tempList4.Count > 0)
            {
                if (tempList4[0].QuestionType == "1")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "1",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 4)
                    });
                }
                else if (tempList4[0].QuestionType == "2")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "2",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 4)
                    });
                }
                else if (tempList4[0].QuestionType == "3")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "3",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 4)
                    });
                }
                else if (tempList4[0].QuestionType == "4")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "4",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 4)
                    });
                }
                else if (tempList4[0].QuestionType == "5")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "5",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 4)
                    });
                }
            }

            List<Question> tempList5 = QuestionList.FindAll(x => x.QuestionTypeSort == 5);
            if (tempList5 != null && tempList5.Count > 0)
            {
                if (tempList5[0].QuestionType == "1")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "1",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 5)
                    });
                }
                else if (tempList5[0].QuestionType == "2")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "2",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 5)
                    });
                }
                else if (tempList5[0].QuestionType == "3")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "3",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 5)
                    });
                }
                else if (tempList5[0].QuestionType == "4")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "4",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 5)
                    });
                }
                else if (tempList5[0].QuestionType == "5")
                {
                    questionTypeSortList.Add(new QuestionOrderByType
                    {
                        QuestionType = "5",
                        questions = QuestionList.FindAll(x => x.QuestionTypeSort == 5)
                    });
                }
            }

            return questionTypeSortList;
        }
        public async Task<object> BasketQuestionsSort(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionTypeScore> questionsSort, TokenModel obj)
        {
            try
            {
                if (questionsSort != null && questionsSort.Count > 0)
                {
                    for (int i = 0; i < questionsSort.Count; i++)
                    {
                        var query = from q in db.t_question_basket
                                    where q.delete_flag == 0 && q.id == questionsSort[i].ID
                                    select q;
                        foreach (var item in query)
                        {
                            item.question_sort = questionsSort[i].QuestionSort;
                        }
                    }
                }
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "试卷篮试题排序";
                log.logSuccessd = 1;
                log.moduleName = "试卷篮管理";
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

        public object BasketQuestionTypeSort(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionTypeScore> questionsSort, TokenModel obj)
        {
            try
            {
                if (questionsSort != null && questionsSort.Count > 0)
                {
                    for (int i = 0; i < questionsSort.Count; i++)
                    {
                        var query = from q in db.t_question_basket
                                    where q.delete_flag == 0 && q.user_number == obj.userNumber && q.question_type == questionsSort[i].QuestionType
                                    select q;
                        foreach (var item in query)
                        {
                            item.question_type_sort = questionsSort[i].QuestionTypeSort;
                        }
                    }
                }
                db.SaveChanges();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "试卷篮试题题型排序";
                log.logSuccessd = 1;
                log.moduleName = "试卷篮管理";
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
        public async Task<object> RemoveQuestionFromBasket(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, long basketID, TokenModel obj)
        {
            try
            {
                var query = (from b in db.t_question_basket
                             where b.id == basketID && b.delete_flag == 0
                             select b).FirstOrDefault();
                query.delete_flag = 1;
                await db.SaveChangesAsync();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "从试卷篮删除了试题";
                log.logSuccessd = 1;
                log.moduleName = "试卷篮管理";
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
        public object RemoveQuestionByQuestionType(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, string questionType, TokenModel obj)
        {
            try
            {
                var query = from q in db.t_question_basket
                            where q.delete_flag == 0 && q.question_type == questionType && q.user_number == obj.userNumber
                            select q;
                foreach (var item in query)
                {
                    item.delete_flag = 1;
                }
                db.SaveChanges();
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 4;
                log.logDesc = "按照题型删除了试题";
                log.logSuccessd = 1;
                log.moduleName = "试卷篮管理";
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

        public async Task<object> SetQuestionScore(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<QuestionTypeScore> typeScoreList, TokenModel obj)
        {
            try
            {
                if (typeScoreList != null && typeScoreList.Count > 0)
                {
                    for (int i = 0; i < typeScoreList.Count; i++)
                    {
                        var query = from b in db.t_question_basket
                                    where b.delete_flag == 0 && b.user_number == obj.userNumber && b.id == typeScoreList[i].ID
                                    select b;
                        foreach (var item in query)
                        {
                            item.question_score = typeScoreList[i].Score;
                        }
                    }
                    db.SaveChanges();

                    await db.SaveChangesAsync();

                }
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "设置了试卷分值";
                log.logSuccessd = 1;
                log.moduleName = "试卷篮管理";
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

        public object GetTestPaperAnalyze(pf_exam_paper_questionsContext db, string userNumber)
        {
            try
            {
                List<QuestionTypePercent> TypeList = new List<QuestionTypePercent>();
                List<QuestionComplexityPercent> ComplexityList = new List<QuestionComplexityPercent>();
                List<QusetionCountAnalyze> QusetionCountAnalyzeList = new List<QusetionCountAnalyze>();

                var queryCount = db.t_question_basket.Where(x => x.delete_flag == 0 && x.user_number == userNumber).Count();
                if (queryCount == 0)
                    return new { code = 200, message = "OK" };

                //题量
                //主观题数量
                var querySubjectiveCount = db.t_question_basket.Where(x => x.question_type == "5" && x.delete_flag == 0 && x.user_number == userNumber).Count();
                //主观题
                QusetionCountAnalyzeList.Add(new QusetionCountAnalyze
                {
                    QuestionCountType = "subjective",
                    Percent = ((decimal)querySubjectiveCount * 100 / queryCount).ToString("#0"),
                    QuestionCount = querySubjectiveCount.ToString()
                });
                //客观题
                var objectiveCount = queryCount - querySubjectiveCount;
                QusetionCountAnalyzeList.Add(new QusetionCountAnalyze
                {
                    QuestionCountType = "objective",
                    Percent = ((decimal)objectiveCount * 100 / queryCount).ToString("#0"),
                    QuestionCount = objectiveCount.ToString()
                });
                QusetionCountAnalyzeList = QusetionCountAnalyzeList.OrderByDescending(x => x.QuestionCount).ToList();

                //题型
                var queryTypeGroup = db.t_question_basket.Where(x => x.delete_flag == 0 && x.user_number == userNumber).GroupBy(x => new { x.question_type }).Select(x => new { x.Key.question_type, count = x.Count() }).ToList();

                //难易度
                var queryCompxityGroup = db.t_question_basket.Where(x => x.delete_flag == 0 && x.user_number == userNumber).GroupBy(x => new { x.complexity }).Select(x => new { x.Key.complexity, count = x.Count() }).ToList();

                foreach (var item in queryTypeGroup)
                {
                    decimal p = (decimal)item.count * 100 / queryCount;
                    TypeList.Add(new QuestionTypePercent
                    {
                        QuestionType = item.question_type,
                        Percent = p.ToString("#0")
                    });
                }

                foreach (var item in queryCompxityGroup)
                {
                    decimal p = (decimal)item.count * 100 / queryCount;
                    ComplexityList.Add(new QuestionComplexityPercent
                    {
                        Complexity = item.complexity,
                        Percent = p.ToString("#0")
                    });
                }

                if (TypeList.Find(x => x.QuestionType == "1") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "1", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "2") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "2", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "3") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "3", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "4") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "4", Percent = "0" });
                if (TypeList.Find(x => x.QuestionType == "5") == null)
                    TypeList.Add(new QuestionTypePercent { QuestionType = "5", Percent = "0" });
                TypeList = TypeList.OrderByDescending(x => x.Percent).ToList();

                if (ComplexityList.Find(x => x.Complexity == null || x.Complexity == "") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "0", Percent = "0" });
                if (ComplexityList.Find(x => x.Complexity == "1") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "1", Percent = "0" });
                if (ComplexityList.Find(x => x.Complexity == "3") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "3", Percent = "0" });
                if (ComplexityList.Find(x => x.Complexity == "5") == null)
                    ComplexityList.Add(new QuestionComplexityPercent { Complexity = "5", Percent = "0" });

                List<KnowledgeTag> TagList = KnowledgeList(db, userNumber);
                var tagCount = TagList.Count;
                if (tagCount == 0)
                    return new
                    {
                        code = 200,
                        result = new
                        {
                            QusetionCountAnalyzeList,
                            TypeList,
                            ComplexityList
                        },
                        message = "OK"
                    };

                var tag = TagList.GroupBy(x => new { x.ID, x.Tag }).Select(a => new { a.Key.Tag, count = a.Count() }).OrderByDescending(arg => arg.count).ToList();
                List<TagPercent> tagList = new List<TagPercent>();
                foreach (var item in tag)
                {
                    decimal t = (decimal)item.count * 100 / tagCount;
                    tagList.Add(new TagPercent
                    {
                        TagName = item.Tag,
                        Percent = t.ToString("#0")
                    });
                }
                var Tags = tagList.Take(6);
                IEnumerable<TagPercent> richTags = null;
                IEnumerable<TagPercent> poolTags = null;
                if (tagList.Count >= 6)
                {
                    richTags = tagList.Take(3);
                    poolTags = tagList.TakeLast(3);
                }
                else
                {
                    int c = tagList.Count;
                    if (c % 2 == 1)
                    {
                        richTags = tagList.Take((c + 1) / 2);
                        poolTags = tagList.TakeLast((c - 1) / 2);
                    }
                    else
                    {
                        richTags = tagList.Take(c / 2);
                        poolTags = tagList.TakeLast(c / 2);
                    }
                }

                return new
                {
                    code = 200,
                    result = new
                    {
                        QusetionCountAnalyzeList,
                        TypeList,
                        ComplexityList,
                        Tag = new
                        {
                            Tags,
                            richTags,
                            poolTags
                        }
                    },
                    message = "OK"
                };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        private List<KnowledgeTag> KnowledgeList(pf_exam_paper_questionsContext db, string userNumber)
        {
            try
            {
                var queryTag = (from a in db.t_question_basket
                                join r in db.t_question_knowledge_ref on a.question_id equals r.question_id
                                join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                where r.delete_flag == 0 && a.delete_flag == 0 && a.user_number == userNumber
                                select new { t.src_id, t.tag }).ToList();
                List<KnowledgeTag> listTag = new List<KnowledgeTag>();
                foreach (var item1 in queryTag)
                {
                    listTag.Add(new KnowledgeTag
                    {
                        ID = (long)item1.src_id,
                        Tag = item1.tag
                    });
                }
                return listTag;

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }

        public async Task<object> SaveBasketQuestions(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, Paper paperInfo, TokenModel obj)
        {
            try
            {
                List<QuestionTypeScore> questionTypeScores = new List<QuestionTypeScore>();
                var queryBasket = from t in db.t_question_basket
                                  where t.delete_flag == 0 && t.user_number == obj.userNumber
                                  select t;
                if (queryBasket.Count() == 0)
                    return new { code = 401, message = "试卷篮中不存在试题，不可以保存！" };
                foreach (var item in queryBasket)
                {
                    item.delete_flag = 1;//删除试卷篮中题目
                    questionTypeScores.Add(new QuestionTypeScore
                    {
                        ID = (long)item.question_id,
                        Score = item.question_score,
                        QuestionSort = item.question_sort
                    });
                }
                await db.SaveChangesAsync();

                t_test_papers paper = new t_test_papers();
                paper.paper_confidential = paperInfo.PaperConfidential;
                paper.paper_title = paperInfo.PaperTitle;
                paper.question_count = paperInfo.QuestionCount;
                paper.exam_score = paperInfo.ExamScore;
                paper.complexity = paperInfo.Complexity;
                paper.create_number = obj.userNumber;
                paper.create_name = obj.userName;
                paper.approval_status = "1";
                db.t_test_papers.Add(paper);
                db.SaveChanges();

                List<Question> questionList = GetQuestionsById(db, questionTypeScores);
                AddQuestions(db, questionList, paper.id, obj);

                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 2;
                log.logDesc = "保存了试卷篮试题，生成了试卷：" + paper.paper_confidential;
                log.logSuccessd = 1;
                log.moduleName = "试卷篮管理";
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
        private List<Question> GetQuestionsById(pf_exam_paper_questionsContext db, List<QuestionTypeScore> idList)
        {
            try
            {
                List<Question> questions = new List<Question>();
                if (idList != null && idList.Count > 0)
                {
                    for (int i = 0; i < idList.Count; i++)
                    {
                        //题型
                        Question question = new Question();
                        var queryQuestion = (from q in db.t_questions
                                             where q.delete_flag == 0 && q.id == idList[i].ID && q.approval_status == "3"
                                             select q).FirstOrDefault();
                        question.ID = queryQuestion.id;
                        question.QuestionTitle = queryQuestion.question_title;
                        question.QuestionType = queryQuestion.question_type;
                        question.Complexity = queryQuestion.complexity;
                        question.QuestionAnswer = queryQuestion.question_answer;
                        question.AnswerAnalyze = queryQuestion.answer_analyze;
                        question.QuestionSort = idList[i].QuestionSort;//排序
                        question.QuestionConfidential = queryQuestion.question_confidential;
                        question.QuestionScore = idList[i].Score;

                        //选项
                        List<OptionInfo> options = new List<OptionInfo>();
                        var queryOption = from o in db.t_question_option
                                          where o.delete_flag == 0 && o.question_id == idList[i].ID
                                          select o;
                        foreach (var item in queryOption)
                        {
                            options.Add(new OptionInfo
                            {
                                OptionNum = item.option_number,
                                OptionContent = item.option_content
                            });
                        }
                        question.OptionInfoList = options;

                        //知识点
                        List<KnowledgeTag> tags = new List<KnowledgeTag>();
                        var queryKnowledge = from r in db.t_question_knowledge_ref
                                             join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                             where r.delete_flag == 0 && r.question_id == idList[i].ID
                                             select t;
                        foreach (var item in queryKnowledge)
                        {
                            tags.Add(new KnowledgeTag
                            {
                                ID = (long)item.src_id,
                                Tag = item.tag
                            });
                        }
                        question.KnowledgeTags = tags;
                        questions.Add(question);
                    }

                }
                return questions;
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return null;
            }
        }
        private void AddQuestions(pf_exam_paper_questionsContext db, List<Question> questionList, long paperID, TokenModel obj)
        {
            try
            {

                if (questionList != null && questionList.Count > 0)
                {
                    for (int i = 0; i < questionList.Count; i++)
                    {
                        t_questions_bus question = new t_questions_bus();
                        question.test_paper_id = paperID;
                        question.src_id = questionList[i].ID;
                        question.question_type = questionList[i].QuestionType;
                        question.complexity = questionList[i].Complexity;
                        question.question_title = questionList[i].QuestionTitle;
                        question.question_answer = questionList[i].QuestionAnswer;
                        question.question_score = questionList[i].QuestionScore;
                        question.answer_analyze = questionList[i].AnswerAnalyze;
                        question.question_confidential = questionList[i].QuestionConfidential;
                        question.question_type_sort = int.Parse(questionList[i].QuestionType);
                        question.question_sort = questionList[i].QuestionSort;
                        question.create_number = obj.userNumber;
                        question.create_name = obj.userName;
                        db.t_questions_bus.Add(question);
                        db.SaveChanges();
                        long questionid = question.id;

                        //更新组卷次数
                        var queryStatistic = (from s in db.t_question_statistic
                                              where s.delete_flag == 0 && s.questionid == questionList[i].ID
                                              select s).FirstOrDefault();
                        if (queryStatistic != null)
                            queryStatistic.use_count = queryStatistic.use_count + 1;
                        else
                        {
                            t_question_statistic statistic = new t_question_statistic();
                            statistic.questionid = questionList[i].ID;
                            statistic.use_count = 1;
                            db.t_question_statistic.Add(statistic);
                        }

                        var query = (from q in db.t_questions
                                     where q.delete_flag == 0 && q.id == questionList[i].ID
                                     select q).FirstOrDefault();
                        if (query != null)
                            query.use_count = query.use_count + 1;

                        db.SaveChanges();

                        //选项
                        if (questionList[i].OptionInfoList != null && questionList[i].OptionInfoList.Count > 0)
                        {
                            for (int j = 0; j < questionList[i].OptionInfoList.Count; j++)
                            {
                                t_question_option_bus option = new t_question_option_bus();
                                option.question_id = questionid;
                                option.option_number = questionList[i].OptionInfoList[j].OptionNum;
                                option.option_content = questionList[i].OptionInfoList[j].OptionContent;
                                db.t_question_option_bus.Add(option);
                                db.SaveChanges();
                            }
                        }

                        //知识点
                        if (questionList[i].KnowledgeTags != null && questionList[i].KnowledgeTags.Count > 0)
                        {
                            for (int k = 0; k < questionList[i].KnowledgeTags.Count; k++)
                            {
                                var queryTag = (from t in db.t_tag_bus
                                                where t.delete_flag == 0 && t.src_id == questionList[i].KnowledgeTags[k].ID
                                                select t).FirstOrDefault();
                                if (queryTag == null)
                                {
                                    t_tag_bus tag = new t_tag_bus();
                                    tag.src_id = questionList[i].KnowledgeTags[k].ID;
                                    tag.tag = questionList[i].KnowledgeTags[k].Tag;
                                    db.t_tag_bus.Add(tag);
                                    db.SaveChanges();
                                    long tagid = tag.id;

                                    t_question_tag_bus_ref tagref = new t_question_tag_bus_ref();
                                    tagref.question_id = questionid;
                                    tagref.knowledge_tag_id = tagid;
                                    db.t_question_tag_bus_ref.Add(tagref);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    t_question_tag_bus_ref tagref = new t_question_tag_bus_ref();
                                    tagref.question_id = questionid;
                                    tagref.knowledge_tag_id = queryTag.id;
                                    db.t_question_tag_bus_ref.Add(tagref);
                                    db.SaveChanges();
                                }
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }

        #endregion

        #region 试卷审核

        public object GetNotApprovalTestPapers(pf_exam_paper_questionsContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
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

                List<Paper> list = new List<Paper>();
                foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    list.Add(new Paper
                    {
                        ID = item.id,
                        PaperTitle = item.paper_title,
                        QuestionCount = (int)item.question_count,
                        PaperConfidential = item.paper_confidential,
                        ApprovalStatus = item.approval_status,
                        ExamScore = (int)item.exam_score,
                        UserName = item.create_name,
                        ShareFlag = item.share_flag,
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
        public object GetApprovaledTestPapers(pf_exam_paper_questionsContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
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

                List<Paper> list = new List<Paper>();
                foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    list.Add(new Paper
                    {
                        ID = item.id,
                        PaperTitle = item.paper_title,
                        QuestionCount = (int)item.question_count,
                        PaperConfidential = item.paper_confidential,
                        ApprovalDateTime = item.approval_date,
                        ApprovalStatus = item.approval_status,
                        ApprovalUserName = item.approval_user_name,
                        ApprovalRemark = item.approval_remarks,
                        ExamScore = (int)item.exam_score,
                        UserName = item.create_name,
                        ShareFlag = item.share_flag,
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

        public object GetPassedTestPapers(pf_exam_paper_questionsContext db, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_test_papers> query = null;
                if (IsAsc && !string.IsNullOrEmpty(FieldName))
                {

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            && (t.approval_status == "3")
                            orderby PubMethod.GetPropertyValue(t, FieldName) ascending
                            select t;
                }
                else if (IsAsc == false && !string.IsNullOrEmpty(FieldName))
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            && (t.approval_status == "3")
                            orderby PubMethod.GetPropertyValue(t, FieldName) descending
                            select t;
                }
                else
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            && (t.approval_status == "3")
                            // orderby PubMethod.GetPropertyValue(t, FieldName) ascending
                            select t;
                }

                List<Paper> list = new List<Paper>();
                foreach (var item in query.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
                {
                    list.Add(new Paper
                    {
                        ID = item.id,
                        PaperTitle = item.paper_title,
                        QuestionCount = (int)item.question_count,
                        PaperConfidential = item.paper_confidential,
                        ApprovalDateTime = item.approval_date,
                        ApprovalStatus = item.approval_status,
                        ApprovalUserName = item.approval_user_name,
                        ApprovalRemark = item.approval_remarks,
                        ExamScore = (int)item.exam_score,
                        UserName = item.create_name,
                        ShareFlag = item.share_flag,
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
        public async Task<object> ApprovalTestPapers(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, List<PaperQuestionApproval> paperQuestionApprovals, TokenModel obj)
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
                            query.approval_date = DateTime.Now;
                            query.approval_remarks = paperQuestionApprovals[i].ApprovalRemark;
                            query.approval_status = paperQuestionApprovals[i].ApprovalResult;
                            query.approval_user_name = paperQuestionApprovals[i].ApprovalUserName;
                            query.approval_user_number = paperQuestionApprovals[i].ApprovalUserNumber;
                        }
                    }
                    await db.SaveChangesAsync();
                }
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 3;
                log.logDesc = "审核了试卷";
                log.logSuccessd = 1;
                log.moduleName = "试卷审核";
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

        public object GetTestPaperInfo(pf_exam_paper_questionsContext db, long id)
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
                                  join q in db.t_questions_bus on t.id equals q.test_paper_id
                                  join o in db.t_question_option_bus on q.id equals o.question_id into qo
                                  from _qo in qo.DefaultIfEmpty()
                                  where t.id == id && _qo.delete_flag == 0 && _qo.delete_flag == 0
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
                                    join q in db.t_questions_bus on t.id equals q.test_paper_id into tq
                                    from _tq in tq.DefaultIfEmpty()
                                    where t.id == id && _tq.delete_flag == 0
                                    orderby _tq.question_type_sort, _tq.question_sort ascending
                                    select new
                                    {
                                        id = _tq == null ? 0 : _tq.id,
                                        srcid = _tq == null ? 0 : _tq.src_id,
                                        testpaperid = _tq == null ? 0 : _tq.test_paper_id,
                                        questiontype = _tq == null ? "null" : _tq.question_type,
                                        questiontitle = _tq == null ? "null" : _tq.question_title,
                                        questionsort = _tq == null ? 0 : _tq.question_sort,
                                        answeranalyze = _tq == null ? "null" : _tq.answer_analyze,
                                        standanswer = _tq == null ? "null" : _tq.question_answer,
                                        questionscore = _tq == null ? 0 : _tq.question_score,
                                        questionconfidential = _tq == null ? "null" : _tq.question_confidential
                                    };
                foreach (var item in queryQuestion)
                {
                    List<KnowledgeTag> knowledgeTags = new List<KnowledgeTag>();
                    //查找知识点
                    var queryTag = from r in db.t_question_tag_bus_ref
                                   join g in db.t_tag_bus
                                   on r.knowledge_tag_id equals g.id
                                   where r.question_id == item.id && r.delete_flag == 0
                                   select g;
                    foreach (var tag in queryTag)
                    {
                        knowledgeTags.Add(new KnowledgeTag
                        {
                            ID = (long)tag.src_id,
                            Tag = tag.tag
                        });
                    }
                    int nUseCount = 0;
                    var queryUse = (from s in db.t_question_statistic
                                    where s.delete_flag == 0 && s.questionid == item.srcid
                                    select s).FirstOrDefault();
                    if (queryUse != null)
                        nUseCount = (int)queryUse.use_count;
                    //筛选题干下的选项
                    var t = optionsList.FindAll(x => x.QuestionID == item.id);
                    //添加到题干中
                    QuestionList.Add(new Question()
                    {
                        ID = item.id,
                        TestPaperID = (long)item.testpaperid,
                        UseCount = nUseCount,
                        QuestionTitle = item.questiontitle,
                        QuestionType = item.questiontype,
                        QuestionSort = (int)item.questionsort,
                        OptionInfoList = t,
                        QuestionAnswer = item.standanswer,
                        AnswerAnalyze = item.answeranalyze,
                        QuestionConfidential = item.questionconfidential,
                        QuestionScore = item.questionscore,
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
                    testPaper.ShareFlag = queryTestPaper_F.share_flag;
                    testPaper.ApprovalStatus = queryTestPaper_F.approval_status;
                    testPaper.ApprovalRemark = queryTestPaper_F.approval_remarks;
                    testPaper.PaperTitle = queryTestPaper_F.paper_title;
                    testPaper.QuestionList = QuestionList;
                    testPaper.ExamScore = queryTestPaper_F.exam_score;
                    testPaper.PaperConfidential = queryTestPaper_F.paper_confidential;
                    testPaper.QuestionCount = queryTestPaper_F.question_count;
                    testPaper.Complexity = queryTestPaper_F.complexity;
                    testPaper.CreateTime = queryTestPaper_F.t_create;
                    testPaper.UserName = queryTestPaper_F.create_name;
                }
                return new { code = 200, result = testPaper, message = "OK" };
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

    public class QuestionsQueryParameter
    {
        public string KeyWord { get; set; }
        public List<long> NodeID { get; set; }
        public string QuestionType { get; set; }
        public string Complexity { get; set; }
        public bool IsAsc { get; set; } = false;
        public string FieldName { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class QuestionAttribute
    {
        public long ID { get; set; }
        public string QuestionType { get; set; }
        public string Complexity { get; set; }
    }

    public class QuestionOrderByType
    {
        public string QuestionType { get; set; }
        public List<Question> questions { get; set; }
    }

    public class QusetionCountAnalyze
    {
        public string QuestionCountType { get; set; }
        public string Percent { get; set; }
        public string QuestionCount { get; set; }
    }

    public class QuestionTypeScore
    {
        public long ID { get; set; }
        public string QuestionType { get; set; }
        public string Complexity { get; set; }
        public int QuestionSort { get; set; }
        public int QuestionTypeSort { get; set; }
        public decimal? Score { get; set; }
    }

    public class QuestionTypePercent
    {
        public string QuestionType { get; set; }
        public string Percent { get; set; }
    }
    public class QuestionComplexityPercent
    {
        public string Complexity { get; set; }
        public string Percent { get; set; }
    }
    public class TagPercent
    {
        public string TagName { get; set; }
        public string Percent { get; set; }
    }

}
