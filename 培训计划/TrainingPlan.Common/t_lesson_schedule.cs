using System;
using System.Collections.Generic;


public partial class t_lesson_schedule
{
    public long id { get; set; }
    public long? classroom_id { get; set; }
    public DateTime schedule_datetime { get; set; }
    public long? plan_id { get; set; }
    public long? content_id { get; set; }
    public string content_name { get; set; }
    public string content_div { get; set; }
    public string create_number { get; set; }
    public string create_name { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public string update_number { get; set; }
    public DateTime? t_modified { get; set; }
}

