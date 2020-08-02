using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Examination.BLL
{
    public class TestPaperInfo
    {
        public static object TestPaperImportToDB(pf_examinationContext db, PaperInfomation paperInfomation, string approvalFlag, long examid, TokenModel token)
        {
            try
            {
                if (!string.IsNullOrEmpty(paperInfomation.PaperInfo.PaperTitle))
                {
                    //将试卷保存至数据库
                    t_test_papers paper = new t_test_papers();
                    paper.examination_id = examid;//考试管理ID
                    paper.paper_title = paperInfomation.PaperInfo.PaperTitle;
                    paper.exam_score = paperInfomation.PaperInfo.PaperScore;
                    paper.question_count = paperInfomation.PaperInfo.QuestionCount;

                    if (approvalFlag == "1")
                        paper.approval_status = "0";//不用审核
                    else if (approvalFlag == "0")
                        paper.approval_status = "2";//待审
                    else
                        paper.approval_status = "3";//审完

                    paper.paper_confidential = paperInfomation.PaperInfo.PaperConfidential;
                    db.t_test_papers.Add(paper);
                    db.SaveChanges();
                    //读出试卷最大值
                    long paperid = paper.id;

                    if (paperInfomation.PaperInfo.QuestionList != null && paperInfomation.PaperInfo.QuestionList.Count > 0)
                    {
                        List<Knowledge> tags = new List<Knowledge>();
                        //循环题干
                        for (int i = 0; i < paperInfomation.PaperInfo.QuestionList.Count; i++)
                        {
                            string strAnswer = "";
                            //保存题干数据
                            t_questions question = new t_questions();
                            question.test_paper_id = paperid;
                            question.question_type = paperInfomation.PaperInfo.QuestionList[i].QuestionType;
                            question.complexity = paperInfomation.PaperInfo.QuestionList[i].Complexity;
                            question.question_title = paperInfomation.PaperInfo.QuestionList[i].QuestionTitle;
                            question.answer_analyze = paperInfomation.PaperInfo.QuestionList[i].AnswerAnalyze;//考点分析
                            strAnswer = paperInfomation.PaperInfo.QuestionList[i].QuestionAnswer;
                            if (paperInfomation.PaperInfo.QuestionList[i].QuestionType == "2")//多选
                            {
                                List<char> list = strAnswer.ToList<char>();
                                list.Sort();
                                strAnswer = string.Join("", list.ToArray());
                            }
                            question.question_answer = strAnswer;
                            question.question_score = paperInfomation.PaperInfo.QuestionList[i].Score;
                            db.t_questions.Add(question);
                            db.SaveChanges();
                            //读出题干最大值
                            long questionid = question.id;

                            //添加统计题干数据
                            t_statistic_question q = new t_statistic_question();
                            q.exam_id = examid;
                            q.question_id = questionid;
                            db.t_statistic_question.Add(q);
                            db.SaveChanges();
                            long statistic_questionid = q.id;

                            //路径
                            if (paperInfomation.PaperInfo.QuestionList[i].FileInfoList != null && paperInfomation.PaperInfo.QuestionList[i].FileInfoList.Count > 0)
                            {
                                //循环文件路径
                                for (int j = 0; j < paperInfomation.PaperInfo.QuestionList[i].FileInfoList.Count; j++)
                                {
                                    t_file_path file = new t_file_path();
                                    file.src_id = questionid;
                                    file.dif = "1";
                                    file.group_name = paperInfomation.PaperInfo.QuestionList[i].FileInfoList[j].group_name;
                                    file.path = paperInfomation.PaperInfo.QuestionList[i].FileInfoList[j].path;
                                    file.file_name = paperInfomation.PaperInfo.QuestionList[i].FileInfoList[j].file_name;
                                    db.t_file_path.Add(file);
                                    db.SaveChanges();
                                }
                            }
                            //知识点
                            if (paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags != null && paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags.Count > 0)
                            {
                                //循环知识点
                                for (int j = 0; j < paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags.Count; j++)
                                {
                                    var queryTag = from t in db.t_knowledge_tag
                                                   where t.delete_flag == 0
                                                         && t.src_id == paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].ID
                                                   select t;
                                    if (queryTag.FirstOrDefault() != null)//存在副本知识点
                                    {
                                        queryTag.FirstOrDefault().tag = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].Tag;
                                        //建立关系
                                        t_question_knowledge_ref obj = new t_question_knowledge_ref();
                                        obj.question_id = questionid;
                                        obj.knowledge_tag_id = queryTag.FirstOrDefault().id;
                                        db.t_question_knowledge_ref.Add(obj);

                                        //添加知识点
                                        tags.Add(new Knowledge()
                                        {
                                            ExamID = examid,
                                            KnowledgeID = queryTag.FirstOrDefault().id,
                                            KnowledgeName = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].Tag
                                        });
                                    }
                                    else//不存在
                                    {
                                        //新建知识点
                                        t_knowledge_tag tag = new t_knowledge_tag();
                                        tag.src_id = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].ID;
                                        tag.tag = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].Tag;
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
                                        tags.Add(new Knowledge()
                                        {
                                            ExamID = examid,
                                            KnowledgeID = tagid,
                                            KnowledgeName = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].Tag
                                        });
                                    }

                                }

                            }
                            //选项
                            if (paperInfomation.PaperInfo.QuestionList[i].OptionInfoList != null && paperInfomation.PaperInfo.QuestionList[i].OptionInfoList.Count > 0)
                            {
                                //循环选项
                                for (int j = 0; j < paperInfomation.PaperInfo.QuestionList[i].OptionInfoList.Count; j++)
                                {
                                    //保存选项
                                    t_question_option option = new t_question_option();
                                    option.question_id = questionid;
                                    option.option_number = paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].OptionNum;
                                    option.option_content = paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].OptionContent;
                                    option.right_flag = paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].RightFlag;
                                    db.t_question_option.Add(option);
                                    db.SaveChanges();

                                    //读出题干最大值
                                    long optionid = option.id;

                                    //添加统计选项数据
                                    t_statistic_option p = new t_statistic_option();
                                    p.statistic_qid = statistic_questionid;
                                    p.option_id = optionid;
                                    db.t_statistic_option.Add(p);

                                    if (paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].FileInfoList != null && paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].FileInfoList.Count > 0)
                                    {
                                        for (int k = 0; k < paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].FileInfoList.Count; k++)
                                        {
                                            t_file_path file = new t_file_path();
                                            file.src_id = optionid;
                                            file.dif = "2";
                                            file.group_name = paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].FileInfoList[k].group_name;
                                            file.path = paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].FileInfoList[k].path;
                                            file.file_name = paperInfomation.PaperInfo.QuestionList[i].OptionInfoList[j].FileInfoList[k].file_name;
                                            db.t_file_path.Add(file);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }

                        AddKnowledge(db,tags);
                    }
                }
                db.SaveChanges();
                return new { code = 200, msg = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(Environment.CurrentDirectory) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        private static void AddKnowledge(pf_examinationContext db, List<Knowledge> knowledges)
        {
            try
            {
                if (knowledges != null && knowledges.Count > 0)
                {
                    //去重操作
                    knowledges = knowledges.GroupBy(k => k.KnowledgeID).Select(x => x.First()).ToList();

                    for (int i = 0; i < knowledges.Count; i++)
                    {
                        t_statistic_exam_knowledge k = new t_statistic_exam_knowledge();
                        k.exam_id = knowledges[i].ExamID;
                        k.know_id = knowledges[i].KnowledgeID;
                        k.know_name = knowledges[i].KnowledgeName;
                        db.t_statistic_exam_knowledge.Add(k);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(Environment.CurrentDirectory) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }
    }

    public class Knowledge
    {
        public long ExamID { get; set; }
        public long KnowledgeID { get; set; }
        public string KnowledgeName { get; set; }
    }
}
