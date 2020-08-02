using System;
using System.Collections.Generic;
using System.Text;


public class PaperInfomation
{
    public Paper PaperInfo { get; set; }
    public string userNumber { get; set; }
    public string userName { get; set; }
}
public class Paper
{
    public long ID { get; set; }
    public string PaperTitle { get; set; }
    public int? QuestionCount { get; set; }
    public string PaperConfidential { get; set; }
    public string ApprovalStatus { get; set; }
    public string ApprovalRemark { get; set; }
    public string EditFlag { get; set; }
    public int? ExamScore { get; set; }
    public int? PaperScore { get; set; }
    public List<Question> QuestionList { get; set; }
}
public class Question
{
    public long ID { get; set; }
    public string QuestionType { get; set; }
    public long? TestPaperID { get; set; }
    public string Complexity { get; set; }
    public string QuestionTitle { get; set; }
    public string QuestionAnswer { get; set; }
    public string AnswerAnalyze { get; set; }
    public string QuestionConfidential { get; set; }
    public int Score { get; set; }
    public List<KnowledgeTag> KnowledgeTags { get; set; }
    public List<OptionInfo> OptionInfoList { get; set; }
    public List<FileInfos> FileInfoList { get; set; }
}
public class OptionInfo
{
    public long ID { get; set; }
    public long? QuestionID { get; set; }
    public string OptionNum { get; set; }
    public string OptionContent { get; set; }
    public sbyte? RightFlag { get; set; }
    public List<FileInfos> FileInfoList { get; set; }
}

public class FileInfos
{
    public string group_name { get; set; }
    public string path { get; set; }
    public string file_name { get; set; }
}

public class KnowledgeTag
{
    public long? ID { get; set; }
    public string Tag { get; set; }
}

