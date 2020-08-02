using System;
using System.Collections.Generic;


public partial class t_examination_record
{
    public long id { get; set; }
    public long? plan_id { get; set; }
    public long? examination_id { get; set; }
    public string user_number { get; set; }
    public string user_name { get; set; }
    public string department { get; set; }
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public float? score { get; set; }
    public string record_status { get; set; }
    public string task_comment { get; set; }
    public sbyte? pass_flag { get; set; }
    public decimal? pass_rate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

