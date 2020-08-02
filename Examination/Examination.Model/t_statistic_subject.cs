using System;
using System.Collections.Generic;
using System.Text;


public class t_statistic_subject
{
    public long id { get; set; }
    public long subject_id { get; set; }
    public long exam_id { get; set; }
    public decimal? right_rate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
    public int? pass_nums { get; set; }
    public int? nopass_nums { get; set; }
    public long? task_id { get; set; }
}

