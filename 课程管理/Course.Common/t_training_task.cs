using System;
using System.Collections.Generic;


public partial class t_training_task
{
    public long id { get; set; }
    public long? src_id { get; set; }
    public string task_name { get; set; }
    public string knowledge_tag { get; set; }
    public string task_type { get; set; }
    public string type_level { get; set; }
    public string level { get; set; }
    public string airplane_type { get; set; }
    public string task_desc { get; set; }
    public int? course_count { get; set; }
    public DateTime? t_create { get; set; }
    public string create_number { get; set; }
    public DateTime? t_modified { get; set; }
    public sbyte? delete_flag { get; set; }
}

