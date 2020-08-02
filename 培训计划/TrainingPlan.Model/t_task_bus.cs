using System;
using System.Collections.Generic;
using System.Text;

public partial class t_task_bus
{
    public long id { get; set; }
    public sbyte delete_flag { get; set; }
    public DateTime t_create { get; set; }
    public DateTime? t_modified { get; set; }
    public long original_id { get; set; }
    public long? plan_id { get; set; }
    public string task_name { get; set; }
    public string task_desc { get; set; }
    public string task_type_key { get; set; }
    public string task_type_value { get; set; }
    public string type_level_key { get; set; }
    public string type_level_value { get; set; }
    public string level_key { get; set; }
    public string level_value { get; set; }
    public string airplane_type_key { get; set; }
    public string airplane_type_value { get; set; }
    public int? class_hour { get; set; }
    public long? creator_id { get; set; }
    public string creator_name { get; set; }
    public string tag_display { get; set; }
}
