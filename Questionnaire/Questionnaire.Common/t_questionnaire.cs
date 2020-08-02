using System;
using System.Collections.Generic;


public partial class t_questionnaire
{
    public long id { get; set; }
    public long? plan_id { get; set; }
    public long? course_id { get; set; }
    public string theme { get; set; }
    public string theme_desc { get; set; }
    public DateTime? start_time { get; set; }
    public DateTime? expiry_time { get; set; }
    public string current_status { get; set; }
    public sbyte? delete_flag { get; set; }
    public long? create_by { get; set; }
    public DateTime? create_time { get; set; }
    public long? update_by { get; set; }
    public DateTime? update_time { get; set; }
}

