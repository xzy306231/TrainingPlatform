using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PaperQuestions.BLL
{
    public partial class ExaminationPapers
    {
        #region 试卷库

        public object GetTestPaper(pf_exam_paper_questionsContext db, string startTime, string endTime, string keyWord, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<t_test_papers> query = null;
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    DateTime start = DateTime.Parse(startTime);
                    DateTime end = DateTime.Parse(endTime);

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.share_flag == 1 && t.approval_status == "3"
                            && t.t_create >= start && t.t_create <= end
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;

                }
                else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    DateTime start = DateTime.Parse(startTime);

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.share_flag == 1 && t.approval_status == "3"
                            && t.t_create >= start
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;
                }
                else if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    DateTime end = DateTime.Parse(endTime);

                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.share_flag == 1 && t.approval_status == "3"
                            && t.t_create <= end
                            && (string.IsNullOrEmpty(keyWord) ? true : t.paper_title.Contains(keyWord))
                            select t;
                }
                else
                {
                    query = from t in db.t_test_papers
                            where t.delete_flag == 0 && t.share_flag == 1 && t.approval_status == "3"
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
                        Complexity = item.complexity,
                        PaperConfidential = item.paper_confidential,
                        QuestionCount = (int)item.question_count,
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

        public object ReuseTestPaper(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, long paperid, TokenModel obj)
        {
            try
            {
                t_test_papers paper = new t_test_papers();
                //查找试卷信息
                t_test_papers paperInfo = db.t_test_papers.Find(paperid);
                paper.paper_confidential = paperInfo.paper_confidential;
                paper.paper_title = paperInfo.paper_title + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                paper.question_count = paperInfo.question_count;
                paper.exam_score = paperInfo.exam_score;
                paper.complexity = paperInfo.complexity;
                paper.create_number = obj.userNumber;
                paper.create_name = obj.userName;
                paper.approval_status = "1";
                db.t_test_papers.Add(paper);
                db.SaveChanges();

                List<QuestionTypeScore> questionTypeScores = new List<QuestionTypeScore>();
                var queryQuestionList = (from q in db.t_questions_bus
                                         where q.delete_flag == 0 && q.test_paper_id == paperid
                                         select q).ToList();
                foreach (var item in queryQuestionList)
                {
                    questionTypeScores.Add(new QuestionTypeScore
                    {
                        ID = (long)item.src_id,
                        Score = item.question_score
                    });
                }

                List<Question> questionList = GetQuestionsById(db, questionTypeScores);
                AddQuestions(db, questionList, paper.id, obj);

                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 2;
                log.logDesc = "重复使用了试卷：" + paperInfo.paper_title;
                log.logSuccessd = 1;
                log.moduleName = "试卷管理";
                rabbit.LogMsg(log);
                return new { code = 200, result = paper.id, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        #endregion

        #region 智能组卷

        public object GetQuestionTypeCount(pf_exam_paper_questionsContext db, IntelligentParameter queryParameter)
        {
            try
            {
                if (queryParameter.TagIDList == null || queryParameter.TagIDList.Count == 0)
                {
                    var count = (from q in db.t_questions
                                 where q.question_type == queryParameter.QuestionType
                                       && q.delete_flag == 0
                                       && q.publish_flag == "1"
                                 select q).Count();
                    return new { code = 200, result = count, message = "OK" };
                }
                else
                {
                    var count = (from q in db.t_questions
                                 join r in db.t_question_knowledge_ref on q.id equals r.question_id
                                 join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                 where q.question_type == queryParameter.QuestionType
                                       && q.delete_flag == 0
                                       && q.publish_flag == "1"
                                       && queryParameter.TagIDList.Contains((long)t.src_id)
                                 select q).Count();
                    return new { code = 200, result = count, message = "OK" };
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object GetAllQuestionTypeCount(pf_exam_paper_questionsContext db, IntelligentParameter queryParameter)
        {
            try
            {
                List<QuestionTypeCount> list = new List<QuestionTypeCount>();
                if (queryParameter.TagIDList == null || queryParameter.TagIDList.Count == 0)
                {
                    var queryQuestionTypeCount = db.t_questions.Where(x => x.delete_flag == 0 && x.publish_flag == "1" && x.approval_status == "3")
                                               .GroupBy(x => new { x.question_type })
                                               .Select(x => new { QuestionType = x.Key.question_type, count = x.Count() }).ToList();
                    foreach (var item in queryQuestionTypeCount)
                    {
                        list.Add(new QuestionTypeCount
                        {
                            QuestionType = item.QuestionType,
                            QuestionCount = item.count
                        });
                    }
                }
                else
                {
                    var queryQuestionTypeCount = (from q in db.t_questions
                                                  join r in db.t_question_knowledge_ref on q.id equals r.question_id
                                                  join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                                  where q.delete_flag == 0
                                                        && q.publish_flag == "1" && q.approval_status == "3"
                                                        && queryParameter.TagIDList.Contains((long)t.src_id)
                                                  select q).GroupBy(x => new { x.question_type }).Select(x => new { QuestionType = x.Key.question_type, count = x.Count() }).ToList();
                    foreach (var item in queryQuestionTypeCount)
                    {
                        list.Add(new QuestionTypeCount
                        {
                            QuestionType = item.QuestionType,
                            QuestionCount = item.count
                        });
                    }
                }
                if (list.Find(x => x.QuestionType == "1") == null)
                    list.Add(new QuestionTypeCount { QuestionType = "1", QuestionCount = 0 });
                if (list.Find(x => x.QuestionType == "2") == null)
                    list.Add(new QuestionTypeCount { QuestionType = "2", QuestionCount = 0 });
                if (list.Find(x => x.QuestionType == "3") == null)
                    list.Add(new QuestionTypeCount { QuestionType = "3", QuestionCount = 0 });
                if (list.Find(x => x.QuestionType == "4") == null)
                    list.Add(new QuestionTypeCount { QuestionType = "4", QuestionCount = 0 });
                if (list.Find(x => x.QuestionType == "5") == null)
                    list.Add(new QuestionTypeCount { QuestionType = "5", QuestionCount = 0 });

                return new { code = 200, result = list, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object IntelligentComposeTestPaper(pf_exam_paper_questionsContext db, RabbitMQClient rabbit, IntelligentParameter parameter, TokenModel obj)
        {
            try
            {
                //筛选完成的试题
                List<QuestionTypeScore> resultQuestions = new List<QuestionTypeScore>();
                //全部试题
                List<Question> allQuestion = GetAllQuestions(db, parameter);
                int sumQuestionCount = 0;
                //计算试题总数
                for (int i = 0; i < parameter.questionTypeCount.Count; i++)
                {
                    sumQuestionCount += parameter.questionTypeCount[i].QuestionCount;
                }

                //读取难易度配置
                int difficult = 0;//难
                int general = 0;//中
                int easy = 0;//易
                var queryConfigComplexity = db.t_config_paper_complexity.Where(x => x.paper_complexity == parameter.PaperComplexity && x.delete_flag == 0).SingleOrDefault();
                if (queryConfigComplexity != null)
                {
                    difficult = (int)queryConfigComplexity.difficulty;
                    general = (int)queryConfigComplexity.general;
                    easy = (int)queryConfigComplexity.easy;
                }
                //难的试题数量
                difficult = difficult * sumQuestionCount / 100;
                //中的试题数量
                general = general * sumQuestionCount / 100;
                //易的试题数量
                easy = easy * sumQuestionCount / 100;

                //困难的试题集合
                List<Question> difficultQuestions = GetRandomQuestions(allQuestion.FindAll(x => x.Complexity == "1"), difficult);
                //中等的试题集合
                List<Question> generalQuestions = GetRandomQuestions(allQuestion.FindAll(x => x.Complexity == "3"), general);
                //容易的试题集合
                List<Question> easyQuestions = GetRandomQuestions(allQuestion.FindAll(x => x.Complexity == "5"), easy);

                //添加到最终的结果集合中
                for (int i = 0; i < difficultQuestions.Count; i++)
                {
                    resultQuestions.Add(new QuestionTypeScore
                    {
                        ID = difficultQuestions[i].ID,
                        Score = 0,
                        Complexity = difficultQuestions[i].Complexity,
                        QuestionType = difficultQuestions[i].QuestionType,
                        QuestionSort = 0
                    });
                }
                for (int i = 0; i < generalQuestions.Count; i++)
                {
                    resultQuestions.Add(new QuestionTypeScore
                    {
                        ID = generalQuestions[i].ID,
                        Score = 0,
                        Complexity = generalQuestions[i].Complexity,
                        QuestionType = generalQuestions[i].QuestionType,
                        QuestionSort = 0
                    });
                }
                for (int i = 0; i < easyQuestions.Count; i++)
                {
                    resultQuestions.Add(new QuestionTypeScore
                    {
                        ID = easyQuestions[i].ID,
                        Score = 0,
                        Complexity = easyQuestions[i].Complexity,
                        QuestionType = easyQuestions[i].QuestionType,
                        QuestionSort = 0
                    });
                }
                //记录循环过的题型
                List<string> recordQuestionType = new List<string>();
                for (int i = 0; i < parameter.questionTypeCount.Count; i++)
                {
                    //题型
                    string questionType = parameter.questionTypeCount[i].QuestionType;
                    recordQuestionType.Add(questionType);

                    //前端要求的数量
                    int count = parameter.questionTypeCount[i].QuestionCount;
                    //最终结果中存在的题型数量
                    int questioncount = resultQuestions.FindAll(x => x.QuestionType == questionType).Count;
                    if (questioncount < count)//比前端要求的少
                    {
                        for (int j = 0; j < count - questioncount; j++)
                        {
                            var notThisTypeList = (from n in resultQuestions
                                                   where !recordQuestionType.Contains(n.QuestionType)
                                                   select n).ToList();
                            if (notThisTypeList.Count == 0)//不存在，则结束本次循环
                                continue;
                            var notThisType = RandomRemoveQuestion(notThisTypeList);
                            //记住难易度
                            var easyDegree = notThisType.Complexity;
                            //删除一个  
                            resultQuestions.Remove(notThisType);
                            //添加一个难易度、题型相符的试题
                            var addObjectList = allQuestion.FindAll(x => x.QuestionType == questionType && x.Complexity == easyDegree);
                            if (addObjectList.Count == 0)
                                continue;
                            //随机抽取试题
                            var addobject = RandomRemoveQuestion(addObjectList);
                            if (resultQuestions.Find(x => x.ID == addobject.ID) != null)//最终结果中存在了，不添加，不计入当前循环次数
                                j--;
                            else//最终结果不存在，则添加
                            {
                                resultQuestions.Add(new QuestionTypeScore
                                {
                                    ID = addobject.ID,
                                    Score = 0,
                                    Complexity = addobject.Complexity,
                                    QuestionSort = 0,
                                    QuestionType = addobject.QuestionType
                                });
                            }
                        }
                    }
                    else if (questioncount > count)//比前端要求的多
                    {
                        for (int j = 0; j < questioncount - count; j++)
                        {
                            //筛选出题型符合的试题
                            var typeList = resultQuestions.FindAll(x => x.QuestionType == questionType);
                            //随机获取将要被删除的试题
                            var removeObject = RandomRemoveQuestion(typeList);
                            //记住难易度
                            var easyDegree = removeObject.Complexity;
                            //删除一个
                            resultQuestions.Remove(removeObject);
                            //添加一个难易度相同，题型不同的试题
                            var addObjectList = (from n in allQuestion
                                                 where !recordQuestionType.Contains(n.QuestionType) && n.Complexity == easyDegree
                                                 select n).ToList();
                            if (addObjectList.Count == 0)
                                continue;
                            var addObject = RandomRemoveQuestion(addObjectList);
                            if (resultQuestions.Find(x => x.ID == addObject.ID) != null)//最终结果中存在了，不添加，不计入当前循环次数
                                j--;
                            else//最终结果不存在，则添加
                            {
                                resultQuestions.Add(new QuestionTypeScore
                                {
                                    ID = addObject.ID,
                                    Score = 0,
                                    Complexity = addObject.Complexity,
                                    QuestionSort = 0,
                                    QuestionType = addObject.QuestionType
                                });
                            }
                        }
                    }
                }
                for (int i = 0; i < parameter.questionTypeCount.Count; i++)
                {
                    //前端要求的题型
                    string questionType = parameter.questionTypeCount[i].QuestionType;
                    //前端要求的数量
                    int questionCount = parameter.questionTypeCount[i].QuestionCount;

                    //最终结果的题型数量
                    var resultTypeCount = resultQuestions.FindAll(x => x.QuestionType == questionType).Count;
                    if (questionCount > resultTypeCount)
                    {
                        //差值
                        int dif = questionCount - resultTypeCount;
                        var resultListId = (from r in resultQuestions
                                            select r.ID).ToList();
                        var notContainList = (from a in allQuestion
                                              where !resultListId.Contains(a.ID) && a.QuestionType == questionType
                                              select a).ToList();
                        var randomQuestionList = GetRandomQuestions(notContainList, dif);
                        for (int k = 0; k < randomQuestionList.Count; k++)
                        {
                            resultQuestions.Add(new QuestionTypeScore
                            {
                                ID = randomQuestionList[k].ID,
                                Complexity = randomQuestionList[k].Complexity,
                                QuestionType = questionType,
                                Score = 0,
                                QuestionSort = 0
                            });
                        }
                    }
                    else if (questionCount < resultTypeCount)//不存在这种情况
                    {
                        int dif = questionCount - resultTypeCount;
                    }
                }
                int nSort = 0;
                var tempList = resultQuestions.OrderBy(x => x.ID).ToList();
                List<QuestionTypeScore> resultList = new List<QuestionTypeScore>();
                for (int i = 0; i < tempList.Count; i++)
                {
                    resultList.Add(new QuestionTypeScore
                    {
                        ID = tempList[i].ID,
                        Score = 0,
                        Complexity = tempList[i].Complexity,
                        QuestionSort = ++nSort,
                        QuestionType = tempList[i].QuestionType
                    });
                }

                t_test_papers paper = new t_test_papers();
                paper.paper_confidential = "3";
                paper.paper_title = obj.userName + "的智能组卷" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                paper.question_count = sumQuestionCount;
                paper.exam_score = 0;
                paper.complexity = parameter.PaperComplexity;
                paper.create_number = obj.userNumber;
                paper.create_name = obj.userName;
                db.t_test_papers.Add(paper);
                db.SaveChanges();

                List<Question> questionList = GetQuestionsById(db, resultList);
                AddQuestions(db, questionList, paper.id, obj);
                //日志消息产生
                SysLogModel log = new SysLogModel();
                log.opNo = obj.userNumber;
                log.opName = obj.userName;
                log.opType = 2;
                log.logDesc = "生成了智能组卷：" + paper.paper_title;
                log.logSuccessd = 1;
                log.moduleName = "试卷管理";
                rabbit.LogMsg(log);
                return new { code = 200, result = paper.id, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        /// <summary>
        /// 从指定的题型集合中随机去除一道试题
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private QuestionTypeScore RandomRemoveQuestion(List<QuestionTypeScore> questions)
        {
            Random rand = new Random();
            int index = rand.Next(questions.Count);
            var t = questions[index];
            return t;
        }
        /// <summary>
        /// 从指定的题型集合中随机去除一道试题
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private Question RandomRemoveQuestion(List<Question> questions)
        {
            Random rand = new Random();
            int index = rand.Next(questions.Count);
            var t = questions[index];
            return t;
        }
        /// <summary>
        /// 随机抽取试题
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="count">指定的数量</param>
        /// <returns></returns>
        private List<Question> GetRandomQuestions(List<Question> questions, int count)
        {
            if (questions.Count <= count)
                return questions;
            Random rand = new Random();
            List<Question> list = new List<Question>();
            for (int i = 0; list.Count < count; i++)
            {
                int index = rand.Next(questions.Count);
                var t = questions[index];
                if (!list.Contains(t))
                    list.Add(questions[index]);
            }
            return list;
        }

        /// <summary>
        /// 获取全部试题
        /// </summary>
        /// <param name="db"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private List<Question> GetAllQuestions(pf_exam_paper_questionsContext db, IntelligentParameter parameter)
        {
            List<Question> allQuestion = new List<Question>();
            if (parameter.TagIDList != null && parameter.TagIDList.Count > 0)//选择了知识点
            {
                //查找出符合知识点的试题
                var queryQuestionIDList = (from q in db.t_questions
                                           join r in db.t_question_knowledge_ref on q.id equals r.question_id
                                           join t in db.t_knowledge_tag on r.knowledge_tag_id equals t.id
                                           where q.delete_flag == 0
                                                 && parameter.TagIDList.Contains((long)t.src_id)
                                                 && q.publish_flag == "1" && q.approval_status == "3"
                                           select new { q.id, q.question_type, q.complexity }).ToList();
                //知识点符合的试题                      
                foreach (var item in queryQuestionIDList)
                {
                    allQuestion.Add(new Question
                    {
                        ID = item.id,
                        QuestionType = item.question_type,
                        Complexity = item.complexity
                    });
                }
            }
            else if (parameter.TagIDList == null || parameter.TagIDList.Count == 0)
            {
                var queryQuestionIDList = db.t_questions.Where(x => x.delete_flag == 0 && x.publish_flag == "1" && x.approval_status == "3").ToList();
                foreach (var item in queryQuestionIDList)
                {
                    allQuestion.Add(new Question
                    {
                        ID = item.id,
                        QuestionType = item.question_type,
                        Complexity = item.complexity
                    });
                }
            }
            return allQuestion;
        }

        #endregion
    }

    public class IntelligentParameter
    {
        public List<long> TagIDList { get; set; }
        public List<QuestionTypeCount> questionTypeCount { get; set; }
        public string PaperComplexity { get; set; }
        public string QuestionType { get; set; }

    }
    public class QuestionTypeCount
    {
        public string QuestionType { get; set; }
        public int QuestionCount { get; set; }
    }

}
