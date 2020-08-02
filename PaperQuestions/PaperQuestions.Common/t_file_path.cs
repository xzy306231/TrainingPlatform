using System;
using System.Collections.Generic;


public partial class t_file_path
{
    public long id { get; set; }
    public long? src_id { get; set; }
    public string dif { get; set; }
    public string group_name { get; set; }
    public string path { get; set; }
    public string file_name { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

