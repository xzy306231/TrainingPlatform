using System;
public partial class t_courseware
{
    public long id { get; set; }
    public string courseware_title { get; set; }
    public string courseware_desc { get; set; }
    public string thumbnail_path { get; set; }
    public string resource_confidential { get; set; }
    public int file_size { get; set; }
    public sbyte? cooperation_flag { get; set; }
    public sbyte? publish_flag { get; set; }
    public long create_id { get; set; }
    public string create_name { get; set; }
    public string create_number { get; set; }
    public string update_number { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_modified { get; set; }
    public DateTime? t_create { get; set; }
}

