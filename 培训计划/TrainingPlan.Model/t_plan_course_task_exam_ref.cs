using System;
using System.Collections.Generic;


public partial class t_plan_course_task_exam_ref
{
    public long id { get; set; }
    public long? plan_id { get; set; }
    public long? content_id { get; set; }
    public int? content_sort { get; set; }
    public string teacher_num { get; set; }
    public string teacher_name { get; set; }
    public decimal finish_rate { get; set; }
    public int? avg_learningtime { get; set; }
    public string dif { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? create_time { get; set; }
    public long? create_by { get; set; }
    public DateTime? update_time { get; set; }
}

