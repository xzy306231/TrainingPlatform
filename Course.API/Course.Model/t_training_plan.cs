using System;
using System.Collections.Generic;


public partial class t_training_plan
{
    public long id { get; set; }
    public string plan_name { get; set; }
    public string plan_desc { get; set; }
    public string plan_status { get; set; }
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public sbyte? publish_flag { get; set; }

    public sbyte? course_flag { get; set; }
    public sbyte? task_flag { get; set; }
    public sbyte? exam_flag { get; set; }
    public sbyte? quit_flag { get; set; }
    public decimal finish_rate { get; set; }
    public int stu_count { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? create_time { get; set; }
    public long create_by { get; set; }
    public string create_number { get; set; }
    public DateTime? update_time { get; set; }
}

