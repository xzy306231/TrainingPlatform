using System;
using System.Collections.Generic;


public partial class t_course
{
    public long id { get; set; }
    public string course_name { get; set; }
    public string course_desc { get; set; }
    public decimal course_count { get; set; }
    public decimal learning_time { get; set; }
    public string thumbnail_path { get; set; }
    public sbyte? delete_flag { get; set; }
    public sbyte? publish_flag { get; set; }
    public DateTime? create_time { get; set; }
    public long? create_by { get; set; }
    public string user_name { get; set; }
    public string user_number { get; set; }
    public DateTime? update_time { get; set; }
    public long? update_by { get; set; }
    public string course_confidential { get; set; }
    public long? approval_user_id { get; set; }
    public string approval_user_number { get; set; }
    public string approval_user_name { get; set; }
    public DateTime? approval_date { get; set; }
    public string approval_remarks { get; set; }
    public string approval_status { get; set; }
}

