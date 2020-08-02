using System;
using System.Collections.Generic;
using System.Text;


public class t_task_log
{
    public long id { get; set; }
    public long? record_id { get; set; }
    public long? task_id { get; set; }
    public long? subject_id { get; set; }
    public string exam_result { get; set; }
    public string do_flag { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

