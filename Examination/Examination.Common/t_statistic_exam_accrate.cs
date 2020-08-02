using System;
using System.Collections.Generic;
using System.Text;


public class t_statistic_exam_accrate
{
    public long id { get; set; }
    public long exam_id { get; set; }
    public string acc_name { get; set; }
    public sbyte? acc_index { get; set; }
    public string exam_num { get; set; }
    public decimal? acc_rate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

