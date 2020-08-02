using System;
using System.Collections.Generic;


public partial class t_statistic_texam
{
    public long id { get; set; }
    public long exam_id { get; set; }
    public int? total_num { get; set; }
    public int? exam_num { get; set; }
    public decimal? tscore { get; set; }
    public decimal? avg_rightrate { get; set; }
    public decimal? pass_rate { get; set; }
    public decimal? nopass_rate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

