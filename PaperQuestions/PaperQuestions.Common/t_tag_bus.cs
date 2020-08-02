using System;
using System.Collections.Generic;
using System.Text;

public partial class t_tag_bus
{
    public long id { get; set; }
    public long? src_id { get; set; }
    public string tag { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}
