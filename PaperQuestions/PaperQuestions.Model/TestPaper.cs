using System;
using System.Collections.Generic;


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
    public string UserName { get; set; }
    public DateTime? CreateTime { get; set; }
    public sbyte? ShareFlag { get; set; }
    public int? QuestionCount { get; set; }
    public decimal? ExamScore { get; set; }
    public string PaperConfidential { get; set; }
    public string Complexity { get; set; }
    public string ApprovalStatus { get; set; }
    public string ApprovalUserName { get; set; }
    public string ApprovalRemark { get; set; }
    public DateTime? ApprovalDateTime { get; set; }
    public List<Question> QuestionList { get; set; }
}
public class Questionmation
{
    public List<Question> QuestionList { get; set; }
}
public class Question
{
    public long ID { get; set; }
    public long SrcID { get; set; }
    public long TestPaperID { get; set; }
    public long BasketID { get; set; }
    public int Number { get; set; }
    public string QuestionTitle { get; set; }
    public string QuestionType { get; set; }
    public string Complexity { get; set; }
    public string PublishFlag { get; set; }
    public string CreateTime { get; set; }
    public string CreateName { get; set; }
    public string AnswerAnalyze { get; set; }
    public string QuestionAnswer { get; set; }
    public decimal? QuestionScore { get; set; }
    public int QuestionSort { get; set; }
    public string QuestionConfidential { get; set; }
    public int QuestionTypeSort { get; set; }
    public int UseCount { get; set; }
    public string ApprovalStatus { get; set; }
    public string ApprovalRemark { get; set; }
    public string ApprovalUserName { get; set; }
    public DateTime? ApprovalDateTime { get; set; }
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
    public long ID { get; set; }
    public string Tag { get; set; }
}

