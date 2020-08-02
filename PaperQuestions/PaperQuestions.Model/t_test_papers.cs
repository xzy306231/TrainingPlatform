using System;
using System.Collections.Generic;


public partial class t_test_papers
{
    public long id { get; set; }
    public string paper_title { get; set; }
    public string paper_desc { get; set; }
    public string paper_confidential { get; set; }
    public int? question_count { get; set; }
    public decimal? exam_score { get; set; }
    public sbyte? share_flag { get; set; }
    public string complexity { get; set; }
    public string approval_user_name { get; set; }
    public string approval_user_number { get; set; }
    public DateTime? approval_date { get; set; }
    public string approval_remarks { get; set; }
    public string approval_status { get; set; }
    public string create_name { get; set; }
    public string create_number { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

