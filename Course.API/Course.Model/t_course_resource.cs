using System;
using System.Collections.Generic;


public partial class t_course_resource
{
    public long id { get; set; }
    public long? src_id { get; set; }
    public string resource_name { get; set; }
    public string resource_desc { get; set; }
    public string resource_type { get; set; }
    public string resource_extension { get; set; }
    public int? resource_time { get; set; }
    public string group_name { get; set; }
    public string resource_url { get; set; }
    public string resource_confidential { get; set; }
    public string thumbnail_path { get; set; }
    public sbyte? delete_flag { get; set; }
    public long? create_by { get; set; }
    public DateTime? create_time { get; set; }
    public long? update_by { get; set; }
    public DateTime? update_time { get; set; }
    public long? approval_user_id { get; set; }
    public DateTime? approval_date { get; set; }
    public string approval_remarks { get; set; }
    public string approval_status { get; set; }
}

