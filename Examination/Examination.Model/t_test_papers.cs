using System;
using System.Collections.Generic;


    public partial class t_test_papers
    {
    public long id { get; set; }
    public long? examination_id { get; set; }
    public long? src_id { get; set; }
    public string paper_title { get; set; }
    public string paper_desc { get; set; }
    public string paper_confidential { get; set; }
    public int? exam_score { get; set; }
    public int? question_count { get; set; }
    public string approval_user_name { get; set; }
    public string approval_user_number { get; set; }
    public string approval_remarks { get; set; }
    public DateTime? approval_date { get; set; }
    public string approval_status { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

