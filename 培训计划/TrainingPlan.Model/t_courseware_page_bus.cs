using System;
using System.Collections.Generic;
using System.Text;

public partial class t_courseware_page_bus
{
    public long id { get; set; }
    public long? courseware_resource_id { get; set; }
    public string page_script { get; set; }
    public int? page_sort { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}
