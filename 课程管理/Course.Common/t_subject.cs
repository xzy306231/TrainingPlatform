using System;
using System.Collections.Generic;


public partial class t_subject
{
    public long id { get; set; }
    public long? task_id { get; set; }
    public string subject_number { get; set; }
    public string subject_name { get; set; }
    public string subject_desc { get; set; }
    public string subject_kind { get; set; }
    public string plane_type { get; set; }
    public string expect_result { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

