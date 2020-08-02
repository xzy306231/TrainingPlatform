using System;
using System.Collections.Generic;


public partial class t_struct_resource
{
    public long id { get; set; }
    public long? course_struct_id { get; set; }
    public long? course_resouce_id { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

