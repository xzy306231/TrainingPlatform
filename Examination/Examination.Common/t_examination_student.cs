using System;
using System.Collections.Generic;


public partial class t_examination_student
{
    public long id { get; set; }
    public long? plan_id { get; set; }
    public long? examination_id { get; set; }
    public string user_number { get; set; }
    public string uesr_name { get; set; }
    public string department { get; set; }
    public DateTime? t_create { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_modified { get; set; }
}

