using System;
using System.Collections.Generic;
using System.Text;


public class t_examination_manage
{
    public long id { get; set; }
    public long? src_id { get; set; }
    public string exam_name { get; set; }
    public string exam_div { get; set; }
    public int? exam_duration { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
    public sbyte? delete_flag { get; set; }
}

