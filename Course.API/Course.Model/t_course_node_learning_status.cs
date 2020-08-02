using System;
using System.Collections.Generic;


public partial class t_course_node_learning_status
{
    public long id { get; set; }
    public long? record_id { get; set; }
    public long src_id { get; set; }
    public long course_struct_id { get; set; }
    public string node_status { get; set; }
    public int? learning_time { get; set; }
    public int? attempt_number { get; set; }
    public DateTime? last_learning_time { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? create_time { get; set; }
    public DateTime? update_time { get; set; }
}

