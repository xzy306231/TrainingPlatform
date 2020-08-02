using System;

public partial class t_subject_bus
{
    public sbyte delete_flag { get; set; }
    public DateTime t_create { get; set; }
    public DateTime? t_modified { get; set; }
    public long id { get; set; }
    public string number { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string classify_key { get; set; }
    public string classify_value { get; set; }
    public string plane_type_key { get; set; }
    public string plane_type_value { get; set; }
    public string expect_result { get; set; }
    public string creator_name { get; set; }
    public long creator_id { get; set; }
    public int version { get; set; }
    public string tag_display { get; set; }
    public long original_id { get; set; }
    public long task_bus_id { get; set; }
}

