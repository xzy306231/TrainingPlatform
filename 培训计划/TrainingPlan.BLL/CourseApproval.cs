using System.Collections.Generic;

namespace Course.BLL
{
    public class CourseApproval
    {


    }

    public class ApprovalResult
    {
        public List<long> list { get; set; }
        public long UserID { get; set; }
        public string ApprovalCode { get; set; }
        public string approvalUserNumber { get; set; }
    }

    public class Approval
    {
        public long courseID { get; set; }
        public string approvalResult { get; set; }
        public long approvalUserID { get; set; }
        public string approvalUserNumber { get; set; }
        public string approvalRemark { get; set; }
    }

    public class Query
    {
        public string CourseName { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string FieldName { get; set; }
        public bool IsAsc { get; set; }

    }
}
