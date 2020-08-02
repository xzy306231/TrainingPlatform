using System;
using System.Collections.Generic;


public partial class t_course_struct
{
    public long id { get; set; }
    public long struct_id { get; set; }
    public long parent_id { get; set; }
    public long course_id { get; set; }
    public string course_node_name { get; set; }
    public string course_node_desc { get; set; }
    public string node_type { get; set; }
    public int node_sort { get; set; }
    public int? resource_count { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? create_time { get; set; }
    public long create_by { get; set; }
    public string create_name { get; set; }
    public DateTime? update_time { get; set; }
    public long? update_by { get; set; }
    public string update_name { get; set; }
}

