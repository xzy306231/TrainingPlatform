using System;
using System.Collections.Generic;
using System.Text;


public partial class t_question_statistic
{
    public long id { get; set; }
    public long? questionid { get; set; }
    public int? use_count { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

