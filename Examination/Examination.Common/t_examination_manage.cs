using System;
using System.Collections.Generic;


public partial class t_examination_manage
{
    public long id { get; set; }
    public string exam_name { get; set; }
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public string exam_div { get; set; }
    public string exam_status { get; set; }
    public int? exam_duration { get; set; }
    public string exam_explain { get; set; }
    public int? pass_scores { get; set; }
    public string correct_status { get; set; }
    public string approval_status { get; set; }
    public string paper_confidential { get; set; }
    public sbyte? used_flag { get; set; }
    public long? content_id { get; set; }
    public sbyte? publish_flag { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public string create_num { get; set; }
    public string create_name { get; set; }
    public DateTime? t_modified { get; set; }
}

