using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Questionnaire.BLL
{
    public class QuestionnaireBLL
    {
        /// <summary>
        /// 获取问卷信息
        /// </summary>
        /// <param name="CourseID"></param>
        /// <param name="strQuestionTheme"></param>
        /// <returns></returns>
        public object GetQuestionnaire(pf_questionnaireContext db, long PlanID, long CourseID, string strQuestionTheme)
        {
            try
            {
                List<QuestionnaireAttend> questionnaireAttend = new List<QuestionnaireAttend>();
                var p = from q in db.t_questionnaire
                        where q.course_id == CourseID
                              && q.delete_flag == 0
                              && q.plan_id == PlanID
                              && (string.IsNullOrEmpty(strQuestionTheme) ? true : q.theme.Contains(strQuestionTheme))
                        select q;
                var TempList = p.ToList();
                if (TempList.Count > 0)
                {
                    for (int i = 0; i < TempList.Count; i++)
                    {
                        var query1 = from q1 in db.t_interaction_log
                                     where q1.questionnaire_id == TempList[i].id
                                     group q1 by q1.participate_id into h
                                     select new { Rec = h.Count() };
                        int n = query1.Count();
                        questionnaireAttend.Add(new QuestionnaireAttend
                        {
                            ID = TempList[i].id,
                            Theme = TempList[i].theme,
                            StartTime = TempList[i].start_time,
                            EndTime = TempList[i].expiry_time,
                            Num = n,
                            CurrentStatus = TempList[i].current_status
                        });
                    }
                    return new { code = 200, result = questionnaireAttend, msg = "OK" };
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

        public object Update_QuestionnaireName(pf_questionnaireContext db,RabbitMQClient rabbit, long ID, string Name, TokenModel token)
        {
            try
            {
                var query = from q in db.t_questionnaire
                            where q.delete_flag == 0 && q.id == ID
                            select q;
                var p = query.FirstOrDefault();
                p.theme = Name;
                db.SaveChanges();

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "将问卷名称修改为：" + Name;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        /// <summary>
        /// 问卷预览
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public object PreviewQuestionnaire(pf_questionnaireContext db, long QuestionnaireID)
        {
            try
            {
                //选项集合
                List<OptionContent> ContentList = new List<OptionContent>();
                //题干集合
                List<QuestionItem> QuestionList = new List<QuestionItem>();
                //一套问卷
                Questionnaire questionnaire = new Questionnaire();

                var OptionContent = from q in db.t_questionnaire
                                    join i in db.t_questionnaire_item on q.id equals i.questionnnaire_id
                                    join p in db.t_item_option on i.id equals p.questionnaire_item_id into ii
                                    from _ii in ii.DefaultIfEmpty()
                                    where q.id == QuestionnaireID && _ii.delete_flag == 0 && i.delete_flag == 0
                                    select new
                                    {
                                        id = _ii == null ? 0 : _ii.id,
                                        item_id = _ii == null ? 0 : _ii.questionnaire_item_id,
                                        option_num = _ii == null ? "null" : _ii.option_number,
                                        content = _ii == null ? "null" : _ii.option_content
                                    };
                var OptionContentListTemp = OptionContent.ToList();
                for (int i = 0; i < OptionContentListTemp.Count; i++)
                {
                    ContentList.Add(new OptionContent
                    {
                        ID = OptionContentListTemp[i].id,
                        ItemID = OptionContentListTemp[i].item_id,
                        OptionNum = OptionContentListTemp[i].option_num,
                        Content = OptionContentListTemp[i].content
                    });
                }

                var Question = from q in db.t_questionnaire
                               join i in db.t_questionnaire_item on q.id equals i.questionnnaire_id into ii
                               from _ii in ii.DefaultIfEmpty()
                               where q.id == QuestionnaireID && _ii.delete_flag == 0
                               orderby _ii.item_sort ascending
                               select new
                               {
                                   id = _ii == null ? 0 : _ii.id,
                                   questionnaire_id = _ii == null ? 0 : _ii.questionnnaire_id,
                                   title = _ii == null ? "null" : _ii.item_title,
                                   itemtype = _ii == null ? "null" : _ii.item_type,
                                   Sort = _ii.item_sort,
                                   _ii.must_answer_flag,
                                   _ii.min_answer_num,
                                   _ii.max_answer_num
                               };
                var QuestionListTemp = Question.ToList();
                int j = 0;
                for (int i = 0; i < QuestionListTemp.Count; i++)
                {
                    long qid = QuestionListTemp[i].id;
                    var c = ContentList.FindAll(t => t.ItemID == qid);

                    QuestionList.Add(new QuestionItem
                    {
                        ID = QuestionListTemp[i].id,
                        QuestionID = qid,
                        Item = QuestionListTemp[i].title,
                        ItemType = QuestionListTemp[i].itemtype,
                        OptionList = c,
                        Sort = ++j,
                        MustAnswer = QuestionListTemp[i].must_answer_flag,
                        MinAnswerNum = QuestionListTemp[i].min_answer_num,
                        MaxAnswerNum = QuestionListTemp[i].max_answer_num
                    });
                }
                var query = from q in db.t_questionnaire
                            where q.id == QuestionnaireID
                            select new
                            {
                                ID = q.id,
                                Theme = q.theme,
                                StartTime = q.start_time,
                                EndTime = q.expiry_time
                            };
                var o = query.FirstOrDefault();
                if (o != null)
                {
                    questionnaire.ID = o.ID;
                    questionnaire.Theme = o.Theme;
                    questionnaire.StartTime = o.StartTime;
                    questionnaire.EndTime = o.EndTime;
                    questionnaire.ItemList = QuestionList;
                    return new { code = 200, result = questionnaire, msg = "OK" };
                }
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
        /// 查看问卷结果
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public object GetQuestionnaireResult(pf_questionnaireContext db, long QuestionnaireID)
        {
            try
            {
                List<OptionContent> ContentList = new List<OptionContent>();
                List<QuestionItem> QuestionList = new List<QuestionItem>();
                Questionnaire questionnaire = new Questionnaire();

                //查找选项
                var OptionContent = from q in db.t_questionnaire
                                    join i in db.t_questionnaire_item on q.id equals i.questionnnaire_id
                                    join p in db.t_item_option on i.id equals p.questionnaire_item_id into ii
                                    from _ii in ii.DefaultIfEmpty()
                                    where q.id == QuestionnaireID && _ii.delete_flag == 0 && i.delete_flag == 0
                                    select new
                                    {
                                        id = _ii == null ? 0 : _ii.id,
                                        item_id = _ii == null ? 0 : _ii.questionnaire_item_id,
                                        option_num = _ii == null ? "null" : _ii.option_number,
                                        content = _ii == null ? "null" : _ii.option_content
                                    };
                var OptionContentListTemp = OptionContent.ToList();
                for (int i = 0; i < OptionContentListTemp.Count; i++)
                {
                    //获取选项参与人数
                    var OptionCount = (from g in db.t_interaction_log
                                       where g.questionnaire_id == QuestionnaireID
                                       && g.questionnaire_item_id == OptionContentListTemp[i].item_id
                                       && g.item_option_id == OptionContentListTemp[i].id
                                       select g.id).ToList().Count;
                    ContentList.Add(new OptionContent
                    {
                        ID = OptionContentListTemp[i].id,//ID
                        ItemID = OptionContentListTemp[i].item_id,//题干ID 
                        OptionNum = OptionContentListTemp[i].option_num,
                        Content = OptionContentListTemp[i].content,//选项内容
                        AnswerCount = OptionCount
                    });
                }

                //查找问卷题干
                var Question = from q in db.t_questionnaire
                               join i in db.t_questionnaire_item on q.id equals i.questionnnaire_id into ii
                               from _ii in ii.DefaultIfEmpty()
                               where q.id == QuestionnaireID && _ii.delete_flag == 0
                               orderby _ii.item_sort ascending
                               select new
                               {
                                   id = _ii == null ? 0 : _ii.id,
                                   questionnaire_id = _ii == null ? 0 : _ii.questionnnaire_id,
                                   title = _ii == null ? "null" : _ii.item_title,
                                   itemtype = _ii == null ? "null" : _ii.item_type,
                                   Sort = _ii.item_sort,
                                   _ii.must_answer_flag,
                                   _ii.min_answer_num,
                                   _ii.max_answer_num
                               };
                var QuestionListTemp = Question.ToList();
                for (int i = 0; i < QuestionListTemp.Count; i++)
                {
                    long qid = QuestionListTemp[i].id;
                    //查找题干下的所有选项
                    var c = ContentList.FindAll(t => t.ItemID == qid);
                    //如果为简答题，查找学生的作答结果
                    List<string> answerList = new List<string>();
                    if (QuestionListTemp[i].itemtype == "3")
                    {
                        var queryAnswerResult = from a in db.t_interaction_log
                                                where a.questionnaire_id == QuestionnaireID
                                                && a.questionnaire_item_id == QuestionListTemp[i].id
                                                select new { a.participate_name, a.interaction_result };
                        foreach (var item in queryAnswerResult)
                        {
                            answerList.Add(item.participate_name + ":" + item.interaction_result);
                        }
                    }
                    //将选项添加到题干下
                    QuestionList.Add(new QuestionItem
                    {
                        ID = QuestionListTemp[i].id,
                        //QuestionID = qid,
                        Item = QuestionListTemp[i].title,
                        ItemType = QuestionListTemp[i].itemtype,
                        OptionList = c,
                        Sort = QuestionListTemp[i].Sort,
                        MustAnswer = QuestionListTemp[i].must_answer_flag,
                        MinAnswerNum = QuestionListTemp[i].min_answer_num,
                        MaxAnswerNum = QuestionListTemp[i].max_answer_num,
                        AnswerResult = answerList
                    });
                }

                //查找问卷的参与人数
                var Num = (from t in db.t_interaction_log
                           where t.questionnaire_id == QuestionnaireID
                           select t.participate_id).Distinct().Count();

                //查找问卷
                var query = from q in db.t_questionnaire
                            where q.id == QuestionnaireID
                            select new
                            {
                                ID = q.id,
                                Theme = q.theme,
                                StartTime = q.start_time,
                                EndTime = q.expiry_time
                            };
                var o = query.FirstOrDefault();
                if (o != null)
                {
                    questionnaire.ID = o.ID;
                    questionnaire.Theme = o.Theme;
                    questionnaire.StartTime = o.StartTime;
                    questionnaire.EndTime = o.EndTime;
                    questionnaire.Num = Num;
                    questionnaire.ItemList = QuestionList;
                    return new { code = 200, result = questionnaire, msg = "OK" };
                }
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
        /// 创建问卷
        /// </summary>
        /// <param name="qt"></param>
        /// <returns></returns>
        public object CreateQuestionnaire(pf_questionnaireContext db,RabbitMQClient rabbit, QuestionnaireInfo qt, TokenModel token)
        {
            try
            {
                t_questionnaire q = new t_questionnaire();
                q.plan_id = qt.PlanID;
                q.course_id = qt.CourseID; ;
                q.theme = qt.Theme;
                q.delete_flag = 0;
                q.current_status = "1";
                q.create_by = qt.CreateBy;
                q.create_time = DateTime.Now;
                q.update_by = qt.CreateBy;
                q.update_time = DateTime.Now;
                db.t_questionnaire.Add(q);
                db.SaveChanges();

                long maxid = q.id;
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.logDesc = "创建了问卷：" + qt.Theme;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
                rabbit.LogMsg(syslog);
                return new { code = 200, result = maxid, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 创建题目
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <param name="TitleInfo"></param>
        /// <param name="ItemType"></param>
        /// <param name="CreateBy"></param>
        /// <returns></returns>
        public object CreateQuestionnaireItem(pf_questionnaireContext db,RabbitMQClient rabbit ,long QuestionnaireID, string TitleInfo, string ItemType, long CreateBy, TokenModel token)
        {
            try
            {
                int? p = (from i in db.t_questionnaire_item
                          where i.questionnnaire_id == QuestionnaireID
                          select i.item_sort).Max();
                if (p == null)
                    p = 0;
                t_questionnaire_item item = new t_questionnaire_item();
                item.questionnnaire_id = QuestionnaireID;
                item.item_title = TitleInfo;
                item.item_type = ItemType;
                item.item_sort = ++p;
                item.create_by = CreateBy;
                item.create_time = DateTime.Now;
                item.delete_flag = 0;
                item.update_by = CreateBy;
                item.update_time = DateTime.Now;
                db.t_questionnaire_item.Add(item);
                db.SaveChanges();
                long id = item.id;

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.logDesc = "创建了题干：" + TitleInfo;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
                rabbit.LogMsg(syslog);

                return new { code = 200, result = id, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 修改题目
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="TitleInfo"></param>
        /// <param name="UpdateBy"></param>
        /// <returns></returns>
        public object UpdateQuestionnaireItem(pf_questionnaireContext db,RabbitMQClient rabbit, long ID, string TitleInfo, long UpdateBy, TokenModel token)
        {
            try
            {
                var query = from q in db.t_questionnaire_item
                            where q.id == ID
                            select q;
                var p = query.FirstOrDefault();
                p.item_title = TitleInfo;
                p.update_by = UpdateBy;
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "修改了题干：" + TitleInfo;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        /// <summary>
        /// 删除题目
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object RemoveQuestionnaireItem(pf_questionnaireContext db,RabbitMQClient rabbit, long ID, TokenModel token)
        {
            try
            {
                var query = from q in db.t_questionnaire_item
                            where q.id == ID
                            select q;
                var p = query.FirstOrDefault();
                p.delete_flag = 1;
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 4;
                syslog.logDesc = "删除了题干：" + p.item_title;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        public object SetQuestionIsMustAnswer(pf_questionnaireContext db,RabbitMQClient rabbit, long id, sbyte? result, TokenModel token)
        {
            try
            {
                var query = from s in db.t_questionnaire_item
                            where s.delete_flag == 0 && s.id == id
                            select s;
                var q = query.FirstOrDefault();
                q.must_answer_flag = result;
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "设置了题目:" + q.item_title + ",是否必答属性";
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        public object SetAnswerQuestionCountMin(pf_questionnaireContext db,RabbitMQClient rabbit, long id, int? minCount, TokenModel token)
        {
            try
            {
                var query = from s in db.t_questionnaire_item
                            where s.delete_flag == 0 && s.id == id
                            select s;
                var q = query.FirstOrDefault();
                q.min_answer_num = minCount;
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "设置了题干：" + q.item_title + ",至少作答属性";
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        public object SetAnswerQuestionCountMax(pf_questionnaireContext db,RabbitMQClient rabbit, long id, int? maxCount, TokenModel token)
        {
            try
            {
                var query = from s in db.t_questionnaire_item
                            where s.delete_flag == 0 && s.id == id
                            select s;
                var q = query.FirstOrDefault();
                q.max_answer_num = maxCount;
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "设置了题干：" + q.item_title + ",至少作答属性";
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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
        public object QuestionItemSort(pf_questionnaireContext db, List<QuestionSort> questionSort)
        {
            try
            {
                if (questionSort.Count > 0)
                {
                    for (int i = 0; i < questionSort.Count; i++)
                    {
                        var query = from q in db.t_questionnaire_item
                                    where q.id == questionSort[i].ID && q.delete_flag == 0
                                    select q;
                        var qq = query.FirstOrDefault();
                        qq.item_sort = questionSort[i].Sort;
                    }
                    db.SaveChanges();
                    return new { code = 200, msg = "OK" };
                }
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
        /// 创建题目选项
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="OptionContent"></param>
        /// <param name="CreateBy"></param>
        /// <returns></returns>
        public object CreateQuestionnaireItemOption(pf_questionnaireContext db,RabbitMQClient rabbit, long ItemID, string OptionNum, string OptionContent, long CreateBy, TokenModel token)
        {
            try
            {

                t_item_option option = new t_item_option();
                option.questionnaire_item_id = ItemID;
                option.option_number = OptionNum;
                option.option_content = OptionContent;
                option.delete_flag = 0;
                option.createby = CreateBy;
                option.create_time = DateTime.Now;
                option.update_by = CreateBy;
                option.update_time = DateTime.Now;
                db.t_item_option.Add(option);
                db.SaveChanges();
                long id = option.id;

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.logDesc = "创建了题目选项:" + OptionContent;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
                rabbit.LogMsg(syslog);
                return new { code = 200, result = id, msg = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public async Task<object> UpdateItemOption(pf_questionnaireContext db, List<OptionContent> optionContents)
        {
            try
            {
                if (optionContents != null && optionContents.Count > 0)
                {
                    for (int i = 0; i < optionContents.Count; i++)
                    {
                        var queryOption = db.t_item_option.Where(x => x.id == optionContents[i].ID).FirstOrDefault();
                        queryOption.option_number = optionContents[i].OptionNum;
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
        /// <summary>
        /// 修改题目选项
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="OptionContent"></param>
        /// <param name="UpdateBy"></param>
        /// <returns></returns>
        public object UpdateQuestionnaireItemOption(pf_questionnaireContext db,RabbitMQClient rabbit, long ID, string OptionContent, long UpdateBy, TokenModel token)
        {
            try
            {
                var query = from i in db.t_item_option
                            where i.id == ID
                            select i;
                var q = query.FirstOrDefault();
                q.option_content = OptionContent;
                q.update_by = UpdateBy;
                q.update_time = DateTime.Now;
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "题目选项内容修改为:" + OptionContent;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        /// <summary>
        /// 删除题目选项
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object RemoveQuestionnaireItemOption(pf_questionnaireContext db,RabbitMQClient rabbit, long ID, TokenModel token)
        {
            try
            {
                var query = from i in db.t_item_option
                            where i.id == ID
                            select i;
                var q = query.FirstOrDefault();
                q.delete_flag = 1;
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 4;
                syslog.logDesc = "删除了题目选项：" + q.option_content;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        /// <summary>
        /// 问卷发布
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public object PublishQuestionnaire(pf_questionnaireContext db,RabbitMQClient rabbit, long QuestionnaireID, TokenModel token)
        {
            try
            {
                var qq = from s in db.t_questionnaire_item
                         where s.questionnnaire_id == QuestionnaireID && s.delete_flag == 0
                         select s;
                if (qq.ToList().Count == 0)
                {
                    return new { code = 400, message = "问卷下没有题目信息，不可以发布哦！" };
                }
                else
                {
                    var queryItem = from s in db.t_questionnaire_item
                                    where s.questionnnaire_id == QuestionnaireID && s.delete_flag == 0 && s.item_type != "3"
                                    select s;
                    foreach (var item in queryItem)
                    {
                        var queryOption = from o in db.t_item_option
                                          where o.questionnaire_item_id == item.id && o.delete_flag == 0
                                          select o;
                        if (queryOption.ToList().Count == 0)
                        {
                            return new { code = 400, message = "问卷下的选择题存在没有选项的，不可以发布哦！" };
                        }
                    }
                }

                var query = from q in db.t_questionnaire
                            where q.delete_flag == 0 && q.id == QuestionnaireID
                            select q;
                var obj = query.FirstOrDefault();
                obj.start_time = DateTime.Now;
                obj.current_status = "2";
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "发布了问卷:" + obj.theme;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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

        /// <summary>
        /// 终止问卷
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public object QuitQuestionnaire(pf_questionnaireContext db,RabbitMQClient rabbit, long QuestionnaireID, TokenModel token)
        {
            try
            {
                var query = from q in db.t_questionnaire
                            where q.delete_flag == 0 && q.id == QuestionnaireID
                            select q;
                var obj = query.FirstOrDefault();
                obj.expiry_time = DateTime.Now;
                obj.current_status = "3";
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "中止了问卷:" + obj.theme;
                syslog.logSuccessd = 1;
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

        public object QuitQuestionByPlanID(pf_questionnaireContext db,RabbitMQClient rabbit, Plan PlanID, TokenModel token)
        {
            try
            {
                var query = from q in db.t_questionnaire
                            where q.delete_flag == 0 && q.plan_id == PlanID.PlanID && q.current_status != "3"
                            select q;
                foreach (var item in query)
                {
                    item.expiry_time = DateTime.Now;
                    item.current_status = "3";
                }
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.logDesc = "中止了问卷:" + query.FirstOrDefault().theme;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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
        public object QuitQuestionByRemote(pf_questionnaireContext db,RabbitMQClient rabbit, Plan PlanID)
        {
            try
            {
                var query = from q in db.t_questionnaire
                            where q.delete_flag == 0 && q.plan_id == PlanID.PlanID && q.current_status != "3"
                            select q;
                foreach (var item in query)
                {
                    item.expiry_time = DateTime.Now;
                    item.current_status = "3";
                }
                db.SaveChanges();
                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = PlanID.UserNumber;
                syslog.opName = PlanID.UserName;
                syslog.opType = 3;
                syslog.logDesc = "中止了问卷:" + query.FirstOrDefault().theme;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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
        /// <summary>
        /// 删除问卷
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public object DeleteQuestionnaire(pf_questionnaireContext db,RabbitMQClient rabbit, long QuestionnaireID, TokenModel token)
        {
            try
            {
                var query = from q in db.t_questionnaire
                            where q.delete_flag == 0 && q.id == QuestionnaireID
                            select q;
                var obj = query.FirstOrDefault();
                obj.delete_flag = 1;
                db.SaveChanges();

                //产生日志消息
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 4;
                syslog.logDesc = "删除了问卷:" + obj.theme;
                syslog.logSuccessd = 1;
                syslog.moduleName = "教学互动";
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
    public class Plan
    {
        public long PlanID { get; set; }
        public string UserName { get; set; }
        public string UserNumber { get; set; }
    }
    public class QuestionSort
    {
        public long ID { get; set; }
        public int Sort { get; set; }
    }
    public class QuestionnaireAttend
    {
        public long ID { get; set; }
        public string Theme { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CurrentStatus { get; set; }
        public int Num { get; set; }
    }
    public class QuestionnaireLog
    {
        public string Theme { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<InteractiveResult> list { get; set; }
    }

    public class InteractiveResult
    {
        public string Title { get; set; }
        public string Result { get; set; }
        public int Num { get; set; }
    }

    /// <summary>
    /// 一套问卷
    /// </summary>
    public class Questionnaire
    {
        public long ID { get; set; }
        public string Theme { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Num { get; set; }
        public List<QuestionItem> ItemList { get; set; }
    }

    /// <summary>
    /// 一道试题
    /// </summary>
    public class QuestionItem
    {
        public long ID { get; set; }
        public long QuestionID { get; set; }
        public string Item { get; set; }
        public string ItemType { get; set; }
        public int? Sort { get; set; }
        public sbyte? MustAnswer { get; set; }
        public int? MinAnswerNum { get; set; }
        public int? MaxAnswerNum { get; set; }
        public List<long> AnswerListID { get; set; }
        public List<string> AnswerResult { get; set; }
        public List<OptionContent> OptionList { get; set; }
    }

    /// <summary>
    /// 选项
    /// </summary>
    public class OptionContent
    {
        public long ID { get; set; }
        public long ItemID { get; set; }
        public string OptionNum { get; set; }
        public string Content { get; set; }
        public int AnswerCount { get; set; }
    }

    /// <summary>
    /// 问卷
    /// </summary>
    public class QuestionnaireInfo
    {
        public long ID { get; set; }
        public long PlanID { get; set; }
        public long CourseID { get; set; }
        public string Theme { get; set; }
        public long CreateBy { get; set; }
    }

}
