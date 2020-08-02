using System;
using System.Collections.Generic;
using System.Text;


public class ExamUserInfo
{
    public string UserNumber { get; set; }
    public string UserName { get; set; }
    public string Department { get; set; }
}
public class ExamUserModel
{
    public long? PlanID { get; set; }
    public List<long?> ExaminationListID { get; set; }
    public List<ExamUserInfo> userInfos { get; set; }
    public string userNumber { get; set; }
    public string userName { get; set; }
}

