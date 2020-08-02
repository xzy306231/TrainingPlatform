using System;
using System.Collections.Generic;


public partial class t_classroom
{
    public long id { get; set; }
    public string room_number { get; set; }
    public string room_name { get; set; }
    public string room_type { get; set; }
    public int? room_capacity { get; set; }
    public string room_function { get; set; }
    public string room_status { get; set; }
    public DateTime? t_create { get; set; }
    public string create_number { get; set; }
    public DateTime? t_modified { get; set; }
    public sbyte? delete_flag { get; set; }
}

