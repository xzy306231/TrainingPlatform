using System;
using System.Collections.Generic;
using System.Text;


public class TaskSubject
{
    public string code { get; set; }
    public string message { get; set; }
    public Result result { get; set; }
}
public class Result
{
    public long ID { get; set; }
    public string TaskName { get; set; }
    public string TaskDesc { get; set; }
    public List<Subjects> SubjectRefEntities { get; set; }
}

public class Subjects
{
    public Subject subject { get; set; }

}
public class Subject
{
    public long OriginalId { get; set; }
    public string SubjectNumb { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public string ClassifyValue { get; set; }
    public string AirplaneValue { get; set; }
    public string ExpectedResult { get; set; }
    public List<Tags> TagRefEntities { get; set; }
}
public class Tags
{
    public Tag tag { get; set; }
}
public class Tag
{
    public long TagId { get; set; }
    public string TagName { get; set; }
}

