using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PaperQuestions.BLL
{
    public class PaperQuestion
    {
        public object TestPaperImportToDB(pf_exam_paper_questionsContext db,RabbitMQClient rabbit, PaperInfomation paperInfomation)
        {
            try
            {
                if (!string.IsNullOrEmpty(paperInfomation.PaperInfo.PaperTitle))
                {
                    //将试卷保存至数据库
                    t_test_papers paper = new t_test_papers();
                    paper.paper_title = paperInfomation.PaperInfo.PaperTitle;
                    paper.exam_score = paperInfomation.PaperInfo.ExamScore;
                    db.t_test_papers.Add(paper);
                    db.SaveChanges();
                    //读出试卷最大值
                    long paperid = paper.id;

                    if (paperInfomation.PaperInfo.QuestionList != null && paperInfomation.PaperInfo.QuestionList.Count > 0)
                    {
                        //循环题干
                        for (int i = 0; i < paperInfomation.PaperInfo.QuestionList.Count; i++)
                        {
                            string strAnswer = "";
                            //保存题干数据
                            t_questions question = new t_questions();
                            question.question_type = paperInfomation.PaperInfo.QuestionList[i].QuestionType;
                            question.complexity = paperInfomation.PaperInfo.QuestionList[i].Complexity;
                            question.question_title = paperInfomation.PaperInfo.QuestionList[i].QuestionTitle;
                            strAnswer = paperInfomation.PaperInfo.QuestionList[i].QuestionAnswer;
                            if (paperInfomation.PaperInfo.QuestionList[i].QuestionType == "2")//多选
                            {
                                List<char> list = strAnswer.ToList<char>();
                                list.Sort();
                                strAnswer = string.Join("", list.ToArray());
                            }
                            question.question_answer = strAnswer;
                            db.t_questions.Add(question);
                            db.SaveChanges();
                            //读出题干最大值
                            long questionid = question.id;

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
                                        tag.src_id = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].ID;
                                        tag.tag = paperInfomation.PaperInfo.QuestionList[i].KnowledgeTags[j].Tag;
                                        db.t_knowledge_tag.Add(tag);
                                        db.SaveChanges();
                                        //读出知识点最大值
                                        //long tagid = (from t in db.t_knowledge_tag select t.id).Max();
                                        long tagid = tag.id;

                                        //建立关系
                                        t_question_knowledge_ref obj = new t_question_knowledge_ref();
                                        obj.question_id = questionid;
                                        obj.knowledge_tag_id = tagid;
                                        db.t_question_knowledge_ref.Add(obj);
                                    }
                                }
                            }

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
                    }
                }
                SysLogModel syslog = new SysLogModel();
                syslog.opNo = paperInfomation.userNumber;
                syslog.opName = paperInfomation.userName;
                syslog.opType = 2;
                syslog.moduleName = "试卷库";
                syslog.logDesc = "导入了试卷：" + paperInfomation.PaperInfo;
                syslog.logSuccessd = 1;
                syslog.moduleName = "试卷管理";
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
