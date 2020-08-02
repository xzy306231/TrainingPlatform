using System;
using System.Collections.Generic;


public partial class t_statistic_trainingtime
{
    public long id { get; set; }
    public string t_year { get; set; }
    public string t_month { get; set; }
    public int training_num { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

