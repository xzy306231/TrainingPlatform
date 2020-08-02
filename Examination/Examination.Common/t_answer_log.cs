using System;
using System.Collections.Generic;


public partial class t_answer_log
{
    public long id { get; set; }
    public long? record_id { get; set; }
    public long? item_id { get; set; }
    public long? option_id { get; set; }
    public string answer_result { get; set; }
    public int? score { get; set; }
    public string correct_flag { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

