using System;
using System.Collections.Generic;


public partial class t_trainingplan_stu
{
    public long id { get; set; }
    public long? trainingplan_id { get; set; }
    public long? user_id { get; set; }
    public string uesr_number { get; set; }
    public string user_name { get; set; }
    public string education { get; set; }
    public string department { get; set; }
    public string airplane { get; set; }
    public string skill_level { get; set; }
    public string actual_duration { get; set; }
    public string fly_status { get; set; }
    public string photo_path { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? create_time { get; set; }
    public long? create_by { get; set; }
    public DateTime? update_time { get; set; }
}

