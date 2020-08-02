using System;
using System.Collections.Generic;


public partial class t_statistic_option
{
    public long id { get; set; }
    public long statistic_qid { get; set; }
    public long option_id { get; set; }
    public decimal? select_rate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
    public int? select_nums { get; set; }
}

