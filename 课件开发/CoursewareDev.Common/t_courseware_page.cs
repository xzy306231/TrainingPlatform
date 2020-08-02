using System;
public partial class t_courseware_page
{
    public long id { get; set; }
    public long? courseware_id { get; set; }
    public string page_script { get; set; }
    public int? page_sort { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

