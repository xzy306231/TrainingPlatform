using System;
using System.Collections.Generic;


public partial class t_learning_record
{
    public long id { get; set; }
    public long? content_id { get; set; }
    public string user_number { get; set; }
    public string learning_progress { get; set; }
    public int? learning_sum_time { get; set; }
    public DateTime? learning_time { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? create_time { get; set; }
    public DateTime? update_time { get; set; }
}

