using System;
using System.Collections.Generic;


public partial class t_index_setting
{
    public long id { get; set; }
    public string index_dif { get; set; }
    public string index_kind { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

