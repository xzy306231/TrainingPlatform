using System;
using System.Collections.Generic;


public partial class t_training_task
{
    public long id { get; set; }
    public long? examination_id { get; set; }
    public long? src_id { get; set; }
    public string task_name { get; set; }
    public string task_type { get; set; }
    public string plane_type { get; set; }
    public string kind_level { get; set; }
    public string rank_level { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

