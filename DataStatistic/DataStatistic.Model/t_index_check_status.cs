using System;
using System.Collections.Generic;


public partial class t_index_check_status
{
    public long id { get; set; }
    public long? index_id { get; set; }
    public sbyte? check_flag { get; set; }
    public long? plan_id { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

