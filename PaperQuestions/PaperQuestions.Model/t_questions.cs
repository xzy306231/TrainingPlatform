using System;
using System.Collections.Generic;


public partial class t_questions
{
    public long id { get; set; }
    public string question_type { get; set; }
    public string complexity { get; set; }
    public string question_title { get; set; }
    public string question_desc { get; set; }
    public string question_answer { get; set; }
   // public decimal? question_score { get; set; }
    public string answer_analyze { get; set; }
    public string question_confidential { get; set; }
    public string publish_flag { get; set; }
    public int use_count { get; set; }
    public string approval_user_name { get; set; }
    public string approval_user_number { get; set; }
    public DateTime? approval_date { get; set; }
    public string approval_remarks { get; set; }
    public string approval_status { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public string create_name { get; set; }
    public string create_number { get; set; }
    public DateTime? t_modified { get; set; }
}

