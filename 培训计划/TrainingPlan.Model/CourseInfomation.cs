using System;
using System.Collections.Generic;

public class ProgramCourseTask
{
    public long ID { get; set; }
    public string RangePurpose { get; set; }
    public string StandardRequest { get; set; }
    public string PlaneType { get; set; }
    public string TrainType { get; set; }
    public string ProgramName { get; set; }
    public string EndorsementType { get; set; }
    public string PlaneType1 { get; set; }
    public int? SumDuration { get; set; }
    public int? TrainTime { get; set; }
    public int? UpDownTimes { get; set; }
    public string TechnicalGrade { get; set; }
    public string OtherRemark { get; set; }
    public List<TrainingTaskInfomation> trainingTaskInfomation { get; set; }
    public List<CourseInfomation> courseInfomation { get; set; }
}
public class TrainingTaskInfomation
{
    public long ID { get; set; }
    public string TaskName { get; set; }
    public string TaskDesc { get; set; }
    public string Tag { get; set; }
    public int? CourseCount { get; set; }
    public string TaskType { get; set; }
    public string TypeLevel { get; set; }
    public string Level { get; set; }
    public string AirplaneType { get; set; }
    public List<SubjectInfo> SubjectList { get; set; }
}
public class SubjectInfo
{
    public long ID { get; set; }
    public string TrainName { get; set; }
    public string TrainDesc { get; set; }
    public string TrainNumber { get; set; }
    public string TrainKind { get; set; }
    public string PlaneType { get; set; }
    public string ExpectResult { get; set; }
    public long CreateBy { get; set; }
}
public class CourseInfomation
{
    public long ID { get; set; }
    public string CourseName { get; set; }
    public string CourseDesc { get; set; }
    public decimal CourseCount { get; set; }
    public decimal LearningTime { get; set; }
    public string CourseConfidential { get; set; }
    public string ThumbnailPath { get; set; }
    public List<CourseStructInfomation> courseStructsList { get; set; }
    public List<KnowledgeTag> knowledgeTagsList { get; set; }
}
public class CourseStructInfomation
{
    public long StructID { get; set; }
    public long ParentID { get; set; }
    public string CourseNodeName { get; set; }
    public string NodeType { get; set; }
    public int? ResourceCount { get; set; }
    public DateTime? CreateTime { get; set; }
    public CourseResourceInfomation resourceInfomation { get; set; }
}
public class CourseResourceInfomation
{
    public string ResourceName { get; set; }
    public string ResourceDesc { get; set; }
    public string ResourceConfidential { get; set; }
    public string ResourceType { get; set; }
    public string ResourceExtension { get; set; }
    public string GroupName { get; set; }
    public string ResourceUrl { get; set; }
    public List<CoursewarePage> PageList { get; set; }
}
public class CoursewarePage
{
    public int Sort { get; set; }
    public string PageScript { get; set; }
}
public class KnowledgeTag
{
    public long ID { get; set; }
    public string Tag { get; set; }
}


