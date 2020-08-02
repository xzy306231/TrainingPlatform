using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Questionnaire.BLL
{
    public class QuestionnaireInteract
    {
        public object GetQuestionnaireByCourseID(pf_questionnaireContext db, long PlanID, long CourseID, string strStatus, string keyWord, long UserID, int PageIndex, int PageSize)
        {
            try
            {
                if (strStatus == "2")//进行中
                {
                    List<QuestionnaireAttendStatus> questionnaireAttend = new List<QuestionnaireAttendStatus>();
                    var p = from q in db.t_questionnaire
                            where q.course_id == CourseID && q.delete_flag == 0 && q.plan_id == PlanID
                                  && q.current_status == strStatus
                                  && (string.IsNullOrEmpty(keyWord) ? true : q.theme.Contains(keyWord))
                            select q;
                    int count = p.Count();
                    var query = p.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
                    var TempList = query.ToList();
                    if (TempList.Count > 0)
                    {
                        for (int i = 0; i < TempList.Count; i++)
                        {
                            string AnswerFlag = "";
                            var queryExist = from g in db.t_interaction_log
                                             where g.questionnaire_id == TempList[i].id && g.participate_id == UserID
                                             select g;
                            var ExistCount = queryExist.Count();
                            if (ExistCount > 0)
                                AnswerFlag = "1";//已作答
                            else
                                AnswerFlag = "0";//未作答

                            var query1 = from q1 in db.t_interaction_log
                                         where q1.questionnaire_id == TempList[i].id
                                         group q1 by q1.participate_id into h
                                         select new { Rec = h.Count() };
                            int n = query1.Count();
                            questionnaireAttend.Add(new QuestionnaireAttendStatus
                            {
                                ID = TempList[i].id,
                                Theme = TempList[i].theme,
                                QuestionStatus = "进行中",
                                StartTime = TempList[i].start_time,
                                EndTime = TempList[i].expiry_time,
                                Num = n,
                                AnswerFlag = AnswerFlag
                            });
                        }
                        return new { code = 200, result = questionnaireAttend, count = count, message = "ok" };
                    }
                    else
                        return new { code = 200, message = "ok" }; ;


                }
                else if (strStatus == "3")//已结束
                {
                    List<QuestionnaireAttendStatus> questionnaireAttend = new List<QuestionnaireAttendStatus>();
                    var p = from q in db.t_questionnaire
                            where q.course_id == CourseID
                                  && q.delete_flag == 0
                                  && q.plan_id == PlanID
                                  && q.current_status == strStatus
                                  && (string.IsNullOrEmpty(keyWord) ? true : q.theme.Contains(keyWord))
                            select q;
                    int count = p.Count();
                    var query = p.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
                    var TempList = query.ToList();
                    if (TempList.Count > 0)
                    {
                        for (int i = 0; i < TempList.Count; i++)
                        {
                            string AnswerFlag = "";
                            var queryExist = from g in db.t_interaction_log
                                             where g.questionnaire_id == TempList[i].id && g.participate_id == UserID
                                             select g;
                            var ExistCount = queryExist.Count();
                            if (ExistCount > 0)
                                AnswerFlag = "1";//已作答
                            else
                                AnswerFlag = "0";//未作答

                            var query1 = from q1 in db.t_interaction_log
                                         where q1.questionnaire_id == TempList[i].id
                                         group q1 by q1.participate_id into h
                                         select new { Rec = h.Count() };
                            int n = query1.Count();
                            questionnaireAttend.Add(new QuestionnaireAttendStatus
                            {
                                ID = TempList[i].id,
                                Theme = TempList[i].theme,
                                QuestionStatus = "已结束",
                                StartTime = TempList[i].start_time,
                                EndTime = TempList[i].expiry_time,
                                Num = n,
                                AnswerFlag = AnswerFlag
                            });
                        }
                        return new { code = 200, result = questionnaireAttend, count = count, message = "ok" };
                    }
                    else
                        return new { code = 200, message = "ok" }; ;


                }
                else//全部
                {
                    List<QuestionnaireAttendStatus> questionnaireAttend = new List<QuestionnaireAttendStatus>();
                    var p = from q in db.t_questionnaire
                            where q.course_id == CourseID
                                 && q.delete_flag == 0
                                 && q.plan_id == PlanID
                                 && (string.IsNullOrEmpty(keyWord) ? true : q.theme.Contains(keyWord))
                            select q;
                    int count = p.Count();
                    var query = p.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
                    var TempList = query.ToList();
                    if (TempList.Count > 0)
                    {
                        for (int i = 0; i < TempList.Count; i++)
                        {
                            string AnswerFlag = "";
                            var queryExist = from g in db.t_interaction_log
                                             where g.questionnaire_id == TempList[i].id && g.participate_id == UserID
                                             select g;
                            var ExistCount = queryExist.Count();
                            if (ExistCount > 0)
                                AnswerFlag = "1";//已作答
                            else
                                AnswerFlag = "0";//未作答

                            string str = string.Empty;
                            if (TempList[i].start_time == null)
                            {
                                str = "未开始";
                                continue;
                            }
                            else if (TempList[i].start_time <= DateTime.Now && TempList[i].expiry_time == null)
                                str = "进行中";
                            else if (TempList[i].start_time != null && TempList[i].expiry_time != null)
                                str = "已结束";

                            var query1 = from q1 in db.t_interaction_log
                                         where q1.questionnaire_id == TempList[i].id
                                         group q1 by q1.participate_id into h
                                         select new { Rec = h.Count() };
                            int n = query1.Count();
                            questionnaireAttend.Add(new QuestionnaireAttendStatus
                            {
                                ID = TempList[i].id,
                                Theme = TempList[i].theme,
                                QuestionStatus = str,
                                StartTime = TempList[i].start_time,
                                EndTime = TempList[i].expiry_time,
                                Num = n,
                                AnswerFlag = AnswerFlag
                            });
                        }
                        return new { code = 200, result = questionnaireAttend, count = count, message = "ok" };
                    }
                    else
                        return new { code = 200, message = "ok" }; ;
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public async Task<int> QuestionnaireInteractLog(pf_questionnaireContext db, InteractionLogList ModelList)
        {
            for (int i = 0; i < ModelList.interactionLogList.Count; i++)
            {
                if (ModelList.interactionLogList[i].ItemOptionID != null && ModelList.interactionLogList[i].ItemOptionID.Count > 0)
                {
                    for (int j = 0; j < ModelList.interactionLogList[i].ItemOptionID.Count; j++)
                    {
                        t_interaction_log log = new t_interaction_log();
                        log.questionnaire_id = ModelList.interactionLogList[i].QuestionnaireID;
                        log.questionnaire_item_id = ModelList.interactionLogList[i].QuestionnaireItemID;
                        log.item_option_id = long.Parse(ModelList.interactionLogList[i].ItemOptionID[j]);
                        log.participate_id = ModelList.interactionLogList[i].ParticipateID;
                        log.participate_name = ModelList.interactionLogList[i].UserName;
                        log.interaction_result = ModelList.interactionLogList[i].InteractionResult;
                        log.interactive_time = DateTime.Now;
                        db.t_interaction_log.Add(log);
                    }
                }
            }
            return await db.SaveChangesAsync();
        }

        public object GetQuestionnaireAnswerResult(pf_questionnaireContext db, long QuestionnaireID, long UserID)
        {
            try
            {
                List<OptionContent> ContentList = new List<OptionContent>();
                List<QuestionItem> QuestionList = new List<QuestionItem>();
                Questionnaire questionnaire = new Questionnaire();

                var OptionContent = from q in db.t_questionnaire
                                    join i in db.t_questionnaire_item on q.id equals i.questionnnaire_id
                                    join p in db.t_item_option on i.id equals p.questionnaire_item_id into ii
                                    from _ii in ii.DefaultIfEmpty()
                                    where q.id == QuestionnaireID && _ii.delete_flag == 0
                                    select new
                                    {
                                        id = _ii == null ? 0 : _ii.id,
                                        item_id = _ii == null ? 0 : _ii.questionnaire_item_id,
                                        optionNum = _ii == null ? "null" : _ii.option_number,
                                        content = _ii == null ? "null" : _ii.option_content
                                    };
                var OptionContentListTemp = OptionContent.ToList();
                for (int i = 0; i < OptionContentListTemp.Count; i++)
                {
                    ContentList.Add(new OptionContent
                    {
                        ID = OptionContentListTemp[i].id,
                        ItemID = OptionContentListTemp[i].item_id,
                        OptionNum = OptionContentListTemp[i].optionNum,
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
                for (int i = 0; i < QuestionListTemp.Count; i++)
                {
                    long qid = QuestionListTemp[i].id;
                    List<string> answerList = new List<string>();
                    List<long> answerListID = new List<long>();
                    var queryAnswerResult = from a in db.t_interaction_log
                                            where a.questionnaire_id == QuestionnaireID //问卷ID
                                            && a.questionnaire_item_id == QuestionListTemp[i].id//选项ID
                                            && a.participate_id == UserID//用户ID
                                            select a;

                    //查找单选多选题答案
                    if (QuestionListTemp[i].itemtype != "3")
                    {
                        foreach (var item in queryAnswerResult)
                        {
                            answerListID.Add(item.item_option_id);
                        }
                    }
                    else//查找简答题答案
                    {
                        foreach (var item in queryAnswerResult)
                        {
                            answerList.Add(item.interaction_result);
                        }
                    }
                    //查找题干下的所有选项
                    var c = ContentList.FindAll(t => t.ItemID == qid);
                    //将选项添加到题干下
                    QuestionList.Add(new QuestionItem
                    {
                        ID = QuestionListTemp[i].id,
                        QuestionID = qid,
                        Item = QuestionListTemp[i].title,
                        ItemType = QuestionListTemp[i].itemtype,
                        OptionList = c,
                        Sort = QuestionListTemp[i].Sort,
                        MustAnswer = QuestionListTemp[i].must_answer_flag,
                        MinAnswerNum = QuestionListTemp[i].min_answer_num,
                        MaxAnswerNum = QuestionListTemp[i].max_answer_num,
                        AnswerResult = answerList,
                        AnswerListID = answerListID
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
    }

    public class QuestionnaireAttendStatus
    {
        public long ID { get; set; }
        public string Theme { get; set; }
        public string QuestionStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Num { get; set; }
        public string AnswerFlag { get; set; }
    }
    public class InteractionLogList
    {
        public List<InteractionLog> interactionLogList { get; set; }
    }
    public class InteractionLog
    {
        public long QuestionnaireID { get; set; }
        public long QuestionnaireItemID { get; set; }
        public List<string> ItemOptionID { get; set; }
        public long ParticipateID { get; set; }
        public string InteractionResult { get; set; }
        public string UserName { get; set; }
    }
}
