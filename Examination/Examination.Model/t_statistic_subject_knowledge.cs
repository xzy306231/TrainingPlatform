using System;
using System.Collections.Generic;
using System.Text;


public class t_statistic_subject_knowledge
{
    public long id { get; set; }
    public long exam_id { get; set; }
    public long know_id { get; set; }
    public string know_name { get; set; }
    public decimal? know_rate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

